using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Globalization;
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
using NLog;

namespace IGServices.Common.Managers.Process
{
    public class SpacerConsumptionUploadManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        
        public static ProcessResponse Process(BusinessSystemSession e1Session, Dictionary<string, SqlConnection> sqlCns, Plant plant, string status)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"PLANT: {plant.PlantCode} BEGIN: {DateTime.Now.ToLocalTime()}");
            var response = new ProcessResponse($"Upload Spacer Consumption", plant, true);
            var spacerConsumptions = new SpacerConsumptions();
            var validationErrors = new List<string>();
            var transferStatus = "N";
            var transferGuid = Guid.NewGuid();
            long transferBatchNum = 0;
            try
            {
                response.Plant = plant;
                response.ConnectionStrings.Add(new ConnectionString("E1", e1Session.DbConnection.ConnectionString));
                foreach (var sqlCn in sqlCns)
                {
                    response.ConnectionStrings.Add(new ConnectionString(sqlCn.Key, sqlCn.Value.ConnectionString));
                }

                var step1AggregateShopFloorConsumptions = new ProcessStep("Step 1. Aggregate ShopFloor Spacer Consumptions");
                var spacerConsumptionsCount = 0;
                try
                {
                    spacerConsumptionsCount = SpacerConsumptionManager.Aggregate(sqlCns["PLANT.INTEGRATION"], transferGuid);
                    step1AggregateShopFloorConsumptions.RowsAffected = spacerConsumptionsCount;
                    response.RowsAffected = spacerConsumptionsCount;
                    if (spacerConsumptionsCount == 0)
                    {
                        response.Messages.Add(new Message("", "No Spacer Consumptions found at ShopFloor"));
                    }
                }
                catch (Exception e)
                {
                    step1AggregateShopFloorConsumptions.Errors.Add(new Error(e));
                }
                finally
                {
                    step1AggregateShopFloorConsumptions.Done();
                    response.ProcessSteps.Add(step1AggregateShopFloorConsumptions);
                }

                if (!step1AggregateShopFloorConsumptions.HasErrors() && spacerConsumptionsCount > 0)
                {
                    var step2GetShopFloorAggregations = new ProcessStep("Step 2. Get ShopFloor Aggregations");
                    try
                    {
                        spacerConsumptions = SpacerConsumptionManager.GetSpacerConsumptionAggregations(sqlCns["PLANT.INTEGRATION"], plant.PlantNum, transferGuid);
                        step2GetShopFloorAggregations.RowsAffected = spacerConsumptions.Count;
                        response.RowsAffected = step2GetShopFloorAggregations.RowsAffected;
                    }
                    catch (Exception e)
                    {
                        step2GetShopFloorAggregations.Errors.Add(new Error(e));
                    }
                    finally
                    {
                        step2GetShopFloorAggregations.Done();
                        response.ProcessSteps.Add(step2GetShopFloorAggregations);
                    }

                    if (!step2GetShopFloorAggregations.HasErrors())
                    {
                        var step3ValidateShopFloorAggregations = new ProcessStep("Step 3. Validate ShopFloor Aggregations");
                        try
                        {
                            validationErrors = SpacerConsumptionManager.Validate(spacerConsumptions, e1Session);
                            if (validationErrors.Any())
                            {
                                response.SerializedObjects.Add(new SerializedObject("ValidationErrors", XmlSerializationUtility.SerializeObject(Encoding.UTF8, validationErrors)));
                                foreach (var validationError in validationErrors)
                                {
                                    step3ValidateShopFloorAggregations.Messages.Add(new Message("ValidationError", validationError));
                                    step3ValidateShopFloorAggregations.Errors.Add(new Error(validationError, false));
                                }
                            } 
                            step3ValidateShopFloorAggregations.RowsAffected = spacerConsumptions.Count;
                        }
                        catch (Exception e)
                        {
                            step3ValidateShopFloorAggregations.Errors.Add(new Error(e));
                        }
                        finally
                        {
                            step3ValidateShopFloorAggregations.Done();
                            response.ProcessSteps.Add(step3ValidateShopFloorAggregations);
                        }

                        if (spacerConsumptions.Any(spacerConsumption => spacerConsumption.IsValid))
                        {
                            var step4BuildInterops = new ProcessStep("Step 4. Build E1 Interops");
                            var spacerConsumptionInteropMgr = new SpacerConsumptionInteropManager(e1Session);
                            var spacerConsumptionInterops = new SpacerConsumptionInterops();
                            var spacerConsumptionControlTrigger = new SpacerConsumptionControlTrigger();
                            try
                            {
                                transferBatchNum = BatchNumManager.GetNextSFEDBT(sqlCns["CORP.INTEGRATION"]);
                                if (!step4BuildInterops.HasErrors())
                                {
                                    foreach (var spacerConsumption in spacerConsumptions.Where(spacerConsumption => spacerConsumption.IsValid))
                                    {
                                        spacerConsumption.TransferBatchNum = transferBatchNum;
                                    }
                                    BatchNumManager.GetSFUKIDs(spacerConsumptions, sqlCns["CORP.INTEGRATION"]);
                                }
                            }
                            catch (Exception e)
                            {
                                step4BuildInterops.Errors.Add(new Error(e));
                            }
                            finally
                            {
                                step4BuildInterops.Done();
                                response.ProcessSteps.Add(step4BuildInterops);
                            }
                            if (!step4BuildInterops.HasErrors())
                            {
                                spacerConsumptionInterops = spacerConsumptionInteropMgr.BuildSpacerConsumptionInterops(spacerConsumptions);
                            }
                            if (!step4BuildInterops.HasErrors() && spacerConsumptionInterops.Any())
                            {
                                spacerConsumptionControlTrigger = spacerConsumptionInteropMgr.BuildSpacerConsumptionTrigger(response.Plant.PlantNum, transferBatchNum, sqlCns["CORP.INTEGRATION"]);
                            }
                            if (!step4BuildInterops.HasErrors())
                            {
                                var step5WriteE1SpacerConsumption = new ProcessStep("Step 5. Write E1 Spacer Consumption");
                                var e1Trans = e1Session.DbConnection.BeginTransaction(IsolationLevel.RepeatableRead);
                                try
                                {
                                    spacerConsumptionInteropMgr.InsertE1SpacerConsumptionInterops(e1Session, e1Trans, spacerConsumptionInterops);
                                    step5WriteE1SpacerConsumption.RowsAffected = spacerConsumptionInterops.Count;
                                    if (!step5WriteE1SpacerConsumption.HasErrors())
                                    {
                                        spacerConsumptionInteropMgr.InsertSpacerConsumptionTrigger(e1Session, e1Trans, spacerConsumptionControlTrigger);
                                    }
                                    if (!step5WriteE1SpacerConsumption.HasErrors())
                                    {
                                        e1Trans.Commit();
                                        transferStatus = "Y";
                                        SuccessE1(spacerConsumptions);
                                    } // if (!step5WriteE1SpacerConsumption.HasErrors())
                                    else
                                    {
                                        spacerConsumptionInteropMgr.RollBack(e1Trans);
                                        //step4BuildInterops.Errors.Add(new Error($"Unable to Write E1 Boxing"));
                                        FailE1(spacerConsumptions);
                                    }
                                }
                                catch (Exception e)
                                {
                                    step5WriteE1SpacerConsumption.Errors.Add(new Error(e));
                                    spacerConsumptionInteropMgr.RollBack(e1Trans);
                                    FailE1(spacerConsumptions);
                                }
                                finally
                                {
                                    step5WriteE1SpacerConsumption.Done();
                                    response.ProcessSteps.Add(step5WriteE1SpacerConsumption);
                                }

                                if (!step5WriteE1SpacerConsumption.HasErrors())
                                {
                                    var step6UpdateShopFloor = new ProcessStep("Step 6. Update ShopFloor SpacerConsumptionAggregate");
                                    var sFUpdateBatchSize = Convert.ToInt16(ConfigurationManager.AppSettings["MaxSFRowsPerBatch"]);
                                    try
                                    {
                                        SpacerConsumptionManager.UpdateSpacerConsumption(sqlCns["PLANT.INTEGRATION"], spacerConsumptions, sFUpdateBatchSize);
                                        step6UpdateShopFloor.RowsAffected = spacerConsumptions.Count;
                                        transferStatus = "Y";
                                    }
                                    catch (Exception e)
                                    {
                                        step6UpdateShopFloor.Errors.Add(new Error(e));
                                    }
                                    finally
                                    {
                                        step6UpdateShopFloor.Done();
                                        response.ProcessSteps.Add(step6UpdateShopFloor);
                                    }
                                }
                            }
                        } // if (spacerConsumptions.Any(spacerConsumption => spacerConsumption.IsValid))
                        else
                        {
                            step3ValidateShopFloorAggregations.Messages.Add(new Message("ValidationError", "No valid Spacer Consumptions available"));
                        }
                    } // if (!step2GetShopFloorAggregations.HasErrors())
                } // if (!step1AggregateShopFloorConsumptions.HasErrors() && isAvailable)

                var stepSummarizeUpload = new ProcessStep("Schedule Summary Collections");
                try
                {
                    var headerSummary = new
                    {
                        Location = $@"{plant.PlantCode} - {plant.PlantNum}",
                        TransferBatchNum = transferBatchNum,
                        Spacers = Convert.ToInt16(spacerConsumptions?.Sum(sc => sc.Quantity)),
                        Items = Convert.ToInt16(spacerConsumptions?.Count),
                        TransferStatus = transferStatus,
                        HasErrors = response.HasErrors() || validationErrors.Any(),
                        StartDate = response.ProcessSteps.Min(x => x.StartDate),
                        StartDateString = response.ProcessSteps.Min(x => x.StartDate).ToString("M/dd/yyyy hh:mm:ss tt", CultureInfo.CreateSpecificCulture("en-US")),
                        EndDate = response.ProcessSteps.Max(x => x.EndDate),
                        EndDateString = response.ProcessSteps.Min(x => x.EndDate).ToString("M/dd/yyyy hh:mm:ss tt", CultureInfo.CreateSpecificCulture("en-US")),
                        ElapsedSeconds = (decimal)(response.ProcessSteps.Max(x => x.EndDate) - response.ProcessSteps.Min(x => x.StartDate)).TotalSeconds
                    };
                    response.SerializedObjects.Add(new SerializedObject("SummaryNode", XmlSerializationUtility.SerializeAnonymousType(headerSummary, "Header")));

                    response.SerializedObjects.Add("SpacerConsumptions", spacerConsumptions);
                    stepSummarizeUpload.RowsAffected = spacerConsumptions.Count;
                }
                catch (Exception e)
                {
                    stepSummarizeUpload.Errors.Add(new Error(e));
                }
                stepSummarizeUpload.Done();
                response.ProcessSteps.Add(stepSummarizeUpload);
                response.Done();

                var stepSendMail = new ProcessStep("Send Mail");
                try
                {
                    var toConfigAddresses1 = ConfigurationManager.AppSettings["MailSpacerUploadToAddress"];
                    var toAddrs1 = $"{toConfigAddresses1}";
                    var serializedXml = response.Serialize();
                    //Logger.Trace($"Serialized Process: {serializedXml}");
                    var xslOutput = XsltManager.Transform(serializedXml, "IGServices.Common.Xslt.SpacerConsumptionUploadReport.xslt", Encoding.UTF8);
                    //Logger.Trace($"Report XML: {xslOutput}");
                    var subject = $"{plant.PlantCode} [{ConfigurationManager.AppSettings["PrimaryEnvironment"]}] Spacer Consumption Uploads - Batch {transferBatchNum}";
                    if (response.HasErrors() || validationErrors.Any())
                    {
                        transferStatus = "E";
                        subject = $"Error - {subject}";
                        Mail.SendHtmlAsync(ConfigurationManager.AppSettings["SmtpHost"],
                           ConfigurationManager.AppSettings["MailFromAddress"],
                           toAddrs1,
                           null,
                           subject,
                           xslOutput);
                    }

                    Mail.StoreResponse($@"{ConfigurationManager.AppSettings["ProcessResponseFilePath"]}\{response.Plant.PlantCode}", subject, xslOutput);

                }
                catch (Exception e)
                {
                    stepSendMail.Errors.Add(new Error(e));
                }
                stepSendMail.Done();
                response.ProcessSteps.Add(stepSendMail);
                // Hack for Karl's Error string-search interface after response email sent.
                if (validationErrors != null)
                {
                    foreach (var validationError in validationErrors)
                    {
                        response.Errors.Add(new Error(validationError, false));
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                throw new FaultException(e.Message);
            }
            finally
            {
                Logger.Info($"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }
            response.Close();
            return response;
        }

        private static void SuccessE1(SpacerConsumptions spacerConsumptions)
        {
            try
            {
                foreach (var spacerConsumption in spacerConsumptions.Where(spacerConsumption => spacerConsumption.IsValid))
                {
                    spacerConsumption.Transferred = true;
                    spacerConsumption.TransferStatus = "Y";
                    spacerConsumption.TransferDate = DateTime.Now.ToLocalTime();
                }
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                throw new ApplicationException(e.ToString());
            }
        }

        private static void FailE1(SpacerConsumptions spacerConsumptions)
        {
            try
            {
                foreach (var spacerConsumption in spacerConsumptions.Where(spacerConsumption => spacerConsumption.IsValid))
                {
                    spacerConsumption.Transferred = false;
                    spacerConsumption.TransferStatus = "E";
                    spacerConsumption.TransferDate = DateTime.Now.ToLocalTime();
                }
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                throw new ApplicationException(e.ToString());
            }
        }
    }
}
