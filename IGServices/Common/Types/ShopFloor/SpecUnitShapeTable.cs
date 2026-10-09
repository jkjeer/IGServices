using System;
using System.Data;
using System.ServiceModel;
using IGServices.Common.Types.Enterprise;

namespace IGServices.Common.Types.ShopFloor
{

    [System.ComponentModel.DesignerCategory("Code")]
    public class SpecUnitShapeTable : DataTable
    {

        public SpecUnitShapeTable()
        {
            ExtendedProperties.Add("Target", "[ITM].[SPEC_UNIT_SHAPE_LEDGER]");
            Columns.Add("ITEM_NUM", typeof(decimal));
            Columns.Add("CONFIGURATION_ID", typeof(decimal));
            Columns.Add("SHAPE_NO", typeof(decimal));
            Columns.Add("BASE_DIMENSION", typeof(decimal));
            Columns.Add("BASE_DIMENSION_MM", typeof(decimal));
            Columns.Add("BASE_DIMENSION_INT", typeof(int));
            Columns.Add("BASE_DIMENSION_NUM", typeof(int));
            Columns.Add("BASE_DIMENSION_DEN", typeof(int));
            Columns.Add("BASE_DIMENSION_DESC", typeof(string));
            Columns.Add("LEFT_DIMENSION", typeof(decimal));
            Columns.Add("LEFT_DIMENSION_MM", typeof(decimal));
            Columns.Add("LEFT_DIMENSION_INT", typeof(int));
            Columns.Add("LEFT_DIMENSION_NUM", typeof(int));
            Columns.Add("LEFT_DIMENSION_DEN", typeof(int));
            Columns.Add("LEFT_DIMENSION_DESC", typeof(string));
            Columns.Add("RIGHT_DIMENSION", typeof(decimal));
            Columns.Add("RIGHT_DIMENSION_MM", typeof(decimal));
            Columns.Add("RIGHT_DIMENSION_INT", typeof(int));
            Columns.Add("RIGHT_DIMENSION_NUM", typeof(int));
            Columns.Add("RIGHT_DIMENSION_DEN", typeof(int));
            Columns.Add("RIGHT_DIMENSION_DESC", typeof(string));
            Columns.Add("TOP_DIMENSION", typeof(decimal));
            Columns.Add("TOP_DIMENSION_MM", typeof(decimal));
            Columns.Add("TOP_DIMENSION_INT", typeof(int));
            Columns.Add("TOP_DIMENSION_NUM", typeof(int));
            Columns.Add("TOP_DIMENSION_DEN", typeof(int));
            Columns.Add("TOP_DIMENSION_DESC", typeof(string));
            Columns.Add("SIDE1_DIMENSION", typeof(decimal));
            Columns.Add("SIDE1_DIMENSION_MM", typeof(decimal));
            Columns.Add("SIDE1_DIMENSION_INT", typeof(int));
            Columns.Add("SIDE1_DIMENSION_NUM", typeof(int));
            Columns.Add("SIDE1_DIMENSION_DEN", typeof(int));
            Columns.Add("SIDE1_DIMENSION_DESC", typeof(string));
            Columns.Add("SIDE2_DIMENSION", typeof(decimal));
            Columns.Add("SIDE2_DIMENSION_MM", typeof(decimal));
            Columns.Add("SIDE2_DIMENSION_INT", typeof(int));
            Columns.Add("SIDE2_DIMENSION_NUM", typeof(int));
            Columns.Add("SIDE2_DIMENSION_DEN", typeof(int));
            Columns.Add("SIDE2_DIMENSION_DESC", typeof(string));
            Columns.Add("ACTUAL_SQFT", typeof(decimal));
            Columns.Add("BILLABLE_SQFT", typeof(decimal));
            Columns.Add("PERIMETER_BLOCK_SIZE", typeof(decimal));
            Columns.Add("GLASS_WEIGHT_LBS", typeof(decimal));
        }

