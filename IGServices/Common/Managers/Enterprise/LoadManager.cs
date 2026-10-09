using System;
using System.Collections.Generic;
using System.Data;
using IBM.Data.DB2.iSeries;
using IGServices.Common.Types;
using IGServices.Common.Types.E1;
using IGServices.Common.Types.Enterprise;
using IGServices.Common.Utilities;

namespace IGServices.Common.Managers.Enterprise
{
    public class LoadManager
    {
        private CustomerManager _customerMgr;
        private GlassItemManager _glassItemMgr;
        private InvoiceManager _invoiceMgr;

        public LoadManager(BusinessSystemSession e1Session)
        {
            var hash = new Dictionary<string, iDB2Command>();
            hash.Add("LoadManager.GetLoad", GetLoad());
            hash.Add("LoadManager.GetLoadLines", GetLoadLines());
            hash.Add("LoadManager.GetLoadInvoices", GetLoadInvoices());
            hash.Add("LoadManager.GetLoadInvoiceLines", GetLoadInvoiceLines());
            foreach (string keyName in hash.Keys)
            {
                if (!e1Session.DbCommands.ContainsKey(keyName))
                {
                    e1Session.DbCommands.Add(keyName, hash[keyName]);
                }
            }
            _customerMgr = new CustomerManager(e1Session);
            _glassItemMgr = new GlassItemManager(e1Session);
            _invoiceMgr = new InvoiceManager(e1Session);
        }

        /******************************************************************
        * Commands
        ******************************************************************/
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

        public iDB2Command GetLoadInvoices()
        {
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText =
                @"SELECT 
                    SDDOC,
                    SDMCU,
                    SDIVD          
                FROM 
	                F4211
                WHERE 
                    SDPSN = ? AND
                    SDLNTY = 'W'
                GROUP BY
                    SDDOC,
                    SDMCU,
                    SDIVD";
            cmd.Parameters.Add(new iDB2Parameter("LoadNum", iDB2DbType.iDB2Decimal));
            cmd.Parameters["LoadNum"].Precision = 8;
            cmd.Parameters["LoadNum"].Scale = 0;
            return cmd;
        }

        public iDB2Command GetLoadInvoiceLines()
        {
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText =
                @"SELECT 
                    SDDOCO,
                    SDDCTO,
                    SDKCOO,
                    SDLNID         
                FROM 
	                F4211
                WHERE 
                    SDPSN = ? AND
                    SDDOC = ? AND
                    SDLNTY = 'W'";
            cmd.Parameters.Add(new iDB2Parameter("LoadNum", iDB2DbType.iDB2Decimal));
            cmd.Parameters["LoadNum"].Precision = 8;
            cmd.Parameters["LoadNum"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("InvoiceNum", iDB2DbType.iDB2Decimal));
            cmd.Parameters["InvoiceNum"].Precision = 8;
            cmd.Parameters["InvoiceNum"].Scale = 0;
            return cmd;
        }

