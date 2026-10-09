using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using IGServices.Common.Managers.E1;
using IGServices.Common.Managers.Enterprise;
using IGServices.Common.Managers.Process;
using IGServices.Common.Managers.ShopFloor;
using IGServices.Common.Types.E1;
using IGServices.Common.Types.Enterprise;
using IGServices.Common.Types.Response;
using IGServices.Common.Utilities;
using NLog;

namespace IGServices.RackSvc
{
    public class RackProcessor
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /********************************************************************************
        * METHODS
        ********************************************************************************/
        //public static ProcessResponse DownloadRackAssetXref(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        //{
        //    var response = new ProcessResponse($"{plant.PlantCode} Rack Asset Xref Download Manager");
        //    try
        //    {
        //        var timeSpan = Data.UnboxParm.To(parms, "TimeSpan", new TimeSpan());
        //        var useHighWaterMark = Data.UnboxParm.To(parms, "HighWaterMark", false);
        //        var assetTypeCodes = Data.UnboxParm.To(parms, "AssetTypeCodes", new List<string>());
        //        var mgr = new RackAssetXrefDownloadManager(session);
        //        response = mgr.Process(session, sqlCns, plant, timeSpan, useHighWaterMark, assetTypeCodes);
        //    }
        //    catch (Exception e)
        //    {
        //        response.Errors.Add(new Error(e));
        //    }
        //    response.Done();
        //    return response;
        //}

        //public static ProcessResponse DownloadRackAssetTypes(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        //{
        //    var response = new ProcessResponse($"{plant.PlantCode} Rack Asset Type Download Manager");
        //    try
        //    {
        //        var timeSpan = Data.UnboxParm.To(parms, "TimeSpan", new TimeSpan());
        //        var useHighWaterMark = Data.UnboxParm.To(parms, "HighWaterMark", false);
        //        var assetTypeCodes = Data.UnboxParm.To(parms, "AssetTypeCodes", new List<string>());
        //        var mgr = new RackAssetTypeDownloadManager(session);
        //        response = mgr.Process(session, sqlCns, plant, timeSpan, useHighWaterMark, assetTypeCodes);
        //    }
        //    catch (Exception e)
        //    {
        //        response.Errors.Add(new Error(e));
        //    }
        //    response.Done();
        //    return response;
        //}

        //public static ProcessResponse DownloadRackAssets(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        //{
        //    var response = new ProcessResponse($"{plant.PlantCode} Rack Asset Download Manager");
        //    try
        //    {
        //        var timeSpan = Data.UnboxParm.To(parms, "TimeSpan", new TimeSpan());
        //        var useHighWaterMark = Data.UnboxParm.To(parms, "HighWaterMark", false);
        //        var rackId = Data.UnboxParm.To<decimal>(parms, "RackId", 0);
        //        var statusCodes = Data.UnboxParm.To(parms, "StatusCodes", new List<string>());
        //        var mgr = new RackAssetDownloadManager(session);
        //        response = mgr.Process(session, sqlCns, plant, timeSpan, useHighWaterMark, rackId, statusCodes);
        //    }
        //    catch (Exception e)
        //    {
        //        response.Errors.Add(new Error(e));
        //    }
        //    response.Done();
        //    return response;
        //}

        //public static ProcessResponse DownloadRackEvents(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        //{
        //    var response = new ProcessResponse($"{plant.PlantCode} Rack Download Manager");
        //    try
        //    {
        //        var timeSpan = Data.UnboxParm.To(parms, "TimeSpan", new TimeSpan());
        //        var useHighWaterMark = Data.UnboxParm.To(parms, "HighWaterMark", false);
        //        var rackId = Data.UnboxParm.To<decimal>(parms, "RackId", 0);
        //        var statusCodes = Data.UnboxParm.To(parms, "StatusCodes", new List<string>());
        //        var mgr = new RackEventDownloadManager(session);
        //        response = mgr.Process(session, sqlCns, plant, timeSpan, useHighWaterMark, rackId, statusCodes);
        //    }
        //    catch (Exception e)
        //    {
        //        response.Errors.Add(new Error(e));
        //    }
        //    response.Done();
        //    return response;
        //}

