using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using IGServices.Common.Managers.Enterprise;
using IGServices.Common.Managers.ShopFloor;
using IGServices.Common.Types.E1;
using IGServices.Common.Types.Enterprise;
using IGServices.Common.Types.Response;
using IGServices.Common.Utilities;
//using NLog;

namespace IGServices.Common.Managers.Response
{
    public class MTOScheduleDownloadResponseManager
    {
        //private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public void Init(MTOScheduleDownloadResponse response, string plantCode, decimal scheduleNum, decimal scheduleDateE1, string batchNum, int verbosityMode, bool write, bool force, string queryString = null)
        {
            //var responseTimesMgr = new ResponseTimesManager();
            //responseTimesMgr.OpenResponseTime(response.ResponseTimes, response.ProcessName, "Job", "");
            var plantAttribute = new PlantAttribute(plantCode);
            response.PlantNum = plantAttribute.NumericCode;
            response.PlantCode = plantAttribute.Code;
            response.ScheduleNum = scheduleNum;
            response.ScheduleDateE1 = scheduleDateE1;
            response.BatchNum = batchNum;
            response.Message = "";
            response.VerbosityMode = verbosityMode;
            response.Write = write;
            response.Write = write;
            response.Force = force;
        }

        public void BuildShopFloorMTOOrders(MTOScheduleDownloadResponse response, BusinessSystemSession e1Session, SqlConnection cnCP)
        {
            //var responseTimesMgr = new ResponseTimesManager();
            var plantNum = response.PlantNum;
            var scheduleNum = response.ScheduleNum;
            try
            {
                //responseTimesMgr.OpenResponseTime(response.ResponseTimes, "BuildMTOScheduleHeader", "E1", e1Session.DbConnection.ConnectionString);
                var scheduleMgr = new MTOScheduleManager(e1Session);
                response.MTOSchedule = scheduleMgr.BuildMTOScheduleHeader(e1Session, plantNum, scheduleNum);
                //responseTimesMgr.CloseResponseTime(response.ResponseTimes, "BuildMTOScheduleHeader", "E1", e1Session.DbConnection.ConnectionString);

                if (response.MTOSchedule.ScheduleNum > 0)
                {
                    var scheduleDateE1 = response.MTOSchedule.ScheduleDateE1;
                    //responseTimesMgr.OpenResponseTime(response.ResponseTimes, "LoadCacheFromSchedule", "E1", e1Session.DbConnection.ConnectionString);
                    var cache = new ItemAttributeCache();
                    cache.LoadCacheFromSchedule(e1Session, plantNum, scheduleNum, scheduleDateE1);
                    //responseTimesMgr.CloseResponseTime(response.ResponseTimes, "LoadCacheFromSchedule", "E1", e1Session.DbConnection.ConnectionString);

                    //responseTimesMgr.OpenResponseTime(response.ResponseTimes, "GetMTOScheduleLines", "E1", e1Session.DbConnection.ConnectionString);
                    response.MTOSchedule.MTOScheduleLines = scheduleMgr.BuildMTOScheduleLines(e1Session, plantNum, scheduleNum, scheduleDateE1, cache);
                    //responseTimesMgr.CloseResponseTime(response.ResponseTimes, "GetMTOScheduleLines", "E1", e1Session.DbConnection.ConnectionString);

                    //responseTimesMgr.OpenResponseTime(response.ResponseTimes, "BuildScheduleLineShipToCustomer", "E1", e1Session.DbConnection.ConnectionString);
                    var customerMgr = new CustomerManager(e1Session);
                    foreach (var line in response.MTOSchedule.MTOScheduleLines)
                    {
                        line.ShipToCustomer = customerMgr.GetCustomer(e1Session, line.ShipToCustNum);
                    }
                    //responseTimesMgr.CloseResponseTime(response.ResponseTimes, "BuildScheduleLineShipToCustomer", "E1", e1Session.DbConnection.ConnectionString);

                    //responseTimesMgr.OpenResponseTime(response.ResponseTimes, "BuildWorkOrder", "E1", e1Session.DbConnection.ConnectionString);
                    foreach (var line in response.MTOSchedule.MTOScheduleLines)
                    {
                        line.WorkOrder = scheduleMgr.BuildWorkOrder(e1Session, line.WorkOrderNum, cache);
                    }
                    //responseTimesMgr.CloseResponseTime(response.ResponseTimes, "BuildWorkOrder", "E1", e1Session.DbConnection.ConnectionString);

                    //responseTimesMgr.OpenResponseTime(response.ResponseTimes, "BuildRelatedSalesOrderLine", "E1", e1Session.DbConnection.ConnectionString);
                    foreach (var line in response.MTOSchedule.MTOScheduleLines)
                    {
                        //line.WorkOrder.RelatedSalesOrderLine = scheduleMgr.BuildRelatedSalesOrderLine(e1Session, line.WorkOrder.RelatedOrderNum, line.WorkOrder.RelatedOrderType, line.WorkOrder.RelatedOrderCompany, line.WorkOrder.RelatedOrderLineNum, sqlResponse);
                        line.WorkOrder.RelatedSalesOrderLine = scheduleMgr.BuildSalesOrderLine(e1Session, line.WorkOrder.RelatedOrderNum, line.WorkOrder.RelatedOrderType, line.WorkOrder.RelatedOrderCompany, line.WorkOrder.RelatedOrderLineNum);
                        line.WorkOrder.RelatedSalesOrderLine.PlantNum = line.WorkOrder.PlantNum;
                        line.SoldToCustNum = line.WorkOrder.RelatedSalesOrderLine.SoldToCustNum;
                    }
                    //responseTimesMgr.CloseResponseTime(response.ResponseTimes, "BuildRelatedSalesOrderLine", "E1", e1Session.DbConnection.ConnectionString);

                    //responseTimesMgr.OpenResponseTime(response.ResponseTimes, "BuildScheduleLineSoldToCustomer", "E1", e1Session.DbConnection.ConnectionString);
                    foreach (var line in response.MTOSchedule.MTOScheduleLines)
                    {
                        line.SoldToCustomer = customerMgr.GetCustomer(e1Session, line.SoldToCustNum);
                    }
                    //responseTimesMgr.CloseResponseTime(response.ResponseTimes, "BuildScheduleLineSoldToCustomer", "E1", e1Session.DbConnection.ConnectionString);

                    //responseTimesMgr.OpenResponseTime(response.ResponseTimes, "BuildRelatedSalesOrderLineSoldToCustomer", "E1", e1Session.DbConnection.ConnectionString);
                    foreach (var line in response.MTOSchedule.MTOScheduleLines)
                    {
                        line.WorkOrder.RelatedSalesOrderLine.SoldToCustomer = customerMgr.GetCustomer(e1Session, line.WorkOrder.RelatedSalesOrderLine.SoldToCustNum);
                    }
                    //responseTimesMgr.CloseResponseTime(response.ResponseTimes, "BuildRelatedSalesOrderLineSoldToCustomer", "E1", e1Session.DbConnection.ConnectionString);

                    //responseTimesMgr.OpenResponseTime(response.ResponseTimes, "BuildRelatedSalesOrderLineShipToCustomer", "E1", e1Session.DbConnection.ConnectionString);
                    foreach (var line in response.MTOSchedule.MTOScheduleLines)
                    {
                        line.WorkOrder.RelatedSalesOrderLine.ShipToCustomer = customerMgr.GetCustomer(e1Session, line.WorkOrder.RelatedSalesOrderLine.ShipToCustNum);
                    }
                    //responseTimesMgr.CloseResponseTime(response.ResponseTimes, "BuildRelatedSalesOrderLineShipToCustomer", "E1", e1Session.DbConnection.ConnectionString);

                    //responseTimesMgr.OpenResponseTime(response.ResponseTimes, "GetEdiMessages", "E1", e1Session.DbConnection.ConnectionString);
                    var ediMessageMgr = new EdiMessageManager(e1Session);
                    foreach (var line in response.MTOSchedule.MTOScheduleLines)
                    {
                        var ediMessages = ediMessageMgr.GetEdiMessages(e1Session, line.WorkOrder.RelatedSalesOrderLine.OrderCompany, line.WorkOrder.RelatedSalesOrderLine.OrderNum, line.WorkOrder.RelatedSalesOrderLine.OrderType, line.WorkOrder.RelatedSalesOrderLine.OrderLineNum * 1000);
                        if (ediMessages.Count > 0)
                        {
                            line.WorkOrder.RelatedSalesOrderLine.EdiMessages = ediMessages;
                        }
                    }
                    //responseTimesMgr.CloseResponseTime(response.ResponseTimes, "GetEdiMessages", "E1", e1Session.DbConnection.ConnectionString);

                    //responseTimesMgr.OpenResponseTime(response.ResponseTimes, "GetCratesDescDynamic", "E1", e1Session.DbConnection.ConnectionString);
                    var crateMgr = new CrateManager(e1Session);
                    var allCrates = crateMgr.GetCratesDescDynamic(e1Session, response.MTOSchedule.MTOScheduleLines);
                    //responseTimesMgr.CloseResponseTime(response.ResponseTimes, "GetCratesDescDynamic", "E1", e1Session.DbConnection.ConnectionString);

                    //if (allCrates.Any())
                    //{
                    //responseTimesMgr.OpenResponseTime(response.ResponseTimes, "MatchCrates", "Processing", "In memory");
                    foreach (var line in response.MTOSchedule.MTOScheduleLines)
                    {
                        var cratesFound = allCrates.FindAll(item => item.CRDOCO == line.WorkOrder.RelatedOrderNum &&
                                                                    item.CRDCTO == line.WorkOrder.RelatedSalesOrderLine.OrderType &&
                                                                    item.CRKCOO == line.WorkOrder.RelatedSalesOrderLine.OrderCompany &&
                                                                    item.CRLNID == line.WorkOrder.RelatedSalesOrderLine.OrderLineNum * 1000);
                        if (cratesFound.Count > 0)
                        {
                            foreach (var crateFound in cratesFound)
                            {
                                var crate = new Crate
                                {
                                    OrderNum = crateFound.CRDOCO,
                                    OrderType = crateFound.CRDCTO,
                                    OrderCompany = crateFound.CRKCOO,
                                    OrderLineNum = crateFound.CRLNID,
                                    OrderQuantity = crateFound.CRUORG,
                                    CrateId = crateFound.CRZ58CRID,
                                    CrateType = crateFound.CRZ58CRTY,
                                    Quantity = crateFound.CRZ58CRQ,
                                    Message = crateFound.CRZ58CRMSG,
                                    Description = crateFound.DRDL01
                                };
                                line.WorkOrder.RelatedSalesOrderLine.Crate = crate;
                            }
                        }
                        else
                        {
                            var crate = new Crate
                            {
                                OrderNum = line.WorkOrder.RelatedOrderNum,
                                OrderType = line.WorkOrder.RelatedSalesOrderLine.OrderType,
                                OrderCompany = line.WorkOrder.RelatedSalesOrderLine.OrderCompany,
                                OrderLineNum = line.WorkOrder.RelatedSalesOrderLine.OrderLineNum * 1000
                            };
                            line.WorkOrder.RelatedSalesOrderLine.Crate = crate;
                        }
                    }
                    //responseTimesMgr.CloseResponseTime(response.ResponseTimes, "MatchCrates", "Processing", "In memory");
                    //}
                    //responseTimesMgr.OpenResponseTime(response.ResponseTimes, "BuildGlassItem", "E1", e1Session.DbConnection.ConnectionString);
                    var glassItemMgr = new GlassItemManager(e1Session);
                    foreach (var line in response.MTOSchedule.MTOScheduleLines)
                    {
                        line.WorkOrder.GlassItem = line.WorkOrder.StockingType == "C" ? glassItemMgr.BuildGlassItem(e1Session, line.WorkOrder.ItemNum, line.WorkOrder.PlantCode, line.WorkOrder.WorkOrderNum, "WO", 0, "*", cache) : glassItemMgr.BuildGlassItem(e1Session, line.WorkOrder.ItemNum, line.WorkOrder.PlantCode);
                        line.WorkOrder.RelatedSalesOrderLine.OriginalSoldToCustNum = line.WorkOrder.GlassItem.OriginalSoldToCustomerNum;
                        line.WorkOrder.RelatedSalesOrderLine.OriginalShipToCustNum = line.WorkOrder.GlassItem.OriginalShipToCustomerNum;
                        line.WorkOrder.RelatedSalesOrderLine.UnitType = line.WorkOrder.GlassItem.UnitType;
                        line.WorkOrder.RelatedSalesOrderLine.UnitTypeDesc = line.WorkOrder.GlassItem.UnitTypeDesc;
                    }
                    //responseTimesMgr.CloseResponseTime(response.ResponseTimes, "BuildGlassItem", "E1", e1Session.DbConnection.ConnectionString);

                    //responseTimesMgr.OpenResponseTime(response.ResponseTimes, "GetItemAttributes", "E1", e1Session.DbConnection.ConnectionString);
                    var itemAttributesManager = new ItemAttributeManager(e1Session);
                    foreach (var line in response.MTOSchedule.MTOScheduleLines)
                    {
                        var attributes = itemAttributesManager.GetItemAttributes(e1Session, line.WorkOrder.ItemNum, line.WorkOrder.WorkOrderNum, "WO", 0, "*", cache);
                        line.WorkOrder.GlassItem.ItemAttributes = attributes;
                        line.WorkOrder.RelatedSalesOrderLine.CustomerLiteNum = line.WorkOrder.GlassItem.ItemAttributes.GetDecimalAttributeValue("Z58L1CLN");
                    }
                    //responseTimesMgr.CloseResponseTime(response.ResponseTimes, "GetItemAttributes", "E1", e1Session.DbConnection.ConnectionString);

                    //responseTimesMgr.OpenResponseTime(response.ResponseTimes, "GetGlassItemCustomer", "E1", e1Session.DbConnection.ConnectionString);
                    var customerManager = new CustomerManager(e1Session);
                    foreach (var line in response.MTOSchedule.MTOScheduleLines)
                    {
                        var customerNum = line.WorkOrder.GlassItem.ItemAttributes.GetDecimalAttributeValue("AN8DR");
                        line.WorkOrder.GlassItem.Customer = customerManager.GetCustomer(e1Session, customerNum);
                    }
                    //responseTimesMgr.CloseResponseTime(response.ResponseTimes, "GetGlassItemCustomer", "E1", e1Session.DbConnection.ConnectionString);

                    //responseTimesMgr.OpenResponseTime(response.ResponseTimes, "GetCustomerItemReference", "E1", e1Session.DbConnection.ConnectionString);
                    foreach (var line in response.MTOSchedule.MTOScheduleLines)
                    {
                        var customerNum = line.WorkOrder.GlassItem.ItemAttributes.GetDecimalAttributeValue("AN8DR");
                        line.WorkOrder.GlassItem.CustomerItemReference = customerManager.GetCustomerItemReference(e1Session, customerNum, line.WorkOrder.ItemNum);
                    }
                    //responseTimesMgr.CloseResponseTime(response.ResponseTimes, "GetCustomerItemReference", "E1", e1Session.DbConnection.ConnectionString);

                    //responseTimesMgr.OpenResponseTime(response.ResponseTimes, "EnrichItem", "E1", e1Session.DbConnection.ConnectionString);
                    foreach (var line in response.MTOSchedule.MTOScheduleLines)
                    {
                        if (line.WorkOrder.GlassItem.ItemAttributes.Count > 0) glassItemMgr.EnrichItem(e1Session, line.WorkOrder.GlassItem, line.WorkOrder.GlassItem.ItemAttributes);
                    }
                    //responseTimesMgr.CloseResponseTime(response.ResponseTimes, "EnrichItem", "E1", e1Session.DbConnection.ConnectionString);

                    //responseTimesMgr.OpenResponseTime(response.ResponseTimes, "GetOriginalSoldToCustomer", "E1", e1Session.DbConnection.ConnectionString);
                    foreach (var line in response.MTOSchedule.MTOScheduleLines)
                    {
                        line.WorkOrder.RelatedSalesOrderLine.OriginalSoldToCustomer = customerMgr.GetCustomer(e1Session, line.WorkOrder.RelatedSalesOrderLine.OriginalSoldToCustNum);
                    }
                    //responseTimesMgr.CloseResponseTime(response.ResponseTimes, "GetOriginalSoldToCustomer", "E1", e1Session.DbConnection.ConnectionString);

                    //responseTimesMgr.OpenResponseTime(response.ResponseTimes, "BuildRelatedSalesOrderLineOriginalShipToCustomer", "E1", e1Session.DbConnection.ConnectionString);
                    foreach (var line in response.MTOSchedule.MTOScheduleLines)
                    {
                        line.WorkOrder.RelatedSalesOrderLine.OriginalShipToCustomer = customerMgr.GetCustomer(e1Session, line.WorkOrder.RelatedSalesOrderLine.OriginalShipToCustNum);
                    }
                    //responseTimesMgr.CloseResponseTime(response.ResponseTimes, "BuildRelatedSalesOrderLineOriginalShipToCustomer", "E1", e1Session.DbConnection.ConnectionString);

                    //responseTimesMgr.OpenResponseTime(response.ResponseTimes, "BuildWorkOrderRoutings", "E1", e1Session.DbConnection.ConnectionString);
                    foreach (var line in response.MTOSchedule.MTOScheduleLines)
                    {
                        line.WorkOrder.Routings = scheduleMgr.BuildWorkOrderRoutings(e1Session, line.WorkOrder.WorkOrderNum);
                    }
                    //responseTimesMgr.CloseResponseTime(response.ResponseTimes, "BuildWorkOrderRoutings", "E1", e1Session.DbConnection.ConnectionString);

                    //responseTimesMgr.OpenResponseTime(response.ResponseTimes, "GetShopFloorMTOOrders", "Processing", "In memory");
                    var shopFloorMTOScheduleMgr = new ShopFloorMTOOrdersManager();
                    response.MTOSchedule.ShopFloorMTOOrders = shopFloorMTOScheduleMgr.GetShopFloorMTOOrders(response.MTOSchedule);
                    //responseTimesMgr.CloseResponseTime(response.ResponseTimes, "GetShopFloorMTOOrders", "Processing", "In memory");

                    var validationMgr = new ShopFloorMTOOrdersManager();
                    validationMgr.Validate(response, e1Session, cnCP);
                }
            }
            catch (Exception e)
            {
                response.Errors = ErrorManager.AddError(response.Errors, e);
            }
        }

