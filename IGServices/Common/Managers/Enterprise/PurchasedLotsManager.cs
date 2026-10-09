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
    public class PurchasedLotsManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly ItemAttributeManager _itemAttributeManager;
        //private readonly GlassItemManager _glassItemManager;

        public PurchasedLotsManager(BusinessSystemSession e1Session)
        {
            var hash = new Dictionary<string, iDB2Command>();
            hash.Add("PurchasedLotsManager.GetRelatedOrderLines", GetRelatedOrderLines());
            hash.Add("PurchasedLotsManager.GetWorkOrder", GetWorkOrder());
            hash.Add("PurchasedLotsManager.GetSalesOrders", GetSalesOrders());
            foreach (string keyName in hash.Keys)
            {
                if (!e1Session.DbCommands.ContainsKey(keyName))
                {
                    e1Session.DbCommands.Add(keyName, hash[keyName]);
                }
            }
            _itemAttributeManager = new ItemAttributeManager(e1Session);
            //_glassItemManager = new GlassItemManager(e1Session);
        }

        /**********************************************************************
         *  Purchased Lots Init
         *********************************************************************/
        private static iDB2Command GetLedger(IReadOnlyCollection<string> filters)
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
                  {filterString}
                  OLITM > 0 AND  
                  EXISTS (SELECT 1 FROM F580400 WHERE OLDOCO = RODOC1 AND OLDCTO = RORCTO AND RODOCO > 0)
                ORDER BY
                  OLUPMJ ,
                  OLTDAY ,
                  OLUKID
                FOR READ ONLY"
            };
            return cmd;
        }

        private static iDB2Command GetCurrent(IReadOnlyCollection<string> filters)
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
                  PDMCU AS OLMCU,
                  PDKCOO AS OLKCOO,
                  PDDOCO AS OLDOCO,
                  PDDCTO AS OLDCTO,
                  PDLNID / 1000 AS OLLNID,
                  PDITM AS OLITM,
                  PDLOTN AS OLLOTN,
                  PDTRDJ AS OLTRDJ,
                  PDCNDJ AS OLCNDJ,
                  PDLTTR AS OLLTTR,
                  PDNXTR AS OLNXTR,
                  PDUORG / 1000 AS OLUORG,
                  PDUOPN / 1000 AS OLUOPN,
                  PDUREC / 1000 AS OLUREC,
                  PDCNID AS OLCNID,
                  PDUPMJ AS OLUPMJ,
                  PDTDAY AS OLTDAY,
                  PDPID AS OLPID,
                  PDRORN AS OLRORN,
                  PDRCTO AS OLRCTO,
                  PDRLLN / 1000 AS OLRLLN,
                  PDAN8 AS OLAN8,
                  CAST(0 AS DECIMAL(15,0)) AS OLUKID,
                  ABAC30
                FROM
                  F4311
                  LEFT OUTER JOIN F0101 
                    ON ABAN8 = PDAN8
	            WHERE 
                  {filterString} 
                  EXISTS (SELECT 1 FROM F580400 WHERE PDDOCO = RODOC1 AND PDDCTO = RORCTO AND RODOCO > 0)
                FOR READ ONLY"
            };
            return cmd;
        }

        public PurchasedLots GetPurchasedLots(BusinessSystemSession session, Plant plant, ProcessRunSpan processRunSpan)
        {
            var lots = new PurchasedLots();
            var filters = new List<string>();
            var parameters = new List<iDB2Parameter>();
            var cmd = new iDB2Command();
            try
            {
                if (processRunSpan.HighWaterMark.Exists)
                {
                    // Plant Num
                    filters.Add("OLMCU = ? AND \r\n");
                    parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
                    parameters.Last().Value = plant.PlantNum.PadLeft(12);

                    // Unique Id
                    filters.Add("OLUKID > ? AND \r\n");
                    parameters.Add(new iDB2Parameter("UKID", iDB2DbType.iDB2Decimal));
                    parameters.Last().Precision = 15;
                    parameters.Last().Scale = 0;
                    parameters.Last().Value = processRunSpan.HighWaterMark.UniqueID;

                    // Command
                    cmd = GetLedger(filters);
                    foreach (iDB2Parameter parameter in parameters)
                    {
                        cmd.Parameters.Add(parameter);
                    }
                }
                else
                {
                    // Plant Num
                    filters.Add("PDMCU = ? AND \r\n");
                    parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
                    parameters.Last().Value = plant.PlantNum.PadLeft(12);

                    // Time
                    filters.Add(E1TimeManager.GetE1DateTimeQueryString("PDUPMJ", "PDTDAY", processRunSpan.E1TimeIdSpan) + " AND \r\n");

                    // Command
                    cmd = GetCurrent(filters);
                    foreach (iDB2Parameter parameter in parameters)
                    {
                        cmd.Parameters.Add(parameter);
                    }
                }
                cmd.Connection = session.DbConnection;
                Logger.Debug(Logging.DbCmdString(cmd));

                // Reader
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
                    }
                    catch (Exception)
                    {
                        lot.SupplierOrderNum = 0;
                    }
                    lot.SupplierOrderType = CastDbNull.To(reader["OLRCTO"], string.Empty).Trim();
                    lot.SupplierOrderLineNum = Math.Round(CastDbNull.To<decimal>(reader["OLRLLN"], 0), 3);

                    lots.Add(lot);
                }
                reader.Close();

                foreach (PurchasedLot lot in lots)
                {
                    lot.ItemAttributes = _itemAttributeManager.GetItemAttributes(session, lot.ItemNum, lot.LotOrderNum,
                        lot.LotOrderType, lot.LotOrderLineNum * 1000, lot.LotOrderCompanyNum);
                    //var glassItem = _glassItemManager.GetGlassItem(session, lot.ItemNum, plant.PlantCode, lot.LotOrderNum, lot.LotOrderType, lot.LotOrderLineNum * 1000, lot.LotOrderCompanyNum);
                    //lot.GlassItem = glassItem;

                    var orders = GetRelatedOrderLines(session, lot.LotOrderNum, lot.LotOrderType, lot.LotOrderLineNum);
                    lot.RelatedOrderLines = orders;
                }
                return lots;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
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
              ROTDAY
            FOR READ ONLY";
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
                var cmd = (e1Session.DbCommands.ContainsKey("PurchasedLotsManager.GetRelatedOrderLines")) ? e1Session.DbCommands["PurchasedLotsManager.GetRelatedOrderLines"] : GetRelatedOrderLines();
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
              ATLNID = ?
            FOR READ ONLY";
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
                var cmd = (e1Session.DbCommands.ContainsKey("PurchasedLotsManager.GetWorkOrder")) ? e1Session.DbCommands["PurchasedLotsManager.GetWorkOrder"] : GetWorkOrder();
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
                ATZ58WODC = ?
            FOR READ ONLY";
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
                var cmd = (e1Session.DbCommands.ContainsKey("PurchasedLotsManager.GetSalesOrders")) ? e1Session.DbCommands["PurchasedLotsManager.GetSalesOrders"] : GetSalesOrders();
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
        public static iDB2Command GetLotShippingEvents(IReadOnlyCollection<string> filters)
        {
            var filterString = new StringBuilder();
            foreach (var filter in filters)
            {
                filterString.Append(filter);
            }
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = $@"
            SELECT
                PDMCU,
                PDDOCO,
                PDDCTO,
                PDLNID / 1000 AS PDLNID,
                PDITM,
                PDLOTN,
                LHMCU,
                LHLDNM,
                LHZ58LDS,
                LHUPMJ,
                LHTDAY,
                LHADDJ,
                LHADTM
            FROM
                F580150
                INNER JOIN F4211
                ON  LHMCU = SDMCU AND
                    LHLDNM = SDPSN AND
                    SDRORN != ''
                INNER JOIN F4311
                ON  PDKCOO = SDRKCO AND
                    PDDOCO = SDRORN AND
                    PDDCTO = SDRCTO AND
                    PDLNID = SDRLLN
            WHERE
                {filterString}
                PDLOTN != ''
            ORDER BY
                PDDOCO ,
                PDLNID
            FOR READ ONLY";
            return cmd;
        }


        public static iDB2Command GetLotShippingEvents2(IReadOnlyCollection<string> filters)
        {
            var filterString = new StringBuilder();
            foreach (var filter in filters)
            {
                filterString.Append(filter);
            }
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = $@"
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
                {filterString}
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
		        PDLNID
            FOR READ ONLY";
            return cmd;
        }

        /**********************************************************************
         *  LG ASN Events
         *********************************************************************/
        public iDB2Command GetLotAsnEvents(IReadOnlyCollection<string> filters)
        { 
            var filterString = new StringBuilder();
            foreach (var filter in filters)
            {
                filterString.Append(filter);
            }
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = $@"
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
                {filterString}
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
              PDLNID
            FOR READ ONLY";
            return cmd;
        }

        public PurchasedLots GetPurchasedLotLoads(BusinessSystemSession e1Session, Plant plant, ProcessRunSpan processRunSpan)
        {
            var lots = new PurchasedLots();
            try
            {
                var filters = new List<string>();
                var parameters = new List<iDB2Parameter>();

                // Plant Num
                filters.Add("PDMCU = ? AND \r\n");
                parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
                parameters.Last().Value = plant.PlantNum.PadLeft(12);

                // Time
                filters.Add(E1TimeManager.GetE1DateTimeQueryString("LHUPMJ", "LHTDAY", processRunSpan.E1TimeIdSpan) + " AND \r\n");

                // Command
                var cmd = GetLotShippingEvents(filters);
                foreach (iDB2Parameter parameter in parameters)
                {
                    cmd.Parameters.Add(parameter);
                }
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

                // LG ASN
                var filters1 = new List<string>();
                var parameters1 = new List<iDB2Parameter>();

                // Plant Num
                filters1.Add("SRMCU = ? AND \r\n");
                parameters1.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
                parameters1.Last().Value = plant.PlantNum.PadLeft(12);

                // Time
                filters1.Add(E1TimeManager.GetE1DateTimeQueryString("SRUPMJ", "SRUPMT", processRunSpan.E1TimeIdSpan) + " AND \r\n");

                // Command
                var cmd1 = GetLotAsnEvents(filters1);
                foreach (iDB2Parameter parameter in parameters1)
                {
                    cmd1.Parameters.Add(parameter);
                }
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
                throw new Exception(err.ToString());
            }
            return lots;
        }

        /**********************************************************************
         *  Receiving Events
         *********************************************************************/
        public iDB2Command GetPurchasedLotReceipts(IReadOnlyCollection<string> filters)
        {
            var filterString = new StringBuilder();
            foreach (var filter in filters)
            {
                filterString.Append(filter);
            }
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = $@"
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
                {filterString}
	            DLLOTN != '' AND
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
	            PDLNID
            FOR READ ONLY";
            return cmd;
        }

        public PurchasedLots GetPurchasedLotReceipts(BusinessSystemSession e1Session, Plant plant, ProcessRunSpan processRunSpan)
        {
            var lots = new PurchasedLots();
            try
            {
                var filters = new List<string>();
                var parameters = new List<iDB2Parameter>();

                // Plant Num
                filters.Add("DLMCU = ? AND \r\n");
                parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
                parameters.Last().Value = plant.PlantNum.PadLeft(12);

                // Time
                filters.Add(E1TimeManager.GetE1DateTimeQueryString("DLUPMJ", "DLTDAY", processRunSpan.E1TimeIdSpan) + " AND \r\n");

                // Command
                var cmd = GetPurchasedLotReceipts(filters);
                foreach (iDB2Parameter parameter in parameters)
                {
                    cmd.Parameters.Add(parameter);
                }
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
                throw new Exception(err.ToString());
            }
            return lots;
        }

    }
}