        public static ProcessResponse DownloadRackItemEvents(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var response = new ProcessResponse($"{plant.PlantCode} Rack Item Download Manager");
            try
            {
                var timeSpan = Data.UnboxParm.To(parms, "TimeSpan", new TimeSpan());
                var useHighWaterMark = Data.UnboxParm.To(parms, "HighWaterMark", false);
                var rackId = Data.UnboxParm.To<decimal>(parms, "RackId", 0);
                var statusCodes = Data.UnboxParm.To(parms, "StatusCodes", new List<string>());
                var mgr = new RackItemEventDownloadManager(session);
                //response = mgr.Process(session, sqlCns, plant, timeSpan, useHighWaterMark, rackId, statusCodes);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
            }
            response.Done();
            return response;
        }

        public static ProcessResponse DownloadRacks(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var response = new ProcessResponse($"{plant.PlantCode} Rack Download Manager");
            try
            {
                var timeSpan = Data.UnboxParm.To(parms, "TimeSpan", new TimeSpan());
                var useHighWaterMark = Data.UnboxParm.To(parms, "HighWaterMark", false);
                var rackId = Data.UnboxParm.To<decimal>(parms, "RackId", 0);
                var statusCodes = Data.UnboxParm.To(parms, "StatusCodes", new List<string>());
                var mgr = new RackDownloadManager(session);
                response = mgr.Process(session, sqlCns, plant, timeSpan, useHighWaterMark, rackId, statusCodes);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
            }
            response.Done();
            return response;
        }

        public static ProcessResponse DownloadRackDetails(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var response = new ProcessResponse($"{plant.PlantCode} Rack Item Download Processor");
            try
            {
                var timeSpan = Data.UnboxParm.To(parms, "TimeSpan", new TimeSpan());
                var useHighWaterMark = Data.UnboxParm.To(parms, "HighWaterMark", false);
                var rackId = Data.UnboxParm.To<decimal>(parms, "RackId", 0);
                var statusCodes = Data.UnboxParm.To(parms, "StatusCodes", new List<string>());
                var mgr = new RackDetailDownloadManager(session);
                response.ProcessResponses.Add(mgr.Process(session, sqlCns, plant, timeSpan, useHighWaterMark, rackId,
                    statusCodes));
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
            }
            response.Done();
            return response;
        }

        public static ProcessResponse DownloadRackItems(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var response = new ProcessResponse($"{plant.PlantCode} Rack Item Download Processor");
            try
            {
                var timeSpan = Data.UnboxParm.To(parms, "TimeSpan", new TimeSpan());
                var useHighWaterMark = Data.UnboxParm.To(parms, "HighWaterMark", false);
                var rackId = Data.UnboxParm.To<decimal>(parms, "RackId", 0);
                var statusCodes = Data.UnboxParm.To(parms, "StatusCodes", new List<string>());
                var mgr = new RackItemDownloadManager(session);
                response.ProcessResponses.Add(mgr.Process(session, sqlCns, plant, timeSpan, useHighWaterMark, rackId,
                    statusCodes));
                //response = mgr.Process(session, sqlCns, plant, timeSpan, useHighWaterMark, rackId, statusCodes);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
            }
            response.Done();
            return response;
        }

        public static ProcessResponse DownloadRawTags(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var response = new ProcessResponse($"{plant.PlantCode} Rack Item Download Manager");
            try
            {
                var timeSpan = Data.UnboxParm.To(parms, "TimeSpan", new TimeSpan());
                var useHighWaterMark = Data.UnboxParm.To(parms, "HighWaterMark", false);
                var rackId = Data.UnboxParm.To<decimal>(parms, "RackId", 0);
                var mgr = new RawTagsDownloadManager(session);
                response = mgr.Process(session, sqlCns, plant, timeSpan, useHighWaterMark, rackId);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
            }
            response.Done();
            return response;
        }

