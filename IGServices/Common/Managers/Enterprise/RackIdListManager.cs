using System;
using System.Collections.Generic;
using System.Data;
using IBM.Data.DB2.iSeries;
using IGServices.Common.Types;
using IGServices.Common.Types.E1;
using IGServices.Common.Types.Enterprise;
using IGServices.Common.Utilities;

namespace IGServices.Common.Managers.Enterprise
{
    public class RackIdListManager
    {

        public RackIdListManager(BusinessSystemSession e1Session)
        {
            var hash = new Dictionary<string, iDB2Command>();
            foreach (string keyName in hash.Keys)
            {
                if (!e1Session.DbCommands.ContainsKey(keyName))
                {
                    e1Session.DbCommands.Add(keyName, hash[keyName]);
                }
            }
        }

        /****************************************************************
         * List By Status
         ***************************************************************/
        public iDB2Command GetRackIdListByStatus()
        {
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText =
                @"SELECT
                        RHZ58RID
                    FROM
                        F580055 
                         INNER JOIN ( SELECT
                                        S1MCU ,
                                        S1Z58SCN ,
                                        MAX(S1Z58SCD) AS S1Z58SCD
                                       FROM
                                        F5834A01
                                       GROUP BY
                                        S1MCU ,
                                        S1Z58SCN
                                     ) 
                          ON S1MCU = S5MCU AND
                             S1Z58SCN = S5Z58SCN AND
                             S1Z58SCD = S5Z58SCD
                    WHERE
                        RHMCU = ? AND
                        RHZ58HST = ? AND
                        RHZ58RSDT >= ? AND
                        RHZ58SCN < 1000
                    ORDER BY
                        RHZ58RID";
            cmd.Parameters.Add(new iDB2Parameter("PlantNum", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("RackStatus", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("RackStatusDateId", iDB2DbType.iDB2Decimal));
            cmd.Parameters["RackStatusDateId"].Precision = 6;
            cmd.Parameters["RackStatusDateId"].Scale = 0;
            return cmd;
        }

        public List<decimal> GetRackIdListByStatus(BusinessSystemSession e1Session, Plant plant, String statusCode, TimeSpan timeSpan)
        {
            var ets = new E1TimeSpan();
            if (timeSpan.TotalDays > 0)
            {
                ets.SetTimeSpan(timeSpan);
            }

            var rackIds = new List<Decimal>();
            try
            {
                var cmd = GetRackIdListByStatus();
                cmd.Connection = e1Session.DbConnection;
                cmd.Parameters[0].Value = plant.PlantNum.PadLeft(12);
                cmd.Parameters[1].Value = statusCode;
                cmd.Parameters[2].Value = ets.DateComponent1;
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
                while (reader.Read())
                {
                    rackIds.Add(CastDbNull.To<Decimal>(reader["RHZ58RID"], 0));
                }
                reader.Close();
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.Message);
            }
            return rackIds;
        }

        /****************************************************************
         * Locked Racks
         ***************************************************************/
        public iDB2Command GetLockedRacks()
        {
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText =
                @"SELECT
                        RHZ58RID
                    FROM
                        F580055 
                    WHERE
                        RHMCU = ? AND
                        RHZ58HST IN ('5','7','8') AND
                        RHZ58SCN < 1000 AND
                        (
                            (RHUPMJ = ? AND RHTDAY >= ?) OR 
                            (RHUPMJ >= ? AND RHUPMJ <= ?)
                        )
                    ORDER BY
                        RHZ58RID";
            cmd.Parameters.Add(new iDB2Parameter("PlantNum", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("Date1", iDB2DbType.iDB2Decimal));
            cmd.Parameters["Date1"].Precision = 6;
            cmd.Parameters["Date1"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("Time1", iDB2DbType.iDB2Decimal));
            cmd.Parameters["Time1"].Precision = 6;
            cmd.Parameters["Time1"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("Date2", iDB2DbType.iDB2Decimal));
            cmd.Parameters["Date2"].Precision = 6;
            cmd.Parameters["Date2"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("Date3", iDB2DbType.iDB2Decimal));
            cmd.Parameters["Date3"].Precision = 6;
            cmd.Parameters["Date3"].Scale = 0;
            return cmd;
        }

        public List<Decimal> GetLockedRacks(BusinessSystemSession e1Session, Plant plant, decimal minutes)
        {
            var dtParms = new E1DateParms();
            dtParms.SetParms(DateTime.Now.AddMinutes((double)minutes*-1));
            var rackIds = new List<Decimal>();
            try
            {
                var cmd = GetRackIdListByStatus();
                cmd.Connection = e1Session.DbConnection;
                cmd.Parameters[0].Value = plant.PlantNum.PadLeft(12);
                cmd.Parameters[1].Value = dtParms.DateComponent1;
                cmd.Parameters[2].Value = dtParms.TimeComponent1;
                cmd.Parameters[3].Value = dtParms.DateComponent2;
                cmd.Parameters[4].Value = dtParms.DateComponent3;
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
                while (reader.Read())
                {
                    rackIds.Add(CastDbNull.To<Decimal>(reader["RHZ58RID"], 0));
                }
                reader.Close();
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.Message);
            }
            return rackIds;
        }

        /****************************************************************
         * List By MTOSchedule
         ***************************************************************/
        public iDB2Command GetRackIdListBySchedule()
        {
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = @"
                    SELECT 
	                    RHZ58RID 
                    FROM  
	                    F580055  
	                    INNER JOIN F580056
	                    ON	RHZ58RID = RDZ58RID  
	                    INNER JOIN F5834A11 
	                    ON	RDDOC1 = S5DOCO AND
                            RDMCU = S5MCU
	                    INNER JOIN ( SELECT
				                    S1MCU ,
				                    S1Z58SCN ,
				                    MAX(S1Z58SCD) AS S1Z58SCD
				                    FROM
				                    F5834A01
				                    GROUP BY
				                    S1MCU ,
				                    S1Z58SCN
                                ) S1
	                    ON	S1.S1MCU = S5MCU AND
		                    S1.S1Z58SCN = S5Z58SCN AND
		                    S1.S1Z58SCD = S5Z58SCD
                    WHERE
                        RHMCU = ? AND
                        RHZ58SCN = ? AND
                        S5Z58SCN = ? 
                    GROUP BY
                        RHZ58RID
                    UNION
                    SELECT 
                          RHZ58RID 
                    FROM  
                          F580055  
                          INNER JOIN F580056
                          ON    RHZ58RID = RDZ58RID  
                    WHERE
                        RHMCU = ? AND
                        RHZ58SCN = ? AND
                        RHZ58SCN < 100 AND
                        RHZ58HST NOT IN ('0','8')                    
                    GROUP BY
                        RHZ58RID
                ";
            cmd.Parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("SCN1", iDB2DbType.iDB2Decimal));
            cmd.Parameters["SCN1"].Precision = 5;
            cmd.Parameters["SCN1"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("SCN2", iDB2DbType.iDB2Decimal));
            cmd.Parameters["SCN2"].Precision = 5;
            cmd.Parameters["SCN2"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("MCU3", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("SCN3", iDB2DbType.iDB2Decimal));
            cmd.Parameters["SCN1"].Precision = 5;
            cmd.Parameters["SCN1"].Scale = 0;
            return cmd;
        }

        public iDB2Command GetRackIdListByScheduleOLD()
        {
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = @"
                    SELECT 
	                    RHZ58RID 
                    FROM  
	                    F580055  
	                    INNER JOIN F580056
	                    ON	RHZ58RID = RDZ58RID  
	                    INNER JOIN F5834A11 
	                    ON	RDDOC1 = S5DOCO AND
                            RDMCU = S5MCU
	                    INNER JOIN ( SELECT
				                    S1MCU ,
				                    S1Z58SCN ,
				                    MAX(S1Z58SCD) AS S1Z58SCD
				                    FROM
				                    F5834A01
				                    GROUP BY
				                    S1MCU ,
				                    S1Z58SCN
                                ) S1
	                    ON	S1.S1MCU = S5MCU AND
		                    S1.S1Z58SCN = S5Z58SCN AND
		                    S1.S1Z58SCD = S5Z58SCD
                    WHERE
                        RHMCU = ? AND
                        RHZ58SCN = ? AND
                        S5Z58SCN = ? AND
                        RHZ58SCN >= 200
                    GROUP BY
                        RHZ58RID
                    UNION
                    SELECT 
                          RHZ58RID 
                    FROM  
                          F580055  
                          INNER JOIN F580056
                          ON    RHZ58RID = RDZ58RID  
                    WHERE
                        RHMCU = ? AND
                        RHZ58SCN = ? AND
                        RHZ58SCN < 200 AND
                        RHZ58HST NOT IN ('0','8')                    
                    GROUP BY
                        RHZ58RID
                ";
            cmd.Parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("SCN1", iDB2DbType.iDB2Decimal));
            cmd.Parameters["SCN1"].Precision = 5;
            cmd.Parameters["SCN1"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("SCN2", iDB2DbType.iDB2Decimal));
            cmd.Parameters["SCN2"].Precision = 5;
            cmd.Parameters["SCN2"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("MCU3", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("SCN3", iDB2DbType.iDB2Decimal));
            cmd.Parameters["SCN1"].Precision = 5;
            cmd.Parameters["SCN1"].Scale = 0;
            return cmd;
        }

        public List<Decimal> GetRackIdListBySchedule(BusinessSystemSession e1Session, Plant plant, Decimal scheduleNum)
        {
            var rackIds = new List<Decimal>();
            try
            {
                var cmd = GetRackIdListBySchedule();
                cmd.Connection = e1Session.DbConnection;
                cmd.Parameters[0].Value = plant.PlantNum.PadLeft(12);
                cmd.Parameters[1].Value = scheduleNum;
                cmd.Parameters[2].Value = scheduleNum;
                cmd.Parameters[3].Value = plant.PlantNum.PadLeft(12);
                cmd.Parameters[4].Value = scheduleNum;
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
                while (reader.Read())
                {
                    rackIds.Add(CastDbNull.To<Decimal>(reader["RHZ58RID"], 0));
                }
                reader.Close();
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.Message);
            }
            return rackIds;
        }

