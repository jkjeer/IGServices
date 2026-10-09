using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Globalization;
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
    public class GlassInventoryTransactionItemManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public GlassInventoryTransactionItemManager(BusinessSystemSession e1Session)
        {
            var hash = new Dictionary<string, iDB2Command>
            {
                //{"AddressBookDownloadManager.GetRelatedOrders", GetRelatedOrders()}
            };
            foreach (var keyName in hash.Keys.Where(keyName => !e1Session.DbCommands.ContainsKey(keyName)))
            {
                e1Session.DbCommands.Add(keyName, hash[keyName]);
            }
        }

        /**********************************************************************
        * Get Transactions
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
                    TransactionId,
                    WorkOrderNum,
                    ScheduleNum,
                    UnitId,
                    LiteNum,
                    LiteCode,
                    TransactionDate,
                    TransactionType,
                    SourceTransactionId,
                    RoutingCode,
                    ProductionLineCode,
                    LineOperationCode,
                    Quantity,
                    RejectReason,
                    RackId,
                    SerialNum,
                    RelatedItemNum,
                    E1WorkCenterId,
                    TransferStatus,
                    TransferBatchNum,
                    TransferId,
                    TransferDate,
                    TransferMessage,
                    WrittenDate,
                    Comments
                FROM 
                    EBS.GlassInventoryTransaction
                WHERE 
                    TransferStatus = 'Q'
                ORDER BY
                    TransactionDate", filterString);
            return cmd;
        }

        public GlassInventoryTransactions Extract(BusinessSystemSession session, SqlConnection cnShopFloor, Plant plant, string status)
        {
            var records = new GlassInventoryTransactions();
            List<String> filters = new List<string>();
            List<SqlParameter> parameters = new List<SqlParameter>();
            try
            {

                // Command
                var cmd = Extract(filters);
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
                    var rec = new GlassInventoryTransaction();
                    rec.TransactionType = CastDbNull.To(reader["TransactionType"], String.Empty).Trim(); ;
                    rec.PlantNum = plant.PlantNum;
                    rec.TransactionId = CastDbNull.To<int>(reader["TransactionId"], 0);
                    rec.WOrderNum = CastDbNull.To<decimal>(reader["WorkOrderNum"], 0);
                    rec.ScheduleNum = CastDbNull.To<int>(reader["ScheduleNum"], 0);
                    rec.UnitId = CastDbNull.To<int>(reader["UnitId"], 0);
                    rec.LiteNum = CastDbNull.To<int>(reader["LiteNum"], 0);
                    rec.LiteName = CastDbNull.To(reader["LiteCode"], String.Empty).Trim();
                    rec.RoutingCode = CastDbNull.To(reader["RoutingCode"], String.Empty).Trim();
                    rec.ProductionLineCode = CastDbNull.To(reader["ProductionLineCode"], String.Empty).Trim();
                    rec.LineOperationCode = CastDbNull.To(reader["LineOperationCode"], String.Empty).Trim();
                    rec.Quantity = CastDbNull.To<int>(reader["Quantity"], 0);
                    rec.RejectReason = CastDbNull.To(reader["RejectReason"], String.Empty).Trim();
                    rec.RackId = CastDbNull.To<decimal>(reader["RackId"], 0);
                    rec.ItemNum = CastDbNull.To<decimal>(reader["RelatedItemNum"], 0);
                    rec.E1WorkCenterId = CastDbNull.To(reader["E1WorkCenterId"], String.Empty).Trim();
                    rec.SerialNum = CastDbNull.To(reader["SerialNum"], String.Empty).Trim();
                    rec.Comments = CastDbNull.To(reader["Comments"], String.Empty).Trim();
                    rec.TransactionDate = CastDbNull.To<DateTime>(reader["TransactionDate"], DateTime.Now);
                    records.Add(rec);
                }
                reader.Close();
            }
            catch (Exception e)
            {
                throw new FaultException(e.Message);
            }
            return records;
        }

        /**********************************************************************
            * Save Transfer Properties to Shop Floor
            **********************************************************************/
        public SqlDmlResponses SaveTransferProperties(GlassInventoryTransactions items, short sFUpdateBatchSize, SqlConnection cn)
        {
            var responses = new SqlDmlResponses();
            var cmd = new SqlCommand
            {
                Connection = cn,
                CommandType = CommandType.Text
            };
            var cmdStringBldr = new StringBuilder();
            try
            {
                var rowCount = 0;
                foreach (var record in items)
                {
                    rowCount += 1;
                    var cmdStatement = $@"UPDATE EBS.GlassInventoryTransaction SET TransferStatus = '{record.TransferStatus}', TransferDate = '{record.TransferDate}', TransferId = {record.TransferId}, TransferBatchNum = '{record.TransferBatchNum}' WHERE TransactionId = {record.TransactionId};";
                    cmdStringBldr.Append(cmdStatement);
                    if (rowCount >= sFUpdateBatchSize)
                    {
                        cmd.CommandText = cmdStringBldr.ToString();
                        Logger.Trace(Logging.DbCmdString(cmd));
                        int rows = cmd.ExecuteNonQuery();
                        cmdStringBldr = new StringBuilder();
                        rowCount = 0;
                    }
                }
                if (rowCount > 0)
                {
                    var dmlResponse = new SqlDmlResponse();
                    cmd.CommandText = cmdStringBldr.ToString();
                    dmlResponse.Init(cmd);
                    Logger.Trace(Logging.DbCmdString(cmd));
                    int rows = cmd.ExecuteNonQuery();
                    dmlResponse.RowsAffected = rows;
                    responses.Add(dmlResponse);
                }
                Logger.Info("ROWS: " + rowCount);
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + cn.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
                throw new ApplicationException(cn.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }
            return responses;
        }

        /**********************************************************************
            * Set Transfer Status
            **********************************************************************/
        public SqlDmlResponse SetTransferStatus(SqlConnection cn, string preStatus, string postStatus)
        {
            var response = new SqlDmlResponse();
            var cmd = new SqlCommand
            {
                Connection = cn,
                CommandType = CommandType.Text
            };
            var cmdStringBldr = new StringBuilder();
            try
            {
                cmd.CommandText = $"UPDATE EBS.GlassInventoryTransaction SET TransferStatus = '{postStatus}' WHERE TransferStatus = '{preStatus}'";
                Logger.Trace(Logging.DbCmdString(cmd));
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + cn.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
                throw new ApplicationException(cn.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }
            return response;
        }

        /**********************************************************************
        * Save Records to E1
        **********************************************************************/
        public F580030s Transform(GlassInventoryTransactions records)
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
                    f580030.SFEDBT = rec.TransferBatchNum.ToString(CultureInfo.InvariantCulture);
                    f580030.SFEDTN = edtn.ToString().PadLeft(10, '0').PadRight(22);
                    f580030.SFEDLN = 1000;
                    f580030.SFEDUS = "SOASVC";
                    f580030.SFEDCT = rec.TransactionType;
                    f580030.SFTYTN = "IGMTO";
                    f580030.SFEDFT = "IGMTO";
                    f580030.SFEDDT = E1TimeManager.CastToE1Date(DateTime.Now);
                    f580030.SFDRIN = "1";
                    f580030.SFEDSP = "";
                    f580030.SFPNID = rec.TransactionType == "PP" ? "IGPurchased" : "IGPicked";
                    f580030.SFTNAC = "02";
                    f580030.SFMMCU = rec.PlantNum.PadLeft(12);
                    f580030.SFACDT = E1TimeManager.CastToE1Date(DateTime.Now);
                    f580030.SFSTTM = $"{rec.TransactionDate:yyyy-MM-dd HH:mm:ss}";
                    f580030.SFDTMS = $"{rec.TransactionDate:yyyy-MM-dd HH:mm:ss}";
                    f580030.SFLOTN = rec.SerialNum;
                    f580030.SFLITM = rec.TransactionType == "PP" ? "" : rec.ItemNum.ToString();  
                    f580030.SFZ58SN = "";
                    f580030.SFDOCO = rec.WOrderNum;
                    f580030.SFDCTO = (rec.WOrderNum > 0) ? "WO" : "";
                    f580030.SFMCU = rec.E1WorkCenterId.PadLeft(12);
                    //f580030.SFMCU = "        1701";
                    f580030.SFZ58SCN = rec.ScheduleNum;
                    f580030.SFZ58SCSN = 0;
                    f580030.SFSFT = "";
                    f580030.SFLOCN = "";
                    f580030.SFUORG = rec.Quantity * 1000;
                    f580030.SFSOCN = 0;
                    f580030.SFZ58IGLN = "";
                    f580030.SFZ58UID = rec.TransactionId.ToString();
                    f580030.SFZ58PT = "";
                    f580030.SFZ58L1CLN = rec.LiteNum;
                    f580030.SFZ58BSF = 0;
                    f580030.SFZ5850000 = 0;
                    f580030.SFFLAG = "";
                    f580030.SFZ58IP01 = "";
                    f580030.SFZ58PCR = "";
                    //f580030.SFZ58LWSTN = rec.E1Operation;
                    f580030.SFZ58LWSTN = "001";
                    f580030.SFZ58LNID = 0;
                    f580030.SFZ58RT = "";
                    f580030.SFZ58HST = "";
                    f580030.SFZ58RCD = "";
                    f580030.SFUPMJ = E1TimeManager.CastToE1Date(DateTime.Now);
                    f580030.SFUPMT = E1TimeManager.CastToE1Time(DateTime.Now);
                    f580030.SFZ58AID = "";
                    f580030.SFZ58PT = rec.Comments;
                    f580030.SFZ58RCC = (rec.RejectReason.Length > 25)
                        ? rec.RejectReason.Substring(0, 25)
                        : rec.RejectReason;
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
