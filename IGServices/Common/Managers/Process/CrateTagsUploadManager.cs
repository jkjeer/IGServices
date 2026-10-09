using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using IBM.Data.DB2.iSeries;
using IGServices.Common.Managers.Enterprise;
using IGServices.Common.Types.E1;
using IGServices.Common.Types.Enterprise;
using IGServices.Common.Types.Response;
using IGServices.Common.Types.ShopFloor;
using IGServices.Common.Utilities;
using NLog;
// ReSharper disable InconsistentNaming

namespace IGServices.Common.Managers.Process
{
    public class CrateTagsUploadManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static ProcessResponse Process(BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Plant plant, decimal scheduleNum, decimal workOrderNum, string serialNum)
        {
            var response = new ProcessResponse("Crate Tags Upload Manager");
            response.Plant = plant;
            response.ConnectionStrings.Add(new ConnectionString("E1", session.DbConnection.ConnectionString));
            foreach (var sqlCn in sqlCns)
            {
                response.ConnectionStrings.Add(new ConnectionString(sqlCn.Key, sqlCn.Value.ConnectionString));
            }
            var crateTags = new CrateTags();
            var f580022s = new F580022s();

            /********************************************************/
            var step2 = new ProcessStep("Get Source Data");
            try
            {
                crateTags = GetCrateTags(sqlCns["PLANT.SHOPFLOOR"], plant.PlantNum, scheduleNum, workOrderNum, serialNum);
                step2.RowsAffected = crateTags.Count;
                response.RowsAffected = step2.RowsAffected;
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                step2.Errors.Add(new Error(e));
            }
            step2.Done();
            response.AddStep(step2);

            /********************************************************/
            if (crateTags.Any())
            {
                var step3 = new ProcessStep("Build F580022s");
                try
                {
                    f580022s = BuildF580022s(crateTags);
                    step3.RowsAffected = crateTags.Count;
                }
                catch (Exception e)
                {
                    Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                    step3.Errors.Add(new Error(e));
                }
                finally
                {
                    step3.RowsAffected = f580022s.Count;
                    step3.Done();
                    response.RowsAffected = step3.RowsAffected;
                    response.AddStep(step3);
                }
            }

            /********************************************************/
            if (response.RowsAffected > 0)
            {
                var step4 = new ProcessStep("Update ShopFloor");
                try
                {
                    if (!response.HasErrors())
                    {
                        step4.RowsAffected = UpdateCrateTagsSuccess(sqlCns["PLANT.SHOPFLOOR"], plant.PlantNum, crateTags);
                    }
                    else
                    {
                        step4.RowsAffected = UpdateCrateTagsFail(sqlCns["PLANT.SHOPFLOOR"], plant.PlantNum, crateTags);
                    }
                }
                catch (Exception e)
                {
                    Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                    step4.Errors.Add(new Error(e));
                }
                finally
                {
                    step4.Done();
                    response.AddStep(step4);
                }
            }
            response.Done();
            if (response.HasErrors())
            {
                response.Close();
                SummarizeCrateTagsUpload(response, crateTags);
                NotifyCrateTagsUpload(response);
            }
            Logging.InsertProcessLog(response);
            return response;
        }

        public static void SummarizeCrateTagsUpload(ProcessResponse response, CrateTags crateTags)
        {
            var stepSummarizeUpload = new ProcessStep("Summary Collections");
            try
            {
                var scheduleHeaderSummary = new
                {
                    Location = response.Plant.PlantCode + " - " + response.Plant.PlantNum,
                    TransferStatus = "E",
                    HasErrors = true,
                    response.StartDate,
                    StartDateString = response.StartDate.ToString("M/dd/yyyy hh:mm:ss tt", CultureInfo.CreateSpecificCulture("en-US")),
                    response.EndDate,
                    EndDateString = response.EndDate.ToString("M/dd/yyyy hh:mm:ss tt", CultureInfo.CreateSpecificCulture("en-US")),
                    ElapsedSeconds = (decimal)(response.EndDate - response.StartDate).TotalSeconds
                };
                response.SerializedObjects.Add(new SerializedObject("ScheduleSummaryNode", XmlSerializationUtility.SerializeAnonymousType(scheduleHeaderSummary, "ScheduleHeader")));

                //var transferError = new
                //{
                //    ForceLink = $"http://svc{ConfigurationManager.AppSettings["PrimaryEnvironment"]}/CTServices/CTServices.BoxingRestService.svc/UploadBoxingSchedule?PlantCode={response.Plant.PlantCode}",
                //    BackoutLink = $"http://svc{ConfigurationManager.AppSettings["PrimaryEnvironment"]}/CTServices/CTServices.ScheduleRestService.svc/BackoutMTOSchedule?PlantCode={response.Plant.PlantCode}&ScheduleNum={scheduleNum}"
                //};
                //response.SerializedObjects.Add(new SerializedObject("TransferErrorNode", XmlSerializationUtility.SerializeAnonymousType(transferError, "TransferError")));
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                stepSummarizeUpload.Errors.Add(new Error(e));
            }
            finally
            {
                stepSummarizeUpload.Done();
                response.ProcessSteps.Add(stepSummarizeUpload);
            }
        }

