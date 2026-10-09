using System;
using IGServices.Common.Types.Enterprise;
using IGServices.Common.Types.Response;
using NLog;
using SoaCore.Wcf;

namespace IGServices.MezzSvc
{

    [ParameterInspectorLogger, CGIErrorHandler]
    public class MezzService : IMezzService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public WorkOrder GetWorkOrder(decimal OrderNum)
        {
            Logger.Trace(OrderNum);
            var processor = new MezzProcessor();
            return processor.GetOrder(OrderNum);
        }

        public WorkOrders GetCancelledOrdersBySchedule(string plantCode, Decimal scheduleNum)
        {
            var processor = new MezzProcessor();
            return processor.GetCancelledOrdersBySchedule(plantCode, scheduleNum);
        }

        public SpacerItems GetSpacerItems(string plantCode)
        {
            var processor = new MezzProcessor();
            return processor.GetSpacerItems(plantCode);
        }

        public GlassItem GetDiscreteItem(string plantCode, decimal itemNum)
        {
            var processor = new MezzProcessor();
            return processor.GetDiscreteItem(plantCode, itemNum);
        }

        public GlassItems GetDiscreteItems(string plantCode)
        {
            var processor = new MezzProcessor();
            return processor.GetDiscreteItems(plantCode);
        }

        public SqlBulkCopyResponse PersistSpacerItems(string plantCode)
        {
            var processor = new MezzProcessor();
            return processor.PersistSpacerItems(plantCode);
        }

    }

    [ParameterInspectorLogger, CGIErrorHandler]
    public class MezzRestService : IMezzRestService
    {
        public WorkOrder GetWorkOrder(string OrderNum)
        {
            var processor = new MezzProcessor();
            return processor.GetOrder(Convert.ToDecimal(OrderNum));
        }

        public WorkOrders GetCancelledOrdersBySchedule(string plantCode, string scheduleNum)
        {
            var processor = new MezzProcessor();
            return processor.GetCancelledOrdersBySchedule(plantCode, Convert.ToDecimal(scheduleNum));
        }

        public SpacerItems GetSpacerItems(string plantCode)
        {
            var processor = new MezzProcessor();
            return processor.GetSpacerItems(plantCode);
        }

        public GlassItem GetDiscreteItem(string plantCode, string itemNum)
        {
            var processor = new MezzProcessor();
            return processor.GetDiscreteItem(plantCode, Convert.ToDecimal(itemNum));
        }

        public GlassItems GetDiscreteItems(string plantCode)
        {
            var processor = new MezzProcessor();
            return processor.GetDiscreteItems(plantCode);
        }

        public SqlBulkCopyResponse PersistSpacerItems(string plantCode)
        {
            var processor = new MezzProcessor();
            return processor.PersistSpacerItems(plantCode);
        }

    }

}
