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
    public class TagInfoItemManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public TagInfoItemManager(BusinessSystemSession e1Session)
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
                TagInfoId,
	            OrderNum,
	            OrderType,
                SerialNumber,
                LineId,
                Shift
                QuantityPacked,
                ScheduleNum,
                WorkStation,
                PackedBy,
                RackTypeCode,
                IPAddress,
                WrittenDate
            FROM	
	            TagInfo
	        WHERE 
                {0} 
                ISNULL(Transferred,0) = 0", filterString.ToString());
            return cmd;
        }

        public TagInfoItems Extract(BusinessSystemSession session, SqlConnection cnShopFloor, Plant plant, string status)
        {
            var records = new TagInfoItems();
            var cmd = new SqlCommand();
            List<String> filters = new List<string>();
            List<SqlParameter> parameters = new List<SqlParameter>();
            try
            {
                // Transfer Status
                if (!String.IsNullOrEmpty(status))
                {
                    filters.Add("Transferred = @Upload AND\r\n");
                    parameters.Add(new SqlParameter("@Upload", SqlDbType.Bit));
                    parameters.Last().Value = (status == "Y") ? 1 : 0;
                }

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
                    var rec = new TagInfoItem();
                    rec.PlantID = plant.PlantNum;
                    rec.TagInfoID = CastDbNull.To<int>(reader["TagInfoID"], 0);
                    rec.PlantID = CastDbNull.To<string>(reader["PlantID"], String.Empty).Trim();
                    rec.OrderNum = CastDbNull.To<decimal>(reader["OrderNum"], 0);
                    rec.OrderType = CastDbNull.To<string>(reader["OrderType"], String.Empty).Trim();
                    rec.LineID = CastDbNull.To<string>(reader["LineID"], String.Empty).Trim();
                    rec.Shift = CastDbNull.To<string>(reader["Shift"], String.Empty).Trim();
                    rec.WrittenDate = CastDbNull.To<DateTime>(reader["WrittenDate"], DateTime.MinValue);
                    rec.WorkStation = CastDbNull.To<string>(reader["WorkStation"], String.Empty).Trim();
                    rec.QuantityPacked = CastDbNull.To<decimal>(reader["QuantityPacked"], 0);
                    rec.ScheduleDate = CastDbNull.To<DateTime>(reader["ScheduleDate"], DateTime.MinValue);
                    rec.PackedBy = CastDbNull.To<string>(reader["PackedBy"], String.Empty).Trim();
                    rec.PrintTo = CastDbNull.To<bool>(reader["PrintTo"], false);
                    rec.PrinterName = CastDbNull.To<string>(reader["PrinterName"], String.Empty).Trim();
                    rec.ScheduleNum = CastDbNull.To<decimal>(reader["ScheduleNum"], 0);
                    rec.SeriaLNumber = CastDbNull.To<string>(reader["SeriaLNumber"], String.Empty).Trim();
                    rec.RackTypeCode = CastDbNull.To<string>(reader["RackTypeCode"], String.Empty).Trim();
                    rec.Transferred = CastDbNull.To<bool>(reader["Transferred"], false);
                    rec.TransferDate = CastDbNull.To<DateTime>(reader["TransferDate"], DateTime.MinValue);
                    rec.IPAddress = CastDbNull.To<string>(reader["IPAddress"], String.Empty).Trim();
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
        public SqlDmlResponses SaveTransferProperties(TagInfoItems items, short sFUpdateBatchSize, SqlConnection cn)
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
                    var cmdStatement = $@"UPDATE dbo.TagInfo SET Transferred = 1, TransferredDate = GETDATE() WHERE TagInfoID = {record.TagInfoID};";
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
                cmd.CommandText = $"UPDATE dbo.TagInfo SET TransferStatus = '{postStatus}' WHERE TransferStatus = '{preStatus}'";
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
        public F580022s Transform(TagInfoItems records)
        {
            var f580022s = new F580022s();
            try
            {
                int edtn = 0;
                foreach (var rec in records)
                {
                    var f580022 = new F580022();
                    edtn++;
                    f580022.CAEDUS = "SQDR";
                    f580022.CAEDBT = rec.TransferBatchId.ToString();
                    f580022.CAEDTN = edtn.ToString().PadLeft(10, '0').PadRight(22);
                    f580022.CAEDLN = 1000;
                    f580022.CAEDCT = "TI";
                    f580022.CATYTN = "IGTAG";
                    f580022.CAEDFT = "SQDR_TAG";
                    f580022.CAEDDT = E1TimeManager.CastToE1Date(DateTime.Now);
                    f580022.CADRIN = "1";
                    f580022.CAEDSP = "";
                    f580022.CAPNID = "TagInfo";
                    f580022.CATNAC = "02";
                    f580022.CAMMCU = rec.PlantID.PadLeft(12);
                    f580022.CADOCO = rec.OrderNum;
                    f580022.CADCTO = rec.OrderType;
                    f580022.CALOTN = rec.SeriaLNumber;
                    f580022.CAMCU = rec.LineID;
                    f580022.CASFT = rec.Shift;
                    f580022.CASOQS = rec.QuantityPacked * 1000;
                    f580022.CAUOM = "EA";
                    f580022.CAZ58SCN = rec.ScheduleNum;
                    f580022.CAURCD = rec.WorkStation;
                    f580022.CAURDT = E1TimeManager.CastToE1Date(rec.WrittenDate.Value);
                    f580022.CAURAT = E1TimeManager.CastToE1Time(rec.WrittenDate.Value);
                    f580022.CAURRF = rec.PackedBy;
                    f580022.CAUSER = "SOASVC";
                    f580022.CAPID = "SQDR_IGTAG";
                    f580022.CAUPMJ = E1TimeManager.CastToE1Date(DateTime.Now);
                    f580022.CATDAY = E1TimeManager.CastToE1Time(DateTime.Now);
                    f580022.CAZ58RT = rec.RackTypeCode;
                    f580022.CAZ58IP01 = rec.IPAddress;
                    f580022s.Add(f580022);
                }
            }
            catch (Exception e)
            {
                throw new FaultException(e.Message);
            }
            return f580022s;
        }
    }
}

