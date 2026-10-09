using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using IGServices.Common.Managers.E1;
using IGServices.Common.Managers.Enterprise;
using IGServices.Common.Managers.Process;
using IGServices.Common.Types.E1;
using IGServices.Common.Types.Enterprise;
using IGServices.Common.Types.Response;
using IGServices.Common.Utilities;
using NLog;

namespace IGServices.ProductionSvc
{
    public class ProductionProcessor
    {
        private static Logger Logger = LogManager.GetCurrentClassLogger();

        public ProcessResponse ProductionUpload(string plantCode, string status)
        {
            var response = new ProcessResponse("Production Upload");
            var session = new BusinessSystemSession();
            var cn = new SqlConnection();
            var cnInt = new SqlConnection();
            try
            {
                // resolve Plant
                var plant = PlantManager.GetPlant(plantCode);

                // Biz Session
                session = BusinessSystemSessionManager.GetBusinessSession();

                // Shop Floor Connection
                cn = ConnectionManager.GetSqlConnectionFromKey(plant.PlantCode + ".SHOPFLOOR");
                cn.Open();

                // Integration Connection
                cnInt = ConnectionManager.GetSqlConnectionFromKey("CORP.INTEGRATION");
                cnInt.Open();

                // Processes
                var mgr1 = new ConsumedStoceUploadManager(session);
                var returnResponse = mgr1.Process(session, cn, cnInt, plant, status);
                response.ProcessResponses.Add(returnResponse);

                var mgr3 = new HvCompletionUploadManager(session);
                var response3 = mgr3.Process(session, cn, cnInt, plant, status);
                response.ProcessResponses.Add(response3);

                //var mgr4 = new HvReceiptUploadManager(session);
                //var response4 = mgr4.Process(session, cn, cnInt, plant, status);
                //response.ProcessResponses.Add(response4);

                var mgr5 = new HvTrimUploadManager(session);
                var response5 = mgr5.Process(session, cn, cnInt, plant, status);
                response.ProcessResponses.Add(response5);

                var mgr6 = new HvIssueUploadManager(session);
                var response6 = mgr6.Process(session, cn, cnInt, plant, status);
                response.ProcessResponses.Add(response6);

                var mgr7 = new HvWoIssueUploadManager(session);
                var response7 = mgr7.Process(session, cn, cnInt, plant, status);
                response.ProcessResponses.Add(response7);

                //var mgr8 = new RemakeUploadManager(session);
                //var response8 = mgr8.Process(session, cn, cnInt, plant, status);
                //response.ProcessResponses.Add(response8);

                var mgr9 = new RackTransactionUploadManager(session);
                var response9 = mgr9.Process(session, cn, cnInt, plant, status);
                response.ProcessResponses.Add(response9);

                var mgr10 = new TagInfoUploadManager(session);
                var returnResponse0 = mgr10.Process(session, cn, cnInt, plant, status);
                response.ProcessResponses.Add(returnResponse0);
            }
            catch (Exception err)
            {
                throw new ApplicationException(err.ToString());
            }
            finally
            {
                session.Close();
                cn.Close();
                cnInt.Close();
            }
            return response;
        }

        public static ProcessResponse GlassInventoryTransactionUpload(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var response = new ProcessResponse($"{plant.PlantCode} Feed Consumption Manager");
            try
            {
                var statusCode = Data.UnboxParm.To(parms, "StatusCode", string.Empty);
                var mgr = new GlassInventoryTransactionUploadManager(session);
                response = mgr.Process(session, sqlCns, plant, statusCode);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
            }
            response.Close();
            return response;
        }

        public static ProcessResponse UploadRemakes(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var response = new ProcessResponse($"{plant.PlantCode} Remake Upload Manager");
            try
            {
                var timeSpan = Data.UnboxParm.To(parms, "TimeSpan", new TimeSpan());
                var useHighWaterMark = Data.UnboxParm.To(parms, "HighWaterMark", false);
                var statusCode = Data.UnboxParm.To(parms, "StatusCode", string.Empty);
                response.ProcessResponses.Add(RemakeUploadManager.Process(session, sqlCns, plant, "N"));
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
            }
            response.Done();
            return response;
        }

