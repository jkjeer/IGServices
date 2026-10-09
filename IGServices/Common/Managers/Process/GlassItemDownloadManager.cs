using System;
using System.Collections.Generic;
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
    public class GlassItemDownloadManager
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public GlassItemDownloadManager(BusinessSystemSession session)
        {
            var hash = new Dictionary<string, iDB2Command>
            {
                {"GlassItemDownloadManager.GetItemDefinition", GetItemDefinition()},
                {"GlassItemDownloadManager.GetCustomerPart", GetCustomerPart()},
                {"GlassItemDownloadManager.GetPrimaryLocation", GetPrimaryLocation()},
                {"GlassItemDownloadManager.GetQuantity", GetQuantity()},
                {"GlassItemDownloadManager.GetWeight", GetWeight()},
                {"GlassItemDownloadManager.GetCost", GetCost()}
            };
            foreach (var keyName in hash.Keys.Where(keyName => !session.DbCommands.ContainsKey(keyName)))
            {
                session.DbCommands.Add(keyName, hash[keyName]);
            }
        }

        public ProcessResponse Process(BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Plant plant, TimeSpan timeSpan, bool useHighWaterMark, decimal itemNum, List<string> stockTypes)
        {
            var response = new ProcessResponse("Glass Item Download Manager");
            var table = new GlassItemTable();

            response.Plant = plant;
            response.ConnectionStrings.Add(new ConnectionString("E1", session.DbConnection.ConnectionString));
            foreach (var sqlCn in sqlCns)
            {
                response.ConnectionStrings.Add(new ConnectionString(sqlCn.Key, sqlCn.Value.ConnectionString));
            }

            ProcessRunSpan processRunSpan = new ProcessRunSpan();
            HighWaterMark highWaterMark = new HighWaterMark();

            /********************************************************/
            var step1 = new ProcessStep("Resolve Run Span");
            try
            {
                highWaterMark = (useHighWaterMark) ? HighWaterMarkManager.GetHighWaterMark(sqlCns["PLANT.SHOPFLOOR"], plant, "GlassItem") : null;
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
                var step2a = new ProcessStep("Get Definition Keys");
                try
                {
                    table = GetDefinitionKeys(session, sqlCns["PLANT.SHOPFLOOR"], plant, processRunSpan, itemNum, stockTypes);
                    step2a.RowsAffected = table.Rows.Count;
                }
                catch (Exception e)
                {
                    step2a.Errors.Add(new Error(e));
                }
                finally
                {
                    step2a.Done();
                    response.ProcessSteps.Add(step2a);
                }

                var step2b = new ProcessStep("Get Balance Keys");
                try
                {
                    DataTable table1;
                    if (processRunSpan.HighWaterMark.Exists)
                    {
                        table1 = GetBalanceKeysLedger(session, sqlCns["PLANT.SHOPFLOOR"], plant, processRunSpan,
                            itemNum, stockTypes);
                        step2b.RowsAffected = table1.Rows.Count;
                    }
                    else
                    {
                        table1 = GetBalanceKeys(session, sqlCns["PLANT.SHOPFLOOR"], plant, processRunSpan,
                            itemNum, stockTypes);
                        step2b.RowsAffected = table1.Rows.Count;
                    }

                    foreach (DataRow dr1 in table1.Rows)
                    {
                        var balanceItemNum = (decimal)dr1["ItemNum"];
                        if (table.AsEnumerable().All(row => balanceItemNum != row.Field<decimal>("ItemNum")))
                        {
                            DataRow dr = table.NewRow();
                            dr["ItemNum"] = dr1["ItemNum"];
                            dr["TransactionId"] = dr1["TransactionId"];
                            dr["TransactionDate"] = dr1["TransactionDate"];
                            table.Rows.Add(dr);
                        }
                    }
                }
                catch (Exception e)
                {
                    step2b.Errors.Add(new Error(e));
                }
                finally
                {
                    step2b.Done();
                    response.ProcessSteps.Add(step2b);
                }


                var step2c = new ProcessStep("Get Item Definition");
                try
                {
                    GetItemDefinition(session, plant, table);
                    step2c.RowsAffected = table.Rows.Count;
                    response.RowsAffected = table.Rows.Count;
                }
                catch (Exception e)
                {
                    step2c.Errors.Add(new Error(e));
                }
                finally
                {
                    step2c.Done();
                    response.ProcessSteps.Add(step2c);
                }

                var step2d = new ProcessStep("Get Quantity");
                try
                {
                    GetQuantity(session, plant, table);
                    step2d.RowsAffected = table.Rows.Count;
                }
                catch (Exception e)
                {
                    step2d.Errors.Add(new Error(e));
                }
                finally
                {
                    step2d.Done();
                    response.ProcessSteps.Add(step2d);
                }

                var step2e = new ProcessStep("Get Cost");
                try
                {
                    GetCost(session, plant, table);
                    step2e.RowsAffected = table.Rows.Count;
                }
                catch (Exception e)
                {
                    step2e.Errors.Add(new Error(e));
                }
                finally
                {
                    step2e.Done();
                    response.ProcessSteps.Add(step2e);
                }

                var step2f = new ProcessStep("Get Customer Part");
                try
                {
                    GetCustomerPart(session, plant, table);
                    step2f.RowsAffected = table.Rows.Count;
                }
                catch (Exception e)
                {
                    step2f.Errors.Add(new Error(e));
                }
                finally
                {
                    step2f.Done();
                    response.ProcessSteps.Add(step2f);
                }

                var step2g = new ProcessStep("Get Primary Location");
                try
                {
                    GetPrimaryLocation(session, plant, table);
                    step2g.RowsAffected = table.Rows.Count;
                }
                catch (Exception e)
                {
                    step2g.Errors.Add(new Error(e));
                }
                finally
                {
                    step2g.Done();
                    response.ProcessSteps.Add(step2g);
                }

                var step2h = new ProcessStep("Get Weight");
                try
                {
                    GetWeight(session, plant, table);
                    step2h.RowsAffected = table.Rows.Count;
                }
                catch (Exception e)
                {
                    step2h.Errors.Add(new Error(e));
                }
                finally
                {
                    step2h.Done();
                    response.ProcessSteps.Add(step2h);
                }

                /********************************************************/
                if (table.Rows.Count > 0)
                {
                    var step3 = new ProcessStep("Bulk Copy");
                    SqlBulkCopyResponse bulkResponse = new SqlBulkCopyResponse();
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
                if (useHighWaterMark)
                {
                    var step4 = new ProcessStep("Update High Water Mark");
                    try
                    {
                        if (table.Rows.Count > 0)
                        {
                            highWaterMark.LocalDateTime = table.MaxDate.AddHours(plant.TimeOffset);
                            highWaterMark.EnterpriseDateTime = table.MaxDate; 
                            highWaterMark.UniqueID = (table.MaxId > highWaterMark.UniqueID)
                                ? (long)table.MaxId
                                : highWaterMark.UniqueID;
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
            response.Done();
            Logging.InsertProcessLog(response);
            return response;
        }

        /******************************************************************
         * Definition Keys
         ******************************************************************/
        public iDB2Command GetDefinitionKeys(List<string> filters)
        {
            var filterString = new StringBuilder();
            foreach (string filter in filters)
            {
                filterString.Append(filter);
            }

            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = 0;
            cmd.CommandText = String.Format(@"
            SELECT * FROM (
            SELECT 
	            IBMCU,
	            IBITM,
                IBUPMJ,
                IBTDAY,
                ROW_NUMBER() OVER (PARTITION BY IBMCU, IBITM ORDER BY IBUPMJ DESC, IBTDAY DESC) AS RN
            FROM 
                F4102
	            INNER JOIN F4101 
	            ON	IMITM = IBITM
	            LEFT OUTER JOIN F30026
	            ON	IEITM = IBITM AND
		            IEMMCU = IBMCU
	            LEFT OUTER JOIN F41002
	            ON	UMITM = IBITM AND
                    UMMCU = IBMCU
	            LEFT OUTER JOIN F4104
	            ON	IVXRT = 'C' AND
                    IVITM = IBITM AND
                    IVAN8 = IBANPL
            WHERE 
                {0} IBGLPT IN ('GLAS','GLAF','GLAX','GLAZ','COMA')) A
            WHERE A.RN = 1
            FOR READ ONLY", filterString);
            return cmd;
        }

        public GlassItemTable GetDefinitionKeys(BusinessSystemSession session, SqlConnection cn, Plant plant, ProcessRunSpan span, decimal itemNum, List<string> stockTypes)
        {
            var table = new GlassItemTable();
            var cmd = new iDB2Command();
            List<string> filters = new List<string>();
            List<iDB2Parameter> parameters = new List<iDB2Parameter>();
            try
            {
                // Plant
                if (!String.IsNullOrEmpty(plant.PlantCode))
                {
                    filters.Add("IBMCU = ? AND\r\n");
                    parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
                    parameters.Last().Value = plant.PlantNum.PadLeft(12);
                }

                // Item Num
                if (itemNum > 0)
                {
                    filters.Add("IBITM = ? AND \r\n");
                    parameters.Add(new iDB2Parameter("ITM", iDB2DbType.iDB2Decimal));
                    parameters.Last().Precision = 8;
                    parameters.Last().Scale = 0;
                    parameters.Last().Value = itemNum;
                }

                // Stock Types
                if (stockTypes.Any())
                {
                    string strInClause = "";
                    foreach (string stockType in stockTypes)
                    {
                        strInClause = strInClause + "?,";
                        parameters.Add(new iDB2Parameter("stockType", iDB2DbType.iDB2Graphic, 1));
                        parameters.Last().Value = stockType;
                    }
                    filters.Add("IBSTKT IN (" + strInClause.Substring(0, strInClause.Length - 1) + ") AND\r\n");
                }

                // Process Run Span
                if (!span.IsEmpty)
                {
                    var queryString1 = E1TimeManager.GetE1DateTimeQueryString("IBUPMJ", "IBTDAY", span.E1TimeIdSpan);
                    var queryString2 = E1TimeManager.GetE1DateTimeQueryString("IMUPMJ", "IMTDAY", span.E1TimeIdSpan);
                    var queryString3 = E1TimeManager.GetE1DateTimeQueryString("IEUPMJ", "IETDAY", span.E1TimeIdSpan);
                    var queryString4 = E1TimeManager.GetE1DateTimeQueryString("UMUPMJ", "UMTDAY", span.E1TimeIdSpan);
                    var queryString5 = E1TimeManager.GetE1DateTimeQueryString("IVUPMJ", "IVTDAY", span.E1TimeIdSpan);
                    filters.Add("(" + queryString1 + " OR " + queryString2 + " OR " + queryString3 + " OR " + queryString4 + " OR " + queryString5 + ")" + " AND \r\n");
                }

                cmd = GetDefinitionKeys(filters);
                foreach (iDB2Parameter parameter in parameters)
                {
                    cmd.Parameters.Add(parameter);
                }
                cmd.Connection = session.DbConnection;
                _logger.Debug(Logging.DbCmdString(cmd));

                var reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
                while (reader.Read())
                {
                    DataRow dr = table.NewRow();
                    dr["ItemNum"] = CastDbNull.To<decimal>(reader["IBITM"], 0);
                    dr["TransactionId"] = (decimal)0;
                    dr["TransactionDate"] = E1TimeManager.CastToDateTime(CastDbNull.To<decimal>(reader["IBUPMJ"], 0),
                        CastDbNull.To<decimal>(reader["IBTDAY"], 0)).AddHours(plant.TimeOffset);
                    table.Rows.Add(dr);
                }
                reader.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return table;
        }

        /******************************************************************
          * Balance Keys
          ******************************************************************/
        public iDB2Command GetBalanceKeys(List<string> filters)
        {
            var filterString = new StringBuilder();
            foreach (string filter in filters)
            {
                filterString.Append(filter);
            }

            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = 0;
            cmd.CommandText = $@"
            SELECT * FROM (
            SELECT 
	            LIMCU,
	            LIITM,
                LIUPMJ,
                LITDAY,
                ROW_NUMBER() OVER (PARTITION BY LIMCU, LIITM ORDER BY LIUPMJ DESC LITDAY DESC) AS RN
            FROM 
                F41021
	            INNER JOIN F4102
	            ON	LIMCU = IBMCU AND
                    LIITM = IBITM
            WHERE 
                {filterString} LIGLPT IN ('GLAS','GLAF','GLAX','GLAZ','COMA')) A
            WHERE A.RN = 1
            FOR READ ONLY";
            return cmd;
        }


        public GlassItemTable GetBalanceKeys(BusinessSystemSession session, SqlConnection cn, Plant plant, ProcessRunSpan span, decimal itemNum, List<string> stockTypes)
        {
            var table = new GlassItemTable();
            var cmd = new iDB2Command();
            List<string> filters = new List<string>();
            List<iDB2Parameter> parameters = new List<iDB2Parameter>();
            try
            {
                // Plant
                filters.Add("LIMCU = ? AND\r\n");
                parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
                parameters.Last().Value = plant.PlantNum.PadLeft(12);

                // Item Num
                if (itemNum > 0)
                {
                    filters.Add("LIITM = ? AND \r\n");
                    parameters.Add(new iDB2Parameter("ITM", iDB2DbType.iDB2Decimal));
                    parameters.Last().Precision = 8;
                    parameters.Last().Scale = 0;
                    parameters.Last().Value = itemNum;
                }

                // Stock Types
                if (stockTypes != null && stockTypes.Any())
                {
                    string strInClause = "";
                    foreach (string stockType in stockTypes)
                    {
                        strInClause = strInClause + "?,";
                        parameters.Add(new iDB2Parameter("stockType", iDB2DbType.iDB2Graphic, 1));
                        parameters.Last().Value = stockType;
                    }
                    filters.Add("IBSTKT IN (" + strInClause.Substring(0, strInClause.Length - 1) + ") AND\r\n");
                }

                // Process Run Span
                if (!span.IsEmpty)
                {
                    var queryString1 = E1TimeManager.GetE1DateTimeQueryString("LIUPMJ", "LITDAY", span.E1TimeIdSpan);
                    filters.Add(queryString1 + " AND \r\n");
                }

                cmd = GetBalanceKeys(filters);
                foreach (iDB2Parameter parameter in parameters)
                {
                    cmd.Parameters.Add(parameter);
                }
                cmd.Connection = session.DbConnection;
                _logger.Debug(Logging.DbCmdString(cmd));

                var reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
                while (reader.Read())
                {
                    DataRow dr = table.NewRow();
                    dr["ItemNum"] = CastDbNull.To<decimal>(reader["LIITM"], 0);
                    dr["TransactionId"] = (decimal)0;
                    dr["TransactionDate"] = E1TimeManager.CastToDateTime(CastDbNull.To<decimal>(reader["LIUPMJ"], 0),
                        CastDbNull.To<decimal>(reader["LITDAY"], 0)).AddHours(plant.TimeOffset);
                    table.Rows.Add(dr);
                }
                reader.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return table;
        }

        public iDB2Command GetBalanceKeysLedger(List<string> filters)
        {
            var filterString = new StringBuilder();
            foreach (string filter in filters)
            {
                filterString.Append(filter);
            }

            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = 0;
            cmd.CommandText = $@"
            SELECT * FROM (
            SELECT 
	            LLMCU,
	            LLITM,
                LLMUPM,
                LLUPMT,
                LLUK01,
                ROW_NUMBER() OVER (PARTITION BY LLMCU, LLITM ORDER BY LLUK01 DESC) AS RN
            FROM 
                F584111
                INNER JOIN F4102
                ON  IBITM = LLITM AND
                    IBMCU = LLMCU
            WHERE 
                {filterString} LLGLPT IN ('GLAS','GLAF','GLAX','GLAZ','COMA')) A
            WHERE A.RN = 1
            FOR READ ONLY";
            return cmd;
        }

        public GlassItemTable GetBalanceKeysLedger(BusinessSystemSession session, SqlConnection cn, Plant plant, ProcessRunSpan span, decimal itemNum, List<string> stockTypes)
        {
            var table = new GlassItemTable();
            var cmd = new iDB2Command();
            List<string> filters = new List<string>();
            List<iDB2Parameter> parameters = new List<iDB2Parameter>();
            try
            {
                // Plant
                filters.Add("LLMCU = ? AND\r\n");
                parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
                parameters.Last().Value = plant.PlantNum.PadLeft(12);

                filters.Add("LLUK01 > ? AND \r\n");
                parameters.Add(new iDB2Parameter("ITM", iDB2DbType.iDB2Decimal));
                parameters.Last().Precision = 15;
                parameters.Last().Scale = 0;
                parameters.Last().Value = span.HighWaterMark.UniqueID;

                cmd = GetBalanceKeysLedger(filters);
                foreach (iDB2Parameter parameter in parameters)
                {
                    cmd.Parameters.Add(parameter);
                }
                cmd.Connection = session.DbConnection;
                _logger.Debug(Logging.DbCmdString(cmd));

                var reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
                while (reader.Read())
                {
                    DataRow dr = table.NewRow();
                    dr["ItemNum"] = CastDbNull.To<decimal>(reader["LLITM"], 0);
                    dr["TransactionId"] = CastDbNull.To<decimal>(reader["LLUK01"], 0);
                    dr["TransactionDate"] = E1TimeManager.CastToDateTime(CastDbNull.To<decimal>(reader["LLMUPM"], 0),
                        CastDbNull.To<decimal>(reader["LLUPMT"], 0)).AddHours(plant.TimeOffset);
                    table.Rows.Add(dr);
                }
                reader.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return table;
        }

        /******************************************************************
         * Item Definition
         ******************************************************************/
        public static iDB2Command GetItemDefinition()
        {
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = 0;
            cmd.CommandText = @"
            SELECT 
	            IBMCU,
	            IBITM,
	            IMSRP4,
	            IMDSC1,
	            IMDSC2,
	            IMUOM1,
	            IBSTKT,
	            IBGLPT,
	            IBVEND,
	            AB2.ABALPH AS VENDALPH,
	            IBANPL,
	            AB1.ABALPH AS ANPLALPH,
	            IMUPMJ,
	            IMTDAY,
	            IBUPMJ,
	            IBTDAY
            FROM 
	            F4102 
	            INNER JOIN F4101 
	            ON IMITM = IBITM
	            LEFT OUTER JOIN F0101 AB1
	            ON AB1.ABAN8 = IBANPL
	            LEFT OUTER JOIN F0101 AB2
	            ON AB2.ABAN8 = IBVEND
            WHERE
                IBMCU = ? AND
                IBITM = ?
            FOR READ ONLY";
            cmd.Parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("ITM", iDB2DbType.iDB2Decimal));
            cmd.Parameters["ITM"].Precision = 8;
            cmd.Parameters["ITM"].Scale = 0;
            return cmd;
        }

        public void GetItemDefinition(BusinessSystemSession session, Plant plant, DataTable dt)
        {
            try
            {
                var cmd = session.DbCommands["GlassItemDownloadManager.GetItemDefinition"];
                cmd.Connection = session.DbConnection;
                foreach (DataRow dr in dt.Rows)
                {
                    cmd.Parameters[0].Value = plant.PlantNum.PadLeft(12);
                    cmd.Parameters[1].Value = (decimal)dr["ItemNum"];
                    IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
                    while (reader.Read())
                    {
                        dr["UnitType"] = CastDbNull.To<string>(reader["IMSRP4"], String.Empty).Trim();
                        dr["Description1"] = CastDbNull.To<string>(reader["IMDSC1"], String.Empty).Trim();
                        dr["Description2"] = CastDbNull.To<string>(reader["IMDSC2"], String.Empty).Trim();
                        dr["StockType"] = CastDbNull.To<string>(reader["IBSTKT"], String.Empty).Trim();
                        dr["GlCategory"] = CastDbNull.To<string>(reader["IBGLPT"], String.Empty).Trim();
                        dr["CustomerNum"] = CastDbNull.To<decimal>(reader["IBANPL"], 0);
                        dr["CustomerName"] = CastDbNull.To<string>(reader["ANPLALPH"], String.Empty).Trim();
                        dr["PrimaryUom"] = CastDbNull.To<string>(reader["IMUOM1"], String.Empty).Trim();
                        dr["SupplierNum"] = CastDbNull.To<decimal>(reader["IBVEND"], 0);
                        dr["SupplierName"] = CastDbNull.To<string>(reader["VENDALPH"], String.Empty).Trim();


                        var imTime = E1TimeManager.CastToDateTime(CastDbNull.To<decimal>(reader["IMUPMJ"], 0),
                            CastDbNull.To<decimal>(reader["IMTDAY"], 0)).AddHours(plant.TimeOffset);
                        if (imTime > (DateTime)dr["TransactionDate"])
                        {
                            dr["TransactionDate"] = imTime;
                        }

                        var ibTime = E1TimeManager.CastToDateTime(CastDbNull.To<decimal>(reader["IBUPMJ"], 0),
                            CastDbNull.To<decimal>(reader["IBTDAY"], 0)).AddHours(plant.TimeOffset);
                        if (ibTime > (DateTime)dr["TransactionDate"])
                        {
                            dr["TransactionDate"] = ibTime;
                        }

                        //dr["TransactionId"] = (decimal)0;

                    }
                    reader.Close();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /******************************************************************
         * Customer Part
         ******************************************************************/
        public static iDB2Command GetCustomerPart()
        {
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = 0;
            cmd.CommandText = @"
            SELECT 
                IVCITM,
                IVUPMJ,
                IVTDAY
            FROM 
                F4104
	        WHERE  
		        IVXRT = 'C' AND
		        IVEXDJ >= ? AND
		        IVEFTJ <= ? AND
                IVITM = ? AND
		        IVAN8 = ?
            FOR READ ONLY";
            cmd.Parameters.Add(new iDB2Parameter("EXDJ", iDB2DbType.iDB2Decimal));
            cmd.Parameters["EXDJ"].Precision = 6;
            cmd.Parameters["EXDJ"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("EFTJ", iDB2DbType.iDB2Decimal));
            cmd.Parameters["EFTJ"].Precision = 6;
            cmd.Parameters["EFTJ"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("ITM", iDB2DbType.iDB2Decimal));
            cmd.Parameters["ITM"].Precision = 8;
            cmd.Parameters["ITM"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("AN8", iDB2DbType.iDB2Decimal));
            cmd.Parameters["AN8"].Precision = 8;
            cmd.Parameters["AN8"].Scale = 0;
            return cmd;
        }

        public void GetCustomerPart(BusinessSystemSession session, Plant plant, DataTable dt)
        {
            try
            {
                var cmd = session.DbCommands["GlassItemDownloadManager.GetCustomerPart"];
                cmd.Connection = session.DbConnection;
                foreach (DataRow dr in dt.Rows)
                {
                    try
                    {
                        cmd.Parameters[0].Value = E1TimeManager.CastToE1Date(DateTime.Now.Date);
                        cmd.Parameters[1].Value = E1TimeManager.CastToE1Date(DateTime.Now.Date);
                        cmd.Parameters[2].Value = (decimal) dr["ItemNum"];
                        cmd.Parameters[3].Value = (decimal) dr["CustomerNum"];
                        IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
                        while (reader.Read())
                        {
                            dr["CustomerPartNum"] = CastDbNull.To<string>(reader["IVCITM"], String.Empty).Trim();
                            var ivTime = E1TimeManager.CastToDateTime(CastDbNull.To<decimal>(reader["IVUPMJ"], 0),
                                CastDbNull.To<decimal>(reader["IVTDAY"], 0)).AddHours(plant.TimeOffset);
                            ;
                            if (ivTime > (DateTime) dr["TransactionDate"])
                            {
                                dr["TransactionDate"] = ivTime;
                                ;
                            }
                        }

                        reader.Close();
                    }
                    catch (Exception e)
                    {
                        throw new Exception(e.Message);
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /******************************************************************
         * Primary Location
         ******************************************************************/
        public static iDB2Command GetPrimaryLocation()
        {
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = 0;
            cmd.CommandText = @"
            SELECT
                LILOCN, 
                LIUPMJ,
                LITDAY
            FROM
                F41021
            WHERE
                LIPBIN = 'P' AND
                LIMCU = ? AND
                LIITM = ?
            FOR READ ONLY";
            cmd.Parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("ITM", iDB2DbType.iDB2Decimal));
            cmd.Parameters["ITM"].Precision = 8;
            cmd.Parameters["ITM"].Scale = 0;
            return cmd;
        }

        public void GetPrimaryLocation(BusinessSystemSession session, Plant plant, DataTable dt)
        {
            try
            {
                var cmd = session.DbCommands["GlassItemDownloadManager.GetPrimaryLocation"];
                cmd.Connection = session.DbConnection;
                foreach (DataRow dr in dt.Rows)
                {
                    cmd.Parameters[0].Value = plant.PlantNum.PadLeft(12);
                    cmd.Parameters[1].Value = (decimal)dr["ItemNum"];
                    IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
                    while (reader.Read())
                    {
                        dr["PrimaryLocation"] = CastDbNull.To<string>(reader["LILOCN"], String.Empty).Trim();
                        dr["PrimaryLocationFull"] = CastDbNull.To<string>(reader["LILOCN"], String.Empty);
                        var liTime = E1TimeManager.CastToDateTime(CastDbNull.To<decimal>(reader["LIUPMJ"], 0),
                            CastDbNull.To<decimal>(reader["LITDAY"], 0)).AddHours(plant.TimeOffset); ;
                        if (liTime > (DateTime)dr["TransactionDate"])
                        {
                            dr["TransactionDate"] = liTime; ;
                        }
                    }
                    reader.Close();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /******************************************************************
         * Quantity
         ******************************************************************/
        public static iDB2Command GetQuantity1()
        {
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = 0;
            cmd.CommandText = @"
            SELECT
                SUM(LIPQOH / 1000) AS LIPQOH,
                MAX(LIUPMJ*1000000+LITDAY) AS LIUPMJTDAY
            FROM
                F41021
            WHERE
                LIPQOH != 0 AND
                LIMCU = ? AND
                LIITM = ?
			HAVING 
                SUM(LIPQOH) IS NOT NULL
            FOR READ ONLY";
            cmd.Parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("ITM", iDB2DbType.iDB2Decimal));
            cmd.Parameters["ITM"].Precision = 8;
            cmd.Parameters["ITM"].Scale = 0;
            return cmd;
        }

        public static iDB2Command GetQuantity()
        {
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = 0;
            cmd.CommandText = @"
            SELECT
                SUM(RDPQOH / 1000) AS LIPQOH,
                MAX(RDUPMJ*1000000+RDTDAY) AS LIUPMJTDAY
            FROM
                F580056
            WHERE
                RDPQOH != 0 AND
                RDZ58DST NOT IN ('8') AND
                RDMCU = ? AND
                RDITM = ? 
			HAVING 
                SUM(RDPQOH) IS NOT NULL
            FOR READ ONLY";
            cmd.Parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("ITM", iDB2DbType.iDB2Decimal));
            cmd.Parameters["ITM"].Precision = 8;
            cmd.Parameters["ITM"].Scale = 0;
            return cmd;
        }

        public void GetQuantity(BusinessSystemSession session, Plant plant, DataTable dt)
        {
            try
            {
                var cmd = session.DbCommands["GlassItemDownloadManager.GetQuantity"];
                cmd.Connection = session.DbConnection;
                foreach (DataRow dr in dt.Rows)
                {
                    cmd.Parameters[0].Value = plant.PlantNum.PadLeft(12);
                    cmd.Parameters[1].Value = (decimal)dr["ItemNum"];
                    IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
                    //dr["QtyOnHand"] = 0;
                    while (reader.Read())
                    {
                        dr["QtyOnHand"] = CastDbNull.To<decimal>(reader["LIPQOH"], 0);
                        var ts = CastDbNull.To<decimal>(reader["LIUPMJTDAY"], 0);
                        var liTime = E1TimeManager.CastToDateTime(Convert.ToDecimal(ts.ToString().Substring(0, 6)),
                            Convert.ToDecimal(ts.ToString().Substring(6, 6))).AddHours(plant.TimeOffset); 
                        if (liTime > (DateTime)dr["TransactionDate"])
                        {
                            dr["TransactionDate"] = liTime; ;
                        }
                    }
                    reader.Close();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /******************************************************************
         * Cost
         ******************************************************************/
        public static iDB2Command GetCost()
        {
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = 0;
            cmd.CommandText = @"
            SELECT
                SUM(IECSL / 10000) AS IECSL,
                MAX(IEUPMJ*1000000+IETDAY) AS IEUPMJTDAY
            FROM
                F30026
            WHERE
                IELEDG = '07' AND
                IEMMCU = ? AND
                IEITM = ?
            HAVING
                SUM(IECSL) IS NOT NULL
            FOR READ ONLY";
            cmd.Parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("ITM", iDB2DbType.iDB2Decimal));
            cmd.Parameters["ITM"].Precision = 8;
            cmd.Parameters["ITM"].Scale = 0;
            return cmd;
        }

        public void GetCost(BusinessSystemSession session, Plant plant, DataTable dt)
        {
            try
            {
                var cmd = session.DbCommands["GlassItemDownloadManager.GetCost"];
                cmd.Connection = session.DbConnection;
                foreach (DataRow dr in dt.Rows)
                {
                    cmd.Parameters[0].Value = plant.PlantNum.PadLeft(12);
                    cmd.Parameters[1].Value = (decimal)dr["ItemNum"];
                    IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
                    //dr["CostPerEa"] = 0;
                    while (reader.Read())
                    {
                        dr["CostPerEa"] = CastDbNull.To<decimal>(reader["IECSL"], 0);
                        var ts = CastDbNull.To<decimal>(reader["IEUPMJTDAY"], 0);
                        var ieTime = E1TimeManager.CastToDateTime(Convert.ToDecimal(ts.ToString().Substring(0, 6)),
                            Convert.ToDecimal(ts.ToString().Substring(6, 6))).AddHours(plant.TimeOffset); ;
                        if (ieTime > (DateTime)dr["TransactionDate"])
                        {
                            dr["TransactionDate"] = ieTime;
                        }
                    }

                    reader.Close();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /******************************************************************
         * Cost
         ******************************************************************/
        public static iDB2Command GetWeight()
        {
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = 0;
            cmd.CommandText = @"
            SELECT
                UMCONV / 10000000 AS UMCONV,
                UMUPMJ,
                UMTDAY
            FROM
                F41002
            WHERE
		        UMMCU = ? AND
                UMITM = ? AND
		        UMUM = 'EA' AND
		        UMRUM = 'LB'
            FOR READ ONLY";
            cmd.Parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("ITM", iDB2DbType.iDB2Decimal));
            cmd.Parameters["ITM"].Precision = 8;
            cmd.Parameters["ITM"].Scale = 0;
            return cmd;
        }

        public void GetWeight(BusinessSystemSession session, Plant plant, DataTable dt)
        {
            try
            {
                var cmd = session.DbCommands["GlassItemDownloadManager.GetWeight"];
                cmd.Connection = session.DbConnection;
                foreach (DataRow dr in dt.Rows)
                {
                    cmd.Parameters[0].Value = plant.PlantNum.PadLeft(12);
                    cmd.Parameters[1].Value = (decimal)dr["ItemNum"];
                    IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
                    //dr["LbsPerEa"] = 0;
                    while (reader.Read())
                    {
                        dr["LbsPerEa"] = CastDbNull.To<decimal>(reader["UMCONV"], 0);
                        var umTime = E1TimeManager.CastToDateTime(CastDbNull.To<decimal>(reader["UMUPMJ"], 0),
                            CastDbNull.To<decimal>(reader["UMTDAY"], 0)).AddHours(plant.TimeOffset); ;
                        if (umTime > (DateTime)dr["TransactionDate"])
                        {
                            dr["TransactionDate"] = umTime;
                        }
                    }
                    reader.Close();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

    }
}
