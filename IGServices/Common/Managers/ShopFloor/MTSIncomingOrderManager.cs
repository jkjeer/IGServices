using System;
using System.Data;
using Microsoft.Data.SqlClient;
using IGServices.Common.Managers.Enterprise;
using IGServices.Common.Types.E1;
using IGServices.Common.Types.Response;
using IGServices.Common.Types.ShopFloor;
using IGServices.Common.Utilities;
using NLog;

namespace IGServices.Common.Managers.ShopFloor
{
    public class MTSIncomingOrderManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static class Tmp<T>
        {
            [ThreadStatic]
            public static T Value;
        }

        public static MTSIncomingOrderLines BuildMTSIncomingOrderLines(E1ScheduleData e1ScheduleData)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"BEGIN: {DateTime.Now.ToLocalTime()}");
            var incomingOrderLines = new MTSIncomingOrderLines();
            try
            {
                // ReSharper disable once LoopCanBeConvertedToQuery
                foreach (var f580021 in e1ScheduleData.F580021s)
                {
                    var f580024 = e1ScheduleData.F580024s?.Find(item => item.S8EDBT == f580021.S7EDBT && item.S8EDLN == f580021.S7EDLN && item.S8Z58SCN == f580021.S7Z58SCN && item.S8Z58SCSN == f580021.S7Z58SCSN);
                    if (f580024 != null)
                    {
                        var incomingOrderLine = new MTSIncomingOrderLine
                        {
                            PlantNum = f580021.S7MCU,
                            ScheduleNum = f580021.S7Z58SCN,
                            ScheduleSeq = f580021.S7Z58SCSN,
                            ScheduleDate = Utilities.Common.CastToDateTime(f580021.S7Z58SCDDJ),
                            ScheduleDateE1 = f580021.S7Z58SCDDJ,
                            ScheduledQty = f580021.S7Z58QTY,
                            WOrderNum = f580021.S7DOCO,
                            LineID = int.TryParse(f580021.S7Z58IGLN, out Tmp<int>.Value) ? Tmp<int>.Value : 0,
                            ItemNumber = f580021.S7LITM,
                            Construction = f580021.S7DSC1,
                            Dimensions = f580021.S7DSC2,
                            WOrderType = f580021.S7DCTO,
                            Customer = f580021.S7ALPH,
                            Lite1_Inventory = Convert.ToInt32(f580021.S7Z58L1QOH),
                            Lite2_Inventory = Convert.ToInt32(f580021.S7Z58L2QOH),
                            Lite3_Inventory = Convert.ToInt32(f580021.S7Z58L3QOH),
                            Finished_Inventory = Convert.ToInt32(f580021.S7Z58L4QOH),
                            Lite1_ReleaseNote = f580021.S7Z58FLRN,
                            Lite1_ReleaseTimeNote = f580021.S7Z58FLTN,
                            Lite2_ReleaseNote = f580021.S7Z58SLRN,
                            Lite2_ReleaseTimeNote = f580021.S7Z58SLTN,
                            Lite3_ReleaseNote = f580021.S7Z58TLRN,
                            Lite3_ReleaseTimeNote = f580021.S7Z58TLTN,
                            Completed = false,
                            MuntinComplete = false,
                            ChannelComplete = false,
                            GlassComplete = false,
                            PackingComplete = false,
                            InterLayerComplete = false,
                            AutoClaveComplete = false,
                            ChangedNote = f580021.S7Z58CHGN,
                            SalesMsg1 = f580021.S7Z58GAL1,
                            SalesMsg2 = f580021.S7Z58GAL2,
                            SalesMsg3 = "",
                            SalesMsg4 = "",
                            ProcessMsg1 = "",
                            ProcessMsg2 = "",
                            ShapeNum = f580021.S7Z58SHAPE,
                            RackType = f580021.S7Z58RT,
                            LitesPerRack = f580021.S7Z58LPR,
                            E1BatchID = f580021.S7EDBT,
                            Lite1Weight = f580024.S8Z5887004 / 1000,
                            Lite2Weight = f580024.S8Z5887005 / 1000,
                            Lite3Weight = f580024.S8Z5887006 / 1000
                        };
                        incomingOrderLine.DisplayCustomer = incomingOrderLine.Customer.Length == 0 ? f580021.ABALPH : incomingOrderLine.Customer;
                        incomingOrderLines.Add(incomingOrderLine);
                    }
                    else
                    {
                        throw new ApplicationException("Missing F580024 at BuildMTSIncomingOrderLines");
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                throw new ApplicationException($"{e.Message}: {e.StackTrace}");
            }
            finally
            {
                Logger.Info($"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }
            return incomingOrderLines;
        }

        public SqlBulkCopyResponse DownloadSchedule(SqlConnection cnSFInt, MTSIncomingOrderLines mtoIncomingOrderLines, bool write)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"BEGIN: {DateTime.Now.ToLocalTime()}");
            const string tableName = "MTSIncomingOrdersLedger";
            SqlBulkCopyResponse sqlBulkCopyResponse;
            try
            {
                var triggerManager = new TriggerManager();
                var triggers = triggerManager.GetTriggers(cnSFInt, tableName);
                if (!write)
                {
                    triggerManager.SetTriggers(cnSFInt, tableName, false);
                }
                var bcMgr = new SqlBulkCopyManager
                {
                    DbConnectionString = cnSFInt.ConnectionString
                };
                IDataReader dr = mtoIncomingOrderLines.GetDataReader();
                sqlBulkCopyResponse = bcMgr.BulkReader(tableName, dr);
                if (!write)
                {
                    triggerManager.ResetTriggers(cnSFInt, triggers);
                }
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {cnSFInt.ConnectionString}  {e.Message}: {e.StackTrace}");
                throw new ApplicationException($"{cnSFInt.ConnectionString}  {e.Message}: {e.StackTrace}");
            }
            finally
            {
                Logger.Info($"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }
            return sqlBulkCopyResponse;
        }

        public int CountDownloadedWorkOrders(SqlConnection cnSFInt, decimal scheduleNum, DateTime scheduleDate, string batchNum)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"BEGIN: {DateTime.Now.ToLocalTime()}");
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
                                  FROM      [INTEGRATION].[EBS].[MTSIncomingOrdersLedger]
                                  WHERE     [ScheduleNum] = @ScheduleNum
                                        AND [ScheduleDate] = @ScheduleDate
                                ) x WHERE [RowNumber] = 1"
                };
                cmd.Parameters.Add(new SqlParameter("@ScheduleNum", SqlDbType.SmallInt));
                cmd.Parameters["@ScheduleNum"].Value = scheduleNum;
                cmd.Parameters.Add(new SqlParameter("@ScheduleDate", SqlDbType.DateTime));
                cmd.Parameters["@ScheduleDate"].Value = scheduleDate;
                cmd.Connection = cnSFInt;
                Logger.Debug(Logging.DbCmdString(cmd));
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
                while (reader.Read())
                {
                    rowCount = CastDbNull.To(reader["RowCount"], 0);
                }
                reader.Close();
                Logger.Trace($@"ROWS: {rowCount}");
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {cnSFInt.ConnectionString}  {e.Message}: {e.StackTrace}");
                throw new ApplicationException($"{cnSFInt.ConnectionString}  {e.Message}: {e.StackTrace}");
            }
            finally
            {
                Logger.Info($"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }
            return rowCount;
        }
    }
}
