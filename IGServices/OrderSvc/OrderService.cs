using System;
using System.Collections.Generic;
using System.Linq;
using IGServices.Common.Managers.Process;
using IGServices.Common.Types.Enterprise;
using IGServices.Common.Types.Response;
using IGServices.Common.Types.ShopFloor;
using IGServices.OrderSvc;
using SoaCore.Wcf;

namespace IGServices.OrderSvc
{
    [ParameterInspectorLogger, CGIErrorHandler]
    public class OrderService : IOrderService
    {


        public ProcessResponse DownloadWorkOrders(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal workOrderNum = 0, string statusCodes = null)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                var parms = new Dictionary<string, object>();
                parms.Add("TimeSpan", timeSpan);
                parms.Add("HighWaterMark", useHighWaterMark);
                parms.Add("WorkOrderNum", workOrderNum);
                parms.Add("StatusCodes", statusCodes?.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList());
                //parms.Add("StatusCodes", (!String.IsNullOrEmpty(statusCodes)) ? statusCodes?.Split(',').ToList() : null);
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, OrderProcessor.DownloadWorkOrders, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadSalesLines(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal salesOrderNum = 0, string nextStatusCodes = null)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                var parms = new Dictionary<string, object>();
                parms.Add("TimeSpan", timeSpan);
                parms.Add("HighWaterMark", useHighWaterMark);
                parms.Add("WorkOrderNum", salesOrderNum);
                parms.Add("StatusCodes", nextStatusCodes?.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList());
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, OrderProcessor.DownloadSalesLines, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadWorkOrderStatus(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal workOrderNum = 0)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                var parms = new Dictionary<string, object>();
                parms.Add("TimeSpan", timeSpan);
                parms.Add("HighWaterMark", useHighWaterMark);
                parms.Add("WorkOrderNum", workOrderNum);
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, OrderProcessor.DownloadWorkOrderStatus, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadRelatedOrders(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                var parms = new Dictionary<string, object>();
                parms.Add("TimeSpan", timeSpan);
                parms.Add("HighWaterMark", useHighWaterMark);
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, OrderProcessor.DownloadRelatedOrders, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadPurchasedLots(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                var parms = new Dictionary<string, object>();
                parms.Add("TimeSpan", timeSpan);
                parms.Add("HighWaterMark", useHighWaterMark);
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, OrderProcessor.DownloadPurchasedLots, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadPurchaseOrders(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal orderNum)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                var parms = new Dictionary<string, object>();
                parms.Add("TimeSpan", timeSpan);
                parms.Add("HighWaterMark", useHighWaterMark);
                parms.Add("OrderNum", orderNum);
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, OrderProcessor.DownloadPurchaseOrders, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadMtoSpecUnits(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string documentTypes, decimal documentNum)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                var parms = new Dictionary<string, object>();
                parms.Add("TimeSpan", timeSpan);
                parms.Add("HighWaterMark", useHighWaterMark);
                parms.Add("DocumentNum", documentNum);
                parms.Add("DocumentTypes", documentTypes?.Split(',').ToList());
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, OrderProcessor.DownloadMtoSpecUnits, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadMtsSpecUnits(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal itemNum)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                var parms = new Dictionary<string, object>();
                parms.Add("TimeSpan", timeSpan);
                parms.Add("HighWaterMark", useHighWaterMark);
                parms.Add("ItemNum", itemNum);
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, OrderProcessor.DownloadMtsSpecUnits, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadPartList(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal orderNum)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                var parms = new Dictionary<string, object>();
                parms.Add("TimeSpan", timeSpan);
                parms.Add("HighWaterMark", useHighWaterMark);
                parms.Add("OrderNum", orderNum);
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, OrderProcessor.DownloadPartList, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

    }


    [ParameterInspectorLogger, CGIErrorHandler]
    public class OrderRestService : IOrderRestService
    {

        public ProcessResponse DownloadWorkOrders(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal workOrderNum = 0, string statusCodes = null)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                var parms = new Dictionary<string, object>();
                parms.Add("TimeSpan", timeSpan);
                parms.Add("HighWaterMark", useHighWaterMark);
                parms.Add("WorkOrderNum", workOrderNum);
                parms.Add("StatusCodes", statusCodes?.Split(',').ToList());
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, OrderProcessor.DownloadWorkOrders, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadSalesLines(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal salesOrderNum = 0, string nextStatusCodes = null)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                var parms = new Dictionary<string, object>();
                parms.Add("TimeSpan", timeSpan);
                parms.Add("HighWaterMark", useHighWaterMark);
                parms.Add("WorkOrderNum", salesOrderNum);
                parms.Add("StatusCodes", nextStatusCodes?.Split(',').ToList());
                //parms.Add("StatusCodes", nextStatusCodes?.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList());
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, OrderProcessor.DownloadSalesLines, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadWorkOrderStatus(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal workOrderNum = 0)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                var parms = new Dictionary<string, object>();
                parms.Add("TimeSpan", timeSpan);
                parms.Add("HighWaterMark", useHighWaterMark);
                parms.Add("WorkOrderNum", workOrderNum);
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, OrderProcessor.DownloadWorkOrderStatus, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadRelatedOrders(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                var parms = new Dictionary<string, object>();
                parms.Add("TimeSpan", timeSpan);
                parms.Add("HighWaterMark", useHighWaterMark);
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, OrderProcessor.DownloadRelatedOrders, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadPurchasedLots(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                var parms = new Dictionary<string, object>();
                parms.Add("TimeSpan", timeSpan);
                parms.Add("HighWaterMark", useHighWaterMark);
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, OrderProcessor.DownloadPurchasedLots, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadPurchaseOrders(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal orderNum)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                var parms = new Dictionary<string, object>();
                parms.Add("TimeSpan", timeSpan);
                parms.Add("HighWaterMark", useHighWaterMark);
                parms.Add("OrderNum", orderNum);
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, OrderProcessor.DownloadPurchaseOrders, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadMtoSpecUnits(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string documentTypes, decimal documentNum)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                var parms = new Dictionary<string, object>();
                parms.Add("TimeSpan", timeSpan);
                parms.Add("HighWaterMark", useHighWaterMark);
                parms.Add("DocumentNum", documentNum);
                parms.Add("DocumentTypes", documentTypes?.Split(',').ToList());
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, OrderProcessor.DownloadMtoSpecUnits, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadMtsSpecUnits(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal itemNum)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                var parms = new Dictionary<string, object>();
                parms.Add("TimeSpan", timeSpan);
                parms.Add("HighWaterMark", useHighWaterMark);
                parms.Add("ItemNum", itemNum);
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, OrderProcessor.DownloadMtsSpecUnits, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadPartList(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal orderNum)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                var parms = new Dictionary<string, object>();
                parms.Add("TimeSpan", timeSpan);
                parms.Add("HighWaterMark", useHighWaterMark);
                parms.Add("OrderNum", orderNum);
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, OrderProcessor.DownloadPartList, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

    }
}
