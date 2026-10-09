using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using IGServices.Common.Managers.E1;
using IGServices.Common.Types.E1;
using IGServices.Common.Types.Enterprise;
using IGServices.Common.Types.Response;
using IGServices.Common.Types.ShopFloor;
using IGServices.Common.Utilities;
using NLog;

namespace IGServices.Common.Managers.ShopFloor
{
    public class BoxingScheduleManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static BoxingSchedule GetBoxingSchedule(BusinessSystemSession e1Session, SqlConnection cnSF, Plant plant, decimal scheduleNum)
        {
            var boxingSchedule = new BoxingSchedule(scheduleNum);
            try
            {
                var step1 = new ProcessStep("GetSFBoxingItems");
                try
                {
                    boxingSchedule = GetSFBoxingSchedule(cnSF, plant, scheduleNum);
                    step1.RowsAffected = boxingSchedule.BoxItems.Count;
                }
                catch (Exception e)
                {
                    step1.Errors.Add(new Error(e));
                }
                step1.Done();
                boxingSchedule.ProcessSteps = new ProcessSteps
                {
                    step1
                };

                if (!step1.HasErrors())
                {
                    var step2 = new ProcessStep("ValidateBoxingItems");
                    try
                    {
                        boxingSchedule.ValidationErrors = Validate(boxingSchedule, e1Session);
                        if (!boxingSchedule.IsValid())
                        {
                            boxingSchedule.TransferStatus = "E";
                            step2.RowsAffected = boxingSchedule.ValidationErrors.Count;
                            foreach (var validationError in boxingSchedule.ValidationErrors.Where(validationError => validationError.Message.Trim().Contains("previously uploaded")))
                            {
                                boxingSchedule.ValidationStatus = "Previously Uploaded";
                            }
                        }
                        step2.RowsAffected = boxingSchedule.BoxItems.Count;
                    }
                    catch (Exception e)
                    {
                        step2.Errors.Add(new Error(e));
                    }
                    step2.Done();
                    boxingSchedule.ProcessSteps.Add(step2);
                }

                if (boxingSchedule.IsValid())
                {
                    var step3 = new ProcessStep("BuildInterops");
                    var boxingInteropMgr = new BoxingInteropManager(e1Session);
                    try
                    {
                        boxingSchedule.BoxF580200 = new BoxF580200();
                        boxingInteropMgr.BuildBoxingInterops(boxingSchedule);
                        boxingInteropMgr.BuildBoxingTrigger(boxingSchedule);
                        step3.RowsAffected = boxingSchedule.BoxItems.Count;
                    }
                    catch (Exception e)
                    {
                        step3.Errors.Add(new Error(e));
                    }
                    step3.Done();
                    boxingSchedule.ProcessSteps.Add(step3);
                }
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + cnSF.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
                throw new ApplicationException(cnSF.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }
            return boxingSchedule;
        }

