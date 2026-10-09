using System;
using System.Data;
using System.Linq;
using System.ServiceModel;
using IGServices.Common.Utilities;

namespace IGServices.Common.Types.ShopFloor
{
    [System.ComponentModel.DesignerCategory("Code")]
    public class RackLedgerTable : DataTable
    {

        public RackLedgerTable()
        {
            ExtendedProperties.Add("Target", "[dbo].[E1RackLedger]");
            Columns.Add("RackId", typeof(decimal));
            Columns.Add("SerialNum", typeof(string));
            Columns.Add("ItemNum", typeof(decimal));
            Columns.Add("LotNum", typeof(string));
            Columns.Add("Location", typeof(string));
            Columns.Add("Status", typeof(string));
            Columns.Add("QtyOnHand", typeof(decimal));
            Columns.Add("ReceivedQty", typeof(decimal));
            Columns.Add("RackDateTime", typeof(DateTime));
            Columns.Add("ReceivedDate", typeof(DateTime));
            Columns.Add("ExpirationDate", typeof(DateTime));
            Columns.Add("ProductionDate", typeof(DateTime));
            Columns.Add("CustomerNum", typeof(decimal));
            Columns.Add("CustomerName", typeof(string));
            Columns.Add("Lite1_Thickness", typeof(decimal));
            Columns.Add("Lite1_Color", typeof(string));
            Columns.Add("Lite1_HeatTreatment", typeof(string));
            Columns.Add("Lite1_1stSrfCoating", typeof(string));
            Columns.Add("Lite1_2ndSrfCoating", typeof(string));
            Columns.Add("Lite1_PatternTexture", typeof(string));
            Columns.Add("Lite1_LamiProdType", typeof(string));
            Columns.Add("Lite1_ProdQualityLevel", typeof(string));
            Columns.Add("AnsiiLogoType", typeof(string));
            Columns.Add("ShapeNum", typeof(decimal));
            Columns.Add("BaseL", typeof(decimal));
            Columns.Add("BaseN", typeof(decimal));
            Columns.Add("BaseD", typeof(decimal));
            Columns.Add("LeftL", typeof(decimal));
            Columns.Add("LeftN", typeof(decimal));
            Columns.Add("LeftD", typeof(decimal));
            Columns.Add("RightL", typeof(decimal));
            Columns.Add("RightN", typeof(decimal));
            Columns.Add("RightD", typeof(decimal));
            Columns.Add("TopL", typeof(decimal));
            Columns.Add("TopN", typeof(decimal));
            Columns.Add("TopD", typeof(decimal));
            Columns.Add("Side1L", typeof(decimal));
            Columns.Add("Side1N", typeof(decimal));
            Columns.Add("Side1D", typeof(decimal));
            Columns.Add("Side2L", typeof(decimal));
            Columns.Add("Side2N", typeof(decimal));
            Columns.Add("Side2D", typeof(decimal));
            Columns.Add("SqftActual", typeof(decimal));
            Columns.Add("SqftBillable", typeof(decimal));
            Columns.Add("RackType", typeof(string));
            Columns.Add("LitesPerRack", typeof(decimal));
            Columns.Add("ProcessID", typeof(string));
            Columns.Add("GLCategory", typeof(string));
            Columns.Add("ReasonCode", typeof(string));
            Columns.Add("SequenceNum", typeof(decimal));
            Columns.Add("WrittenDate", typeof(DateTime));
        }

