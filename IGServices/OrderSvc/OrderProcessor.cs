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

namespace IGServices.OrderSvc
{
    public class OrderProcessor
    {

        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public static ProcessResponse DownloadSalesLines(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var response = new ProcessResponse($"{plant.PlantCode} Sales Line Download Manager");
            try
            {
                var timeSpan = Data.UnboxParm.To(parms, "TimeSpan", new TimeSpan());
                var useHighWaterMark = Data.UnboxParm.To(parms, "HighWaterMark", false);
                var workOrderNum = Data.UnboxParm.To<decimal>(parms, "SalesOrderNum", 0);
                var statusCodes = Data.UnboxParm.To(parms, "StatusCodes", new List<string>());
                response.ProcessResponses.Add(SalesLineDownloadManager.Process(session, sqlCns, plant, timeSpan, useHighWaterMark, workOrderNum, statusCodes));
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
            }
            response.Done();
            return response;
        }

        public static ProcessResponse DownloadWorkOrders(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var response = new ProcessResponse($"{plant.PlantCode} Work Order Download Manager");
            try
            {
                var timeSpan = Data.UnboxParm.To(parms, "TimeSpan", new TimeSpan());
                var useHighWaterMark = Data.UnboxParm.To(parms, "HighWaterMark", false);
                var workOrderNum = Data.UnboxParm.To<decimal>(parms, "WorkOrderNum", 0);
                var statusCodes = Data.UnboxParm.To(parms, "StatusCodes", new List<string>());
                response.ProcessResponses.Add(WorkOrderDownloadManager.Process(session, sqlCns, plant, timeSpan, useHighWaterMark, workOrderNum, statusCodes));
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
            }
            response.Done();
            return response;
        }

        public static ProcessResponse DownloadWorkOrderStatus(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var response = new ProcessResponse($"{plant.PlantCode} Work Order Status Download Manager");
            try
            {
                var timeSpan = Data.UnboxParm.To(parms, "TimeSpan", new TimeSpan());
                var useHighWaterMark = Data.UnboxParm.To(parms, "HighWaterMark", false);
                var workOrderNum = Data.UnboxParm.To<decimal>(parms, "WorkOrderNum", 0);
                response.ProcessResponses.Add(WorkOrderStatusDownloadManager.Process(session, sqlCns, plant, timeSpan, useHighWaterMark, workOrderNum));
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
            }
            response.Done();
            return response;
        }

        public static ProcessResponse DownloadRelatedOrders(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var response = new ProcessResponse($"{plant.PlantCode} Related Order Download Manager");
            try
            {
                var timeSpan = Data.UnboxParm.To(parms, "TimeSpan", new TimeSpan());
                var useHighWaterMark = Data.UnboxParm.To(parms, "HighWaterMark", false);
                response = RelatedOrderDownloadManager.Process(session, sqlCns, plant, timeSpan, useHighWaterMark);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
            }
            response.Done();
            return response;
        }

        public static ProcessResponse DownloadPurchasedLots(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var response = new ProcessResponse($"{plant.PlantCode} Purchased Lot Download Manager");
            try
            {
                var timeSpan = Data.UnboxParm.To(parms, "TimeSpan", new TimeSpan());
                var useHighWaterMark = Data.UnboxParm.To(parms, "HighWaterMark", false);
                response = PurchasedLotDownloadManager.Process(session, sqlCns, plant, timeSpan, useHighWaterMark);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
            }
            response.Done();
            return response;
        }

        public static ProcessResponse DownloadPurchaseOrders(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var response = new ProcessResponse($"{plant.PlantCode} Purchase Order Download Manager");
            try
            {
                var timeSpan = Data.UnboxParm.To(parms, "TimeSpan", new TimeSpan());
                var useHighWaterMark = Data.UnboxParm.To(parms, "HighWaterMark", false);
                var orderNum = Data.UnboxParm.To<decimal>(parms, "OrderNum", 0);
                response = PurchaseOrderDownloadManager.Process(session, sqlCns, plant, timeSpan, useHighWaterMark, orderNum);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
            }
            response.Done();
            return response;
        }

        public static ProcessResponse DownloadMtoSpecUnits(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var response = new ProcessResponse($"{plant.PlantCode} MTO Spec Unit Download Manager");
            try
            {
                var timeSpan = Data.UnboxParm.To(parms, "TimeSpan", new TimeSpan());
                var useHighWaterMark = Data.UnboxParm.To(parms, "HighWaterMark", false);
                var documentTypes = Data.UnboxParm.To(parms, "DocumentTypes", new List<string>());
                var documentNum = Data.UnboxParm.To<decimal>(parms, "DocumentNum", 0);
                var mgr = new SpecUnitMtoDownloadManager(session);
                response = mgr.Process(session, sqlCns, plant, timeSpan, useHighWaterMark, documentTypes, documentNum);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
            }
            response.Done();
            return response;
        }

        public static ProcessResponse DownloadMtsSpecUnits(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var response = new ProcessResponse($"{plant.PlantCode} MTO Spec Unit Download Manager");
            try
            {
                var timeSpan = Data.UnboxParm.To(parms, "TimeSpan", new TimeSpan());
                var useHighWaterMark = Data.UnboxParm.To(parms, "HighWaterMark", false);
                var itemNum = Data.UnboxParm.To<decimal>(parms, "ItemNum", 0);
                var mgr = new SpecUnitMtsDownloadManager(session);
                response = mgr.Process(session, sqlCns, plant, timeSpan, useHighWaterMark, itemNum);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
            }
            response.Done();
            return response;
        }

        public static ProcessResponse DownloadPartList(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var response = new ProcessResponse($"{plant.PlantCode} Part List Download Manager");
            try
            {
                var timeSpan = Data.UnboxParm.To(parms, "TimeSpan", new TimeSpan());
                var useHighWaterMark = Data.UnboxParm.To(parms, "HighWaterMark", false);
                var orderNum = Data.UnboxParm.To<decimal>(parms, "OrderNum", 0);
                response = PartListDownloadManager.Process(session, sqlCns, plant, timeSpan, useHighWaterMark, orderNum);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
            }
            response.Done();
            return response;
        }
    }
}
