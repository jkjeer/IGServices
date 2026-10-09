using System;
using System.Data;
using System.Linq;
using System.Text;
using IGServices.Common.Types.Enterprise;

namespace IGServices.Common.Types.ShopFloor
{
    [System.ComponentModel.DesignerCategory("Code")]
    public class SpecUnitTable : DataTable
    {

        public DateTime MaxDate
        {
            get { return (from a in this.AsEnumerable() select a.Field<DateTime>("TransactionDate")).Max(); }
            set { }
        }

        public decimal MaxId
        {
            get { return (from a in this.AsEnumerable() select a.Field<decimal>("ConfigurationId")).Max(); }
            set { }
        }


        public SpecUnitTable()
        {
            ExtendedProperties.Add("Target", "[EBS].[SpecUnitLedger]");
            Columns.Add("ConfigurationId", typeof(decimal));
            Columns.Add("ItemNum", typeof (decimal));
            Columns.Add("StockType", typeof(string));
            Columns.Add("UnitType", typeof(string));
            Columns.Add("TransactionDate", typeof(DateTime));

            Columns.Add("ExtLite_Thick", typeof(decimal));
            Columns.Add("ExtLite_Color", typeof(string));
            Columns.Add("ExtLite_Heat", typeof(string));
            Columns.Add("ExtLite_CoatingSurface1", typeof(string));
            Columns.Add("ExtLite_CoatingSurface2", typeof(string));
            Columns.Add("ExtLite_Pattern", typeof(string));
            Columns.Add("ExtLite_Lami", typeof(string));

            Columns.Add("IntLite_Thick", typeof(decimal));
            Columns.Add("IntLite_Color", typeof(string));
            Columns.Add("IntLite_Heat", typeof(string));
            Columns.Add("IntLite_CoatingSurface1", typeof(string));
            Columns.Add("IntLite_CoatingSurface2", typeof(string));
            Columns.Add("IntLite_Pattern", typeof(string));
            Columns.Add("IntLite_Lami", typeof(string));

            Columns.Add("MidLite_Thick", typeof(decimal));
            Columns.Add("MidLite_Color", typeof(string));
            Columns.Add("MidLite_Heat", typeof(string));
            Columns.Add("MidLite_CoatingSurface1", typeof(string));
            Columns.Add("MidLite_CoatingSurface2", typeof(string));
            Columns.Add("MidLite_Pattern", typeof(string));
            Columns.Add("MidLite_Lami", typeof(string));

            Columns.Add("ShapeNum", typeof(decimal));
            Columns.Add("Dim_Base", typeof(decimal));
            Columns.Add("Dim_Left", typeof(decimal));
            Columns.Add("Dim_Right", typeof(decimal));
            Columns.Add("Dim_Top", typeof(decimal));
            Columns.Add("Dim_Side1", typeof(decimal));
            Columns.Add("Dim_Side2", typeof(decimal));
            Columns.Add("Dim_Base_Frac", typeof(string));
            Columns.Add("Dim_Left_Frac", typeof(string));
            Columns.Add("Dim_Right_Frac", typeof(string));
            Columns.Add("Dim_Top_Frac", typeof(string));
            Columns.Add("Dim_Side1_Frac", typeof(string));
            Columns.Add("Dim_Side2_Frac", typeof(string));
            Columns.Add("TotalWeight", typeof(decimal));
            Columns.Add("UnitWeight", typeof(decimal));
            Columns.Add("ActualSqft", typeof(decimal));

            Columns.Add("LitesPerRack", typeof(decimal));
            Columns.Add("RackOrientVH", typeof(string));
            Columns.Add("RackOrientIO", typeof(string));
            Columns.Add("RackOrientRows", typeof(decimal));
            Columns.Add("RackOrientTiers", typeof(decimal));
            Columns.Add("RackType", typeof(string));

        }