        public void Add(Enterprise.Racks racks)
        {
            try
            {
                foreach (Enterprise.Rack rack in racks)
                {
                    if (rack.RackItems != null)
                    {
                        foreach (var rackItem in rack.RackItems)
                        {
                            DataRow dr = this.NewRow();
                            dr["RackId"] = rackItem.RackId;
                            dr["SerialNum"] = rackItem.SerialNum;
                            dr["ItemNum"] = rackItem.ItemNum;
                            dr["LotNum"] = rackItem.LotNum;
                            dr["Location"] = rack.Location;
                            dr["Status"] = rack.HeaderStatus;
                            dr["QtyOnHand"] = rackItem.Quantity;
                            dr["ReceivedQty"] = rackItem.ReceivedQty;
                            dr["RackDateTime"] = rack.HeaderStatusDate;
                            dr["ReceivedDate"] = rackItem.ReceivedDate;
                            //dr["ExpirationDate"] = rackItem.ExpirationDate;
                            dr["ProductionDate"] = rackItem.ProductionDate;
                            dr["CustomerNum"] = rack.Customer.CustomerNum;
                            dr["CustomerName"] = rack.Customer.CustomerName;
                            dr["Lite1_Thickness"] = rackItem.GlassItem.Lites[0].GlassType.Thickness;
                            dr["Lite1_Color"] = rackItem.GlassItem.Lites[0].GlassType.Color;
                            dr["Lite1_HeatTreatment"] = rackItem.GlassItem.Lites[0].GlassType.HeatTreatment;
                            dr["Lite1_1stSrfCoating"] = rackItem.GlassItem.Lites[0].GlassType.Surface1Coating;
                            dr["Lite1_2ndSrfCoating"] = rackItem.GlassItem.Lites[0].GlassType.Surface2Coating;
                            dr["Lite1_PatternTexture"] = rackItem.GlassItem.Lites[0].GlassType.PatternTexture;
                            dr["Lite1_LamiProdType"] = rackItem.GlassItem.Lites[0].GlassType.LamiProductTypeCode;
                            dr["Lite1_ProdQualityLevel"] = rackItem.GlassItem.Lites[0].QualityLevel;
                            dr["AnsiiLogoType"] = rackItem.GlassItem.Logo.AnsiLogo;
                            dr["ShapeNum"] = rackItem.GlassItem.Shape.ShapeNum;
                            dr["BaseL"] = rackItem.GlassItem.Shape.BaseDimension.Base;
                            dr["BaseN"] = rackItem.GlassItem.Shape.BaseDimension.Numerator;
                            dr["BaseD"] = rackItem.GlassItem.Shape.BaseDimension.Denominator;
                            dr["LeftL"] = rackItem.GlassItem.Shape.LeftDimension.Base;
                            dr["LeftN"] = rackItem.GlassItem.Shape.LeftDimension.Numerator;
                            dr["LeftD"] = rackItem.GlassItem.Shape.LeftDimension.Denominator;
                            dr["RightL"] = rackItem.GlassItem.Shape.RightDimension.Base;
                            dr["RightN"] = rackItem.GlassItem.Shape.RightDimension.Numerator;
                            dr["RightD"] = rackItem.GlassItem.Shape.RightDimension.Denominator;
                            dr["TopL"] = rackItem.GlassItem.Shape.TopDimension.Base;
                            dr["TopN"] = rackItem.GlassItem.Shape.TopDimension.Numerator;
                            dr["TopD"] = rackItem.GlassItem.Shape.TopDimension.Denominator;
                            dr["Side1L"] = rackItem.GlassItem.Shape.Side1Dimension.Base;
                            dr["Side1N"] = rackItem.GlassItem.Shape.Side1Dimension.Numerator;
                            dr["Side1D"] = rackItem.GlassItem.Shape.Side1Dimension.Denominator;
                            dr["Side2L"] = rackItem.GlassItem.Shape.Side2Dimension.Base;
                            dr["Side2N"] = rackItem.GlassItem.Shape.Side2Dimension.Numerator;
                            dr["Side2D"] = rackItem.GlassItem.Shape.Side2Dimension.Denominator;
                            dr["SqftActual"] = rackItem.GlassItem.Shape.ActualSqft;
                            dr["SqftBillable"] = rackItem.GlassItem.Shape.BillableSqft;
                            dr["RackType"] = rack.RackType;
                            dr["LitesPerRack"] = rackItem.GlassItem.LitesPerRack;
                            dr["ProcessID"] = rackItem.ProcessId;
                            dr["GLCategory"] = rackItem.GLCategory;
                            dr["ReasonCode"] = rack.ReasonCode;
                            dr["SequenceNum"] = 0;
                            dr["WrittenDate"] = DateTime.Now;
                            this.Rows.Add(dr);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new FaultException(e.Message);
            }
        }
    }
}
