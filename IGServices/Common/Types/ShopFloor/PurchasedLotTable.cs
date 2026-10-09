using System;
using System.Data;
using System.Data.SqlTypes;
using System.Linq;
using System.Linq.Expressions;
using System.ServiceModel;
using IGServices.Common.Types.Enterprise;
using IGServices.Common.Utilities;
using NLog;

namespace IGServices.Common.Types.ShopFloor
{
    [System.ComponentModel.DesignerCategory("Code")]
    public class PurchasedLotTable : DataTable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public DateTime MaxDate
        {
            get { return (from a in this.AsEnumerable() select a.Field<DateTime>("EventDate")).Max(); }
            set { }
        }

        public decimal MaxId
        {
            get { return (from a in this.AsEnumerable() select a.Field<decimal>("TransactionId")).Max(); }
            set { }
        }

        public PurchasedLotTable()
        {
            TableName = "Lot";
            ExtendedProperties.Add("Target", "[EBS].[PurchasedLotsLedger]");
            Columns.Add("LotOrderNum", typeof(decimal));
            Columns.Add("LotOrderType", typeof(string));
            Columns.Add("LotOrderLineNum", typeof(decimal));
            Columns.Add("ItemNum", typeof(decimal));
            Columns.Add("LotNum", typeof(string));
            Columns.Add("Thickness", typeof(decimal));
            Columns.Add("Color", typeof(string));
            Columns.Add("Treatment", typeof(string));
            Columns.Add("FirstSrfcCoat", typeof(string));
            Columns.Add("SecondSrfcCoat", typeof(string));
            Columns.Add("Lami", typeof(string));
            Columns.Add("Pattern", typeof(string));
            Columns.Add("ShapeNum", typeof(string));
            Columns.Add("BaseDim", typeof(decimal));
            Columns.Add("LeftDim", typeof(decimal));
            Columns.Add("RightDim", typeof(decimal));
            Columns.Add("TopDim", typeof(decimal));
            Columns.Add("Side1Dim", typeof(decimal));
            Columns.Add("Side2Dim", typeof(decimal));
            Columns.Add("LastStatus", typeof(string));
            Columns.Add("NextStatus", typeof(string));
            Columns.Add("OrderDate", typeof(DateTime));
            Columns.Add("CancelDate", typeof(SqlDateTime));
            Columns.Add("OrderQty", typeof(decimal));
            Columns.Add("ReceivedQty", typeof(decimal));
            Columns.Add("OpenQty", typeof(decimal));
            Columns.Add("SupplierPlant", typeof(string));
            Columns.Add("SupplierOrderNum", typeof(decimal));
            Columns.Add("SupplierOrderType", typeof(string));
            Columns.Add("SupplierOrderLineNum", typeof(decimal));
            Columns.Add("ProcessId", typeof(string));
            Columns.Add("TransactionId", typeof(decimal));
            Columns.Add("EventDate", typeof(DateTime));
        }