        public void CloseMTOIncomingOrders(MTOScheduleDownloadResponse response, bool isNotify = false)
        {
            response.Message = (!response.MTOSchedule.IsValid) ? response.MTOSchedule.ValidationStatus : response.TransferMessage;
            response.MTOSchedule.Lines = response.MTOSchedule.MTOIncomingOrderLines?.Count ?? 0;
            if (response.MTOSchedule.Lines > 0) SummarizeMTOIncomingOrders(response);
            //var responseTimesMgr = new ResponseTimesManager();
            //responseTimesMgr.CloseResponseTime(response.ResponseTimes, response.ProcessName, "Job", "");
            if (isNotify) NotifyMTOIncomingOrders(response);
            SetVerbosity(response);
        }

        public void CloseShopFloorMTOOrders(MTOScheduleDownloadResponse response, bool isNotify = false)
        {
            response.Message = response.TransferMessage;
            response.MTOSchedule.Lines = response.MTOSchedule.ShopFloorMTOOrders.Count;
            if (response.MTOSchedule.Lines > 0) SummarizeShopFloorMTOOrders(response);
            //var responseTimesMgr = new ResponseTimesManager();
            //responseTimesMgr.CloseResponseTime(response.ResponseTimes, response.ProcessName, "Job", "");
            if (isNotify) NotifyShopFloorMTOOrders(response);
            SetVerbosity(response);
        }