        public static ProcessResponse HvReceiptUpload(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var response = new ProcessResponse($"{plant.PlantCode} Hv Receipt Manager");
            try
            {
                var statusCode = Data.UnboxParm.To(parms, "StatusCode", string.Empty);
                var mgr = new HvReceiptUploadManager(session);
                response = mgr.Process(session, sqlCns, plant, statusCode);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
            }
            response.Close();
            return response;
        }

        public ProcessResponse ConsumedStoceUpload(string plantCode, string status)
        {
            var session = new BusinessSystemSession();
            var cn = new SqlConnection();
            var cnInt = new SqlConnection();
            try
            {
                // resolve Plant
                var plant = PlantManager.GetPlant(plantCode);

                // Biz Session
                session = BusinessSystemSessionManager.GetBusinessSession();

                // Shop Floor Connection
                cn = ConnectionManager.GetSqlConnectionFromKey(plant.PlantCode + ".SHOPFLOOR");
                cn.Open();

                // Integration Connection
                cnInt = ConnectionManager.GetSqlConnectionFromKey("CORP.INTEGRATION");
                cnInt.Open();

                // Process
                var mgr = new ConsumedStoceUploadManager(session);
                return mgr.Process(session, cn, cnInt, plant, status);
            }
            catch (Exception err)
            {
                throw new ApplicationException(err.ToString());
            }
            finally
            {
                session.Close();
                cn.Close();
                cnInt.Close();
            }
        }


        public ProcessResponse HvCompletionUpload(string plantCode, string status)
        {
            var session = new BusinessSystemSession();
            var cn = new SqlConnection();
            var cnInt = new SqlConnection();
            try
            {
                // resolve Plant
                var plant = PlantManager.GetPlant(plantCode);

                // Biz Session
                session = BusinessSystemSessionManager.GetBusinessSession();

                // Shop Floor Connection
                cn = ConnectionManager.GetSqlConnectionFromKey(plant.PlantCode + ".SHOPFLOOR");
                cn.Open();

                // Integration Connection
                cnInt = ConnectionManager.GetSqlConnectionFromKey("CORP.INTEGRATION");
                cnInt.Open();

                // Process
                var mgr = new HvCompletionUploadManager(session);
                return mgr.Process(session, cn, cnInt, plant, status);
            }
            catch (Exception err)
            {
                throw new ApplicationException(err.ToString());
            }
            finally
            {
                session.Close();
                cn.Close();
                cnInt.Close();
            }
        }

        //public ProcessResponse HvReceiptUpload(string plantCode, string status)
        //{
        //    var session = new BusinessSystemSession();
        //    var cn = new SqlConnection();
        //    var cnInt = new SqlConnection();
        //    try
        //    {
        //        // resolve Plant
        //        var plant = PlantManager.GetPlant(plantCode);

        //        // Biz Session
        //        session = BusinessSystemSessionManager.GetBusinessSession();

        //        // Shop Floor Connection
        //        cn = ConnectionManager.GetSqlConnectionFromKey(plant.PlantCode + ".SHOPFLOOR");
        //        cn.Open();

        //        // Integration Connection
        //        cnInt = ConnectionManager.GetSqlConnectionFromKey("CORP.INTEGRATION");
        //        cnInt.Open();

        //        // Process
        //        var mgr = new HvReceiptUploadManager(session);
        //        return mgr.Process(session, cn, cnInt, plant, status);
        //    }
        //    catch (Exception err)
        //    {
        //        throw new ApplicationException(err.ToString());
        //    }
        //    finally
        //    {
        //        session.Close();
        //        cn.Close();
        //        cnInt.Close();
        //    }
        //}

        public ProcessResponse HvTrimUpload(string plantCode, string status)
        {
            var session = new BusinessSystemSession();
            var cn = new SqlConnection();
            var cnInt = new SqlConnection();
            try
            {
                // resolve Plant
                var plant = PlantManager.GetPlant(plantCode);

                // Biz Session
                session = BusinessSystemSessionManager.GetBusinessSession();

                // Shop Floor Connection
                cn = ConnectionManager.GetSqlConnectionFromKey(plant.PlantCode + ".SHOPFLOOR");
                cn.Open();

                // Integration Connection
                cnInt = ConnectionManager.GetSqlConnectionFromKey("CORP.INTEGRATION");
                cnInt.Open();

                // Process
                var mgr = new HvTrimUploadManager(session);
                return mgr.Process(session, cn, cnInt, plant, status);
            }
            catch (Exception err)
            {
                throw new ApplicationException(err.ToString());
            }
            finally
            {
                session.Close();
                cn.Close();
                cnInt.Close();
            }
        }

