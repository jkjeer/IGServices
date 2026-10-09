using System;
using System.Data;
using IGServices.Common.Managers.Enterprise;
using IGServices.Common.Types.E1;
using IGServices.Common.Types.Response;
using IGServices.Common.Types.ShopFloor;
using IGServices.Common.Utilities;

namespace IGServices.Common.Managers.ShopFloor
{
    public class RackLedgerManager
    {
        public RackLedgerManager(BusinessSystemSession e1Session)
        {
        }

        public RackLedgerItems GetRackLedgerItems(BusinessSystemSession e1Session, decimal rackId, string serialNum, string plantCode, string glCategory, double days, bool attributes)
        {
            var rackLedgerItems = new RackLedgerItems();
            try
            {
                var rackMgr = new Enterprise.RackManager(e1Session);
                var racks = rackMgr.GetRacks(e1Session, rackId, serialNum, plantCode, attributes);
                foreach (var rack in racks)
                {
                    if (rack.RackId != 0 && rack.RackItems.Count > 0)
                    {
                        foreach (var rackItem in rack.RackItems)
                        {
                            var tag = new RackLedgerItem
                            {
                                PlantNum = rack.CostCenter,
                                RackId = rack.RackId,
                                SerialNum = rackItem.SerialNum,
                                QtyOnHand = rackItem.Quantity,
                                Status = rackItem.DetailStatus,
                                StatusDate = rack.HeaderStatusDate,
                                Location = rack.Location,
                                ProductionDate = rackItem.ProductionDate,
                                ManufacturedDate = rackItem.ManufacturedDate,
                                ReceivedDate = rack.ReceivedDate,
                                ReceivedQty = rack.ReceivedQty,
                                UpdateDateTime = rackItem.UpdateDateTime,
                                LotNum = rackItem.LotNum,
                                HasGlassAttributes = rackItem.HasGlassAttributes,
                                ItemNum = rackItem.ItemNum,
                                RackType = rack.RackType
                            };

                            if (rackItem.HasGlassAttributes)
                            {
                                //tag.ItemNum = rackItem.GlassItem.ItemNum;
                                tag.GlCategory = rackItem.GlassItem.GLCategory;
                                tag.StockingType = rackItem.GlassItem.StockingType;
                                tag.LitesPerRack = rackItem.GlassItem.LitesPerRack;

                                var shape = rackItem.GlassItem.Shape;
                                tag.ShapeNum = shape.ShapeNum;
                                tag.SqftActual = shape.ActualSqft;
                                tag.SqftBillable = shape.BillableSqft;
                                tag.BaseLeg = shape.BaseDimension.Base;
                                tag.BaseNum = shape.BaseDimension.Numerator;
                                tag.BaseDen = shape.BaseDimension.Denominator;
                                tag.LeftLeg = shape.LeftDimension.Base;
                                tag.LeftNum = shape.LeftDimension.Numerator;
                                tag.LeftDen = shape.LeftDimension.Denominator;
                                tag.RightLeg = shape.RightDimension.Base;
                                tag.RightNum = shape.RightDimension.Numerator;
                                tag.RightDen = shape.RightDimension.Denominator;
                                tag.TopLeg = shape.TopDimension.Base;
                                tag.TopNum = shape.TopDimension.Numerator;
                                tag.TopDen = shape.TopDimension.Denominator;
                                tag.Side1Leg = shape.Side1Dimension.Base;
                                tag.Side1Num = shape.Side1Dimension.Numerator;
                                tag.Side1Den = shape.Side1Dimension.Denominator;
                                tag.Side2Leg = shape.Side2Dimension.Base;
                                tag.Side2Num = shape.Side2Dimension.Numerator;
                                tag.Side2Den = shape.Side2Dimension.Denominator;

                                var lite = rackItem.GlassItem.Lites[0];
                                tag.Lite1ProdQualityLevel = lite.QualityLevel;

                                var glassType = lite.GlassType;
                                tag.Lite1Thickness = glassType.Thickness;
                                tag.Lite1Color = glassType.Color;
                                tag.Lite1HeatTreatment = glassType.HeatTreatment;
                                tag.Lite1LamiProdType = glassType.LamiProductType;
                                tag.Lite1FirstSrfCoating = glassType.Surface1Coating;
                                tag.Lite1SecondSrfCoating = glassType.Surface2Coating;
                                tag.Lite1PatternTexture = glassType.PatternTexture;
                                tag.Lite1ProdQualityLevel = lite.QualityLevel;

                                tag.AnsiiLogoType = rackItem.GlassItem.Logo.AnsiLogo;
                                tag.ReasonCode = rack.ReasonCode;
                                tag.ProcessId = rackItem.ProcessId;
                                tag.CustomerNum = rack.Customer.CustomerNum;
                                tag.CustomerName = rack.Customer.CustomerName;
                            }
                            rackLedgerItems.Add(tag);
                        }
                    }
                }
            }
            catch (Exception err)
            {
                throw new ApplicationException(err.ToString());
            }
            return rackLedgerItems;
        }