        public void SummarizeMTOIncomingOrders(MTOScheduleDownloadResponse response)
        {
            SummarizeMTOIncomingOrdersLines(response);
            SummarizeMTOIncomingOrdersUnits(response);
            SummarizeMTOIncomingOrdersWorkOrders(response);
            SummarizeMTOIncomingOrdersSalesOrders(response);
            SummarizeMTOIncomingOrdersCustomers(response);
            SummarizeMTOIncomingOrdersCustomerLines(response);
        }

        //public void SummarizeRawGlassItems(MTOScheduleDownloadResponse response)
        //{
        //    SummarizeRawGlassItemsLines(response);
        //}

        //public void SummarizeRawGlassItemsLines(MTOScheduleDownloadResponse response)
        //{
        //    try
        //    {
        //        response.ScheduleSummary.RawGlassItems = Convert.ToInt16(response.MTOSchedule.ShopFloorRawGlassItems.Count);
        //    }
        //    catch (Exception e)
        //    {
        //        response.Errors = ErrorManager.AddError(response.Errors, e);
        //    }
        //}

        public void SummarizeMTOIncomingOrdersLines(MTOScheduleDownloadResponse response)
        {
            try
            {
                response.ScheduleSummary.Lines = Convert.ToInt16(response.MTOSchedule.MTOIncomingOrderLines.Count);
            }
            catch (Exception e)
            {
                response.Errors = ErrorManager.AddError(response.Errors, e);
            }
        }