        public static ProcessResponse DownloadCurrentTags(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var response = new ProcessResponse($"{plant.PlantCode} Rack Item Download Manager");
            try
            {
                var timeSpan = Data.UnboxParm.To(parms, "TimeSpan", new TimeSpan());
                var useHighWaterMark = Data.UnboxParm.To(parms, "HighWaterMark", false);
                var rackId = Data.UnboxParm.To<decimal>(parms, "RackId", 0);
                var serialNum = Data.UnboxParm.To<string>(parms, "SerialNum", null);
                response = CurrentTagsDownloadManager.Process(session, sqlCns, plant, timeSpan, useHighWaterMark, rackId, serialNum);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
            }
            response.Done();
            return response;
        }

        //public static RackLedgerItems GetRackLedger(decimal rackId, string serialNum, string plantCode, string glCategory, double days, bool attributes)
        //{
        //    //if (rackId == 0 & serialNum == null)
        //    //{
        //    //    throw new ApplicationException("Rack Id and/or Serial Num is REQUIRED");
        //    //}
        //    var e1Session = GetE1Session();
        //    RackLedgerItems tags;
        //    try
        //    {
        //        if (attributes)
        //        {
        //            var tagManager = new RackLedgerManager(e1Session);
        //            tags = tagManager.GetRackLedgerItems(e1Session, rackId, serialNum, plantCode, glCategory, days, true);
        //        }
        //        else
        //        {
        //            var tagManager = new RackStatusManager(e1Session);
        //            tags = tagManager.GetRackStatusItems(e1Session, rackId, serialNum, plantCode, glCategory, days, false);
        //        }
        //    }
        //    catch (Exception err)
        //    {
        //        throw new ApplicationException(err.ToString());
        //    }
        //    finally
        //    {
        //        CloseE1Session(e1Session);
        //    }
        //    return tags;
        //}

        //public static SqlBulkCopyResponse PersistRackLedger(decimal rackId, string serialNum, string plantCode, string glCategory, double days, bool attributes)
        //{
        //    //if (String.IsNullOrEmpty(plantCode))
        //    //{
        //    //    throw new ApplicationException("Plant Code is REQUIRED");
        //    //}
        //    var e1Session = GetE1Session();
        //    try
        //    {
        //        RackLedgerItems tags;
        //        if (attributes)
        //        {
        //            var tagManager = new RackLedgerManager(e1Session);
        //            tags = tagManager.GetRackLedgerItems(e1Session, rackId, serialNum, plantCode, glCategory, days, attributes);
        //            return tagManager.PersistRackLedger(e1Session, plantCode, tags);
        //        }
        //        else
        //        {
        //            var tagManager = new RackStatusManager(e1Session);
        //            tags = tagManager.GetRackStatusItems(e1Session, rackId, serialNum, plantCode, glCategory, days, attributes);
        //            return tagManager.PersistRackStatus(e1Session, plantCode, tags);
        //        }
        //    }
        //    catch (Exception err)
        //    {
        //        throw new ApplicationException(err.ToString());
        //    }
        //    finally
        //    {
        //        CloseE1Session(e1Session);
        //    }
        //}

        //public static Racks GetRacks(decimal rackId, string serialNum, string plantCode, string glCategory, double days, bool attributes)
        //{
        //    var e1Session = GetE1Session();
        //    try
        //    {
        //        var rackManager = new RackManager1(e1Session);
        //        return rackManager.GetRacks(e1Session, rackId, serialNum, plantCode, glCategory, days, attributes);
        //    }
        //    catch (Exception err)
        //    {
        //        throw new ApplicationException(err.ToString());
        //    }
        //    finally
        //    {
        //        CloseE1Session(e1Session);
        //    }
        //}

        //public static Racks GetPurchasedRacks(string plantCode)
        //{
        //    var e1Session = GetE1Session();
        //    try
        //    {
        //        var rackManager = new RackManager1(e1Session);
        //        return rackManager.GetPurchasedRacks(e1Session, plantCode);
        //    }
        //    catch (Exception err)
        //    {
        //        throw new ApplicationException(err.ToString());
        //    }
        //    finally
        //    {
        //        CloseE1Session(e1Session);
        //    }
        //}

