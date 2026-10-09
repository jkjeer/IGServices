using System;
using System.Collections.Generic;
using System.Data;
using IBM.Data.DB2.iSeries;
using IGServices.Common.Types.E1;
using IGServices.Common.Types.Enterprise;
using IGServices.Common.Utilities;

namespace IGServices.Common.Managers.Enterprise
{
    public class RackItemManager
    {
        private GlassItemManager _glassItemMgr;
        private CustomerManager _customerMgr;

        public RackItemManager(BusinessSystemSession e1Session)
        {
            var hash = new Dictionary<string, iDB2Command>();
            hash.Add("RackItemManager.GetRackItem", GetRackItem());
            hash.Add("RackItemManager.GetRackItems", GetRackItems());
            foreach (string keyName in hash.Keys)
            {
                if (!e1Session.DbCommands.ContainsKey(keyName))
                {
                    e1Session.DbCommands.Add(keyName, hash[keyName]);
                }
            }
            _glassItemMgr = new GlassItemManager(e1Session);
            _customerMgr = new CustomerManager(e1Session);
        }

        /******************************************************************
        * Get
        ******************************************************************/
        private string rackItemSelectClause =
          @"RDMCU,
            RDZ58RID,
            RDMCU,
            RDKCOO,
            RDZ58SD,
            RDZ58PS,
            RDZ58STK,
            RDZ58ROW,
            RDZ58SL,
            RDITM,
            RDLOTN,
            RDZ58DST,
            RDTRDJ,
            RDZ58MDT,
            RDZ58PDT,
            RDPQOH,
            RDUOM1,
            RDZ58GWT,
            RDUWUM, 
            RDZ58SN,
            RDUPMJ,
            RDTDAY,
            RDDOCO,
            RDDCTO,
            RDLNID,
            RDDOC1,
            RDDOC2,
            RDXCTO,
            RDLNIC,
            RDPKCO,
            RDPID ";

        public iDB2Command GetRackItems()
        {
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = String.Format(
                @"SELECT 
                        {0}            
                    FROM 
	                    F580056
                    WHERE 
                        RDZ58RID = ?",
                rackItemSelectClause);
            cmd.Parameters.Add(new iDB2Parameter("RackId", iDB2DbType.iDB2Decimal));
            cmd.Parameters["RackId"].Precision = 15;
            cmd.Parameters["RackId"].Scale = 0;
            return cmd;
        }

        public iDB2Command GetRackItem()
        {
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = String.Format(
                @"SELECT 
                        {0}            
                    FROM 
	                    F580056
                    WHERE 
                        RDZ58RID = ? AND
                        RDZ58SN = ?",
                rackItemSelectClause);
            cmd.Parameters.Add(new iDB2Parameter("RackId", iDB2DbType.iDB2Decimal));
            cmd.Parameters["RackId"].Precision = 15;
            cmd.Parameters["RackId"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("SerialNum", iDB2DbType.iDB2Graphic, 20));
            return cmd;
        }

