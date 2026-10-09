using System;
using System.Data;
using System.Linq;
using IGServices.Common.Managers.E1;
using IGServices.Common.Types.Enterprise;
using IGServices.Common.Utilities;

namespace IGServices.Common.Types.ShopFloor
{
    [System.ComponentModel.DesignerCategory("Code")]
    public class RackItemTable : DataTable
    {

        public DateTime MaxDate
        {
            get { return (from a in this.AsEnumerable() select a.Field<DateTime>("TransactionDate")).Max(); }
            set { }
        }

        public decimal MaxId
        {
            get { return (from a in this.AsEnumerable() select a.Field<decimal>("TransactionId")).Max(); }
            set { }
        }

        public RackItemTable()
        {
            ExtendedProperties.Add("Target", "[EBS].[RackDetailLedger]");
            Columns.Add("LotNum", typeof(string));
            Columns.Add("SerialNum", typeof(string));
            Columns.Add("ItemNum", typeof(decimal));
            Columns.Add("StatusCode", typeof(string));

            Columns.Add("RackId", typeof(decimal));

            Columns.Add("QtyOnHand", typeof(decimal));
            Columns.Add("QtyRecd", typeof(decimal));
            Columns.Add("Side", typeof(string));
            Columns.Add("Stack", typeof(decimal));
            Columns.Add("Row", typeof(decimal));
            Columns.Add("Slot", typeof(decimal));
            Columns.Add("Position", typeof(decimal));
            Columns.Add("Rotated", typeof(string));

            Columns.Add("WorkOrderNum", typeof(decimal));
            Columns.Add("DocumentCompany", typeof(string));
            Columns.Add("DocumentNum", typeof(decimal));
            Columns.Add("DocumentType", typeof(string));
            Columns.Add("DocumentLineNum", typeof(decimal));

            Columns.Add("ConfigId", typeof(decimal));
            Columns.Add("StockCode", typeof(string));
            Columns.Add("GlCatCode", typeof(string));

            Columns.Add("ReceivedDate", typeof(DateTime));
            Columns.Add("ManufacturedDate", typeof(DateTime));
            Columns.Add("TransactionDate", typeof(DateTime));

            Columns.Add("CustomerNum", typeof(decimal));
            Columns.Add("CustomerName", typeof(string));
            Columns.Add("SupplierNum", typeof(decimal));
            Columns.Add("SupplierName", typeof(string));

            Columns.Add("TransactionId", typeof(decimal));
            Columns.Add("OperationCode", typeof(string));
            Columns.Add("ProcessCode", typeof(string));
            Columns.Add("UserId", typeof(string));
            Columns.Add("ProgramId", typeof(string));
        }

