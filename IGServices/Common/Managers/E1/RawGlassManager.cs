using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using IBM.Data.DB2.iSeries;
using IGServices.Common.Types.E1;
using IGServices.Common.Types.Response;
using IGServices.Common.Utilities;
using NLog;

namespace IGServices.Common.Managers.E1
{
    public class RawGlassManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /*********************************************************************************
        * Get F580220s Dynamic
        *******************************************************************************/
        private static iDB2Command GetRawGlassDynamic(IReadOnlyCollection<string> filters)
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
                        F3111.WMCPIT,
                        F3111.WMCMCU,
                        F3111.WMDOCO,
                        F3111.WMDCTO,
                        F3111.WMDSC1,
                        F3111.WMDSC2,
                        F3111.WMLOCN,
                        F3111.WMUORG,
                        F3111.WMTRQT,
                        F3111.WMUKID,
                        F3111.WMOPSQ,
                        F3111.WMUPMJ,
                        F3111.WMTDAY,
                        F580014.CRZ58L1TK,
                        F580014.CRZ58L1HT,
                        F580014.CRZ58L1CL,
                        F580014.CRZ58L11C,
                        F580014.CRZ58L12C,
                        F580014.CRZ58L1PX,
                        F580014.CRZ58L1BDM,
                        F580014.CRZ58L1BNM,
                        F580014.CRZ58L1BDN,
                        F580014.CRZ58L1LDM,
                        F580014.CRZ58L1LNM,
                        F580014.CRZ58L1LDN,
                        F580014.CRZ58L1RDM,
                        F580014.CRZ58L1RNM,
                        F580014.CRZ58L1RDN,
                        F580014.CRZ58L1TDM,
                        F580014.CRZ58L1TNM,
                        F580014.CRZ58L1TDN,
                        F580014.CRZ58SHAPE,
                        F4102R.IBGLPT,
                        F580021.S7EDBT,
                        F580021.S7ITM,
                        F580021.S7Z58SCN,
	                    F4102F.IBSRP4,
	                    ROW_NUMBER() OVER (PARTITION BY WMDOCO ORDER BY WMOPSQ) AS WMOPSQ_RN
                    FROM
                        F3111 F3111
                        INNER JOIN F4102 F4102R
                        ON  F3111.WMCPIT = F4102R.IBITM AND
                            F3111.WMMCU = F4102R.IBMCU
                        INNER JOIN F580014 F580014
                        ON  F4102R.IBITM = F580014.CRITM
                        INNER JOIN F580021 F580021
                        ON  F3111.WMDOCO = F580021.S7DOCO
                        INNER JOIN F580020 F580020
                        ON  F580020.S6EDBT = F580021.S7EDBT AND
                            F580020.S6MCU = F580021.S7MCU AND
                            F580020.S6Z58SCN = F580021.S7Z58SCN
                        INNER JOIN F4801
	                    ON	WADOCO = WMDOCO
	                    INNER JOIN F4102 F4102F
	                    ON	F4102F.IBMCU = WAMCU AND
		                    F4102F.IBITM = WAITM
                    {filterString}"
            };
            return cmd;
        }

        /*********************************************************************************
        * Methods
        ********************************************************************************/
        public static RawGlassItems GetRawGlassItems(BusinessSystemSession e1Session, string plantNum = null, decimal scheduleNum = 0, decimal scheduleDateE1 = 0, string batchNum = null, decimal workOrderNum = 0, SqlDmlResponse sqlResponse = null)
        {
            var rawGlassItems = new RawGlassItems();
            var filters = new List<string>();
            var parameters = new List<iDB2Parameter>();
            try
            {
                // Part Type
                filters.Add("F4102R.IBGLPT IN ('GLAS', 'GLAF', 'GLAC' ) AND \r\n");
                // MTS Items
                filters.Add("F580020.S6Z58SFS IN ('6', '7') AND \r\n");
                // Plant Num
                if (!string.IsNullOrEmpty(plantNum) && !plantNum.Equals("0"))
                {
                    filters.Add("S7MCU = ? AND \r\n");
                    parameters.Add(new iDB2Parameter("PlantNum", iDB2DbType.iDB2Graphic, 12));
                    parameters.Last().Value = plantNum.PadLeft(12);
                }
                // Schedule Num
                if (scheduleNum > 0)
                {
                    filters.Add("S7Z58SCN = ? AND \r\n");
                    parameters.Add(new iDB2Parameter("SchedNum", iDB2DbType.iDB2Decimal));
                    parameters.Last().Precision = 5;
                    parameters.Last().Scale = 0;
                    parameters.Last().Value = scheduleNum;
                }
                // Batch Num
                if (!string.IsNullOrEmpty(batchNum) && !batchNum.Equals("0"))
                {
                    filters.Add("S7EDBT = ? AND \r\n");
                    parameters.Add(new iDB2Parameter("BatchNum", iDB2DbType.iDB2Graphic, 15));
                    parameters.Last().Value = batchNum;
                }
                // Schedule Date
                if (scheduleDateE1 > 0)
                {
                    filters.Add("S7Z58SCD = ? AND \r\n");
                    parameters.Add(new iDB2Parameter("SchedDate", iDB2DbType.iDB2Decimal));
                    parameters.Last().Precision = 6;
                    parameters.Last().Scale = 0;
                    parameters.Last().Value = scheduleDateE1;
                }
                // Work Order Num
                if (workOrderNum > 0)
                {
                    filters.Add("S7DOCO = ? AND \r\n");
                    parameters.Add(new iDB2Parameter("WorkOrderNum", iDB2DbType.iDB2Graphic, 8));
                    parameters.Last().Value = workOrderNum.ToString(CultureInfo.InvariantCulture);
                }
                filters[filters.Count - 1] = filters.Last().ToUpper().Replace(" AND", "");
                var cmd = GetRawGlassDynamic(filters);
                foreach (var parameter in parameters)
                {
                    cmd.Parameters.Add(parameter);
                }
                cmd.Connection = e1Session.DbConnection;
                if (sqlResponse == null) sqlResponse = new SqlDmlResponse();
                sqlResponse.Init(cmd);
                Logger.Info(Logging.DbCmdString(cmd));
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
                while (reader.Read())
                {
                    var rawGlassItem = new RawGlassItem
                    {
                        WMCPIT = CastDbNull.To<decimal>(reader["WMCPIT"], 0),
                        WMCMCU = CastDbNull.To(reader["WMCMCU"], string.Empty).Trim(),
                        WMDOCO = CastDbNull.To<decimal>(reader["WMDOCO"], 0),
                        WMDCTO = CastDbNull.To(reader["WMDCTO"], string.Empty).Trim(),
                        WMDSC1 = CastDbNull.To(reader["WMDSC1"], string.Empty).Trim(),
                        WMDSC2 = CastDbNull.To(reader["WMDSC2"], string.Empty).Trim(),
                        WMLOCN = CastDbNull.To(reader["WMLOCN"], string.Empty).Trim(),
                        WMUORG = CastDbNull.To<decimal>(reader["WMUORG"], 0),
                        WMTRQT = CastDbNull.To<decimal>(reader["WMTRQT"], 0),
                        WMUKID = CastDbNull.To<decimal>(reader["WMUKID"], 0),
                        WMOPSQ = CastDbNull.To<decimal>(reader["WMOPSQ"], 0),
                        WMOPSQ_RN = CastDbNull.To<Int64>(reader["WMOPSQ_RN"], 0),
                        WMUPMJ = CastDbNull.To<decimal>(reader["WMUPMJ"], 0),
                        WMTDAY = CastDbNull.To<decimal>(reader["WMTDAY"], 0),
                        CRZ58L1TK = CastDbNull.To<decimal>(reader["CRZ58L1TK"], 0),
                        CRZ58L1HT = CastDbNull.To(reader["CRZ58L1HT"], string.Empty).Trim(),
                        CRZ58L1CL = CastDbNull.To(reader["CRZ58L1CL"], string.Empty).Trim(),
                        CRZ58L11C = CastDbNull.To(reader["CRZ58L11C"], string.Empty).Trim(),
                        CRZ58L12C = CastDbNull.To(reader["CRZ58L12C"], string.Empty).Trim(),
                        CRZ58L1PX = CastDbNull.To(reader["CRZ58L1PX"], string.Empty).Trim(),
                        CRZ58L1BDM = CastDbNull.To<decimal>(reader["CRZ58L1BDM"], 0),
                        CRZ58L1BNM = CastDbNull.To<decimal>(reader["CRZ58L1BNM"], 0),
                        CRZ58L1BDN = CastDbNull.To<decimal>(reader["CRZ58L1BDN"], 0),
                        CRZ58L1LDM = CastDbNull.To<decimal>(reader["CRZ58L1LDM"], 0),
                        CRZ58L1LNM = CastDbNull.To<decimal>(reader["CRZ58L1LNM"], 0),
                        CRZ58L1LDN = CastDbNull.To<decimal>(reader["CRZ58L1LDN"], 0),
                        CRZ58L1RDM = CastDbNull.To<decimal>(reader["CRZ58L1RDM"], 0),
                        CRZ58L1RNM = CastDbNull.To<decimal>(reader["CRZ58L1RNM"], 0),
                        CRZ58L1RDN = CastDbNull.To<decimal>(reader["CRZ58L1RDN"], 0),
                        CRZ58L1TDM = CastDbNull.To<decimal>(reader["CRZ58L1TDM"], 0),
                        CRZ58L1TNM = CastDbNull.To<decimal>(reader["CRZ58L1TNM"], 0),
                        CRZ58L1TDN = CastDbNull.To<decimal>(reader["CRZ58L1TDN"], 0),
                        CRZ58SHAPE = CastDbNull.To(reader["CRZ58SHAPE"], string.Empty).Trim(),
                        IBGLPT = CastDbNull.To(reader["IBGLPT"], string.Empty).Trim(),
                        S7EDBT = CastDbNull.To(reader["S7EDBT"], string.Empty).Trim(),
                        S7ITM = CastDbNull.To<decimal>(reader["S7ITM"], 0),
                        S7Z58SCN = CastDbNull.To<decimal>(reader["S7Z58SCN"], 0),
                        IBSRP4 = CastDbNull.To(reader["IBSRP4"], string.Empty).Trim(),
                    };
                    rawGlassItems.Add(rawGlassItem);
                }
                reader.Close();
                sqlResponse.RowsAffected = rawGlassItems.Count;
                sqlResponse.EndTime = DateTime.Now.ToLocalTime();
                sqlResponse.ElapsedSeconds = Convert.ToDecimal((sqlResponse.EndTime - sqlResponse.StartTime).TotalSeconds);
            }
            catch (Exception e)
            {
                Logger.Error($@"ERROR: {e1Session.DbConnection.ConnectionString}  {e.Message}: {e.StackTrace}");
                throw new Exception($@"{e1Session.DbConnection.ConnectionString}  {e.Message}: {e.StackTrace}");
            }
            return rawGlassItems;
        }
    }
}