        public void SummarizeMTOIncomingOrdersUnits(MTOScheduleDownloadResponse response)
        {
            try
            {
                response.ScheduleSummary.Units = Convert.ToInt16(response.MTOSchedule.MTOIncomingOrderLines.Select(x => x.Quantity).Sum());
            }
            catch (Exception e)
            {
                response.Errors = ErrorManager.AddError(response.Errors, e);
            }
        }

        public void SummarizeMTOIncomingOrdersWorkOrders(MTOScheduleDownloadResponse response)
        {
            try
            {
                response.ScheduleSummary.WorkOrders = Convert.ToInt16(response.MTOSchedule.MTOIncomingOrderLines.Select(x => x.WOrderNum).Distinct().Count());
            }
            catch (Exception e)
            {
                response.Errors = ErrorManager.AddError(response.Errors, e);
            }
        }

        public void SummarizeMTOIncomingOrdersSalesOrders(MTOScheduleDownloadResponse response)
        {
            try
            {
                response.ScheduleSummary.SalesOrders = Convert.ToInt16(response.MTOSchedule.MTOIncomingOrderLines.Select(x => x.SOrderNum).Distinct().Count());
            }
            catch (Exception e)
            {
                response.Errors = ErrorManager.AddError(response.Errors, e);
            }
        }

