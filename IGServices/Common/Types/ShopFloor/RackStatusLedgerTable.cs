using System;
using System.Data;
using System.Linq;
using System.ServiceModel;
using IGServices.Common.Utilities;

namespace IGServices.Common.Types.ShopFloor
{
    [System.ComponentModel.DesignerCategory("Code")]
    public class RackStatusLedgerTable : DataTable
    {

        public RackStatusLedgerTable()
        {
            ExtendedProperties.Add("Target", "[dbo].[E1RackStatusLedger]");
            Columns.Add("RackId", typeof(decimal));
            Columns.Add("SerialNum", typeof(string));
            Columns.Add("Location", typeof(string));
            Columns.Add("Status", typeof(string));
            Columns.Add("RackStatusDateTime", typeof(DateTime));
            Columns.Add("QtyOnHand", typeof(decimal));
            Columns.Add("ProcessId", typeof(string));
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
                            dr["RackId"] = rack.RackId;
                            dr["SerialNum"] = rackItem.SerialNum;
                            dr["Location"] = rack.Location;
                            dr["Status"] = rackItem.DetailStatus;
                            dr["RackStatusDateTime"] = rack.HeaderStatusDate;
                            dr["QtyOnHand"] = rackItem.Quantity;
                            dr["ProcessId"] = rackItem.ProcessId;
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
