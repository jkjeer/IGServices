using System;
using System.Collections.Generic;
using System.Data;
using System.ServiceModel;
using IGServices.Common.Types.E1;
using IGServices.Common.Types.Enterprise;
using IGServices.Common.Types.Response;
using IGServices.Common.Utilities;
using IBM.Data.DB2.iSeries;

namespace IGServices.Common.Managers.Enterprise
{
    public class EdiMessageManager
    {

        public EdiMessageManager(BusinessSystemSession e1Session)
        {
            var hash = new Dictionary<string, iDB2Command>
            {
                {"GetEdiMessages", GetEdiMessages()}
            };
            foreach (var keyName in hash.Keys)
            {
                if (!e1Session.DbCommands.ContainsKey(keyName))
                {
                    e1Session.DbCommands.Add(keyName, hash[keyName]);
                }
            }
        }

        /*********************************************************************************
         * Prepared Commands
         *******************************************************************************/
        public iDB2Command GetEdiMessages()
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = @"
                    SELECT
                        ZDEDTY,
                        ZDEDSQ,
                        ZDTXLN
                    FROM
                        F4715
                    WHERE
                        ZDTXLN IS NOT NULL AND
                        ZDKCOO = ? AND
                        ZDDOCO = ? AND
                        ZDDCTO = ? AND
                        ZDLNID = ?
                    ORDER BY
                        ZDEDTY,
                        ZDEDSQ"
            };
            cmd.Parameters.Add(new iDB2Parameter("SalesOrderCompany", iDB2DbType.iDB2Graphic, 5));
            cmd.Parameters.Add(new iDB2Parameter("SalesOrderNum", iDB2DbType.iDB2Decimal));
            cmd.Parameters["SalesOrderNum"].Precision = 8;
            cmd.Parameters["SalesOrderNum"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("SalesOrderType", iDB2DbType.iDB2Graphic, 2));
            cmd.Parameters.Add(new iDB2Parameter("SalesOrderLineNum", iDB2DbType.iDB2Decimal));
            cmd.Parameters["SalesOrderLineNum"].Precision = 6;
            cmd.Parameters["SalesOrderLineNum"].Scale = 0;
            return cmd;
        }

        public EdiMessages GetEdiMessages(BusinessSystemSession e1Session, string companyNum, decimal orderNum, string orderType, decimal orderLineNum, SqlDmlResponse sqlResponse = null)
        {
            var messages = new EdiMessages();
            try
            {
                var cmd = (e1Session.DbCommands.ContainsKey("GetEdiMessages")) ? e1Session.DbCommands["GetEdiMessages"] : GetEdiMessages();
                cmd.Connection = e1Session.DbConnection;
                cmd.Parameters["SalesOrderCompany"].Value = companyNum;
                cmd.Parameters["SalesOrderNum"].Value = orderNum;
                cmd.Parameters["SalesOrderType"].Value = orderType;
                cmd.Parameters["SalesOrderLineNum"].Value = orderLineNum;
                if (sqlResponse == null) sqlResponse = new SqlDmlResponse();
                sqlResponse.Init(cmd);
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
                while (reader.Read())
                {
                    var message = new EdiMessage
                    {
                        EdiType = CastDbNull.To(reader["ZDEDTY"], string.Empty).Trim(),
                        EdiSeqNum = CastDbNull.To<decimal>(reader["ZDEDSQ"], 0),
                        Message = CastDbNull.To(reader["ZDTXLN"], string.Empty).Trim()
                    };
                    messages.Add(message);
                }
                reader.Close();
                sqlResponse.RowsAffected = messages.Count;
                sqlResponse.EndTime = DateTime.Now.ToLocalTime();
                sqlResponse.ElapsedSeconds = Convert.ToDecimal((sqlResponse.EndTime - sqlResponse.StartTime).TotalSeconds);
                sqlResponse.Name = "GetEdiMessages";
            }
            catch (Exception e)
            {
                throw new FaultException(e.Message);
            }
            return messages;
        }
    }
}