        public void Add(SpecUnit item)
        {
            try
            {
                DataRow dr = NewRow();
                var gs = item.GlassItem.Shape;
                dr["ITEM_NUM"] = item.ItemNum;
                dr["CONFIGURATION_ID"] = item.Id;
                dr["SHAPE_NO"] = (int)gs.ShapeNum;

                var leg = new Dimension();

                leg = gs.BaseDimension;
                dr["BASE_DIMENSION"] = leg.DecimalValue;
                dr["BASE_DIMENSION_MM"] = leg.MMValue;
                dr["BASE_DIMENSION_INT"] = (int)leg.Base;
                dr["BASE_DIMENSION_NUM"] = (int)leg.Numerator;
                dr["BASE_DIMENSION_DEN"] = (int)leg.Denominator;
                dr["BASE_DIMENSION_DESC"] = leg.Description;

                leg = gs.LeftDimension;
                dr["LEFT_DIMENSION"] = leg.DecimalValue;
                dr["LEFT_DIMENSION_MM"] = leg.MMValue;
                dr["LEFT_DIMENSION_INT"] = (int)leg.Base;
                dr["LEFT_DIMENSION_NUM"] = (int)leg.Numerator;
                dr["LEFT_DIMENSION_DEN"] = (int)leg.Denominator;
                dr["LEFT_DIMENSION_DESC"] = leg.Description;

                leg = gs.RightDimension;
                dr["RIGHT_DIMENSION"] = leg.DecimalValue;
                dr["RIGHT_DIMENSION_MM"] = leg.MMValue;
                dr["RIGHT_DIMENSION_INT"] = (int)leg.Base;
                dr["RIGHT_DIMENSION_NUM"] = (int)leg.Numerator;
                dr["RIGHT_DIMENSION_DEN"] = (int)leg.Denominator;
                dr["RIGHT_DIMENSION_DESC"] = leg.Description;

                leg = gs.TopDimension;
                dr["TOP_DIMENSION"] = leg.DecimalValue;
                dr["TOP_DIMENSION_MM"] = leg.MMValue;
                dr["TOP_DIMENSION_INT"] = (int)leg.Base;
                dr["TOP_DIMENSION_NUM"] = (int)leg.Numerator;
                dr["TOP_DIMENSION_DEN"] = (int)leg.Denominator;
                dr["TOP_DIMENSION_DESC"] = leg.Description;

                leg = gs.Side1Dimension;
                dr["SIDE1_DIMENSION"] = leg.DecimalValue;
                dr["SIDE1_DIMENSION_MM"] = leg.MMValue;
                dr["SIDE1_DIMENSION_INT"] = (int)leg.Base;
                dr["SIDE1_DIMENSION_NUM"] = (int)leg.Numerator;
                dr["SIDE1_DIMENSION_DEN"] = (int)leg.Denominator;
                dr["SIDE1_DIMENSION_DESC"] = leg.Description;

                leg = gs.Side2Dimension;
                dr["SIDE2_DIMENSION"] = leg.DecimalValue;
                dr["SIDE2_DIMENSION_MM"] = leg.MMValue;
                dr["SIDE2_DIMENSION_INT"] = (int)leg.Base;
                dr["SIDE2_DIMENSION_NUM"] = (int)leg.Numerator;
                dr["SIDE2_DIMENSION_DEN"] = (int)leg.Denominator;
                dr["SIDE2_DIMENSION_DESC"] = leg.Description;

                dr["ACTUAL_SQFT"] = gs.ActualSqft;
                dr["BILLABLE_SQFT"] = gs.BillableSqft;
                dr["PERIMETER_BLOCK_SIZE"] = gs.BlockSizePerimeter;
                dr["GLASS_WEIGHT_LBS"] = item.GlassItem.TotalWeight;

                Rows.Add(dr);
            }
            catch (Exception e)
            {
                throw new FaultException(e.Message);
            }
        }

    }
}