        public void SummarizeMTOIncomingOrdersCustomers(MTOScheduleDownloadResponse response)
        {
            try
            {
                response.ScheduleSummary.Customers = Convert.ToInt16(response.MTOSchedule.MTOIncomingOrderLines.Select(x => x.CustNum).Distinct().Count());
            }
            catch (Exception e)
            {
                response.Errors = ErrorManager.AddError(response.Errors, e);
            }
        }

        public void SummarizeMTOIncomingOrdersCustomerLines(MTOScheduleDownloadResponse response)
        {
            var scheduleCustomerLines = new ScheduleCustomerLines();
            try
            {
                var customers = from i in response.MTOSchedule.MTOIncomingOrderLines
                                group i by new
                                {
                                    i.CustNum,
                                    i.CustName
                                }
                    into g
                                select new
                                {
                                    Keys = g.Key,
                                    Values = g,
                                    g.Key.CustNum,
                                    g.Key.CustName,
                                    WorkOrders = g.Select(x => x.WOrderNum).Distinct().Count(),
                                    SalesOrders = g.Select(x => x.SOrderNum).Distinct().Count(),
                                    UnitsQty = g.Sum(x => x.Quantity)
                                };
                scheduleCustomerLines.AddRange(customers.Select(customer => new ScheduleCustomerLine
                {
                    CustNum = customer.CustNum,
                    CustName = customer.CustName,
                    WorkOrders = customer.WorkOrders,
                    SalesOrders = customer.SalesOrders,
                    UnitsQty = (int)customer.UnitsQty
                }));
            }
            catch (Exception e)
            {
                response.Errors = ErrorManager.AddError(response.Errors, e);
            }
            response.ScheduleSummary.ScheduleCustomerLines = scheduleCustomerLines;
        }

