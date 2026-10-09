using System;
using IGServices.Common.Types.Enterprise;
using IGServices.Common.Types.Response;
using SoaCore.Wcf;

namespace IGServices.PurchasedLotSvc
{

    [ParameterInspectorLogger, CGIErrorHandler]
    public class PurchasedLotService : IPurchasedLotService
    {
        public PurchasedLots GetPurchasedLots(string plantCode, int minutes)
        {
            var processor = new PurchasedLotProcessor();
            try
            {
                return processor.GetPurchasedLots(plantCode, minutes);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.Message);
            }
        }

        public SqlBulkCopyResponses PersistAllEvents(string plantCode, int minutes)
        {
            var processor = new PurchasedLotProcessor();
            try
            {
                return processor.PersistAllEvents(plantCode, minutes);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.Message);
            }
        }

        public SqlBulkCopyResponse PersistPurchasedLotLoads(string plantCode, int minutes)
        {
            var processor = new PurchasedLotProcessor();
            try
            {
                return processor.PersistPurchasedLotLoads(plantCode, minutes);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.Message);
            }
        }

        public SqlBulkCopyResponse PersistPurchasedLotReceipts(string plantCode, int minutes)
        {
            var processor = new PurchasedLotProcessor();
            try
            {
                return processor.PersistPurchasedLotReceipts(plantCode, minutes);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.Message);
            }
        }
    }

    [ParameterInspectorLogger, CGIErrorHandler]
    public class PurchasedLotRestService : IPurchasedLotRestService
    {
        public PurchasedLots GetPurchasedLots(string plantCode, string minutes)
        {
            var processor = new PurchasedLotProcessor();
            try
            {
                return processor.GetPurchasedLots(plantCode, Convert.ToInt32(minutes));
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.Message);
            }
        }

        public SqlBulkCopyResponses PersistPurchasedLots(string plantCode, string minutes)
        {
            var processor = new PurchasedLotProcessor();
            try
            {
                return processor.PersistPurchasedLots(plantCode, Convert.ToInt32(minutes));
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.Message);
            }
        }

        public PurchasedLots GetPurchasedLotLoads(string plantCode, string minutes)
        {
            var processor = new PurchasedLotProcessor();
            try
            {
                return processor.GetPurchasedLotLoads(plantCode, Convert.ToInt32(minutes));
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.Message);
            }
        }

        public SqlBulkCopyResponse PersistPurchasedLotLoads(string plantCode, string minutes)
        {
            var processor = new PurchasedLotProcessor();
            try
            {
                return processor.PersistPurchasedLotLoads(plantCode, Convert.ToInt32(minutes));
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.Message);
            }
        }

        public PurchasedLots GetPurchasedLotReceipts(string plantCode, string minutes)
        {
            var processor = new PurchasedLotProcessor();
            try
            {
                return processor.GetPurchasedLotReceipts(plantCode, Convert.ToInt32(minutes));
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.Message);
            }
        }

        public SqlBulkCopyResponse PersistPurchasedLotReceipts(string plantCode, string minutes)
        {
            var processor = new PurchasedLotProcessor();
            try
            {
                return processor.PersistPurchasedLotReceipts(plantCode, Convert.ToInt32(minutes));
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.Message);
            }
        }

        public SqlBulkCopyResponses PersistAllEvents(string plantCode, string minutes)
        {
            var processor = new PurchasedLotProcessor();
            try
            {
                return processor.PersistAllEvents(plantCode, Convert.ToInt32(minutes));
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.Message);
            }
        }

    }
}
