using Microsoft.Data.SqlClient;
using IGServices.Common.Types.Response;
using IGServices.Common.Types.ShopFloor;
using IGServices.Common.Utilities;

namespace IGServices.Common.Managers.ShopFloor
{
    class RackItemManager
    {
        public Types.ShopFloor.RackItems GetRacks(Types.Enterprise.Racks entRacks)
        {
            var sfRackItems = new RackItems();
            foreach (var entRack in entRacks)
            {
                foreach (var entRackItem in entRack.RackItems)
                {
                    var sfRackItem = new RackItem();
                    sfRackItem.RackId = entRackItem.RackId;
                    sfRackItem.SerialNum = entRackItem.SerialNum;
                    sfRackItems.Add(sfRackItem);
                }
            }
            return sfRackItems;
        }

        public SqlBulkCopyResponse DownloadRackItemEvents(SqlConnection cn, RackItemEvents events)
        {
            var bcMgr = new SqlBulkCopyManager();
            bcMgr.DbConnectionString = cn.ConnectionString;
            return bcMgr.BulkReader("RackItemEvent", events.GetDataReader());
        }
    }
}