        //public static RackItems GetRackItems(decimal rackId, string serialNum, bool attributes)
        //{
        //    var e1Session = GetE1Session();
        //    try
        //    {
        //        var rackItemManager = new RackItemManager(e1Session);
        //        return rackItemManager.GetRackItems(e1Session, rackId, serialNum, attributes);
        //    }
        //    catch (Exception err)
        //    {
        //        throw new ApplicationException(err.ToString());
        //    }
        //    finally
        //    {
        //        CloseE1Session(e1Session);
        //    }
        //}

        //public static RackItemEvents GetRackItemEvents(string plantCode, decimal ukid)
        //{
        //    var e1Session = GetE1Session();
        //    try
        //    {
        //        var mgr = new RackItemEventManager(e1Session);
        //        return mgr.GetRackItemEvents(e1Session, plantCode, ukid);
        //    }
        //    catch (Exception err)
        //    {
        //        throw new ApplicationException(err.ToString());
        //    }
        //    finally
        //    {
        //        CloseE1Session(e1Session);
        //    }
        //}


        //public static SqlBulkCopyResponse DownloadRackItemEvents(string plantCode, decimal ukid)
        //{
        //    var e1Session = GetE1Session();
        //    var cn = new SqlConnection();
        //    try
        //    {
        //        var plant = PlantManager.GetPlant(plantCode);
        //        cn = ConnectionManager.GetSqlConnectionFromKey(plant.PlantNum + ".INTEGRATION");
        //        cn.Open();

        //        var mgr1 = new RackItemEventManager(e1Session);
        //        var entRackItemEvents = mgr1.GetRackItemEvents(e1Session, plantCode, ukid);
        //        var mgr2 = new Common.Managers.ShopFloor.RackItemEventManager();
        //        var sfRackItems = mgr2.GetRackItemEvents(entRackItemEvents);
        //        return mgr2.DownloadRackItemEvents(cn, sfRackItems);
        //    }
        //    catch (Exception err)
        //    {
        //        throw new ApplicationException(err.ToString());
        //    }
        //    finally
        //    {
        //        cn.Close();
        //        CloseE1Session(e1Session);
        //    }
        //}


        //public static ProcessResponse DownloadRackItems(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        //{
        //    var response = new ProcessResponse($"{plant.PlantCode} Rack Item Download Manager");
        //    try
        //    {
        //        var timeSpan = Data.UnboxParm.To(parms, "TimeSpan", new TimeSpan());
        //        var useHighWaterMark = Data.UnboxParm.To(parms, "HighWaterMark", false);
        //        var rackId = Data.UnboxParm.To<decimal>(parms, "RackId", 0);
        //        var statusCodes = Data.UnboxParm.To(parms, "StatusCodes", new List<string>());
        //        var mgr = new RackItemDownloadManager(session);
        //        response = mgr.Process(session, sqlCns, plant, timeSpan, useHighWaterMark, rackId, statusCodes);
        //    }
        //    catch (Exception e)
        //    {
        //        response.Errors.Add(new Error(e));
        //    }
        //    response.Done();
        //    return response;
        //}

        //public static ProcessResponse DownloadRackItems(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        //{
        //    var response = new ProcessResponse($"{plant.PlantCode} Rack Item Download Manager");
        //    try
        //    {
        //        var timeSpan = Data.UnboxParm.To(parms, "TimeSpan", new TimeSpan());
        //        var useHighWaterMark = Data.UnboxParm.To(parms, "HighWaterMark", false);
        //        var rackId = Data.UnboxParm.To<decimal>(parms, "RackId", 0);
        //        var statusCodes = Data.UnboxParm.To(parms, "StatusCodes", new List<string>());
        //        var mgr = new RackItemDownloadManager(session);
        //        response = mgr.Process(session, sqlCns, plant, timeSpan, useHighWaterMark, rackId, statusCodes);
        //    }
        //    catch (Exception e)
        //    {
        //        response.Errors.Add(new Error(e));
        //    }
        //    response.Close();
        //    return response;
        //}

