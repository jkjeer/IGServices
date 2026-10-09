using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
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

namespace IGServices.Common.Managers.Process
{
    public class MuntinCodeDownloadManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /********************************************************************************
        * CONSTRUCTOR
        ********************************************************************************/
        public MuntinCodeDownloadManager(BusinessSystemSession e1Session)
        {
            var hash = new Dictionary<string, iDB2Command>
            {
                {"MuntinCodeDownloadManager.GetMuntinCodes", GetMuntinCodes()}
            };
            foreach (var keyName in hash.Keys.Where(keyName => !e1Session.DbCommands.ContainsKey(keyName)))
            {
                e1Session.DbCommands.Add(keyName, hash[keyName]);
            }
        }

        /********************************************************************************
        * PROCESS
        ********************************************************************************/
        public ProcessResponse Process(BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Plant plant, TimeSpan timeSpan, bool useHighWaterMark)
        {
            var response = new ProcessResponse("Muntin Code Download Manager");
            response.Plant = plant;
            response.ConnectionStrings.Add(new ConnectionString("E1", session.DbConnection.ConnectionString));
            foreach (var sqlCn in sqlCns)
            {
                response.ConnectionStrings.Add(new ConnectionString(sqlCn.Key, sqlCn.Value.ConnectionString));
            }

            /********************************************************/
            var step1 = new ProcessStep("Resolve Run Span");
            ProcessRunSpan processRunSpan = new ProcessRunSpan();
            HighWaterMark highWaterMark = new HighWaterMark();
            try
            {
                highWaterMark = (useHighWaterMark) ? HighWaterMarkManager.GetHighWaterMark(sqlCns["PLANT.SHOPFLOOR"], plant, "MuntinCode") : null;
                processRunSpan = ProcessRunSpanManager.GetRunSpan(highWaterMark, timeSpan, E1TimeManager.GetE1TimeStamp(session));
                step1.Messages.Add(new Message("RunSpan", processRunSpan.Serialize()));
            }
            catch (Exception e)
            {
                step1.Errors.Add(new Error(e));
            }
            finally
            {
                step1.Done();
                response.AddStep(step1);
            }

            if (!response.HasErrors())
            {

                /********************************************************/
                //var step2a = new ProcessStep("Get Keys");
                //var keys = new List<string>();
                //try
                //{
                //    keys = GetMuntinCodeKeys(session, plant, processRunSpan);
                //    step2a.RowsAffected = keys.Count;
                //    response.RowsAffected = keys.Count;
                //}
                //catch (Exception e)
                //{
                //    step2a.Errors.Add(new Error(e));
                //}
                //step2a.Done();
                //response.AddStep(step2a);

                /********************************************************/
                //var step2b = new ProcessStep("Get Codes");
                //var table = new MuntinCodeTable();
                //try
                //{
                //    GetMuntinCodes(session, plant, keys, table);

                //}
                //catch (Exception e)
                //{
                //    step2b.Errors.Add(new Error(e));
                //}
                //step2b.Done();
                //response.AddStep(step2b);

                /********************************************************/
                var step2 = new ProcessStep("Get Source Data");
                var table = new MuntinCodeTable();
                try
                {
                    var reader = GetMuntinCodes(session, plant, processRunSpan);
                    table.Add(reader, plant);
                    step2.RowsAffected = table.Rows.Count;
                    response.RowsAffected = table.Rows.Count;
                }
                catch (Exception e)
                {
                    step2.Errors.Add(new Error(e));
                }
                step2.Done();
                response.AddStep(step2);

                /********************************************************/
                if (table.Rows.Count > 0)
                {
                    var step3 = new ProcessStep("Bulk Copy");
                    var bulkResponse = new SqlBulkCopyResponse();
                    try
                    {
                        if (table.Rows.Count > 0)
                        {
                            bulkResponse =
                                BulkCopyManager.BulkTable(table.ExtendedProperties["Target"].ToString(),
                                    table, sqlCns["PLANT.INTEGRATION"]);
                        }
                    }
                    catch (Exception e)
                    {
                        step3.Errors.Add(new Error(e));
                    }
                    finally
                    {
                        step3.RowsAffected = (int)bulkResponse.RowsCopied;
                        step3.SqlBulkCopyResponses.Add(bulkResponse);
                        step3.Done();
                        response.RowsAffected = step3.RowsAffected;
                        response.AddStep(step3);
                    }
                }

                /********************************************************/
                if (useHighWaterMark && !response.HasErrors())
                {
                    var step4 = new ProcessStep("Update High Water Mark");
                    try
                    {
                        if (table.Rows.Count > 0)
                        {
                            highWaterMark.LocalDateTime = table.MaxDate;
                            highWaterMark.EnterpriseDateTime = table.MaxDate.AddHours(-plant.TimeOffset);
                        }
                        step4.SqlDmlResponses.Add(HighWaterMarkManager.SaveHighWaterMark(sqlCns["PLANT.SHOPFLOOR"], highWaterMark));
                    }
                    catch (Exception e)
                    {
                        step4.Errors.Add(new Error(e));
                    }
                    finally
                    {
                        step4.Done();
                        response.AddStep(step4);
                    }
                }
            }
            response.Close();
            return response;
        }

