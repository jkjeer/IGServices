using System;
using System.Collections.Generic;
using System.Linq;
using IGServices.Common.Managers.Process;
using IGServices.Common.Types.Enterprise;
using IGServices.Common.Types.Response;
using IGServices.OrderSvc;
using SoaCore.Wcf;
using SpacerItems = IGServices.Common.Types.Enterprise.SpacerItems;

namespace IGServices.ItemSvc
{
    [ParameterInspectorLogger, CGIErrorHandler]
    public class ItemService : IItemService
    {

        public ItemAttributes GetGlassItemAttributes(decimal itemNum, decimal documentNum = 0, string documentType = null, decimal documentLineNum = 0, string companyCode = null)
        {
            var processor = new ItemProcessor();
            return processor.GetGlassItemAttributes(itemNum, documentNum, documentType, documentLineNum, companyCode);
        }

        public GlassItem GetGlassItem(decimal itemNum, string plantCode, decimal documentNum = 0, string documentType = null, decimal documentLineNum = 0, string companyCode = null)
        {
            var processor = new ItemProcessor();
            return processor.GetGlassItem(itemNum, plantCode, documentNum, documentType, documentLineNum, companyCode);
        }

        public GlassItems GetDiscreteGlassItems(string plantCode, decimal itemNum = 0, string stockingTypes = null, string categoryCodes = null, TimeSpan? timeSpan = null)
        {
            try
            {
                var processor = new ItemProcessor();
                var stockingTypesList = new List<string>();
                var categoryCodesList = new List<string>();
                if (!string.IsNullOrEmpty(stockingTypes))
                {
                    stockingTypesList = stockingTypes.Split(',').ToList();
                }
                if (!string.IsNullOrEmpty(categoryCodes))
                {
                    categoryCodesList = categoryCodes.Split(',').ToList();
                }
                return processor.GetDiscreteGlassItems(plantCode, itemNum, stockingTypesList, categoryCodesList, timeSpan);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.Message);
            }
        }

