using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;
using IBM.Data.DB2.iSeries;
using IGServices.Common.Managers.E1;
using IGServices.Common.Managers.Enterprise;
using IGServices.Common.Types.E1;
using IGServices.Common.Types.Enterprise;
using IGServices.Common.Types.Response;
using IGServices.Common.Types.ShopFloor;
using IGServices.Common.Utilities;
using NLog;
using FlexItemManager = IGServices.Common.Managers.Enterprise.FlexItemManager;

namespace IGServices.Common.Managers.Process
{
    public class SpecUnitMtoDownloadManager
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly E1.ItemAttributeManager _itemAttMgr;
        //private readonly SpecUnitManager _specUnitManager;
        private readonly FlexItemManager _flexMgr;

        public SpecUnitMtoDownloadManager(BusinessSystemSession session)
        {
            var hash = new Dictionary<string, iDB2Command>
            {
                //{"SpecUnitMtoDownloadManager.GetRelatedOrders", GetRelatedOrders()}
            };
            foreach (var keyName in hash.Keys.Where(keyName => !session.DbCommands.ContainsKey(keyName)))
            {
                session.DbCommands.Add(keyName, hash[keyName]);
            }
            _itemAttMgr = new E1.ItemAttributeManager(session);
            _flexMgr = new FlexItemManager(session);
            //_specUnitManager = new SpecUnitManager(session);
        }