        public void Add(IDataReader reader, Plant plant)
        {
            try
            {
                while (reader.Read())
                {
                    DataRow dr = this.NewRow();
                    dr["LotNum"] = Data.CastDbNull.To<string>(reader["LOTN"], String.Empty).Trim();
                    dr["SerialNum"] = CastDbNull.To<string>(reader["Z58SN"], String.Empty).Trim();
                    dr["ItemNum"] = CastDbNull.To<decimal>(reader["ITM"], 0);
                    dr["StatusCode"] = CastDbNull.To<string>(reader["Z58DST"], String.Empty).Trim();
                    dr["RackId"] = Data.CastDbNull.To<decimal>(reader["Z58RID"], 0);
                    dr["QtyOnHand"] = CastDbNull.To<decimal>(reader["PQOH"], 0) / 1000;
                    dr["QtyRecd"] = CastDbNull.To<decimal>(reader["RQOH"], 0) / 1000;
                    dr["Side"] = CastDbNull.To<string>(reader["Z58SD"], String.Empty).Trim();
                    dr["Stack"] = CastDbNull.To<decimal>(reader["Z58STK"], 0);
                    dr["Row"] = CastDbNull.To<decimal>(reader["Z58ROW"], 0);
                    dr["Slot"] = CastDbNull.To<decimal>(reader["Z58SL"], 0);
                    dr["Position"] = CastDbNull.To<decimal>(reader["Z58PS"], 0);
                    dr["Rotated"] = CastDbNull.To<string>(reader["Z58BXRO"], String.Empty).Trim();
                    dr["StockCode"] = CastDbNull.To<string>(reader["STKT"], String.Empty).Trim();
                    dr["GlCatCode"] = CastDbNull.To<string>(reader["GLPT"], String.Empty).Trim();
                    var mfgDateId = Data.CastDbNull.To<decimal>(reader["Z58MDT"], 0);
                    dr["ManufacturedDate"] = E1TimeManager.CastToDateTime(mfgDateId);
                    var tranDateId = Data.CastDbNull.To<decimal>(reader["UPMJ"], 0);
                    var tranTimeId = Data.CastDbNull.To<decimal>(reader["TDAY"], 0);
                    dr["TransactionDate"] = E1TimeManager.CastToDateTime(tranDateId, tranTimeId).AddHours(plant.TimeOffset);
                    dr["TransactionId"] = CastDbNull.To<decimal>(reader["UKID"], 0);
                    dr["OperationCode"] = CastDbNull.To<string>(reader["IOPR"], String.Empty).Trim();
                    dr["UserId"] = CastDbNull.To<string>(reader["USERID"], String.Empty).Trim();
                    dr["ProgramId"] = CastDbNull.To<string>(reader["PID"], String.Empty).Trim();

                    // conditional properties
                    if ((string)dr["OperationCode"] == "Init" || (string)dr["OperationCode"] == "Insert")
                    {
                        var recdDateId = Data.CastDbNull.To<decimal>(reader["UPMJ"], 0);
                        dr["ReceivedDate"] = E1TimeManager.CastToDateTime(recdDateId);

                        var mcu = CastDbNull.To<string>(reader["MCU"], String.Empty).Trim();
                        var pdmcu = CastDbNull.To<string>(reader["PDMCU"], String.Empty).Trim();
                        var sdmcu = CastDbNull.To<string>(reader["SDMCU"], String.Empty).Trim();
                        var wamcu = CastDbNull.To<string>(reader["WAMCU"], String.Empty).Trim();
                        var stkt = CastDbNull.To<string>(reader["STKT"], String.Empty).Trim();

                        if (mcu == wamcu)
                        {
                            dr["WorkOrderNum"] = CastDbNull.To<decimal>(reader["DOC1"], 0);
                            dr["CustomerNum"] = CastDbNull.To<decimal>(reader["WAAN8"], 0);
                            dr["CustomerName"] = CastDbNull.To<string>(reader["WAALPH"], String.Empty).Trim();
                        }
                        else
                        {
                            dr["WorkOrderNum"] = 0;
                        }

                        dr["DocumentCompany"] = "";
                        dr["DocumentNum"] = 0;
                        dr["DocumentType"] = "";
                        dr["DocumentLineNum"] = 0;
                        if (mcu == pdmcu)
                        {
                            dr["DocumentCompany"] = CastDbNull.To<string>(reader["PKCO"], String.Empty).Trim();
                            dr["DocumentNum"] = CastDbNull.To<decimal>(reader["DOC2"], 0);
                            dr["DocumentType"] = CastDbNull.To<string>(reader["XCTO"], String.Empty).Trim();
                            dr["DocumentLineNum"] = CastDbNull.To<decimal>(reader["LNIC"], 0) / 1000;
                            dr["SupplierNum"] = CastDbNull.To<decimal>(reader["PDAN8"], 0);
                            dr["SupplierName"] = CastDbNull.To<string>(reader["PDALPH"], String.Empty).Trim();
                        }
                        if (mcu == sdmcu)
                        {
                            dr["DocumentCompany"] = CastDbNull.To<string>(reader["KCOO"], String.Empty).Trim();
                            dr["DocumentNum"] = CastDbNull.To<decimal>(reader["DOCO"], 0);
                            dr["DocumentType"] = CastDbNull.To<string>(reader["DCTO"], String.Empty).Trim();
                            dr["DocumentLineNum"] = CastDbNull.To<decimal>(reader["LNID"], 0) / 1000;
                            dr["CustomerNum"] = CastDbNull.To<decimal>(reader["SDAN8"], 0);
                            dr["CustomerName"] = CastDbNull.To<string>(reader["SDALPH"], String.Empty).Trim();
                        }

                        if (stkt == "C")
                        {
                            dr["ConfigId"] = CastDbNull.To<decimal>(reader["CFGID"], 0);
                            if (mcu == pdmcu)
                            {
                                dr["ProcessCode"] = "PURCFG";
                            }
                            else if (mcu == wamcu)
                            {
                                dr["ProcessCode"] = "PRDCFG";
                            }
                        }
                        else
                        {
                            dr["ConfigId"] = 0;
                            if (mcu == pdmcu)
                            {
                                dr["ProcessCode"] = "PURSTK";
                            }
                            else if (mcu == wamcu)
                            {
                                dr["ProcessCode"] = "PRDSTK";
                            }
                            else
                            {
                                dr["ProcessCode"] = "MISC";
                            }
                        }
                    }
                    Rows.Add(dr);
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
