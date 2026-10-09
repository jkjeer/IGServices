using System;
using System.Linq;
using IGServices.Common.Managers;
using IGServices.Common.Managers.E1;
using IGServices.Common.Managers.Enterprise;
using IGServices.Common.Types;
using IGServices.Common.Types.E1;
using IGServices.Common.Types.Enterprise;
using IGServices.Common.Types.Response;
using IGServices.Common.Utilities;

namespace IGServices.PurchasedLotSvc
{
    public class PurchasedLotProcessor
    {
        public PurchasedLots GetPurchasedLots(string plantCode, int minutes)
        {
            var dateParms = new E1DateParms();
            var e1Session = GetE1Session();
            try
            {
                var lotMgr = new PurchasedLotManager(e1Session);
                var hw = lotMgr.GetHighWaterMark(plantCode, "PurchasedLots");
                if (minutes > 0)
                {
                    hw.EnterpriseDateTime = DateTime.Now.AddMinutes(minutes * -1);
                }
                return lotMgr.GetPurchasedLots(e1Session, plantCode, hw);
            }
            catch (Exception err)
            {
                throw new Exception(err.ToString());
            }
            finally
            {
                Closee1Session(e1Session);
            }
        }

        public SqlBulkCopyResponses PersistPurchasedLots(string plantCode, int minutes)
        {
            if (String.IsNullOrEmpty(plantCode))
            {
                throw new ApplicationException("Plant Code is REQUIRED");
            }
            var e1Session = GetE1Session();
            var lots = new PurchasedLots();
            try
            {
                var lotManager = new PurchasedLotManager(e1Session);
                lots = GetPurchasedLots(plantCode, minutes);
                return lotManager.PersistPurchasedLots(e1Session, plantCode, lots);
            }
            catch (Exception err)
            {
                throw new ApplicationException(err.ToString());
            }
            finally
            {
                Closee1Session(e1Session);
            }
        }

        public PurchasedLots GetPurchasedLotLoads(string plantCode, int minutes)
        {
            var dateParms = new E1DateParms();
            var timeSpan = new TimeSpan(0, minutes, 0);
            dateParms.SetParms(timeSpan);
            var e1Session = GetE1Session();
            try
            {
                var lotMgr = new PurchasedLotManager(e1Session);
                return lotMgr.GetPurchasedLotLoads(e1Session, plantCode, dateParms);
            }
            catch (Exception err)
            {
                throw new ApplicationException(err.ToString());
            }
            finally
            {
                Closee1Session(e1Session);
            }
        }

        public SqlBulkCopyResponse PersistPurchasedLotLoads(string plantCode, int minutes)
        {
            if (String.IsNullOrEmpty(plantCode))
            {
                throw new ApplicationException("Plant Code is REQUIRED");
            }
            var e1Session = GetE1Session();
            var lots = new PurchasedLots();
            try
            {
                var lotManager = new PurchasedLotManager(e1Session);
                lots = GetPurchasedLotLoads(plantCode, minutes);
                return lotManager.PersistPurchasedLotLoads(e1Session, plantCode, lots);
            }
            catch (Exception err)
            {
                throw new ApplicationException(err.ToString());
            }
            finally
            {
                Closee1Session(e1Session);
            }
        }

        public PurchasedLots GetPurchasedLotReceipts(string plantCode, int minutes)
        {
            var dateParms = new E1DateParms();
            var timeSpan = new TimeSpan(0, minutes, 0);
            dateParms.SetParms(timeSpan);
            var e1Session = GetE1Session();
            try
            {
                var lotMgr = new PurchasedLotManager(e1Session);
                return lotMgr.GetPurchasedLotReceipts(e1Session, plantCode, dateParms);
            }
            catch (Exception err)
            {
                throw new ApplicationException(err.ToString());
            }
            finally
            {
                Closee1Session(e1Session);
            }
        }

        public SqlBulkCopyResponse PersistPurchasedLotReceipts(string plantCode, int minutes)
        {
            if (String.IsNullOrEmpty(plantCode))
            {
                throw new ApplicationException("Plant Code is REQUIRED");
            }
            var e1Session = GetE1Session();
            var lots = new PurchasedLots();
            try
            {
                var lotManager = new PurchasedLotManager(e1Session);
                lots = GetPurchasedLotReceipts(plantCode, minutes);
                return lotManager.PersistPurchasedLotReceipts(e1Session, plantCode, lots);
            }
            catch (Exception err)
            {
                throw new ApplicationException(err.ToString());
            }
            finally
            {
                Closee1Session(e1Session);
            }
        }

        public SqlBulkCopyResponses PersistAllEvents(string plantCode, int minutes)
        {
            var dateParms = new E1DateParms();
            var responses = new SqlBulkCopyResponses();

            if (String.IsNullOrEmpty(plantCode))
            {
                throw new ApplicationException("Plant Code is REQUIRED");
            }
            var e1Session = GetE1Session();

            try
            {
                var nowTime = e1Session.GetCurrentTimestamp();
                var lotManager = new PurchasedLotManager(e1Session);

                var hw = lotManager.GetHighWaterMark(plantCode, "PurchasedLots");
                if (minutes > 0)
                {
                    hw.EnterpriseDateTime = DateTime.Now.AddMinutes(minutes * -1);
                }
                dateParms.SetParms(hw.EnterpriseDateTime);

                var lots1 = lotManager.GetPurchasedLots(e1Session, plantCode, hw);
                var returnResponse = lotManager.PersistPurchasedLots(e1Session, plantCode, lots1);
                responses.AddRange(returnResponse);

                var lots2 = lotManager.GetPurchasedLotLoads(e1Session, plantCode, dateParms);
                var response2 = lotManager.PersistPurchasedLotLoads(e1Session, plantCode, lots2);
                responses.Add(response2);

                var lots3 = lotManager.GetPurchasedLotReceipts(e1Session, plantCode, dateParms);
                var response3 = lotManager.PersistPurchasedLotReceipts(e1Session, plantCode, lots3);
                responses.Add(response3);

                var uniqueId = hw.UniqueID;
                if (lots1.Max(x => x.UniqueId) > hw.UniqueID)
                {
                    uniqueId = (long)lots1.Max(x => x.UniqueId);
                }
                var markResponse = lotManager.SetHighWaterMark(e1Session, plantCode, "PurchasedLots", nowTime, uniqueId);
            }
            catch (Exception err)
            {
                throw new Exception(err.ToString());
            } 
            finally
            {
                Closee1Session(e1Session);
            }
            return responses;
        }

        private static BusinessSystemSession GetE1Session()
        {
            var e1Session = BusinessSystemSessionManager.GetBusinessSession();
            return e1Session;
        }

        private static void Closee1Session(BusinessSystemSession e1Session)
        {
            var e1SessionMgr = new BusinessSystemSessionManager();
            e1SessionMgr.CloseBusinessSession(e1Session);
        }
    }
}