        public ProcessResponse Process(BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Plant plant, TimeSpan timeSpan, bool useHighWaterMark, List<string> documentTypes, decimal documentNum)
        {
            var response = new ProcessResponse("MTO Spec Unit Download Manager");
            response.Plant = plant;
            response.ConnectionStrings.Add(new ConnectionString("E1", session.DbConnection.ConnectionString));
            foreach (var sqlCn in sqlCns)
            {
                response.ConnectionStrings.Add(new ConnectionString(sqlCn.Key, sqlCn.Value.ConnectionString));
            }

            ProcessRunSpan processRunSpan = new ProcessRunSpan();
            HighWaterMark highWaterMark = new HighWaterMark();

            /********************************************************/
            var step1 = new ProcessStep("Resolve Run Span");
            try
            {
                highWaterMark = (useHighWaterMark) ? HighWaterMarkManager.GetHighWaterMark(sqlCns["PLANT.SHOPFLOOR"], plant, "SpecUnitMTO") : null;
                processRunSpan = ProcessRunSpanManager.GetRunSpan(highWaterMark, timeSpan,
                    E1TimeManager.GetE1TimeStamp(session));
                step1.Messages.Add(new Message("RunSpan", processRunSpan.Serialize()));
            }
            catch (Exception e)
            {
                step1.Errors.Add(new Error(e));
            }
            finally
            {
                step1.Done();
                response.AddStep(step1);
            }

            var specUnitDocuments = new SpecUnitDocuments();
            if (!response.HasErrors())
            {
                /********************************************************/
                var step2 = new ProcessStep("Get Spec Unit Documents");
                try
                {
                    specUnitDocuments = GetSpecUnitDocuments(session, plant, processRunSpan, documentTypes, documentNum);
                }
                catch (Exception e)
                {
                    step2.Errors.Add(new Error(e));
                }
                finally
                {
                    step2.RowsAffected = specUnitDocuments.Count;
                    step2.Done();
                    response.ProcessSteps.Add(step2);
                }
            }

            var specUnitItems = new SpecUnitItems();
            if (!response.HasErrors() && specUnitDocuments.Count > 0)
            {
                var step2a = new ProcessStep("Get Spec Unit Items");
                try
                {
                    var query = specUnitDocuments.GroupBy(x => x.ConfigurationId)
                        .Select(group =>
                            new
                            {
                                Name = group.Key,
                                Record = group.OrderByDescending(x => x.TransactionDate).FirstOrDefault()
                            });

                    foreach (var group in query)
                    {
                        var specUnitItem = new SpecUnitItem();
                        specUnitItem.ConfigId = group.Record.ConfigurationId;
                        specUnitItem.ItemNum = group.Record.ItemNum;
                        specUnitItem.UnitType = group.Record.UnitType;
                        specUnitItem.StockType = group.Record.StockType;
                        specUnitItem.TransactionDate = group.Record.TransactionDate;
                        specUnitItems.Add(specUnitItem);
                    }
                }
                catch (Exception e)
                {
                    step2a.Errors.Add(new Error(e));
                }
                finally
                {
                    step2a.RowsAffected = specUnitItems.Count;
                    step2a.Done();
                    response.ProcessSteps.Add(step2a);
                }
            }

            /********************************************************/
            if (!response.HasErrors() && specUnitDocuments.Count > 0)
            {
                var step3 = new ProcessStep("Assign Spec Unit");
                try
                {
                    int j = 0;
                    foreach (var specUnitItem in specUnitItems)
                    {
                        j++;
                        if (j % 1000 == 0)
                        {
                            _logger.Info($"Attributes {j}");
                        }

                        var itemAtts = _itemAttMgr.GetConfiguredItemAttributes(session, specUnitItem.ConfigId);
                        specUnitItem.ItemAttributes = itemAtts;
                    }
                }
                catch (Exception e)
                {
                    step3.Errors.Add(new Error(e));
                }
                finally
                {
                    step3.RowsAffected = specUnitItems.Count;
                    step3.Done();
                    response.ProcessSteps.Add(step3);
                }
            }

            /********************************************************/
            var specUnitTable = new SpecUnitTable();
            if (!response.HasErrors() && specUnitItems.Count > 0)
            {
                var step4 = new ProcessStep("Build SpecUnit Data Table");
                try
                {
                    specUnitTable.Add(specUnitItems);
                }
                catch (Exception e)
                {
                    step4.Errors.Add(new Error(e));
                }
                finally
                {
                    step4.RowsAffected = specUnitTable.Rows.Count;
                    step4.Done();
                    response.ProcessSteps.Add(step4);
                }
            }

            /********************************************************/
            var specUnitDocumentTable = new SpecUnitDocumentTable();
            if (!response.HasErrors() && specUnitDocuments.Count > 0)
            {
                var step4a = new ProcessStep("Build SpecUnitDocument Data Table");
                try
                {
                    specUnitDocumentTable.Add(specUnitDocuments);
                }
                catch (Exception e)
                {
                    step4a.Errors.Add(new Error(e));
                }
                finally
                {
                    step4a.RowsAffected = specUnitDocumentTable.Rows.Count;
                    step4a.Done();
                    response.ProcessSteps.Add(step4a);
                }
            }

            /********************************************************/
            if (!response.HasErrors() && specUnitDocumentTable.Rows.Count > 0)
            {
                var step5 = new ProcessStep("Bulk Copy SpecUnitDocument");
                SqlBulkCopyResponse bulkResponse = new SqlBulkCopyResponse();
                try
                {
                    if (specUnitDocumentTable.Rows.Count > 0)
                    {
                        bulkResponse =
                            BulkCopyManager.BulkTable(specUnitDocumentTable.ExtendedProperties["Target"].ToString(),
                                specUnitDocumentTable, sqlCns["PLANT.INTEGRATION"]);
                    }
                }
                catch (Exception e)
                {
                    step5.Errors.Add(new Error(e));
                }
                finally
                {
                    step5.RowsAffected = (int)bulkResponse.RowsCopied;
                    step5.SqlBulkCopyResponses.Add(bulkResponse);
                    step5.Done();
                    response.RowsAffected = step5.RowsAffected;
                    response.AddStep(step5);
                }
            }

            /********************************************************/
            if (!response.HasErrors() && specUnitTable.Rows.Count > 0)
            {
                var step5a = new ProcessStep("Bulk Copy SpecUnit");
                SqlBulkCopyResponse bulkResponse = new SqlBulkCopyResponse();
                try
                {
                    if (specUnitTable.Rows.Count > 0)
                    {
                        bulkResponse =
                            BulkCopyManager.BulkTable(specUnitTable.ExtendedProperties["Target"].ToString(),
                                specUnitTable, sqlCns["PLANT.INTEGRATION"]);
                    }
                }
                catch (Exception e)
                {
                    step5a.Errors.Add(new Error(e));
                }
                finally
                {
                    step5a.RowsAffected = (int)bulkResponse.RowsCopied;
                    step5a.SqlBulkCopyResponses.Add(bulkResponse);
                    step5a.Done();
                    response.RowsAffected = step5a.RowsAffected;
                    response.AddStep(step5a);
                }
            }

            /********************************************************/
            if (!response.HasErrors() && useHighWaterMark)
            {
                var step6 = new ProcessStep("Update High Water Mark");
                try
                {
                    if (specUnitDocumentTable.Rows.Count > 0)
                    {
                        highWaterMark.LocalDateTime = specUnitDocumentTable.MaxDate.AddHours(-plant.TimeOffset);
                        highWaterMark.EnterpriseDateTime = specUnitDocumentTable.MaxDate;
                    }

                    step6.SqlDmlResponses.Add(
                        HighWaterMarkManager.SaveHighWaterMark(sqlCns["PLANT.SHOPFLOOR"], highWaterMark));
                }
                catch (Exception e)
                {
                    step6.Errors.Add(new Error(e));
                }
                finally
                {
                    step6.Done();
                    response.AddStep(step6);
                }
            }

            response.Done();
            Logging.InsertProcessLog(response);
            return response;
        }