        //public static ProcessResponse DownloadRackLedger(string plantCode, decimal rackId, bool attributes)
        //{
        //    var response = new ProcessResponse("Rack Ledger Download");
        //    var session = BusinessSystemSessionManager.GetBusinessSession();
        //    response.ConnectionStrings.Add(new ConnectionString("E1", session.DbConnection.ConnectionString));
        //    var cn = new SqlConnection();
        //    try
        //    {
        //        // resolve Plant
        //        var plant = PlantManager.GetPlant(plantCode);

        //        // Shop Floor Connection
        //        cn = ConnectionManager.GetSqlConnectionFromKey(plant.PlantCode + ".INTEGRATION");
        //        response.ConnectionStrings.Add(new ConnectionString("INTEGRATION", cn.ConnectionString));
        //        cn.Open();

        //        if (attributes)
        //        {
        //            var rkMgr = new RackLedgerDownloadManager();
        //            response.ProcessResponses.Add(rkMgr.ProcessAttributes(session, cn, plant, rackId));
        //        }
        //        else
        //        {
        //            var rkMgr = new RackLedgerDownloadManager();
        //            response.ProcessResponses.Add(rkMgr.ProcessStatus(session, cn, plant, rackId));
        //        }
        //    }
        //    catch (Exception err)
        //    {
        //        throw new ApplicationException(err.ToString());
        //    }
        //    finally
        //    {
        //        cn.Close();
        //        session.Close();
        //    }
        //    response.Close();
        //    return response;
        //}

        //public static ProcessResponse DownloadRawTags(string plantCode, decimal rackId)
        //{
        //    var response = new ProcessResponse("Raw Tags Download");
        //    var session = BusinessSystemSessionManager.GetBusinessSession();
        //    response.ConnectionStrings.Add(new ConnectionString("E1", session.DbConnection.ConnectionString));
        //    var cn = new SqlConnection();
        //    try
        //    {
        //        // resolve Plant
        //        var plant = PlantManager.GetPlant(plantCode);

        //        // Shop Floor Connection
        //        cn = ConnectionManager.GetSqlConnectionFromKey(plant.PlantCode + ".SHOPFLOOR");
        //        response.ConnectionStrings.Add(new ConnectionString("SHOPFLOOR", cn.ConnectionString));
        //        cn.Open();

        //        // Processes
        //        var rtMgr = new RawTagsDownloadManager();
        //        response.ProcessResponses.Add(rtMgr.Process(session, cn, plant, rackId));
        //    }
        //    catch (Exception err)
        //    {
        //        throw new ApplicationException(err.ToString());
        //    }
        //    finally
        //    {
        //        cn.Close();
        //        session.Close();
        //    }
        //    response.Close();
        //    return response;
        //}

        /*
        public SqlBulkCopyResponses DownloadRacks(string plantCode, decimal rackId, string glCategory)
        {
            var e1Session = GetE1Session();
            var cn = new SqlConnection();
            //var responses = new SqlBulkCopyResponses();
            try
            {
                var plant = e1Session.PlantMap.GetAttributebyId(plantCode);
                cn = ConnectionManager.GetSqlConnectionFromKey(plant.Code + ".INTEGRATION");
                cn.Open();

                //var mgr1 = new IGServices.Common.Managers.Enterprise.RackManager(e1Session);
                //var entRacks = mgr1.GetRacks(e1Session, rackId, null, plantCode, glCategory, 0, true);
                var mgr1 = new IGServices.Common.Managers.Enterprise.RackManager1(e1Session);
                var entRacks = mgr1.GetRacks(e1Session, 0, null, plantCode, null, 0, true );
                var mgr2 = new IGServices.Common.Managers.ShopFloor.RackManager();
                var sfRackItems = mgr2.GetRacks(entRacks);
                return mgr2.DownloadRacks(cn, sfRackItems);
            }
            catch (Exception err)
            {
                throw new ApplicationException(err.ToString());
            }
            finally
            {
                cn.Close();
                CloseE1Session(e1Session);
            }
        }
        */

