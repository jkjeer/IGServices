using System;
using IGServices.Common.Managers.E1;
using IGServices.Common.Managers.Enterprise;
using IGServices.Common.Managers.ShopFloor;
using IGServices.Common.Types.E1;
using IGServices.Common.Types.Enterprise;
using IGServices.Common.Types.Response;
using NLog;
using SpacerItemManager = IGServices.Common.Managers.Enterprise.SpacerItemManager;

namespace IGServices.MezzSvc
{
    public class MezzProcessor
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public WorkOrders GetCancelledOrdersBySchedule(string plantCode, decimal scheduleNum)
        {
            var plantNum = PlantManager.GetPlant(plantCode).PlantNum;
            var e1Session = GetE1Session();
            var orders = new WorkOrders();
            try
            {
                var manager = new WorkOrderManager(e1Session);
                var orderNums = manager.GetCancelledOrdersBySchedule(e1Session, plantNum, scheduleNum);
                foreach (var orderNum in orderNums)
                {
                    var order = manager.GetWorkOrder(e1Session, orderNum);
                    var unitIds = manager.GetUnitIds(e1Session, orderNum);
                    order.UnitIds = unitIds;
                    orders.Add(order);
                }
            }
            catch (Exception err)
            {
                throw new ApplicationException(err.ToString());
            }
            finally
            {
                CloseE1Session(e1Session);
            }
            return orders;
        }

        public WorkOrder GetOrder(decimal orderNum)
        {
            var e1Session = GetE1Session();
            WorkOrder order;
            try
            {
                var orderManager = new WorkOrderManager(e1Session);
                Logger.Trace(orderNum);
                order = orderManager.GetWorkOrder(e1Session, orderNum, false, true);
                order.UnitIds = orderManager.GetUnitIds(e1Session, orderNum);
            }
            catch (Exception err)
            {
                throw new ApplicationException(err.ToString());
            }
            finally
            {
                CloseE1Session(e1Session);
            }
            return order;
        }

        public SpacerItems GetSpacerItems(string plantCode)
        {
            var e1Session = GetE1Session();
            SpacerItems items;
            try
            {
                var manager = new SpacerItemManager(e1Session);
                var plant = PlantManager.GetPlant(plantCode);
                items = manager.GetSpacerItems(e1Session, plant, new ProcessRunSpan(), 0);
            }
            catch (Exception err)
            {
                throw new ApplicationException(err.ToString());
            }
            finally
            {
                CloseE1Session(e1Session);
            }
            return items;
        }

        public GlassItems GetDiscreteItems(string plantCode)
        {
            var e1Session = GetE1Session();
            var items = new GlassItems();
            try
            {
                //var manager = new GlassItemManager(e1Session);
                //items = manager.GetFinishedGlassItems(e1Session, plantCode);
            }
            catch (Exception err)
            {
                throw new ApplicationException(err.ToString());
            }
            finally
            {
                CloseE1Session(e1Session);
            }
            return items;
        }

        public GlassItem GetDiscreteItem(string plantCode, decimal itemNum)
        {
            var e1Session = GetE1Session();
            GlassItem item;
            try
            {
                var manager = new GlassItemManager(e1Session);
                var plantNum = PlantManager.GetPlant(plantCode).PlantNum;
                item = manager.GetGlassItem(e1Session, itemNum, plantNum);
            }
            catch (Exception err)
            {
                throw new ApplicationException(err.ToString());
            }
            finally
            {
                CloseE1Session(e1Session);
            }
            return item;
        }

        public SqlBulkCopyResponse PersistSpacerItems(string plantCode)
        {
            var e1Session = GetE1Session();
            SqlBulkCopyResponse response;
            try
            {
                var items = GetSpacerItems(plantCode);
                var manager = new SpacerItemPersistenceManager(e1Session);
                response = manager.PersistSpacerItems(e1Session, plantCode, items);
            }
            catch (Exception err)
            {
                throw new ApplicationException(err.ToString());
            }
            finally
            {
                CloseE1Session(e1Session);
            }
            return response;
        }

        private static BusinessSystemSession GetE1Session()
        {
            var connectionString = new ConnectionString();
            var e1Session = BusinessSystemSessionManager.GetBusinessSession();
            return e1Session;
        }

        private static void CloseE1Session(BusinessSystemSession e1Session)
        {
            var e1SessionMgr = new BusinessSystemSessionManager();
            e1SessionMgr.CloseBusinessSession(e1Session);
        }
    }
}