        /******************************************************************
         * Item Attributes
         ******************************************************************/
        public iDB2Command GetSpecUnitDocuments(List<string> filters)
        {
            var filterString = new StringBuilder();
            foreach (string filter in filters)
            {
                filterString.Append(filter);
            }

            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = $@"
            SELECT * FROM (
            SELECT
	            CMCFGID,
                CMKCOO,
                CMDOCO,
                CMDCTO,
                CMLNID,
	            CMUPMJ,
	            CMTDAY,
	            CCITM,
	            IBSRP4,
	            IBSTKT,
	            ROW_NUMBER() OVER (PARTITION BY CMDOCO, CMDCTO, CMLNID, CMCFGID ORDER BY CMUPMJ DESC, CMTDAY DESC) AS RN
            FROM 
	            F3201
	            INNER JOIN F3215
	            ON  CCCFGID = CMCFGID AND
		            CCCFGCID = 1
	            INNER JOIN F4102
	            ON  IBMCU = CCMCU AND
		            IBITM = CCITM
            WHERE 
                {filterString}
                CMDOCO > 0 AND
                CMPID <> 'P580150') A
            WHERE 
                A.RN = 1
            FOR READ ONLY";
            return cmd;
        }

        public SpecUnitDocuments GetSpecUnitDocuments(BusinessSystemSession session, Plant plant, ProcessRunSpan span, List<string> documentTypes, decimal documentNum)
        {
            var specUnitDocuments = new SpecUnitDocuments();
            var cmd = new iDB2Command();
            List<string> filters = new List<string>();
            List<iDB2Parameter> parameters = new List<iDB2Parameter>();
            try
            {
                // Plant
                if (!String.IsNullOrEmpty(plant.PlantCode))
                {
                    filters.Add("CMEMCU = ? AND\r\n");
                    parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
                    parameters.Last().Value = plant.PlantNum.PadLeft(12);
                }

                // Run Span
                if (!span.IsEmpty)
                {
                    var queryString = E1TimeManager.GetE1DateTimeQueryString("CMUPMJ", "CMTDAY", span.E1TimeIdSpan);
                    filters.Add(queryString + " AND \r\n");
                }

                // Document Num
                if (documentNum > 0)
                {
                    filters.Add("CMDOCO = ? AND \r\n");
                    parameters.Add(new iDB2Parameter("DOCO", iDB2DbType.iDB2Decimal));
                    parameters.Last().Precision = 8;
                    parameters.Last().Scale = 0;
                    parameters.Last().Value = documentNum;
                }

                // Document Types
                if (documentTypes.Any())
                {
                    string strInClause = "";
                    foreach (string documentType in documentTypes)
                    {
                        strInClause = $"{strInClause} '{documentType}',";
                    }
                    filters.Add("CMDCTO IN (" + strInClause.Substring(0, strInClause.Length - 1) + ") AND \r\n");
                }

                cmd = GetSpecUnitDocuments(filters);
                foreach (iDB2Parameter parameter in parameters)
                {
                    cmd.Parameters.Add(parameter);
                }
                cmd.Connection = session.DbConnection;
                _logger.Info(Logging.DbCmdString(cmd));

                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
                while (reader.Read())
                {
                    var specUnitDocument = new SpecUnitDocument();
                    specUnitDocument.ConfigurationId = Data.CastDbNull.To<decimal>(reader["CMCFGID"], 0);
                    specUnitDocument.ItemNum = Data.CastDbNull.To<decimal>(reader["CCITM"], 0);
                    specUnitDocument.UnitType = Data.CastDbNull.To<string>(reader["IBSRP4"], String.Empty).Trim();
                    specUnitDocument.StockType = Data.CastDbNull.To<string>(reader["IBSTKT"], String.Empty).Trim();
                    specUnitDocument.DocumentNum = Data.CastDbNull.To<decimal>(reader["CMDOCO"], 0);
                    specUnitDocument.DocumentType = Data.CastDbNull.To<string>(reader["CMDCTO"], String.Empty).Trim();
                    specUnitDocument.DocumentLineNum = Data.CastDbNull.To<decimal>(reader["CMLNID"], 0) / 1000;
                    specUnitDocument.TransactionDate = E1TimeManager.CastToDateTime(
                        Data.CastDbNull.To<decimal>(reader["CMUPMJ"], 0),
                        Data.CastDbNull.To<decimal>(reader["CMTDAY"], 0));
                    specUnitDocuments.Add(specUnitDocument);
                }
                reader.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return specUnitDocuments;
        }

    }
}