        public static void NotifyCrateTagsUpload(ProcessResponse response)
        {
            var stepSendMail = new ProcessStep("Send Mail");
            try
            {
                var toConfigAddresses1 = ConfigurationManager.AppSettings["MailConfirmToAddress"];
                var toPlantAddress = "";
                if (ConfigurationManager.AppSettings["SendPlantMail"] == "true") toPlantAddress = $"{response.Plant.PlantCode}{ConfigurationManager.AppSettings["PlantEmailFormat"]}";
                var toAddrs1 = $"{toPlantAddress}, {toConfigAddresses1}";
                var serializedXml = response.Serialize();
                Logger.Info($"Serialized Process: {serializedXml}");
                var xslOutput = XsltManager.Transform(serializedXml, "IGServices.Common.Xslt.CrateTagsDownloadReport.xslt", Encoding.UTF8);
                Logger.Info($"Report XML: {xslOutput}");
                var subject = $"{response.Plant.PlantCode} [{ConfigurationManager.AppSettings["PrimaryEnvironment"]}] CrateTags Upload";
                if (response.HasErrors()) subject = $"Error - {subject}";
                Mail.SendHtmlAsync(ConfigurationManager.AppSettings["SmtpHost"],
                    ConfigurationManager.AppSettings["MailFromAddress"],
                    toAddrs1,
                    null,
                    subject,
                    xslOutput);
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                stepSendMail.Errors.Add(new Error(e));
            }
            finally
            {
                stepSendMail.Done();
                response.ProcessSteps.Add(stepSendMail);
            }
        }

