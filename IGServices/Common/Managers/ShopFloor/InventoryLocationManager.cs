using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using IGServices.Common.Managers.Response;
using IGServices.Common.Types.Enterprise;
using IGServices.Common.Types.Response;
using NLog;

namespace IGServices.Common.Managers.ShopFloor
{
    public class InventoryLocationManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /******************************************************************
        * GET ShopFloor Address Book Items
        ******************************************************************/
        public Types.ShopFloor.InventoryLocations GetShopFloorInventoryLocations(InventoryLocations locations)
        {
            var sfLocs = new Types.ShopFloor.InventoryLocations();
            sfLocs.AddRange(locations.Select(loc => new Types.ShopFloor.InventoryLocation
            {
                LocationCode = loc.LocationCode,
                Aisle = loc.Aisle,
                Bin = loc.Bin,
                CategoryCode = loc.Category03,
                DetailLevel = loc.DetailLevel,
                StagingFlag = loc.PutawayZone == "1" ? "Y" : "N",
                Category3 = loc.Category03,
                Category4 = loc.Category04,
                PutawayZone = loc.PutawayZone,
                UpdateDate = loc.UpdateDateTime
            }));
            return sfLocs;
        }

        /******************************************************************
        * Download 
        ******************************************************************/
        public Transfer DownloadInventoryLocations(InventoryLocations inventoryLocations, SqlConnection cnSFInt)
        {
            var transfer = new Transfer
            {
                TransferType = "Download"
            };
            var bcMgr = new SqlBulkCopyManager { DbConnectionString = cnSFInt.ConnectionString };
            var inventoryLocationsSF = GetShopFloorInventoryLocations(inventoryLocations);
            IDataReader dr = inventoryLocationsSF.GetDataReader();
            var sqlBulkCopyResponse = bcMgr.BulkReader("InventoryLocationLedger", dr);
            transfer.SqlBulkCopyResponses.Add(sqlBulkCopyResponse);
            transfer.Write = 1;
            transfer.Transferred = true;
            transfer.TransferStatus = "Y";
            transfer.TransferDate = DateTime.Now.ToLocalTime();
            if (sqlBulkCopyResponse.HasErrors)
            {
                Logger.Error("ERROR: " + cnSFInt.ConnectionString + "  " + sqlBulkCopyResponse.Message);
                throw new ApplicationException(sqlBulkCopyResponse.Message);
            }
            return transfer;
        }
    }
}
