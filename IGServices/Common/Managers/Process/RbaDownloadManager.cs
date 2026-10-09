using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
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

namespace IGServices.Common.Managers.Process
{
    public class RbaDownloadManager
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public RbaDownloadManager(BusinessSystemSession session)
        {
            var hash = new Dictionary<string, iDB2Command>
            {
                //{"RbaDownloadManager.GetItemDefinition", GetItemDefinition()}
            };
            foreach (var keyName in hash.Keys.Where(keyName => !session.DbCommands.ContainsKey(keyName)))
            {
                session.DbCommands.Add(keyName, hash[keyName]);
            }
        }

        public ProcessResponse Process(BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Plant plant, TimeSpan timeSpan, bool useHighWaterMark, decimal rackNum, string report)
        {
            var response = new ProcessResponse("Rba Download Manager");
            var table1 = new RbaRackItemTable();
            var table2 = new RbaRackItemTable();

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
                highWaterMark = (useHighWaterMark) ? ShopFloor.HighWaterMarkManager.GetHighWaterMark(sqlCns["SHOPFLOOR"], plant, "RbaShippedRackItem") : null;
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
                var step2 = new ProcessStep("Get Shipped Racks");
                try
                {
                    var reader = GetShippedRackItem(session, sqlCns["INTEGRATION"], plant, processRunSpan, rackNum);
                    table1.AddShippedRackItem(reader, plant);
                    step2.RowsAffected = table1.Rows.Count;
                    response.RowsAffected = table1.Rows.Count;
                }
                catch (Exception e)
                {
                    step2.Errors.Add(new Error(e));
                }
                step2.Done();
                response.AddStep(step2);
            }

            if (!response.HasErrors())
            {
                /********************************************************/
                var step3 = new ProcessStep("Get Current Racks");
                try
                {
                    var reader = GetCurrentRackItem(session, sqlCns["SHOPFLOOR"], plant, processRunSpan, rackNum);
                    table2.AddCurrentRackItem(reader, plant);
                    step3.RowsAffected = table2.Rows.Count;
                    response.RowsAffected = table2.Rows.Count;
                }
                catch (Exception e)
                {
                    step3.Errors.Add(new Error(e));
                }
                step3.Done();
                response.AddStep(step3);
            }

