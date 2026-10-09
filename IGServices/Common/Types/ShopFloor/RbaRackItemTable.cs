using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IGServices.Common.Managers.E1;
using IGServices.Common.Types.Enterprise;
using IGServices.Common.Utilities;

namespace IGServices.Common.Types.ShopFloor
{
    [System.ComponentModel.DesignerCategory("Code")]

    public class RbaRackItemTable : DataTable
    {
        public DateTime MaxDate
        {
            get { return (from a in this.AsEnumerable() select a.Field<DateTime>("TransactionDate")).Max(); }
            set { }
        }

        public RbaRackItemTable()
        {
            Columns.Add("LotNum", typeof(string));
            Columns.Add("SerialNum", typeof(string));

            Columns.Add("HeaderStatus", typeof(string));
            Columns.Add("DetailStatus", typeof(string));
            Columns.Add("RackNum", typeof(decimal));
            Columns.Add("RackType", typeof(string));
            Columns.Add("CardinalProductType", typeof(string));
            Columns.Add("ProductType", typeof(string));
            Columns.Add("CustomerSoldNum", typeof(decimal));
            Columns.Add("CustomerShipNum", typeof(decimal));
            Columns.Add("CustomerPart1", typeof(string));
            Columns.Add("Batch", typeof(decimal));
            Columns.Add("Bin", typeof(int));
            Columns.Add("CustomerPart2", typeof(string));
            Columns.Add("CustomerPart3", typeof(string));
            Columns.Add("PriorityCode", typeof(string));

            Columns.Add("SOrderNum", typeof(decimal));
            Columns.Add("SOrderType", typeof(string));
            Columns.Add("SOrderLine", typeof(decimal));
            Columns.Add("WOrderNum", typeof(decimal));
            Columns.Add("ScheduleNum", typeof(decimal));
            Columns.Add("BoxNum", typeof(decimal));
            Columns.Add("SlotNum", typeof(decimal));
            Columns.Add("LoadNum", typeof(decimal));
            Columns.Add("LoadStatus", typeof(string));

            Columns.Add("OrderDate", typeof(DateTime));
            Columns.Add("ScheduleDate", typeof(DateTime));
            Columns.Add("DueDate", typeof(DateTime));
            Columns.Add("ShipDate", typeof(DateTime));
            Columns.Add("ShipEstDate", typeof(DateTime));
            Columns.Add("TransactionDate", typeof(DateTime));
        }