        public SqlBulkCopyResponse PersistRackLedger(BusinessSystemSession e1Session, string plantCode, RackLedgerItems tags)
        {
            var response = new SqlBulkCopyResponse();
            if (tags.Count > 0)
            {
                var plantNum1 = tags[0].PlantNum;
                try
                {
                    var dt = GetDataTable("E1RackLedger");
                    AddRows(dt, tags);
                    //var plantNum = Utilities.GetPlantNum(plantCode);
                    var bc = new SqlBulkCopyManager();
                    var cn = ConnectionManager.GetSqlConnectionFromKey(plantNum1 + ".INTEGRATION");
                    bc.DbConnectionString = cn.ConnectionString;
                    response = bc.BulkTable("E1RackLedger", dt);
                }
                catch (Exception e)
                {
                    throw new ApplicationException(e.Message);
                }
            }
            return response;
        }

        public DataTable GetDataTable(String tableName)
        {
            var dt = new DataTable(tableName);
            //dt.Columns.Add("RackLedgerID", typeof(int));
            dt.Columns.Add("RackId", typeof(decimal));
            dt.Columns.Add("SerialNum", typeof(string));
            dt.Columns.Add("ItemNum", typeof(decimal));
            dt.Columns.Add("LotNum", typeof(string));
            dt.Columns.Add("Location", typeof(string));
            dt.Columns.Add("Status", typeof(string));
            dt.Columns.Add("QtyOnHand", typeof(decimal));
            dt.Columns.Add("ReceivedQty", typeof(decimal));
            dt.Columns.Add("RackDateTime", typeof(DateTime));
            dt.Columns.Add("ReceivedDate", typeof(DateTime));
            dt.Columns.Add("ExpirationDate", typeof(DateTime));
            dt.Columns.Add("ProductionDate", typeof(DateTime));
            dt.Columns.Add("CustomerNum", typeof(decimal));
            dt.Columns.Add("CustomerName", typeof(string));
            dt.Columns.Add("Lite1_Thickness", typeof(decimal));
            dt.Columns.Add("Lite1_Color", typeof(string));
            dt.Columns.Add("Lite1_HeatTreatment", typeof(string));
            dt.Columns.Add("Lite1_1stSrfCoating", typeof(string));
            dt.Columns.Add("Lite1_2ndSrfCoating", typeof(string));
            dt.Columns.Add("Lite1_PatternTexture", typeof(string));
            dt.Columns.Add("Lite1_LamiProdType", typeof(string));
            dt.Columns.Add("Lite1_ProdQualityLevel", typeof(string));
            dt.Columns.Add("AnsiiLogoType", typeof(string));
            dt.Columns.Add("ShapeNum", typeof(decimal));
            dt.Columns.Add("BaseL", typeof(decimal));
            dt.Columns.Add("BaseN", typeof(decimal));
            dt.Columns.Add("BaseD", typeof(decimal));
            dt.Columns.Add("LeftL", typeof(decimal));
            dt.Columns.Add("LeftN", typeof(decimal));
            dt.Columns.Add("LeftD", typeof(decimal));
            dt.Columns.Add("RightL", typeof(decimal));
            dt.Columns.Add("RightN", typeof(decimal));
            dt.Columns.Add("RightD", typeof(decimal));
            dt.Columns.Add("TopL", typeof(decimal));
            dt.Columns.Add("TopN", typeof(decimal));
            dt.Columns.Add("TopD", typeof(decimal));
            dt.Columns.Add("Side1L", typeof(decimal));
            dt.Columns.Add("Side1N", typeof(decimal));
            dt.Columns.Add("Side1D", typeof(decimal));
            dt.Columns.Add("Side2L", typeof(decimal));
            dt.Columns.Add("Side2N", typeof(decimal));
            dt.Columns.Add("Side2D", typeof(decimal));
            dt.Columns.Add("SqftActual", typeof(decimal));
            dt.Columns.Add("SqftBillable", typeof(decimal));
            dt.Columns.Add("RackType", typeof(string));
            dt.Columns.Add("LitesPerRack", typeof(decimal));
            dt.Columns.Add("ProcessID", typeof(string));
            dt.Columns.Add("GLCategory", typeof(string));
            dt.Columns.Add("ReasonCode", typeof(string));
            dt.Columns.Add("SequenceNum", typeof(decimal));
            return dt;
        }