        public Load GetLoad(BusinessSystemSession e1Session, Decimal loadNum)
        {
            var load = new Load();
            try
            {
                var cmd = (e1Session.DbCommands.ContainsKey("GetLoad")) ? e1Session.DbCommands["GetLoad"] : GetLoad();
                cmd.Parameters[0].Value = loadNum;
                cmd.Connection = e1Session.DbConnection;
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
                while (reader.Read())
                {
                    load.LoadNum = CastDbNull.To<Decimal>(reader["LHLDNM"], 0);
                    load.PlantNum = CastDbNull.To<String>(reader["LHMCU"], String.Empty).Trim();
                    var updateDateId = CastDbNull.To<Decimal>(reader["LHZ58EDTA"], 0);
                    var updateTimeId = CastDbNull.To<Decimal>(reader["LHZ58ETMA"], 0);
                    load.ArrivalDate = Utilities.Common.CastToDateTime(updateDateId, 0);
                    load.ArrivalHour = load.ArrivalDate.Hour;
                    load.ActualShipDate = Utilities.Common.CastToDateTime(CastDbNull.To<Decimal>(reader["LHADDJ"], 0), 0);
                    load.FreightHandlingCode = CastDbNull.To<String>(reader["LHFRTH"], String.Empty).Trim();
                    load.TrailerNum = CastDbNull.To<String>(reader["LHZ58TN"], String.Empty).Trim();
                    load.BorderCrossing = CastDbNull.To<String>(reader["ELZ58BDRCS"], String.Empty).Trim();
                    load.BrokerName = CastDbNull.To<String>(reader["ELZ58BKNM1"], String.Empty).Trim();
                    load.BillOfLadingNum = CastDbNull.To<Decimal>(reader["LHBOLN"], 0);
                    load.ParsNum = CastDbNull.To<String>(reader["ELZ58PARS"], String.Empty).Trim();
                    var customerNum = CastDbNull.To<Decimal>(reader["LHAN8"], 0);
                    load.Customer = _customerMgr.GetCustomer(e1Session, customerNum);
                    var shipToCustomerNum = CastDbNull.To<Decimal>(reader["LHSHAN"], 0);
                    load.ShipToCustomer = _customerMgr.GetCustomer(e1Session, shipToCustomerNum);

                    //load.LoadLines = GetLoadLines(e1Session, loadNum);
                    load.Invoices = GetLoadInvoices(e1Session, loadNum);
                }
                reader.Close();
            }
            catch (Exception err)
            {
                throw new ApplicationException(err.ToString());
            }
            return load;
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


        public Invoices GetLoadInvoices(BusinessSystemSession e1Session, Decimal loadNum)
        {
            var invoices = new Invoices();
            try
            {
                var cmd = (e1Session.DbCommands.ContainsKey("GetLoadInvoices")) ? e1Session.DbCommands["GetLoadInvoices"] : GetLoadInvoices();
                cmd.Parameters[0].Value = loadNum;
                cmd.Connection = e1Session.DbConnection;
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
                while (reader.Read())
                {
                    var invoiceNum = CastDbNull.To<Decimal>(reader["SDDOC"], 0);
                    var invoice = _invoiceMgr.GetInvoice(e1Session, invoiceNum);
                    invoices.Add(invoice);
                }
                reader.Close();
            }
            catch (Exception err)
            {
                throw new ApplicationException(err.ToString());
            }
            return invoices;
        }

        public Invoices GetLoadInvoices2(BusinessSystemSession e1Session, Decimal loadNum)
        {
            var invoices = new Invoices();
            try
            {
                var cmd1 = (e1Session.DbCommands.ContainsKey("GetLoadInvoices")) ? e1Session.DbCommands["GetLoadInvoices"] : GetLoadInvoices();
                cmd1.Parameters[0].Value = loadNum;
                cmd1.Connection = e1Session.DbConnection;
                var cmd2 = (e1Session.DbCommands.ContainsKey("GetLoadInvoiceLines")) ? e1Session.DbCommands["GetLoadInvoiceLines"] : GetLoadInvoiceLines();
                cmd2.Parameters[0].Value = loadNum;
                cmd2.Connection = e1Session.DbConnection;
                IDataReader reader1 = cmd1.ExecuteReader(CommandBehavior.SingleResult);
                while (reader1.Read())
                {
                    var invoice = new Invoice();
                    invoice.InvoiceNum = CastDbNull.To<Decimal>(reader1["SDDOC"], 0);
                    var invoiceDateId = CastDbNull.To<Decimal>(reader1["SDIVD"], 0);
                    invoice.InvoiceDate = Utilities.Common.CastToDateTime(invoiceDateId, 0);
                    invoice.PlantNum = CastDbNull.To<String>(reader1["SDMCU"], String.Empty);

                    // Get Lines
                    cmd2.Parameters[1].Value = invoice.InvoiceNum;
                    IDataReader reader2 = cmd2.ExecuteReader(CommandBehavior.SingleResult);
                    var invoiceLines = new InvoiceLines();
                    while (reader2.Read())
                    {
                        var invoiceLine = new InvoiceLine();
                        invoiceLine.SalesOrderNum = CastDbNull.To<Decimal>(reader2["SDDOCO"], 0);
                        invoiceLine.SalesOrderType = CastDbNull.To<String>(reader2["SDDCTO"], String.Empty);
                        var companyNum = CastDbNull.To<String>(reader2["SDDCTO"], String.Empty);
                        var lineNumId = CastDbNull.To<Decimal>(reader2["SDLNID"], 0);
                        invoiceLine.SalesOrderLineNum = lineNumId / 1000;
                        var glassItem = _glassItemMgr.GetGlassItem(e1Session, invoiceLine.ItemNum, invoice.PlantNum, invoiceLine.SalesOrderNum, invoiceLine.SalesOrderType, lineNumId, companyNum);
                        invoiceLine.Item = glassItem;
                        invoiceLines.Add(invoiceLine);
                    }
                    reader2.Close();
                    invoice.InvoiceLines = invoiceLines;
                    invoices.Add(invoice);
                }
                reader1.Close();
            }
            catch (Exception err)
            {
                throw new ApplicationException(err.ToString());
            }
            return invoices;
        }

    }
}
