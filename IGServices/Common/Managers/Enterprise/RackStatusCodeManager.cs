using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using IBM.Data.DB2.iSeries;
using IGServices.Common.Types;
using IGServices.Common.Types.E1;
using IGServices.Common.Types.Enterprise;
using IGServices.Common.Utilities;

namespace IGServices.Common.Managers.Enterprise
{
    public class RackStatusCodeManager
    {

        public RackStatusCodeManager(BusinessSystemSession e1Session)
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

        public iDB2Command GetRackStatusCodes()
        {
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = String.Format(
                @"SELECT
                        DRKY,
                        DRDL01
                    FROM
                        {0}.F0005 
                    WHERE
                        DRSY = '58' AND
                        DRRT = 'ST'", ConfigurationManager.AppSettings["ControlLib"]);
            return cmd;
        }

        public RackStatusCodes GetRackStatusCodes(BusinessSystemSession e1Session)
        {
            var rackStatusCodes = new RackStatusCodes();
            try
            {
                var cmd = GetRackStatusCodes();
                cmd.Connection = e1Session.DbConnection;
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
                while (reader.Read())
                {
                    var code = new RackStatusCode();
                    code.UserCode = CastDbNull.To<String>(reader["DRKY"], String.Empty).Trim();
                    code.Description1 = CastDbNull.To<String>(reader["DRDL01"], String.Empty).Trim();
                    rackStatusCodes.Add(code);
                }
                reader.Close();
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.Message);
            }
            return rackStatusCodes;
        }


    }
}
