using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using IGServices.Common.Managers.Enterprise;
using IGServices.Common.Types.Response;
using IGServices.Common.Types.ShopFloor;
using IGServices.Common.Utilities;
using NLog;

namespace IGServices.Common.Managers.ShopFloor
{
    public class MTSIncomingOrderDownloadManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public ProcessResponse Process(SqlConnection cnSFInt, MTSIncomingOrderLines incomingOrderLines)
        {
            var response = new ProcessResponse("MTS_INCOMING_ORDERS_LEDGER Download");
            var incomingOrderTable = new ScheduleInfoMTSTable();
            incomingOrderTable.Add(incomingOrderLines);
            if (incomingOrderTable.Rows.Count > 0)
            {
                //var responseTime2 = new ResponseTime(response.ProcessName, "Download", cnSFInt.ConnectionString);
                var bulkResponse = BulkCopyManager.BulkTable(incomingOrderTable.ExtendedProperties["Target"].ToString(), incomingOrderTable, cnSFInt);
                response.SqlBulkCopyResponses.Add(bulkResponse);
                //response.ResponseTimes.Add(responseTime2.Done());
            }
            return response;
        }

        public int CountDownloadedWorkOrders(SqlConnection cnSFInt, decimal scheduleNum, DateTime scheduleDate, string batchNum)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info("BEGIN: " + DateTime.Now.ToLocalTime());
            var rowCount = 0;
            try
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.Text,
                    CommandText = @"
                        SELECT  COUNT(*) AS 'RowCount'
                        FROM    ( SELECT DISTINCT
                                            [ScheduleNum]
                                           ,[ScheduleDate]
                                           ,[ScheduleSeq]
                                           ,[WOrderNum]
                                           ,[WOrderType]
                                           ,ROW_NUMBER() OVER (PARTITION BY [ScheduleNum], [ScheduleSeq] ORDER BY [DownloadDate] DESC) AS 'RowNumber'
                                  FROM      [INTEGRATION].[dbo].[MTS_INCOMING_ORDERS_LEDGER]
                                  WHERE     [ScheduleNum] = @ScheduleNum
                                        AND [ScheduleDate] = @ScheduleDate
                                ) x WHERE [RowNumber] = 1"
                };
                cmd.Parameters.Add(new SqlParameter("@ScheduleNum", SqlDbType.SmallInt));
                cmd.Parameters["@ScheduleNum"].Value = scheduleNum;
                cmd.Parameters.Add(new SqlParameter("@ScheduleDate", SqlDbType.DateTime));
                cmd.Parameters["@ScheduleDate"].Value = scheduleDate;
                cmd.Connection = cnSFInt;
                Logger.Trace("CN: " + cnSFInt.ConnectionString);
                Logger.Trace("SQL: " + Environment.NewLine + cmd.CommandText);
                Logger.Trace("PARMS: " + string.Join(", ", cmd.Parameters.Cast<SqlParameter>().AsQueryable().Select(parm => parm.Value)));
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
                var counter = 0;
                while (reader.Read())
                {
                    rowCount = CastDbNull.To(reader["RowCount"], 0);
                    counter++;
                }
                reader.Close();
                Logger.Trace("ROWS: " + counter);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.ToString());
            }
            Logger.Trace("CLOSE: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            return rowCount;
        }
    }
}