        public static SqlCommand GetCmdBoxItemsDynamic(List<string> filters)
        {
            var filterString = new StringBuilder();
            if (filters.Any()) filterString.Append("WHERE ");
            foreach (var filter in filters)
            {
                filterString.Append(filter);
            }
            var cmd = new SqlCommand();
            try
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = $@"
                    SELECT DISTINCT
                        bhh.[ScheduleNum] 
  			            ,bhh.[BoxHoldHeaderID]
                        ,bhh.[WrittenDate] AS 'HeaderWrittenDate'
                        ,bhh.[IsCompleted]
                        ,bhh.[Transferred] AS 'HeaderTransferred'
                        ,bhh.[TransferredDate] AS 'HeaderTransferredDate'
                        ,bhh.[TransferBatch] as 'HeaderTransferBatch'
                        ,bhh.[TransferStatus] as 'HeaderTransferStatus'
                        ,bh.[BoxHoldID]
                        ,bh.[PlantID]
                        ,bh.[WOrderNum]
                        ,bh.[WOrderType]
                        ,bh.[SOrderNum]
                        ,bh.[SOrderType]
                        ,bh.[SOrderLine]
                        ,bh.[UnitID]
                        ,bh.[SerialNum]
                        ,bh.[WrittenDate]
                        ,bh.[BoxCrate]
                        ,bh.[BoxType]
                        ,ISNULL(r.[DescriptionOne], 'Undefined Rack Type') AS 'RackTypeDesc'
                        ,ISNULL(ub.[CustName], 'Unknown Customer') AS 'UnitBoxCustomer'
                        ,ISNULL(ub.[CustName], io.[CustName]) AS 'Customer'
                        ,ub.[DueDate] AS 'UnitBoxDueDate'
                        ,ub.[TimeStamp] AS 'UnitBoxTimeStamp'
						,io.[ScheduleDate] AS 'IncomingOrdersScheduleDate'
						,io.[DownloadDate] AS 'IncomingOrdersDownloadDate'
                        ,ISNULL(io.[CustName], 'Unknown Customer') AS 'IncomingOrdersCustomer'
                        ,ISNULL(io.[CustNum], 0) AS 'IncomingOrdersCustNum'
                        ,ISNULL(io.[CustomerShipTo], 0) AS 'IncomingOrdersCustShipToNum'
                        ,ISNULL(io.[WOrderNum], 0) AS 'IncomingOrdersWorkOrderNum'
                        ,ISNULL(io.[Quantity], 0) AS 'IncomingOrdersQuantity'
                        ,bh.[BoxNumber]
                        ,bh.[BoxRotated]
                        ,bh.[BoxSide]
                        ,bh.[BoxStack]
                        ,bh.[BoxRow]
                        ,bh.[BoxPosition]
                        ,bh.[LineID]
                        ,bh.[RunSeq]
                        ,bh.[Rack1]
                        ,bh.[Slot1]
                        ,bh.[Rack2]
                        ,bh.[Slot2]
                        ,bh.[Rack3]
                        ,bh.[Slot3]
                        ,bh.[Transferred]
                        ,bh.[TransferredDate]
                        ,bh.[InProgress]
                        ,bh.[TransferBatch]
                        ,bh.[TransferUniqueID]
                        ,bh.[TransferStatus] 
                        ,ubc.[UnitBoxCount]
                        ,ubc.[UnitBoxMaxTimeStamp]
                        ,ioc.[IncomingOrdersUnitCount]
	                    ,ioc.[IncomingOrdersWorkOrderCount]
                        ,ioc.[IncomingOrdersMaxDownLoadDate]
                        ,ioc.[IncomingOrdersMinScheduleDate]
                    FROM    [dbo].[BoxHoldHeader] bhh
                            INNER JOIN [dbo].[BoxHold] bh ON bh.[ScheduleNum] = bhh.[ScheduleNum]
							LEFT OUTER JOIN [dbo].[UnitBox] ub ON ub.[SchedNum] = bh.[ScheduleNum]
							                                   AND ub.[UnitID] = bh.[UnitID]
                            LEFT OUTER JOIN [dbo].[IncomingOrders] io ON io.[ScheduleNum] = bh.[ScheduleNum]
                                                                      AND io.[WOrderNum] = bh.[WOrderNum]
                            LEFT OUTER JOIN [dbo].[E1ControlInfo] r ON r.[KeyValue] = bh.[BoxType]
                                                                    AND r.[AppCode] = '57'
                                                                    AND r.[SysCode] = 'RK'
                            CROSS APPLY ( SELECT    CONVERT(INT, COUNT(*)) AS 'UnitBoxCount'
                                                    ,MAX([TimeStamp]) AS 'UnitBoxMaxTimeStamp'
                                          FROM      [dbo].[UnitBox] u
                                          WHERE     u.[SchedNum] = bhh.[ScheduleNum]
                                        ) ubc
                            CROSS APPLY ( SELECT    CONVERT(INT, SUM(i.[Quantity])) AS 'IncomingOrdersUnitCount'
                                                    ,CONVERT(INT, COUNT(i.[WOrderNum])) AS 'IncomingOrdersWorkOrderCount'
                                                    ,MAX([DownloadDate]) AS 'IncomingOrdersMaxDownLoadDate'
                                                     ,MIN([ScheduleDate]) AS 'IncomingOrdersMinScheduleDate'
                                         FROM      [dbo].[IncomingOrders] i
                                         WHERE     i.[ScheduleNum] = bhh.[ScheduleNum]
                                        ) ioc
                    {filterString}";
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + e.Message + ": " + e.StackTrace);
                throw new ApplicationException(e.Message + ": " + e.StackTrace);
            }
            return cmd;
        }

        public static BoxingSchedule GetSFBoxingSchedule(SqlConnection cnSF, Plant plant, decimal scheduleNum)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info("BEGIN: " + DateTime.Now.ToLocalTime());
            var boxingSchedule = new BoxingSchedule(scheduleNum);
            var boxItems = new BoxItems();
            var filters = new List<string>();
            var parameters = new List<SqlParameter>();
            try
            {
                // MTOSchedule Num
                if (scheduleNum > 0)
                {
                    filters.Add("bhh.[ScheduleNum] = @ScheduleNum AND \r\n");
                    parameters.Add(new SqlParameter("@ScheduleNum", SqlDbType.SmallInt));
                    parameters.Last().Value = scheduleNum;
                }
                if (filters.Any())
                {
                    filters[filters.Count - 1] = filters.Last().ToUpper().Replace(" AND \r\n", "");
                    var cmd = GetCmdBoxItemsDynamic(filters);
                    foreach (var parameter in parameters)
                    {
                        cmd.Parameters.Add(parameter);
                    }
                    cmd.Connection = cnSF;
                    cmd.CommandTimeout = int.Parse(ConfigurationManager.AppSettings["BoxingQueryTimeout"]);
                    Logger.Trace(Logging.DbCmdString(cmd));
                    IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
                    while (reader.Read())
                    {
                        boxingSchedule.PlantNum = plant.PlantNum;
                        boxingSchedule.PlantCode = plant.PlantCode;
                        boxingSchedule.ScheduleNum = scheduleNum;
                        boxingSchedule.TransferStatus = CastDbNull.To(reader["HeaderTransferStatus"], string.Empty).Trim();
                        boxingSchedule.BoxHoldHeaderID = CastDbNull.To(reader["BoxHoldHeaderID"], 0);
                        boxingSchedule.BatchDate = CastDbNull.To(reader["HeaderTransferredDate"], new DateTime(1900, 1, 1));
                        boxingSchedule.TransferBatchNum = CastDbNull.To(reader["HeaderTransferBatch"], string.Empty).Trim();
                        boxingSchedule.IsCompleted = CastDbNull.To(reader["IsCompleted"], false);
                        boxingSchedule.UpdateDate = CastDbNull.To(reader["HeaderWrittenDate"], new DateTime(1900, 1, 1));
                        boxingSchedule.Transferred = CastDbNull.To(reader["HeaderTransferred"], false);
                        boxingSchedule.TransferDate = CastDbNull.To(reader["HeaderTransferredDate"], new DateTime(1900, 1, 1));
                        boxingSchedule.TransferDateString = boxingSchedule.TransferDate.ToString("M/dd/yyyy hh:mm:ss tt");
                        boxingSchedule.TransferStatus = CastDbNull.To(reader["HeaderTransferStatus"], string.Empty).Trim();
                        boxingSchedule.UnitBoxCount = CastDbNull.To(reader["UnitBoxCount"], 0);
                        boxingSchedule.UnitBoxMaxTimeStamp = CastDbNull.To(reader["UnitBoxMaxTimeStamp"], new DateTime(1900, 1, 1));
                        boxingSchedule.IncomingOrdersUnitCount = CastDbNull.To(reader["IncomingOrdersUnitCount"], 0);
                        boxingSchedule.IncomingOrdersWorkOrderCount = CastDbNull.To(reader["IncomingOrdersWorkOrderCount"], 0);
                        boxingSchedule.IncomingOrdersMaxDownLoadDate = CastDbNull.To(reader["IncomingOrdersMaxDownLoadDate"], new DateTime(1900, 1, 1));
                        boxingSchedule.IncomingOrdersMinScheduleDate = CastDbNull.To(reader["IncomingOrdersMinScheduleDate"], new DateTime(1900, 1, 1));
                        var boxItem = new BoxItem
                        {
                            PlantNum = plant.PlantNum,
                            ScheduleNum = CastDbNull.To<short>(reader["ScheduleNum"], 0),
                            BoxHoldId = CastDbNull.To(reader["BoxHoldId"], 0),
                            BoxHoldHeaderID = CastDbNull.To(reader["BoxHoldHeaderID"], 0),
                            TransferBatchNum = CastDbNull.To(reader["TransferBatch"], string.Empty).Trim(),
                            UniqueID = CastDbNull.To<decimal>(reader["TransferUniqueID"], 0),
                            WorkOrderNum = CastDbNull.To<decimal>(reader["WOrderNum"], 0),
                            WorkOrderType = CastDbNull.To(reader["WOrderType"], string.Empty).Trim(),
                            UnitBoxCustomer = CastDbNull.To(reader["UnitBoxCustomer"], string.Empty).Trim(),
                            UnitBoxDueDate = CastDbNull.To(reader["UnitBoxDueDate"], new DateTime(1900, 1, 1)),
                            UnitBoxTimeStamp = CastDbNull.To(reader["UnitBoxTimeStamp"], new DateTime(1900, 1, 1)),
                            IncomingOrdersScheduleDate = CastDbNull.To(reader["IncomingOrdersScheduleDate"], new DateTime(1900, 1, 1)),
                            IncomingOrdersDownloadDate = CastDbNull.To(reader["IncomingOrdersDownloadDate"], new DateTime(1900, 1, 1)),
                            IncomingOrdersCustomer = CastDbNull.To(reader["IncomingOrdersCustomer"], string.Empty).Trim(),
                            IncomingOrdersCustNum = CastDbNull.To(reader["IncomingOrdersCustNum"], 0),
                            IncomingOrdersShipToCustNum = CastDbNull.To<decimal>(reader["IncomingOrdersCustShipToNum"], 0),
                            Customer = CastDbNull.To(reader["Customer"], string.Empty).Trim(),
                            IncomingOrdersWorkOrderNum = CastDbNull.To<decimal>(reader["IncomingOrdersWorkOrderNum"], 0),
                            IncomingOrdersQuantity = CastDbNull.To<decimal>(reader["IncomingOrdersQuantity"], 0),
                            SalesOrderNum = CastDbNull.To<decimal>(reader["SOrderNum"], 0),
                            SalesOrderType = CastDbNull.To(reader["SOrderType"], string.Empty).Trim(),
                            SalesOrderLine = CastDbNull.To<decimal>(reader["SOrderLine"], 0),
                            UnitId = CastDbNull.To(reader["UnitId"], string.Empty).Trim(),
                            BoxCrate = CastDbNull.To(reader["BoxCrate"], false),
                            SerialNum = CastDbNull.To(reader["SerialNum"], string.Empty).Trim(),
                            RackType = CastDbNull.To(reader["BoxType"], string.Empty).Trim(),
                            BoxNum = CastDbNull.To<short>(reader["BoxNumber"], 0),
                            BoxRotated = CastDbNull.To(reader["BoxRotated"], false),
                            Side = CastDbNull.To(reader["BoxSide"], string.Empty).Trim(),
                            Stack = CastDbNull.To<byte>(reader["BoxStack"], 0),
                            Row = CastDbNull.To<byte>(reader["BoxRow"], 0),
                            Slot = CastDbNull.To<short>(reader["BoxPosition"], 0),
                            LineID = CastDbNull.To<byte>(reader["LineID"], 0),
                            RunSequence = CastDbNull.To<short>(reader["RunSeq"], 0),
                            Lite1Rack = CastDbNull.To(reader["Rack1"], string.Empty).Trim(),
                            Lite1Slot = CastDbNull.To(reader["Slot1"], 0),
                            Lite2Rack = CastDbNull.To(reader["Rack2"], string.Empty).Trim(),
                            Lite2Slot = CastDbNull.To(reader["Slot2"], 0),
                            Lite3Rack = CastDbNull.To(reader["Rack3"], string.Empty).Trim(),
                            Lite3Slot = CastDbNull.To(reader["Slot3"], 0),
                            UpdateDate = CastDbNull.To(reader["WrittenDate"], new DateTime(1900, 1, 1)),
                            OnSkid = "", // CastDbNull.To<String>(reader["OnSkid"], String.Empty);
                            RackId = 0,
                            RackTypeDesc = CastDbNull.To(reader["RackTypeDesc"], string.Empty).Trim(),
                            Transferred = CastDbNull.To(reader["Transferred"], false),
                            TransferStatus = CastDbNull.To(reader["TransferStatus"], string.Empty).Trim(),
                            TransferDate = CastDbNull.To(reader["TransferredDate"], new DateTime(1900, 1, 1)),
                            InProgress = CastDbNull.To(reader["InProgress"], false)
                        };
                        boxItem.TransferDateString = boxItem.TransferDate.ToString("M/dd/yyyy hh:mm:ss tt", CultureInfo.CreateSpecificCulture("en-US"));
                        boxItem.IGLine = Convert.ToString(boxItem.LineID);
                        boxItems.Add(boxItem);
                    }
                    reader.Close();
                    Logger.Trace("ROWS: " + boxItems.Count);
                    boxingSchedule.BoxItems = boxItems;
                    boxingSchedule.WorkOrderCount = boxingSchedule.BoxItems.Select(x => x.WorkOrderNum).Distinct().Count();
                }
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + cnSF.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
                throw new ApplicationException(cnSF.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }
            finally
            {
                Logger.Info("END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            }
            return boxingSchedule;
        }

        public static BoxingEvent GetBoxingEvent(SqlConnection cnSF, Plant plant, decimal scheduleNum)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info("BEGIN: " + DateTime.Now.ToLocalTime());
            var boxEvent = new BoxingEvent();
            try
            {
                var cmd = new SqlCommand
                {
                    Connection = cnSF,
                    CommandType = CommandType.Text,
                    CommandText = @"
                        SELECT  bh.[PlantID]
                               ,bhh.[ScheduleNum]
                               ,bhh.[BoxHoldHeaderID]
                               ,bhh.[WrittenDate] AS 'HeaderWrittenDate'
                               ,bhh.[IsCompleted]
                               ,bhh.[Transferred] AS 'HeaderTransferred'
                               ,bhh.[TransferStatus] AS 'HeaderTransferStatus'
                               ,COUNT(DISTINCT bh.[UnitID]) AS 'Units'
                               ,COUNT(DISTINCT bh.[WOrderNum]) AS 'WorkOrders'
                               ,COUNT(DISTINCT bh.[BoxNumber]) AS 'Boxes'
                               ,COUNT(DISTINCT ISNULL(r.[DescriptionOne], 'Undefined Rack Type')) AS 'RackTypes'
                               ,COUNT(DISTINCT ISNULL(ub.[CustName], 'Unknown Customer')) AS 'Customers'
                               ,MIN(bh.[UnitID]) AS 'MinUnitId'
                               ,MAX(bh.[UnitID]) AS 'MaxUnitId'
                               ,MIN(ub.[DueDate]) AS 'MinDueDate'
                               ,MAX(ub.[DueDate]) AS 'MaxDueDate'
                        FROM    [dbo].[BoxHoldHeader] bhh
                                INNER JOIN [dbo].[BoxHold] bh ON bh.[ScheduleNum] = bhh.[ScheduleNum]
                                LEFT OUTER JOIN [dbo].[UnitBox] AS ub ON ub.[UnitID] = bh.[UnitID]
                                LEFT OUTER JOIN [dbo].[E1ControlInfo] r ON r.[KeyValue] = bh.[BoxType]
                                                                           AND r.[AppCode] = '57'
                                                                           AND r.[SysCode] = 'RK'
                        WHERE   bhh.[ScheduleNum] = @ScheduleNum
                        GROUP BY bh.[PlantID]
                               ,bhh.[ScheduleNum]
                               ,bhh.[BoxHoldHeaderID]
                               ,bhh.[WrittenDate]
                               ,bhh.[IsCompleted]
                               ,bhh.[Transferred]
                               ,bhh.[TransferStatus]"
                };
                cmd.Parameters.Add(new SqlParameter("@ScheduleNum", SqlDbType.SmallInt));
                cmd.Parameters["@ScheduleNum"].Value = scheduleNum;
                cmd.CommandTimeout = int.Parse(ConfigurationManager.AppSettings["BoxingQueryTimeout"]);
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
                var rowCount = 0;
                while (reader.Read())
                {
                    boxEvent = new BoxingEvent
                    {
                        BoxHoldHeaderID = CastDbNull.To(reader["BoxHoldHeaderID"], 0),
                        PlantNum = plant.PlantNum,
                        PlantCode = plant.PlantCode,
                        ScheduleNum = CastDbNull.To<short>(reader["ScheduleNum"], 0),
                        HeaderWrittenDate = CastDbNull.To(reader["HeaderWrittenDate"], default(DateTime)),
                        IsCompleted = CastDbNull.To(reader["IsCompleted"], false),
                        HeaderTransferred = CastDbNull.To(reader["HeaderTransferred"], false),
                        HeaderTransferStatus = CastDbNull.To(reader["HeaderTransferStatus"], string.Empty).Trim(),
                        Units = CastDbNull.To(reader["Units"], 0),
                        WorkOrders = CastDbNull.To(reader["WorkOrders"], 0),
                        Boxes = CastDbNull.To(reader["Boxes"], 0),
                        RackTypes = CastDbNull.To(reader["RackTypes"], 0),
                        Customers = CastDbNull.To(reader["Customers"], 0),
                        MinUnitID = CastDbNull.To(reader["MinUnitID"], string.Empty).Trim(),
                        MaxUnitID = CastDbNull.To(reader["MaxUnitID"], string.Empty).Trim(),
                        MinDueDate = CastDbNull.To(reader["MinDueDate"], default(DateTime)),
                        MaxDueDate = CastDbNull.To(reader["MaxDueDate"], default(DateTime))
                    };
                    rowCount++;
                }
                reader.Close();
                Logger.Trace("ROWS: " + rowCount);
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + cnSF.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
                throw new ApplicationException(cnSF.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }
            finally
            {
                Logger.Info("END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            }
            return boxEvent;
        }

        public static BoxingEvents GetUnprocessedBoxingEvents(SqlConnection cnSF, Plant plant)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info("BEGIN: " + DateTime.Now.ToLocalTime());
            var boxEvents = new BoxingEvents();
            var lockExpireMinutes = Convert.ToInt16(ConfigurationManager.AppSettings["LockExpireMinutes"]);
            try
            {
                var cmd = new SqlCommand
                {
                    Connection = cnSF,
                    CommandType = CommandType.Text,
                    CommandText = @"
                        SELECT  bh.[PlantID]
                               ,bhh.[ScheduleNum]
					           ,bhh.[BoxHoldHeaderID]
                               ,bhh.[WrittenDate] AS 'HeaderWrittenDate'
                               ,bhh.[IsCompleted]
                               ,bhh.[Transferred] AS 'HeaderTransferred'
                               ,bhh.[TransferStatus] AS 'HeaderTransferStatus'
   					           ,bhh.[TransferredDate]
                               ,CONVERT(INT, COUNT(DISTINCT bh.[UnitID])) AS 'Units'
                               ,CONVERT(INT, COUNT(DISTINCT bh.[WOrderNum])) AS 'WorkOrders'
                               ,CONVERT(INT, COUNT(DISTINCT bh.[BoxNumber])) AS 'Boxes'
                               ,COUNT(DISTINCT ISNULL(r.[DescriptionOne], 'Undefined Rack Type')) AS 'RackTypes'
                               ,COUNT(DISTINCT ISNULL(ub.[CustName], 'Unknown Customer')) AS 'Customers'
                               ,MIN(bh.[UnitID]) AS 'MinUnitId'
                               ,MAX(bh.[UnitID]) AS 'MaxUnitId'
                               ,MIN(ub.[DueDate]) AS 'MinDueDate'
                               ,MAX(ub.[DueDate]) AS 'MaxDueDate'
                        FROM    [dbo].[BoxHoldHeader] bhh
                                INNER JOIN [dbo].[BoxHold] bh ON bh.[ScheduleNum] = bhh.[ScheduleNum]
                                LEFT OUTER JOIN [dbo].[UnitBox] AS ub ON ub.[UnitID] = bh.[UnitID]
                                LEFT OUTER JOIN [dbo].[E1ControlInfo] r ON r.[KeyValue] = bh.[BoxType]
                                                                           AND r.[AppCode] = '57'
                                                                           AND r.[SysCode] = 'RK'
                        WHERE   bh.Transferred = 0
                                AND bhh.IsCompleted = 1
                                AND ( 
								        bhh.[TransferStatus] IN ( '', 'N' ) 
						                OR ( 
                                                bhh.[TransferStatus] IN ( 'L' ) 
							                    AND bhh.[TransferredDate] < DATEADD(MINUTE, -@LockExpireMinutes, GETDATE())
                                            )
							        )
                        GROUP BY bh.[PlantID]
                               ,bhh.[ScheduleNum]
					           ,bhh.[BoxHoldHeaderID]
                               ,bhh.[WrittenDate]
                               ,bhh.[IsCompleted]
                               ,bhh.[Transferred]
                               ,bhh.[TransferStatus]
   					           ,bhh.[TransferredDate] 
                        ORDER BY bhh.[ScheduleNum]
                        "
                };
                cmd.Parameters.Add(new SqlParameter("@LockExpireMinutes", SqlDbType.SmallInt));
                cmd.Parameters["@LockExpireMinutes"].Value = lockExpireMinutes;
                cmd.CommandTimeout = int.Parse(ConfigurationManager.AppSettings["BoxingQueryTimeout"]);
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
                while (reader.Read())
                {
                    var boxEvent = new BoxingEvent
                    {
                        PlantNum = plant.PlantNum,
                        PlantCode = plant.PlantCode,
                        ScheduleNum = CastDbNull.To<short>(reader["ScheduleNum"], 0),
                        BoxHoldHeaderID = CastDbNull.To(reader["BoxHoldHeaderID"], 0),
                        HeaderWrittenDate = CastDbNull.To(reader["HeaderWrittenDate"], default(DateTime)),
                        IsCompleted = CastDbNull.To(reader["IsCompleted"], false),
                        HeaderTransferred = CastDbNull.To(reader["HeaderTransferred"], false),
                        HeaderTransferStatus = CastDbNull.To(reader["HeaderTransferStatus"], string.Empty).Trim(),
                        Units = CastDbNull.To(reader["Units"], 0),
                        WorkOrders = CastDbNull.To(reader["WorkOrders"], 0),
                        Boxes = CastDbNull.To(reader["Boxes"], 0),
                        RackTypes = CastDbNull.To(reader["RackTypes"], 0),
                        Customers = CastDbNull.To(reader["Customers"], 0),
                        MinUnitID = CastDbNull.To(reader["MinUnitID"], string.Empty).Trim(),
                        MaxUnitID = CastDbNull.To(reader["MaxUnitID"], string.Empty).Trim(),
                        MinDueDate = CastDbNull.To(reader["MinDueDate"], default(DateTime)),
                        MaxDueDate = CastDbNull.To(reader["MaxDueDate"], default(DateTime))
                    };
                    boxEvents.Add(boxEvent);
                }
                reader.Close();
                Logger.Trace("ROWS: " + boxEvents.Count);
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + cnSF.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
                throw new ApplicationException(cnSF.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }
            finally
            {
                Logger.Info("END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            }
            return boxEvents;
        }

        public static ValidationErrors Validate(BoxingSchedule boxingSchedule, BusinessSystemSession e1Session)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info("BEGIN: " + DateTime.Now.ToLocalTime());
            var validationErrors = new ValidationErrors();
            if (boxingSchedule.TransferStatus == "U")
            {
                var validationError = new ValidationError($"Plant {boxingSchedule.PlantCode}-{boxingSchedule.PlantNum} SF BoxHold unable to lock MTOSchedule {boxingSchedule.ScheduleNum}", "SF Validation", "Error", true);
                validationErrors.Add(validationError);
                boxingSchedule.ValidationStatus = validationError.Message;
            }
            else
            {
                if (!boxingSchedule.IsCompleted)
                {
                    var validationError = new ValidationError($"Plant {boxingSchedule.PlantCode}-{boxingSchedule.PlantNum} SF BoxHold MTOSchedule {boxingSchedule.ScheduleNum} not complete", "SF Validation", "Error", true);
                    validationErrors.Add(validationError);
                    boxingSchedule.ValidationStatus = validationError.Message;
                }
                else
                {
                    var msg = BoxingInteropManager.GetF580200sCountDynamic(e1Session, boxingSchedule.PlantNum, boxingSchedule.ScheduleNum, boxingSchedule.BoxItems);
                    var oldScheduleNumber = msg.Name;
                    var workOrdersCount = Convert.ToInt16(msg.Value);
                    if (workOrdersCount > 0)
                    {
                        var validationError = new ValidationError($"Plant {boxingSchedule.PlantCode}-{boxingSchedule.PlantNum} MTO Work Orders previously uploaded - {workOrdersCount} Work Orders found in E1 F580200 on {oldScheduleNumber}", "SF Validation", "Error", true);
                        validationErrors.Add(validationError);
                        boxingSchedule.ValidationStatus = validationError.Message;
                    }
                    else if (boxingSchedule.TransferStatus == "Y" || boxingSchedule.Transferred)
                    {
                        var validationError = new ValidationError($"Plant {boxingSchedule.PlantCode}-{boxingSchedule.PlantNum} SF BoxHold MTOSchedule {boxingSchedule.ScheduleNum} previously uploaded", "SF Validation", "Error", false);
                        validationErrors.Add(validationError);
                        boxingSchedule.ValidationStatus = validationError.Message;
                    }
                    else if (boxingSchedule.IncomingOrdersUnitCount == 0)
                    {
                        var validationError = new ValidationError($"Plant {boxingSchedule.PlantCode}-{boxingSchedule.PlantNum} SF MTOSchedule {boxingSchedule.ScheduleNum} not found in IncomingOrders", "SF Validation", "Error", true);
                        validationErrors.Add(validationError);
                        boxingSchedule.ValidationStatus = validationError.Message;
                    }
                    else if (boxingSchedule.UnitBoxCount == 0)
                    {
                        var validationError = new ValidationError($"Plant {boxingSchedule.PlantCode}-{boxingSchedule.PlantNum} SF MTOSchedule {boxingSchedule.ScheduleNum} not found in UnitBox", "SF Validation", "Error", true);
                        validationErrors.Add(validationError);
                        boxingSchedule.ValidationStatus = validationError.Message;
                    }

                    var isHDDCAW100Series = boxingSchedule.BoxItems.Any(i => i.IncomingOrdersShipToCustNum == 10472113); //Cardinal Hudson Andersen DC-100 series
                    if (!isHDDCAW100Series)
                    {
                        if (boxingSchedule.IncomingOrdersUnitCount > boxingSchedule.BoxItems.Count)
                        {
                            var validationError = new ValidationError($"Plant {boxingSchedule.PlantCode}-{boxingSchedule.PlantNum} SF IncomingOrders ({boxingSchedule.IncomingOrdersUnitCount} units, downloaded {boxingSchedule.IncomingOrdersMaxDownLoadDate}) has more units than BoxHold MTOSchedule {boxingSchedule.ScheduleNum} ({boxingSchedule.BoxItems.Count} units, written {boxingSchedule.UpdateDate})", "SF Validation", "Error", true);
                            validationErrors.Add(validationError);
                            boxingSchedule.ValidationStatus = validationError.Message;
                        }
                        else if (boxingSchedule.IncomingOrdersUnitCount < boxingSchedule.BoxItems.Count)
                        {
                            var validationError = new ValidationError($"Plant {boxingSchedule.PlantCode}-{boxingSchedule.PlantNum} SF IncomingOrders ({boxingSchedule.IncomingOrdersUnitCount} units, downloaded {boxingSchedule.IncomingOrdersMaxDownLoadDate}) has fewer units than BoxHold MTOSchedule {boxingSchedule.ScheduleNum} ({boxingSchedule.BoxItems.Count} units, written {boxingSchedule.UpdateDate})", "SF Validation", "Error", true);
                            validationErrors.Add(validationError);
                            boxingSchedule.ValidationStatus = validationError.Message;
                        }
                        else if (boxingSchedule.IncomingOrdersWorkOrderCount > boxingSchedule.WorkOrderCount)
                        {
                            var validationError = new ValidationError($"Plant {boxingSchedule.PlantCode}-{boxingSchedule.PlantNum} SF IncomingOrders ({boxingSchedule.IncomingOrdersWorkOrderCount} work orders, downloaded {boxingSchedule.IncomingOrdersMaxDownLoadDate}) has more work orders than BoxHold MTOSchedule {boxingSchedule.ScheduleNum} ({boxingSchedule.WorkOrderCount} work orders, written {boxingSchedule.UpdateDate})", "SF Validation", "Error", true);
                            validationErrors.Add(validationError);
                            boxingSchedule.ValidationStatus = validationError.Message;
                        }
                        else if (boxingSchedule.IncomingOrdersWorkOrderCount < boxingSchedule.WorkOrderCount)
                        {
                            var validationError = new ValidationError($"Plant {boxingSchedule.PlantCode}-{boxingSchedule.PlantNum} SF IncomingOrders ({boxingSchedule.IncomingOrdersWorkOrderCount} work orders, downloaded {boxingSchedule.IncomingOrdersMaxDownLoadDate}) has fewer work orders than BoxHold MTOSchedule {boxingSchedule.ScheduleNum} ({boxingSchedule.WorkOrderCount} work orders, written {boxingSchedule.UpdateDate})", "SF Validation", "Error", true);
                            validationErrors.Add(validationError);
                            boxingSchedule.ValidationStatus = validationError.Message;
                        }
                        else if (boxingSchedule.UnitBoxCount > boxingSchedule.BoxItems.Count)
                        {
                            var validationError = new ValidationError($"Plant {boxingSchedule.PlantCode}-{boxingSchedule.PlantNum} SF UnitBox ({boxingSchedule.UnitBoxCount} units, written {boxingSchedule.UnitBoxMaxTimeStamp}) has more units than BoxHold MTOSchedule {boxingSchedule.ScheduleNum} ({boxingSchedule.BoxItems.Count} units, written {boxingSchedule.UpdateDate})", "SF Validation", "Error", true);
                            validationErrors.Add(validationError);
                            boxingSchedule.ValidationStatus = validationError.Message;
                        }
                        else if (boxingSchedule.UnitBoxCount < boxingSchedule.BoxItems.Count)
                        {
                            var validationError = new ValidationError($"Plant {boxingSchedule.PlantCode}-{boxingSchedule.PlantNum} SF UnitBox ({boxingSchedule.UnitBoxCount} units, written {boxingSchedule.UnitBoxMaxTimeStamp}) has fewer units than BoxHold MTOSchedule {boxingSchedule.ScheduleNum} ({boxingSchedule.BoxItems.Count} units, written {boxingSchedule.UpdateDate})", "SF Validation", "Error", true);
                            validationErrors.Add(validationError);
                            boxingSchedule.ValidationStatus = validationError.Message;
                        }
                    }
                }
            }
            foreach (var validationError in validationErrors)
            {
                Logger.Trace(validationError);
            }
            Logger.Trace("CLOSE: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            return validationErrors;
        }

        public static bool RollBack(SqlTransaction shopFloorTran)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"BEGIN: {DateTime.Now.ToLocalTime()}");
            try
            {
                shopFloorTran.Rollback();
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {shopFloorTran.Connection.ConnectionString}  {e.Message}: {e.StackTrace}");
                throw new ApplicationException($"{shopFloorTran.Connection.ConnectionString}  {e.Message}: {e.StackTrace}");
            }
            finally
            {
                Logger.Info($"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }
            return true;
        }

        public static bool UpdateSFPreviouslyUploaded(SqlConnection cnSF, decimal scheduleNum)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"BEGIN: {DateTime.Now.ToLocalTime()}");
            bool isUpdated;
            try
            {
                var cmd = new SqlCommand
                {
                    Connection = cnSF,
                    CommandType = CommandType.Text,
                    CommandText = @"
                        UPDATE  bhh
                        SET     bhh.[TransferStatus] = 'Y'
                               ,bhh.[Transferred] = 1
                               ,bhh.[TransferMessage] = 'PreviouslyUploaded'
                        FROM    [dbo].[BoxHoldHeader] bhh
                        WHERE   bhh.[ScheduleNum] = @ScheduleNum
                    "
                };
                cmd.Parameters.Add("@ScheduleNum", SqlDbType.Int).Direction = ParameterDirection.Input;
                cmd.Parameters["@ScheduleNum"].Value = scheduleNum;
                var rowCount = cmd.ExecuteNonQuery();
                Logger.Trace($"ROWS: {rowCount}");
                cmd = new SqlCommand
                {
                    Connection = cnSF,
                    CommandType = CommandType.Text,
                    CommandText = @"
                        UPDATE  bh
                        SET     bh.[TransferStatus] = 'Y'
                               ,bh.[Transferred] = 1
                               ,bh.[TransferMessage] = 'PreviouslyUploaded'
                        FROM    [dbo].[BoxHold] bh
                        WHERE   bh.[ScheduleNum] = @ScheduleNum
                    "
                };
                cmd.Parameters.Add("@ScheduleNum", SqlDbType.Int).Direction = ParameterDirection.Input;
                cmd.Parameters["@ScheduleNum"].Value = scheduleNum;
                rowCount = cmd.ExecuteNonQuery();
                Logger.Trace("ROWS: " + rowCount);
                isUpdated = rowCount > 0;
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {cnSF.ConnectionString}  {e.Message}: {e.StackTrace}");
                throw new ApplicationException($"{cnSF.ConnectionString}  {e.Message}: {e.StackTrace}");
            }
            finally
            {
                Logger.Info($"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }
            return isUpdated;
        }

        public static void UpdateSFBoxHold(SqlConnection cnSF, BoxingSchedule boxingSchedule, short sfUpdateBatchSize)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"BEGIN: {DateTime.Now.ToLocalTime()}");
            var boxItems = boxingSchedule.BoxItems;
            var cmd = new SqlCommand
            {
                Connection = cnSF,
                CommandType = CommandType.Text
            };
            var cmdStringBldr = new StringBuilder();
            try
            {
                var rowCount = 0;
                foreach (var boxItem in boxItems)
                {
                    rowCount++;
                    var cmdStatement = $@"
                        UPDATE bh
                        SET bh.[Transferred] = 1
                               ,bh.[TransferredDate] = GETDATE()
                               ,bh.[TransferBatch] = '{boxItem.TransferBatchNum}'
                               ,bh.[TransferUniqueID] = {boxItem.UniqueID}
			                   ,bh.[TransferMessage] = ''
                        FROM   [dbo].[BoxHold] bh
                        WHERE bh.[BoxHoldID] = {boxItem.BoxHoldId};
                    ";
                    cmdStringBldr.Append(cmdStatement);
                    if (rowCount >= sfUpdateBatchSize)
                    {
                        cmd.CommandText = cmdStringBldr.ToString();
                        cmd.CommandTimeout = int.Parse(ConfigurationManager.AppSettings["BoxingQueryTimeout"]);
                        Logger.Trace(Logging.DbCmdString(cmd));
                        cmd.ExecuteNonQuery();
                        Logger.Trace($"ROWS: {rowCount}");
                        cmdStringBldr = new StringBuilder();
                        rowCount = 0;
                    }
                }
                if (rowCount > 0)
                {
                    cmd.CommandText = cmdStringBldr.ToString();
                    Logger.Trace(Logging.DbCmdString(cmd));
                    cmd.ExecuteNonQuery();
                    Logger.Trace($"ROWS: {rowCount}");
                }
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {cnSF.ConnectionString}  {e.Message}: {e.StackTrace}");
                throw new ApplicationException($"{cnSF.ConnectionString}  {e.Message}: {e.StackTrace}");
            }
            finally
            {
                Logger.Info($"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }
        }

        public static void UpdateSFBoxHoldOld(SqlConnection cnSF, SqlTransaction transSF, BoxingSchedule boxingSchedule, short sfUpdateBatchSize)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"BEGIN: {DateTime.Now.ToLocalTime()}");
            var boxItems = boxingSchedule.BoxItems;
            var cmd = new SqlCommand
            {
                Connection = cnSF,
                Transaction = transSF,
                CommandType = CommandType.Text
            };
            var cmdStringBldr = new StringBuilder();
            try
            {
                var rowCount = 0;
                foreach (var boxItem in boxItems)
                {
                    rowCount++;
                    var cmdStatement = $@"
                        UPDATE bh
                        SET bh.[Transferred] = 1
                               ,bh.[TransferredDate] = GETDATE()
                               ,bh.[TransferBatch] = '{boxItem.TransferBatchNum}'
                               ,bh.[TransferUniqueID] = {boxItem.UniqueID}
			                   ,bh.[TransferMessage] = ''
                        FROM   [dbo].[BoxHold] bh
                        WHERE bh.[ScheduleNum] = {boxItem.ScheduleNum}
                        AND bh.[BoxHoldID] = {boxItem.BoxHoldId};
                    ";
                    cmdStringBldr.Append(cmdStatement);
                    if (rowCount >= sfUpdateBatchSize)
                    {
                        cmd.CommandText = cmdStringBldr.ToString();
                        cmd.CommandTimeout = int.Parse(ConfigurationManager.AppSettings["BoxingQueryTimeout"]);
                        Logger.Trace(Logging.DbCmdString(cmd));
                        cmd.ExecuteNonQuery();
                        Logger.Trace($"ROWS: {rowCount}");
                        cmdStringBldr = new StringBuilder();
                        rowCount = 0;
                    }
                }
                if (rowCount > 0)
                {
                    cmd.CommandText = cmdStringBldr.ToString();
                    Logger.Trace(Logging.DbCmdString(cmd));
                    cmd.ExecuteNonQuery();
                    Logger.Trace($"ROWS: {rowCount}");
                }
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {cnSF.ConnectionString}  {e.Message}: {e.StackTrace}");
                throw new ApplicationException($"{cnSF.ConnectionString}  {e.Message}: {e.StackTrace}");
            }
            finally
            {
                Logger.Info($"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }
        }

        public static void UpdateSFBoxHeader(SqlConnection cnSF, BoxingSchedule boxingSchedule)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"BEGIN: {DateTime.Now.ToLocalTime()}");
            try
            {
                var cmd = new SqlCommand
                {
                    Connection = cnSF,
                    CommandType = CommandType.Text,
                    CommandText = @"
                        UPDATE  bhh
                        SET     bhh.[Transferred] = 1
			                    ,bhh.[TransferStatus] = 'Y'
                                ,bhh.[TransferredDate] = GETDATE()
                                ,bhh.[TransferBatch] = @TransferBatchNum
                        FROM    [dbo].[BoxHoldHeader] bhh
                        WHERE   bhh.[ScheduleNum] = @ScheduleNum
                    "
                };
                cmd.Parameters.Add("@ScheduleNum", SqlDbType.Int).Direction = ParameterDirection.Input;
                cmd.Parameters.Add("@TransferBatchNum", SqlDbType.VarChar).Direction = ParameterDirection.Input;
                cmd.Parameters["@ScheduleNum"].Value = boxingSchedule.ScheduleNum;
                cmd.Parameters["@TransferBatchNum"].Value = boxingSchedule.TransferBatchNum;
                cmd.CommandTimeout = int.Parse(ConfigurationManager.AppSettings["BoxingQueryTimeout"]);
                Logger.Trace(Logging.DbCmdString(cmd));
                var rowCount = cmd.ExecuteNonQuery();
                Logger.Trace($"ROWS: {rowCount}");
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {cnSF.ConnectionString}  {e.Message}: {e.StackTrace}");
                throw new ApplicationException($"{cnSF.ConnectionString}  {e.Message}: {e.StackTrace}");
            }
            finally
            {
                Logger.Info($"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }
        }

        public static void UpdateSFBoxHeaderOld(SqlConnection cnSF, SqlTransaction tranSF, BoxingSchedule boxingSchedule)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"BEGIN: {DateTime.Now.ToLocalTime()}");
            try
            {
                var cmd = new SqlCommand
                {
                    Connection = cnSF,
                    Transaction = tranSF,
                    CommandType = CommandType.Text,
                    CommandText = @"
                        UPDATE  bhh
                        SET     bhh.[Transferred] = 1
			                    ,bhh.[TransferStatus] = 'Y'
                                ,bhh.[TransferredDate] = GETDATE()
                                ,bhh.[TransferBatch] = @TransferBatchNum
                        FROM    [dbo].[BoxHoldHeader] bhh
                        WHERE   bhh.[ScheduleNum] = @ScheduleNum
                    "
                };
                cmd.Parameters.Add("@ScheduleNum", SqlDbType.Int).Direction = ParameterDirection.Input;
                cmd.Parameters.Add("@TransferBatchNum", SqlDbType.VarChar).Direction = ParameterDirection.Input;
                cmd.Parameters["@ScheduleNum"].Value = boxingSchedule.ScheduleNum;
                cmd.Parameters["@TransferBatchNum"].Value = boxingSchedule.TransferBatchNum;
                cmd.CommandTimeout = int.Parse(ConfigurationManager.AppSettings["BoxingQueryTimeout"]);
                Logger.Trace(Logging.DbCmdString(cmd));
                var rowCount = cmd.ExecuteNonQuery();
                Logger.Trace($"ROWS: {rowCount}");
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {cnSF.ConnectionString}  {e.Message}: {e.StackTrace}");
                throw new ApplicationException($"{cnSF.ConnectionString}  {e.Message}: {e.StackTrace}");
            }
            finally
            {
                Logger.Info($"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }
        }
    }
}