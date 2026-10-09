using System;
using System.Data;
using System.Linq;
using System.ServiceModel;
using IGServices.Common.Types.Configuration;
using IGServices.Common.Utilities;

namespace IGServices.Common.Types.ShopFloor
{
    [System.ComponentModel.DesignerCategory("Code")]
    public class SalesLineTable : DataTable
    {

        public DateTime MaxDate
        {
            get { return (from a in this.AsEnumerable() select a.Field<DateTime>("TransactionDate")).Max(); }
            set { }
        }

        public SalesLineTable()
        {
            ExtendedProperties.Add("Target", "[EBS].[SalesLineLedger]");
            Columns.Add("OrderNum", typeof(decimal));
            Columns.Add("OrderType", typeof(string));
            Columns.Add("OrderLineNum", typeof(decimal));

            Columns.Add("LineType", typeof(string));
            Columns.Add("VolumeType", typeof(string));
            Columns.Add("Quantity", typeof(decimal));
            Columns.Add("LastStatus", typeof(string));
            Columns.Add("NextStatus", typeof(string));

            Columns.Add("WOrderNum", typeof(decimal));
            Columns.Add("ItemNum", typeof(decimal));
            Columns.Add("ConfigId", typeof(decimal));
            Columns.Add("GLCategory", typeof(string));
            Columns.Add("StockType", typeof(string));

            Columns.Add("OrderDate", typeof(DateTime));
            Columns.Add("RequestedDate", typeof(DateTime));
            Columns.Add("CancelDate", typeof(DateTime));
            Columns.Add("ShipDate", typeof(DateTime));
            Columns.Add("InvoiceDate", typeof(DateTime));

            Columns.Add("CustomerSoldNum", typeof(decimal));
            Columns.Add("CustomerShipNum", typeof(decimal));
            Columns.Add("CustomerOrderNum", typeof(string));
            Columns.Add("CustomerPart1", typeof(string));
            Columns.Add("CustomerPart2", typeof(string));
            Columns.Add("CustomerPart3", typeof(string));
            Columns.Add("CustomerSeqNum", typeof(string));
            Columns.Add("GenericSeqNum", typeof(string));
            Columns.Add("ProductType", typeof(string));
            Columns.Add("ReasonCode", typeof(string));
            Columns.Add("Priority", typeof(string));

            Columns.Add("UnitCostAmt", typeof(decimal));
            Columns.Add("UnitPriceAmt", typeof(decimal));

            Columns.Add("InvoiceNum", typeof(decimal));
            Columns.Add("LoadNum", typeof(decimal));

            Columns.Add("TransactionDate", typeof(DateTime));
        }

        public void Add(IDataReader reader, Enterprise.Plant plant)
        {
            try
            {
                while (reader.Read())
                {
                    DataRow dr = this.NewRow();
                    dr["OrderNum"] = CastDbNull.To<decimal>(reader["SDDOCO"], 0);
                    dr["OrderType"] = CastDbNull.To<string>(reader["SDDCTO"], String.Empty).Trim();
                    dr["OrderLineNum"] = CastDbNull.To<decimal>(reader["SDLNID"], 0) / 1000;

                    dr["LineType"] = CastDbNull.To<string>(reader["SDLNTY"], String.Empty).Trim();
                    dr["VolumeType"] = CastDbNull.To<string>(reader["ATZ5840460"], String.Empty).Trim();
                    dr["Quantity"] = CastDbNull.To<decimal>(reader["SDUORG"], 0) / 1000;
                    dr["LastStatus"] = CastDbNull.To<string>(reader["SDLTTR"], String.Empty).Trim();
                    dr["NextStatus"] = CastDbNull.To<string>(reader["SDNXTR"], String.Empty).Trim();

                    dr["WOrderNum"] = Data.CastStringToDecimal(CastDbNull.To<string>(reader["SDRORN"], String.Empty).Trim());
                    dr["ItemNum"] = CastDbNull.To<decimal>(reader["SDITM"], 0);
                    dr["ConfigId"] = CastDbNull.To<decimal>(reader["CMCFGID"], 0);
                    dr["GLCategory"] = CastDbNull.To<string>(reader["SDGLC"], String.Empty).Trim();
                    //dr["StockType"] = CastDbNull.To<string>(reader["IBSTKT"], String.Empty).Trim();

                    dr["OrderDate"] = Timestamp.CastToDateTime(Data.CastDbNull.To<decimal>(reader["SDTRDJ"], 0));
                    dr["RequestedDate"] = Timestamp.CastToDateTime(Data.CastDbNull.To<decimal>(reader["SDDRQJ"], 0));
                    dr["CancelDate"] = Timestamp.CastToDateTime(Data.CastDbNull.To<decimal>(reader["SDCNDJ"], 0));
                    dr["ShipDate"] = Timestamp.CastToDateTime(Data.CastDbNull.To<decimal>(reader["SDADDJ"], 0));
                    dr["InvoiceDate"] = Timestamp.CastToDateTime(Data.CastDbNull.To<decimal>(reader["SDIVD"], 0));

                    dr["CustomerSoldNum"] = CastDbNull.To<decimal>(reader["SDAN8"], 0);
                    dr["CUstomerShipNum"] = CastDbNull.To<decimal>(reader["SDSHAN"], 0);
                    dr["CustomerOrderNum"] = CastDbNull.To<string>(reader["SDVR01"], String.Empty).Trim();
                    dr["CustomerPart1"] = CastDbNull.To<string>(reader["ATZ58CP2"], String.Empty).Trim();
                    dr["CustomerPart2"] = CastDbNull.To<string>(reader["ATZ58CP3"], String.Empty).Trim();
                    dr["CustomerPart3"] = CastDbNull.To<string>(reader["ATZ58CP4"], String.Empty).Trim();
                    dr["CustomerSeqNum"] = CastDbNull.To<string>(reader["ATZ58CSQNO"], String.Empty).Trim();
                    dr["GenericSeqNum"] = CastDbNull.To<string>(reader["ATZ58GSQNO"], String.Empty).Trim();
                    dr["ProductType"] = CastDbNull.To<string>(reader["ATZ58PT"], String.Empty).Trim();
                    dr["ReasonCode"] = CastDbNull.To<string>(reader["SDRCD"], String.Empty).Trim();
                    dr["Priority"] = CastDbNull.To<string>(reader["SDURCD"], String.Empty).Trim();

                    dr["UnitCostAmt"] = CastDbNull.To<decimal>(reader["SDUNCS"], 0) / 10000;
                    dr["UnitPriceAmt"] = CastDbNull.To<decimal>(reader["SDUPRC"], 0) / 10000;

                    dr["LoadNum"] = CastDbNull.To<decimal>(reader["SDPSN"], 0);
                    dr["InvoiceNum"] = CastDbNull.To<decimal>(reader["SDDOC"], 0);

                    //dr["TransactionDate"] = Timestamp.CastToDateTime(Data.CastDbNull.To<decimal>(reader["SDUPMJ"], 0), Data.CastDbNull.To<decimal>(reader["SDTDAY"], 0));
                    dr["TransactionDate"] = Timestamp.CastToDateTime(Data.CastDbNull.To<decimal>(reader["SDUPMJ"], 0),
                        Data.CastDbNull.To<decimal>(reader["SDTDAY"], 0)).AddHours(plant.TimeOffset);
                    this.Rows.Add(dr);
                }
                reader.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
