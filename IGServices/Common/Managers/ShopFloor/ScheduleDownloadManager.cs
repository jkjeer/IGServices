using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.ServiceModel;
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

namespace IGServices.Common.Managers.ShopFloor
{
    public class ScheduleDownloadManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public ScheduleDownloadManager(BusinessSystemSession session)
        {
            var hash = new Dictionary<string, iDB2Command>
            {
                {"ScheduleDownloadManager.GetQuantity", GetQuantity()}
            };
            foreach (var keyName in hash.Keys.Where(keyName => !session.DbCommands.ContainsKey(keyName)))
            {
                session.DbCommands.Add(keyName, hash[keyName]);
            }
        }

        public ProcessResponse Process(BusinessSystemSession session, SqlConnection cn, Plant plant, TimeSpan span, bool useHighWaterMark)
        {
            var response = new ProcessResponse("Schedule Download");
            var table = new ScheduleTable();
            var mark = (useHighWaterMark) ? HighWaterMarkManager.GetHighWaterMark(cn, plant, "E1_F5834A01") : null;
            var processRunSpan = ProcessRunSpanManager.GetRunSpan(mark, span, E1TimeManager.GetE1TimeStamp(session));
            var reader = GetReader(session, cn, plant, processRunSpan);
            GetQuantity(session, plant, table);
            //table.Add(reader);
            if (table.Rows.Count > 0)
            {
                // Bulk Copy
                var bulkResponse = BulkCopyManager.BulkTable(table.ExtendedProperties["Target"].ToString(), table, cn);
                response.SqlBulkCopyResponses.Add(bulkResponse);
                // Set High Water Mark
                if (mark != null)
                {
                    mark.EnterpriseDateTime = table.MaxDate;
                    response.SqlDmlResponses.Add(HighWaterMarkManager.SaveHighWaterMark(cn, mark));
                }
            }
            return response;
        }

        public iDB2Command GetReader(List<string> filters)
        {
            var filterString = new StringBuilder();
            foreach (var filter in filters)
            {
                filterString.Append(filter);
            }
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandTimeout = 0,
                CommandText = $@"
                    SELECT 
                        S1MCU,
                        S1Z58SCN,
                        S1Z58SCD,
                        S1Z58SCSTS,
                        S1Z58SCTYP,
                        S1Z58SFS,
                        DRDL01 AS S1Z58SFS_DESC,
                        S1Z58SCDDJ,
                        S1UPMJ,
                        S1TDAY
                    FROM 
	                    F5834A01
                        LEFT OUTER JOIN {ConfigurationManager.AppSettings["ControlLib"]}.F0005
                        ON  DRSY = '58' AND
                            DRRT = 'SY' AND
                            TRIM(DRKY) = TRIM(S1Z58SFS)
	                WHERE 
                        {filterString} 1=1"
            };
            return cmd;
        }

        public IDataReader GetReader(BusinessSystemSession session, SqlConnection cn, Plant plant, ProcessRunSpan span)
        {
            var filters = new List<string>();
            var parameters = new List<iDB2Parameter>();
            try
            {
                // Plant - Required
                filters.Add("S1MCU = ? AND \r\n");
                parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
                parameters.Last().Value = plant.PlantNum.PadLeft(12);
                // Run Span
                if (!span.IsEmpty)
                {
                    var queryString = E1TimeManager.GetE1DateTimeQueryString("S1UPMJ", "S1TDAY", span.E1TimeIdSpan);
                    filters.Add(queryString + " AND \r\n");
                }
                var cmd = GetReader(filters);
                foreach (var parameter in parameters)
                {
                    cmd.Parameters.Add(parameter);
                }
                cmd.Connection = session.DbConnection;
                Logger.Debug(Logging.DbCmdString(cmd));
                Logger.Trace("Start Get Orders");
                return cmd.ExecuteReader(CommandBehavior.SingleResult);
            }
            catch (Exception e)
            {
                throw new FaultException(e.Message);
            }
        }

        /******************************************************************
          * Quantity
          ******************************************************************/
        public static iDB2Command GetQuantity()
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandTimeout = 0,
                CommandText = @"
                    SELECT
                        COUNT(*) AS LINES,
                        SUM(S5Z58QTY) AS UNITS
                    FROM
                        F5834A11
                    WHERE
                        S5MCU = ? AND
                        S5Z58SCN = ? AND
                        S5Z58SCD = ?"
            };
            cmd.Parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("SCN", iDB2DbType.iDB2Decimal));
            cmd.Parameters["SCN"].Precision = 5;
            cmd.Parameters["SCN"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("SCD", iDB2DbType.iDB2Decimal));
            cmd.Parameters["SCD"].Precision = 6;
            cmd.Parameters["SCD"].Scale = 0;
            return cmd;
        }

        public void GetQuantity(BusinessSystemSession session, Plant plant, DataTable dt)
        {
            try
            {
                var cmd = session.DbCommands["ScheduleDownloadManager.GetQuantity"];
                cmd.Connection = session.DbConnection;
                foreach (DataRow dr in dt.Rows)
                {
                    cmd.Parameters[0].Value = plant.PlantNum.PadLeft(12);
                    cmd.Parameters[1].Value = (decimal)dr["SCHEDULE_NUM"];
                    cmd.Parameters[2].Value = E1TimeManager.CastToE1Date((DateTime)dr["SCHEDULE_DATE"]);
                    Logger.Info(Logging.DbCmdString(cmd));
                    IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
                    while (reader.Read())
                    {
                        dr["LINES"] = CastDbNull.To(reader["LINES"], 0);
                        dr["QUANTITY"] = CastDbNull.To<decimal>(reader["UNITS"], 0);
                    }
                    reader.Close();
                }
            }
            catch (Exception e)
            {
                throw new FaultException(e.Message);
            }
        }
    }
}
