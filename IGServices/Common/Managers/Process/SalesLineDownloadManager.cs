using System;
using System.Collections.Generic;
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
    public class SalesLineDownloadManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /********************************************************************************
        * CONSTRUCTOR
        ********************************************************************************/
        //public SalesLineDownloadManager(BusinessSystemSession e1Session)
        //{
        //    var hash = new Dictionary<string, iDB2Command>
        //    {
        //        //{"InventoryLocationDownloadManager.GetRelatedOrders", GetRelatedOrders()}
        //    };
        //    foreach (var keyName in hash.Keys.Where(keyName => !e1Session.DbCommands.ContainsKey(keyName)))
        //    {
        //        e1Session.DbCommands.Add(keyName, hash[keyName]);
        //    }
        //}

        /********************************************************************************
        * PROCESS
        ********************************************************************************/
        public static ProcessResponse Process(BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Plant plant, TimeSpan timeSpan, bool useHighWaterMark, decimal salesOrderNum, List<string> nextStatusCodes)
        {
            var response = new ProcessResponse("Sales Line Download");
            response.Plant = plant;
            response.ConnectionStrings.Add(new ConnectionString("E1", session.DbConnection.ConnectionString));
            foreach (var sqlCn in sqlCns) { response.ConnectionStrings.Add(new ConnectionString(sqlCn.Key, sqlCn.Value.ConnectionString)); }

            var table = new SalesLineTable();
            var tableArch = new SalesLineTable();
            ProcessRunSpan processRunSpan = new ProcessRunSpan();
            HighWaterMark highWaterMark = new HighWaterMark();

            /********************************************************/
            var step1 = new ProcessStep("Resolve Run Span");
            try
            {
                highWaterMark = (useHighWaterMark) ? HighWaterMarkManager.GetHighWaterMark(sqlCns["PLANT.SHOPFLOOR"], plant, "SalesLine") : null;
                processRunSpan = ProcessRunSpanManager.GetRunSpan(highWaterMark, timeSpan, E1TimeManager.GetE1TimeStamp(session));
                step1.Messages.Add(new Message("RunSpan", processRunSpan.Serialize()));
            }
            catch (Exception e) { step1.Errors.Add(new Error(e)); }
            finally
            {
                step1.Done();
                response.AddStep(step1);
            }

            if (!response.HasErrors())
            {

                /********************************************************/
                var step4 = new ProcessStep("Get Archive Data");
                try
                {
                    var reader = GetSalesLineArchive(session, processRunSpan, plant.PlantNum, salesOrderNum, nextStatusCodes);
                    tableArch.Add(reader, plant);
                    step4.RowsAffected = tableArch.Rows.Count;
                }
                catch (Exception e) { step4.Errors.Add(new Error(e)); }
                finally
                {
                    step4.Done();
                    response.AddStep(step4);
                }

                /********************************************************/
                if (table.Rows.Count > 0)
                {
                    var step5 = new ProcessStep("Bulk Copy");
                    var bulkResponse = new SqlBulkCopyResponse();
                    try
                    {
                        if (tableArch.Rows.Count > 0)
                        {
                            bulkResponse = BulkCopyManager.BulkTable(table.ExtendedProperties["Target"].ToString(),
                                tableArch, sqlCns["PLANT.INTEGRATION"]);
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
                var step2 = new ProcessStep("Get Source Data");
                try
                {
                    var reader = GetSalesLine(session, processRunSpan, plant.PlantNum, salesOrderNum, nextStatusCodes);
                    table.Add(reader, plant);
                    step2.RowsAffected = table.Rows.Count;
                }
                catch (Exception e) { step2.Errors.Add(new Error(e)); }
                finally
                {
                    step2.Done();
                    response.AddStep(step2);
                }

                /********************************************************/
                if (table.Rows.Count > 0)
                {
                    var step3 = new ProcessStep("Bulk Copy");
                    var bulkResponse = new SqlBulkCopyResponse();
                    try
                    {
                        if (table.Rows.Count > 0)
                        {
                            bulkResponse = BulkCopyManager.BulkTable(table.ExtendedProperties["Target"].ToString(),
                                table, sqlCns["PLANT.INTEGRATION"]);
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
                        response.RowsAffected += step3.RowsAffected;
                        response.AddStep(step3);
                    }
                }

                /********************************************************/
                if (useHighWaterMark)
                {
                    var step6 = new ProcessStep("Update High Water Mark");
                    try
                    {
                        if (table.Rows.Count > 0)
                        {
                            highWaterMark.LocalDateTime = table.MaxDate;
                            highWaterMark.EnterpriseDateTime = table.MaxDate.AddHours(-plant.TimeOffset);
                            //highWaterMark.LocalDateTime = table.MaxDate.AddHours(plant.TimeOffset);
                            //highWaterMark.EnterpriseDateTime = table.MaxDate;
                        }
                        step4.SqlDmlResponses.Add(HighWaterMarkManager.SaveHighWaterMark(sqlCns["PLANT.SHOPFLOOR"], highWaterMark));
                    }
                    catch (Exception e) { step6.Errors.Add(new Error(e)); }
                    finally
                    {
                        step6.Done();
                        response.AddStep(step6);
                    }
                }
            }
            response.Done();
            Logging.InsertProcessLog(response);
            return response;
        }

        /********************************************************************************
        * EVENTS
        ********************************************************************************/
        public static IDataReader GetSalesLine(BusinessSystemSession session, ProcessRunSpan processRunSpan, string plantNum, decimal salesOrderNum, List<string> nextStatusCodes)
        {
            var filters = new List<string>();
            var parameters = new List<iDB2Parameter>();
            try
            {
                // Plant Num
                filters.Add("SDMCU = ? AND \r\n");
                parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
                parameters.Last().Value = plantNum.PadLeft(12);

                // Process Run Span
                if (!processRunSpan.IsEmpty)
                {
                    //var dts = new List<string> {"SDUPMJ", "ATUPMJ", "CMUPMJ"};
                    //var tms = new List<string> { "SDTDAY", "ATTDAY", "CMTDAY" };
                    //filters.Add(E1TimeManager.GetE1DateTimeQueryString(dts,tms, processRunSpan.StartDateTime, processRunSpan.EndDateTime) + " AND \r\n");
                    filters.Add(E1TimeManager.GetE1DateTimeQueryString("SDUPMJ", "SDTDAY", processRunSpan.E1TimeIdSpan) + " AND \r\n");
                }

                // SalesOrderNum
                if (salesOrderNum > 0)
                {
                    filters.Add("SDDOCO = ? AND \r\n");
                    parameters.Add(new iDB2Parameter("DOCO", iDB2DbType.iDB2Decimal));
                    parameters.Last().Precision = 8;
                    parameters.Last().Scale = 0;
                    parameters.Last().Value = salesOrderNum;
                }

                // Status Codes
                if (nextStatusCodes.Any())
                {
                    var strInClause = "";
                    foreach (var statusCode in nextStatusCodes)
                    {
                        strInClause = strInClause + "?,";
                        parameters.Add(new iDB2Parameter("NXTR", iDB2DbType.iDB2Graphic, 3));
                        parameters.Last().Value = statusCode;
                    }
                    filters.Add("SDNXTR IN (" + strInClause.Substring(0, strInClause.Length - 1) + ") AND \r\n");
                }

                // Command
                var cmd = GetSalesLine(filters);
                foreach (iDB2Parameter parameter in parameters) { cmd.Parameters.Add(parameter); }
                cmd.Connection = session.DbConnection;
                Logger.Debug(Logging.DbCmdString(cmd));

                // Reader
                return cmd.ExecuteReader(CommandBehavior.SingleResult);
            }
            catch (Exception e) { throw new Exception(e.Message); }
        }

        private static iDB2Command GetSalesLine(List<string> filters)
        {
            var filterString = new StringBuilder();
            foreach (var filter in filters) { filterString.Append(filter); }
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = $@"
                SELECT 
                    SDKCOO,
	                SDDOCO,
	                SDDCTO,
	                SDLNID,
	                SDRORN,
	                SDRCTO,
	                SDTRDJ,
	                SDDRQJ,
	                SDCNDJ,
                    SDADDJ,
	                SDIVD,
	                SDITM,
	                SDLNTY,
	                SDNXTR,
	                SDLTTR,
	                SDAN8,
	                SDSHAN,
	                SDUORG,
	                SDUNCS,
	                SDECST,
	                SDUPRC,
	                SDAEXP,
	                SDDOC,
	                SDGLC,
	                SDRCD,
                    SDURCD,
	                SDVR01,
                    SDPSN,
	                SDUPMJ,
	                SDTDAY,
	                ATZ58PT,
	                ATZ58CSQNO,
	                ATZ58GSQNO,
	                ATZ58CP2,
	                ATZ58CP3,
	                ATZ58CP4,
	                ATZ5840460,
	                ATUPMJ,
	                ATTDAY,	                
                    COALESCE(CMCFGID,0) AS CMCFGID,
	                CMUPMJ,
	                CMTDAY
                FROM
                   F4211
                   LEFT OUTER JOIN F580032
                   ON ATKCOO = SDKCOO AND
	                ATDOCO = SDDOCO AND
	                ATDCTO = SDDCTO AND
	                ATLNID = SDLNID
                   LEFT OUTER JOIN F3201
                   ON CMKCOO = SDKCOO AND
	                CMDOCO = SDDOCO AND
	                CMDCTO = SDDCTO AND
	                CMLNID = SDLNID
	            WHERE 
                    {filterString}
                    1=1
                FOR READ ONLY"
            };
            return cmd;
        }

        public static IDataReader GetSalesLineArchive(BusinessSystemSession session, ProcessRunSpan processRunSpan, string plantNum, decimal salesOrderNum, List<string> nextStatusCodes)
        {
            var filters = new List<string>();
            var parameters = new List<iDB2Parameter>();
            try
            {
                // Plant Num
                filters.Add("SDMCU = ? AND \r\n");
                parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
                parameters.Last().Value = plantNum.PadLeft(12);

                // Process Run Span
                if (!processRunSpan.IsEmpty)
                {
                    //var dts = new List<string> { "SDUPMJ", "ATUPMJ", "CMUPMJ" };
                    //var tms = new List<string> { "SDTDAY", "ATTDAY", "CMTDAY" };
                    //filters.Add(E1TimeManager.GetE1DateTimeQueryString(dts, tms, processRunSpan.StartDateTime, processRunSpan.EndDateTime) + " AND \r\n");
                    filters.Add(E1TimeManager.GetE1DateTimeQueryString("SDUPMJ", "SDTDAY", processRunSpan.E1TimeIdSpan) + " AND \r\n");
                }

                // SalesOrderNum
                if (salesOrderNum > 0)
                {
                    filters.Add("SDDOCO = ? AND \r\n");
                    parameters.Add(new iDB2Parameter("DOCO", iDB2DbType.iDB2Decimal));
                    parameters.Last().Precision = 8;
                    parameters.Last().Scale = 0;
                    parameters.Last().Value = salesOrderNum;
                }

                // Status Codes
                if (nextStatusCodes.Any())
                {
                    var strInClause = "";
                    foreach (var statusCode in nextStatusCodes)
                    {
                        strInClause = strInClause + "?,";
                        parameters.Add(new iDB2Parameter("NXTR", iDB2DbType.iDB2Graphic, 3));
                        parameters.Last().Value = statusCode;
                    }
                    filters.Add("SDNXTR IN (" + strInClause.Substring(0, strInClause.Length - 1) + ") AND \r\n");
                }

                // Command
                var cmd = GetSalesLineArchive(filters);
                foreach (iDB2Parameter parameter in parameters) { cmd.Parameters.Add(parameter); }
                cmd.Connection = session.DbConnection;
                Logger.Debug(Logging.DbCmdString(cmd));

                // Reader
                return cmd.ExecuteReader(CommandBehavior.SingleResult);
            }
            catch (Exception e) { throw new Exception(e.Message); }
        }

        private static iDB2Command GetSalesLineArchive(List<string> filters)
        {
            var filterString = new StringBuilder();
            foreach (var filter in filters) { filterString.Append(filter); }
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = $@"
                SELECT 
                    SDKCOO,
	                SDDOCO,
	                SDDCTO,
	                SDLNID,
	                SDRORN,
	                SDRCTO,
	                SDTRDJ,
	                SDDRQJ,
	                SDCNDJ,
                    SDADDJ,
	                SDIVD,
	                SDITM,
	                SDLNTY,
	                SDNXTR,
	                SDLTTR,
	                SDAN8,
	                SDSHAN,
	                SDUORG,
	                SDUNCS,
	                SDECST,
	                SDUPRC,
	                SDAEXP,
	                SDDOC,
	                SDGLC,
	                SDRCD,
                    SDURCD,
	                SDVR01,
                    SDPSN,
	                SDUPMJ,
	                SDTDAY,
	                ATZ58PT,
	                ATZ58CSQNO,
	                ATZ58GSQNO,
	                ATZ58CP2,
	                ATZ58CP3,
	                ATZ58CP4,
	                ATZ5840460,
	                ATUPMJ,
	                ATTDAY,	                
                    COALESCE(CMCFGID,0) AS CMCFGID,
	                CMUPMJ,
	                CMTDAY
                FROM
                   F42119
                   LEFT OUTER JOIN F580032
                   ON ATKCOO = SDKCOO AND
	                ATDOCO = SDDOCO AND
	                ATDCTO = SDDCTO AND
	                ATLNID = SDLNID
                   LEFT OUTER JOIN F3201
                   ON CMKCOO = SDKCOO AND
	                CMDOCO = SDDOCO AND
	                CMDCTO = SDDCTO AND
	                CMLNID = SDLNID
	            WHERE 
                    {filterString}
                    1=1
                FOR READ ONLY"
            };
            return cmd;
        }
    }
}