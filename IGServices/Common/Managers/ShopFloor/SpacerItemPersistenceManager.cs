using System;
using System.Collections.Generic;
using System.Data;
using IBM.Data.DB2.iSeries;
using IGServices.Common.Managers.Enterprise;
using IGServices.Common.Types.E1;
using IGServices.Common.Types.Enterprise;
using IGServices.Common.Types.Response;
using IGServices.Common.Utilities;

namespace IGServices.Common.Managers.ShopFloor
{
    public class SpacerItemPersistenceManager
    {

        public SpacerItemPersistenceManager(BusinessSystemSession e1Session)
        {
            var hash = new Dictionary<string, iDB2Command>();
            foreach (var keyName in hash.Keys)
            {
                if (!e1Session.DbCommands.ContainsKey(keyName))
                {
                    e1Session.DbCommands.Add(keyName, hash[keyName]);
                }
            }
        }

        public SqlBulkCopyResponse PersistSpacerItems(BusinessSystemSession e1Session, String plantCode, SpacerItems items)
        {
            SqlBulkCopyResponse response;
            try
            {
                var dt = GetDataTable(items);
                var bc = new SqlBulkCopyManager();
                var plantNum = PlantManager.GetPlant(plantCode).PlantNum;
                bc.DbConnectionString = ConnectionManager.GetSqlConnectionFromKey(plantNum).ConnectionString;
                //bc.DbConnectionString = ConnectionManager.GetSFConnectionString(plantCode, "ShopFloor");
                response = bc.BulkTable("E1SpacerItemsLedger", dt);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.Message);
            }
            return response;
        }

        public DataTable GetDataTable(SpacerItems items)
        {
            var dt = new DataTable("E1SpacerItemsLedger");
            dt.Columns.Add("ItemNum", typeof(decimal));
            dt.Columns.Add("Description", typeof(String));
            dt.Columns.Add("Construction", typeof(String));
            dt.Columns.Add("StockingType", typeof(String));
            dt.Columns.Add("PrimaryUOM", typeof(String));
            dt.Columns.Add("SpacerLength", typeof(decimal));
            dt.Columns.Add("CostPerLinealFoot", typeof(decimal));

            foreach (var item in items)
            {
                var dr = dt.NewRow();
                dr["ItemNum"] = item.ItemNum;
                dr["Description"] = item.Description;
                dr["Construction"] = item.Construction;
                dr["StockingType"] = item.StockingType;
                dr["PrimaryUOM"] = item.PrimaryUom;
                dr["SpacerLength"] = item.Length;
                dr["CostPerLinealFoot"] = item.CostPerLinealFoot;
                dt.Rows.Add(dr);
            }
            return dt;
        }
    }

}