        public void AddShippedRackItem(IDataReader reader, Plant plant)
        {
            try
            {
                while (reader.Read())
                {
                    var dr = NewRow();
                    dr["LotNum"] = CastDbNull.To<string>(reader["RDLOTN"], String.Empty).Trim();
                    dr["SerialNum"] = CastDbNull.To<string>(reader["RDZ58SN"], String.Empty).Trim();
                    dr["HeaderStatus"] = CastDbNull.To<string>(reader["RHZ58HST"], String.Empty).Trim();
                    dr["DetailStatus"] = CastDbNull.To<string>(reader["RDZ58DST"], String.Empty).Trim();
                    dr["RackNum"] = CastDbNull.To<decimal>(reader["RDZ58RID"], 0);
                    dr["RackType"] = CastDbNull.To<string>(reader["RHZ58RT"], String.Empty).Trim();
                    dr["CardinalProductType"] = CastDbNull.To<string>(reader["Z5840310"], String.Empty).Trim();
                    dr["ProductType"] = CastDbNull.To<string>(reader["ATZ58PT"], String.Empty).Trim();
                    dr["CustomerSoldNum"] = CastDbNull.To<decimal>(reader["SDAN8"], 0);
                    dr["CustomerShipNum"] = CastDbNull.To<decimal>(reader["SDSHAN"], 0);
                    var custPart1 = CastDbNull.To<string>(reader["ATZ58CP2"], String.Empty).Trim();
                    dr["CustomerPart1"] = custPart1;
                    var arrBatchBin = custPart1.Split('-');
                    if (arrBatchBin.Length > 1)
                    {
                        decimal batch = Utilities.Data.CastStringToDecimal(arrBatchBin[0]);
                        decimal bin = Utilities.Data.CastStringToDecimal(arrBatchBin[1]);
                        dr["Batch"] = batch;
                        dr["Bin"] = bin;
                        if (batch == 0 && bin == 0)
                        {
                            dr["CardinalProductType"] = "CRATE_RBA_RECT";
                        }
                    }
                    else
                    {
                        dr["Batch"] = 0;
                        dr["Bin"] = 0;
                        dr["CardinalProductType"] = "CRATE_RBA_RECT";
                    }
                    dr["CustomerPart2"] = CastDbNull.To<string>(reader["ATZ58CP3"], String.Empty).Trim();
                    dr["CustomerPart3"] = CastDbNull.To<string>(reader["ATZ58CP4"], String.Empty).Trim();
                    dr["PriorityCode"] = CastDbNull.To<string>(reader["SDURCD"], String.Empty).Trim();
                    dr["SOrderNum"] = CastDbNull.To<decimal>(reader["SDDOCO"], 0);
                    dr["SOrderType"] = CastDbNull.To<string>(reader["SDDCTO"], String.Empty).Trim();
                    dr["SOrderLine"] = CastDbNull.To<decimal>(reader["SDLNID"], 0) / 1000;
                    dr["WOrderNum"] = Utilities.Data.ParseStringToDecimal(CastDbNull.To<string>(reader["SDRORN"], String.Empty).Trim());
                    dr["ScheduleNum"] = CastDbNull.To<decimal>(reader["S5Z58SCN"], 0);
                    dr["BoxNum"] = CastDbNull.To<decimal>(reader["RHZ58BOX"], 0);
                    dr["SlotNum"] = CastDbNull.To<decimal>(reader["RDZ58SL"], 0);
                    dr["LoadNum"] = CastDbNull.To<decimal>(reader["LHLDNM"], 0);
                    dr["LoadStatus"] = CastDbNull.To<string>(reader["LHZ58LDS"], String.Empty).Trim();
                    dr["OrderDate"] = E1TimeManager.CastToDateTime(CastDbNull.To<decimal>(reader["SDTRDJ"], 0));
                    dr["ScheduleDate"] = E1TimeManager.CastToDateTime(CastDbNull.To<decimal>(reader["S5Z58SCD"], 0));
                    dr["DueDate"] = E1TimeManager.CastToDateTime(CastDbNull.To<decimal>(reader["SDDRQJ"], 0));
                    var shipDateId = CastDbNull.To<decimal>(reader["LHADDJ"], 0);
                    var shipTimeId = CastDbNull.To<decimal>(reader["LHADTM"], 0);
                    dr["ShipDate"] = E1TimeManager.CastToDateTime(shipDateId, shipTimeId);
                    var shipEstDateId = CastDbNull.To<decimal>(reader["LHZ58EDTA"], 0);
                    var shipEstTimeId = CastDbNull.To<decimal>(reader["LHZ58ETMA"], 0);
                    dr["ShipEstDate"] = E1TimeManager.CastToDateTime(shipEstDateId, shipEstTimeId);
                    var updateDateId = CastDbNull.To<decimal>(reader["RDUPMJ"], 0);
                    var updateTimeId = CastDbNull.To<decimal>(reader["RDTDAY"], 0);
                    dr["TransactionDate"] = E1TimeManager.CastToDateTime(updateDateId, updateTimeId).AddHours(plant.TimeOffset);
                    Rows.Add(dr);
                }
                reader.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void AddCurrentRackItem(IDataReader reader, Plant plant)
        {
            try
            {
                while (reader.Read())
                {
                    var dr = NewRow();
                    dr["LotNum"] = CastDbNull.To<string>(reader["RDLOTN"], String.Empty).Trim();
                    dr["SerialNum"] = CastDbNull.To<string>(reader["RDZ58SN"], String.Empty).Trim();
                    dr["HeaderStatus"] = CastDbNull.To<string>(reader["RHZ58HST"], String.Empty).Trim();
                    dr["DetailStatus"] = CastDbNull.To<string>(reader["RDZ58DST"], String.Empty).Trim();
                    dr["RackNum"] = CastDbNull.To<decimal>(reader["RDZ58RID"], 0);
                    dr["RackType"] = CastDbNull.To<string>(reader["RHZ58RT"], String.Empty).Trim();
                    dr["CardinalProductType"] = CastDbNull.To<string>(reader["Z5840310"], String.Empty).Trim();
                    dr["ProductType"] = CastDbNull.To<string>(reader["ATZ58PT"], String.Empty).Trim();
                    dr["CustomerSoldNum"] = CastDbNull.To<decimal>(reader["SDAN8"], 0);
                    dr["CustomerShipNum"] = CastDbNull.To<decimal>(reader["SDSHAN"], 0);
                    var custPart1 = CastDbNull.To<string>(reader["ATZ58CP2"], String.Empty).Trim();
                    dr["CustomerPart1"] = custPart1;
                    var arrBatchBin = custPart1.Split('-');
                    if (arrBatchBin.Length > 1)
                    {
                        decimal batch = Utilities.Data.CastStringToDecimal(arrBatchBin[0]);
                        decimal bin = Utilities.Data.CastStringToDecimal(arrBatchBin[1]);
                        dr["Batch"] = batch;
                        dr["Bin"] = bin;
                        if (batch == 0 && bin == 0)
                        {
                            dr["CardinalProductType"] = "CRATE_RBA_RECT";
                        }
                    }
                    else
                    {
                        dr["Batch"] = 0;
                        dr["Bin"] = 0;
                        dr["CardinalProductType"] = "CRATE_RBA_RECT";
                    }
                    dr["CustomerPart2"] = CastDbNull.To<string>(reader["ATZ58CP3"], String.Empty).Trim();
                    dr["CustomerPart3"] = CastDbNull.To<string>(reader["ATZ58CP4"], String.Empty).Trim();
                    dr["PriorityCode"] = CastDbNull.To<string>(reader["SDURCD"], String.Empty).Trim();
                    dr["SOrderNum"] = CastDbNull.To<decimal>(reader["SDDOCO"], 0);
                    dr["SOrderType"] = CastDbNull.To<string>(reader["SDDCTO"], String.Empty).Trim();
                    dr["SOrderLine"] = CastDbNull.To<decimal>(reader["SDLNID"], 0) / 1000;
                    dr["WOrderNum"] = Utilities.Data.ParseStringToDecimal(CastDbNull.To<string>(reader["SDRORN"], String.Empty).Trim());
                    dr["ScheduleNum"] = CastDbNull.To<decimal>(reader["S5Z58SCN"], 0);
                    dr["BoxNum"] = CastDbNull.To<decimal>(reader["RHZ58BOX"], 0);
                    dr["SlotNum"] = CastDbNull.To<decimal>(reader["RDZ58SL"], 0);
                    dr["LoadNum"] = CastDbNull.To<decimal>(reader["RHPSN"], 0);
                    dr["LoadStatus"] = CastDbNull.To<string>(reader["LHZ58LDS"], String.Empty).Trim();
                    dr["OrderDate"] = E1TimeManager.CastToDateTime(CastDbNull.To<decimal>(reader["SDTRDJ"], 0));
                    dr["ScheduleDate"] = E1TimeManager.CastToDateTime(CastDbNull.To<decimal>(reader["S5Z58SCD"], 0));
                    dr["DueDate"] = E1TimeManager.CastToDateTime(CastDbNull.To<decimal>(reader["SDDRQJ"], 0));
                    //var shipDateId = CastDbNull.To<decimal>(reader["LHADDJ"], 0);
                    //var shipTimeId = CastDbNull.To<decimal>(reader["LHADTM"], 0);
                    //dr["ShipDate"] = E1TimeManager.CastToDateTime(shipDateId, shipTimeId);
                    var shipEstDateId = CastDbNull.To<decimal>(reader["LHZ58EDTA"], 0);
                    var shipEstTimeId = CastDbNull.To<decimal>(reader["LHZ58ETMA"], 0);
                    dr["ShipEstDate"] = E1TimeManager.CastToDateTime(shipEstDateId, shipEstTimeId);
                    var updateDateId = CastDbNull.To<decimal>(reader["RDUPMJ"], 0);
                    var updateTimeId = CastDbNull.To<decimal>(reader["RDTDAY"], 0);
                    dr["TransactionDate"] = E1TimeManager.CastToDateTime(updateDateId, updateTimeId).AddHours(plant.TimeOffset);
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
