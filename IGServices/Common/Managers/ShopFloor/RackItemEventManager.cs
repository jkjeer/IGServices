using Microsoft.Data.SqlClient;
using IGServices.Common.Types.Response;
using IGServices.Common.Types.ShopFloor;
using IGServices.Common.Utilities;

namespace IGServices.Common.Managers.ShopFloor
{
    public class RackItemEventManager
    {

        public RackItemEvents GetRackItemEvents(Types.Enterprise.RackItemEvents entEvents)
        {
            var sfEvents = new RackItemEvents();
            foreach (var entEvent in entEvents)
            {
                var sfEvent = new RackItemEvent();
                sfEvent.EventId = entEvent.EventId;
                sfEvent.EventCode = entEvent.EventCode;
                sfEvent.EventDate = entEvent.EventDate;
                sfEvent.RackId = entEvent.RackId;
                sfEvent.SerialNum = entEvent.SerialNum;
                sfEvent.Status = entEvent.Status;
                sfEvent.Quantity = entEvent.Quantity;

                if (entEvent.RackItem != null)
                {
                    sfEvent.ItemNo = entEvent.RackItem.ItemNum;
                    sfEvent.LotNo = entEvent.RackItem.LotNum;
                    sfEvent.GLCategory = entEvent.RackItem.GlassItem.GLCategory;
                    sfEvent.MfgDate = entEvent.RackItem.ManufacturedDate;
                    sfEvent.Lite1Thickness = entEvent.RackItem.GlassItem.Lites[0].GlassType.Thickness;
                    sfEvent.Lite1Heat = entEvent.RackItem.GlassItem.Lites[0].GlassType.HeatTreatment;
                    sfEvent.Lite1Color = entEvent.RackItem.GlassItem.Lites[0].GlassType.ColorCode;
                    sfEvent.Lite1Pattern = entEvent.RackItem.GlassItem.Lites[0].GlassType.PatternTexture;
                    sfEvent.Lite1Lami = entEvent.RackItem.GlassItem.Lites[0].GlassType.LamiProductType;
                    sfEvent.Lite1FirstSrfCoat = entEvent.RackItem.GlassItem.Lites[0].GlassType.Surface1Coating;
                    sfEvent.Lite1SecondSrfCoat = entEvent.RackItem.GlassItem.Lites[0].GlassType.Surface2Coating;
                    sfEvent.Lite1Quality = entEvent.RackItem.GlassItem.Lites[0].QualityLevel;
                    sfEvent.ShapeNum = entEvent.RackItem.GlassItem.Shape.ShapeNum;
                    sfEvent.BaseDim = entEvent.RackItem.GlassItem.Shape.BaseDimension.DecimalValue;
                    sfEvent.LeftDim = entEvent.RackItem.GlassItem.Shape.LeftDimension.DecimalValue;
                    sfEvent.RightDim = entEvent.RackItem.GlassItem.Shape.RightDimension.DecimalValue;
                    sfEvent.TopDim = entEvent.RackItem.GlassItem.Shape.TopDimension.DecimalValue;
                    sfEvent.Side1Dim = entEvent.RackItem.GlassItem.Shape.Side1Dimension.DecimalValue;
                    sfEvent.Side2Dim = entEvent.RackItem.GlassItem.Shape.Side2Dimension.DecimalValue;
                    sfEvent.ActualSqft = entEvent.RackItem.GlassItem.Shape.ActualSqft;
                    sfEvent.BillableSqft = entEvent.RackItem.GlassItem.Shape.BillableSqft;
                }
                sfEvents.Add(sfEvent);
            }
            return sfEvents;
        }

        public SqlBulkCopyResponse DownloadRackItemEvents(SqlConnection cn, RackItemEvents events)
        {
            var bcMgr = new SqlBulkCopyManager();
            bcMgr.DbConnectionString = cn.ConnectionString;
            return bcMgr.BulkReader("RackItemEvent", events.GetDataReader());
        }
    }
}
