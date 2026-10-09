using System;
using System.Configuration;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.ServiceModel;
using System.Text;
using IGServices.Common.Managers.E1;
using IGServices.Common.Managers.ShopFloor;
using IGServices.Common.Types.E1;
using IGServices.Common.Types.Enterprise;
using IGServices.Common.Types.Response;
using IGServices.Common.Types.ShopFloor;
using IGServices.Common.Utilities;
using IBM.Data.DB2.iSeries;
using IGServices.SubscriptionSvc;
using NLog;
using Schedule = IGServices.Common.Types.Enum.Schedule;

// ReSharper disable InconsistentNaming

namespace IGServices.Common.Managers.Enterprise
{
    public class MTOScheduleManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static MTOSchedule BuildMTOSchedule(BusinessSystemSession e1Session, Plant plant, decimal scheduleNum, decimal scheduleDateE1, string batchNum, out ProcessResponse returnResponse)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"PLANT: {plant.PlantNum}  SCHEDULE: {scheduleNum} BEGIN: {DateTime.Now.ToLocalTime()}");
            var processResponse = new ProcessResponse("Build MTO Schedule", true);
            processResponse.RowsAffected = 1;
            var f580020 = new F580020();
            var mtoSchedule = new MTOSchedule(plant.PlantNum, scheduleNum);
            var mtoScheduleEvent = new MTOScheduleEvent();
            try
            {
                var step1GetMTOScheduleEvent = new ProcessStep("Step 1. Build MTO Schedule Event from F580020");
                try
                {
                    f580020 = F580020Manager.GetF580020(e1Session, plant.PlantNum, scheduleNum, scheduleDateE1, batchNum);
                    step1GetMTOScheduleEvent.RowsAffected = 1;
                    if (!step1GetMTOScheduleEvent.HasErrors() && f580020.S6Z58SCD > 0)
                    {
                        mtoScheduleEvent = MTOScheduleEventsManager.BuildMTOScheduleEventFromF580020(f580020);
                        step1GetMTOScheduleEvent.RowsAffected = 1;
                    }
                }
                catch (Exception e)
                {
                    step1GetMTOScheduleEvent.Errors.Add(new Error(e));
                }
                finally
                {
                    step1GetMTOScheduleEvent.Done();
                    processResponse.AddStep(step1GetMTOScheduleEvent);
                }

                if (!step1GetMTOScheduleEvent.HasErrors() && f580020.S6Z58SCD > 0)
                {
                    var step2BuildMTOScheduleHeader = new ProcessStep("Step 2. Build MTO Schedule Header");
                    try
                    {
                        mtoSchedule = BuildMTOScheduleHeader(e1Session, plant.PlantNum, scheduleNum, f580020.S6Z58SCD);
                        mtoSchedule.E1ScheduleData.F580020 = f580020;
                        mtoSchedule.ScheduleDateE1 = mtoSchedule.E1ScheduleData.F580020.S6Z58SCD;
                        mtoSchedule.BatchNum = mtoSchedule.E1ScheduleData.F580020.S6EDBT;
                        mtoSchedule.MTOScheduleEvent = mtoScheduleEvent;
                        mtoSchedule.ScheduleDate = Utilities.Common.CastToDateTime(mtoSchedule.ScheduleDateE1);
                        step2BuildMTOScheduleHeader.RowsAffected = 1;
                    }
                    catch (Exception e)
                    {
                        step2BuildMTOScheduleHeader.Errors.Add(new Error(e));
                    }
                    finally
                    {
                        step2BuildMTOScheduleHeader.Done();
                        processResponse.AddStep(step2BuildMTOScheduleHeader);
                    }

                    if (!step2BuildMTOScheduleHeader.HasErrors() && f580020.S6Z58SCD > 0)
                    {
                        var step3BuildMTOScheduleDetail = new ProcessStep("Step 3. Build MTO Schedule Detail");
                        try
                        {
                            mtoSchedule.MTOScheduleLines = BuildMTOScheduleDetail(e1Session, plant.PlantNum, scheduleNum, f580020.S6Z58SCD);
                            mtoSchedule.E1ScheduleData.F580020 = f580020;
                            mtoSchedule.ScheduleDateE1 = mtoSchedule.E1ScheduleData.F580020.S6Z58SCD;
                            mtoSchedule.BatchNum = mtoSchedule.E1ScheduleData.F580020.S6EDBT;
                            mtoSchedule.MTOScheduleEvent = mtoScheduleEvent;
                            mtoSchedule.ScheduleDate = Utilities.Common.CastToDateTime(mtoSchedule.ScheduleDateE1);
                            step3BuildMTOScheduleDetail.RowsAffected = 1;
                        }
                        catch (Exception e)
                        {
                            step3BuildMTOScheduleDetail.Errors.Add(new Error(e));
                        }
                        finally
                        {
                            step3BuildMTOScheduleDetail.Done();
                            processResponse.AddStep(step3BuildMTOScheduleDetail);
                        }

                        if (!step3BuildMTOScheduleDetail.HasErrors() && mtoSchedule.ScheduleNum > 0 && !string.IsNullOrEmpty(mtoSchedule.E1ScheduleData.F580020.S6EDBT))
                        {
                            var step3GetF580021s = new ProcessStep("Step 4. Get MTO Schedule Lines F580021s");
                            try
                            {
                                mtoSchedule.E1ScheduleData.F580021s = F580021Manager.GetF580021s(e1Session, plant.PlantNum, scheduleNum, mtoSchedule.BatchNum);
                                step3GetF580021s.RowsAffected = mtoSchedule.E1ScheduleData.F580021s.Count;
                            }
                            catch (Exception e)
                            {
                                step3GetF580021s.Errors.Add(new Error(e));
                            }
                            finally
                            {
                                step3GetF580021s.Done();
                                processResponse.AddStep(step3GetF580021s);
                            }

                            if (!step3GetF580021s.HasErrors() && mtoSchedule.E1ScheduleData.F580021s.Any())
                            {
                                var step4GetF580024s = new ProcessStep("Step 5. Get MTO Schedule Lines F580024s");
                                try
                                {
                                    mtoSchedule.E1ScheduleData.F580024s = F580024Manager.GetF580024s(e1Session, plant.PlantNum, scheduleNum, mtoSchedule.BatchNum);
                                    step4GetF580024s.RowsAffected = mtoSchedule.E1ScheduleData.F580024s.Count;
                                }
                                catch (Exception e)
                                {
                                    step4GetF580024s.Errors.Add(new Error(e));
                                }
                                finally
                                {
                                    step4GetF580024s.Done();
                                    processResponse.AddStep(step4GetF580024s);
                                }

                                if (!step4GetF580024s.HasErrors() && mtoSchedule.E1ScheduleData.F580024s.Any())
                                {
                                    var step5GetF584715s = new ProcessStep("Step 6. Get MTO Schedule 584715s");
                                    try
                                    {
                                        mtoSchedule.E1ScheduleData.F584715Ls = F584715Manager.GetF584715Ls(e1Session, plant.PlantNum, scheduleNum, mtoSchedule.BatchNum);
                                        step5GetF584715s.RowsAffected = mtoSchedule.E1ScheduleData.F584715Ls.Count;
                                        if (!step5GetF584715s.HasErrors())
                                        {
                                            mtoSchedule.E1ScheduleData.F584715Es = F584715Manager.GetF584715Es(e1Session, plant.PlantNum, scheduleNum, mtoSchedule.BatchNum);
                                            step5GetF584715s.RowsAffected = mtoSchedule.E1ScheduleData.F584715Es.Count;
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        step5GetF584715s.Errors.Add(new Error(e));
                                    }
                                    finally
                                    {
                                        step5GetF584715s.Done();
                                        processResponse.AddStep(step5GetF584715s);
                                    }

                                    if (!step5GetF584715s.HasErrors() && (mtoSchedule.E1ScheduleData.F584715Es.Any() || mtoSchedule.E1ScheduleData.F584715Ls.Any()))
                                    {
                                        var step6GetF4715s = new ProcessStep("Step 7. Get MTO Schedule F4715s");
                                        try
                                        {
                                            var f4715Mgr = new F4715Manager(e1Session);
                                            foreach (var f580021 in mtoSchedule.E1ScheduleData.F580021s)
                                            {
                                                var s7Rorn = Data.ParseStringToDecimal(f580021.S7RORN);
                                                var f4715Values = f4715Mgr.GetF4715s(e1Session, plant.CompanyNum, s7Rorn, f580021.S7RCTO, f580021.S7LNID);
                                                mtoSchedule.E1ScheduleData.F4715s.AddRange(f4715Values);
                                            }
                                            step6GetF4715s.RowsAffected = mtoSchedule.E1ScheduleData.F4715s.Count;
                                            if (mtoSchedule.E1ScheduleData.F4715s.Any())
                                            {
                                                mtoSchedule.E1ScheduleData.F4715Pivots = F4715Manager.PivotF4715s(mtoSchedule.E1ScheduleData.F4715s);
                                                step6GetF4715s.RowsAffected = mtoSchedule.E1ScheduleData.F4715Pivots.Count;
                                            }
                                        }
                                        catch (Exception e)
                                        {
                                            step6GetF4715s.Errors.Add(new Error(e));
                                        }
                                        finally
                                        {
                                            step6GetF4715s.Done();
                                            processResponse.AddStep(step6GetF4715s);
                                        }
                                    }

                                    var step7GetF580220FirstRacks = new ProcessStep($@"Step {processResponse.ProcessSteps.Count + 1}. Get F580220 First Racks");
                                    try
                                    {
                                        mtoSchedule.E1ScheduleData.F580220s = F580220Manager.GetF580220FirstRacks(e1Session, plant.PlantNum, scheduleNum, mtoSchedule.ScheduleDateE1, mtoSchedule.BatchNum);
                                        step7GetF580220FirstRacks.RowsAffected = mtoSchedule.E1ScheduleData.F580220s.Count;
                                    }
                                    catch (Exception e)
                                    {
                                        step7GetF580220FirstRacks.Errors.Add(new Error(e));
                                    }
                                    finally
                                    {
                                        step7GetF580220FirstRacks.Done();
                                        processResponse.AddStep(step7GetF580220FirstRacks);
                                    }

                                    var stepOrigSalesLines = new ProcessStep("E1 Schedule - Related Original Sales Lines");
                                    try
                                    {
                                        mtoSchedule.OriginalSalesLineAttributes = SalesLineAttributeManager.GetOriginalSalesLineAttributes(e1Session, plant, mtoSchedule.BatchNum);
                                        stepOrigSalesLines.RowsAffected = mtoSchedule.OriginalSalesLineAttributes.Count;
                                    }
                                    catch (Exception e)
                                    {
                                        stepOrigSalesLines.Errors.Add(new Error(e));
                                    }
                                    stepOrigSalesLines.Done();
                                    processResponse.AddStep(stepOrigSalesLines);

                                    var step8BuildMTOIncomingOrderLines = new ProcessStep($@"Step {processResponse.ProcessSteps.Count + 1}. Build MTO Incoming Order Lines");
                                    try
                                    {
                                        mtoSchedule.MTOIncomingOrderLines = MTOIncomingOrderManager.BuildMTOIncomingOrderLines(mtoSchedule, e1Session);
                                        step8BuildMTOIncomingOrderLines.RowsAffected = mtoSchedule.MTOIncomingOrderLines.Count;
                                    }
                                    catch (Exception e)
                                    {
                                        step8BuildMTOIncomingOrderLines.Errors.Add(new Error(e));
                                    }
                                    finally
                                    {
                                        step8BuildMTOIncomingOrderLines.Done();
                                        processResponse.AddStep(step8BuildMTOIncomingOrderLines);
                                    }
                                } // if (mtoSchedule.E1ScheduleData.F580024s.Any())
                                else
                                {
                                    var validationError = new ValidationError($"E1 no F580024 records found", "E1 Validation", "Error", true);
                                    mtoSchedule.ValidationErrors.Add(validationError);
                                    mtoSchedule.ValidationStatus = validationError.Message;
                                }
                            } // if (mtoSchedule.E1ScheduleData.F580021s.Any())
                            else
                            {
                                var validationError = new ValidationError($"E1 no F580021 records found", "E1 Validation", "Error", true);
                                mtoSchedule.ValidationErrors.Add(validationError);
                                mtoSchedule.ValidationStatus = validationError.Message;
                            }
                        } // if (!step2BuildMTOScheduleHeader.HasErrors() && f580020.S6Z58SCD > 0)
                    } // if (!step2BuildMTOScheduleHeader.HasErrors() && f580020.S6Z58SCD > 0)
                    else
                    {
                        var validationError = new ValidationError($"E1 no F5834A01 records found", "E1 Validation", "Error", true);
                        mtoSchedule.ValidationErrors.Add(validationError);
                        mtoSchedule.ValidationStatus = validationError.Message;
                    }
                } // if (!step1GetRecentScheduleDateE1.HasErrors() && scheduleDateE1 > 0)
                else
                {
                    var validationError = new ValidationError($"E1 no F580020 records found", "E1 Validation", "Error", true);
                    mtoSchedule.ValidationErrors.Add(validationError);
                    mtoSchedule.ValidationStatus = validationError.Message;
                }
            }
            catch (Exception e)
            {
                processResponse.Errors.Add(new Error(e));
                throw new FaultException(e.Message);
            }
            finally
            {
                Logger.Info($"PLANT: {plant.PlantNum} SCHEDULE: {scheduleNum} END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
                processResponse.Done();
                returnResponse = processResponse;
            }
            return mtoSchedule;
        }

        public static void SummarizeMTOSchedule(ProcessResponse response, MTOSchedule mtoSchedule)
        {
            var stepSummarize = new ProcessStep("Schedule Summary Collections");
            try
            {
                var scheduleCustomerLines =
                    mtoSchedule.MTOIncomingOrderLines.GroupBy(x => new { x.CustBillToNum, x.ProductType })
                        .Select(
                            ng =>
                                new
                                {
                                    CustNum = ng.Key.CustBillToNum,
                                    ng.Key.ProductType,
                                    CustName = ng.Select(x => x.CustName).FirstOrDefault()?.Trim(),
                                    ScheduleLines = (ng.Count()),
                                    SalesOrders = (ng.Select(x => x.SOrderNum).Distinct().Count()),
                                    WorkOrders = (ng.Select(x => x.WOrderNum).Distinct().Count()),
                                    UnitsQty = (ng.Sum(x => x.Quantity)),
                                    MinRequestedDate = ng.Select(x => x.CustomerDate).Min(),
                                    MaxRequestedDate = ng.Select(x => x.CustomerDate).Max()
                                })
                        .ToList();
                response.SerializedObjects.Add("ScheduleCustomerLine", scheduleCustomerLines, "ScheduleCustomerLines");

                var headerSummary = new
                {
                    Location = $@"{response.Plant.PlantCode} - {response.Plant.PlantNum}",
                    mtoSchedule.ScheduleNum,
                    mtoSchedule.ScheduleDateE1,
                    ShopFloorSystem = mtoSchedule.E1ScheduleData.F580020.S6Z58SFS,
                    ShopFloorSystemName = mtoSchedule.E1ScheduleData.F580020.S6Z58SFS_NAME,
                    mtoSchedule.BatchNum,
                    Customers = Convert.ToInt16(mtoSchedule.MTOIncomingOrderLines.Select(x => x.CustNum).Distinct().Count()),
                    SalesOrders = Convert.ToInt16(mtoSchedule.MTOIncomingOrderLines.Select(x => x.SOrderNum).Distinct().Count()),
                    Lines = Convert.ToInt16(mtoSchedule.MTOIncomingOrderLines.Count),
                    WorkOrders = Convert.ToInt16(mtoSchedule.MTOIncomingOrderLines.Select(x => x.WOrderNum).Distinct().Count()),
                    Units = Convert.ToInt16(mtoSchedule.MTOIncomingOrderLines.Select(x => x.Quantity).Sum()),
                    mtoSchedule.DueDate,
                    HasErrors = response.HasErrors() || mtoSchedule.ValidationErrors.Any()
                };
                response.SerializedObjects.Add(new SerializedObject("SummaryNode", XmlSerializationUtility.SerializeAnonymousType(headerSummary, "Header")));
                stepSummarize.RowsAffected = mtoSchedule.MTOIncomingOrderLines.Count;
            }
            catch (Exception e)
            {
                stepSummarize.Errors.Add(new Error(e));
            }
            finally
            {
                stepSummarize.Done();
                response.ProcessSteps.Add(stepSummarize);
            }
        }

        /******************************************************************
         * Get MTOSchedule Lines
         ******************************************************************/
        private static iDB2Command GetMTOScheduleDetail()
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = @"
                    SELECT
                        S5Z58SCSN,
                        S5Z58SCSTS,
                        S5DOCO,
                        S5DCTO,
					    S5RKCO,
                        S5ODOC,
                        S5RCTO,
                        S5LNID,
                        S5RORN,
                        S5RLLN,
                        S5SHAN,
                        S5ALPH,
                        S5VR01,
                        S5VR02,
                        S5ITM,
                        S5LITM,
                        S5DSC1,
                        S5DSC2,
                        S5Z58QTY,
                        S5UM,
                        S5DRQJ,
                        S5RSDJ,
                        S5RSDT,
                        S5Z58CSQNO,
                        S5Z58GSQNO,
                        S5Z58PT,
                        S5Z58IGLN,
                        S5Z58BOXP,
                        S5Z58BGEE,
                        S5Z58BGSZ,
                        S5Z58L1RG,
                        S5Z58L2RG,
                        S5Z58L3RG,
                        S5Z58MSG1,
                        S5Z58MSG2,
                        S5Z58CMT1,
                        S5Z58CMT2,
                        S5Z58CMT3,
                        BDZ58IGLN,
                        BDZ58CCV01,
                        BDZ58CCV02,
                        BDZ58CCV03
                    FROM
                        F5834A11     
                        LEFT OUTER JOIN F580232 
                            ON S5DOCO = BDZ58WODC
                    WHERE
                        S5MCU = ? AND
                        S5Z58SCN = ? AND
                        S5Z58SCD = ?
                    ORDER BY S5Z58SCSN
                    FOR READ ONLY"
            };
            cmd.Parameters.Add(new iDB2Parameter("PlantNum", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("ScheduleNum", iDB2DbType.iDB2Decimal));
            cmd.Parameters["ScheduleNum"].Precision = 5;
            cmd.Parameters["ScheduleNum"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("ScheduleDateId", iDB2DbType.iDB2Decimal));
            cmd.Parameters["ScheduleDateId"].Precision = 8;
            cmd.Parameters["ScheduleDateId"].Scale = 0;
            return cmd;
        }

        public static MTOScheduleLines BuildMTOScheduleDetail(BusinessSystemSession e1Session, string plantNum, decimal scheduleNum, decimal scheduleDateId)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"PLANT: {plantNum}  SCHEDULE: {scheduleNum} BEGIN: {DateTime.Now.ToLocalTime()}");
            var lines = new MTOScheduleLines();
            try
            {
                var cmd = GetMTOScheduleDetail();
                cmd.Connection = e1Session.DbConnection;
                cmd.Parameters[0].Value = plantNum.PadLeft(12);
                cmd.Parameters[1].Value = scheduleNum;
                cmd.Parameters[2].Value = scheduleDateId;
                Logger.Trace(Logging.DbCmdString(cmd));
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
                while (reader.Read())
                {
                    var line = new MTOScheduleLine
                    {
                        WorkOrderNum = CastDbNull.To<decimal>(reader["S5DOCO"], 0),
                        ScheduleSeqNum = CastDbNull.To<decimal>(reader["S5Z58SCSN"], 0),
                        Quantity = CastDbNull.To<decimal>(reader["S5Z58QTY"], 0),
                        IgLine = CastDbNull.To(reader["S5Z58IGLN"], string.Empty).Trim(),
                        BoxPriority = CastDbNull.To(reader["S5Z58BOXP"], string.Empty).Trim(),
                        Bungee = CastDbNull.To(reader["S5Z58BGEE"], string.Empty).Trim(),
                        BungeeSize = CastDbNull.To<decimal>(reader["S5Z58BGSZ"], 0),
                        SchedulerMsg1 = CastDbNull.To(reader["S5Z58MSG1"], string.Empty).Trim(),
                        SchedulerMsg2 = CastDbNull.To(reader["S5Z58MSG2"], string.Empty).Trim(),
                        Comment1 = CastDbNull.To(reader["S5Z58CMT1"], string.Empty).Trim(),
                        Comment2 = CastDbNull.To(reader["S5Z58CMT2"], string.Empty).Trim(),
                        Comment3 = "",
                        ReleaseDate = Utilities.Common.CastToDateTime(CastDbNull.To<decimal>(reader["S5RSDJ"], 0)),
                        RequestedDate = Utilities.Common.CastToDateTime(CastDbNull.To<decimal>(reader["S5DRQJ"], 0)),
                        Value01 = CastDbNull.To(reader["BDZ58CCV01"], string.Empty).Trim(),
                        Value02 = CastDbNull.To(reader["BDZ58CCV02"], string.Empty).Trim(),
                        Value03 = CastDbNull.To(reader["BDZ58CCV03"], string.Empty).Trim(),
                        ShipToCustNum = CastDbNull.To<decimal>(reader["S5SHAN"], 0),
                    };
                    lines.Add(line);
                }
                reader.Close();
                Logger.Trace($"ROWS: {lines.Count}");
            }
            catch (Exception e)
            {
                Logger.Error($@"ERROR: {e1Session.DbConnection.ConnectionString}  {e.Message}: {e.StackTrace}");
                throw new ApplicationException($@"{e1Session.DbConnection.ConnectionString}  {e.Message}: {e.StackTrace}");
            }
            finally
            {
                Logger.Info($"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }
            return lines;
        }

        public static ValidationErrors Validate(MTOSchedule mtoSchedule, BusinessSystemSession e1Session) //, SqlConnection cnCP)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"BEGIN: {DateTime.Now.ToLocalTime()}");
            var validationErrors = new ValidationErrors();
            var plantCode = PlantManager.GetPlant(mtoSchedule.PlantNum).PlantCode;
            try
            {
                if (!mtoSchedule.E1ScheduleData.F580021s.Any())
                {
                    var validationError = new ValidationError($"{plantCode} - {mtoSchedule.PlantNum} Sched {mtoSchedule.ScheduleNum} E1: no F580021 records found", "E1 Validation", "Error", true);
                    validationErrors.Add(validationError);
                    mtoSchedule.ValidationStatus = validationError.Message;
                    mtoSchedule.TransferStatus = "E";
                }
                else if (!mtoSchedule.E1ScheduleData.F580024s.Any())
                {
                    var validationError = new ValidationError($"{plantCode} - {mtoSchedule.PlantNum} Sched {mtoSchedule.ScheduleNum} E1: no F580024 records found", "E1 Validation", "Error", true);
                    validationErrors.Add(validationError);
                    mtoSchedule.ValidationStatus = validationError.Message;
                    mtoSchedule.TransferStatus = "E";
                }
                else if (mtoSchedule.E1ScheduleData.F580021s.Count != mtoSchedule.E1ScheduleData.F580024s.Count)
                {
                    var validationError = new ValidationError($"{plantCode} - {mtoSchedule.PlantNum} Sched {mtoSchedule.ScheduleNum} E1: F580021 record count does not match F580024 record count", "E1 Validation", "Error", true);
                    validationErrors.Add(validationError);
                    mtoSchedule.ValidationStatus = validationError.Message;
                    mtoSchedule.TransferStatus = "E";
                }
                else
                {
                    var prodLitesCount = 0; //GetProdLitesCountIncomingOrdersDynamic(cnCP, mtoSchedule.MTOIncomingOrderLines);  // This validatiion is not currently used.  Always returns 0.
                    if (prodLitesCount != 0)
                    {
                        var validationError = new ValidationError($@"{plantCode} - {mtoSchedule.PlantNum} Sched {mtoSchedule.ScheduleNum} CP: {prodLitesCount} work orders on this MTO Schedule already have production records in CP", "CP Validation", "Warning", false);
                        validationErrors.Add(validationError);
                        mtoSchedule.ValidationStatus = validationError.Message;
                        mtoSchedule.TransferStatus = "E";
                    }
                    else
                    {
                        var producedWorkOrdersCount = WorkOrderManager.GetE1CompletedWorkOrdersDynamic(e1Session, mtoSchedule.MTOIncomingOrderLines);
                        if (producedWorkOrdersCount != 0)
                        {
                            var validationError = new ValidationError($@"{plantCode} - {mtoSchedule.PlantNum} Sched {mtoSchedule.ScheduleNum} E1: {producedWorkOrdersCount} work orders on this MTO Schedule already have Status > 92", "E1 Validation", "Warning", false);
                            validationErrors.Add(validationError);
                            mtoSchedule.ValidationStatus = validationError.Message;
                            mtoSchedule.TransferStatus = "E";
                        }
                        else
                        {
                            var missingDueDateCount = mtoSchedule.E1ScheduleData.F580021s.Count(f580021 => f580021.S7Z58SCDDJ == 0);
                            if (missingDueDateCount != 0)
                            {
                                var validationError = new ValidationError($@"{plantCode} - {mtoSchedule.PlantNum} Sched {mtoSchedule.ScheduleNum} E1: {missingDueDateCount} work orders on this MTO Schedule have a missing Due Date", "E1 Validation", "Error", true);
                                validationErrors.Add(validationError);
                                mtoSchedule.ValidationStatus = validationError.Message;
                                mtoSchedule.TransferStatus = "E";
                            }
                            else
                            {
                                var zeroQuantityCount = mtoSchedule.E1ScheduleData.F580021s.Count(f580021 => f580021.S7Z58QTY < 1);
                                if (zeroQuantityCount != 0)
                                {
                                    var validationError = new ValidationError($@"{plantCode} - {mtoSchedule.PlantNum} Sched {mtoSchedule.ScheduleNum} E1: {missingDueDateCount} work orders on this MTO Schedule have a Quantity < 1", "E1 Validation", "Error", true);
                                    validationErrors.Add(validationError);
                                    mtoSchedule.ValidationStatus = validationError.Message;
                                    mtoSchedule.TransferStatus = "E";
                                }
                            }
                            //else
                            //{
                            //    if (mtoSchedule.E1ScheduleData.F580020.S6EDSP == "Y")
                            //    {
                            //        validationErrors.Add("This schedule has already been downloaded to ShopFloor");
                            //        mtoSchedule.ValidationStatus = "This schedule has already been downloaded to ShopFloor";
                            //    }
                            //}
                        }
                    }
                }
            }
            catch (Exception e)
            {
                mtoSchedule.TransferStatus = "E";
                Logger.Error($@"ERROR: {e1Session.DbConnection.ConnectionString}  {e.Message}: {e.StackTrace}");
                throw new ApplicationException($@"{e1Session.DbConnection.ConnectionString}  {e.Message}: {e.StackTrace}");
            }
            finally
            {
                if (!validationErrors.Any()) mtoSchedule.ValidationStatus = "Valid";
                Logger.Info($"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }
            return validationErrors;
        }

        public static int GetProdLitesCountIncomingOrdersDynamic(SqlConnection cnCP, MTOIncomingOrderLines mtoIncomingOrderLines)
        {
            if (!mtoIncomingOrderLines.Any())
            {
                return 0;
            }
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"BEGIN: {DateTime.Now.ToLocalTime()}");
            var prodLitesCount = 0;
            try
            {
                var inString = new StringBuilder();
                foreach (var line in mtoIncomingOrderLines)
                {
                    inString.Append(line.WOrderNum + ",");
                }
                if (inString.Length > 0) inString.Length -= 1; //Strip off trailing comma.
                var cmd = GetProdLitesByWorkOrder(inString.ToString());
                cmd.Connection = cnCP;
                Logger.Trace(Logging.DbCmdString(cmd));
                //IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
                //while (reader.Read())
                //{
                //    prodLitesCount = CastDbNull.To(reader["RECS"], 0);
                //}
                //reader.Close();
                Logger.Trace($"ROWS: {prodLitesCount}");
            }
            catch (Exception e)
            {
                Logger.Error($@"ERROR: {cnCP.ConnectionString}  {e.Message}: {e.StackTrace}");
                throw new ApplicationException($@"{cnCP.ConnectionString}  {e.Message}: {e.StackTrace}");
            }
            finally
            {
                Logger.Info($"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }
            return prodLitesCount;
        }

        public static SqlCommand GetProdLitesByWorkOrder(string workOrders)
        {
            var cmd = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = $@"
                    SELECT  cpl.[WorkOrderNum]
                           ,ps.[ScheduleNum]
                           ,ps.[ProdScheduleTypeID]
                           ,ps.[CompletedDate]
                           ,ps.[CanceledDate]
                           ,ps.[LastImportDate]
                           ,ps.[InActive]
                           ,ps.[InActiveTS]
                           ,ps.[SoftDeleted]
                    FROM    [ClearPath].[ProdSchedule] ps
                            INNER JOIN [ClearPathReporting].[ProdLites] cpl ON cpl.[CurrentScheduleNum] = ps.[ScheduleNum]
                    WHERE   cpl.[WorkOrderNum] IN ( {workOrders} )
                            AND ps.[CompletedDate] IS NULL
                            AND ps.[CanceledDate] IS NULL
                            AND ps.[InActive] = 0
                            AND ps.[SoftDeleted] = 0"
            };
            return cmd;
        }

        /******************************************************************
        * Get MTOSchedule
        ******************************************************************/
        private static iDB2Command GetMTOScheduleBySCD()
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = $@"
                    SELECT
                        S1.S1MCU,
                        S1.S1Z58SCN,
                        S1.S1Z58SCD,
                        S1.S1Z58SFS,
                        DRDL01,
                        S1.S1Z58SCTYP,
                        S1.S1Z58SCSTS,
                        S1.S1Z58SCDDJ,
                        S1.S1Z58SAREA
                    FROM
                        F5834A01 S1
                        INNER JOIN {ConfigurationManager.AppSettings["ControlLib"]}.F0005
                        ON  DRSY = '58' AND
                            DRRT = 'SY' AND
                            TRIM(DRKY) = S1.S1Z58SFS
                    WHERE
                        S1.S1MCU = ? AND
                        S1.S1Z58SCN = ? AND
                        S1.S1Z58SCD = ?
                    FOR READ ONLY"
            };
            cmd.Parameters.Add(new iDB2Parameter("PlantNum", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("ScheduleNum", iDB2DbType.iDB2Decimal));
            cmd.Parameters["ScheduleNum"].Precision = 5;
            cmd.Parameters["ScheduleNum"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("S1Z58SCD", iDB2DbType.iDB2Decimal));
            cmd.Parameters["S1Z58SCD"].Precision = 6;
            cmd.Parameters["S1Z58SCD"].Scale = 0;
            return cmd;
        }

        public static MTOSchedule BuildMTOScheduleHeader(BusinessSystemSession e1Session, string plantNum, decimal scheduleNum, decimal scheduleDateE1)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"BEGIN: {DateTime.Now.ToLocalTime()}");
            var schedule = new MTOSchedule(plantNum, scheduleNum);
            try
            {
                var cmd = GetMTOScheduleBySCD();
                cmd.Connection = e1Session.DbConnection;
                cmd.Parameters["PlantNum"].Value = plantNum.PadLeft(12);
                cmd.Parameters["ScheduleNum"].Value = scheduleNum;
                cmd.Parameters["S1Z58SCD"].Value = scheduleDateE1;
                Logger.Trace(Logging.DbCmdString(cmd));
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
                var rowCount = 0;
                while (reader.Read())
                {
                    schedule.PlantNum = plantNum;
                    schedule.ScheduleNum = CastDbNull.To<decimal>(reader["S1Z58SCN"], 0);
                    schedule.ScheduleDateE1 = CastDbNull.To<decimal>(reader["S1Z58SCD"], 0);
                    schedule.ScheduleDate = Utilities.Common.CastToDateTime(CastDbNull.To<decimal>(reader["S1Z58SCD"], 0));
                    schedule.DueDate = Utilities.Common.CastToDateTime(CastDbNull.To<decimal>(reader["S1Z58SCDDJ"], 0));
                    schedule.ShopFloorCode = CastDbNull.To(reader["S1Z58SFS"], string.Empty).Trim();
                    schedule.ShopFloorName = CastDbNull.To(reader["DRDL01"], string.Empty).Trim();
                    schedule.ScheduleStatus = CastDbNull.To(reader["S1Z58SCSTS"], string.Empty).Trim();
                    schedule.ScheduleType = CastDbNull.To(reader["S1Z58SCTYP"], string.Empty).Trim();
                    rowCount++;
                }
                reader.Close();
                Logger.Trace($"ROWS: {rowCount}");
            }
            catch (Exception e)
            {
                Logger.Error($@"ERROR: {e1Session.DbConnection.ConnectionString}  {e.Message}: {e.StackTrace}");
                throw new ApplicationException($@"{e1Session.DbConnection.ConnectionString}  {e.Message}: {e.StackTrace}");
            }
            finally
            {
                Logger.Info($"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }
            return schedule;
        }


    }
}