        private static SqlCommand GetCrateTags(IReadOnlyCollection<string> filters)
        {
            var filterString = new StringBuilder();
            foreach (var filter in filters)
            {
                filterString.Append(filter);
            }
            var cmd = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = $@"
                    SELECT [TagInfoID]
                      ,[PlantID]
                      ,[OrderNum]
                      ,[OrderType]
                      ,[LineID]
                      ,[Shift]
                      ,[WrittenDate]
                      ,[WorkStation]
                      ,[QuantityPacked]
                      ,[ScheduleDate]
                      ,[PackedBy]
                      ,[PrintTo]
                      ,[PrinterName]
                      ,[ScheduleNum]
                      ,[SeriaLNumber]
                      ,[RackTypeCode]
                      ,[Transferred]
                      ,[TransferDate]
                      ,[IPAddress]
                  FROM [ShopFloor].[dbo].[TagInfo]
	                WHERE 
                        {filterString} 
                        1=1
                "
            };
            return cmd;
        }

        public static CrateTags GetCrateTags(SqlConnection cnSF, string plantNum, decimal scheduleNum, decimal workOrderNum, string serialNum)
        {
            var crateTags = new CrateTags();
            var filters = new List<string>();
            var parameters = new List<SqlParameter>();
            try
            {
                //// Plant - Required
                //filters.Add("PlantID = ? AND \r\n");
                //parameters.Add(new SqlParameter("PlantID", SqlDbType.VarChar, 12));
                //parameters.Last().Value = plantNum;

                if (scheduleNum > 0 || workOrderNum > 0 || !string.IsNullOrEmpty(serialNum))
                {
                    // Schedule Num
                    if (scheduleNum > 0)
                    {
                        filters.Add("ScheduleNum = ? AND \r\n");
                        parameters.Add(new SqlParameter("ScheduleNum", SqlDbType.Decimal));
                        parameters.Last().Precision = 8;
                        parameters.Last().Scale = 0;
                        parameters.Last().Value = scheduleNum;
                    }

                    // Work Order Num
                    if (workOrderNum > 0)
                    {
                        filters.Add("OrderNum = ? AND \r\n");
                        parameters.Add(new SqlParameter("OrderNum", SqlDbType.Decimal));
                        parameters.Last().Precision = 8;
                        parameters.Last().Scale = 0;
                        parameters.Last().Value = workOrderNum;
                    }

                    // Serial Number
                    else if (!string.IsNullOrEmpty(serialNum))
                    {
                        filters.Add("SeriaLNumber = ? AND \r\n");
                        parameters.Add(new SqlParameter("SeriaLNumber", SqlDbType.VarChar, 10));
                        parameters.Last().Value = serialNum;
                    }
                }
                else // Reserve EDSP = ""
                {
                    filters.Add("Transferred = 0 AND ");
                }
                // Command
                var cmd = GetCrateTags(filters);
                foreach (SqlParameter parameter in parameters)
                {
                    cmd.Parameters.Add(parameter);
                }
                cmd.Connection = cnSF;
                Logger.Debug(Logging.DbCmdString(cmd));

                // Reader
                cmd.ExecuteReader(CommandBehavior.SingleResult);
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                throw new Exception(e.Message);
            }
            return crateTags;
        }

        public static int UpdateCrateTagsSuccess(SqlConnection cnSF, string plantNum, CrateTags crateTags)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"PLANT: {plantNum} BEGIN: {DateTime.Now.ToLocalTime()}");
            int rowCount;
            try
            {
                var filterString = new StringBuilder();
                foreach (var tag in crateTags)
                {
                    filterString.Append($@"{tag.TagInfoID},");
                }
                // Command
                var cmd = UpdateCrateTagsSuccess(filterString.ToString());
                cmd.Connection = cnSF;
                Logger.Trace(Logging.DbCmdString(cmd));
                rowCount = cmd.ExecuteNonQuery();
                Logger.Trace("ROWS: " + rowCount);
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + e.Message + ": " + e.StackTrace);
                throw new ApplicationException(cnSF.ConnectionString + "  " + e);
            }
            finally
            {
                Logger.Info("END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            }
            return rowCount;
        }

        private static SqlCommand UpdateCrateTagsSuccess(string filterString)
        {
            var cmd = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = $@"
                    UPDATE dbo.TagInfo
	                SET Transferred = 1
                        ,TransferDate = getdate()
                    WHERE TagInfoID in ({filterString})"
            };
            return cmd;
        }

        public static int UpdateCrateTagsFail(SqlConnection cnSF, string plantNum, CrateTags crateTags)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"PLANT: {plantNum} BEGIN: {DateTime.Now.ToLocalTime()}");
            int rowCount;
            try
            {
                var filterString = new StringBuilder();
                foreach (var tag in crateTags)
                {
                    filterString.Append($@"{tag.TagInfoID},");
                }
                // Command
                var cmd = UpdateCrateTagsFail(filterString.ToString());
                cmd.Connection = cnSF;
                Logger.Trace(Logging.DbCmdString(cmd));
                rowCount = cmd.ExecuteNonQuery();
                Logger.Trace("ROWS: " + rowCount);
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + e.Message + ": " + e.StackTrace);
                throw new ApplicationException(cnSF.ConnectionString + "  " + e);
            }
            finally
            {
                Logger.Info("END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            }
            return rowCount;
        }

        private static SqlCommand UpdateCrateTagsFail(string filterString)
        {
            var cmd = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = $@"
                    UPDATE dbo.TagInfo
	                SET Transferred = 0
                    WHERE TagInfoID in ({filterString})"
            };
            return cmd;
        }

        private static F580022s BuildF580022s(CrateTags crateTags)
        {
            var f580022s = new F580022s();
            foreach (var tag in crateTags)
            {
                
            }
            return f580022s;
        }
    }
}
