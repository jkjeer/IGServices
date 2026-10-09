using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;
using IBM.Data.DB2.iSeries;
using IGServices.Common.Managers.E1;
using IGServices.Common.Managers.Enterprise;
using IGServices.Common.Managers.ShopFloor;
using IGServices.Common.Types.E1;
using IGServices.Common.Types.Enterprise;
using IGServices.Common.Types.Response;
using IGServices.Common.Types.ShopFloor;
using IGServices.Common.Utilities;
using NLog;

namespace IGServices.Common.Managers.Process
{
    public class SpecUnitMtsDownloadManager
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly E1.ItemAttributeManager _itemAttMgr;
        private readonly IGServices.Common.Managers.Enterprise.FlexItemManager _flexMgr;

        public SpecUnitMtsDownloadManager(BusinessSystemSession session)
        {
            var hash = new Dictionary<string, iDB2Command>
            {
                {"SpecUnitMtsDownloadManager.GetWeight", GetWeight()}
            };
            foreach (var keyName in hash.Keys.Where(keyName => !session.DbCommands.ContainsKey(keyName)))
            {
                session.DbCommands.Add(keyName, hash[keyName]);
            }
            _itemAttMgr = new E1.ItemAttributeManager(session);
            _flexMgr = new IGServices.Common.Managers.Enterprise.FlexItemManager(session);
        }

        public ProcessResponse Process(BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Plant plant, TimeSpan timeSpan, bool useHighWaterMark, decimal itemNum)
        {
            var response = new ProcessResponse("MTS Spec Unit Download Manager");
            var table = new SpecUnitTable();

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
                highWaterMark = (useHighWaterMark) ? HighWaterMarkManager.GetHighWaterMark(sqlCns["PLANT.SHOPFLOOR"], plant, "SpecUnitMTS") : null;
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

            if (!response.HasErrors())
            {
                /********************************************************/
                var specUnits = new SpecUnits();
                var step2 = new ProcessStep("Get MTS Items");
                try
                {
                    specUnits = GetMtsItems(session, sqlCns["PLANT.SHOPFLOOR"], plant, processRunSpan, itemNum);
                }
                catch (Exception e)
                {
                    step2.Errors.Add(new Error(e));
                }
                finally
                {
                    step2.RowsAffected = specUnits.Count;
                    step2.Done();
                    response.ProcessSteps.Add(step2);
                }

                /********************************************************/
                var step2a = new ProcessStep("Get Item Attributes");
                try
                {
                    table.Add(specUnits);
                }
                catch (Exception e)
                {
                    step2a.Errors.Add(new Error(e));
                }
                finally
                {
                    step2a.RowsAffected = specUnits.Count;
                    step2a.Done();
                    response.ProcessSteps.Add(step2a);
                }

                var step2b = new ProcessStep("Get Weight");
                try
                {
                    GetWeight(session, plant, specUnits);
                }
                catch (Exception e)
                {
                    step2b.Errors.Add(new Error(e));
                }
                finally
                {
                    step2b.RowsAffected = specUnits.Count;
                    step2b.Done();
                    response.ProcessSteps.Add(step2b);
                }

                /********************************************************/
                if (table.Rows.Count > 0)
                {
                    var step3 = new ProcessStep("Bulk Copy");
                    SqlBulkCopyResponse bulkResponse = new SqlBulkCopyResponse();
                    try
                    {
                        if (table.Rows.Count > 0)
                        {
                            bulkResponse =
                                BulkCopyManager.BulkTable(table.ExtendedProperties["Target"].ToString(), table, sqlCns["PLANT.INTEGRATION"]);
                        }
                    }
                    catch (Exception e)
                    {
                        step3.Errors.Add(new Error(e));
                    }
                    finally
                    {
                        step3.RowsAffected = (int)bulkResponse.RowsCopied;
                        step3.SqlBulkCopyResponses.Add(bulkResponse);
                        step3.Done();
                        response.RowsAffected = step3.RowsAffected;
                        response.AddStep(step3);
                    }
                }

                /********************************************************/
                if (useHighWaterMark)
                {
                    var step4 = new ProcessStep("Update High Water Mark");
                    try
                    {
                        if (table.Rows.Count > 0)
                        {
                            highWaterMark.LocalDateTime = table.MaxDate;
                            highWaterMark.EnterpriseDateTime = table.MaxDate.AddHours(-plant.TimeOffset);
                        }
                        step4.SqlDmlResponses.Add(HighWaterMarkManager.SaveHighWaterMark(sqlCns["PLANT.SHOPFLOOR"], highWaterMark));
                    }
                    catch (Exception e)
                    {
                        step4.Errors.Add(new Error(e));
                    }
                    finally
                    {
                        step4.Done();
                        response.AddStep(step4);
                    }
                }
            }
            response.Done();
            Logging.InsertProcessLog(response);
            return response;
        }

