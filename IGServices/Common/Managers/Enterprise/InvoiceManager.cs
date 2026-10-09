using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using IBM.Data.DB2.iSeries;
using IGServices.Common.Types;
using IGServices.Common.Types.E1;
using IGServices.Common.Types.Enterprise;
using IGServices.Common.Utilities;

namespace IGServices.Common.Managers.Enterprise
{
    public class InvoiceManager
    {
        private CustomerManager _customerMgr;
        private GlassItemManager _glassItemMgr;

        public InvoiceManager(BusinessSystemSession e1Session)
        {
            var hash = new Dictionary<string, iDB2Command>();
            hash.Add("InvoiceManager.GetInvoice", GetInvoice());
            hash.Add("InvoiceManager.GetInvoiceLines", GetInvoiceLines());
            foreach (string keyName in hash.Keys)
            {
                if (!e1Session.DbCommands.ContainsKey(keyName))
                {
                    e1Session.DbCommands.Add(keyName, hash[keyName]);
                }
            }
            _customerMgr = new CustomerManager(e1Session);
            _glassItemMgr = new GlassItemManager(e1Session);
        }

        /******************************************************************
        * Commands
        ******************************************************************/
        public iDB2Command GetInvoice()
        {
            var cmd = new iDB2Command();
            try
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText =
                    @"SELECT 
                        SDMCU,
	                    SDDOC,
                        SDIVD,
                        SHAN8,
                        SHSHAN
                    FROM
	                    F4211
                        INNER JOIN F4201
                        ON  SHDOCO = SDDOCO AND
                            SHDCTO = SDDCTO AND
                            SHKCOO = SDKCOO
                    WHERE
	                    SDDOC = ?
                    GROUP BY
                        SDMCU,
	                    SDDOC,
                        SDIVD,
                        SHAN8,
                        SHSHAN";
                cmd.Parameters.Add(new iDB2Parameter("InvoiceNum", iDB2DbType.iDB2Decimal));
                cmd.Parameters["InvoiceNum"].Precision = 8;
                cmd.Parameters["InvoiceNum"].Scale = 0;
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
            return cmd;
        }

        public iDB2Command GetInvoiceLines2()
        {
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText =
                @"SELECT 
                    SDDOCO,
                    SDDCTO,
                    SDLNID          
                FROM 
	                F4211
                WHERE 
                    SDDOC = ? AND
                    SDLNTY = 'W'";
            cmd.Parameters.Add(new iDB2Parameter("InvoiceNum", iDB2DbType.iDB2Decimal));
            cmd.Parameters["InvoiceNum"].Precision = 8;
            cmd.Parameters["InvoiceNum"].Scale = 0;
            return cmd;
        }