            if (!response.HasErrors())
            {
                /********************************************************/
                if (table1.Rows.Count > 0)
                {
                    var step4 = new ProcessStep("Bulk Copy Shipped Racks");
                    SqlBulkCopyResponse bulkShippedResponse = new SqlBulkCopyResponse();
                    try
                    {
                        if (table1.Rows.Count > 0)
                        {
                            bulkShippedResponse =
                                BulkCopyManager.BulkTable("EBS.RbaShippedRackItem", table1, sqlCns["INTEGRATION"]);
                        }
                    }
                    catch (Exception e)
                    {
                        step4.Errors.Add(new Error(e));
                    }
                    finally
                    {
                        step4.RowsAffected = (int)bulkShippedResponse.RowsCopied;
                        step4.SqlBulkCopyResponses.Add(bulkShippedResponse);
                        step4.Done();
                        response.RowsAffected = step4.RowsAffected;
                        response.AddStep(step4);
                    }
                }

                /********************************************************/
                if (table2.Rows.Count > 0)
                {

                    var step5a = new ProcessStep("Truncate Current Racks");
                    try
                    {
                        if (table2.Rows.Count > 0)
                        {
                            int rows = TruncateCurrentRackItem(sqlCns["SHOPFLOOR"]);
                            step5a.RowsAffected = rows;
                        }
                    }
                    catch (Exception e)
                    {
                        step5a.Errors.Add(new Error(e));
                    }
                    finally
                    {
                        step5a.Done();
                        response.AddStep(step5a);
                    }

                    var step5b = new ProcessStep("Bulk Copy Current Racks");
                    SqlBulkCopyResponse bulkCurrentResponse = new SqlBulkCopyResponse();
                    try
                    {
                        if (table2.Rows.Count > 0)
                        {
                            bulkCurrentResponse =
                                BulkCopyManager.BulkTable("EBS.RbaCurrentRackItem", table2, sqlCns["INTEGRATION"]);
                        }
                    }
                    catch (Exception e)
                    {
                        step5b.Errors.Add(new Error(e));
                    }
                    finally
                    {
                        step5b.RowsAffected = (int)bulkCurrentResponse.RowsCopied;
                        step5b.SqlBulkCopyResponses.Add(bulkCurrentResponse);
                        step5b.Done();
                        response.RowsAffected = step5b.RowsAffected;
                        response.AddStep(step5b);
                    }
                }

                /********************************************************/
                var step6 = new ProcessStep("Bulk Copy Rba Files");
                SqlBulkCopyResponse bulkRbaResponse = new SqlBulkCopyResponse();
                try
                {
                    var dt = GetRbaFiles();
                    if (table2.Rows.Count > 0)
                    {
                        bulkRbaResponse =
                            BulkCopyManager.BulkTable("EBS.RbaConsumedBatchBin", dt, sqlCns["INTEGRATION"]);
                    }
                }
                catch (Exception e)
                {
                    step6.Errors.Add(new Error(e));
                }
                finally
                {
                    step6.RowsAffected = (int)bulkRbaResponse.RowsCopied;
                    step6.SqlBulkCopyResponses.Add(bulkRbaResponse);
                    step6.Done();
                    response.RowsAffected = step6.RowsAffected;
                    response.AddStep(step6);
                }

                /********************************************************/
                var step6a = new ProcessStep("Post Download Activities");
                try
                {
                    int rows = ExecutePostDownloadProcedure(sqlCns["SHOPFLOOR"]);
                    step6a.RowsAffected = rows;
                }
                catch (Exception e)
                {
                    step6a.Errors.Add(new Error(e));
                }
                finally
                {
                    step6a.RowsAffected = (int)bulkRbaResponse.RowsCopied;
                    step6a.Done();
                    response.RowsAffected = step6a.RowsAffected;
                    response.AddStep(step6a);
                }

                /********************************************************/
                if (useHighWaterMark)
                {
                    var step7 = new ProcessStep("Update High Water Mark");
                    try
                    {
                        if (table1.Rows.Count > 0)
                        {
                            highWaterMark.LocalDateTime = table1.MaxDate;
                            highWaterMark.EnterpriseDateTime = table1.MaxDate.AddHours(-plant.TimeOffset);
                        }
                        step7.SqlDmlResponses.Add(ShopFloor.HighWaterMarkManager.SaveHighWaterMark(sqlCns["SHOPFLOOR"], highWaterMark));
                    }
                    catch (Exception e)
                    {
                        step7.Errors.Add(new Error(e));
                    }
                    finally
                    {
                        step7.Done();
                        response.AddStep(step7);
                    }
                }

                /********************************************************/
                var step8a = new ProcessStep("Summary Collections");
                bool isReleaseHour = false;
                try
                {
                    var headerSummary = new
                    {
                        Plant = plant.PlantNum,
                        PlantCode = plant.PlantCode,
                        StartDate = response.StartDate.ToString("MM/dd/yyyy hh:mm:ss tt"),
                        EndDate = DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt"),
                        ElapsedSeconds = (DateTime.Now - response.StartDate).TotalSeconds
                    };
                    response.SerializedObjects.Add("Summary", headerSummary, null);

                    var productLineManager = new RbaProductLineManager();
                    var productLines = productLineManager.GetRbaProductLines(plant.PlantCode, sqlCns["SHOPFLOOR"]);
                    response.SerializedObjects.Add("SerializedProductLines1", productLines);
                    response.SerializedObjects.Add("SerializedProductLines2", productLines);

                    var truckListDt = productLineManager.GetTruckList(plant.PlantCode, sqlCns["SHOPFLOOR"]).AsEnumerable();
                    var truckTimes = from p in truckListDt
                        select new
                        {
                            TruckNum = p.Field<int>("SequenceNum"),
                            DepartureDate = p.Field<DateTime>("DepartureDate"),
                            LoadStartDate = p.Field<DateTime>("LoadStartDate"),
                            LoadEndDate = p.Field<DateTime>("LoadEndDate"),
                            ReleaseDate = p.Field<DateTime>("ReleaseDate"),
                            ReleaseDateString = p.Field<DateTime>("ReleaseDate").ToString("MM/dd/yyyy hh:mm tt"),
                            DepartureDateString = p.Field<DateTime>("DepartureDate").ToString("MM/dd/yyyy hh:mm tt"),
                            ArrivalDateString = p.Field<DateTime>("ArrivalDate").ToString("MM/dd/yyyy hh:mm tt"),
                            LoadStartDateString = p.Field<DateTime>("LoadStartDate").ToString("MM/dd/yyyy hh:mm tt"),
                            LoadEndDateString = p.Field<DateTime>("LoadEndDate").ToString("MM/dd/yyyy hh:mm tt"),
                            NextTruck = p.Field<string>("NextTruck"),
                            CurrentRelease = p.Field<string>("CurrentRelease"),
                        };
                    response.SerializedObjects.Add("SerializedTruckList", truckTimes, "Trucks", "Truck");
                    isReleaseHour = truckTimes.Any(x => x.ReleaseDate.Hour == DateTime.Now.Hour);

                    var shippedTodayDt = productLineManager.GetShippedToday(plant.PlantCode, sqlCns["SHOPFLOOR"]).AsEnumerable();
                    var shippedTodayLines = from p in shippedTodayDt
                                           select new
                        {
                            ProductLineCode = p.Field<string>("ShortCode"),
                            T1 = p.Field<string>("1"),
                            T2 = p.Field<string>("2"),
                            T3 = p.Field<string>("3"),
                            T4 = p.Field<string>("4"),
                            T5 = p.Field<string>("5"),
                            T6 = p.Field<string>("6"),
                            T7 = p.Field<string>("7"),
                            T8 = p.Field<string>("8"),
                            T9 = p.Field<string>("9")
                                        };
                    response.SerializedObjects.Add("SerializedShippedLines", shippedTodayLines, "ShippedLines", "ShippedLine");

                    var loadsDt = productLineManager.GetLoads(plant.PlantCode, sqlCns["SHOPFLOOR"]).AsEnumerable();
                    var loads = from p in loadsDt
                                select new
                        {
                            LoadNum = p.Field<decimal>("LoadNum"),
                            LoadStatus = p.Field<string>("LoadStatus"),
                            ShipDate = CastDbNull.To<DateTime>(p.Field<DateTime?>("ShipDate"), new DateTime(1900, 1, 1)).ToString("MM/dd/yyyy hh:mm tt"),
                            ArrivalDate = CastDbNull.To<DateTime>(p.Field<DateTime?>("ShipEstDate"), new DateTime(1900, 1, 1)).ToString("MM/dd/yyyy hh:mm tt"),
                            DepartureDate = CastDbNull.To<DateTime>(p.Field<DateTime?>("ShipEstDate"), new DateTime(1900, 1, 1)).AddHours(-3).ToString("MM/dd/yyyy hh:mm tt"),
                            Racks = p.Field<int>("Racks"),
                            Units = p.Field<int>("Units")
                                };
                    response.SerializedObjects.Add("SerializedLoads", loads, "Loads", "Load");

                    var rbaShiftsDt = productLineManager.GetRbaShiftCalendar(plant.PlantCode, sqlCns["SHOPFLOOR"]).AsEnumerable();
                    var rbaShifts = from p in rbaShiftsDt
                                     select new
                        {
                            ProductLine = p.Field<string>("ShortCode"),
                            S1 = p.Field<string>("1"),
                            S2 = p.Field<string>("2"),
                            S3 = p.Field<string>("3")
                        };
                    response.SerializedObjects.Add("SerializedRbaShifts", rbaShifts, "Shifts", "Shift");

                    var consumedTodayDt = productLineManager.GetConsumedTodayByInterval(plant.PlantCode, sqlCns["SHOPFLOOR"]).AsEnumerable();
                    var consumedTodayLines = from p in consumedTodayDt
                                             select new
                        {
                            ProductLineCode = p.Field<string>("ShortCode"),
                            T1 = p.Field<string>("2AM"),
                            T2 = p.Field<string>("4AM"),
                            T3 = p.Field<string>("6AM"),
                            T4 = p.Field<string>("8AM"),
                            T5 = p.Field<string>("10AM"),
                            T6 = p.Field<string>("12PM"),
                            T7 = p.Field<string>("2PM"),
                            T8 = p.Field<string>("4PM"),
                            T9 = p.Field<string>("6PM"),
                            T10 = p.Field<string>("8PM"),
                            T11 = p.Field<string>("10PM"),
                            T12 = p.Field<string>("12AM")
                                             };
                    response.SerializedObjects.Add("SerializedConsumedLines", consumedTodayLines, "ConsumedLines", "ConsumedLine");

                    var hotUnitsDt = productLineManager.GetHotRacks(plant.PlantCode, sqlCns["SHOPFLOOR"]).AsEnumerable();
                    var hotUnits = from p in hotUnitsDt
                                   select new
                        {
                            RackNum = p.Field<decimal>("RackNum"),
                            ScheduleNum = p.Field<decimal>("ScheduleNum"),
                            BoxNum = p.Field<decimal>("BoxNum"),
                            RackType = p.Field<string>("RackType"),
                            HeaderStatus = p.Field<string>("HeaderStatus"),
                            PriorityCode = p.Field<string>("PriorityCode"),
                            DueDate = p.Field<DateTime>("DueDate")
                                   };
                    var hotRacks = from p in hotUnits
                                   group p by p.RackNum
                        into g
                                   select new
                                   {
                                       RackNum = g.Key,
                                       ScheduleNum = g.First().ScheduleNum,
                                       BoxNum = g.First().BoxNum,
                                       HeaderStatus = g.GroupBy(x => x.HeaderStatus).OrderByDescending(z => z.Count()).First().Key,
                                       RackType = g.GroupBy(x => x.RackType).OrderByDescending(z => z.Count()).First().Key,
                                       PriorityCode = g.GroupBy(x => x.PriorityCode).OrderByDescending(z => z.Count()).First().Key,
                                       DueDate = g.GroupBy(x => x.DueDate).OrderByDescending(z => z.Count()).First().Key.ToString("MM/dd"),
                                       Units = g.Count()
                                   };
                    response.SerializedObjects.Add("SerializedHotRacks", hotRacks, "HotRacks", "HotRack");

                    //var nextRacks = new List<RbaRack>();
                    //var productLinesGroup = productLines.GroupBy(x => x.ProductLineCode);
                    //foreach (var plg in productLinesGroup)
                    //{
                    //    nextRacks.AddRange(plg.First().NextRacks.Where(x => x.IsRequired == "Y"));
                    //}
                    //var rackTypes = from p in nextRacks
                    //    group p by p.RackType
                    //    into g
                    //    select new
                    //    {
                    //        RackType = g.Key,
                    //        Racks = g.Count()
                    //    };
                    //response.SerializedObjects.Add("SerializedRackTypes", rackTypes, "RackTypes", "RackType");

                    step8a.RowsAffected = 0;
                }
                catch (Exception e)
                {
                    step8a.Errors.Add(new Error(e));
                }
                step8a.Done();
                response.ProcessSteps.Add(step8a);

                var serializedXml = response.Serialize();
                _logger.Info("Serialized Process: " + serializedXml);


                /********************************************************/
                var step9 = new ProcessStep("Send Mail");
                var arrReport = report.ToUpper().Split(',');
                SqlConnection corpIntCn = new SqlConnection();
                try
                {
                    var adminRecipients = new List<string>();
                    corpIntCn = ConnectionManager.GetSqlConnectionFromKey("CORP.INTEGRATION");
                    corpIntCn.Open();

                    // Admin Report
                    var xslOutput = XsltManager.Transform(serializedXml,
                        "IGServices.Common.Xslt.RbaShippingAdminReport.xslt", Encoding.UTF8);

                    // Recipients
                    adminRecipients.AddRange(Mail.GetRecipients(corpIntCn, "TOIG_RBA_KANBAN", "PUBLIC", plant.PlantCode));
                    adminRecipients.AddRange(Mail.GetRecipients(corpIntCn, "TOIG_RBA_KANBAN", "ADMIN"));

                    Mail.SendHtmlAsync(System.Configuration.ConfigurationManager.AppSettings["SmtpHost"],
                        System.Configuration.ConfigurationManager.AppSettings["MailFromAddress"],
                        adminRecipients,
                        null,
                        plant.PlantCode + " [" +
                        System.Configuration.ConfigurationManager.AppSettings["Environment"] + "] " +
                        "Rba Pull ADMIN Report ",
                        xslOutput);

                    // User Reports
                    if (isReleaseHour || arrReport.Contains("USER"))
                    {
                        var user1Recipients = new List<string>();
                        var xslOutputMini1 = XsltManager.Transform(serializedXml,
                            "IGServices.Common.Xslt.RbaShippingUserReport2.xslt", Encoding.UTF8);

                        // Recipients
                        user1Recipients.AddRange(Mail.GetRecipients(corpIntCn, "TOIG_RBA_KANBAN", "USER",
                            plant.PlantCode));
                        user1Recipients.AddRange(Mail.GetRecipients(corpIntCn, "TOIG_RBA_KANBAN", "ADMIN"));

                        Mail.SendHtmlAsync(System.Configuration.ConfigurationManager.AppSettings["SmtpHost"],
                            System.Configuration.ConfigurationManager.AppSettings["MailFromAddress"],
                            user1Recipients,
                            null,
                            plant.PlantCode + " [" +
                            System.Configuration.ConfigurationManager.AppSettings["Environment"] + "] " +
                            "Rba Pull Report ",
                            xslOutputMini1);

                        // Early AM
                        if (DateTime.Now.Hour <= 3 || DateTime.Now.Hour >= 18)
                        {
                            var user2Recipients = new List<string>();
                            var xslOutputMini2 = XsltManager.Transform(serializedXml,
                                "IGServices.Common.Xslt.RbaShippingUserReport1.xslt", Encoding.UTF8);

                            // Recipients
                            user2Recipients.AddRange(Mail.GetRecipients(corpIntCn, "TOIG_RBA_KANBAN", "USER_AM",
                                plant.PlantCode));
                            user2Recipients.AddRange(Mail.GetRecipients(corpIntCn, "TOIG_RBA_KANBAN", "ADMIN"));

                            Mail.SendHtmlAsync(System.Configuration.ConfigurationManager.AppSettings["SmtpHost"],
                                System.Configuration.ConfigurationManager.AppSettings["MailFromAddress"],
                                user2Recipients,
                                null,
                                plant.PlantCode + " [" +
                                System.Configuration.ConfigurationManager.AppSettings["Environment"] + "] " +
                                "Rba Pull Report ",
                                xslOutputMini2);
                        }
                    }
                }
                catch (Exception e)
                {
                    step9.Errors.Add(new Error(e));
                }
                finally
                {
                    corpIntCn.Close();
                }
                step9.Done();
                response.AddStep(step9);

            }
            response.Done();
            Logging.InsertProcessLog(response);
            return response;
        }

        /******************************************************************
         * Shipped Rack Item
         ******************************************************************/
        public iDB2Command GetShippedRackItem(List<string> filters)
        {
            var filterString = new StringBuilder();
            foreach (string filter in filters)
            {
                filterString.Append(filter);
            }

            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = 0;
            cmd.CommandText = String.Format(@"
            SELECT 
	            SDAN8, 
	            SDSHAN, 
	            SDDOCO,
	            SDDCTO,
	            SDLNID,
	            SDRORN,
	            WASRST,
	            SDTRDJ,
	            SDDRQJ,
	            SDURCD,
	            SDLTTR, 
	            SDNXTR,
	            ATZ58PT,
	            ATZ58CP2,
	            ATZ58CP3,
	            ATZ58CP4,
	            RDZ58RID,
                RDLOTN,
	            RDZ58SN,
	            RHZ58HST,
	            RDZ58DST,
                RDZ58SL,
	            RDPQOH,
                RDUPMJ,
                RDTDAY,
	            RHLOCN,
	            RHZ58RT,
                RHZ58BOX,
                S5Z58SCN,
                S5Z58SCD,
	            KZSGVL AS Z5840310,
                LHLDNM,
                LHZ58LDS,
                LHADDJ,
                LHADTM,
                LHZ58EDTA,
                LHZ58ETMA
            FROM 
	            F4211 
	            INNER JOIN F580032 
	            ON	ATKCOO = SDKCOO AND
		            ATDOCO = SDDOCO AND
		            ATDCTO = SDDCTO AND
		            ATLNID = SDLNID 
	            INNER JOIN F4801
	            ON  WADOCO = ATZ58WODC
	            INNER JOIN F580056
	            ON	RDKCOO = SDKCOO AND
		            RDDOCO = SDDOCO AND
		            RDDCTO = SDDCTO AND
		            RDLNID = SDLNID 
	            INNER JOIN F580055
	            ON	RHZ58RID = RDZ58RID
				INNER JOIN F5834A11
				ON	S5DOCO = WADOCO
				INNER JOIN F5834A01
				ON	S1MCU = S5MCU AND
					S1Z58SCN = S5Z58SCN AND
					S1Z58SCD = S5Z58SCD
	            INNER JOIN F3201
	            ON	CMKCOO = SDKCOO AND
		            CMDOCO = SDDOCO AND
		            CMDCTO = SDDCTO AND
		            CMLNID = SDLNID 
	            INNER JOIN F3211
	            ON	KZCFGID = CMCFGID AND
		            KZPEL = 40310
                INNER JOIN F580150
                ON  LHLDNM = SDPSN
            WHERE 
                {0}
	            SDAN8 IN (10186912)
            FOR READ ONLY", filterString);
            return cmd;
        }

        public IDataReader GetShippedRackItem(BusinessSystemSession session, SqlConnection cn, Plant plant, ProcessRunSpan span, decimal rackNum)
        {
            var table = new ItemTable();
            var cmd = new iDB2Command();
            List<string> filters = new List<string>();
            List<iDB2Parameter> parameters = new List<iDB2Parameter>();
            try
            {
                // SHIPPED
                filters.Add("RHZ58HST IN ('7','8') AND\r\n");

                // Plant
                if (!String.IsNullOrEmpty(plant.PlantCode))
                {
                    filters.Add("SDMCU = ? AND\r\n");
                    parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
                    parameters.Last().Value = plant.PlantNum.PadLeft(12);
                }

                // Rack Num
                if (rackNum > 0)
                {
                    filters.Add("RHZ58RID = ? AND \r\n");
                    parameters.Add(new iDB2Parameter("Z58RID", iDB2DbType.iDB2Decimal));
                    parameters.Last().Precision = 15;
                    parameters.Last().Scale = 0;
                    parameters.Last().Value = rackNum;
                }

                // Stock Types
                //if (stockTypes.Any())
                //{
                //    string strInClause = "";
                //    foreach (string stockType in stockTypes)
                //    {
                //        strInClause = strInClause + "?,";
                //        parameters.Add(new iDB2Parameter("stockType", iDB2DbType.iDB2Graphic, 1));
                //        parameters.Last().Value = stockType;
                //    }
                //    filters.Add("SDLNTY IN (" + strInClause.Substring(0, strInClause.Length - 1) + ") AND\r\n");
                //}

                // Process Run Span
                if (!span.IsEmpty)
                {
                    filters.Add(E1TimeManager.GetE1DateTimeQueryString("RDUPMJ", "RDTDAY", span.E1TimeIdSpan) + " AND \r\n");
                }

                cmd = GetShippedRackItem(filters);
                foreach (iDB2Parameter parameter in parameters)
                {
                    cmd.Parameters.Add(parameter);
                }
                cmd.Connection = session.DbConnection;
                _logger.Debug(Logging.DbCmdString(cmd));
                // Reader
                return cmd.ExecuteReader(CommandBehavior.SingleResult);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /******************************************************************
          * Current Rack Item
          ******************************************************************/
        public iDB2Command GetCurrentRackItem(List<string> filters)
        {
            var filterString = new StringBuilder();
            foreach (string filter in filters)
            {
                filterString.Append(filter);
            }

            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = 0;
            cmd.CommandText = String.Format(@"
            SELECT 
	            SDAN8, 
	            SDSHAN, 
	            SDDOCO,
	            SDDCTO,
	            SDLNID,
	            SDRORN,
	            WASRST,
	            SDTRDJ,
	            SDDRQJ,
	            SDURCD,
	            SDLTTR, 
	            SDNXTR,
	            ATZ58PT,
	            ATZ58CP2,
	            ATZ58CP3,
	            ATZ58CP4,
	            RDZ58RID,
                RDLOTN,
	            RDZ58SN,
	            RHZ58HST,
	            RDZ58DST,
                RDZ58SL,
	            RDPQOH,
                RDUPMJ,
                RDTDAY,
	            RHLOCN,
	            RHZ58RT,
                RHZ58BOX,
                RHPSN,
                S5Z58SCN,
                S5Z58SCD,
	            KZSGVL AS Z5840310,
                LHLDNM,
                LHZ58LDS,
                LHADDJ,
                LHADTM,
                LHZ58EDTA,
                LHZ58ETMA
            FROM 
	            F4211 
	            INNER JOIN F580032 
	            ON	ATKCOO = SDKCOO AND
		            ATDOCO = SDDOCO AND
		            ATDCTO = SDDCTO AND
		            ATLNID = SDLNID 
	            INNER JOIN F4801
	            ON  WADOCO = ATZ58WODC
	            INNER JOIN F580056
	            ON	RDKCOO = SDKCOO AND
		            RDDOCO = SDDOCO AND
		            RDDCTO = SDDCTO AND
		            RDLNID = SDLNID 
	            INNER JOIN F580055
	            ON	RHZ58RID = RDZ58RID
				INNER JOIN F5834A11
				ON	S5DOCO = WADOCO
				INNER JOIN F5834A01
				ON	S1MCU = S5MCU AND
					S1Z58SCN = S5Z58SCN AND
					S1Z58SCD = S5Z58SCD
	            INNER JOIN F3201
	            ON	CMKCOO = SDKCOO AND
		            CMDOCO = SDDOCO AND
		            CMDCTO = SDDCTO AND
		            CMLNID = SDLNID 
	            INNER JOIN F3211
	            ON	KZCFGID = CMCFGID AND
		            KZPEL = 40310
				LEFT OUTER JOIN F580150
				ON	LHLDNM = RHPSN
            WHERE 
                {0}
	            SDAN8 IN (10186912)
            FOR READ ONLY", filterString);
            return cmd;
        }

        public IDataReader GetCurrentRackItem(BusinessSystemSession session, SqlConnection cn, Plant plant, ProcessRunSpan span, decimal rackNum)
        {
            var table = new ItemTable();
            var cmd = new iDB2Command();
            List<string> filters = new List<string>();
            List<iDB2Parameter> parameters = new List<iDB2Parameter>();
            try
            {
                // SHIPPED
                filters.Add("RHZ58HST NOT IN ('7','8') AND\r\n");

                // Plant
                if (!String.IsNullOrEmpty(plant.PlantCode))
                {
                    filters.Add("SDMCU = ? AND\r\n");
                    parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
                    parameters.Last().Value = plant.PlantNum.PadLeft(12);
                }

                // Rack Num
                if (rackNum > 0)
                {
                    filters.Add("RHZ58RID = ? AND \r\n");
                    parameters.Add(new iDB2Parameter("Z58RID", iDB2DbType.iDB2Decimal));
                    parameters.Last().Precision = 15;
                    parameters.Last().Scale = 0;
                    parameters.Last().Value = rackNum;
                }

                // Stock Types
                //if (stockTypes.Any())
                //{
                //    string strInClause = "";
                //    foreach (string stockType in stockTypes)
                //    {
                //        strInClause = strInClause + "?,";
                //        parameters.Add(new iDB2Parameter("stockType", iDB2DbType.iDB2Graphic, 1));
                //        parameters.Last().Value = stockType;
                //    }
                //    filters.Add("SDLNTY IN (" + strInClause.Substring(0, strInClause.Length - 1) + ") AND\r\n");
                //}

                // Process Run Span
                //if (!span.IsEmpty)
                //{
                //    filters.Add(E1TimeManager.GetE1DateTimeQueryString("RDUPMJ", "RDTDAY", span.E1TimeIdSpan) + " AND \r\n");
                //}

                cmd = GetCurrentRackItem(filters);
                foreach (iDB2Parameter parameter in parameters)
                {
                    cmd.Parameters.Add(parameter);
                }
                cmd.Connection = session.DbConnection;
                _logger.Debug(Logging.DbCmdString(cmd));
                // Reader
                return cmd.ExecuteReader(CommandBehavior.SingleResult);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public SqlCommand TruncateCurrentRackItem()
        {
            var cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = 0;
            cmd.CommandText = "TRUNCATE TABLE ShopFloor.EBS.RbaCurrentRackItem";
            return cmd;
        }

        public int TruncateCurrentRackItem(SqlConnection cn)
        {
            try
            {
                var cmd = TruncateCurrentRackItem();
                cmd.Connection = cn;
                _logger.Debug(Logging.DbCmdString(cmd));
                return cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public SqlCommand ExecutePostDownloadProcedure()
        {
            var cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = 0;
            cmd.CommandText = "EXEC ShopFloor.EBS.usp_RbaDownload";
            return cmd;
        }

        public int ExecutePostDownloadProcedure(SqlConnection cn)
        {
            try
            {
                var cmd = ExecutePostDownloadProcedure();
                cmd.Connection = cn;
                _logger.Debug(Logging.DbCmdString(cmd));
                return cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /******************************************************************
          * Get Rba Files
          ******************************************************************/
        public static DataTable GetRbaFiles()
        {
            var ret = new List<string>();
            var sourceDirPath = "\\\\cgi.int\\sterling\\EDIData\\Inbound\\TOIG\\AWRenewal\\Pull";
            var archiveDirPath = "\\\\cgi.int\\sterling\\EDIData\\Archive\\Inbound\\TOIG_AWRenewalPull";
            //var archiveFolderName = String.Format("{0:yyyyMMdd}", DateTime.Now);
            var files = FileManager.GetFiles(sourceDirPath);

            var dt = new DataTable("rba");
            dt.Columns.Add("FileName", typeof(string));
            dt.Columns.Add("TransactionDate", typeof(DateTime));
            dt.Columns.Add("Batch", typeof(decimal));
            dt.Columns.Add("Bin", typeof(int));
            dt.Columns.Add("Location", typeof(string));

            foreach (var file in files)
            {
                FileManager.CopyFile(sourceDirPath, archiveDirPath, file.Name);
                string[] lines = System.IO.File.ReadAllLines(file.FullName);
                foreach (var line in lines)
                {
                    string[] fields = line.Split('\t');
                    if (fields.Length > 0 && fields[0].Length > 0)
                    {
                        var row = dt.NewRow();
                        row["FileName"] = file.Name;
                        row["TransactionDate"] = Convert.ToDateTime(fields[0]);
                        row["Batch"] = Convert.ToDecimal(fields[1]);
                        row["Bin"] = Convert.ToInt32(fields[2]);
                        row["Location"] = fields[3];
                        dt.Rows.Add(row);
                    }
                }
                FileManager.DeleteFile(file.FullName);
            }
            return dt;
        }
    }
}