        //public static RawTags GetRawTags(string plantCode, decimal rackId, string serialNum)
        //{
        //    if (String.IsNullOrEmpty(plantCode))
        //    {
        //        throw new ApplicationException("Plant Code is REQUIRED");
        //    }
        //    try
        //    {
        //        var plant = PlantManager.GetPlant(plantCode);
        //        var tagManager = new RawTagManager();
        //        return tagManager.GetRawTags(plant.PlantNum, rackId, serialNum ?? "");
        //    }
        //    catch (Exception err)
        //    {
        //        throw new ApplicationException(err.ToString());
        //    }
        //}

        //private static BusinessSystemSession GetE1Session()
        //{
        //    var e1Session = BusinessSystemSessionManager.GetBusinessSession();
        //    return e1Session;
        //}

        //private static void CloseE1Session(BusinessSystemSession e1Session)
        //{
        //    var e1SessionMgr = new BusinessSystemSessionManager();
        //    e1SessionMgr.CloseBusinessSession(e1Session);
        //}

        //public static ProcessResponse DownloadRackEvents(string plants, TimeSpan span, bool useHighWaterMark, decimal rackId)
        //{
        //    var response = new ProcessResponse("Download Racks");
        //    var plantList = plants.Split(',').ToList();
        //    var exceptions = new ConcurrentQueue<Exception>();

        //    try
        //    {
        //        Parallel.ForEach(plantList, plantItem =>
        //        {
        //            var session = BusinessSystemSessionManager.GetBusinessSession();
        //            var cn = new SqlConnection();
        //            var plantResponse = new ProcessResponse(plantItem);
        //            try
        //            {
        //                // resolve Plant
        //                var plant = PlantManager.GetPlant(plantItem);

        //                // Shop Floor Connection
        //                cn = ConnectionManager.GetSqlConnectionFromKey(plant.PlantCode + ".INTEGRATION");
        //                cn.Open();

        //                // Racks
        //                var mark1 = (useHighWaterMark)
        //                    ? HighWaterMarkManager.GetHighWaterMark(cn, plant, "E1_F580559")
        //                    : null;
        //                var processRunSpan1 = ProcessRunSpanManager.GetRunSpan(mark1, span,
        //                    E1TimeManager.GetE1TimeStamp(session));

        //                var rkMgr1 = new RackEventDownloadManager(session);
        //                var procResponse1 = rkMgr1.Process(session, cn, plant, processRunSpan1, rackId);
        //                plantResponse.ProcessResponses.Add(procResponse1);

        //                // Rack Items
        //                var mark2 = (useHighWaterMark)
        //                    ? HighWaterMarkManager.GetHighWaterMark(cn, plant, "E1_F580569")
        //                    : null;
        //                var processRunSpan2 = ProcessRunSpanManager.GetRunSpan(mark2, span,
        //                    E1TimeManager.GetE1TimeStamp(session));

        //                var rkMgr2 = new RackItemEventDownloadManager(session);
        //                var procResponse2 = rkMgr2.Process(session, cn, plant, processRunSpan1, rackId);
        //                plantResponse.ProcessResponses.Add(procResponse2);

        //            }
        //            catch (Exception e)
        //            {
        //                plantResponse.Errors.Add(new Error(e));
        //                exceptions.Enqueue(e);
        //            }
        //            finally
        //            {
        //                cn.Close();
        //                session.Close();
        //                plantResponse.Done();
        //                response.ProcessResponses.Add(plantResponse);
        //            }
        //        });
        //        if (exceptions.Count > 0) throw new AggregateException(exceptions);
        //    }
        //    catch (AggregateException es)
        //    {
        //        foreach (var e in es.InnerExceptions)
        //        {
        //            var ez = new Error(e);
        //        }
        //    }
        //    finally
        //    {
        //        response.Close();
        //    }
        //    return response;
        //}

        //public static ProcessResponse DownloadRacks(string plants, TimeSpan span, bool useHighWaterMark, decimal rackId, string status)
        //{
        //    var response = new ProcessResponse("Download Rack Events");
        //    var plantList = plants.Split(',').ToList();
        //    var exceptions = new ConcurrentQueue<Exception>();