        /****************************************************************
         * List By MTOSchedule
         ***************************************************************/
        public iDB2Command GetRackIdListByLoad()
        {
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText =
                @"SELECT 
	                    RHZ58RID 
                    FROM  
	                    F580055  
                    WHERE
                        RHMCU = ? AND
                        RHPSN = ?
                    GROUP BY
                        RHZ58RID";
            cmd.Parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("PSN", iDB2DbType.iDB2Decimal));
            cmd.Parameters["PSN"].Precision = 8;
            cmd.Parameters["PSN"].Scale = 0;
            return cmd;
        }

        public List<Decimal> GetRackIdListByLoad(BusinessSystemSession e1Session, Plant plant, Decimal loadNum)
        {
            var rackIds = new List<Decimal>();
            try
            {
                var cmd = GetRackIdListByLoad();
                cmd.Connection = e1Session.DbConnection;
                cmd.Parameters[0].Value = plant.PlantNum.PadLeft(12);
                cmd.Parameters[1].Value = loadNum;
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
                while (reader.Read())
                {
                    rackIds.Add(CastDbNull.To<Decimal>(reader["RHZ58RID"], 0));
                }
                reader.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return rackIds;
        }

        /****************************************************************
     * List By MTOSchedule
     ***************************************************************/
        public iDB2Command GetRackListBySchedule()
        {
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText =
                @"SELECT 
	                    RHZ58RID 
						,RHZ58SCN
						,RHZ58BOX
                    FROM  
	                    F580055  
	                    INNER JOIN F580056
	                    ON	RHZ58RID = RDZ58RID  
	                    INNER JOIN F5834A11 
	                    ON	RDDOC1 = S5DOCO AND
                            RDMCU = S5MCU
	                    INNER JOIN ( SELECT
				                    S1MCU ,
				                    S1Z58SCN ,
				                    MAX(S1Z58SCD) AS S1Z58SCD
				                    FROM
				                    F5834A01
				                    GROUP BY
				                    S1MCU ,
				                    S1Z58SCN
                                ) S1
	                    ON	S1.S1MCU = S5MCU AND
		                    S1.S1Z58SCN = S5Z58SCN AND
		                    S1.S1Z58SCD = S5Z58SCD
                    WHERE
                        RHMCU = ? AND
                        RHZ58SCN = ? AND
                        S5Z58SCN = ? AND
                        RHZ58SCN >= 200
                    GROUP BY
                        RHZ58RID
						,RHZ58SCN
						,RHZ58BOX
                    UNION
                    SELECT 
                          RHZ58RID 
						,RHZ58SCN
						,RHZ58BOX
                    FROM  
                          F580055  
                          INNER JOIN F580056
                          ON    RHZ58RID = RDZ58RID  
                    WHERE
                        RHMCU = ? AND
                        RHZ58SCN = ? AND
                          RHZ58SCN < 200 AND
                        RHZ58HST NOT IN ('0','8')                    
                    GROUP BY
                        RHZ58RID
						,RHZ58SCN
						,RHZ58BOX";
            cmd.Parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("SCN1", iDB2DbType.iDB2Decimal));
            cmd.Parameters["SCN1"].Precision = 5;
            cmd.Parameters["SCN1"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("SCN2", iDB2DbType.iDB2Decimal));
            cmd.Parameters["SCN2"].Precision = 5;
            cmd.Parameters["SCN2"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("MCU3", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("SCN3", iDB2DbType.iDB2Decimal));
            cmd.Parameters["SCN1"].Precision = 5;
            cmd.Parameters["SCN1"].Scale = 0;
            return cmd;
        }

        public Racks GetRackListBySchedule(BusinessSystemSession e1Session, Plant plant, Decimal scheduleNum)
        {
            var racks = new Racks();
            try
            {
                var cmd = GetRackListBySchedule();
                cmd.Connection = e1Session.DbConnection;
                cmd.Parameters[0].Value = plant.PlantNum.PadLeft(12);
                cmd.Parameters[1].Value = scheduleNum;
                cmd.Parameters[2].Value = scheduleNum;
                cmd.Parameters[3].Value = plant.PlantNum.PadLeft(12);
                cmd.Parameters[4].Value = scheduleNum;
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
                while (reader.Read())
                {
                    var rack = new Rack();
                    rack.RackId = CastDbNull.To<Decimal>(reader["RHZ58RID"], 0);
                    rack.ScheduleNum = CastDbNull.To<Decimal>(reader["RHZ58SCN"], 0);
                    rack.BoxNum = CastDbNull.To<Decimal>(reader["RHZ58BOX"], 0);
                    racks.Add(rack);
                }
                reader.Close();
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.Message);
            }
            return racks;
        }

    }
}