        public iDB2Command GetInvoiceLines()
        {
            var cmd = new iDB2Command();
            try
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = String.Format(
                    @"SELECT 
	                    SDMCU,
	                    SDDOCO,
	                    SDDCTO,
	                    SDLNID,
	                    SDSRP1,
	                    SDAEXP,
	                    SDUORG,
	                    SDLNTY,
	                    SDITM,
	                    SDDSC1,
	                    SDDSC2,
	                    SDITWT,
	                    SDPTC,
                        SDDELN,
                        SDUPRC,
                        SDLPRC,
	                    PNPTD,
                        SDVR01,
	                    ATZ58WODC,
	                    ATZ58CP2,
	                    ATZ58CP3,
	                    ATZ58CP4,
                        IBSTKT,
                        IMSRP6,
                        DRDL01
                    FROM
	                    F4211	                    
                        LEFT OUTER JOIN F580032
	                    ON	ATDOCO = SDDOCO AND
		                    ATDCTO = SDDCTO AND
		                    ATLNID = SDLNID
	                    LEFT OUTER JOIN F0014
	                    ON	PNPTC = SDPTC
                        LEFT OUTER JOIN F4102
                        ON  IBMCU = SDMCU AND
                            IBITM = SDITM
                        LEFT OUTER JOIN F4101
                        ON  IMITM = IBITM
                        LEFT OUTER JOIN {0}.F0005
                        ON  TRIM(DRSY) = '41' AND
                            TRIM(DRRT) = '06' AND
                            TRIM(DRKY) = TRIM(IMSRP6)
                    WHERE
	                    SDDOC = ?", ConfigurationManager.AppSettings["ControlLib"]);
                cmd.Parameters.Add(new iDB2Parameter("InvoiceNum", iDB2DbType.iDB2Decimal));
                cmd.Parameters["InvoiceNum"].Precision = 8;
                cmd.Parameters["InvoiceNum"].Scale = 0;
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
            return cmd;
        }

        public Invoice GetInvoice(BusinessSystemSession e1Session, Decimal invoiceNum)
        {
            var invoice = new Invoice();
            try
            {
                var cmd = (e1Session.DbCommands.ContainsKey("GetInvoice")) ? e1Session.DbCommands["GetInvoice"] : GetInvoice();
                cmd.Parameters[0].Value = invoiceNum;
                cmd.Connection = e1Session.DbConnection;
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
                while (reader.Read())
                {
                    invoice.InvoiceNum = CastDbNull.To<Decimal>(reader["SDDOC"], 0);
                    var invoiceDateId = CastDbNull.To<Decimal>(reader["SDIVD"], 0);
                    invoice.InvoiceDate = Utilities.Common.CastToDateTime(invoiceDateId, 0);
                    invoice.PlantNum = CastDbNull.To<String>(reader["SDMCU"], String.Empty);
                    invoice.InvoiceLines = GetInvoiceLines(e1Session, invoiceNum);
                }
                reader.Close();
            }
            catch (Exception err)
            {
                throw new ApplicationException(err.ToString());
            }
            return invoice;
        }

        public InvoiceLines GetInvoiceLines(BusinessSystemSession e1Session, Decimal invoiceNum)
        {
            var invoiceLines = new InvoiceLines();
            try
            {
                var cmd = (e1Session.DbCommands.ContainsKey("GetInvoiceLines")) ? e1Session.DbCommands["GetInvoiceLines"] : GetInvoiceLines();
                cmd.Parameters[0].Value = invoiceNum;
                cmd.Connection = e1Session.DbConnection;
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
                    while (reader.Read())
                    {
                        var invoiceLine = new InvoiceLine();
                        invoiceLine.SalesOrderNum = CastDbNull.To<Decimal>(reader["SDDOCO"], 0);
                        invoiceLine.SalesOrderType = CastDbNull.To<String>(reader["SDDCTO"], String.Empty);
                        var companyNum = CastDbNull.To<String>(reader["SDKCOO"], String.Empty);
                        var lineNumId = CastDbNull.To<Decimal>(reader["SDLNID"], 0);
                        invoiceLine.SalesOrderLineNum = lineNumId / 1000;

                        invoiceLine.PlantNum = CastDbNull.To<String>(reader["SDMCU"], String.Empty).Trim();
                        invoiceLine.WorkOrderNum = CastDbNull.To<Decimal>(reader["ATZ58WODC"], 0);
                        invoiceLine.ItemNum = CastDbNull.To<Decimal>(reader["SDITM"], 0);
                        invoiceLine.StockingType = CastDbNull.To<String>(reader["IBSTKT"], String.Empty).Trim();
                        invoiceLine.LineType = CastDbNull.To<String>(reader["SDLNTY"], String.Empty).Trim();
                        invoiceLine.Description1 = CastDbNull.To<String>(reader["SDDSC1"], String.Empty).Trim();
                        invoiceLine.Description2 = CastDbNull.To<String>(reader["SDDSC2"], String.Empty).Trim();
                        invoiceLine.CustomerPo = CastDbNull.To<String>(reader["SDVR01"], String.Empty).Trim();
                        invoiceLine.CustomerPart1 = CastDbNull.To<String>(reader["ATZ58CP2"], String.Empty).Trim();
                        invoiceLine.CustomerPart2 = CastDbNull.To<String>(reader["ATZ58CP3"], String.Empty).Trim();
                        invoiceLine.CustomerPart3 = CastDbNull.To<String>(reader["ATZ58CP4"], String.Empty).Trim();
                        invoiceLine.Quantity = CastDbNull.To<Decimal>(reader["SDUORG"], 0) / 1000;
                        invoiceLine.UnitPrice = CastDbNull.To<Decimal>(reader["SDUPRC"], 0) / 10000;
                        invoiceLine.UnitListPrice = CastDbNull.To<Decimal>(reader["SDLPRC"], 0) / 10000;
                        invoiceLine.ExtendedPrice = CastDbNull.To<Decimal>(reader["SDAEXP"], 0) / 100;
                        invoiceLine.ProductType = CastDbNull.To<String>(reader["SDSRP1"], String.Empty).Trim();
                        invoiceLine.UnitOfMeasure = invoiceLine.ProductType == "NG" ? "EA" : "SF";
                        invoiceLine.ItemWeight = CastDbNull.To<Decimal>(reader["SDITWT"], 0) / 10000;
                        invoiceLine.PayTerms = CastDbNull.To<String>(reader["PNPTD"], String.Empty).Trim();

                        var glassItem = _glassItemMgr.GetGlassItem(e1Session, invoiceLine.ItemNum, invoiceLine.PlantNum, invoiceLine.SalesOrderNum, invoiceLine.SalesOrderType, lineNumId, companyNum);
                        invoiceLine.Item = glassItem;

                        var drdl01 = CastDbNull.To<String>(reader["DRDL01"], String.Empty).Trim();
                        invoiceLine.HarmonizingCode = invoiceLine.StockingType == "C" ? glassItem.ItemAttributes.GetStringAttributeDesc1("Z5852240") : drdl01;

                        invoiceLines.Add(invoiceLine);
                    }
                    reader.Close();
            }
            catch (Exception err)
            {
                throw new ApplicationException(err.ToString());
            }
            return invoiceLines;
        }


        public iDB2Command GetInvoicesByLoad()
        {
            var cmd = new iDB2Command();
            try
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText =
                    @"SELECT 
                        SDMCU,
	                    SDDOC,
                        SHAN8,
                        SHSHAN
                    FROM
	                    F4211
                        INNER JOIN F4201
                        ON  SHDOCO = SDDOCO AND
                            SHDCTO = SDDCTO AND
                            SHKCOO = SDKCOO
                    WHERE
	                    SDPSN = ?
                    GROUP BY
                        SDMCU,
	                    SDDOC,
                        SHAN8,
                        SHSHAN";
                cmd.Parameters.Add(new iDB2Parameter("LoadNum", iDB2DbType.iDB2Decimal));
                cmd.Parameters["LoadNum"].Precision = 8;
                cmd.Parameters["LoadNum"].Scale = 0;
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
            return cmd;
        }


        public iDB2Command GetLoad()
        {
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText =
                    @"SELECT 
	                    LHLDNM,
                        LHMCU,
                        LHADDJ,
	                    LHZ58EDTA,
	                    LHZ58ETMA,
	                    LHFRTH,
	                    LHZ58TN,
                        LHBOLN,
                        LHAN8,
                        LHSHAN,
	                    ELZ58BDRCS,
	                    ELZ58BKNM1,
                        ELZ58PARS
                    FROM
	                    F580150
	                    LEFT OUTER JOIN F580157
	                    ON	ELLDNM = LHLDNM AND
		                    ELMCU = LHMCU
                    WHERE
	                    LHLDNM = ?";
            cmd.Parameters.Add(new iDB2Parameter("LoadNum", iDB2DbType.iDB2Decimal));
            cmd.Parameters["LoadNum"].Precision = 8;
            cmd.Parameters["LoadNum"].Scale = 0;
            return cmd;
        }

        public iDB2Command GetLoadLines()
        {
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText =
                @"SELECT 
                    LPLDNM,
                    LPOSEQ,
                    LPOPLNID,
                    LPZ58RID,
                    LPZ58RKN,
                    LPZ58RKL           
                FROM 
	                F580151
                WHERE 
                    LPLDNM = ?";
            cmd.Parameters.Add(new iDB2Parameter("LoadNum", iDB2DbType.iDB2Decimal));
            cmd.Parameters["LoadNum"].Precision = 8;
            cmd.Parameters["LoadNum"].Scale = 0;
            return cmd;
        }

        public LoadLines GetLoadLines(BusinessSystemSession e1Session, Decimal loadNum)
        {
            var loadLines = new LoadLines();
            try
            {
                var cmd = (e1Session.DbCommands.ContainsKey("GetLoadLines")) ? e1Session.DbCommands["GetLoadLines"] : GetLoadLines();
                cmd.Parameters[0].Value = loadNum;
                cmd.Connection = e1Session.DbConnection;
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
                while (reader.Read())
                {
                    var loadLine = new LoadLine();
                    loadLine.SequenceNum = CastDbNull.To<Decimal>(reader["LPOSEQ"], 0);
                    loadLine.RackId = CastDbNull.To<Decimal>(reader["LPZ58RID"], 0);
                    loadLine.RacksShip = CastDbNull.To<Decimal>(reader["LPZ58RKN"], 0);
                    loadLine.RacksLoad = CastDbNull.To<Decimal>(reader["LPZ58RKL"], 0);
                    loadLines.Add(loadLine);
                }
                reader.Close();
            }
            catch (Exception err)
            {
                throw new ApplicationException(err.ToString());
            }
            return loadLines;
        }

    }
}