        /******************************************************************
         * Item Attributes
         ******************************************************************/
        public iDB2Command GetMtsItems(List<string> filters)
        {
            var filterString = new StringBuilder();
            foreach (string filter in filters)
            {
                filterString.Append(filter);
            }

            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = $@"
            SELECT 
	            *
            FROM (
	            SELECT
		            IAITM,
		            IAUPMJ,
		            IATDAY,
                    IBSRP4,
                    IBSTKT,
		            ROW_NUMBER() OVER (PARTITION BY IAITM ORDER BY IAUPMJ DESC, IATDAY DESC) AS ROWNUM
	            FROM 
		            F4102
		            INNER JOIN F580002
		            ON IAITM = IBITM
	            WHERE 
                    {filterString}
                    IBSTKT != 'C' AND
                    IBGLPT IN ('GLAS','GLAF','GLAX','GLAZ')) A
            WHERE
	            A.ROWNUM = 1
            FOR READ ONLY";
            return cmd;
        }

        public SpecUnits GetMtsItems(BusinessSystemSession session, SqlConnection cn, Plant plant, ProcessRunSpan span, decimal itemNum)
        {
            var specUnits = new SpecUnits();
            var cmd = new iDB2Command();
            List<string> filters = new List<string>();
            List<iDB2Parameter> parameters = new List<iDB2Parameter>();
            try
            {
                // Plant
                if (!String.IsNullOrEmpty(plant.PlantCode))
                {
                    filters.Add("IBMCU = ? AND\r\n");
                    parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
                    parameters.Last().Value = plant.PlantNum.PadLeft(12);
                }

                // Run Span
                if (!span.IsEmpty)
                {
                    var queryString1 = E1TimeManager.GetE1DateTimeQueryString("IBUPMJ", "IBTDAY", span.E1TimeIdSpan);
                    var queryString2 = E1TimeManager.GetE1DateTimeQueryString("IAUPMJ", "IATDAY", span.E1TimeIdSpan);
                    filters.Add("(" + queryString1 + " OR " + queryString2 + ")" + " AND \r\n");
                    //var queryString = E1TimeManager.GetE1DateTimeQueryString("IAUPMJ", "IATDAY", span.E1TimeIdSpan);
                    //filters.Add(queryString + " AND \r\n");
                }

                // Item Num
                if (itemNum > 0)
                {
                    filters.Add("IAITM = ? AND \r\n");
                    parameters.Add(new iDB2Parameter("ITM", iDB2DbType.iDB2Decimal));
                    parameters.Last().Precision = 8;
                    parameters.Last().Scale = 0;
                    parameters.Last().Value = itemNum;
                }

                cmd = GetMtsItems(filters);
                foreach (iDB2Parameter parameter in parameters)
                {
                    cmd.Parameters.Add(parameter);
                }
                cmd.Connection = session.DbConnection;
                _logger.Info(Logging.DbCmdString(cmd));

                _logger.Info("Start MTS Units");
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
                while (reader.Read())
                {
                    var specUnit = new SpecUnit();
                    specUnit.Id = 0;
                    specUnit.ItemNum = Data.CastDbNull.To<decimal>(reader["IAITM"], 0);
                    specUnit.StockType = Data.CastDbNull.To<string>(reader["IBSTKT"], String.Empty).Trim();
                    specUnit.UnitType = Data.CastDbNull.To<string>(reader["IBSRP4"], String.Empty).Trim();
                    specUnit.TransactionDate = E1TimeManager.CastToDateTime(
                        Data.CastDbNull.To<decimal>(reader["IAUPMJ"], 0),
                        Data.CastDbNull.To<decimal>(reader["IATDAY"], 0)
                    );

                    var itemAtts = _itemAttMgr.GetDiscreteItemAttributes(session, specUnit.ItemNum);
                    specUnit.ItemAttributes = itemAtts;
                    specUnits.Add(specUnit);
                }

                reader.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return specUnits;
        }

        /******************************************************************
         * Weight
         ******************************************************************/
        public static iDB2Command GetWeight()
        {
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = 0;
            cmd.CommandText = @"
            SELECT
                UMCONV / 10000000 AS UMCONV
            FROM
                F41002
            WHERE
		        UMMCU = ? AND
                UMITM = ? AND
		        UMUM = 'EA' AND
		        UMRUM = 'LB'";
            cmd.Parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("ITM", iDB2DbType.iDB2Decimal));
            cmd.Parameters["ITM"].Precision = 8;
            cmd.Parameters["ITM"].Scale = 0;
            return cmd;
        }

        public void GetWeight(BusinessSystemSession session, Plant plant, SpecUnits specUnits)
        {
            try
            {
                var cmd = session.DbCommands["SpecUnitMtsDownloadManager.GetWeight"];
                cmd.Connection = session.DbConnection;
                foreach (var specUnit in specUnits)
                {
                    specUnit.GlassWeightLbs = 0;
                    cmd.Parameters[0].Value = plant.PlantNum.PadLeft(12);
                    cmd.Parameters[1].Value = specUnit.ItemNum;
                    IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
                    while (reader.Read())
                    {
                        specUnit.GlassWeightLbs = CastDbNull.To<decimal>(reader["UMCONV"], 0);
                    }
                    reader.Close();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

    }
}