        /********************************************************************************
        * KEYS
        ********************************************************************************/
        private static iDB2Command GetMuntinCodeKeys(IReadOnlyCollection<string> filters, string plantNum)
        {
            var filterString = new StringBuilder();
            foreach (var filter in filters)
            {
                filterString.Append(filter);
            }
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = $@"
	            SELECT
		            DRKY
	            FROM (
                    SELECT 
	                    U1.DRKY,
                        U1.DRUPMJ AS UPMJD,
                        U1.DRUPMT AS UPMTD,
			            C1.RDUPMJ AS UPMJC,
			            C1.RDTDAY AS TDAYC,
			            Q1.RDUPMJ AS UPMJQ,
			            Q1.RDTDAY AS TDAYQ
                    FROM
	                    {ConfigurationManager.AppSettings["ControlLib"]}.F0005 U1
	                    LEFT OUTER JOIN F3283 C1
	                    ON	RIGHT('00000' || TRIM(C1.RDTSV1), 5) = TRIM(U1.DRKY) AND
		                    C1.RDRTBT = 'C' AND
		                    C1.RDMCU = '' AND
		                    C1.RDTBLC = 'IGMCODEMST' 
	                    LEFT OUTER JOIN F3283 Q1
	                    ON	Q1.RDTBLC = 'M1BARS' AND
		                    RIGHT('00000' || TRIM(Q1.RDTSV1), 5)  = TRIM(U1.DRKY) AND
		                    Q1.RDMCU = '{plantNum.PadLeft(12)}'
                    WHERE
                        {filterString}
                        U1.DRSY = '57' AND
	                    U1.DRRT = 'BR' 
			            ) A 
		            GROUP BY 
			            A.DRKY
                    ORDER BY
		                1
                    FOR READ ONLY"
            };
            return cmd;
        }

        public static List<string> GetMuntinCodeKeys(BusinessSystemSession session, Plant plant, ProcessRunSpan processRunSpan)
        {
            var filters = new List<string>();
            var parameters = new List<iDB2Parameter>();
            var muntinCodes = new List<string>();
            try
            {

                // Process Run Span
                if (!processRunSpan.IsEmpty)
                {
                    var queryString1 = E1TimeManager.GetE1DateTimeQueryString("U1.DRUPMJ", "U1.DRUPMT", processRunSpan.E1TimeIdSpan);
                    var queryString2 = E1TimeManager.GetE1DateTimeQueryString("C1.RDUPMJ", "C1.RDTDAY", processRunSpan.E1TimeIdSpan);
                    var queryString3 = E1TimeManager.GetE1DateTimeQueryString("Q1.RDUPMJ", "Q1.RDTDAY", processRunSpan.E1TimeIdSpan);
                    filters.Add("(" + queryString1 + " OR " + queryString2 + " OR " + queryString3 + ")" + " AND \r\n");
                }

                // Command
                var cmd = GetMuntinCodeKeys(filters, plant.PlantNum);
                foreach (iDB2Parameter parameter in parameters)
                {
                    cmd.Parameters.Add(parameter);
                }
                cmd.Connection = session.DbConnection;
                Logger.Debug(Logging.DbCmdString(cmd));

                // Reader
                var reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
                while (reader.Read())
                {
                    muntinCodes.Add(CastDbNull.To<string>(reader["DRKY"], String.Empty));
                }
                reader.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

            return muntinCodes;
        }


        /********************************************************************************
        * DEFINITION
        ********************************************************************************/
        private static iDB2Command GetMuntinCodes(IReadOnlyCollection<string> filters, string plantNum)
        {
            var filterString = new StringBuilder();
            foreach (var filter in filters)
            {
                filterString.Append(filter);
            }
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = $@"
                SELECT
	                U1.DRKY,
	                CAST(U1.DRKY AS INT) AS DRKY_INT,
	                DRDL01,
	                DRDL02,
                    COALESCE(TRIM(C1.RDTVAL), '') AS RDTVAL1,
                    COALESCE(TRIM(C2.RDTVAL), '0') AS RDTVAL2,
                    COALESCE(TRIM(C3.RDTVAL), '0') AS RDTVAL3,
                    COALESCE(TRIM(C4.RDTVAL), '') AS RDTVAL4,
                    COALESCE(TRIM(C5.RDTVAL), '') AS RDTVAL5,
	                Q1.RDMCU,
	                Q1.RDITM,
                    U1.DRUPMJ,
                    U1.DRUPMT
                FROM
	                {ConfigurationManager.AppSettings["ControlLib"]}.F0005 U1
	                LEFT OUTER JOIN F3283 C1
	                ON	RIGHT('00000' || TRIM(C1.RDTSV1), 5) = TRIM(U1.DRKY) AND
		                C1.RDRTBT = 'C' AND
		                C1.RDMCU = '' AND
		                C1.RDTBLC = 'IGMCODEMST' AND
		                C1.RDBSEQ = 1
	                LEFT OUTER JOIN F3283 C2
	                ON	RIGHT('00000' || TRIM(C2.RDTSV1), 5)  = TRIM(U1.DRKY) AND
		                C2.RDRTBT = 'C' AND
		                C2.RDMCU = '' AND
		                C2.RDTBLC = 'IGMCODEMST' AND
		                C2.RDBSEQ = 2
	                LEFT OUTER JOIN F3283 C3
	                ON	RIGHT('00000' || TRIM(C3.RDTSV1), 5)  = TRIM(U1.DRKY) AND
		                C3.RDRTBT = 'C' AND
		                C3.RDMCU = '' AND
		                C3.RDTBLC = 'IGMCODEMST' AND
		                C3.RDBSEQ = 3
	                LEFT OUTER JOIN F3283 C4
	                ON	RIGHT('00000' || TRIM(C4.RDTSV1), 5)  = TRIM(U1.DRKY) AND
		                C4.RDRTBT = 'C' AND
		                C4.RDMCU = '' AND
		                C4.RDTBLC = 'IGMCODEMST' AND
		                C4.RDBSEQ = 4
	                LEFT OUTER JOIN F3283 C5
	                ON	RIGHT('00000' || TRIM(C5.RDTSV1), 5)  = TRIM(U1.DRKY) AND
		                C5.RDRTBT = 'C' AND
		                C5.RDMCU = '' AND
		                C5.RDTBLC = 'IGMCODEMST' AND
		                C5.RDBSEQ = 5
	                LEFT OUTER JOIN F3283 Q1
	                ON	Q1.RDTBLC = 'M1BARS' AND
		                RIGHT('00000' || TRIM(Q1.RDTSV1), 5)  = TRIM(U1.DRKY) AND
		                Q1.RDMCU = '{plantNum.PadLeft(12)}'
                WHERE
                    U1.DRSY = '57' AND
	                U1.DRRT = 'BR' AND
					-- TIME SELECTION
					EXISTS (
						SELECT 
							1 
						FROM  (
							SELECT 
								U2.DRKY
							FROM
								{ConfigurationManager.AppSettings["ControlLib"]}.F0005 U2
								LEFT OUTER JOIN F3283 C2
								ON	RIGHT('00000' || TRIM(C2.RDTSV1), 5) = TRIM(U2.DRKY) AND
									C2.RDRTBT = 'C' AND
									C2.RDMCU = '' AND
									C2.RDTBLC = 'IGMCODEMST' 
								LEFT OUTER JOIN F3283 Q2
								ON	Q2.RDTBLC = 'M1BARS' AND
									RIGHT('00000' || TRIM(Q2.RDTSV1), 5)  = TRIM(U2.DRKY) AND
									Q2.RDMCU = '          03'
							WHERE
                                {filterString}
                                U2.DRSY = '57' AND
								U2.DRRT = 'BR' 
								) A 
						WHERE 
							A.DRKY = U1.DRKY)	
							FOR READ ONLY"
            };
            return cmd;
        }

        private static iDB2Command GetMuntinCodes0(IReadOnlyCollection<string> filters, string plantNum)
        {
            var filterString = new StringBuilder();
            foreach (var filter in filters)
            {
                filterString.Append(filter);
            }
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = $@"
                SELECT
	                U1.DRKY,
	                CAST(U1.DRKY AS INT) AS DRKY_INT,
	                DRDL01,
	                DRDL02,
                    COALESCE(TRIM(C1.RDTVAL), '') AS RDTVAL1,
                    COALESCE(TRIM(C2.RDTVAL), '0') AS RDTVAL2,
                    COALESCE(TRIM(C3.RDTVAL), '0') AS RDTVAL3,
                    COALESCE(TRIM(C4.RDTVAL), '') AS RDTVAL4,
                    COALESCE(TRIM(C5.RDTVAL), '') AS RDTVAL5,
	                Q1.RDMCU,
	                Q1.RDITM,
                    U1.DRUPMJ,
                    U1.DRUPMT
                FROM
	                {ConfigurationManager.AppSettings["ControlLib"]}.F0005 U1
	                LEFT OUTER JOIN F3283 C1
	                ON	RIGHT('00000' || TRIM(C1.RDTSV1), 5) = TRIM(U1.DRKY) AND
		                C1.RDRTBT = 'C' AND
		                C1.RDMCU = '' AND
		                C1.RDTBLC = 'IGMCODEMST' AND
		                C1.RDBSEQ = 1
	                LEFT OUTER JOIN F3283 C2
	                ON	RIGHT('00000' || TRIM(C2.RDTSV1), 5)  = TRIM(U1.DRKY) AND
		                C2.RDRTBT = 'C' AND
		                C2.RDMCU = '' AND
		                C2.RDTBLC = 'IGMCODEMST' AND
		                C2.RDBSEQ = 2
	                LEFT OUTER JOIN F3283 C3
	                ON	RIGHT('00000' || TRIM(C3.RDTSV1), 5)  = TRIM(U1.DRKY) AND
		                C3.RDRTBT = 'C' AND
		                C3.RDMCU = '' AND
		                C3.RDTBLC = 'IGMCODEMST' AND
		                C3.RDBSEQ = 3
	                LEFT OUTER JOIN F3283 C4
	                ON	RIGHT('00000' || TRIM(C4.RDTSV1), 5)  = TRIM(U1.DRKY) AND
		                C4.RDRTBT = 'C' AND
		                C4.RDMCU = '' AND
		                C4.RDTBLC = 'IGMCODEMST' AND
		                C4.RDBSEQ = 4
	                LEFT OUTER JOIN F3283 C5
	                ON	RIGHT('00000' || TRIM(C5.RDTSV1), 5)  = TRIM(U1.DRKY) AND
		                C5.RDRTBT = 'C' AND
		                C5.RDMCU = '' AND
		                C5.RDTBLC = 'IGMCODEMST' AND
		                C5.RDBSEQ = 5
	                LEFT OUTER JOIN F3283 Q1
	                ON	Q1.RDTBLC = 'M1BARS' AND
		                RIGHT('00000' || TRIM(Q1.RDTSV1), 5)  = TRIM(U1.DRKY) AND
		                Q1.RDMCU = '{plantNum.PadLeft(12)}'
                WHERE
                    {filterString}
                    U1.DRSY = '57' AND
	                U1.DRRT = 'BR' 
                ORDER BY
		                1
                FOR READ ONLY"
            };
            return cmd;
        }

        public static IDataReader GetMuntinCodes(BusinessSystemSession session, Plant plant, ProcessRunSpan processRunSpan)
        {
            var filters = new List<string>();
            var parameters = new List<iDB2Parameter>();
            try
            {

                // Process Run Span
                if (!processRunSpan.IsEmpty)
                {
                    var queryString1 = E1TimeManager.GetE1DateTimeQueryString("U2.DRUPMJ", "U2.DRUPMT", processRunSpan.E1TimeIdSpan);
                    var queryString2 = E1TimeManager.GetE1DateTimeQueryString("C2.RDUPMJ", "C2.RDTDAY", processRunSpan.E1TimeIdSpan);
                    var queryString3 = E1TimeManager.GetE1DateTimeQueryString("Q2.RDUPMJ", "Q2.RDTDAY", processRunSpan.E1TimeIdSpan);
                    filters.Add("(" + queryString1 + " OR " + queryString2 + " OR " + queryString3 + ")" + " AND \r\n");
                    //filters.Add(E1TimeManager.GetE1DateTimeQueryString("U1.DRUPMJ", "U1.DRUPMT", processRunSpan.E1TimeIdSpan) + " AND \r\n");
                }

                // Command
                var cmd = GetMuntinCodes(filters, plant.PlantNum);
                foreach (iDB2Parameter parameter in parameters)
                {
                    cmd.Parameters.Add(parameter);
                }
                cmd.Connection = session.DbConnection;
                Logger.Debug(Logging.DbCmdString(cmd));

                // Reader
                return cmd.ExecuteReader(CommandBehavior.SingleResult);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /********************************************************************************
        * DEFINITION BY CODES
        ********************************************************************************/
        private static iDB2Command GetMuntinCodes()
        {
            var cmd = new iDB2Command();
            try
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = $@"
                SELECT
	                U1.DRKY,
	                CAST(U1.DRKY AS INT) AS DRKY_INT,
	                DRDL01,
	                DRDL02,
                    COALESCE(TRIM(C1.RDTVAL), '') AS RDTVAL1,
                    COALESCE(TRIM(C2.RDTVAL), '0') AS RDTVAL2,
                    COALESCE(TRIM(C3.RDTVAL), '0') AS RDTVAL3,
                    COALESCE(TRIM(C4.RDTVAL), '') AS RDTVAL4,
                    COALESCE(TRIM(C5.RDTVAL), '') AS RDTVAL5,
	                Q1.RDMCU,
	                Q1.RDITM,
                    U1.DRUPMJ,
                    U1.DRUPMT
                FROM
	                {ConfigurationManager.AppSettings["ControlLib"]}.F0005 U1
	                LEFT OUTER JOIN F3283 C1
	                ON	RIGHT('00000' || TRIM(C1.RDTSV1), 5) = TRIM(U1.DRKY) AND
		                C1.RDRTBT = 'C' AND
		                C1.RDMCU = '' AND
		                C1.RDTBLC = 'IGMCODEMST' AND
		                C1.RDBSEQ = 1
	                LEFT OUTER JOIN F3283 C2
	                ON	RIGHT('00000' || TRIM(C2.RDTSV1), 5)  = TRIM(U1.DRKY) AND
		                C2.RDRTBT = 'C' AND
		                C2.RDMCU = '' AND
		                C2.RDTBLC = 'IGMCODEMST' AND
		                C2.RDBSEQ = 2
	                LEFT OUTER JOIN F3283 C3
	                ON	RIGHT('00000' || TRIM(C3.RDTSV1), 5)  = TRIM(U1.DRKY) AND
		                C3.RDRTBT = 'C' AND
		                C3.RDMCU = '' AND
		                C3.RDTBLC = 'IGMCODEMST' AND
		                C3.RDBSEQ = 3
	                LEFT OUTER JOIN F3283 C4
	                ON	RIGHT('00000' || TRIM(C4.RDTSV1), 5)  = TRIM(U1.DRKY) AND
		                C4.RDRTBT = 'C' AND
		                C4.RDMCU = '' AND
		                C4.RDTBLC = 'IGMCODEMST' AND
		                C4.RDBSEQ = 4
	                LEFT OUTER JOIN F3283 C5
	                ON	RIGHT('00000' || TRIM(C5.RDTSV1), 5)  = TRIM(U1.DRKY) AND
		                C5.RDRTBT = 'C' AND
		                C5.RDMCU = '' AND
		                C5.RDTBLC = 'IGMCODEMST' AND
		                C5.RDBSEQ = 5
	                LEFT OUTER JOIN F3283 Q1
	                ON	Q1.RDTBLC = 'M1BARS' AND
		                RIGHT('00000' || TRIM(Q1.RDTSV1), 5)  = TRIM(U1.DRKY) AND
		                Q1.RDMCU = ?
                WHERE
                    U1.DRKY = ? AND
                    U1.DRSY = '57' AND
	                U1.DRRT = 'BR' 
                ORDER BY
		                1
                FOR READ ONLY";
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
            cmd.Parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("DRKY", iDB2DbType.iDB2Graphic, 10));
            return cmd;
        }

        //public static List<MuntinCodeTable.MuntinCode> GetMuntinCodes(BusinessSystemSession session, Plant plant, List<string> codes)
        //{
        //    try
        //    {
        //        var cmd = session.DbCommands["MuntinCodeDownloadManager.GetMuntinCodes"];
        //        cmd.Connection = session.DbConnection;
        //        foreach (var code in codes)
        //        {
        //            cmd.Parameters["MCU"].Value = plant.PlantNum;
        //            cmd.Parameters["DRKY"].Value = code;
        //            Logger.Debug(Logging.DbCmdString(cmd));
        //            table.Add(cmd.ExecuteReader(CommandBehavior.SingleResult), plant);
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        throw new Exception(e.Message);
        //    }
        //}

    }
}
