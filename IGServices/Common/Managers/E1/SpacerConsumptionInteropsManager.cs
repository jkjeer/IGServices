using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using IBM.Data.DB2.iSeries;
using IGServices.Common.Types.E1;
using IGServices.Common.Types.Response;
using IGServices.Common.Types.ShopFloor;
using IGServices.Common.Utilities;
using NLog;

namespace IGServices.Common.Managers.E1
{
    public class SpacerConsumptionInteropsManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public SpacerConsumptionInteropsManager(BusinessSystemSession e1Session)
        {
            var hash = new Dictionary<string, iDB2Command>
            {
                {"SpacerConsumptionInteropsManager.GetExistingF580030", GetExistingF580030()}
            };
            foreach (var keyName in hash.Keys.Where(keyName => !e1Session.DbCommands.ContainsKey(keyName)))
            {
                e1Session.DbCommands.Add(keyName, hash[keyName]);
            }
        }

        public iDB2Command GetExistingF580030()
        {
            var cmd = new iDB2Command();
            try
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"
                    SELECT
                        COUNT(*) AS RECS
                    FROM
                        F580030
                    WHERE
                        SFMMCU = ?
                        AND SFEDCT = ?
                        AND SFZ58UID = ? ";
                cmd.Parameters.Add(new iDB2Parameter("SFMMCU", iDB2DbType.iDB2Graphic, 12));
                cmd.Parameters.Add(new iDB2Parameter("SFEDCT", iDB2DbType.iDB2Graphic, 2));
                cmd.Parameters.Add(new iDB2Parameter("SFZ58UID", iDB2DbType.iDB2Decimal));
                cmd.Parameters["SFZ58UID"].Precision = 14;
                cmd.Parameters["SFZ58UID"].Scale = 0;
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
            return cmd;
        }

        public static List<string> ValidateE1(BusinessSystemSession e1Session, SpacerConsumptions spacerConsumptions, out ProcessResponse returnResponse)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info("BEGIN: " + DateTime.Now.ToLocalTime());
            ProcessResponse childResponse = null;
            var spacerConsumptionInteropsManager = new SpacerConsumptionInteropsManager(e1Session);
            var validationErrors = new List<string>();
            foreach (var spacerConsumption in spacerConsumptions)
            {
                if (spacerConsumptionInteropsManager.IsExistingF580030(e1Session, spacerConsumption.PlantNum, spacerConsumption.ConsumptionAggregateId, out childResponse))
                {
                    spacerConsumption.IsValid = false;
                    spacerConsumption.Transferred = true;
                    spacerConsumption.TransferStatus = "Y";
                    spacerConsumption.ValidationStatus = "Previously Uploaded";
                    spacerConsumption.TransferMessage = "Previously Uploaded to E1 F580030";
                    validationErrors.Add("Previously Uploaded to E1 F580030: " + spacerConsumption.ItemNum);
                }
            }
            returnResponse = childResponse;
            Logger.Info("END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            return validationErrors;
        }

        private bool IsExistingF580030(BusinessSystemSession e1Session, string plantNum, int consumptionAggregateId, out ProcessResponse returnResponse)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info("BEGIN: " + DateTime.Now.ToLocalTime());
            var processResponse = new ProcessResponse("Get Existing F580030");
            var sqlResponse = new SqlDmlResponse();
            var retVal = false;
            try
            {
                var cmd = (e1Session.DbCommands.ContainsKey("SpacerConsumptionInteropsManager.GetExistingF580030")) ? e1Session.DbCommands["SpacerConsumptionInteropsManager.GetExistingF580030"] : GetExistingF580030();
                cmd.Parameters["SFMMCU"].Value = plantNum.PadLeft(12);
                cmd.Parameters["SFEDCT"].Value = "SC";
                cmd.Parameters["SFZ58UID"].Value = consumptionAggregateId;
                cmd.Connection = e1Session.DbConnection;
                Logger.Trace("CN: " + e1Session.DbConnection.ConnectionString);
                Logger.Trace("SQL: " + Environment.NewLine + cmd.CommandText);
                Logger.Trace("PARMS: " + string.Join(", ", cmd.Parameters.Cast<iDB2Parameter>().AsQueryable().Select(parm => parm.Value)));
                sqlResponse.Init(cmd);
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
                var rowCount = 0;
                while (reader.Read())
                {
                    rowCount = CastDbNull.To(reader["RECS"], 0);
                    retVal = (rowCount > 0);
                }
                reader.Close();
                sqlResponse.Done(rowCount);
                Logger.Trace("ROWS: " + rowCount);
            }
            catch (Exception e)
            {
                processResponse.Errors.Add(new Error(e1Session.DbConnection.ConnectionString + "  " + e));
            }
            finally
            {
                processResponse.SqlDmlResponses.Add(sqlResponse);
                processResponse.Done();
                Logger.Info("END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
                returnResponse = processResponse;
            }
            return retVal;
        }
    }
}
