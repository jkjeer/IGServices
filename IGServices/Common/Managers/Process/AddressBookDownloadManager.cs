using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;
using IGServices.Common.Managers.E1;
using IGServices.Common.Managers.Enterprise;
using IGServices.Common.Types.E1;
using IGServices.Common.Types.Enterprise;
using IGServices.Common.Types.Response;
using IGServices.Common.Types.ShopFloor;
using IGServices.Common.Utilities;
using IBM.Data.DB2.iSeries;
using NLog;

namespace IGServices.Common.Managers.Process
{
    public class AddressBookDownloadManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /********************************************************************************
        * CONSTRUCTOR
        ********************************************************************************/
        //public AddressBookDownloadManager(BusinessSystemSession e1Session)
        //{
        //    var hash = new Dictionary<string, iDB2Command>
        //    {
        //        //{"UdcItemDownloadManager.GetUdcItem", GetUdcItem()}
        //    };
        //    foreach (var keyName in hash.Keys.Where(keyName => !e1Session.DbCommands.ContainsKey(keyName)))
        //    {
        //        e1Session.DbCommands.Add(keyName, hash[keyName]);
        //    }
        //}

        /********************************************************************************
        * PROCESS
        ********************************************************************************/
        public static ProcessResponse Process(BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Plant plant,
            TimeSpan timeSpan, bool useHighWaterMark, List<string> searchTypes = null, decimal addressNum = 0)
        {
            var response = new ProcessResponse("Address Book Download Manager");
            response.Plant = plant;
            response.ConnectionStrings.Add(new ConnectionString("E1", session.DbConnection.ConnectionString));
            foreach (var sqlCn in sqlCns)
            {
                response.ConnectionStrings.Add(new ConnectionString(sqlCn.Key, sqlCn.Value.ConnectionString));
            }

            var table = new AddressBookTable();
            ProcessRunSpan processRunSpan = new ProcessRunSpan();
            HighWaterMark highWaterMark = new HighWaterMark();

            /********************************************************/
            var step1 = new ProcessStep("Resolve Run Span");
            try
            {
                highWaterMark = (useHighWaterMark) ? HighWaterMarkManager.GetHighWaterMark(sqlCns["PLANT.SHOPFLOOR"], plant, "AddressBook") : null;
                processRunSpan = ProcessRunSpanManager.GetRunSpan(highWaterMark, timeSpan, E1TimeManager.GetE1TimeStamp(session));
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
                var step2 = new ProcessStep("Get Source Data");
                try
                {
                    var reader = GetReader(session, processRunSpan, plant.PlantNum, searchTypes, addressNum);
                    table.Add(reader, plant);
                    Logger.Info($"Rows: {table.Rows.Count}  Plant: {plant.PlantCode}");
                    step2.RowsAffected = table.Rows.Count;
                }
                catch (Exception e)
                {
                    step2.Errors.Add(new Error(e));
                }
                step2.Done();
                response.AddStep(step2);

                /********************************************************/
                if (table.Rows.Count > 0)
                {
                    var step3 = new ProcessStep("Bulk Copy");
                    var bulkResponse = new SqlBulkCopyResponse();
                    try
                    {
                        if (table.Rows.Count > 0)
                        {
                            bulkResponse = BulkCopyManager.BulkTable(table.ExtendedProperties["Target"].ToString(), table, sqlCns["PLANT.INTEGRATION"]);
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
            response.Close();
            return response;
        }

        /********************************************************************************
        * EVENTS
        ********************************************************************************/
        private static iDB2Command GetReader(IReadOnlyCollection<string> filters)
        {
            var filterString = new StringBuilder();
            if (filters.Any()) filterString.Append("WHERE ");
            foreach (var filter in filters)
            {
                filterString.Append(filter);
            }
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = $@"
		            SELECT *
		            FROM (
	                        SELECT 
	                            ABAN8,
                                ABAT1,
                                DRDL01,
                                ABALPH,
                                ALADD1,
                                ALADD2,
                                ALADD3,
                                ALADD4,
                                ALADDZ,
                                ALCTY1,
                                ALADDS,
                                ALCTR,
                                WPAR1,
                                WPPH1,
                                COALESCE(MAPA8,ABAN8) AS MAPA8,
                                ABAC05,
                                ABAC29,
                                  ABUPMJ,
                                  ABUPMT,
                                  ALUPMJ,
                                  ALUPMT,
                                  WPUPMJ,
                                  WPUPMT,
                                  MAUPMJ,
                                  MAUPMT,
                                  DRUPMJ,
                                  DRUPMT,
					            RowNumber() over (PARTITION BY ABAN8 ORDER BY MAUPMJ desc, MAUPMT desc) AS ROWNUM
                            FROM 
                                F0101
	                            LEFT OUTER JOIN F0116 
                                ON  ALAN8 = ABAN8 AND
				                    ALEFTB = ABEFTB
	                            LEFT OUTER JOIN F0115 
                                ON  WPAN8 = ABAN8 AND
				                    WPIDLN = 0 AND
				                    WPRCK7 = 1 AND
				                    WPCNLN = 0
	                            LEFT OUTER JOIN F0150 
                                ON  MAAN8 = ABAN8
                                LEFT OUTER JOIN {ConfigurationManager.AppSettings["ControlLib"]}.F0005 
                                ON  DRSY = '01' AND
                                    DRRT = 'ST' AND
                                    TRIM(DRKY) = ABAT1
                                {filterString}
                      ) X WHERE ROWNUM = 1
                    FOR READ ONLY"
            };
            return cmd;
        }

        public static IDataReader GetReader(BusinessSystemSession session, ProcessRunSpan processRunSpan, string plantNum, List<string> searchTypes, decimal addressNum)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"BEGIN: {DateTime.Now.ToLocalTime()}  Plant: {plantNum}");
            var filters = new List<string>();
            var parameters = new List<iDB2Parameter>();
            try
            {
                // Search Types
                if (searchTypes.Any())
                {
                    var strInClause = "";
                    foreach (var searchType in searchTypes)
                    {
                        strInClause = strInClause + "?,";
                        parameters.Add(new iDB2Parameter("SearchTypes", iDB2DbType.iDB2Graphic, 3));
                        parameters.Last().Value = searchType;
                    }
                    filters.Add("ABAT1 IN (" + strInClause.Substring(0, strInClause.Length - 1) + ") AND \r\n");
                }
                // Address Num
                if (addressNum > 0)
                {
                    filters.Add("ABAN8 = ? AND \r\n");
                    parameters.Add(new iDB2Parameter("AN8", iDB2DbType.iDB2Decimal));
                    parameters.Last().Precision = 8;
                    parameters.Last().Scale = 0;
                    parameters.Last().Value = addressNum;
                }
                // Process Run Span
                if (!processRunSpan.IsEmpty)
                {
                    var queryString1 = E1TimeManager.GetE1DateTimeQueryString("ALUPMJ", "ALUPMT", processRunSpan.E1TimeIdSpan);
                    var queryString2 = E1TimeManager.GetE1DateTimeQueryString("ABUPMJ", "ABUPMT", processRunSpan.E1TimeIdSpan);
                    var queryString3 = E1TimeManager.GetE1DateTimeQueryString("DRUPMJ", "DRUPMT", processRunSpan.E1TimeIdSpan);
                    var queryString4 = E1TimeManager.GetE1DateTimeQueryString("WPUPMJ", "WPUPMT", processRunSpan.E1TimeIdSpan);
                    var queryString5 = E1TimeManager.GetE1DateTimeQueryString("MAUPMJ", "MAUPMT", processRunSpan.E1TimeIdSpan);
                    filters.Add($"({queryString1} OR {queryString2} OR {queryString3} OR {queryString4} OR {queryString5}) AND \r\n");
                }
                // Command
                iDB2Command cmd;
                if (filters.Any())
                {
                    filters[filters.Count - 1] = filters.Last().ToUpper().Replace(" AND \r\n", "");
                    cmd = GetReader(filters);
                    foreach (var parameter in parameters)
                    {
                        cmd.Parameters.Add(parameter);
                    }
                }
                else
                {
                    cmd = GetReader(filters);
                }
                cmd.Connection = session.DbConnection;
                Logger.Debug(Logging.DbCmdString(cmd));
                // Reader
                return cmd.ExecuteReader(CommandBehavior.SingleResult);
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                throw new Exception(e.Message);
            }
            finally
            {
                Logger.Info("END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS   Plant: " + plantNum.ToString());
            }
        }
    }
}
