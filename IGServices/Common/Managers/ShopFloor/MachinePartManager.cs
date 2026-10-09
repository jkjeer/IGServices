using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.ServiceModel;
using System.Text;
using IBM.Data.DB2.iSeries;
using IGServices.Common.Managers.E1;
using IGServices.Common.Types.E1;
using IGServices.Common.Types.Enterprise;
using IGServices.Common.Types.Response;
using IGServices.Common.Types.ShopFloor;
using IGServices.Common.Utilities;
using NLog;

namespace IGServices.Common.Managers.ShopFloor
{
    public class MachinePartManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public MachinePartManager(BusinessSystemSession session)
        {
            var hash = new Dictionary<string, iDB2Command>
            {
                {"MachinePartManager.GetItemLocation", GetItemLocation()}
            };
            foreach (var keyName in hash.Keys.Where(keyName => !session.DbCommands.ContainsKey(keyName)))
            {
                session.DbCommands.Add(keyName, hash[keyName]);
            }
        }

        /**********************************************************************
        * Get Cust Pack Records
        **********************************************************************/
        public SqlCommand Extract(List<string> filters)
        {
            var filterString = new StringBuilder();
            foreach (string filter in filters)
            {
                filterString.Append(filter);
            }

            var cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = String.Format(@"
	        SELECT 
                MachinePartConsumedId,
                ItemNum,
                Quantity,
                UnitMeasure,
                Location,
                TransactionId,
                TransactionDate,
                WrittenDate,
                WrittenBy,
                TransferStatus,
                TransferDate,
                TransferMessage,
                TransferBatchNum,
                TransferId
	        FROM 
                MachinePartConsumed
	        WHERE 
                {0} 1=1", filterString.ToString());
            return cmd;
        }

        public MachinePartConsumedItems Extract(BusinessSystemSession session, SqlConnection cnShopFloor, Plant plant, string status)
        {
            var records = new MachinePartConsumedItems();
            var cmd = new SqlCommand();
            List<String> filters = new List<string>();
            List<SqlParameter> parameters = new List<SqlParameter>();
            try
            {
                // Transfer Status
                //if (!String.IsNullOrEmpty(status))
                //{
                filters.Add("TransferStatus = @TransferStatus AND\r\n");
                parameters.Add(new SqlParameter("@TransferStatus", SqlDbType.VarChar, 10));
                parameters.Last().Value = "Q";
                //}

                // Command
                cmd = Extract(filters);
                foreach (SqlParameter parameter in parameters)
                {
                    cmd.Parameters.Add(parameter);
                }
                cmd.Connection = cnShopFloor;
                Logger.Info(Logging.DbCmdString(cmd));

                // Reader
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
                while (reader.Read())
                {
                    var rec = new MachinePartConsumedItem();
                    rec.PlantNum = plant.PlantNum;
                    rec.ConsumedId = CastDbNull.To<int>(reader["MachinePartConsumedId"], 0);
                    rec.ItemNum = CastDbNull.To<decimal>(reader["ItemNum"], 0);
                    rec.Quantity = CastDbNull.To<int>(reader["Quantity"], 0);
                    rec.UnitMeasure = CastDbNull.To<String>(reader["UnitMeasure"], String.Empty).Trim();
                    rec.Location = CastDbNull.To<String>(reader["Location"], String.Empty).Trim();
                    rec.TransactionId = CastDbNull.To<int>(reader["TransactionId"], 0);
                    rec.TransactionDate = CastDbNull.To<DateTime>(reader["TransactionDate"], DateTime.MinValue);
                    rec.WrittenDate = CastDbNull.To<DateTime>(reader["WrittenDate"], DateTime.MinValue);
                    rec.E1Location = GetItemLocation(session, rec.PlantNum, rec.Location);
                    records.Add(rec);
                }
                reader.Close();

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return records;
        }

        /**********************************************************************
        * Item Location
        **********************************************************************/
        public iDB2Command GetItemLocation()
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = @"
                SELECT 
	                LMLOCN        
                FROM 
	                F4100
                WHERE 
                    LMMCU = ? AND
                    LMLOCN LIKE ?"
            };
            cmd.Parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("LOCN", iDB2DbType.iDB2Graphic, 20));
            return cmd;
        }

        public string GetItemLocation(BusinessSystemSession session, string plantNum, string location)
        {
            string ret = "";
            try
            {
                var cmd = session.DbCommands["MachinePartManager.GetItemLocation"];
                cmd.Parameters[0].Value = plantNum.PadLeft(12);
                cmd.Parameters[1].Value = "%" + location + "%";
                cmd.Connection = session.DbConnection;
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
                while (reader.Read())
                {
                    ret = CastDbNull.To<string>(reader["LMLOCN"], string.Empty);
                }
                reader.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
            return ret;
        }

        /**********************************************************************
        * Save Cust Pack Records to Shop Floor
        **********************************************************************/
        public SqlDmlResponses SaveTransferProperties(List<MachinePartConsumedItem> items, short sFUpdateBatchSize, SqlConnection cn)
        {
            var statements = new List<string>();
            var responses = new SqlDmlResponses();
            try
            {
                foreach (var record in items)
                {
                    string statement = "";
                    if (record.TransferStatus== "E")
                    {
                        statement = $@"
                     UPDATE dbo.MachinePartConsumed
                    SET TransferStatus = '{record.TransferStatus}', 
                        TransferMessage = '{record.TransferMessage}'
                    WHERE MachinePartConsumedId = { record.ConsumedId }; ";
                    }
                    else
                    {
                        statement = $@"
                    UPDATE dbo.MachinePartConsumed 
                    SET TransferStatus = 'Y', 
                        TransferBatchNum = '{record.TransferBatchNum}', 
                        TransferId = '{record.TransferId}', 
                        TransferDate = GETDATE(), 
                        TransferMessage = '{record.TransferMessage}' 
                    WHERE MachinePartConsumedId = {record.ConsumedId};";
                    }
                    statements.Add(statement);
                }
                responses = Sql.SqlBatchExecution(statements, sFUpdateBatchSize, cn);
            }
            catch (Exception e)
            {
                throw new Exception(cn.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }
            return responses;
        }

        /**********************************************************************
        * Set Transfer Status
        **********************************************************************/
        public SqlDmlResponse SetTransferStatus(SqlConnection cn, string preStatus, string postStatus)
        {
            var cmd = new SqlCommand
            {
                Connection = cn,
                CommandType = CommandType.Text
            };
            var response = new SqlDmlResponse(cmd);
            var cmdStringBldr = new StringBuilder();
            try
            {
                cmd.CommandText =
                    $"UPDATE dbo.MachinePartConsumed SET TransferStatus = '{postStatus}' WHERE ISNULL(TransferStatus, 'N') = '{preStatus}'";
                response.Init(cmd);
                Logger.Trace(Logging.DbCmdString(cmd));
                var rows = cmd.ExecuteNonQuery();
                response.RowsAffected = rows;
            }
            catch (Exception e)
            {
                throw new Exception(cn.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }
            finally
            {
                response.Done();
            }
            return response;
        }

        /**********************************************************************
        * Save Cust Pack Records to E1
        **********************************************************************/
        public F580030s Transform(MachinePartConsumedItems records)
        {
            var f580030s = new F580030s();
            try
            {
                int edtn = 0;
                foreach (var rec in records)
                {
                    var f580030 = new F580030();
                    edtn++;
                    f580030.SFUKID = rec.TransferId;
                    f580030.SFEDBT = rec.TransferBatchNum.ToString();
                    f580030.SFEDTN = edtn.ToString().PadLeft(10, '0').PadRight(22);
                    f580030.SFEDLN = 1000;
                    f580030.SFEDUS = "SVCSOA";
                    f580030.SFEDCT = "HM";
                    f580030.SFTYTN = "IGMTO";
                    f580030.SFEDFT = "IGHM";
                    f580030.SFEDDT = E1TimeManager.CastToE1Date(DateTime.Now);
                    f580030.SFDRIN = "1";
                    f580030.SFEDSP = "";
                    f580030.SFPNID = "IGPart";
                    f580030.SFTNAC = "02";
                    f580030.SFMMCU = rec.PlantNum.PadLeft(12);
                    f580030.SFLITM = rec.ItemNum.ToString();
                    f580030.SFACDT = E1TimeManager.CastToE1Date(DateTime.Now);
                    f580030.SFSTTM = $"{rec.TransactionDate:yyyy-MM-dd HH:mm:ss}";
                    f580030.SFDTMS = $"{rec.WrittenDate:yyyy-MM-dd HH:mm:ss}";
                    f580030.SFLOCN = rec.E1Location;
                    f580030.SFUORG = rec.Quantity*1000;
                    f580030.SFZ58UID = rec.ConsumedId.ToString();
                    f580030.SFJFLG = rec.UnitMeasure;
                    f580030.SFUPMJ = E1TimeManager.CastToE1Date(DateTime.Now);
                    f580030.SFUPMT = E1TimeManager.CastToE1Time(DateTime.Now);
                    f580030s.Add(f580030);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return f580030s;
        }
    }
}

