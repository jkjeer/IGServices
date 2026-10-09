using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using IBM.Data.DB2.iSeries;
using IGServices.Common.Managers.E1;
using IGServices.Common.Types.E1;
using IGServices.Common.Types.Enterprise;
using IGServices.Common.Types.Response;
using IGServices.Common.Types.ShopFloor;
using IGServices.Common.Utilities;
using NLog;

namespace IGServices.Common.Managers.Enterprise
{
    public class PurchasedLotManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly GlassItemManager _glassItemManager;

        public PurchasedLotManager(BusinessSystemSession e1Session)
        {
            var hash = new Dictionary<string, iDB2Command>();
            hash.Add("PurchasedLotManager.GetRelatedOrderLines", GetRelatedOrderLines());
            hash.Add("PurchasedLotManager.GetWorkOrder", GetWorkOrder());
            hash.Add("PurchasedLotManager.GetSalesOrders", GetSalesOrders());
            foreach (string keyName in hash.Keys)
            {
                if (!e1Session.DbCommands.ContainsKey(keyName))
                {
                    e1Session.DbCommands.Add(keyName, hash[keyName]);
                }
            }
            _glassItemManager = new GlassItemManager(e1Session);
        }

        /**********************************************************************
         *  Purchased Lots Init
         *********************************************************************/
        public iDB2Command GetPurchasedLots()
        {
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = @"
                SELECT
                  OLMCU,
                  OLKCOO,
                  OLDOCO,
                  OLDCTO,
                  OLLNID / 1000 AS OLLNID,
                  OLITM,
                  OLLOTN,
                  OLTRDJ,
                  OLCNDJ,
                  OLLTTR,
                  OLNXTR,
                  OLUORG / 1000 AS OLUORG,
                  OLUOPN / 1000 AS OLUOPN,
                  OLUREC / 1000 AS OLUREC,
                  OLCNID,
                  OLUPMJ,
                  OLTDAY,
                  OLPID,
                  OLRORN,
                  OLRCTO,
                  OLRLLN / 1000 AS OLRLLN,
                  OLAN8,
                  OLUKID,
                  ABAC30
                FROM
                  F43199
                  LEFT OUTER JOIN F0101 
                    ON ABAN8 = OLAN8
                WHERE
                  OLMCU = ? AND
                  OLUKID >= ? AND
                  EXISTS (SELECT 1 FROM F580400 WHERE OLDOCO = RODOC1 AND OLDCTO = RORCTO AND RODOCO > 0)
                ORDER BY
                  OLUPMJ ,
                  OLTDAY ,
                  OLUKID";
            cmd.Parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("UKID", iDB2DbType.iDB2Decimal));
            cmd.Parameters["UKID"].Precision = 15;
            cmd.Parameters["UKID"].Scale = 0;
            return cmd;
        }

        public iDB2Command GetPurchasedLots2()
        {
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = @"
                SELECT
                  OLMCU,
                  OLKCOO,
                  OLDOCO,
                  OLDCTO,
                  OLLNID / 1000 AS OLLNID,
                  OLITM,
                  OLLOTN,
                  OLTRDJ,
                  OLCNDJ,
                  OLLTTR,
                  OLNXTR,
                  OLUORG / 1000 AS OLUORG,
                  OLUOPN / 1000 AS OLUOPN,
                  OLUREC / 1000 AS OLUREC,
                  OLCNID,
                  OLUPMJ,
                  OLTDAY,
                  OLPID,
                  OLRORN,
                  OLRCTO,
                  OLRLLN / 1000 AS OLRLLN,
                  OLAN8,
                  ABAC30
                FROM
                  F43199
                  LEFT OUTER JOIN F0101 
                    ON ABAN8 = OLAN8
                WHERE
                  OLMCU = ? AND
                  ((OLUPMJ = ? AND OLTDAY >= ?) OR (OLUPMJ >= ? AND OLUPMJ <= ?)) AND
                  EXISTS (SELECT 1 FROM F580400 WHERE OLDOCO = RODOC1 AND OLDCTO = RORCTO AND RODOCO > 0)
                ORDER BY
                  OLUPMJ ,
                  OLTDAY ,
                  OLUKID";
            cmd.Parameters.Add(new iDB2Parameter("Plant", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("Date1", iDB2DbType.iDB2Decimal));
            cmd.Parameters["Date1"].Precision = 8;
            cmd.Parameters["Date1"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("Time1", iDB2DbType.iDB2Decimal));
            cmd.Parameters["Time1"].Precision = 8;
            cmd.Parameters["Time1"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("Date2", iDB2DbType.iDB2Decimal));
            cmd.Parameters["Date2"].Precision = 8;
            cmd.Parameters["Date2"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("Date3", iDB2DbType.iDB2Decimal));
            cmd.Parameters["Date3"].Precision = 8;
            cmd.Parameters["Date3"].Scale = 0;
            return cmd;
        }

        public PurchasedLots GetPurchasedLots(BusinessSystemSession e1Session, string plantCode, HighWaterMark hw)
        {
            var lots = new PurchasedLots();
            try
            {
                var plant = PlantManager.GetPlant(plantCode);
                var cmd = (e1Session.DbCommands.ContainsKey("PurchasedLotManager.GetPurchasedLots")) ? e1Session.DbCommands["PurchasedLotManager.GetPurchasedLots"] : GetPurchasedLots();
                cmd.Parameters[0].Value = plant.PlantNum.PadLeft(12);
                cmd.Parameters[1].Value = hw.UniqueID;
                cmd.Connection = e1Session.DbConnection;
                Logger.Debug(Logging.DbCmdString(cmd));
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
                while (reader.Read())
                {
                    var lot = new PurchasedLot();
                    lot.UniqueId = CastDbNull.To<decimal>(reader["OLUKID"], 0);
                    lot.PlantNum = CastDbNull.To(reader["OLMCU"], string.Empty).Trim();
                    lot.LotOrderCompanyNum = CastDbNull.To(reader["OLKCOO"], string.Empty).Trim();
                    lot.LotOrderNum = CastDbNull.To<decimal>(reader["OLDOCO"], 0);
                    lot.LotOrderType = CastDbNull.To(reader["OLDCTO"], string.Empty).Trim();
                    lot.LotOrderLineNum = Math.Round(CastDbNull.To<decimal>(reader["OLLNID"], 0), 3);
                    lot.ItemNum = CastDbNull.To<decimal>(reader["OLITM"], 0);
                    lot.LotNum = CastDbNull.To(reader["OLLOTN"], string.Empty).Trim();
                    lot.LastStatus = CastDbNull.To(reader["OLLTTR"], string.Empty).Trim();
                    lot.NextStatus = CastDbNull.To(reader["OLNXTR"], string.Empty).Trim();
                    var orderDateId = CastDbNull.To<decimal>(reader["OLTRDJ"], 0);
                    lot.OrderDate = Utilities.Common.CastToDateTime(orderDateId);
                    lot.OrderQty = Math.Round(CastDbNull.To<decimal>(reader["OLUORG"], 0), 3);
                    lot.ReceivedQty = Math.Round(CastDbNull.To<decimal>(reader["OLUREC"], 0), 3);
                    lot.OpenQty = Math.Round(CastDbNull.To<decimal>(reader["OLUOPN"], 0), 3);
                    var cancelDateId = CastDbNull.To<decimal>(reader["OLCNDJ"], 0);
                    lot.CancelDate = Utilities.Common.CastToDateTime(cancelDateId);
                    var eventDateId = CastDbNull.To<decimal>(reader["OLUPMJ"], 0);
                    var eventTimeId = CastDbNull.To<decimal>(reader["OLTDAY"], 0);
                    lot.ProcessId = CastDbNull.To(reader["OLPID"], string.Empty).Trim();
                    lot.EventDate = Utilities.Common.CastToDateTime(eventDateId, eventTimeId);
                    lot.SupplierPlantNum = CastDbNull.To(reader["ABAC30"], string.Empty).Trim();
                    try
                    {
                        lot.SupplierOrderNum = Convert.ToDecimal(CastDbNull.To(reader["OLRORN"], string.Empty).Trim());
                    } catch (Exception)
                    {
                        lot.SupplierOrderNum = 0;
                    }
                    lot.SupplierOrderType = CastDbNull.To(reader["OLRCTO"], string.Empty).Trim();
                    lot.SupplierOrderLineNum = Math.Round(CastDbNull.To<decimal>(reader["OLRLLN"], 0), 3);

                    //var glassItem = _glassItemManager.GetItemByPurchaseOrder(e1Session, lot.LotOrderNum, lot.LotOrderType, lot.LotOrderLineNum * 1000);
                    //lot.GlassItem = glassItem;

                    //var orders = GetRelatedOrderLines(e1Session, lot.LotOrderNum, lot.LotOrderType, lot.LotOrderLineNum);
                    //lot.RelatedOrderLines = orders;
                    lots.Add(lot);
                }
                reader.Close();

                foreach (PurchasedLot lot in lots)
                {
                    var glassItem = _glassItemManager.GetGlassItem(e1Session, lot.ItemNum, plantCode, lot.LotOrderNum, lot.LotOrderType, lot.LotOrderLineNum * 1000, lot.LotOrderCompanyNum);
                    lot.GlassItem = glassItem;

                    var orders = GetRelatedOrderLines(e1Session, lot.LotOrderNum, lot.LotOrderType, lot.LotOrderLineNum);
                    lot.RelatedOrderLines = orders;                    
                }
            }
            catch (Exception err)
            {
                throw new ApplicationException(err.ToString());
            }
            return lots;
        }

        /**********************************************************************
          *  Related Order Lines
          *********************************************************************/
        public iDB2Command GetRelatedOrderLines()
        {
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = @"
            SELECT
              ROMCU ,
              RODOCO ,
              RODCTO ,
              ROLNID / 1000 AS ROLNID ,
              ROUPMJ ,
              ROTDAY ,
              ROZ58L1CLN ,
              ROEDTR ,
              SDRORN ,
              SDRCTO
            FROM
              F580400
              INNER JOIN F4211
              ON SDDOCO = RODOCO AND
                 SDDCTO = RODCTO AND
                 SDLNID = ROLNID
            WHERE
              RODOC1 = ? AND
              RORCTO = ? AND
              ABS(RORLLN - ?) < 100  AND
              RODOCO > 0
            ORDER BY
              ROUPMJ ,
              ROTDAY";
            cmd.Parameters.Add(new iDB2Parameter("OrderNum", iDB2DbType.iDB2Decimal));
            cmd.Parameters["OrderNum"].Precision = 8;
            cmd.Parameters["OrderNum"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("OrderType", iDB2DbType.iDB2Graphic, 2));
            cmd.Parameters.Add(new iDB2Parameter("OrderLineNum", iDB2DbType.iDB2Decimal));
            cmd.Parameters["OrderLineNum"].Precision = 6;
            cmd.Parameters["OrderLineNum"].Scale = 0;
            return cmd;
        }

        public RelatedOrderLines GetRelatedOrderLines(BusinessSystemSession e1Session, decimal orderNum, string orderType, decimal orderLineNum)
        {
            var lines = new RelatedOrderLines();
            try
            {
                var cmd = (e1Session.DbCommands.ContainsKey("PurchasedLotManager.GetRelatedOrderLines")) ? e1Session.DbCommands["PurchasedLotManager.GetRelatedOrderLines"] : GetRelatedOrderLines();
                cmd.Parameters[0].Value = orderNum;
                cmd.Parameters[1].Value = orderType;
                cmd.Parameters[2].Value = orderLineNum * 1000;
                cmd.Connection = e1Session.DbConnection;
                Logger.Debug(Logging.DbCmdString(cmd));
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
                while (reader.Read())
                {
                    try
                    {
                        var line = new RelatedOrderLine();
                        line.OrderNum = CastDbNull.To<decimal>(reader["RODOCO"], 0);
                        line.OrderType = CastDbNull.To(reader["RODCTO"], string.Empty).Trim();
                        line.OrderLineNum = Math.Round(CastDbNull.To<decimal>(reader["ROLNID"], 0), 3);
                        line.LiteNum = CastDbNull.To<decimal>(reader["ROZ58L1CLN"], 0);
                        line.TransactionType = CastDbNull.To(reader["ROEDTR"], string.Empty).Trim();
                        var workOrder = GetWorkOrder(e1Session, line.OrderNum, line.OrderType, line.OrderLineNum);
                        line.WorkOrder = workOrder;
                        lines.Add(line);
                    }
                    catch (Exception e)
                    {
                        throw new ApplicationException(e.ToString());
                    }
                }
                reader.Close();
            }
            catch (Exception err)
            {
                throw new ApplicationException(err.ToString());
            }
            return lines;
        }

        /**********************************************************************
          *  Work Order
          *********************************************************************/
        public iDB2Command GetWorkOrder()
        {
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = @"
            SELECT
              WACO,
              WADOCO,
              WADCTO,
              WATRDJ,
              WASRST,
              WAITM,
              IMSRP4,
              WAUORG / 1000 AS WAUORG
            FROM
              F580032
              INNER JOIN F4801 
              ON WADOCO = ATZ58WODC
              INNER JOIN F4101
              ON IMITM = WAITM
            WHERE
              ATDOCO = ? AND
              ATDCTO = ? AND
              ATLNID = ?";
            cmd.Parameters.Add(new iDB2Parameter("OrderNum", iDB2DbType.iDB2Decimal));
            cmd.Parameters["OrderNum"].Precision = 8;
            cmd.Parameters["OrderNum"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("OrderType", iDB2DbType.iDB2Graphic, 2));
            cmd.Parameters.Add(new iDB2Parameter("OrderLineNum", iDB2DbType.iDB2Decimal));
            cmd.Parameters["OrderLineNum"].Precision = 6;
            cmd.Parameters["OrderLineNum"].Scale = 0;
            return cmd;
        }

        public WorkOrder GetWorkOrder(BusinessSystemSession e1Session, decimal orderNum, string orderType, decimal orderLineNum)
        {
            var workOrder = new WorkOrder();
            try
            {
                var cmd = (e1Session.DbCommands.ContainsKey("PurchasedLotManager.GetWorkOrder")) ? e1Session.DbCommands["PurchasedLotManager.GetWorkOrder"] : GetWorkOrder();
                cmd.Parameters[0].Value = orderNum;
                cmd.Parameters[1].Value = orderType;
                cmd.Parameters[2].Value = orderLineNum * 1000;
                cmd.Connection = e1Session.DbConnection;
                Logger.Debug(Logging.DbCmdString(cmd));
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
                while (reader.Read())
                {
                    try
                    {
                        workOrder.WorkOrderNum = CastDbNull.To<decimal>(reader["WADOCO"], 0);
                        workOrder.Quantity = CastDbNull.To<decimal>(reader["WAUORG"], 0);
                        workOrder.StatusCode = CastDbNull.To(reader["WASRST"], string.Empty).Trim();
                        var dateId = CastDbNull.To<decimal>(reader["WATRDJ"], 0);
                        workOrder.OrderDate = Utilities.Common.CastToDateTime(dateId);
                        workOrder.UnitType = CastDbNull.To(reader["IMSRP4"], string.Empty).Trim();
                        workOrder.ItemNum = CastDbNull.To<decimal>(reader["WAITM"], 0);
                        var salesLines = GetSalesOrders(e1Session, workOrder.WorkOrderNum);
                        workOrder.RelatedSalesOrderLines = salesLines;
                    }
                    catch (Exception e)
                    {
                        throw new ApplicationException(e.ToString());
                    }
                }
                reader.Close();
            }
            catch (Exception err)
            {
                throw new ApplicationException(err.ToString());
            }
            return workOrder;
        }

        /**********************************************************************
          *  Sales orders
          *********************************************************************/
        public iDB2Command GetSalesOrders()
        {
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = @"
            SELECT
              ATDOCO ,
              ATDCTO ,
              ATLNID / 1000 AS ATLNID ,
              SDITM,
              SDLTTR,
              SDNXTR,
              SDUORG / 1000 AS SDUORG,
              SDTRDJ,
              SDDRQJ,
              SDAN8,
              AB1.ABALPH AS SOLD_CUST_NAME,
              SDSHAN,
              AB2.ABALPH AS SHIP_CUST_NAME
            FROM
              F580032
              INNER JOIN F4211
              ON SDDOCO = ATDOCO AND
	            SDDCTO = ATDCTO AND
                SDLNID = ATLNID
              INNER JOIN F0101 AB1
                ON AB1.ABAN8 = SDAN8
                INNER JOIN F0101 AB2
                ON AB2.ABAN8 = SDSHAN
            WHERE
                ATZ58WODC = ?";
            cmd.Parameters.Add(new iDB2Parameter("OrderNum", iDB2DbType.iDB2Decimal));
            cmd.Parameters["OrderNum"].Precision = 8;
            cmd.Parameters["OrderNum"].Scale = 0;
            return cmd;
        }

        public RelatedSalesOrderLines GetSalesOrders(BusinessSystemSession e1Session, decimal orderNum)
        {
            var relatedSalesLines = new RelatedSalesOrderLines();
            try
            {
                var cmd = (e1Session.DbCommands.ContainsKey("PurchasedLotManager.GetSalesOrders")) ? e1Session.DbCommands["PurchasedLotManager.GetSalesOrders"] : GetSalesOrders();
                cmd.Parameters[0].Value = orderNum;
                cmd.Connection = e1Session.DbConnection;
                Logger.Debug(Logging.DbCmdString(cmd));
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
                while (reader.Read())
                {
                    //try
                    //{
                        var relatedSalesLine = new RelatedSalesOrderLine();
                        relatedSalesLine.OrderNum = CastDbNull.To<decimal>(reader["ATDOCO"], 0);
                        relatedSalesLine.OrderType = CastDbNull.To(reader["ATDCTO"], string.Empty).Trim();
                        relatedSalesLine.OrderLineNum = Math.Round(CastDbNull.To<decimal>(reader["ATLNID"], 0), 3);
                        relatedSalesLine.OrderQty = Math.Round(CastDbNull.To<decimal>(reader["ATLNID"], 0), 3);
                        var orderDateId = CastDbNull.To<decimal>(reader["SDTRDJ"], 0);
                        relatedSalesLine.OrderDate = Utilities.Common.CastToDateTime(orderDateId);
                        var requestedDateId = CastDbNull.To<decimal>(reader["SDDRQJ"], 0);
                        relatedSalesLine.RequestedDate = Utilities.Common.CastToDateTime(requestedDateId);
                        var soldToCust = new Customer();
                        soldToCust.CustomerNum = CastDbNull.To<decimal>(reader["SDAN8"], 0);
                        soldToCust.CustomerName = CastDbNull.To(reader["SOLD_CUST_NAME"], string.Empty).Trim();
                        relatedSalesLine.SoldToCustomer = soldToCust;
                        var shipToCust = new Customer();
                        shipToCust.CustomerNum = CastDbNull.To<decimal>(reader["SDSHAN"], 0);
                        shipToCust.CustomerName = CastDbNull.To(reader["SHIP_CUST_NAME"], string.Empty).Trim();
                        relatedSalesLine.ShipToCustomer = shipToCust;
                        relatedSalesLines.Add(relatedSalesLine);
                    //}
                    //catch (Exception)
                    //{
                    //}
                }
                reader.Close();
            }
            catch (Exception err)
            {
                throw new ApplicationException(err.ToString());
            }
            return relatedSalesLines;
        }

        /**********************************************************************
         *  Lot Shipping Events
         *********************************************************************/
        public iDB2Command GetLotShippingEvents()
        {
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = @"
	        SELECT
		        PDMCU ,
		        PDDOCO ,
		        PDDCTO ,
		        PDLNID / 1000 AS PDLNID ,
		        PDITM,
		        PDLOTN,
		        LHMCU,
		        LHLDNM,
		        LHZ58LDS,
		        LHUPMJ,
		        LHTDAY,
		        LHADDJ ,
		        LHADTM
	        FROM
		        F580150
		        INNER JOIN F4211
		        ON	LHMCU = SDMCU AND
			        LHLDNM = SDPSN
		        INNER JOIN F4311
		        ON	PDLOTN = SDLOTN AND
			        PDLOTN != ''
	        WHERE
		        PDMCU = ? AND
                ((LHUPMJ = ? AND LHTDAY >= ?) OR (LHUPMJ >= ? AND LHUPMJ <= ?)) AND
		        EXISTS ( SELECT
					        1
				        FROM
					        F580400
				        WHERE
					        RORCTO = 'OT' AND
					        RODOCO > 0 AND
					        PDDOCO = RODOC1 AND
					        PDDCTO = RORCTO )
	        ORDER BY
		        PDDOCO ,
		        PDLNID";
            cmd.Parameters.Add(new iDB2Parameter("Plant", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("Date1", iDB2DbType.iDB2Decimal));
            cmd.Parameters["Date1"].Precision = 8;
            cmd.Parameters["Date1"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("Time1", iDB2DbType.iDB2Decimal));
            cmd.Parameters["Time1"].Precision = 8;
            cmd.Parameters["Time1"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("Date2", iDB2DbType.iDB2Decimal));
            cmd.Parameters["Date2"].Precision = 8;
            cmd.Parameters["Date2"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("Date3", iDB2DbType.iDB2Decimal));
            cmd.Parameters["Date3"].Precision = 8;
            cmd.Parameters["Date3"].Scale = 0;
            return cmd;
        }

        /**********************************************************************
         *  LG ASN Events
         *********************************************************************/
        public iDB2Command GetLotAsnEvents()
        {
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = @"
            SELECT
	            PDMCU ,
	            PDDOCO ,
	            PDDCTO ,
	            PDLNID / 1000 AS PDLNID ,
	            PDITM,
	            PDLOTN,
	            PDNXTR,
	            PDUPMJ ,
	            PDTDAY ,
                SRMMCU,
	            SRLODN,
	            SRUPMJ,
	            SRUPMT
            FROM
	            F580074
	            INNER JOIN F4311
	            ON PDDOCO = SRDOCO AND
		            PDDCTO = SRDCTO AND
		            PDLNID = SRLNID
            WHERE
	            PDMCU = ? AND
                ((SRUPMJ = ? AND SRUPMT >= ?) OR (SRUPMJ >= ? AND SRUPMJ <= ?)) AND
	            SRMMCU IN ('          08', '          29', '          37') AND 
	            SRPID = 'P5500460' AND 
	            SREDSP = '' AND
              EXISTS ( SELECT
                        1
                       FROM
                        F580400
                       WHERE
                        RORCTO = 'OT' AND
                        RODOCO > 0 AND
                        PDDOCO = RODOC1 AND
                        PDDCTO = RORCTO  )
            ORDER BY
              PDDOCO ,
              PDDCTO,
              PDLNID";
            cmd.Parameters.Add(new iDB2Parameter("Plant", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("Date1", iDB2DbType.iDB2Decimal));
            cmd.Parameters["Date1"].Precision = 8;
            cmd.Parameters["Date1"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("Time1", iDB2DbType.iDB2Decimal));
            cmd.Parameters["Time1"].Precision = 8;
            cmd.Parameters["Time1"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("Date2", iDB2DbType.iDB2Decimal));
            cmd.Parameters["Date2"].Precision = 8;
            cmd.Parameters["Date2"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("Date3", iDB2DbType.iDB2Decimal));
            cmd.Parameters["Date3"].Precision = 8;
            cmd.Parameters["Date3"].Scale = 0;
            return cmd;
        }

        public PurchasedLots GetPurchasedLotLoads(BusinessSystemSession e1Session, string plantCode, E1DateParms dateParms)
        {
            var lots = new PurchasedLots();
            var plantNum = PlantManager.GetPlant(plantCode).PlantNum;
            try
            {
                var cmd = (e1Session.DbCommands.ContainsKey("PurchasedLotManager.GetLotShippingEvents")) ? e1Session.DbCommands["PurchasedLotManager.GetLotShippingEvents"] : GetLotShippingEvents();
                cmd.Parameters[0].Value = plantNum.PadLeft(12);
                cmd.Parameters[1].Value = dateParms.DateComponent1;
                cmd.Parameters[2].Value = dateParms.TimeComponent1;
                cmd.Parameters[3].Value = dateParms.DateComponent2;
                cmd.Parameters[4].Value = dateParms.DateComponent3;
                cmd.Connection = e1Session.DbConnection;
                cmd.CommandTimeout = 0;
                Logger.Debug(Logging.DbCmdString(cmd));
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
                while (reader.Read())
                {
                    var lot = new PurchasedLot();
                    lot.PlantNum = CastDbNull.To(reader["PDMCU"], string.Empty).Trim();
                    lot.LotOrderNum = CastDbNull.To<decimal>(reader["PDDOCO"], 0);
                    lot.LotOrderType = CastDbNull.To(reader["PDDCTO"], string.Empty).Trim();
                    lot.LotOrderLineNum = Math.Round(CastDbNull.To<decimal>(reader["PDLNID"], 0), 3);
                    lot.SupplierPlantNum = CastDbNull.To(reader["LHMCU"], string.Empty).Trim();
                    lot.LoadNum = CastDbNull.To<decimal>(reader["LHLDNM"], 0);
                    var shipDateId = CastDbNull.To<decimal>(reader["LHADDJ"], 0);
                    var shipTimeId = CastDbNull.To<decimal>(reader["LHADTM"], 0);
                    lot.LoadDate = Utilities.Common.CastToDateTime(shipDateId, shipTimeId);
                    var eventDateId = CastDbNull.To<decimal>(reader["LHUPMJ"], 0);
                    var eventTimeId = CastDbNull.To<decimal>(reader["LHTDAY"], 0);
                    lot.EventDate = Utilities.Common.CastToDateTime(eventDateId, eventTimeId);
                    lots.Add(lot);
                }
                reader.Close();

                var cmd1 = (e1Session.DbCommands.ContainsKey("PurchasedLotManager.GetLotAsnEvents")) ? e1Session.DbCommands["PurchasedLotManager.GetLotAsnEvents"] : GetLotAsnEvents();
                cmd1.Parameters[0].Value = plantNum.PadLeft(12);
                cmd1.Parameters[1].Value = dateParms.DateComponent1;
                cmd1.Parameters[2].Value = dateParms.TimeComponent1;
                cmd1.Parameters[3].Value = dateParms.DateComponent2;
                cmd1.Parameters[4].Value = dateParms.DateComponent3;
                cmd1.Connection = e1Session.DbConnection;
                cmd1.CommandTimeout = 0;
                Logger.Debug(Logging.DbCmdString(cmd1));
                IDataReader reader1 = cmd1.ExecuteReader(CommandBehavior.SingleResult);
                while (reader1.Read())
                {
                    var lot = new PurchasedLot();
                    lot.PlantNum = CastDbNull.To(reader1["PDMCU"], string.Empty).Trim();
                    lot.LotOrderNum = CastDbNull.To<decimal>(reader1["PDDOCO"], 0);
                    lot.LotOrderType = CastDbNull.To(reader1["PDDCTO"], string.Empty).Trim();
                    lot.LotOrderLineNum = Math.Round(CastDbNull.To<decimal>(reader1["PDLNID"], 0), 3);
                    lot.SupplierPlantNum = CastDbNull.To(reader1["SRMMCU"], string.Empty).Trim();
                    lot.LoadNum = CastDbNull.To<decimal>(reader1["SRLODN"], 0);
                    var shipDateId1 = CastDbNull.To<decimal>(reader1["SRUPMJ"], 0);
                    var shipTimeId1 = CastDbNull.To<decimal>(reader1["SRUPMT"], 0);
                    lot.LoadDate = Utilities.Common.CastToDateTime(shipDateId1, shipTimeId1);
                    lot.EventDate = Utilities.Common.CastToDateTime(shipDateId1, shipTimeId1);
                    lots.Add(lot);
                }
                reader1.Close();
            }
            catch (Exception err)
            {
                throw new ApplicationException(err.ToString());
            }
            return lots;
        }

        /**********************************************************************
         *  Receiving Events
         *********************************************************************/
        public iDB2Command GetPurchasedLotReceipts()
        {
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = @"
            SELECT
	            PDMCU ,
	            PDDOCO ,
	            PDDCTO ,
	            PDLNID / 1000 AS PDLNID,
                DLLOTN,
	            DLPSN,
	            DLZ58RID,
	            DLZ58SN,
                DLPQOH / 1000 AS DLPQOH,
	            DLUPMJ ,
	            DLTDAY
            FROM
	            F580569
	            INNER JOIN F4311
	            ON	DLLOTN = PDLOTN AND
		            DLMCU = PDMCU 
            WHERE
	            DLMCU = ? AND
	            DLLOTN != '' AND
                ((DLUPMJ = ? AND DLTDAY >= ?) OR (DLUPMJ >= ? AND DLUPMJ <= ?)) AND
	            (
                    (DLPID IN ('N580060') AND DLZ58DST = '8' AND DLPSN > 0) OR
	                (DLPID IN ('R47071') AND DLZ58DST = '4' AND DLPSN = 0) 
                ) AND
	            EXISTS ( SELECT
				            1
			            FROM
				            F580400
			            WHERE
				            RORCTO = 'OT' AND
				            RODOCO > 0 AND
				            PDDOCO = RODOC1 AND
				            PDDCTO = RORCTO )
            ORDER BY
	            PDDOCO ,
                PDDCTO,
	            PDLNID";
            cmd.Parameters.Add(new iDB2Parameter("Plant", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("Date1", iDB2DbType.iDB2Decimal));
            cmd.Parameters["Date1"].Precision = 8;
            cmd.Parameters["Date1"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("Time1", iDB2DbType.iDB2Decimal));
            cmd.Parameters["Time1"].Precision = 8;
            cmd.Parameters["Time1"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("Date2", iDB2DbType.iDB2Decimal));
            cmd.Parameters["Date2"].Precision = 8;
            cmd.Parameters["Date2"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("Date3", iDB2DbType.iDB2Decimal));
            cmd.Parameters["Date3"].Precision = 8;
            cmd.Parameters["Date3"].Scale = 0;
            return cmd;
        }

        public PurchasedLots GetPurchasedLotReceipts(BusinessSystemSession e1Session, string plantCode, E1DateParms dateParms)
        {
            var lots = new PurchasedLots();
            var plantNum = PlantManager.GetPlant(plantCode).PlantNum;
            try
            {
                var cmd = (e1Session.DbCommands.ContainsKey("PurchasedLotManager.GetPurchasedLotReceipts")) ? e1Session.DbCommands["PurchasedLotManager.GetPurchasedLotReceipts"] : GetPurchasedLotReceipts();
                cmd.Parameters[0].Value = plantNum.PadLeft(12);
                cmd.Parameters[1].Value = dateParms.DateComponent1;
                cmd.Parameters[2].Value = dateParms.TimeComponent1;
                cmd.Parameters[3].Value = dateParms.DateComponent2;
                cmd.Parameters[4].Value = dateParms.DateComponent3;
                cmd.Connection = e1Session.DbConnection;
                Logger.Debug(Logging.DbCmdString(cmd));
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
                while (reader.Read())
                {
                    var lot = new PurchasedLot();
                    lot.PlantNum = CastDbNull.To(reader["PDMCU"], string.Empty).Trim();
                    lot.LotOrderNum = CastDbNull.To<decimal>(reader["PDDOCO"], 0);
                    lot.LotOrderType = CastDbNull.To(reader["PDDCTO"], string.Empty).Trim();
                    lot.LotOrderLineNum = Math.Round(CastDbNull.To<decimal>(reader["PDLNID"], 0), 3);
                    lot.LotNum = CastDbNull.To(reader["DLLOTN"], string.Empty).Trim();

                    lot.LoadNum = CastDbNull.To<decimal>(reader["DLPSN"], 0);
                    var recdDateId = CastDbNull.To<decimal>(reader["DLUPMJ"], 0);
                    var recdTimeId = CastDbNull.To<decimal>(reader["DLTDAY"], 0);
                    lot.EventDate = Utilities.Common.CastToDateTime(recdDateId, recdTimeId);
                
                    lot.RackId = CastDbNull.To<decimal>(reader["DLZ58RID"], 0);
                    lot.SerialNum = CastDbNull.To(reader["DLZ58SN"], string.Empty).Trim();
                    lot.RackQty = Math.Round(CastDbNull.To<decimal>(reader["DLPQOH"], 0), 3);

                    lots.Add(lot);
                }
                reader.Close();
            }
            catch (Exception err)
            {
                throw new ApplicationException(err.ToString());
            }
            return lots;
        }

        /**********************************************************************
         *  Persist Purchased Orders
         *********************************************************************/
        public SqlBulkCopyResponses PersistPurchasedLots(BusinessSystemSession e1Session, string plantCode, PurchasedLots lots)
        {
            var responses = new SqlBulkCopyResponses();
            var plantNum = PlantManager.GetPlant(plantCode).PlantNum;
            try
            {
                var dt1 = GetLotTable("PurchasedLots");
                var dt2 = GetLotWorkOrdersTable("WorkOrders");
                var dt3 = GetLotSalesOrdersTable("SalesOrders");
                AddLotRows(dt1, dt2, dt3, lots);
                var bc = new SqlBulkCopyManager();
                var cn = ConnectionManager.GetSqlConnectionFromKey(plantNum + ".INTEGRATION");
                bc.DbConnectionString = cn.ConnectionString;
                var returnResponse = bc.BulkTable("PurchasedLotsLedger", dt1);
                var response2 = bc.BulkTable("PurchasedLotWorkOrdersLedger", dt2);
                var response3 = bc.BulkTable("PurchasedLotSalesOrdersLedger", dt3);
                responses.Add(returnResponse);
                responses.Add(response2);
                responses.Add(response3);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.Message);
            }
            return responses;
        }

        public DataTable GetLotTable(string tableName)
        {
            var dt = new DataTable(tableName);
            dt.Columns.Add("LotOrderNum", typeof(decimal));
            dt.Columns.Add("LotOrderType", typeof(string));
            dt.Columns.Add("LotOrderLineNum", typeof(decimal));
            dt.Columns.Add("ItemNum", typeof(decimal));
            dt.Columns.Add("LotNum", typeof(string));

            dt.Columns.Add("Thickness", typeof(decimal));
            dt.Columns.Add("Color", typeof(string));
            dt.Columns.Add("Treatment", typeof(string));
            dt.Columns.Add("FirstSrfcCoat", typeof(string));
            dt.Columns.Add("SecondSrfcCoat", typeof(string));
            dt.Columns.Add("Lami", typeof(string));
            dt.Columns.Add("Pattern", typeof(string));

            dt.Columns.Add("ShapeNum", typeof(string));
            dt.Columns.Add("BaseDim", typeof(decimal));
            dt.Columns.Add("LeftDim", typeof(decimal));
            dt.Columns.Add("RightDim", typeof(decimal));
            dt.Columns.Add("TopDim", typeof(decimal));
            dt.Columns.Add("Side1Dim", typeof(decimal));
            dt.Columns.Add("Side2Dim", typeof(decimal));

            dt.Columns.Add("LastStatus", typeof(string));
            dt.Columns.Add("NextStatus", typeof(string));
            dt.Columns.Add("OrderDate", typeof(DateTime));
            dt.Columns.Add("CancelDate", typeof(SqlDateTime));
            dt.Columns.Add("OrderQty", typeof(decimal));
            dt.Columns.Add("ReceivedQty", typeof(decimal));
            dt.Columns.Add("OpenQty", typeof(decimal));
            dt.Columns.Add("SupplierPlant", typeof(string));
            dt.Columns.Add("SupplierOrderNum", typeof(decimal));
            dt.Columns.Add("SupplierOrderType", typeof(string));
            dt.Columns.Add("SupplierOrderLineNum", typeof(decimal));
            dt.Columns.Add("ProcessId", typeof(string));
            dt.Columns.Add("EventDate", typeof(DateTime));
            return dt;
        }

        public DataTable GetLotWorkOrdersTable(string tableName)
        {
            var dt = new DataTable(tableName);
            dt.Columns.Add("LotOrderNum", typeof(decimal));
            dt.Columns.Add("LotOrderType", typeof(string));
            dt.Columns.Add("LotOrderLineNum", typeof(decimal));
            dt.Columns.Add("RelatedLiteNum", typeof(decimal));
            dt.Columns.Add("RelatedTranType", typeof(string));
            dt.Columns.Add("WorkOrderDate", typeof(DateTime));
            dt.Columns.Add("WorkOrderNum", typeof(decimal));
            dt.Columns.Add("WorkOrderStatus", typeof(string));
            dt.Columns.Add("WorkOrderQty", typeof(decimal));
            dt.Columns.Add("ItemNum", typeof(decimal));
            dt.Columns.Add("UnitType", typeof(string));
            dt.Columns.Add("EventDate", typeof(DateTime));
            return dt;
        }

        public DataTable GetLotSalesOrdersTable(string tableName)
        {
            var dt = new DataTable(tableName);
            dt.Columns.Add("WorkOrderNum", typeof(decimal));
            dt.Columns.Add("SalesOrderNum", typeof(decimal));
            dt.Columns.Add("SalesOrderType", typeof(string));
            dt.Columns.Add("SalesOrderLineNum", typeof(decimal));
            dt.Columns.Add("OrderQty", typeof(decimal));
            dt.Columns.Add("OrderDate", typeof(DateTime));
            dt.Columns.Add("RequestedDate", typeof(DateTime));
            dt.Columns.Add("SoldToCustomerNum", typeof(decimal));
            dt.Columns.Add("SoldToCustomerName", typeof(string));
            dt.Columns.Add("ShipToCustomerNum", typeof(decimal));
            dt.Columns.Add("ShipToCustomerName", typeof(string));
            dt.Columns.Add("EventDate", typeof(DateTime));
            return dt;
        }

        public void AddLotRows(DataTable ltTable, DataTable woTable, DataTable soTable, PurchasedLots lots)
        {
            foreach (PurchasedLot lot in lots)
            {
                if (lot.GlassItem.Shape != null)
                {
                    DataRow dr = ltTable.NewRow();
                    dr["LotOrderNum"] = lot.LotOrderNum;
                    dr["LotOrderType"] = lot.LotOrderType;
                    dr["LotOrderLineNum"] = lot.LotOrderLineNum;
                    dr["ItemNum"] = lot.ItemNum;
                    dr["LotNum"] = lot.LotNum;

                    var gt = lot.GlassItem.Lites[0].GlassType;
                    dr["Thickness"] = gt.Thickness;
                    dr["Color"] = gt.Color;
                    dr["Treatment"] = gt.HeatTreatment;
                    dr["FirstSrfcCoat"] = gt.Surface1Coating;
                    dr["SecondSrfcCoat"] = gt.Surface2Coating;
                    dr["Lami"] = gt.LamiProductType;
                    dr["Pattern"] = gt.PatternTexture;
                    var sh = lot.GlassItem.Shape;
                    dr["ShapeNum"] = sh.ShapeNum;
                    dr["BaseDim"] = sh.BaseDimension.DecimalValue;
                    dr["LeftDim"] = sh.LeftDimension.DecimalValue;
                    dr["RightDim"] = sh.RightDimension.DecimalValue;
                    dr["TopDim"] = sh.TopDimension.DecimalValue;
                    dr["Side1Dim"] = sh.Side1Dimension.DecimalValue;
                    dr["Side2Dim"] = sh.Side2Dimension.DecimalValue;

                    dr["LastStatus"] = lot.LastStatus;
                    dr["NextStatus"] = lot.NextStatus;
                    dr["OrderDate"] = lot.OrderDate;
                    dr["CancelDate"] = (lot.CancelDate == new DateTime(1900, 1, 1))
                                           ? SqlDateTime.Null
                                           : (SqlDateTime) lot.CancelDate;
                    dr["OrderQty"] = lot.OrderQty;
                    dr["ReceivedQty"] = lot.ReceivedQty;
                    dr["OpenQty"] = lot.OpenQty;
                    dr["SupplierPlant"] = lot.SupplierPlantNum;
                    dr["SupplierOrderNum"] = lot.SupplierOrderNum;
                    dr["SupplierOrderType"] = lot.SupplierOrderType;
                    dr["SupplierOrderLineNum"] = lot.SupplierOrderLineNum;
                    dr["ProcessId"] = lot.ProcessId;
                    dr["EventDate"] = lot.EventDate;
                    ltTable.Rows.Add(dr);
                    foreach (RelatedOrderLine relOrderLine in lot.RelatedOrderLines)
                    {
                        DataRow dr1 = woTable.NewRow();
                        dr1["LotOrderNum"] = lot.LotOrderNum;
                        dr1["LotOrderType"] = lot.LotOrderType;
                        dr1["LotOrderLineNum"] = lot.LotOrderLineNum;
                        dr1["RelatedLiteNum"] = relOrderLine.LiteNum;
                        dr1["RelatedTranType"] = relOrderLine.TransactionType;
                        dr1["WorkOrderDate"] = relOrderLine.WorkOrder.OrderDate;
                        dr1["WorkOrderNum"] = relOrderLine.WorkOrder.WorkOrderNum;
                        dr1["WorkOrderStatus"] = relOrderLine.WorkOrder.StatusCode;
                        dr1["WorkOrderQty"] = relOrderLine.WorkOrder.Quantity;
                        dr1["ItemNum"] = relOrderLine.WorkOrder.ItemNum;
                        dr1["UnitType"] = relOrderLine.WorkOrder.UnitType;
                        dr1["EventDate"] = lot.EventDate;
                        woTable.Rows.Add(dr1);
                        if (relOrderLine.WorkOrder.RelatedSalesOrderLines != null)
                        {
                            foreach (RelatedSalesOrderLine line in relOrderLine.WorkOrder.RelatedSalesOrderLines)
                            {
                                DataRow dr2 = soTable.NewRow();
                                dr2["WorkOrderNum"] = relOrderLine.WorkOrder.WorkOrderNum;
                                dr2["SalesOrderNum"] = line.OrderNum;
                                dr2["SalesOrderType"] = line.OrderType;
                                dr2["SalesOrderLineNum"] = line.OrderLineNum;
                                dr2["OrderQty"] = line.OrderQty;
                                dr2["OrderDate"] = line.OrderDate;
                                dr2["RequestedDate"] = line.RequestedDate;
                                dr2["SoldToCustomerNum"] = line.SoldToCustomer.CustomerNum;
                                dr2["SoldToCustomerName"] = line.SoldToCustomer.CustomerName;
                                dr2["ShipToCustomerNum"] = line.ShipToCustomer.CustomerNum;
                                dr2["ShipToCustomerName"] = line.ShipToCustomer.CustomerName;
                                dr2["EventDate"] = lot.EventDate;
                                soTable.Rows.Add(dr2);
                            }
                        }
                    }
                }
            }
        }

        /**********************************************************************
         *  Persist Loads
         *********************************************************************/
        public SqlBulkCopyResponse PersistPurchasedLotLoads(BusinessSystemSession e1Session, string plantCode, PurchasedLots lots)
        {
            SqlBulkCopyResponse response;
            var plantNum = PlantManager.GetPlant(plantCode).PlantNum;
            try
            {
                var dt = GetLoadsTable("PurchasedLotLoadsLedger");
                AddLoadRows(dt, lots);
                var bc = new SqlBulkCopyManager();
                var cn = ConnectionManager.GetSqlConnectionFromKey(plantNum + ".INTEGRATION");
                bc.DbConnectionString = cn.ConnectionString;
                response = bc.BulkTable("PurchasedLotLoadsLedger", dt);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.Message);
            }
            return response;
        }

        public DataTable GetLoadsTable(string tableName)
        {
            var dt = new DataTable(tableName);
            dt.Columns.Add("LotOrderNum", typeof(decimal));
            dt.Columns.Add("LotOrderType", typeof(string));
            dt.Columns.Add("LotOrderLineNum", typeof(decimal));
            dt.Columns.Add("SupplierPlant", typeof(string));
            dt.Columns.Add("LoadNum", typeof(decimal));
            dt.Columns.Add("LoadDate", typeof(SqlDateTime));
            dt.Columns.Add("EventDate", typeof(SqlDateTime));
            return dt;
        }

        public void AddLoadRows(DataTable dt, PurchasedLots lots)
        {
            foreach (PurchasedLot lot in lots)
            {
                DataRow dr = dt.NewRow();
                dr["LotOrderNum"] = lot.LotOrderNum;
                dr["LotOrderType"] = lot.LotOrderType;
                dr["LotOrderLineNum"] = lot.LotOrderLineNum;
                dr["SupplierPlant"] = lot.SupplierPlantNum;
                dr["LoadNum"] = lot.LoadNum;
                dr["LoadDate"] = (lot.LoadDate == new DateTime(1900, 1, 1)) ? SqlDateTime.Null : (SqlDateTime)lot.LoadDate;
                dr["EventDate"] = (lot.EventDate == new DateTime(1900, 1, 1)) ? SqlDateTime.Null : (SqlDateTime)lot.EventDate;
                dt.Rows.Add(dr);
            }
        }

        /**********************************************************************
         *  Persist Receipts
         *********************************************************************/
        public SqlBulkCopyResponse PersistPurchasedLotReceipts(BusinessSystemSession e1Session, string plantCode, PurchasedLots lots)
        {
            SqlBulkCopyResponse response;
            var plantNum = PlantManager.GetPlant(plantCode).PlantNum;
            try
            {
                var dt = GetReceiptsTable("PurchasedLotReceiptsLedger");
                AddReceiptRows(dt, lots);
                var bc = new SqlBulkCopyManager();
                var cn = ConnectionManager.GetSqlConnectionFromKey(plantNum + ".INTEGRATION");
                bc.DbConnectionString = cn.ConnectionString;
                response = bc.BulkTable("PurchasedLotReceiptsLedger", dt);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.Message);
            }
            return response;
        }

        public DataTable GetReceiptsTable(string tableName)
        {
            var dt = new DataTable(tableName);
            dt.Columns.Add("LotOrderNum", typeof(decimal));
            dt.Columns.Add("LotOrderType", typeof(string));
            dt.Columns.Add("LotOrderLineNum", typeof(decimal));
            dt.Columns.Add("LotNum", typeof(string));
            dt.Columns.Add("LoadNum", typeof(decimal));
            dt.Columns.Add("RackId", typeof(decimal));
            dt.Columns.Add("SerialNum", typeof(string));
            dt.Columns.Add("RackQty", typeof(decimal));
            dt.Columns.Add("EventDate", typeof(DateTime));
            return dt;
        }

        public void AddReceiptRows(DataTable dt, PurchasedLots lots)
        {
            foreach (PurchasedLot lot in lots)
            {
                DataRow dr = dt.NewRow();
                dr["LotOrderNum"] = lot.LotOrderNum;
                dr["LotOrderType"] = lot.LotOrderType;
                dr["LotOrderLineNum"] = lot.LotOrderLineNum;
                dr["LotNum"] = lot.LotNum;
                dr["LoadNum"] = lot.LoadNum;
                dr["RackId"] = lot.RackId;
                dr["SerialNum"] = lot.SerialNum;
                dr["RackQty"] = lot.RackQty;
                dr["EventDate"] = lot.EventDate;
                dt.Rows.Add(dr);
            }
        }

        /**********************************************************************
        *  High Water Marks
        *********************************************************************/
        public SqlCommand GetHighWaterMark()
        {
            var cmd = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = @"
                    SELECT
                      MarkName,
                      EnterpriseDateTime,
                      LocalDateTime,
                      UniqueId
                    FROM
                      HighWaterMarks
                    WHERE
                      MarkName = @MarkName"
            };
            cmd.Parameters.Add(new SqlParameter("MarkName", SqlDbType.VarChar, 30));
            return cmd;
        }

        public HighWaterMark GetHighWaterMark(string plantCode, string markName)
        {
            var hw = new HighWaterMark();
            var dt = DateTime.Now;
            var plantNum = PlantManager.GetPlant(plantCode).PlantNum;
            var cn = ConnectionManager.GetSqlConnectionFromKey(plantNum + ".INTEGRATION");
            try
            {
                cn.Open();
                var cmd = GetHighWaterMark();
                cmd.Parameters[0].Value = markName;
                cmd.Connection = cn;
                Logger.Debug(Logging.DbCmdString(cmd));
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
                while (reader.Read())
                {
                    try
                    {
                        hw.HighWaterMarkName = CastDbNull.To<string>(reader["MarkName"], String.Empty).Trim();
                        hw.EnterpriseDateTime = CastDbNull.To<DateTime>(reader["EnterpriseDateTime"], new DateTime(1900, 1, 1));
                        hw.LocalDateTime = CastDbNull.To<DateTime>(reader["LocalDateTime"], new DateTime(1900, 1, 1));
                        hw.UniqueID = CastDbNull.To<long>(reader["UniqueId"], 0);
                    }
                    catch (Exception e)
                    {
                        throw new ApplicationException(e.ToString());
                    }
                }
                reader.Close();
            }
            catch (Exception err)
            {
                throw new Exception(err.ToString());
            }
            finally
            {
                cn.Close();
            }
            return hw;
        }

        public SqlCommand SetHighWaterMark()
        {
            var cmd = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = @"
                    UPDATE
                      HighWaterMarks
                    SET
                      EnterpriseDateTime = @EnterpriseDateTime,
                      LocalDateTime = @LocalDateTime,
                        UniqueId = @UniqueId,
                      WrittenDate = GETDATE()
                    WHERE
                      MarkName = @MarkName"
            };
            cmd.Parameters.Add(new SqlParameter("@EnterpriseDateTime", SqlDbType.DateTime));
            cmd.Parameters.Add(new SqlParameter("@LocalDateTime", SqlDbType.DateTime));
            cmd.Parameters.Add(new SqlParameter("@UniqueId", SqlDbType.BigInt));
            cmd.Parameters.Add(new SqlParameter("@MarkName", SqlDbType.VarChar, 30));
            return cmd;
        }

        public SqlDmlResponse SetHighWaterMark(BusinessSystemSession e1Session, string plantCode, string markName, DateTime enterpriseDateTime, Int64 uniqueId)
        {
            var response = new SqlDmlResponse();
            var cn = new SqlConnection();
            var plant = PlantManager.GetPlant(plantCode);
            var plantNum = plant.PlantNum;
            var localDateTime = enterpriseDateTime.AddHours(plant.TimeOffset);
            try
            {
                cn = ConnectionManager.GetSqlConnectionFromKey(plantNum + ".INTEGRATION");
                cn.Open();
                var cmd = SetHighWaterMark();
                cmd.Parameters[0].Value = enterpriseDateTime;
                cmd.Parameters[1].Value = localDateTime;
                cmd.Parameters[2].Value = uniqueId;
                cmd.Parameters[3].Value = markName;
                cmd.Connection = cn;
                Logger.Debug(Logging.DbCmdString(cmd));
                response.Init(cmd);
                int rows = cmd.ExecuteNonQuery();
                response.RowsAffected = rows;
            }
            catch (Exception err)
            {
                throw new ApplicationException(err.ToString());
            }
            finally
            {
                cn.Close();
            }
            return response;
        }
    }
}