        //    try
        //    {
        //        Parallel.ForEach(plantList, plantItem =>
        //        {
        //            var session = BusinessSystemSessionManager.GetBusinessSession();
        //            var cn = new SqlConnection();
        //            var plantResponse = new ProcessResponse(plantItem);
        //            try
        //            {
        //                // resolve Plant
        //                var plant = PlantManager.GetPlant(plantItem);

        //                // Shop Floor Connection
        //                cn = ConnectionManager.GetSqlConnectionFromKey(plant.PlantCode + ".INTEGRATION");
        //                cn.Open();

        //                // Racks
        //                var mark1 = (useHighWaterMark)
        //                    ? HighWaterMarkManager.GetHighWaterMark(cn, plant, "E1_F580055")
        //                    : null;
        //                var processRunSpan1 = ProcessRunSpanManager.GetRunSpan(mark1, span,
        //                    E1TimeManager.GetE1TimeStamp(session));

        //                var rkMgr1 = new RackDownloadManager(session);
        //                var procResponse1 = rkMgr1.Process(session, cn, plant, processRunSpan1, rackId, status);
        //                plantResponse.ProcessResponses.Add(procResponse1);

        //                // Rack Items
        //                var mark2 = (useHighWaterMark)
        //                    ? HighWaterMarkManager.GetHighWaterMark(cn, plant, "E1_F580056")
        //                    : null;
        //                var processRunSpan2 = ProcessRunSpanManager.GetRunSpan(mark2, span,
        //                    E1TimeManager.GetE1TimeStamp(session));

        //                //var rkMgr2 = new RackItemDownloadManager(session);
        //                //var procResponse2 = rkMgr2.Process(session, cn, plant, processRunSpan1, rackId, status);
        //                //plantResponse.ProcessResponses.Add(procResponse2);

        //            }
        //            catch (Exception e)
        //            {
        //                plantResponse.Errors.Add(new Error(e));
        //                exceptions.Enqueue(e);
        //            }
        //            finally
        //            {
        //                cn.Close();
        //                session.Close();
        //                plantResponse.Close();
        //                response.ProcessResponses.Add(plantResponse);
        //            }
        //        });
        //        if (exceptions.Count > 0) throw new AggregateException(exceptions);
        //    }
        //    catch (AggregateException es)
        //    {
        //        foreach (var e in es.InnerExceptions)
        //        {
        //            var ez = new Error(e);
        //        }
        //    }
        //    finally
        //    {
        //        response.Close();
        //    }
        //    return response;
        //}

        public static ProcessResponse DownloadShippableRacks(string plantCode)
        {
            var response = new ProcessResponse("Shippable Racks Download");
            var session = BusinessSystemSessionManager.GetBusinessSession();
            var cn = new SqlConnection();
            try
            {
                // resolve Plant
                var plant = PlantManager.GetPlant(plantCode);

                // Shop Floor Connection
                cn = ConnectionManager.GetSqlConnectionFromKey(plant.PlantCode + ".INTEGRATION");
                cn.Open();

                // Response Properties
                response.Plant = plant;
                response.ConnectionStrings.Add(new ConnectionString("E1", session.DbConnection.ConnectionString));
                response.ConnectionStrings.Add(new ConnectionString("INT", cn.ConnectionString));

                // Download Manager
                var step1 = new ProcessStep("Shippable Rack Download Manager");
                try
                {
                    var rtMgr = new ShippableRackDownloadManager(session);
                    step1.ProcessResponses.Add(rtMgr.Process(session, cn, plant));
                    response.RowsAffected = step1.ProcessResponses[0].RowsAffected;
                }
                catch (Exception e)
                {
                    step1.Errors.Add(new Error(e));
                }
                step1.Done();
                response.AddStep(step1);
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {cn.ConnectionString}  {e.Message}: {e.StackTrace}");
                response.Errors.Add(new Error(e));
            }
            finally
            {
                cn.Close();
                session.Close();
            }
            response.Close();
            return response;
        }
    }
}