        public void SummarizeShopFloorMTOOrders(MTOScheduleDownloadResponse response)
        {
            SummarizeShopFloorMTOOrdersLines(response);
            SummarizeShopFloorMTOOrdersUnits(response);
            SummarizeShopFloorMTOOrdersWorkOrders(response);
            SummarizeShopFloorMTOOrdersSalesOrders(response);
            SummarizeShopFloorMTOOrdersCustomers(response);
            SummarizeShopFloorMTOOrdersCustomerLines(response);
        }

        public void SummarizeShopFloorMTOOrdersLines(MTOScheduleDownloadResponse response)
        {
            try
            {
                response.ScheduleSummary.Lines = Convert.ToInt16(response.MTOSchedule.ShopFloorMTOOrders.Count);
            }
            catch (Exception e)
            {
                response.Errors = ErrorManager.AddError(response.Errors, e);
            }
        }

        public void SummarizeShopFloorMTOOrdersUnits(MTOScheduleDownloadResponse response)
        {
            try
            {
                response.ScheduleSummary.Units = Convert.ToInt16(response.MTOSchedule.ShopFloorMTOOrders.Select(x => x.Quantity).Sum());
            }
            catch (Exception e)
            {
                response.Errors = ErrorManager.AddError(response.Errors, e);
            }
        }

