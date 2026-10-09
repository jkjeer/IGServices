using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using IBM.Data.DB2.iSeries;
using IGServices.Common.Types.E1;
using IGServices.Common.Types.Enterprise;
using IGServices.Common.Utilities;
using NLog;

namespace IGServices.Common.Managers.E1
{
    public class SalesLineAttributeManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public SalesLineAttributeManager(BusinessSystemSession session)
        {
            var hash = new Dictionary<string, iDB2Command>
            {
                {"SalesLineAttributeManager.GetOriginalSalesLineAttribute", GetOriginalSalesLineAttribute()}
            };
            foreach (var keyName in hash.Keys.Where(keyName => !session.DbCommands.ContainsKey(keyName)))
            {
                session.DbCommands.Add(keyName, hash[keyName]);
            }
        }

        /********************************************************************************
        * Commands
        *******************************************************************************/
        public iDB2Command GetOriginalSalesLineAttribute()
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = $@"
                select 
	                SDMCU,
	                SDAN8,
	                SDKCOO,
	                SDDOCO,
	                SDDCTO,
	                SDLNID,
	                SDVR01,
	                ATKCOO,
	                ATDOCO,
	                ATDCTO,
	                ATLNID,
	                ROMCU2,
	                RODOC1,
	                RORCTO,
	                RORLLN
                from 
	                F580032 
	                INNER JOIN F580400
	                ON RORKCO = ATZ58POCO AND
		                RODOC1 = ATZ58PODC AND
		                RORCTO = ATZ58PODT AND
		                RORLLN = ATZ58POLI AND
		                ROEDTR = 'S'
	                INNER JOIN F4211
	                ON	SDKCOO = ROKCOO AND
		                SDDOCO = RODOCO AND
		                SDDCTO = RODCTO AND
		                SDLNID = ROLNID
                where 
	                ATDOCO = ? and 
	                ATDCTO = ? AND 
	                ATLNID = ? AND
                    ATKCOO = '00120'"
            };
            cmd.Parameters.Add(new iDB2Parameter("DOCO", iDB2DbType.iDB2Decimal));
            cmd.Parameters["DOCO"].Precision = 8;
            cmd.Parameters["DOCO"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("DCTO", iDB2DbType.iDB2Graphic, 2));
            cmd.Parameters.Add(new iDB2Parameter("LNID", iDB2DbType.iDB2Decimal));
            cmd.Parameters["LNID"].Precision = 6;
            cmd.Parameters["LNID"].Scale = 0;
            //cmd.Parameters.Add(new iDB2Parameter("KCOO", iDB2DbType.iDB2Graphic, 5));
            return cmd;
        }

        public SalesLineAttribute GetOriginalSalesLineAttribute(BusinessSystemSession session, decimal orderNum, string orderType, decimal orderLineNum)
        {
            Logger.Info($"BEGIN: {DateTime.Now.ToLocalTime()}");
            var saleLineAttribute = new SalesLineAttribute();
            try
            {
                var cmd = session.DbCommands["SalesLineAttributeManager.GetOriginalSalesLineAttribute"];
                cmd.Parameters["DOCO"].Value = orderNum;
                cmd.Parameters["DCTO"].Value = orderType;
                cmd.Parameters["LNID"].Value = orderLineNum;
                cmd.Connection = session.DbConnection;
                Logger.Debug(Logging.DbCmdString(cmd));
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
                while (reader.Read())
                {
                    saleLineAttribute.OrigCustomerPO = CastDbNull.To<string>(reader["SDVR01"], String.Empty).Trim();
                }
                reader.Close();
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + session.DbConnection.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
                throw new Exception(session.DbConnection.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }
            return saleLineAttribute;
        }

        public static iDB2Command GetOriginalSalesLineAttributes()
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = $@"
                SELECT
	                S7MCU,
	                S7EDBT,
	                S7DOCO,
	                S7RORN,
	                S7RCTO,
	                S7LNID,
	                ROMCU2,
	                RODOC1,
	                RORCTO,
	                RORLLN,
	                SD2.SDVR01
                FROM 
	                F580021
	                INNER JOIN F4211 SD1 
	                ON	SD1.SDKCOO = '00120' AND
						SD1.SDDOCO = S7RORN AND
						SD1.SDDCTO = S7RCTO AND
						SD1.SDLNID = S7LNID
	                INNER JOIN F580400
	                ON  RORKCO = SD1.SDRKCO AND
		                RODOC1 = SD1.SDRORN AND
		                RORCTO = SD1.SDRCTO AND
		                RORLLN = SD1.SDRLLN AND
		                ROEDTR = 'S'
	                INNER JOIN F4211 SD2
	                ON	SD2.SDKCOO = ROKCOO AND
		                SD2.SDDOCO = RODOCO AND
		                SD2.SDDCTO = RODCTO AND
		                SD2.SDLNID = ROLNID
                WHERE 
                    S7MCU = ? AND
	                S7EDBT = ?
                "
            };
            cmd.Parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("EDBT", iDB2DbType.iDB2Graphic, 20));
            return cmd;
        }

        public static SalesLineAttributes GetOriginalSalesLineAttributes(BusinessSystemSession session, Plant plant, string scheduleBatchNum)
        {
            var saleLineAttributes = new SalesLineAttributes();
            try
            {
                var cmd = GetOriginalSalesLineAttributes();
                cmd.Parameters["MCU"].Value = plant.PlantNum.PadLeft(12);
                cmd.Parameters["EDBT"].Value = scheduleBatchNum;
                cmd.Connection = session.DbConnection;
                Logger.Trace(Logging.DbCmdString(cmd));
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
                while (reader.Read())
                {
                    var att = new SalesLineAttribute();
                    att.WorkOrderNum = CastDbNull.To<decimal>(reader["S7DOCO"], 0);
                    att.OrigCustomerPO = CastDbNull.To<string>(reader["SDVR01"], String.Empty).Trim();
                    saleLineAttributes.Add(att);
                }
                reader.Close();
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + session.DbConnection.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
                throw new Exception(session.DbConnection.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }
            return saleLineAttributes;
        }

    }

}