        public void Add(PurchasedLots lots, Plant plant)
        {
            try
            {
                foreach (PurchasedLot lot in lots)
                {
                    DataRow dr = NewRow();
                    dr["LotOrderNum"] = lot.LotOrderNum;
                    dr["LotOrderType"] = lot.LotOrderType;
                    dr["LotOrderLineNum"] = lot.LotOrderLineNum;
                    dr["ItemNum"] = lot.ItemNum;
                    dr["LotNum"] = lot.LotNum;

                    var atts = lot.ItemAttributes;
                    dr["Thickness"] = atts.GetDecimalAttributeValue("Z58L1TK");
                    dr["Color"] = atts.GetStringAttributeValue("Z58L1CL");
                    dr["Treatment"] = atts.GetStringAttributeValue("Z58L1HT");
                    dr["FirstSrfcCoat"] = atts.GetStringAttributeValue("Z58L11C");
                    dr["SecondSrfcCoat"] = atts.GetStringAttributeValue("Z58L12C");
                    dr["Lami"] = atts.GetStringAttributeValue("Z58L1LPT");
                    dr["Pattern"] = atts.GetStringAttributeValue("Z58L1PX");
                    dr["ShapeNum"] = atts.GetDecimalAttributeValue("Z58SHAPE");
                    dr["BaseDim"] = atts.GetDecimalAttributeValue("Z581BDEC");
                    dr["LeftDim"] = atts.GetDecimalAttributeValue("Z581LDEC");
                    dr["RightDim"] = atts.GetDecimalAttributeValue("Z581RDEC");
                    dr["TopDim"] = atts.GetDecimalAttributeValue("Z581TDEC");
                    dr["Side1Dim"] = atts.GetDecimalAttributeValue("Z5811DEC");
                    dr["Side2Dim"] = atts.GetDecimalAttributeValue("Z5812DEC");

                    //if (lot.GlassItem?.Lites[0]?.GlassType != null)
                    //{
                    //    var gt = lot.GlassItem?.Lites[0]?.GlassType;
                    //    dr["Thickness"] = gt.Thickness;
                    //    dr["Color"] = gt.Color;
                    //    dr["Treatment"] = gt.HeatTreatment;
                    //    dr["FirstSrfcCoat"] = gt.Surface1Coating;
                    //    dr["SecondSrfcCoat"] = gt.Surface2Coating;
                    //    dr["Lami"] = gt.LamiProductType;
                    //    dr["Pattern"] = gt.PatternTexture;
                    //    var sh = lot.GlassItem?.Shape;
                    //    dr["ShapeNum"] = sh.ShapeNum;
                    //    dr["BaseDim"] = sh.BaseDimension.DecimalValue;
                    //    dr["LeftDim"] = sh.LeftDimension.DecimalValue;
                    //    dr["RightDim"] = sh.RightDimension.DecimalValue;
                    //    dr["TopDim"] = sh.TopDimension.DecimalValue;
                    //    dr["Side1Dim"] = sh.Side1Dimension.DecimalValue;
                    //    dr["Side2Dim"] = sh.Side2Dimension.DecimalValue;
                    //}

                    dr["LastStatus"] = lot.LastStatus;
                    dr["NextStatus"] = lot.NextStatus;
                    dr["OrderDate"] = lot.OrderDate;
                    dr["CancelDate"] = (lot.CancelDate == new DateTime(1900, 1, 1))
                        ? SqlDateTime.Null
                        : (SqlDateTime) lot.CancelDate;
                    dr["OrderQty"] = lot.OrderQty;
                    dr["ReceivedQty"] = lot.ReceivedQty;
                    dr["OpenQty"] = lot.OpenQty;
                    dr["SupplierPlant"] = lot.SupplierPlantNum;
                    dr["SupplierOrderNum"] = lot.SupplierOrderNum;
                    dr["SupplierOrderType"] = lot.SupplierOrderType;
                    dr["SupplierOrderLineNum"] = lot.SupplierOrderLineNum;
                    dr["ProcessId"] = lot.ProcessId;
                    dr["TransactionId"] = lot.UniqueId;
                    dr["EventDate"] = lot.EventDate.AddHours(plant.TimeOffset);
                    Rows.Add(dr);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }


[System.ComponentModel.DesignerCategory("Code")]
    public class PurchasedLotWorkOrderTable : DataTable
    {

        public DateTime MaxDate
        {
            get { return (from a in this.AsEnumerable() select a.Field<DateTime>("EventDate")).Max(); }
            set { }
        }

        public PurchasedLotWorkOrderTable()
        {
            TableName = "LotWorkOrder";
            ExtendedProperties.Add("Target", "[EBS].[PurchasedLotWorkOrdersLedger]");
            Columns.Add("LotOrderNum", typeof(decimal));
            Columns.Add("LotOrderType", typeof(string));
            Columns.Add("LotOrderLineNum", typeof(decimal));
            Columns.Add("RelatedLiteNum", typeof(decimal));
            Columns.Add("RelatedTranType", typeof(string));
            Columns.Add("WorkOrderDate", typeof(DateTime));
            Columns.Add("WorkOrderNum", typeof(decimal));
            Columns.Add("WorkOrderStatus", typeof(string));
            Columns.Add("WorkOrderQty", typeof(decimal));
            Columns.Add("ItemNum", typeof(decimal));
            Columns.Add("UnitType", typeof(string));
            Columns.Add("EventDate", typeof(DateTime));
        }

        public void Add(PurchasedLots lots, Plant plant)
        {
            foreach (PurchasedLot lot in lots)
            {
                foreach (RelatedOrderLine relOrderLine in lot.RelatedOrderLines)
                {
                    DataRow dr = NewRow();
                    dr["LotOrderNum"] = lot.LotOrderNum;
                    dr["LotOrderType"] = lot.LotOrderType;
                    dr["LotOrderLineNum"] = lot.LotOrderLineNum;
                    dr["RelatedLiteNum"] = relOrderLine.LiteNum;
                    dr["RelatedTranType"] = relOrderLine.TransactionType;
                    dr["WorkOrderDate"] = relOrderLine.WorkOrder.OrderDate;
                    dr["WorkOrderNum"] = relOrderLine.WorkOrder.WorkOrderNum;
                    dr["WorkOrderStatus"] = relOrderLine.WorkOrder.StatusCode;
                    dr["WorkOrderQty"] = relOrderLine.WorkOrder.Quantity;
                    dr["ItemNum"] = relOrderLine.WorkOrder.ItemNum;
                    dr["UnitType"] = relOrderLine.WorkOrder.UnitType;
                    dr["EventDate"] = lot.EventDate.AddHours(plant.TimeOffset);
                    Rows.Add(dr);
                }
            }
        }
    }

    [System.ComponentModel.DesignerCategory("Code")]
    public class PurchasedLotSalesOrderTable : DataTable
    {

        public DateTime MaxDate
        {
            get { return (from a in this.AsEnumerable() select a.Field<DateTime>("EventDate")).Max(); }
            set { }
        }

        public PurchasedLotSalesOrderTable()
        {
            TableName = "LotSalesOrder";
            ExtendedProperties.Add("Target", "[EBS].[PurchasedLotSalesOrdersLedger]");
            Columns.Add("WorkOrderNum", typeof(decimal));
            Columns.Add("SalesOrderNum", typeof(decimal));
            Columns.Add("SalesOrderType", typeof(string));
            Columns.Add("SalesOrderLineNum", typeof(decimal));
            Columns.Add("OrderQty", typeof(decimal));
            Columns.Add("OrderDate", typeof(DateTime));
            Columns.Add("RequestedDate", typeof(DateTime));
            Columns.Add("SoldToCustomerNum", typeof(decimal));
            Columns.Add("SoldToCustomerName", typeof(string));
            Columns.Add("ShipToCustomerNum", typeof(decimal));
            Columns.Add("ShipToCustomerName", typeof(string));
            Columns.Add("EventDate", typeof(DateTime));
        }

        public void Add(PurchasedLots lots, Plant plant)
        {
            foreach (PurchasedLot lot in lots)
            {
                foreach (RelatedOrderLine relOrderLine in lot.RelatedOrderLines)
                {
                    if (relOrderLine.WorkOrder.RelatedSalesOrderLines != null)
                    {
                        foreach (RelatedSalesOrderLine line in relOrderLine.WorkOrder.RelatedSalesOrderLines)
                        {
                            DataRow dr = NewRow();
                            dr["WorkOrderNum"] = relOrderLine.WorkOrder.WorkOrderNum;
                            dr["SalesOrderNum"] = line.OrderNum;
                            dr["SalesOrderType"] = line.OrderType;
                            dr["SalesOrderLineNum"] = line.OrderLineNum;
                            dr["OrderQty"] = line.OrderQty;
                            dr["OrderDate"] = line.OrderDate;
                            dr["RequestedDate"] = line.RequestedDate;
                            dr["SoldToCustomerNum"] = line.SoldToCustomer.CustomerNum;
                            dr["SoldToCustomerName"] = line.SoldToCustomer.CustomerName;
                            dr["ShipToCustomerNum"] = line.ShipToCustomer.CustomerNum;
                            dr["ShipToCustomerName"] = line.ShipToCustomer.CustomerName;
                            dr["EventDate"] = lot.EventDate.AddHours(plant.TimeOffset);
                            Rows.Add(dr);
                        }
                    }
                }
            }
        }
    }

    [System.ComponentModel.DesignerCategory("Code")]
    public class PurchasedLotLoadTable : DataTable
    {

        public DateTime MaxDate
        {
            get { return (from a in this.AsEnumerable() select a.Field<DateTime>("EventDate")).Max(); }
            set { }
        }

        public PurchasedLotLoadTable()
        {
            TableName = "LotLoad";
            ExtendedProperties.Add("Target", "[EBS].[PurchasedLotLoadsLedger]");
            Columns.Add("LotOrderNum", typeof(decimal));
            Columns.Add("LotOrderType", typeof(string));
            Columns.Add("LotOrderLineNum", typeof(decimal));
            Columns.Add("SupplierPlant", typeof(string));
            Columns.Add("LoadNum", typeof(decimal));
            Columns.Add("LoadDate", typeof(SqlDateTime));
            Columns.Add("EventDate", typeof(DateTime));
        }

        public void Add(PurchasedLots lots, Plant plant)
        {
            foreach (PurchasedLot lot in lots)
            {
                DataRow dr = NewRow();
                dr["LotOrderNum"] = lot.LotOrderNum;
                dr["LotOrderType"] = lot.LotOrderType;
                dr["LotOrderLineNum"] = lot.LotOrderLineNum;
                dr["SupplierPlant"] = lot.SupplierPlantNum;
                dr["LoadNum"] = lot.LoadNum;
                dr["LoadDate"] = (lot.LoadDate == new DateTime(1900, 1, 1)) ? SqlDateTime.Null : (SqlDateTime)lot.LoadDate.AddHours(plant.TimeOffset);
                dr["EventDate"] = lot.EventDate.AddHours(plant.TimeOffset); ;
                Rows.Add(dr);
            }
        }
    }

    [System.ComponentModel.DesignerCategory("Code")]
    public class PurchasedLotReceiptTable : DataTable
    {

        public DateTime MaxDate
        {
            get { return (from a in this.AsEnumerable() select a.Field<DateTime>("EventDate")).Max(); }
            set { }
        }

        public PurchasedLotReceiptTable()
        {
            TableName = "LotReceipt";
            ExtendedProperties.Add("Target", "[EBS].[PurchasedLotReceiptsLedger]");
            Columns.Add("LotOrderNum", typeof(decimal));
            Columns.Add("LotOrderType", typeof(string));
            Columns.Add("LotOrderLineNum", typeof(decimal));
            Columns.Add("LotNum", typeof(string));
            Columns.Add("LoadNum", typeof(decimal));
            Columns.Add("RackId", typeof(decimal));
            Columns.Add("SerialNum", typeof(string));
            Columns.Add("RackQty", typeof(decimal));
            Columns.Add("EventDate", typeof(DateTime));
        }

        public void Add(PurchasedLots lots, Plant plant)
        {
            foreach (PurchasedLot lot in lots)
            {
                DataRow dr = NewRow();
                dr["LotOrderNum"] = lot.LotOrderNum;
                dr["LotOrderType"] = lot.LotOrderType;
                dr["LotOrderLineNum"] = lot.LotOrderLineNum;
                dr["LotNum"] = lot.LotNum;
                dr["LoadNum"] = lot.LoadNum;
                dr["RackId"] = lot.RackId;
                dr["SerialNum"] = lot.SerialNum;
                dr["RackQty"] = lot.RackQty;
                dr["EventDate"] = lot.EventDate.AddHours(plant.TimeOffset);
                Rows.Add(dr);
            }
        }
    }
}