        public void SummarizeShopFloorMTOOrdersWorkOrders(MTOScheduleDownloadResponse response)
        {
            try
            {
                response.ScheduleSummary.WorkOrders = Convert.ToInt16(response.MTOSchedule.ShopFloorMTOOrders.Select(x => x.WOrderNum).Distinct().Count());
            }
            catch (Exception e)
            {
                response.Errors = ErrorManager.AddError(response.Errors, e);
            }
        }

        public void SummarizeShopFloorMTOOrdersSalesOrders(MTOScheduleDownloadResponse response)
        {
            try
            {
                response.ScheduleSummary.SalesOrders = Convert.ToInt16(response.MTOSchedule.ShopFloorMTOOrders.Select(x => x.SOrderNum).Distinct().Count());
            }
            catch (Exception e)
            {
                response.Errors = ErrorManager.AddError(response.Errors, e);
            }
        }

        public void SummarizeShopFloorMTOOrdersCustomers(MTOScheduleDownloadResponse response)
        {
            try
            {
                response.ScheduleSummary.Customers = Convert.ToInt16(response.MTOSchedule.ShopFloorMTOOrders.Select(x => x.CustNum).Distinct().Count());
            }
            catch (Exception e)
            {
                response.Errors = ErrorManager.AddError(response.Errors, e);
            }
        }

        public void SummarizeShopFloorMTOOrdersCustomerLines(MTOScheduleDownloadResponse response)
        {
            var scheduleCustomerLines = new ScheduleCustomerLines();
            try
            {
                var customers = from i in response.MTOSchedule.ShopFloorMTOOrders
                                group i by new
                                {
                                    i.CustNum,
                                    i.CustName
                                }
                    into g
                                select new
                                {
                                    Keys = g.Key,
                                    Values = g,
                                    g.Key.CustNum,
                                    g.Key.CustName,
                                    WorkOrders = g.Select(x => x.WOrderNum).Distinct().Count(),
                                    SalesOrders = g.Select(x => x.SOrderNum).Distinct().Count(),
                                    UnitsQty = g.Sum(x => x.Quantity)
                                };
                scheduleCustomerLines.AddRange(customers.Select(customer => new ScheduleCustomerLine
                {
                    CustNum = customer.CustNum,
                    CustName = customer.CustName,
                    WorkOrders = customer.WorkOrders,
                    SalesOrders = customer.SalesOrders,
                    UnitsQty = (int)customer.UnitsQty
                }));
            }
            catch (Exception e)
            {
                response.Errors = ErrorManager.AddError(response.Errors, e);
            }
            response.ScheduleSummary.ScheduleCustomerLines = scheduleCustomerLines;
        }

        /******************************************************************
        * Notification
        ******************************************************************/
        public bool NotifyShopFloorMTOOrders(MTOScheduleDownloadResponse response)
        {
            try
            {
                var xml = XmlSerializationUtility.SerializeObject(Encoding.UTF8, response);
                var stream = new MemoryStream(Encoding.Default.GetBytes(xml));
                var document = new XPathDocument(stream);
                var strmXsltGood = Assembly.GetExecutingAssembly().GetManifestResourceStream("IGServices.Common.Xslt.MTOScheduleDownloadResponseBody.xslt");
                var transform = new XslCompiledTransform();
                if (strmXsltGood != null)
                {
                    var reader1 = XmlReader.Create(strmXsltGood);
                    transform.Load(reader1);
                }
                var writer1 = new StringWriter();
                transform.Transform(document, null, writer1);
                var toPlantAddress = "";
                if (ConfigurationManager.AppSettings["SendPlantMail"] == "true") toPlantAddress = response.PlantCode + ConfigurationManager.AppSettings["PlantEmailFormat"];
                var sc = new SmtpClient
                {
                    Host = ConfigurationManager.AppSettings["SmtpHost"],
                    Timeout = Convert.ToInt16(ConfigurationManager.AppSettings["SmtpHostTimeout"])
                };
                if (response.MTOSchedule.Lines > 0)
                {
                    var msg1 = new MailMessage();
                    var toConfigAddresses1 = ConfigurationManager.AppSettings["MailConfirmToAddress"];
                    var toAddrs1 = $"{toPlantAddress}, {toConfigAddresses1}";
                    msg1.To.Add(toAddrs1);
                    msg1.From = new MailAddress(ConfigurationManager.AppSettings["MailFromAddress"]);
                    msg1.Subject = response.PlantCode + " " +
                                   "[" + response.Environment + "] " + "Schedule Downloads - " +
                                   "MTO Schedule " + (response.ScheduleNum.ToString(CultureInfo.CurrentCulture)) + " ";
                    msg1.IsBodyHtml = true;
                    msg1.Body = writer1.ToString();
                    //sc.Send(msg1);
                    sc.SendAsync(msg1, null);
                }
            }
            catch (Exception e)
            {
                response.Message = e.Message;
                response.Errors = ErrorManager.AddError(response.Errors, e);
            }
            return true;
        }

