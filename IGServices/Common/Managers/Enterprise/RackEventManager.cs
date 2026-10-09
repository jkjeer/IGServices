using System;
using System.Collections.Generic;
using System.Data;
using IBM.Data.DB2.iSeries;
using IGServices.Common.Types.E1;
using IGServices.Common.Types.Enterprise;
using IGServices.Common.Utilities;

namespace IGServices.Common.Managers.Enterprise
{
    public class RackEventManager
    {

        public RackEventManager(BusinessSystemSession e1Session)
        {
            var hash = new Dictionary<string, iDB2Command>();
            hash.Add("RackEventManager.GetRackEvents", GetRackEvents());
            foreach (string keyName in hash.Keys)
            {
                if (!e1Session.DbCommands.ContainsKey(keyName))
                {
                    e1Session.DbCommands.Add(keyName, hash[keyName]);
                }
            }
        }

        /******************************************************************
        * Get
        ******************************************************************/
        private string rackEventSelectClause = @"    
        HLUKID ,
        HLMCU ,
        HLZ58RID ,
        HLZ58HST ,
        HLZ58RT ,
        HLLOCN ,
        HLZ58RCD ,
        HLAN8 ,
        HLSHAN,
        HLZ58SCN ,
        HLZ58BOX ,
        HLZ58GWTT ,
        HLZ58RTWT ,
	    HLPSN ,
        HLUPMJ ,
        HLUPMT";

        public iDB2Command GetRackEvents()
        {
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = string.Format(@"
            SELECT 
                {0}            
            FROM 
	            F580559
            WHERE 
                HLMCU = ? AND
                HLUKID >= ?
            ORDER BY HLUKID",
                rackEventSelectClause);
            cmd.Parameters.Add(new iDB2Parameter("PlantCode", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("EventId", iDB2DbType.iDB2Decimal));
            cmd.Parameters["EventId"].Precision = 15;
            cmd.Parameters["EventId"].Scale = 0;
            return cmd;
        }

        public RackEvents GetRackEvents(BusinessSystemSession e1Session, string plantCode, decimal ukid)
        {
            var rackEvents = new RackEvents();
            var plant = PlantManager.GetPlant(plantCode);
            try
            {
                var cmd = (e1Session.DbCommands.ContainsKey("RackEventManger.GetRackEvents")) ? e1Session.DbCommands["RackEventManger.GetRackEvents"] : GetRackEvents();
                cmd.Parameters[0].Value = plant.PlantNum.PadLeft(12);
                cmd.Parameters[1].Value = ukid;
                cmd.Connection = e1Session.DbConnection;
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
                while (reader.Read())
                {
                    var rackEvent = new RackEvent();
                    rackEvent.RackId = CastDbNull.To<decimal>(reader["HLZ58RID"], 0);
                    rackEvent.EventId = CastDbNull.To<decimal>(reader["HLUKID"], 0);
                    var eventDateId = CastDbNull.To<decimal>(reader["HLUPMJ"], 0);
                    var eventTimeId = CastDbNull.To<decimal>(reader["HLUPMT"], 0);
                    rackEvent.EventDate = Utilities.Common.CastToDateTime(eventDateId, eventTimeId);
                    rackEvent.Status = CastDbNull.To(reader["HLZ58HST"], string.Empty).Trim();
                    rackEvent.Location = CastDbNull.To(reader["HLLOCN"], string.Empty).Trim();
                    rackEvent.RackType = CastDbNull.To(reader["HLZ58RT"], string.Empty).Trim();
                    rackEvent.ReasonCode = CastDbNull.To(reader["HLZ58RCD"], string.Empty).Trim();
                    rackEvent.ScheduleNum = CastDbNull.To<decimal>(reader["HLZ58SCN"], 0);
                    rackEvent.BoxNum = CastDbNull.To<decimal>(reader["HLZ58BOX"], 0);
                    rackEvent.CustomerNum = CastDbNull.To<decimal>(reader["HLAN8"], 0);
                    rackEvent.CustomerShipNum = CastDbNull.To<decimal>(reader["HLSHAN"], 0);
                    rackEvent.LoadNum = CastDbNull.To<decimal>(reader["HLPSN"], 0);
                    rackEvent.GlassLbs = CastDbNull.To<decimal>(reader["HLZ58GWTT"], 0);
                    rackEvent.RackLbs = CastDbNull.To<decimal>(reader["HLZ58RTWT"], 0);
                    rackEvents.Add(rackEvent);
                }
                reader.Close();
            }
            catch (Exception err)
            {
                throw new ApplicationException(err.ToString());
            }
            return rackEvents;
        }
    }
}
