using System;
using System.Collections.Generic;
using IGServices.Common.Managers.Process;
using IGServices.Common.Types.Response;
using IGServices.Common.Types.ShopFloor;
using IGServices.OrderSvc;
using SoaCore.Wcf;

namespace IGServices.ProductionSvc
{

    [ParameterInspectorLogger, CGIErrorHandler]
    public class ProductionService : IProductionService
    {
        public SpacerConsumption GetSpacerConsumption(string plantCode)
        {
            if (string.IsNullOrEmpty(plantCode))
            {
                throw new ApplicationException("Plant Code is REQUIRED.");
            }
            try
            {
                return SpacerProcessor.GetSpacerConsumption(plantCode);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.Message);
            }
        }

        public ProcessResponse UploadSpacerConsumption(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string status)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                var parms = new Dictionary<string, object>
                {
                    {"TimeSpan", timeSpan},
                    {"HighWaterMark", useHighWaterMark},
                    {"StatusCode", status}
                };
                var resources = new List<string> { "PLANT.INTEGRATION", "CORP.INTEGRATION", "E1.PD" };
                return BusinessSystemTransferProcessor.Invoke(plants, SpacerProcessor.UploadSpacerConsumption, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse UploadMachinePartConsumption(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string status)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                var parms = new Dictionary<string, object>
                {
                    {"TimeSpan", timeSpan},
                    {"HighWaterMark", useHighWaterMark},
                    {"StatusCode", status}
                };
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION", "CORP.INTEGRATION", "E1.PD" };
                return BusinessSystemTransferProcessor.Invoke(plants, MachinePartProcessor.UploadMachinePartConsumption, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse GlassInventoryTransactionUpload(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string status)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                var parms = new Dictionary<string, object>
                {
                    {"TimeSpan", timeSpan},
                    {"HighWaterMark", useHighWaterMark},
                    {"StatusCode", status}
                };
                var resources = new List<string> { "PLANT.SHOPFLOOR", "CORP.INTEGRATION", "E1.PD" };
                return BusinessSystemTransferProcessor.Invoke(plants, ProductionProcessor.GlassInventoryTransactionUpload, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse UploadRemakes(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string status)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                var parms = new Dictionary<string, object>
                {
                    {"TimeSpan", timeSpan},
                    {"HighWaterMark", useHighWaterMark},
                    {"StatusCode", status}
                };
                var resources = new List<string> { "PLANT.SHOPFLOOR", "CORP.INTEGRATION", "E1.PD" };
                return BusinessSystemTransferProcessor.Invoke(plants, ProductionProcessor.UploadRemakes, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse HvReceiptUpload(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string status)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                var parms = new Dictionary<string, object>
                {
                    {"TimeSpan", timeSpan},
                    {"HighWaterMark", useHighWaterMark},
                    {"StatusCode", status}
                };
                var resources = new List<string> { "PLANT.SHOPFLOOR", "CORP.INTEGRATION", "E1.PD" };
                return BusinessSystemTransferProcessor.Invoke(plants, ProductionProcessor.HvReceiptUpload, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse GetQCInspections(string plantCode)
        {
            if (string.IsNullOrEmpty(plantCode))
            {
                throw new ApplicationException("Plant Code is REQUIRED.");
            }
            try
            {
                return QCInspectionsProcessor.GetQCInspections(plantCode);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.Message);
            }
        }

        public ProcessResponse UnitBusinessTransactionUpload(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string status)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                var parms = new Dictionary<string, object>
                {
                    {"TimeSpan", timeSpan},
                    {"HighWaterMark", useHighWaterMark},
                    {"StatusCode", status}
                };
                var resources = new List<string> { "PLANT.SHOPFLOOR", "CORP.INTEGRATION", "E1.PD" };
                return BusinessSystemTransferProcessor.Invoke(plants, ProductionProcessor.UnitBusinessTransactionUpload, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse GlassBusinessTransactionUpload(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string statusCode = null)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                var parms = new Dictionary<string, object>
                {
                    {"TimeSpan", timeSpan},
                    {"HighWaterMark", useHighWaterMark},
                    {"StatusCode", statusCode}
                };
                var resources = new List<string> { "PLANT.SHOPFLOOR", "CORP.INTEGRATION", "E1.PD" };
                return BusinessSystemTransferProcessor.Invoke(plants, ProductionProcessor.GlassBusinessTransactionUpload, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }
    }

    public class ProductionRestService : IProductionRestService
    {
        public SpacerConsumption GetSpacerConsumption(string plantCode)
        {
            if (string.IsNullOrEmpty(plantCode))
            {
                throw new ApplicationException("Plant Code is REQUIRED.");
            }
            try
            {
                return SpacerProcessor.GetSpacerConsumption(plantCode);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.Message);
            }
        }

        public ProcessResponse UploadSpacerConsumption(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string status)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                var parms = new Dictionary<string, object>
                {
                    {"TimeSpan", timeSpan},
                    {"HighWaterMark", useHighWaterMark},
                    {"StatusCode", status}
                };
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION", "CORP.INTEGRATION", "E1.PD" };
                return BusinessSystemTransferProcessor.Invoke(plants, SpacerProcessor.UploadSpacerConsumption, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse UploadMachinePartConsumption(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string status)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                var parms = new Dictionary<string, object>
                {
                    {"TimeSpan", timeSpan},
                    {"HighWaterMark", useHighWaterMark},
                    {"StatusCode", status}
                };
                var resources = new List<string> { "PLANT.SHOPFLOOR", "PLANT.INTEGRATION", "CORP.INTEGRATION", "E1.PD" };
                return BusinessSystemTransferProcessor.Invoke(plants, MachinePartProcessor.UploadMachinePartConsumption, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse GetQCInspections(string plantCode)
        {
            if (string.IsNullOrEmpty(plantCode))
            {
                throw new ApplicationException("Plant Code is REQUIRED.");
            }
            try
            {
                return QCInspectionsProcessor.GetQCInspections(plantCode);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.Message);
            }
        }

        public ProcessResponse GlassInventoryTransactionUpload(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string statusCode = null)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                var parms = new Dictionary<string, object>
                {
                    {"TimeSpan", timeSpan},
                    {"HighWaterMark", useHighWaterMark},
                    {"StatusCode", statusCode}
                };
                var resources = new List<string> { "PLANT.SHOPFLOOR", "CORP.INTEGRATION", "E1.PD" };
                return BusinessSystemTransferProcessor.Invoke(plants, ProductionProcessor.GlassInventoryTransactionUpload, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse HvReceiptUpload(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string status)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                var parms = new Dictionary<string, object>
                {
                    {"TimeSpan", timeSpan},
                    {"HighWaterMark", useHighWaterMark},
                    {"StatusCode", status}
                };
                var resources = new List<string> { "PLANT.SHOPFLOOR", "CORP.INTEGRATION", "E1.PD" };
                return BusinessSystemTransferProcessor.Invoke(plants, ProductionProcessor.HvReceiptUpload, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse UploadRemakes(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string statusCode = null)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                var parms = new Dictionary<string, object>
                {
                    {"TimeSpan", timeSpan},
                    {"HighWaterMark", useHighWaterMark},
                    {"StatusCode", statusCode}
                };
                var resources = new List<string> { "PLANT.SHOPFLOOR", "CORP.INTEGRATION", "E1.PD" };
                return BusinessSystemTransferProcessor.Invoke(plants, ProductionProcessor.UploadRemakes, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse UnitBusinessTransactionUpload(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string status)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                var parms = new Dictionary<string, object>
                {
                    {"TimeSpan", timeSpan},
                    {"HighWaterMark", useHighWaterMark},
                    {"StatusCode", status}
                };
                var resources = new List<string> { "PLANT.SHOPFLOOR", "CORP.INTEGRATION", "E1.PD" };
                return BusinessSystemTransferProcessor.Invoke(plants, ProductionProcessor.UnitBusinessTransactionUpload, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public ProcessResponse GlassBusinessTransactionUpload(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string statusCode = null)
        {
            if (string.IsNullOrEmpty(plants))
            {
                throw new Exception("Plant Code is REQUIRED.");
            }
            try
            {
                var parms = new Dictionary<string, object>
                {
                    {"TimeSpan", timeSpan},
                    {"HighWaterMark", useHighWaterMark},
                    {"StatusCode", statusCode}
                };
                var resources = new List<string> { "PLANT.SHOPFLOOR", "CORP.INTEGRATION", "E1.PD" };
                return BusinessSystemTransferProcessor.Invoke(plants, ProductionProcessor.GlassBusinessTransactionUpload, parallel, parms, resources);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

    }
}
