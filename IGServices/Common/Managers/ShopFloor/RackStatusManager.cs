using System;
using System.Data;
using IGServices.Common.Managers.Enterprise;
using IGServices.Common.Types.E1;
using IGServices.Common.Types.Response;
using IGServices.Common.Types.ShopFloor;
using IGServices.Common.Utilities;

namespace IGServices.Common.Managers.ShopFloor
{
    public class RackStatusManager
    {

        public RackStatusManager(BusinessSystemSession e1Session)
        {
        }


        public RackLedgerItems GetRackStatusItems(BusinessSystemSession e1Session, Decimal rackId, string serialNum, string plantCode, string glCategory, double days, bool attributes)
        {
            var rackStatusItems = new RackLedgerItems();
            try
            {
                var rackMgr = new Managers.Enterprise.RackManager(e1Session);
                var racks = rackMgr.GetRacks(e1Session, rackId, serialNum, plantCode, attributes);
                foreach (var rack in racks)
                {
                    if (rack.RackItems != null)
                    {
                        foreach (var rackItem in rack.RackItems)
                        {
                            var tag = new RackLedgerItem();
                            tag.PlantNum = rack.CostCenter;
                            tag.RackId = rack.RackId;
                            tag.SerialNum = rackItem.SerialNum;
                            tag.QtyOnHand = rackItem.Quantity;
                            tag.Status = rack.HeaderStatus;
                            tag.StatusDate = rack.HeaderStatusDate;
                            tag.Location = rack.Location;
                            tag.UpdateDateTime = rackItem.UpdateDateTime;
                            tag.ReasonCode = rack.ReasonCode;
                            tag.ProcessId = rackItem.ProcessId;
                            rackStatusItems.Add(tag);
                        }
                    }
                }
            }
            catch (Exception err)
            {
                throw new ApplicationException(err.ToString());
            }
            return rackStatusItems;
        }

        public SqlBulkCopyResponse PersistRackStatus(BusinessSystemSession e1Session, string plantCode, RackLedgerItems tags)
        {
            var response = new SqlBulkCopyResponse();
            if (tags.Count > 0)
            {
                var plantNum1 = tags[0].PlantNum;
                try
                {
                    var dt = GetDataTable("E1RackStatusLedger");
                    AddRows(dt, tags);
                    //var plantNum = Utilities.GetPlantNum(plantCode);
                    var bc = new SqlBulkCopyManager();
                    var cn = ConnectionManager.GetSqlConnectionFromKey(plantNum1 + ".INTEGRATION");
                    bc.DbConnectionString = cn.ConnectionString;
                    response = bc.BulkTable("E1RackStatusLedger", dt);
                }
                catch (Exception e)
                {
                    throw new ApplicationException(e.Message);
                }
            }
            return response;
        }

        public DataTable GetDataTable(String tableName)
        {
            var dt = new DataTable(tableName);
            //dt.Columns.Add("RackLedgerID", typeof(int));
            dt.Columns.Add("RackId", typeof(decimal));
            dt.Columns.Add("SerialNum", typeof(string));
            dt.Columns.Add("Location", typeof(string));
            dt.Columns.Add("Status", typeof(string));
            dt.Columns.Add("QtyOnHand", typeof(decimal));
            dt.Columns.Add("RackStatusDateTime", typeof(DateTime));
            dt.Columns.Add("ProcessId", typeof(string));
            dt.Columns.Add("ReasonCode", typeof(string));
            return dt;
        }

        public void AddRows(DataTable dt, RackLedgerItems tags)
        {
            foreach (var tag in tags)
            {
                var dr = dt.NewRow();
                dr["RackId"] = tag.RackId;
                dr["SerialNum"] = tag.SerialNum;
                dr["Location"] = tag.Location;
                dr["Status"] = tag.Status;
                dr["QtyOnHand"] = tag.QtyOnHand;
                dr["RackStatusDateTime"] = tag.UpdateDateTime;
                dr["ProcessId"] = tag.ProcessId;
                dr["ReasonCode"] = tag.ReasonCode;
                dt.Rows.Add(dr);
            }

        }
    }
}
