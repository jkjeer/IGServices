using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using IGServices.Common.Types.E1;
using IGServices.Common.Types.Enterprise;
using IGServices.Common.Types.Response;
using IGServices.Common.Utilities;
using IBM.Data.DB2.iSeries;
using NLog;

namespace IGServices.Common.Managers.Enterprise
{
    public class CrateManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly UdcItemManager _udcMgr;

        public CrateManager(BusinessSystemSession e1Session)
        {
            var hash = new Dictionary<string, iDB2Command>
            {
                {"CrateManager.GetCrate", GetCrate()},
                {"CrateManager.GetCrates", GetCrates()},
                {"CrateManager.GetCratesDesc", GetCratesDesc()}
            };
            foreach (var keyName in hash.Keys.Where(keyName => !e1Session.DbCommands.ContainsKey(keyName)))
            {
                e1Session.DbCommands.Add(keyName, hash[keyName]);
            }
            _udcMgr = new UdcItemManager(e1Session);
        }

        /******************************************************************
        * Commands
        ******************************************************************/
        public iDB2Command GetCrates()
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = @"
                    SELECT 
                        CRZ58CRID,
                        CRZ58CRTY,   
                        CRZ58CRQ,
                        CRZ58CRMSG                                  
                    FROM 
	                    F580220
                    WHERE 
                        CRDOCO = ? AND
                        CRDCTO = ? AND
                        CRKCOO = ? AND
                        CRLNID = ?"
            };
            cmd.Parameters.Add(new iDB2Parameter("OrderNum", iDB2DbType.iDB2Decimal));
            cmd.Parameters["OrderNum"].Precision = 8;
            cmd.Parameters["OrderNum"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("OrderType", iDB2DbType.iDB2Graphic, 2));
            cmd.Parameters.Add(new iDB2Parameter("CompanyNum", iDB2DbType.iDB2Graphic, 5));
            cmd.Parameters.Add(new iDB2Parameter("OrderLineNum", iDB2DbType.iDB2Decimal));
            cmd.Parameters["OrderLineNum"].Precision = 6;
            cmd.Parameters["OrderLineNum"].Scale = 0;
            return cmd;
        }

        public Crates GetCrates(BusinessSystemSession e1Session, decimal orderNum, string orderType, string companyNum, decimal orderLineNum)
        {
            var crates = new Crates();
            try
            {
                var cmd = (e1Session.DbCommands.ContainsKey("CrateManager.GetCrates")) ? e1Session.DbCommands["CrateManager.GetCrates"] : GetCrates();
                cmd.Parameters[0].Value = orderNum;
                cmd.Parameters[1].Value = orderType;
                cmd.Parameters[2].Value = companyNum;
                cmd.Parameters[3].Value = orderLineNum;
                cmd.Connection = e1Session.DbConnection;
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
                while (reader.Read())
                {
                    var crate = new Crate
                    {
                        CrateId = CastDbNull.To<decimal>(reader["CRZ58CRID"], 0),
                        CrateType = CastDbNull.To(reader["CRZ58CRTY"], string.Empty).Trim()
                    };
                    crate.Description = _udcMgr.GetUdcItem(e1Session, "58", "CT", crate.CrateType).Description1;
                    crate.Quantity = CastDbNull.To<decimal>(reader["CRZ58CRQ"], 0);
                    crate.Message = CastDbNull.To(reader["CRZ58CRMSG"], string.Empty).Trim();
                    crates.Add(crate);
                }
                reader.Close();
            }
            catch (Exception err)
            {
                throw new ApplicationException(err.ToString());
            }
            return crates;
        }

        /******************************************************************
        * Commands
        ******************************************************************/
        public iDB2Command GetCrate()
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = @"
                    SELECT 
                        CRZ58CRID,
                        CRZ58CRTY,   
                        CRZ58CRQ,
                        CRZ58CRMSG                          
                    FROM 
	                    F580220
                    WHERE
                        CRDOCO = ? AND
                        CRDCTO = ? AND
                        CRKCOO = ? AND
                        CRLNID = ?"
            };
            cmd.Parameters.Add(new iDB2Parameter("OrderNum", iDB2DbType.iDB2Decimal));
            cmd.Parameters["OrderNum"].Precision = 8;
            cmd.Parameters["OrderNum"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("OrderType", iDB2DbType.iDB2Graphic, 2));
            cmd.Parameters.Add(new iDB2Parameter("CompanyNum", iDB2DbType.iDB2Graphic, 5));
            cmd.Parameters.Add(new iDB2Parameter("OrderLineNum", iDB2DbType.iDB2Decimal));
            cmd.Parameters["OrderLineNum"].Precision = 6;
            cmd.Parameters["OrderLineNum"].Scale = 0;
            return cmd;
        }

        public iDB2Command GetCratesDesc()
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = $@"
                    SELECT 
                        CRZ58CRID,
                        CRZ58CRTY,   
                        CRZ58CRQ,
                        CRZ58CRMSG,
                        DRDL01                              
                    FROM 
                     F580220
                    INNER JOIN {ConfigurationManager.AppSettings["ControlLib"]}.F0005
                    ON DRSY = '58' AND
                        DRRT = 'CT' AND
                        TRIM(DRKY) = CRZ58CRTY
                    WHERE
                        CRDOCO = ? AND
                        CRDCTO = ? AND
                        CRKCOO = ? AND
                        CRLNID = ?"
            };
            cmd.Parameters.Add(new iDB2Parameter("OrderNum", iDB2DbType.iDB2Decimal));
            cmd.Parameters["OrderNum"].Precision = 8;
            cmd.Parameters["OrderNum"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("OrderType", iDB2DbType.iDB2Graphic, 2));
            cmd.Parameters.Add(new iDB2Parameter("CompanyNum", iDB2DbType.iDB2Graphic, 5));
            cmd.Parameters.Add(new iDB2Parameter("OrderLineNum", iDB2DbType.iDB2Decimal));
            cmd.Parameters["OrderLineNum"].Precision = 6;
            cmd.Parameters["OrderLineNum"].Scale = 0;
            return cmd;
        }

        public iDB2Command GetCratesDescDynamic(List<string> filters)
        {
            var filterString = new StringBuilder();
            if (filters.Any()) filterString.Append("WHERE ");
            foreach (var filter in filters)
            {
                filterString.Append(filter);
            }
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = $@"
                    SELECT 
                        CRDOCO,
                        CRDCTO,
                        CRKCOO,
                        CRLNID,
                        CRUORG,
                        CRZ58CRID,
                        CRZ58CRTY,   
                        CRZ58CRQ,
                        CRZ58CRMSG,
                        DRDL01
                    FROM 
	                    F580220
                    INNER JOIN {ConfigurationManager.AppSettings["ControlLib"]}.F0005
                    ON DRSY = '58' AND
                        DRRT = 'CT' AND
                        TRIM(DRKY) = CRZ58CRTY
                    {filterString}"
            };
            return cmd;
        }

        public Crate GetCrate(BusinessSystemSession e1Session, decimal orderNum, string orderType, string companyNum, decimal orderLineNum, SqlDmlResponse sqlResponse = null)
        {
            var crate = new Crate();
            try
            {
                var cmd = (e1Session.DbCommands.ContainsKey("CrateManager.GetCrate")) ? e1Session.DbCommands["CrateManager.GetCrate"] : GetCrate();
                cmd.Parameters["OrderNum"].Value = orderNum;
                cmd.Parameters["OrderType"].Value = orderType;
                cmd.Parameters["CompanyNum"].Value = companyNum;
                cmd.Parameters["OrderLineNum"].Value = orderLineNum;
                cmd.Connection = e1Session.DbConnection;
                if (sqlResponse == null) sqlResponse = new SqlDmlResponse();
                sqlResponse.Init(cmd);
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
                while (reader.Read())
                {
                    crate = new Crate
                    {
                        OrderNum = CastDbNull.To<decimal>(reader["CRDOCO"], 0),
                        OrderType = CastDbNull.To(reader["CRDCTO"], string.Empty).Trim(),
                        OrderCompany = CastDbNull.To(reader["CRKCOO"], string.Empty).Trim(),
                        OrderLineNum = CastDbNull.To<decimal>(reader["CRLNID"], 0),
                        OrderQuantity = CastDbNull.To<decimal>(reader["CRUORG"], 0),
                        CrateId = CastDbNull.To<decimal>(reader["CRZ58CRID"], 0),
                        CrateType = CastDbNull.To(reader["CRZ58CRTY"], string.Empty).Trim(),
                        Quantity = CastDbNull.To<decimal>(reader["CRZ58CRQ"], 0),
                        Message = CastDbNull.To(reader["CRZ58CRMSG"], string.Empty).Trim(),
                        Description = CastDbNull.To(reader["DRDL01"], string.Empty).Trim()
                    };
                }
                reader.Close();
                sqlResponse.RowsAffected = 1;
                sqlResponse.EndTime = DateTime.Now.ToLocalTime();
                sqlResponse.ElapsedSeconds = Convert.ToDecimal((sqlResponse.EndTime - sqlResponse.StartTime).TotalSeconds);
                sqlResponse.Name = "GetCrate";
            }
            catch (Exception err)
            {
                throw new ApplicationException(err.ToString());
            }
            return crate;
        }

        public F580220s GetCratesDescDynamic(BusinessSystemSession e1Session, MTOScheduleLines mtoScheduleLines, SqlDmlResponse sqlResponse = null)
        {
            var f580220S = new F580220s();
            var filters = new List<string>();
            try
            {
                var inString = new StringBuilder();
                foreach (var line in mtoScheduleLines)
                {
                    inString.Append(line.WorkOrder.RelatedOrderNum + ",");
                }
                inString.Length -= 1;
                // Related Orders
                filters.Add("CRDOCO IN (" + inString + ") \r\n");
                var cmd = GetCratesDescDynamic(filters);
                cmd.Connection = e1Session.DbConnection;
                if (sqlResponse == null) sqlResponse = new SqlDmlResponse();
                sqlResponse.Init(cmd);
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
                while (reader.Read())
                {
                    var f580220 = new F580220
                    {
                        CRDOCO = CastDbNull.To<decimal>(reader["CRDOCO"], 0),
                        CRDCTO = CastDbNull.To(reader["CRDCTO"], string.Empty).Trim(),
                        CRKCOO = CastDbNull.To(reader["CRKCOO"], string.Empty).Trim(),
                        CRLNID = CastDbNull.To<decimal>(reader["CRLNID"], 0),
                        CRUORG = CastDbNull.To<decimal>(reader["CRUORG"], 0),
                        CRZ58CRID = CastDbNull.To<decimal>(reader["CRZ58CRID"], 0),
                        CRZ58CRTY = CastDbNull.To(reader["CRZ58CRTY"], string.Empty).Trim(),
                        CRZ58CRQ = CastDbNull.To<decimal>(reader["CRZ58CRQ"], 0),
                        CRZ58CRMSG = CastDbNull.To(reader["CRZ58CRMSG"], string.Empty).Trim(),
                        DRDL01 = CastDbNull.To(reader["DRDL01"], string.Empty).Trim()
                    };
                    f580220S.Add(f580220);
                }
                reader.Close();
                sqlResponse.RowsAffected = f580220S.Count;
                sqlResponse.EndTime = DateTime.Now.ToLocalTime();
                sqlResponse.ElapsedSeconds = Convert.ToDecimal((sqlResponse.EndTime - sqlResponse.StartTime).TotalSeconds);
                sqlResponse.Name = "GetCratesDescDynamic";
            }
            catch (Exception err)
            {
                throw new ApplicationException(err.ToString());
            }
            return f580220S;
        }
    }
}
