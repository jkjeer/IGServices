using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.ServiceModel;
using System.Text;
using IBM.Data.DB2.iSeries;
using IGServices.Common.Managers.E1;
using IGServices.Common.Managers.Enterprise;
using IGServices.Common.Types.E1;
using IGServices.Common.Types.Enterprise;
using IGServices.Common.Types.Response;
using IGServices.Common.Types.ShopFloor;
using IGServices.Common.Utilities;
using NLog;

namespace IGServices.Common.Managers.ShopFloor
{
    public class RemakeItemManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public RemakeItemManager(BusinessSystemSession e1Session)
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
	            r.E1RemakeId,
                r.WOrderNum,
                r.WOrderType,
                CAST(r.LiteNumber AS INT) AS LiteNumber,
                r.UnitID,
                r.TransactionDate,
                r.ProductionDate,
                r.Quantity,
                ub.SchedNum AS OriginalScheduleNum,
                COALESCE(TRY_CAST(rk.RemakeSchedule AS DECIMAL(5,0)), 0) AS ScheduleNum,
                COALESCE(TRY_CAST(rk.Sequence AS INT), 0) AS ScheduleSeq,
                CASE
                    WHEN r.LiteNumber = 1 THEN o.Lite1Plant
                    WHEN r.LiteNumber = 2 THEN o.Lite2Plant
                    WHEN r.LiteNumber = 3 THEN o.Lite3Plant
                    ELSE ''
                END AS Supplier,
                CASE
                    WHEN r.LiteNumber = 1 THEN ub.Glass1
                    WHEN r.LiteNumber = 2 THEN ub.Glass2
                    WHEN r.LiteNumber = 3 THEN ub.Glass3
                    ELSE ''
                END AS GlassType,
                ub.ShapeNum,
                ub.Dimensions,
	            ub.CustName,
                CASE
                    WHEN r.ShopFloorSystem = 'IG' THEN dbo.fnConvertToE1WorkCenterByStation(r.LineId)
                    ELSE dbo.fnConvertToE1WorkCenterByStation2(RTRIM(r.WorkCenter), r.LineId)
                END AS WorkCenter,
                dbo.fnConvertToE1WorkStation(r.WorkCenter, r.ShopFloorSystem) AS WorkStation,
                r.LineId,
                r.Shift,
                r.Crew,
				CASE 
					WHEN rk.RemakeSchedule IS NULL THEN '' 
					WHEN r.LiteNumber = 1 THEN rk.ExteriorRack
					WHEN r.LiteNumber = 2 AND ISNULL(ub.Glass3, '') = '' THEN rk.InteriorRack
					WHEN r.LiteNumber = 2 AND ISNULL(ub.Glass3, '') <> '' THEN rk.ThirdLiteRack
					WHEN r.LiteNumber = 3 THEN rk.InteriorRack
				END AS WipRack,
				CASE 
					WHEN rk.RemakeSchedule IS NULL THEN CAST(0 AS TINYINT)
					WHEN r.LiteNumber = 1 THEN rk.ExteriorSlot
					WHEN r.LiteNumber = 2 AND ISNULL(ub.Glass3, '') = '' THEN rk.InteriorSlot
					WHEN r.LiteNumber = 2 AND ISNULL(ub.Glass3, '') <> '' THEN rk.ThirdLiteSlot
					WHEN r.LiteNumber = 3 THEN rk.InteriorSlot
				END AS WipSlot,
                r.Transferred,
                r.TransferredDate
            FROM
                E1Remake r
				INNER JOIN dbo.Remakes rk
				ON  rk.UnitID = r.UnitID AND
                    rk.OrderNum = r.WOrderNum
                LEFT OUTER JOIN IncomingOrders o
                ON  o.WOrderNum = r.WOrderNum
                LEFT OUTER JOIN UnitBox ub
                ON  ub.UnitIDtxt = r.UnitID
	        WHERE 
                {0} 
                ISNULL(r.Transferred,0) = 0", filterString);
            return cmd;
        }

        public RemakeItems Extract(BusinessSystemSession session, SqlConnection cnShopFloor, Plant plant, string status)
        {
            var records = new RemakeItems();
            var cmd = new SqlCommand();
            List<String> filters = new List<string>();
            List<SqlParameter> parameters = new List<SqlParameter>();
            try
            {
                // Transfer Status
                if (!String.IsNullOrEmpty(status))
                {
                    filters.Add("r.Transferred = @Upload AND\r\n");
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
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
                {
                    while (reader.Read())
                    {
                        var rec = new RemakeItem();
                        rec.E1RemakeId = CastDbNull.To<int>(reader["E1RemakeId"], 0);
                        rec.PlantId = plant.PlantNum;
                        rec.WOrderNum = CastDbNull.To<decimal>(reader["WOrderNum"], 0);
                        rec.WOrderType = CastDbNull.To<string>(reader["WOrderType"], String.Empty).Trim();
                        rec.LiteNumber = Convert.ToDecimal(CastDbNull.To<int>(reader["LiteNumber"], 0));
                        rec.TransactionDate = CastDbNull.To<DateTime>(reader["TransactionDate"], DateTime.MinValue);
                        rec.TransactionDateString = rec.TransactionDate.ToString("MM/dd/yyyy hh:mm:ss tt");
                        rec.ProductionDate = CastDbNull.To<DateTime>(reader["ProductionDate"], DateTime.MinValue);
                        rec.OriginalScheduleNum = CastDbNull.To<int>(reader["OriginalScheduleNum"], 0);
                        rec.ScheduleNum = CastDbNull.To<decimal>(reader["ScheduleNum"], 0);
                        rec.ScheduleSeq = Convert.ToDecimal(CastDbNull.To<int>(reader["ScheduleSeq"], 0));
                        rec.UnitID = CastDbNull.To<string>(reader["UnitID"], String.Empty).Trim();
                        rec.WorkCenter = CastDbNull.To<string>(reader["WorkCenter"], String.Empty).Trim();
                        rec.WorkStation = CastDbNull.To<string>(reader["WorkStation"], String.Empty).Trim();
                        rec.LineId = CastDbNull.To<string>(reader["LineId"], String.Empty).Trim();
                        rec.Shift = CastDbNull.To<string>(reader["Shift"], String.Empty).Trim();
                        rec.Crew = CastDbNull.To<string>(reader["Crew"], String.Empty).Trim();
                        rec.CustName = CastDbNull.To<string>(reader["CustName"], String.Empty).Trim();
                        rec.GlassType = CastDbNull.To<string>(reader["GlassType"], String.Empty).Trim();
                        rec.Dimension = CastDbNull.To<string>(reader["Dimensions"], String.Empty).Trim();
                        rec.ShapeNum = CastDbNull.To<int>(reader["ShapeNum"], 0);
                        var supplierPlant = CastDbNull.To<string>(reader["Supplier"], String.Empty).Trim();
                        rec.Supplier = (String.IsNullOrEmpty(supplierPlant))
                            ? ""
                            : PlantManager.GetPlant(supplierPlant)?.PlantCode;
                        rec.WipRack = CastDbNull.To<string>(reader["WipRack"], String.Empty).Trim();
                        rec.WipSlot = CastDbNull.To<Byte>(reader["WipSlot"], 0).ToString();
                        rec.Quantity = CastDbNull.To<decimal>(reader["Quantity"], 0);
                        rec.Transferred = CastDbNull.To<bool>(reader["Transferred"], false);
                        rec.TransferredDate = CastDbNull.To<DateTime>(reader["TransferredDate"], DateTime.MinValue);
                        records.Add(rec);
                    }
                    reader.Close();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return records;
        }

        /**********************************************************************
            * Save Cust Pack Records to Shop Floor
            **********************************************************************/
        public SqlDmlResponses SaveTransferProperties(RemakeItems items, short sFUpdateBatchSize, SqlConnection cn)
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
                    var cmdStatement = $@"UPDATE dbo.E1Remake SET Transferred = 1, TransferredDate = GETDATE() WHERE E1RemakeId = {record.E1RemakeId};";
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
                throw new Exception(cn.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
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
                cmd.CommandText = $"UPDATE dbo.E1Remakes SET TransferStatus = '{postStatus}' WHERE TransferStatus = '{preStatus}'";
                Logger.Trace(Logging.DbCmdString(cmd));
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + cn.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
                throw new Exception(cn.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }
            return response;
        }

        /**********************************************************************
        * Save Cust Pack Records to E1
        **********************************************************************/
        public F580030s Transform(RemakeItems records)
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
                    f580030.SFEDCT = "RM";
                    f580030.SFTYTN = "REMAKE";
                    f580030.SFEDFT = "REMAKE";
                    f580030.SFEDDT = E1TimeManager.CastToE1Date(DateTime.Now);
                    f580030.SFDRIN = "1";
                    f580030.SFEDDL = 0;
                    f580030.SFEDSP = "";
                    f580030.SFPNID = "Remake";
                    f580030.SFTNAC = "02";
                    f580030.SFMMCU = rec.PlantId.PadLeft(12);
                    f580030.SFACDT = E1TimeManager.CastToE1Date(DateTime.Now);
                    f580030.SFSTTM = $"{rec.TransactionDate:yyyy-MM-dd HH:mm:ss}";
                    f580030.SFDTMS = $"{rec.ProductionDate:yyyy-MM-dd HH:mm:ss}";
                    f580030.SFLOTN = "";
                    f580030.SFZ58SN = "";
                    f580030.SFDOCO = rec.WOrderNum;
                    f580030.SFDCTO = rec.WOrderNum > 0 ? "WO" : "";
                    f580030.SFMCU = rec.WorkCenter.PadLeft(3, '0').PadLeft(12);
                    f580030.SFZ58SCN = rec.ScheduleNum;
                    f580030.SFZ58SCSN = rec.ScheduleSeq;
                    f580030.SFSFT = rec.Shift;
                    f580030.SFZ58RCP = "";
                    f580030.SFUORG = rec.Quantity * 1000;
                    f580030.SFSOCN = 0;
                    f580030.SFZ58IGLN = rec.LineId.PadLeft(2).Substring(0, 2).Trim();
                    f580030.SFZ58UID = rec.E1RemakeId.ToString();
                    f580030.SFZ58PT = String.IsNullOrEmpty(rec.WipRack) ? "" : $"{rec.WipRack}:{rec.WipSlot.PadLeft(2, '0')}";
                    //f580030.SFZ58PT = "MTO";
                    f580030.SFZ58L1CLN = rec.LiteNumber;
                    f580030.SFZ58BSF = 0;
                    f580030.SFZ5850000 = 0;
                    f580030.SFZ58PCR = rec.Crew.PadLeft(2).Substring(0, 2).Trim();
                    f580030.SFZ58LWSTN = rec.WorkStation;
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