        public void AddRows(DataTable dt, RackLedgerItems tags)
        {
            foreach (var tag in tags)
            {
                var dr = dt.NewRow();
                if (tag.HasGlassAttributes)
                {
                    dr["RackId"] = tag.RackId;
                    dr["SerialNum"] = tag.SerialNum;
                    dr["ItemNum"] = tag.ItemNum;
                    dr["LotNum"] = tag.LotNum;
                    dr["Location"] = tag.Location;
                    dr["Status"] = tag.Status;
                    dr["QtyOnHand"] = tag.QtyOnHand;
                    dr["ReceivedQty"] = tag.ReceivedQty;
                    dr["RackDateTime"] = tag.UpdateDateTime;
                    if (tag.ReceivedDate == null)
                    {
                        dr["ReceivedDate"] = DBNull.Value;
                    }
                    else
                    {
                        dr["ReceivedDate"] = tag.ReceivedDate;
                    }
                    //dr["ExpirationDate"] = tag.ExpirationDate;
                    dr["ProductionDate"] = tag.ManufacturedDate;
                    dr["CustomerNum"] = tag.CustomerNum;
                    dr["CustomerName"] = tag.CustomerName;
                    dr["Lite1_Thickness"] = tag.Lite1Thickness;
                    dr["Lite1_Color"] = tag.Lite1Color;
                    dr["Lite1_HeatTreatment"] = tag.Lite1HeatTreatment;
                    dr["Lite1_1stSrfCoating"] = tag.Lite1FirstSrfCoating;
                    dr["Lite1_2ndSrfCoating"] = tag.Lite1SecondSrfCoating;
                    dr["Lite1_PatternTexture"] = tag.Lite1PatternTexture;
                    dr["Lite1_LamiProdType"] = tag.Lite1LamiProdType;
                    dr["Lite1_ProdQualityLevel"] = tag.Lite1ProdQualityLevel;
                    dr["AnsiiLogoType"] = tag.AnsiiLogoType;
                    dr["ShapeNum"] = tag.ShapeNum;
                    dr["BaseL"] = tag.BaseLeg;
                    dr["BaseN"] = tag.BaseNum;
                    dr["BaseD"] = tag.BaseDen;
                    dr["LeftL"] = tag.LeftLeg;
                    dr["LeftN"] = tag.LeftNum;
                    dr["LeftD"] = tag.LeftDen;
                    dr["RightL"] = tag.RightLeg;
                    dr["RightN"] = tag.RightNum;
                    dr["RightD"] = tag.RightDen;
                    dr["TopL"] = tag.TopLeg;
                    dr["TopN"] = tag.TopNum;
                    dr["TopD"] = tag.TopDen;
                    dr["Side1L"] = tag.Side1Leg;
                    dr["Side1N"] = tag.Side1Num;
                    dr["Side1D"] = tag.Side1Den;
                    dr["Side2L"] = tag.Side2Leg;
                    dr["Side2N"] = tag.Side2Num;
                    dr["Side2D"] = tag.Side2Den;
                    dr["SqftActual"] = tag.SqftActual;
                    dr["SqftBillable"] = tag.SqftBillable;
                    dr["RackType"] = tag.RackType;
                    dr["LitesPerRack"] = tag.LitesPerRack;
                    dr["ProcessID"] = tag.ProcessId;
                    dr["GLCategory"] = tag.GlCategory;
                    dr["ReasonCode"] = tag.ReasonCode;
                    dr["SequenceNum"] = tag.SequenceNum;
                }
                else
                {
                    dr["RackId"] = tag.RackId;
                    dr["RackType"] = tag.RackType;
                    dr["SerialNum"] = tag.SerialNum;
                    dr["ItemNum"] = tag.ItemNum;
                    dr["Location"] = tag.Location;
                    dr["Status"] = tag.Status;
                    dr["QtyOnHand"] = tag.QtyOnHand;
                    dr["ReceivedQty"] = tag.ReceivedQty;
                    dr["RackDateTime"] = tag.UpdateDateTime;
                    if (tag.ReceivedDate == DateTime.MinValue)
                    {
                        dr["ReceivedDate"] = DBNull.Value;
                    }
                    else
                    {
                        dr["ReceivedDate"] = tag.ReceivedDate;
                    }
                    dr["ProductionDate"] = tag.ManufacturedDate;
                    dr["CustomerNum"] = tag.CustomerNum;
                    dr["CustomerName"] = tag.CustomerName;
                }
                dt.Rows.Add(dr);
            }
        }
    }
}
