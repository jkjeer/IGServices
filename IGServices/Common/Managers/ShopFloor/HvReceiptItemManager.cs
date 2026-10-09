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
    public class HvReceiptItemManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public HvReceiptItemManager(BusinessSystemSession e1Session)
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
                HVReceiptsID,
                PlantID,
                WOrderNum,
                WOrderType,
                E1Schedule,
                FirmSchedule,
                TransactionDate,
                ProductionDate,
                SerialNumber,
                Location,
                ProductionLineCode,
                LineOperationCode,
                E1Workcenter,
                E1Operation,
                Workstation,
                LineID,
                Shift,
                GlassHandler1,
                GlassHandler2,
                WIP,
                Quantity,
                SqftActual,
                SqftBillable,
                Transferred,
                TransferredDate,
                Upload,
                ProdType,
                Crew,
                Lite,
                ShiftOverride,
                IPAddress,
                AutoConsume
	        FROM 
                dbo.HVReceipts
	        WHERE 
                {0}
                (Upload = 0 OR Upload IS NULL)", filterString);
            return cmd;
        }

        public HvReceiptItems Extract(BusinessSystemSession session, SqlConnection cnShopFloor, Plant plant, string status)
        {
            var records = new HvReceiptItems();
            var cmd = new SqlCommand();
            List<String> filters = new List<string>();
            List<SqlParameter> parameters = new List<SqlParameter>();
            try
            {
                //// Transfer Status
                //if (!String.IsNullOrEmpty(status))
                //{
                //    filters.Add("Upload = @Upload AND\r\n");
                //    parameters.Add(new SqlParameter("@Upload", SqlDbType.Bit));
                //    parameters.Last().Value = (status == "Y") ? 1 : 0;
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
                    var rec = new HvReceiptItem();
                    rec.PlantID = plant.PlantNum;
                    rec.HVReceiptsID = CastDbNull.To<int>(reader["HVReceiptsID"], 0);
                    //rec.PlantID = CastDbNull.To<string>(reader["PlantID"], String.Empty).Trim();
                    rec.WOrderNum = CastDbNull.To<decimal>(reader["WOrderNum"], 0);
                    rec.WOrderType = CastDbNull.To<string>(reader["WOrderType"], String.Empty).Trim();
                    rec.E1Schedule = CastDbNull.To<decimal>(reader["E1Schedule"], 0);
                    rec.FirmSchedule = CastDbNull.To<decimal>(reader["FirmSchedule"], 0);
                    rec.TransactionDate = CastDbNull.To<DateTime>(reader["TransactionDate"], DateTime.MinValue);
                    rec.ProductionDate = CastDbNull.To<DateTime>(reader["ProductionDate"], DateTime.MinValue);
                    rec.SerialNumber = CastDbNull.To<string>(reader["SerialNumber"], String.Empty).Trim();
                    rec.Location = CastDbNull.To<string>(reader["Location"], String.Empty).Trim();
                    rec.ProductionLineCode = CastDbNull.To<string>(reader["ProductionLineCode"], String.Empty).Trim();
                    rec.LineOperationCode = CastDbNull.To<string>(reader["LineOperationCode"], String.Empty).Trim();
                    rec.E1Workcenter = CastDbNull.To<string>(reader["E1Workcenter"], String.Empty).Trim();
                    rec.E1Operation = CastDbNull.To<string>(reader["E1Operation"], String.Empty).Trim();
                    rec.Workstation = CastDbNull.To<string>(reader["Workstation"], String.Empty).Trim();
                    rec.LineID = CastDbNull.To<string>(reader["LineID"], String.Empty).Trim();
                    rec.Shift = CastDbNull.To<string>(reader["Shift"], String.Empty).Trim();
                    rec.GlassHandler1 = CastDbNull.To<decimal>(reader["GlassHandler1"], 0);
                    rec.GlassHandler2 = CastDbNull.To<decimal>(reader["GlassHandler2"], 0);
                    rec.WIP = CastDbNull.To<bool>(reader["WIP"], false);
                    rec.Quantity = CastDbNull.To<decimal>(reader["Quantity"], 0);
                    rec.SqftActual = CastDbNull.To<decimal>(reader["SqftActual"], 0);
                    rec.SqftBillable = CastDbNull.To<decimal>(reader["SqftBillable"], 0);
                    rec.Transferred = CastDbNull.To<bool>(reader["Transferred"], false);
                    rec.TransferredDate = CastDbNull.To<DateTime>(reader["TransferredDate"], DateTime.MinValue);
                    rec.Upload = CastDbNull.To<bool>(reader["Upload"], false);
                    rec.ProdType = CastDbNull.To<string>(reader["ProdType"], String.Empty).Trim();
                    rec.Crew = CastDbNull.To<string>(reader["Crew"], String.Empty).Trim();
                    rec.Lite = CastDbNull.To<string>(reader["Lite"], String.Empty).Trim();
                    rec.ShiftOverride = CastDbNull.To<bool>(reader["ShiftOverride"], false);
                    rec.IPAddress = CastDbNull.To<string>(reader["IPAddress"], String.Empty).Trim();
                    rec.AutoConsume = CastDbNull.To<bool>(reader["AutoConsume"], false);
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
            * Save Cust Pack Records to Shop Floor
            **********************************************************************/
        public SqlDmlResponses SaveTransferProperties(HvReceiptItems items, short sFUpdateBatchSize, SqlConnection cn)
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
                    rowCount++;
                    var cmdStatement = $@"UPDATE dbo.HVReceipts SET Upload = 1, TransferDate = GETDATE() WHERE HVReceiptsID = {record.HVReceiptsID};";
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
                cmd.CommandText = $"UPDATE dbo.HVReceipts SET TransferStatus = '{postStatus}' WHERE TransferStatus = '{preStatus}'";
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
        * Save Cust Pack Records to E1
        **********************************************************************/
        public F580030s Transform(HvReceiptItems records)
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
                    f580030.SFEDBT = rec.TransferBatchId.ToString();
                    f580030.SFEDTN = edtn.ToString().PadLeft(10, '0').PadRight(22);
                    f580030.SFEDLN = 1000;
                    f580030.SFEDUS = "SOASVC";
                    f580030.SFEDCT = "HR";
                    f580030.SFTYTN = (rec.ProdType == "MTO") ? "IGMTO" : "IGMTS";
                    f580030.SFEDFT = "IGHVR";
                    f580030.SFEDDT = E1TimeManager.CastToE1Date(DateTime.Now);
                    f580030.SFDRIN = "1";
                    f580030.SFEDSP = "";
                    f580030.SFPNID = "HVReceipts";
                    f580030.SFTNAC = "02";
                    f580030.SFMMCU = rec.PlantID.PadLeft(12);
                    f580030.SFACDT = E1TimeManager.CastToE1Date(DateTime.Now);
                    f580030.SFSTTM = $"{rec.TransactionDate:yyyy-MM-dd HH:mm:ss}";
                    f580030.SFDTMS = $"{rec.ProductionDate:yyyy-MM-dd HH:mm:ss}";
                    f580030.SFLOTN = rec.SerialNumber;
                    f580030.SFZ58SN = "";
                    f580030.SFDOCO = rec.WOrderNum;
                    f580030.SFDCTO = (rec.WOrderNum > 0) ? "WO" : "";
                    f580030.SFMCU = rec.E1Workcenter.PadLeft(12);
                    f580030.SFZ58SCN = rec.E1Schedule;
                    f580030.SFZ58SCSN = rec.FirmSchedule;
                    f580030.SFSFT = rec.Shift;
                    f580030.SFLOCN = rec.Location;
                    f580030.SFUORG = 1000;
                    f580030.SFSOCN = 0;
                    f580030.SFZ58IGLN = rec.LineID.PadLeft(2).Substring(0, 2).Trim();
                    f580030.SFZ58UID = rec.HVReceiptsID.ToString();
                    f580030.SFZ58PT = "";
                    f580030.SFZ58L1CLN = 0;
                    f580030.SFZ58BSF = rec.SqftBillable * 100;
                    f580030.SFZ5850000 = rec.SqftActual * 100;
                    f580030.SFFLAG = (rec.WIP) ? "1" : "";
                    f580030.SFZ58FLAG = (rec.AutoConsume) ? "" : "R";
                    f580030.SFZ58IP01 = rec.IPAddress;
                    f580030.SFZ58PCR = rec.Crew.PadLeft(2).Substring(0, 2).Trim();
                    f580030.SFZ58LWSTN = rec.E1Operation;
                    //f580030.SFZ58LNID = rec.HVTransactionHistoryID;
                    //f580030.SFZ58RT = rec.RackType;
                    //f580030.SFZ58HST = rec.RackStatus;
                    //f580030.SFZ58RCD = rec.RackReasonStatus;
                    f580030.SFUPMJ = E1TimeManager.CastToE1Date(DateTime.Now);
                    f580030.SFUPMT = E1TimeManager.CastToE1Time(DateTime.Now);
                    f580030s.Add(f580030);
                }
            }
            catch (Exception e)
            {
                throw new FaultException(e.Message);
            }
            return f580030s;
        }
    }
}

