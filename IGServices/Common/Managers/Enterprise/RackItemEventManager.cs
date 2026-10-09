using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using IBM.Data.DB2.iSeries;
using IGServices.Common.Types;
using IGServices.Common.Types.E1;
using IGServices.Common.Types.Enterprise;
using IGServices.Common.Utilities;

namespace IGServices.Common.Managers.Enterprise
{
    public class RackItemEventManager
    {
        private RackItemManager _rackItemManager;

        public RackItemEventManager(BusinessSystemSession e1Session)
        {
            var hash = new Dictionary<string, iDB2Command>();
            hash.Add("RackItemEventManager.GetRackItemEvents", GetRackItemEvents());
            foreach (string keyName in hash.Keys)
            {
                if (!e1Session.DbCommands.ContainsKey(keyName))
                {
                    e1Session.DbCommands.Add(keyName, hash[keyName]);
                }
            }
            _rackItemManager = new RackItemManager(e1Session);
        }

        /******************************************************************
        * Get
        ******************************************************************/
        private string rackItemEventSelectClause = @"
        DLUKID ,
        DLMCU ,
        DLZ58RID ,
        DLZ58SN ,
        DLLOTN ,
        DLITM ,
        DLZ58DST ,
        DLDOCO ,
        DLDCTO ,
        DLLNID ,
        DLDOC1 ,
        DLXDCTO ,
        DLDOC2 ,
        DLXCTO ,
        DLLNIC ,
        DLPSN ,
        DLUPMJ ,
        DLUPMT";

        public iDB2Command GetRackItemEvents()
        {
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = String.Format(@"
            SELECT 
                {0}            
            FROM 
	            F580569
            WHERE 
                DLMCU = ? AND
                DLUKID >= ?
            ORDER BY DLUKID",
                rackItemEventSelectClause);
            cmd.Parameters.Add(new iDB2Parameter("PlantCode", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("EventId", iDB2DbType.iDB2Decimal));
            cmd.Parameters["EventId"].Precision = 15;
            cmd.Parameters["EventId"].Scale = 0;
            return cmd;
        }

        public RackItemEvents GetRackItemEvents(BusinessSystemSession e1Session, string plantCode, decimal ukid)
        {
            var rackItemEvents = new RackItemEvents();
            var plant = PlantManager.GetPlant(plantCode);
            try
            {
                var cmd = new iDB2Command();
                cmd = (e1Session.DbCommands.ContainsKey("RackItemEventManger.GetRackItemEvents")) ? e1Session.DbCommands["RackItemEventManger.GetRackItemEvents"] : GetRackItemEvents();
                cmd.Parameters[0].Value = plant.PlantNum.PadLeft(12);
                cmd.Parameters[1].Value = ukid;
                cmd.Connection = e1Session.DbConnection;
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
                while (reader.Read())
                {
                    var rackItemEvent = new RackItemEvent();
                    rackItemEvent.RackId = CastDbNull.To<Decimal>(reader["DLZ58RID"], 0);
                    rackItemEvent.SerialNum = CastDbNull.To<String>(reader["DLZ58SN"], String.Empty).Trim();
                    rackItemEvent.EventId = CastDbNull.To<Decimal>(reader["DLUKID"], 0);
                    var eventDateId = CastDbNull.To<Decimal>(reader["DLUPMJ"], 0);
                    var eventTimeId = CastDbNull.To<Decimal>(reader["DLTDAY"], 0);
                    rackItemEvent.EventDate = Utilities.Common.CastToDateTime(eventDateId, eventTimeId);
                    rackItemEvent.LotNum = CastDbNull.To<String>(reader["DLLOTN"], String.Empty).Trim();
                    rackItemEvent.ItemNum = CastDbNull.To<Decimal>(reader["DLITM"], 0);
                    rackItemEvent.Status = CastDbNull.To<String>(reader["DLZ58DST"], String.Empty).Trim();
                    rackItemEvent.Quantity = CastDbNull.To<Decimal>(reader["DLPQOH"], 0) / 1000;

                    rackItemEvent.SOrderNum = CastDbNull.To<Decimal>(reader["DLDOCO"], 0);
                    rackItemEvent.SOrderType = CastDbNull.To<String>(reader["DLDCTO"], String.Empty).Trim();
                    rackItemEvent.SOrderLineNum = CastDbNull.To<Decimal>(reader["DLLNID"], 0) / 1000;
                    rackItemEvent.WOrderNum = CastDbNull.To<Decimal>(reader["DLDOC1"], 0);
                    rackItemEvent.POrderNum = CastDbNull.To<Decimal>(reader["DLDOC2"], 0);
                    rackItemEvent.POrderType = CastDbNull.To<String>(reader["DLXCTO"], String.Empty).Trim();
                    rackItemEvent.POrderLineNum = CastDbNull.To<Decimal>(reader["DLLNIC"], 0) / 1000;

                    rackItemEvent.LoadNum = CastDbNull.To<Decimal>(reader["DLPSN"], 0);
                   
                    rackItemEvents.Add(rackItemEvent);
                }
                reader.Close();
            }
            catch (Exception err)
            {
                throw new ApplicationException(err.ToString());
            }
            return rackItemEvents;
        }
    }
}