        public RackItems GetRackItems(BusinessSystemSession e1Session, Decimal rackId, string serialNum = null, bool attributes = true)
        {
            var rackItems = new RackItems();
            try
            {

                var cmd = new iDB2Command();
                if (String.IsNullOrEmpty(serialNum))
                {
                    cmd = (e1Session.DbCommands.ContainsKey("RackManger.GetRackItems")) ? e1Session.DbCommands["RackManger.GetRackItems"] : GetRackItems();
                    cmd.Parameters[0].Value = rackId;
                }
                else
                {
                    cmd = (e1Session.DbCommands.ContainsKey("RackManger.GetRackItem")) ? e1Session.DbCommands["RackManger.GetRackItem"] : GetRackItem();
                    cmd.Parameters[0].Value = rackId;
                    cmd.Parameters[1].Value = serialNum;
                }
                cmd.Connection = e1Session.DbConnection;

                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
                while (reader.Read())
                {
                    var rackItem = new RackItem();
                    rackItem.RackId = CastDbNull.To<Decimal>(reader["RDZ58RID"], 0);
                    rackItem.SerialNum = CastDbNull.To(reader["RDZ58SN"], String.Empty).Trim();
                    rackItem.ItemNum = CastDbNull.To<Decimal>(reader["RDITM"], 0);
                    rackItem.PlantNum = CastDbNull.To(reader["RDMCU"], String.Empty).Trim();
                    rackItem.Side = CastDbNull.To(reader["RDZ58SD"], String.Empty).Trim();
                    rackItem.Position = CastDbNull.To<Decimal>(reader["RDZ58PS"], 0);
                    rackItem.StackTier = CastDbNull.To<Decimal>(reader["RDZ58STK"], 0);
                    rackItem.Row = CastDbNull.To<Decimal>(reader["RDZ58ROW"], 0);
                    rackItem.Slot = CastDbNull.To<Decimal>(reader["RDZ58SL"], 0);
                    rackItem.LotNum = CastDbNull.To(reader["RDLOTN"], String.Empty).Trim();
                    rackItem.DetailStatus = CastDbNull.To(reader["RDZ58DST"], String.Empty).Trim();
                    var transactionDateId = CastDbNull.To<Decimal>(reader["RDTRDJ"], 0);
                    rackItem.TransactionDate = Utilities.Common.CastToDateTime(transactionDateId, 0);
                    var manufacturedDateId = CastDbNull.To<Decimal>(reader["RDZ58MDT"], 0);
                    rackItem.ManufacturedDate = Utilities.Common.CastToDateTime(manufacturedDateId, 0);
                    //rackItem.ManufacturedPlant = CastDbNull.To<String>(reader["WAMCU"], String.Empty).Trim();
                    var productionDateId = CastDbNull.To<Decimal>(reader["RDZ58PDT"], 0);
                    rackItem.ProductionDate = Utilities.Common.CastToDateTime(productionDateId, 0);
                    var updateDateId = CastDbNull.To<Decimal>(reader["RDUPMJ"], 0);
                    var updateTimeId = CastDbNull.To<Decimal>(reader["RDTDAY"], 0);
                    rackItem.UpdateDateTime = Utilities.Common.CastToDateTime(updateDateId, updateTimeId);
                    rackItem.Quantity = CastDbNull.To<Decimal>(reader["RDPQOH"], 0) / 1000;
                    rackItem.QuantityUom = CastDbNull.To(reader["RDUOM1"], String.Empty).Trim();
                    rackItem.GlassWeight = CastDbNull.To<Decimal>(reader["RDZ58GWT"], 0) / 100;
                    rackItem.GlassWeightUom = CastDbNull.To(reader["RDUWUM"], String.Empty).Trim();
                    rackItem.ProcessId = CastDbNull.To(reader["RDPID"], String.Empty).Trim();

                    if (attributes)
                    {
                        var plantCode = PlantManager.GetPlant(rackItem.PlantNum).PlantCode;
                        //var stockingType = CastDbNull.To<String>(reader["IBSTKT"], String.Empty).Trim();
                        var rddoco = CastDbNull.To<Decimal>(reader["RDDOCO"], 0);
                        var rddcto = CastDbNull.To(reader["RDDCTO"], String.Empty).Trim();
                        var rdlnid = CastDbNull.To<Decimal>(reader["RDLNID"], 0);
                        var rdkcoo = CastDbNull.To(reader["RDKCOO"], String.Empty).Trim();
                        var rddoc1 = CastDbNull.To<Decimal>(reader["RDDOC1"], 0);
                        var rddoc2 = CastDbNull.To<Decimal>(reader["RDDOC2"], 0);
                        var rdxcto = CastDbNull.To(reader["RDXCTO"], String.Empty).Trim();
                        var rdlnic = CastDbNull.To<Decimal>(reader["RDLNIC"], 0);
                        var rdpkco = CastDbNull.To(reader["RDPKCO"], String.Empty).Trim();
                        if (rackItem.LotNum == "")
                        {
                            rackItem.GlassItem = _glassItemMgr.GetGlassItem(e1Session, rackItem.ItemNum, plantCode);
                        }
                        else
                        {
                            if (rddoc2 > 0)
                            {
                                rackItem.GlassItem = _glassItemMgr.GetGlassItem(e1Session, rackItem.ItemNum, rackItem.PlantNum, rddoc2, rdxcto, rdlnic, rdpkco);
                            }
                            else
                            {
                                if (rddoco > 0)
                                {
                                    rackItem.GlassItem = _glassItemMgr.GetGlassItem(e1Session, rackItem.ItemNum, rackItem.PlantNum, rddoco, rddcto, rdlnid, rdkcoo);
                                }
                                else
                                {
                                    rackItem.GlassItem = _glassItemMgr.GetGlassItem(e1Session, rackItem.ItemNum, rackItem.PlantNum, rddoc1, "WO", 0, "*");
                                }
                            }
                        }
                        try
                        {
                            rackItem.HasGlassAttributes = (rackItem.GlassItem.ItemAttributes.Count > 0);
                        }
                        catch (Exception)
                        {
                            rackItem.HasGlassAttributes = false;
                        }
                    }
                    //AddRackItemReceiving(e1Session, rackItem);
                    rackItems.Add(rackItem);
                }
                reader.Close();
            }
            catch (Exception err)
            {
                throw new ApplicationException(err.ToString());
            }
            return rackItems;
        }
    }
}
