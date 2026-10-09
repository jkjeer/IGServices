using Microsoft.Data.SqlClient;
using IGServices.Common.Types.Response;
using IGServices.Common.Types.ShopFloor;
using IGServices.Common.Utilities;

namespace IGServices.Common.Managers.ShopFloor
{
    public class RackManager
    {
        public object[] GetRacks(Types.Enterprise.Racks entRacks)
        {
            var sfArray = new object[2];
            var sfRacks = new Racks();
            var sfRackItems = new RackItems();

            foreach (var entRack in entRacks)
            {
                var sfRack = new Rack();
                sfRack.RackId = entRack.RackId;
                sfRack.Status = entRack.HeaderStatus;
                sfRack.StatusDate = entRack.HeaderStatusDate;
                sfRack.Location = entRack.Location;
                sfRack.ReasonCode = entRack.ReasonCode;
                sfRack.RackType = entRack.RackType;
                sfRack.ScheduleNum = entRack.ScheduleNum;
                sfRack.BoxNum = entRack.BoxNum;
                sfRack.LoadNum = entRack.LoadNum;
                sfRack.CustomerNum = (entRack.Customer == null) ? 0 : entRack.Customer.CustomerNum;
                sfRack.CustomerShipNum = (entRack.ShipToCustomer == null) ? 0 : entRack.ShipToCustomer.CustomerNum;
                sfRack.SupplierNum = entRack.SupplierNum;
                sfRack.ReceivedDate = entRack.ReceivedDate;
                sfRack.GlassLbs = entRack.GlassWeight;
                sfRack.RackLbs = entRack.TareWeight;

                foreach (var entRackItem in entRack.RackItems)
                {
                    var sfRackItem = new RackItem();
                    sfRackItem.RackId = entRackItem.RackId;
                    sfRackItem.SerialNum = entRackItem.SerialNum;
                    sfRackItem.Status = entRackItem.DetailStatus;
                    sfRackItem.StatusDate = entRackItem.UpdateDateTime;
                    sfRackItem.GLCategory = entRackItem.GLCategory;
                    sfRackItem.Quantity = entRackItem.Quantity;
                    sfRackItem.ItemNum = entRackItem.ItemNum;
                    sfRackItem.LotNum = entRackItem.LotNum;
                    sfRackItem.GlassLbs = entRackItem.GlassWeight;
                    sfRackItem.WorkOrderNum = entRackItem.WorkOrderNum;
                    if (entRackItem.WorkOrderPlantNum == entRackItem.PlantNum)
                    {
                        sfRackItem.OrderNum = entRackItem.SalesOrderNum;
                        sfRackItem.OrderType = entRackItem.SalesOrderType;
                        sfRackItem.OrderLineNum = entRackItem.SalesOrderLineNum;
                    }
                    else
                    {
                        sfRackItem.OrderNum = entRackItem.PurchaseOrderNum;
                        sfRackItem.OrderType = entRackItem.PurchaseOrderType;
                        sfRackItem.OrderLineNum = entRackItem.PurchaseOrderLineNum;                       
                    }
                    if (entRackItem.HasGlassAttributes)
                    {
                        sfRackItem.Lite1Thickness = entRackItem.GlassItem.Lites[0].GlassType.Thickness;
                        sfRackItem.Lite1Heat = entRackItem.GlassItem.Lites[0].GlassType.HeatTreatment;
                        sfRackItem.Lite1Color = entRackItem.GlassItem.Lites[0].GlassType.ColorCode;
                        sfRackItem.Lite1Pattern = entRackItem.GlassItem.Lites[0].GlassType.PatternTexture;
                        sfRackItem.Lite1Lami = entRackItem.GlassItem.Lites[0].GlassType.LamiProductType;
                        sfRackItem.Lite1FirstSrfCoat = entRackItem.GlassItem.Lites[0].GlassType.Surface1Coating;
                        sfRackItem.Lite1SecondSrfCoat = entRackItem.GlassItem.Lites[0].GlassType.Surface2Coating;
                        sfRackItem.Lite1Quality = entRackItem.GlassItem.Lites[0].QualityLevel;
                        sfRackItem.ShapeNum = entRackItem.GlassItem.Shape.ShapeNum;
                        sfRackItem.BaseDim = entRackItem.GlassItem.Shape.BaseDimension.DecimalValue;
                        sfRackItem.LeftDim = entRackItem.GlassItem.Shape.LeftDimension.DecimalValue;
                        sfRackItem.RightDim = entRackItem.GlassItem.Shape.RightDimension.DecimalValue;
                        sfRackItem.TopDim = entRackItem.GlassItem.Shape.TopDimension.DecimalValue;
                        sfRackItem.Side1Dim = entRackItem.GlassItem.Shape.Side1Dimension.DecimalValue;
                        sfRackItem.Side2Dim = entRackItem.GlassItem.Shape.Side2Dimension.DecimalValue;
                        sfRackItem.ActualSqft = entRackItem.GlassItem.Shape.ActualSqft;
                        sfRackItem.BillableSqft = entRackItem.GlassItem.Shape.BillableSqft;
                    }
                    sfRackItems.Add(sfRackItem);
                }
                sfRacks.Add(sfRack);
            }

            sfArray[0] = sfRacks;
            sfArray[1] = sfRackItems;
            return sfArray;
        }

        public SqlBulkCopyResponses DownloadRacks(SqlConnection cn, object[] rackArray)
        {
            var responses = new SqlBulkCopyResponses();
            var bcMgr = new SqlBulkCopyManager();
            bcMgr.DbConnectionString = cn.ConnectionString;
            var returnResponse = bcMgr.BulkReader("Rack", ((Racks)rackArray[0]).GetDataReader());
            var response2 = bcMgr.BulkReader("RackItem", ((RackItems)rackArray[1]).GetDataReader());
            responses.Add(returnResponse);
            responses.Add(response2);
            return responses;
        }

    }
}
