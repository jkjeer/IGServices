using System;
using System.Collections.Generic;
using System.Linq;
using IGServices.Common.Managers.Process;
using IGServices.Common.Types.Response;

namespace IGServices.RackSvc
{
    public class RackService : IRackService
    {

        //public ProcessResponse DownloadRackEvents(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal rackId = 0, string statusCodes = null)
        //{
        //    if (string.IsNullOrEmpty(plants))
        //    {
        //        throw new Exception("Plant Code is REQUIRED.");
        //    }
        //    try
        //    {
        //        var parms = new Dictionary<string, object>();
        //        parms.Add("TimeSpan", timeSpan);
        //        parms.Add("HighWaterMark", useHighWaterMark);
        //        parms.Add("RackId", rackId);
        //        parms.Add("StatusCodes", statusCodes?.Split(',').ToList());
        //        var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION" };
        //        return BusinessSystemTransferProcessor.Invoke(plants, RackProcessor.DownloadRackEvents, parallel, parms, resources);
        //        //return InvocationProcessor.Invoke(plants, RackProcessor.DownloadRackEvents, parallel, parms);
        //    }
        //    catch (Exception e)
        //    {
        //        throw new Exception(e.ToString());
        //    }
        //}

        public ProcessResponse DownloadRackItemEvents(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal rackId = 0, string statusCodes = null)
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
                parms.Add("RackId", rackId);
                parms.Add("StatusCodes", statusCodes?.Split(',').ToList());
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, RackProcessor.DownloadRackItemEvents, parallel, parms, resources);
                //return InvocationProcessor.Invoke(plants, RackProcessor.DownloadRackItemEvents, parallel, parms);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadRacks(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal rackId = 0, string statusCodes = null)
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
                parms.Add("RackId", rackId);
                parms.Add("StatusCodes", statusCodes?.Split(',').ToList());
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, RackProcessor.DownloadRacks, parallel, parms, resources);
                //return InvocationProcessor.Invoke(plants, RackProcessor.DownloadRacks, parallel, parms);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadRackDetails(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal rackId = 0, string statusCodes = null)
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
                parms.Add("RackId", rackId);
                parms.Add("StatusCodes", statusCodes?.Split(',').ToList());
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, RackProcessor.DownloadRackDetails, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadRackItems(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal rackId = 0, string statusCodes = null)
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
                parms.Add("RackId", rackId);
                parms.Add("StatusCodes", statusCodes?.Split(',').ToList());
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, RackProcessor.DownloadRackItems, parallel, parms, resources);
                //return InvocationProcessor.Invoke(plants, RackProcessor.DownloadRackItems, parallel, parms);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }


        public ProcessResponse DownloadRawTags(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal rackId = 0)
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
                parms.Add("RackId", rackId);
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, RackProcessor.DownloadRawTags, parallel, parms, resources);
                //return InvocationProcessor.Invoke(plants, RackProcessor.DownloadRawTags, parallel, parms);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadCurrentTags(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal rackId, string serialNum)
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
                parms.Add("RackId", rackId);
                parms.Add("SerialNum", serialNum);
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION", "CORP.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, RackProcessor.DownloadCurrentTags, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadShippableRacks(string plantCode)
        {
            try
            {
                return RackProcessor.DownloadShippableRacks(plantCode);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }

    public class RackRestService : IRackRestService
    {

        //public ProcessResponse DownloadRackEvents(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal rackId = 0, string statusCodes = null)
        //{
        //    if (string.IsNullOrEmpty(plants))
        //    {
        //        throw new Exception("Plant Code is REQUIRED.");
        //    }
        //    try
        //    {
        //        var parms = new Dictionary<string, object>();
        //        parms.Add("TimeSpan", timeSpan);
        //        parms.Add("HighWaterMark", useHighWaterMark);
        //        parms.Add("RackId", rackId);
        //        parms.Add("StatusCodes", statusCodes?.Split(',').ToList());
        //        var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION" };
        //        return BusinessSystemTransferProcessor.Invoke(plants, RackProcessor.DownloadRackEvents, parallel, parms, resources);
        //        //return InvocationProcessor.Invoke(plants, RackProcessor.DownloadRackEvents, parallel, parms);
        //    }
        //    catch (Exception e)
        //    {
        //        throw new Exception(e.ToString());
        //    }
        //}

        public ProcessResponse DownloadRackItemEvents(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal rackId = 0, string statusCodes = null)
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
                parms.Add("RackId", rackId);
                parms.Add("StatusCodes", statusCodes?.Split(',').ToList());
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, RackProcessor.DownloadRackItemEvents, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadRacks(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal rackId = 0, string statusCodes = null)
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
                parms.Add("RackId", rackId);
                parms.Add("StatusCodes", statusCodes?.Split(',').ToList());
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, RackProcessor.DownloadRacks, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadRackDetails(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal rackId = 0, string statusCodes = null)
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
                parms.Add("RackId", rackId);
                parms.Add("StatusCodes", statusCodes?.Split(',').ToList());
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, RackProcessor.DownloadRackDetails, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadRackItems(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal rackId = 0, string statusCodes = null)
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
                parms.Add("RackId", rackId);
                parms.Add("StatusCodes", statusCodes?.Split(',').ToList());
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, RackProcessor.DownloadRackItems, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadRawTags(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal rackId = 0)
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
                parms.Add("RackId", rackId);
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, RackProcessor.DownloadRawTags, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadCurrentTags(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal rackId, string serialNum)
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
                parms.Add("RackId", rackId);
                parms.Add("SerialNum", serialNum);
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION", "CORP.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, RackProcessor.DownloadCurrentTags, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadShippableRacks(string plantCode)
        {
            var processor = new RackProcessor();
            try
            {
                return RackProcessor.DownloadShippableRacks(plantCode);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

    }
}