        public SpacerItems GetSpacerItems(string plantCode, TimeSpan timeSpan, bool useHighWaterMark, decimal itemNum)
        {
            if (string.IsNullOrEmpty(plantCode))
            {
                throw new ApplicationException("Plant Code is REQUIRED. ");
            }
            try
            {
                var processor = new ItemProcessor();
                return processor.GetSpacerItems(plantCode, timeSpan, useHighWaterMark, itemNum);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.Message);
            }
        }


        public MuntinBarItems GetMuntinBarItems(string plantCode, TimeSpan timeSpan, bool useHighWaterMark, decimal itemNum)
        {
            if (string.IsNullOrEmpty(plantCode))
            {
                throw new ApplicationException("Plant Code is REQUIRED. ");
            }
            try
            {
                var processor = new ItemProcessor();
                return processor.GetMuntinBarItems(plantCode, timeSpan, useHighWaterMark, itemNum);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.Message);
            }
        }

        /* DOWNLOADS */
        public ProcessResponse DownloadSpacerItems(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal itemNum)
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
                return BusinessSystemTransferProcessor.Invoke(plants, ItemProcessor.DownloadSpacerItems, parallel, parms, resources);
                //return InvocationProcessor.Invoke(plants, ItemProcessor.DownloadSpacerItems, parallel, parms);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadMuntinBarItems(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal itemNum)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED");
            }
            try
            {
                var parms = new Dictionary<string, object>();
                parms.Add("TimeSpan", timeSpan);
                parms.Add("HighWaterMark", useHighWaterMark);
                parms.Add("ItemNum", itemNum);
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, ItemProcessor.DownloadMuntinBarItems, parallel, parms, resources);
                //return InvocationProcessor.Invoke(plants, ItemProcessor.DownloadMuntinBarItems, parallel, parms);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.Message);
            }
        }


        public ProcessResponse DownloadGlassItems(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal itemNum, string stockTypes)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED");
            }
            try
            {
                var parms = new Dictionary<string, object>();
                parms.Add("TimeSpan", timeSpan);
                parms.Add("HighWaterMark", useHighWaterMark);
                parms.Add("ItemNum", itemNum);
                parms.Add("StockTypes", stockTypes?.Split(',').ToList());
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, ItemProcessor.DownloadGlassItems, parallel, parms, resources);
                //return InvocationProcessor.Invoke(plants, ItemProcessor.DownloadGlassItems, parallel, parms);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.Message);
            }
        }


        public ProcessResponse DownloadItems(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED");
            }
            try
            {
                var parms = new Dictionary<string, object>();
                parms.Add("TimeSpan", timeSpan);
                parms.Add("HighWaterMark", useHighWaterMark);
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, ItemProcessor.DownloadItems, parallel, parms, resources);
                //return InvocationProcessor.Invoke(plants, ItemProcessor.DownloadItems, parallel, parms);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public ProcessResponse DownloadMuntinCodes(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel)
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
                return BusinessSystemTransferProcessor.Invoke(plants, ItemProcessor.DownloadMuntinCodes, parallel, parms, resources);
                //return InvocationProcessor.Invoke(plants, ItemProcessor.DownloadMuntinCodes, parallel, parms);
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
                return BusinessSystemTransferProcessor.Invoke(plants, ItemProcessor.DownloadMtoSpecUnits, parallel, parms, resources);
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
                return BusinessSystemTransferProcessor.Invoke(plants, ItemProcessor.DownloadMtsSpecUnits, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

    }

    [ParameterInspectorLogger, CGIErrorHandler]
    public class ItemRestService : IItemRestService
    {

        public ItemAttributes GetGlassItemAttributes(string itemNum, string documentNum, string documentType, string documentLineNum, string companyCode)
        {
            var processor = new ItemProcessor();
            return processor.GetGlassItemAttributes(Convert.ToDecimal(itemNum), Convert.ToDecimal(documentNum), documentType, Convert.ToDecimal(documentLineNum), companyCode);
        }

        public GlassItem GetGlassItem(string itemNum, string plantCode, string documentNum, string documentType, string documentLineNum, string companyCode)
        {
            var processor = new ItemProcessor();
            return processor.GetGlassItem(Convert.ToDecimal(itemNum), plantCode, Convert.ToDecimal(documentNum), documentType, Convert.ToDecimal(documentLineNum), companyCode);
        }

        public GlassItems GetDiscreteGlassItems(string plantCode, decimal itemNum, string stockingTypes, string categoryCodes, TimeSpan timeSpan)
        {
            try
            {
                var processor = new ItemProcessor();
                var stockingTypesList = new List<string>();
                var categoryCodesList = new List<string>();
                if (!string.IsNullOrEmpty(stockingTypes))
                {
                    stockingTypesList = stockingTypes.Split(',').ToList();
                }
                if (!string.IsNullOrEmpty(categoryCodes))
                {
                    categoryCodesList = categoryCodes.Split(',').ToList();
                }
                return processor.GetDiscreteGlassItems(plantCode, itemNum, stockingTypesList, categoryCodesList, timeSpan);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.Message);
            }
        }

        public SpacerItems GetSpacerItems(string plantCode, TimeSpan span, bool useHighWaterMark, decimal itemNum)
        {
            if (string.IsNullOrEmpty(plantCode))
            {
                throw new ApplicationException("Plant Code is REQUIRED. ");
            }
            try
            {
                var processor = new ItemProcessor();
                return processor.GetSpacerItems(plantCode, span, useHighWaterMark, itemNum);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.Message);
            }
        }


        public MuntinBarItems GetMuntinBarItems(string plantCode, TimeSpan span, bool useHighWaterMark, decimal itemNum)
        {
            if (string.IsNullOrEmpty(plantCode))
            {
                throw new ApplicationException("Plant Code is REQUIRED. ");
            }
            try
            {
                var processor = new ItemProcessor();
                return processor.GetMuntinBarItems(plantCode, span, useHighWaterMark, itemNum);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.Message);
            }
        }

        /* DOWNLOADS */

        public ProcessResponse DownloadSpacerItems(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal itemNum)
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
                return BusinessSystemTransferProcessor.Invoke(plants, ItemProcessor.DownloadSpacerItems, parallel, parms, resources);
                //return InvocationProcessor.Invoke(plants, ItemProcessor.DownloadSpacerItems, parallel, parms);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse DownloadMuntinBarItems(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal itemNum)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED");
            }
            try
            {
                var parms = new Dictionary<string, object>();
                parms.Add("TimeSpan", timeSpan);
                parms.Add("HighWaterMark", useHighWaterMark);
                parms.Add("ItemNum", itemNum);
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, ItemProcessor.DownloadMuntinBarItems, parallel, parms, resources);
                //return InvocationProcessor.Invoke(plants, ItemProcessor.DownloadMuntinBarItems, parallel, parms);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.Message);
            }
        }


        public ProcessResponse DownloadGlassItems(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal itemNum, string stockTypes)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED");
            }
            try
            {
                var parms = new Dictionary<string, object>();
                parms.Add("TimeSpan", timeSpan);
                parms.Add("HighWaterMark", useHighWaterMark);
                parms.Add("ItemNum", itemNum);
                parms.Add("StockTypes", stockTypes?.Split(',').ToList());
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, ItemProcessor.DownloadGlassItems, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public ProcessResponse DownloadItems(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED");
            }
            try
            {
                var parms = new Dictionary<string, object>();
                parms.Add("TimeSpan", timeSpan);
                parms.Add("HighWaterMark", useHighWaterMark);
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION" };
                return BusinessSystemTransferProcessor.Invoke(plants, ItemProcessor.DownloadItems, parallel, parms, resources);
                //return InvocationProcessor.Invoke(plants, ItemProcessor.DownloadItems, parallel, parms);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public ProcessResponse DownloadMuntinCodes(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel)
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
                return BusinessSystemTransferProcessor.Invoke(plants, ItemProcessor.DownloadMuntinCodes, parallel, parms, resources);
                //return InvocationProcessor.Invoke(plants, ItemProcessor.DownloadMuntinCodes, parallel, parms);
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
                return BusinessSystemTransferProcessor.Invoke(plants, ItemProcessor.DownloadMtoSpecUnits, parallel, parms, resources);
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
                return BusinessSystemTransferProcessor.Invoke(plants, ItemProcessor.DownloadMtsSpecUnits, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

    }
}