        public void Add(SpecUnitItems specUnitItems)
        {
            try
            {
                foreach (var specUnit in specUnitItems)
                {
                    DataRow dr = NewRow();
                    dr["ConfigurationId"] = specUnit.ConfigId;
                    dr["ItemNum"] = specUnit.ItemNum;
                    dr["StockType"] = specUnit.StockType;
                    dr["UnitType"] = specUnit.UnitType;
                    dr["TransactionDate"] = specUnit.TransactionDate;
                    //dr["SerializedXML"] = XmlSerializationUtility.DataContractSerializeObject(specUnitItem.SpecUnit);

                    dr["ExtLite_Thick"] = specUnit.ItemAttributes.GetDecimalAttributeValue("Z58L1TK");
                    dr["ExtLite_Color"] = specUnit.ItemAttributes.GetStringAttributeDesc3("Z58L1CL", "CL");
                    dr["ExtLite_Heat"] = specUnit.ItemAttributes.GetStringAttributeValue("Z58L1HT");
                    dr["ExtLite_CoatingSurface1"] = specUnit.ItemAttributes.GetStringAttributeValue("Z58L11C");
                    dr["ExtLite_CoatingSurface2"] = specUnit.ItemAttributes.GetStringAttributeValue("Z58L12C");
                    dr["ExtLite_Pattern"] = specUnit.ItemAttributes.GetStringAttributeValue("Z58L1PX");
                    dr["ExtLite_Lami"] = specUnit.ItemAttributes.GetStringAttributeValue("Z58L1LPT");

                    if (specUnit.UnitType == "2")
                    {
                        dr["IntLite_Thick"] = specUnit.ItemAttributes.GetDecimalAttributeValue("Z58L2TK");
                        dr["IntLite_Color"] = specUnit.ItemAttributes.GetStringAttributeDesc3("Z58L2CL", "CL");
                        dr["IntLite_Heat"] = specUnit.ItemAttributes.GetStringAttributeValue("Z58L2HT");
                        dr["IntLite_CoatingSurface1"] = specUnit.ItemAttributes.GetStringAttributeValue("Z58L21C");
                        dr["IntLite_CoatingSurface2"] = specUnit.ItemAttributes.GetStringAttributeValue("Z58L22C");
                        dr["IntLite_Pattern"] = specUnit.ItemAttributes.GetStringAttributeValue("Z58L2PX");
                        dr["IntLite_Lami"] = specUnit.ItemAttributes.GetStringAttributeValue("Z58L2LPT");
                    }
                    else
                    {
                        if (specUnit.UnitType == "3")
                        {
                            dr["MidLite_Thick"] = specUnit.ItemAttributes.GetDecimalAttributeValue("Z58L2TK");
                            dr["MidLite_Color"] = specUnit.ItemAttributes.GetStringAttributeDesc3("Z58L2CL", "CL");
                            dr["MidLite_Heat"] = specUnit.ItemAttributes.GetStringAttributeValue("Z58L2HT");
                            dr["MidLite_CoatingSurface1"] = specUnit.ItemAttributes.GetStringAttributeValue("Z58L21C");
                            dr["MidLite_CoatingSurface2"] = specUnit.ItemAttributes.GetStringAttributeValue("Z58L22C");
                            dr["MidLite_Pattern"] = specUnit.ItemAttributes.GetStringAttributeValue("Z58L2PX");
                            dr["MidLite_Lami"] = specUnit.ItemAttributes.GetStringAttributeValue("Z58L2LPT");

                            dr["IntLite_Thick"] = specUnit.ItemAttributes.GetDecimalAttributeValue("Z58L3TK");
                            dr["IntLite_Color"] = specUnit.ItemAttributes.GetStringAttributeDesc3("Z58L3CL", "CL");
                            dr["IntLite_Heat"] = specUnit.ItemAttributes.GetStringAttributeValue("Z58L3HT");
                            dr["IntLite_CoatingSurface1"] = specUnit.ItemAttributes.GetStringAttributeValue("Z58L31C");
                            dr["IntLite_CoatingSurface2"] = specUnit.ItemAttributes.GetStringAttributeValue("Z58L32C");
                            dr["IntLite_Pattern"] = specUnit.ItemAttributes.GetStringAttributeValue("Z58L3PX");
                            dr["IntLite_Lami"] = specUnit.ItemAttributes.GetStringAttributeValue("Z58L3LPT");
                        }
                    }

                    dr["ShapeNum"] = specUnit.ItemAttributes.GetDecimalAttributeValue("Z58SHAPE");
                    dr["Dim_Base"] = specUnit.ItemAttributes.GetDecimalAttributeValue("Z581BDEC");
                    dr["Dim_Left"] = specUnit.ItemAttributes.GetDecimalAttributeValue("Z581LDEC");
                    dr["Dim_Right"] = specUnit.ItemAttributes.GetDecimalAttributeValue("Z581RDEC");
                    dr["Dim_Top"] = specUnit.ItemAttributes.GetDecimalAttributeValue("Z581TDEC");
                    dr["Dim_Side1"] = specUnit.ItemAttributes.GetDecimalAttributeValue("Z5811DEC");
                    dr["Dim_Side2"] = specUnit.ItemAttributes.GetDecimalAttributeValue("Z5812DEC");

                    dr["Dim_Base_Frac"] = GetDimensionDescription(
                        specUnit.ItemAttributes.GetDecimalAttributeValue("Z58L1BDM"),
                        specUnit.ItemAttributes.GetDecimalAttributeValue("Z58L1BNM"),
                        specUnit.ItemAttributes.GetDecimalAttributeValue("Z58L1BDN"));
                    dr["Dim_Left_Frac"] = GetDimensionDescription(
                        specUnit.ItemAttributes.GetDecimalAttributeValue("Z58L1LDM"),
                        specUnit.ItemAttributes.GetDecimalAttributeValue("Z58L1LNM"),
                        specUnit.ItemAttributes.GetDecimalAttributeValue("Z58L1LDN"));
                    dr["Dim_Right_Frac"] = GetDimensionDescription(
                        specUnit.ItemAttributes.GetDecimalAttributeValue("Z58L1RDM"),
                        specUnit.ItemAttributes.GetDecimalAttributeValue("Z58L1RNM"),
                        specUnit.ItemAttributes.GetDecimalAttributeValue("Z58L1RDN"));
                    dr["Dim_Top_Frac"] = GetDimensionDescription(
                        specUnit.ItemAttributes.GetDecimalAttributeValue("Z58L1TDM"),
                        specUnit.ItemAttributes.GetDecimalAttributeValue("Z58L1TNM"),
                        specUnit.ItemAttributes.GetDecimalAttributeValue("Z58L1TDN"));
                    dr["Dim_Side1_Frac"] = GetDimensionDescription(
                        specUnit.ItemAttributes.GetDecimalAttributeValue("Z58L11DM"),
                        specUnit.ItemAttributes.GetDecimalAttributeValue("Z58L11NM"),
                        specUnit.ItemAttributes.GetDecimalAttributeValue("Z58L11DN"));
                    dr["Dim_Side2_Frac"] = GetDimensionDescription(
                        specUnit.ItemAttributes.GetDecimalAttributeValue("Z58L12DM"),
                        specUnit.ItemAttributes.GetDecimalAttributeValue("Z58L12NM"),
                        specUnit.ItemAttributes.GetDecimalAttributeValue("Z58L12DN"));

                    dr["ActualSqft"] = specUnit.ItemAttributes.GetDecimalAttributeValue("Z58USF");
                    dr["TotalWeight"] = specUnit.ItemAttributes.GetDecimalAttributeValue("Z58GLWT");
                    dr["UnitWeight"] = specUnit.ItemAttributes.GetDecimalAttributeValue("Z58GLWT");

                    dr["LitesPerRack"] = specUnit.ItemAttributes.GetDecimalAttributeValue("Z58LPR");
                    dr["RackOrientVH"] = specUnit.ItemAttributes.GetStringAttributeValue("Z58RPRVH");
                    dr["RackOrientIO"] = specUnit.ItemAttributes.GetStringAttributeValue("Z58RTNIO");
                    dr["RackOrientRows"] = specUnit.ItemAttributes.GetDecimalAttributeValue("Z58ROW");
                    dr["RackOrientTiers"] = specUnit.ItemAttributes.GetDecimalAttributeValue("Z58STK");
                    dr["RackType"] = specUnit.ItemAttributes.GetStringAttributeValue("Z58RT");

                    Rows.Add(dr);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void Add(SpecUnits specUnits)
        {
            try
            {
                foreach (SpecUnit specUnit in specUnits)
                {
                    DataRow dr = NewRow();
                    dr["ConfigurationId"] = specUnit.Id;
                    dr["ItemNum"] = specUnit.ItemNum;
                    dr["StockType"] = specUnit.StockType;
                    dr["UnitType"] = specUnit.UnitType;
                    dr["TransactionDate"] = specUnit.TransactionDate;

                    dr["ExtLite_Thick"] = specUnit.ItemAttributes.GetDecimalAttributeValue("Z58L1TK");
                    dr["ExtLite_Color"] = specUnit.ItemAttributes.GetStringAttributeDesc3("Z58L1CL", "CL");
                    dr["ExtLite_Heat"] = specUnit.ItemAttributes.GetStringAttributeValue("Z58L1HT");
                    dr["ExtLite_CoatingSurface1"] = specUnit.ItemAttributes.GetStringAttributeValue("Z58L11C");
                    dr["ExtLite_CoatingSurface2"] = specUnit.ItemAttributes.GetStringAttributeValue("Z58L12C");
                    dr["ExtLite_Pattern"] = specUnit.ItemAttributes.GetStringAttributeValue("Z58L1PX");
                    dr["ExtLite_Lami"] = specUnit.ItemAttributes.GetStringAttributeValue("Z58L1LPT");

                    if (specUnit.UnitType == "2")
                    {
                        dr["IntLite_Thick"] = specUnit.ItemAttributes.GetDecimalAttributeValue("Z58L2TK");
                        dr["IntLite_Color"] = specUnit.ItemAttributes.GetStringAttributeDesc3("Z58L2CL", "CL");
                        dr["IntLite_Heat"] = specUnit.ItemAttributes.GetStringAttributeValue("Z58L2HT");
                        dr["IntLite_CoatingSurface1"] = specUnit.ItemAttributes.GetStringAttributeValue("Z58L21C");
                        dr["IntLite_CoatingSurface2"] = specUnit.ItemAttributes.GetStringAttributeValue("Z58L22C");
                        dr["IntLite_Pattern"] = specUnit.ItemAttributes.GetStringAttributeValue("Z58L2PX");
                        dr["IntLite_Lami"] = specUnit.ItemAttributes.GetStringAttributeValue("Z58L2LPT");
                    }
                    else
                    {
                        if (specUnit.UnitType == "3")
                        {
                            dr["MidLite_Thick"] = specUnit.ItemAttributes.GetDecimalAttributeValue("Z58L2TK");
                            dr["MidLite_Color"] = specUnit.ItemAttributes.GetStringAttributeDesc3("Z58L2CL", "CL");
                            dr["MidLite_Heat"] = specUnit.ItemAttributes.GetStringAttributeValue("Z58L2HT");
                            dr["MidLite_CoatingSurface1"] = specUnit.ItemAttributes.GetStringAttributeValue("Z58L21C");
                            dr["MidLite_CoatingSurface2"] = specUnit.ItemAttributes.GetStringAttributeValue("Z58L22C");
                            dr["MidLite_Pattern"] = specUnit.ItemAttributes.GetStringAttributeValue("Z58L2PX");
                            dr["MidLite_Lami"] = specUnit.ItemAttributes.GetStringAttributeValue("Z58L2LPT");

                            dr["IntLite_Thick"] = specUnit.ItemAttributes.GetDecimalAttributeValue("Z58L3TK");
                            dr["IntLite_Color"] = specUnit.ItemAttributes.GetStringAttributeDesc3("Z58L3CL", "CL");
                            dr["IntLite_Heat"] = specUnit.ItemAttributes.GetStringAttributeValue("Z58L3HT");
                            dr["IntLite_CoatingSurface1"] = specUnit.ItemAttributes.GetStringAttributeValue("Z58L31C");
                            dr["IntLite_CoatingSurface2"] = specUnit.ItemAttributes.GetStringAttributeValue("Z58L32C");
                            dr["IntLite_Pattern"] = specUnit.ItemAttributes.GetStringAttributeValue("Z58L3PX");
                            dr["IntLite_Lami"] = specUnit.ItemAttributes.GetStringAttributeValue("Z58L3LPT");
                        }
                    }

                    dr["ShapeNum"] = specUnit.ItemAttributes.GetDecimalAttributeValue("Z58SHAPE");
                    dr["Dim_Base"] = specUnit.ItemAttributes.GetDecimalAttributeValue("Z581BDEC");
                    dr["Dim_Left"] = specUnit.ItemAttributes.GetDecimalAttributeValue("Z581LDEC");
                    dr["Dim_Right"] = specUnit.ItemAttributes.GetDecimalAttributeValue("Z581RDEC");
                    dr["Dim_Top"] = specUnit.ItemAttributes.GetDecimalAttributeValue("Z581TDEC");
                    dr["Dim_Side1"] = specUnit.ItemAttributes.GetDecimalAttributeValue("Z5811DEC");
                    dr["Dim_Side2"] = specUnit.ItemAttributes.GetDecimalAttributeValue("Z5812DEC");

                    dr["Dim_Base_Frac"] = GetDimensionDescription(
                        specUnit.ItemAttributes.GetDecimalAttributeValue("Z58L1BDM"),
                        specUnit.ItemAttributes.GetDecimalAttributeValue("Z58L1BNM"),
                        specUnit.ItemAttributes.GetDecimalAttributeValue("Z58L1BDN"));
                    dr["Dim_Left_Frac"] = GetDimensionDescription(
                        specUnit.ItemAttributes.GetDecimalAttributeValue("Z58L1LDM"),
                        specUnit.ItemAttributes.GetDecimalAttributeValue("Z58L1LNM"),
                        specUnit.ItemAttributes.GetDecimalAttributeValue("Z58L1LDN"));
                    dr["Dim_Right_Frac"] = GetDimensionDescription(
                        specUnit.ItemAttributes.GetDecimalAttributeValue("Z58L1RDM"),
                        specUnit.ItemAttributes.GetDecimalAttributeValue("Z58L1RNM"),
                        specUnit.ItemAttributes.GetDecimalAttributeValue("Z58L1RDN"));
                    dr["Dim_Top_Frac"] = GetDimensionDescription(
                        specUnit.ItemAttributes.GetDecimalAttributeValue("Z58L1TDM"),
                        specUnit.ItemAttributes.GetDecimalAttributeValue("Z58L1TNM"),
                        specUnit.ItemAttributes.GetDecimalAttributeValue("Z58L1TDN"));
                    dr["Dim_Side1_Frac"] = GetDimensionDescription(
                        specUnit.ItemAttributes.GetDecimalAttributeValue("Z58L11DM"),
                        specUnit.ItemAttributes.GetDecimalAttributeValue("Z58L11NM"),
                        specUnit.ItemAttributes.GetDecimalAttributeValue("Z58L11DN"));
                    dr["Dim_Side2_Frac"] = GetDimensionDescription(
                        specUnit.ItemAttributes.GetDecimalAttributeValue("Z58L12DM"),
                        specUnit.ItemAttributes.GetDecimalAttributeValue("Z58L12NM"),
                        specUnit.ItemAttributes.GetDecimalAttributeValue("Z58L12DN"));

                    dr["ActualSqft"] = specUnit.ItemAttributes.GetDecimalAttributeValue("Z58USF");
                    dr["TotalWeight"] = specUnit.ItemAttributes.GetDecimalAttributeValue("Z58GLWT");
                    dr["UnitWeight"] = specUnit.ItemAttributes.GetDecimalAttributeValue("Z58GLWT");

                    dr["LitesPerRack"] = specUnit.ItemAttributes.GetDecimalAttributeValue("Z58LPR");
                    dr["RackOrientVH"] = specUnit.ItemAttributes.GetStringAttributeValue("Z58RPRVH");
                    dr["RackOrientIO"] = specUnit.ItemAttributes.GetStringAttributeValue("Z58RTNIO");
                    dr["RackOrientRows"] = specUnit.ItemAttributes.GetDecimalAttributeValue("Z58ROW");
                    dr["RackOrientTiers"] = specUnit.ItemAttributes.GetDecimalAttributeValue("Z58STK");
                    dr["RackType"] = specUnit.ItemAttributes.GetStringAttributeValue("Z58RT");

                    Rows.Add(dr);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        private string GetDimensionDescription(decimal baseDim, decimal numDim, decimal denDim)
        {
            string result = "";
            if (baseDim > 0 || numDim > 0)
            {
                var sb = new StringBuilder();
                sb.Append(baseDim.ToString());
                if (numDim > 0)
                {
                    sb.Append(" ");
                    sb.Append(numDim.ToString());
                    sb.Append("/");
                    sb.Append(denDim.ToString());
                }
                result = sb.ToString();
            }
            return result;
        }

    }
}