        public ProcessResponse HvIssueUpload(string plantCode, string status)
        {
            var session = new BusinessSystemSession();
            var cn = new SqlConnection();
            var cnInt = new SqlConnection();
            try
            {
                // resolve Plant
                var plant = PlantManager.GetPlant(plantCode);

                // Biz Session
                session = BusinessSystemSessionManager.GetBusinessSession();

                // Shop Floor Connection
                cn = ConnectionManager.GetSqlConnectionFromKey(plant.PlantCode + ".SHOPFLOOR");
                cn.Open();

                // Integration Connection
                cnInt = ConnectionManager.GetSqlConnectionFromKey("CORP.INTEGRATION");
                cnInt.Open();

                // Process
                var mgr = new HvIssueUploadManager(session);
                return mgr.Process(session, cn, cnInt, plant, status);
            }
            catch (Exception err)
            {
                throw new ApplicationException(err.ToString());
            }
            finally
            {
                session.Close();
                cn.Close();
                cnInt.Close();
            }
        }

        public ProcessResponse HvWoIssueUpload(string plantCode, string status)
        {
            var session = new BusinessSystemSession();
            var cn = new SqlConnection();
            var cnInt = new SqlConnection();
            try
            {
                // resolve Plant
                var plant = PlantManager.GetPlant(plantCode);

                // Biz Session
                session = BusinessSystemSessionManager.GetBusinessSession();

                // Shop Floor Connection
                cn = ConnectionManager.GetSqlConnectionFromKey(plant.PlantCode + ".SHOPFLOOR");
                cn.Open();

                // Integration Connection
                cnInt = ConnectionManager.GetSqlConnectionFromKey("CORP.INTEGRATION");
                cnInt.Open();

                // Process
                var mgr = new HvWoIssueUploadManager(session);
                return mgr.Process(session, cn, cnInt, plant, status);
            }
            catch (Exception err)
            {
                throw new ApplicationException(err.ToString());
            }
            finally
            {
                session.Close();
                cn.Close();
                cnInt.Close();
            }
        }

        public ProcessResponse RackTransactionUpload(string plantCode, string status)
        {
            var session = new BusinessSystemSession();
            var cn = new SqlConnection();
            var cnInt = new SqlConnection();
            try
            {
                // resolve Plant
                var plant = PlantManager.GetPlant(plantCode);

                // Biz Session
                session = BusinessSystemSessionManager.GetBusinessSession();

                // Shop Floor Connection
                cn = ConnectionManager.GetSqlConnectionFromKey(plant.PlantCode + ".SHOPFLOOR");
                cn.Open();

                // Integration Connection
                cnInt = ConnectionManager.GetSqlConnectionFromKey("CORP.INTEGRATION");
                cnInt.Open();

                // Process
                var mgr = new RackTransactionUploadManager(session);
                return mgr.Process(session, cn, cnInt, plant, status);
            }
            catch (Exception err)
            {
                throw new ApplicationException(err.ToString());
            }
            finally
            {
                session.Close();
                cn.Close();
                cnInt.Close();
            }
        }

        public static ProcessResponse UnitBusinessTransactionUpload(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var response = new ProcessResponse($"{plant.PlantCode} Unit Bus Tran Upload Processor");
            try
            {
                var statusCode = Data.UnboxParm.To(parms, "StatusCode", string.Empty);
                var mgr = new UnitBusinessTransactionUploadManager(session);
                response = mgr.Process(session, sqlCns, plant, statusCode);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
            }
            response.Close();
            return response;
        }

        public static ProcessResponse GlassBusinessTransactionUpload(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var response = new ProcessResponse($"{plant.PlantCode} Glass Bus Tran Upload Processor");
            try
            {
                var statusCode = Data.UnboxParm.To(parms, "StatusCode", string.Empty);
                var mgr = new GlassBusinessTransactionUploadManager(session);
                response = mgr.Process(session, sqlCns, plant, statusCode);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
            }
            response.Close();
            return response;
        }


    }
}