        public bool NotifyMTOIncomingOrders(MTOScheduleDownloadResponse response)
        {
            try
            {
                var xml = XmlSerializationUtility.SerializeObject(Encoding.UTF8, response);
                var stream = new MemoryStream(Encoding.Default.GetBytes(xml));
                var document = new XPathDocument(stream);
                var strmXsltGood = Assembly.GetExecutingAssembly().GetManifestResourceStream("IGServices.Common.Xslt.MTOScheduleDownloadResponseBody.xslt");
                var transform = new XslCompiledTransform();
                if (strmXsltGood != null)
                {
                    var reader1 = XmlReader.Create(strmXsltGood);
                    transform.Load(reader1);
                }
                var writer1 = new StringWriter();
                transform.Transform(document, null, writer1);
                var toPlantAddress = "";
                if (ConfigurationManager.AppSettings["SendPlantMail"] == "true") toPlantAddress = response.PlantCode + ConfigurationManager.AppSettings["PlantEmailFormat"];
                var sc = new SmtpClient
                {
                    Host = ConfigurationManager.AppSettings["SmtpHost"],
                    Timeout = Convert.ToInt16(ConfigurationManager.AppSettings["SmtpHostTimeout"])
                };
                if (response.MTOSchedule.Lines > 0)
                {
                    var msg1 = new MailMessage();
                    var toConfigAddresses1 = ConfigurationManager.AppSettings["MailConfirmToAddress"];
                    var toAddrs1 = $"{toPlantAddress}, {toConfigAddresses1}";
                    msg1.To.Add(toAddrs1);
                    msg1.From = new MailAddress(ConfigurationManager.AppSettings["MailFromAddress"]);
                    msg1.Subject = response.PlantCode + " " +
                                   "[" + response.Environment + "] " + "Schedule Downloads - " +
                                   "MTO Schedule " + (response.ScheduleNum.ToString(CultureInfo.CurrentCulture)) + " " +
                                   "Batch " + (response.MTOSchedule.BatchNum) + " ";
                    msg1.IsBodyHtml = true;
                    msg1.Body = writer1.ToString();
                    sc.SendAsync(msg1, null);
                }
            }
            catch (Exception e)
            {
                response.Message = e.Message;
                response.Errors = ErrorManager.AddError(response.Errors, e);
            }
            return true;
        }

        public void SetVerbosity(MTOScheduleDownloadResponse response)
        {
            switch (response.VerbosityMode)
            {
                case 0:
                    response.ServiceContext = null;
                    response.ConnectionStrings = null;
                    //response.ResponseTimes = null;
                    response.MTOSchedule = null;
                    response.ScheduleSummary = null;
                    break;
                case 1:
                    response.ServiceContext = null;
                    response.ConnectionStrings = null;
                    //response.ResponseTimes = null;
                    //response.MTOSchedule = null;
                    break;
                case 2:
                    response.MTOSchedule.E1ScheduleData.F580020 = null;
                    response.MTOSchedule.E1ScheduleData.F580021s = null;
                    response.MTOSchedule.E1ScheduleData.F580024s = null;
                    response.MTOSchedule.E1ScheduleData.F584715Ls = null;
                    response.MTOSchedule.MTOIncomingOrderLines = null;
                    break;
                case 3:
                    response.MTOSchedule.E1ScheduleData.F580020 = null;
                    response.MTOSchedule.E1ScheduleData.F580021s = null;
                    response.MTOSchedule.E1ScheduleData.F580024s = null;
                    response.MTOSchedule.E1ScheduleData.F584715Ls = null;
                    break;
            }
        }
    }
}
