using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
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
    public class SpecUnitDownloadManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /******************************************************************
        * Build Shop Floor objects
        ******************************************************************/
        public ProcessResponse ProcessMto(BusinessSystemSession session, SqlConnection cn, Plant plant, ProcessRunSpan span, decimal orderNum, string orderType, decimal orderLineNum)
        {
            var response = new ProcessResponse("MTO Unit Download");
            response.Messages.Add(new Message("RunSpan", span.Serialize()));
            response.Plant = plant;
            response.ConnectionStrings.Add(new ConnectionString("E1", session.DbConnection.ConnectionString));
            response.ConnectionStrings.Add(new ConnectionString("SF", cn.ConnectionString));

            // Get Enterprise Objects
            var ids = new SpecUnits();
            var step1 = new ProcessStep("Get MTO Units");
            try
            {
                var specUnitMgr = new SpecUnitManager(session);
                ids = specUnitMgr.GetMtoUnits(session, plant, span, orderNum, orderType, orderLineNum);
                step1.RowsAffected = ids.Count;
            }
            catch (Exception e)
            {
                step1.Errors.Add(new Error(e));
            }
            step1.Done();
            response.AddStep(step1);


            if (ids.Count > 0)
            {
                var dts = new List<DataTable>();
                var glassTypeTable = new SpecGlassTypeTable();
                var glassProcessingTable = new SpecGlassProcessingTable();
                var unitTable = new SpecUnitTable();
                var unitXrefTable = new SpecUnitXrefTable();
                var unitShapeTable = new SpecUnitShapeTable();
                var unitAirspaceTable = new SpecUnitAirspaceTable();
                var unitGridTable = new SpecUnitGridTable();
                var unitSurfaceTable = new SpecUnitSurfaceTable();
                var unitFlexTable = new SpecUnitFlexTable();

                var step2 = new ProcessStep("Load Tables");
                try
                {
                    foreach (SpecUnit item in ids)
                    {
                        glassTypeTable.Add(item);
                        glassProcessingTable.Add(item);
                        unitTable.Add(item);
                        unitXrefTable.Add(item);
                        unitShapeTable.Add(item);
                        unitAirspaceTable.Add(item);
                        unitGridTable.Add(item);
                        unitSurfaceTable.Add(item);
                        unitFlexTable.Add(item);
                    }

                    dts.Add(glassTypeTable);
                    dts.Add(glassProcessingTable);
                    dts.Add(unitTable);
                    dts.Add(unitXrefTable);
                    dts.Add(unitShapeTable);
                    dts.Add(unitAirspaceTable);
                    dts.Add(unitGridTable);
                    dts.Add(unitSurfaceTable);
                    dts.Add(unitFlexTable);
                }
                catch (Exception e)
                {
                    step2.Errors.Add(new Error(e));
                }
                step2.Done();
                response.AddStep(step2);

                var step3 = new ProcessStep("Bulk Copy");
                try
                {
                    foreach (DataTable dt in dts)
                    {
                        step3.SqlBulkCopyResponses.Add(
                            BulkCopyManager.BulkTable(dt.ExtendedProperties["Target"].ToString(), dt, cn));
                    }
                }
                catch (Exception e)
                {
                    step3.Errors.Add(new Error(e));
                }
                step3.Done();
                response.AddStep(step3);

                var step4 = new ProcessStep("Set High Water Mark");
                try
                {
                    if (span.HighWaterMark.Exists)
                    {
                        span.HighWaterMark.EnterpriseDateTime = unitTable.MaxDate;
                        response.SqlDmlResponses.Add(HighWaterMarkManager.SaveHighWaterMark(cn, span.HighWaterMark));
                    }
                }
                catch (Exception e)
                {
                    step4.Errors.Add(new Error(e));
                }
                step4.Done();
                response.AddStep(step4);
            }

            return response;
        }

        public ProcessResponse ProcessPurchasedMto(BusinessSystemSession session, SqlConnection cn, Plant plant, ProcessRunSpan span)
        {
            var response = new ProcessResponse("MTO Purchased Unit Download");
            response.Messages.Add(new Message("RunSpan", span.Serialize()));
            response.Plant = plant;
            response.ConnectionStrings.Add(new ConnectionString("E1", session.DbConnection.ConnectionString));
            response.ConnectionStrings.Add(new ConnectionString("SF", cn.ConnectionString));

            // Get Enterprise Objects
            var ids = new SpecUnits();
            var step1 = new ProcessStep("Get MTO Units");
            try
            {
                var specUnitMgr = new SpecUnitManager(session);
                ids = specUnitMgr.GetPurchasedMtoUnits(session, plant, span);
                step1.RowsAffected = ids.Count;
            }
            catch (Exception e)
            {
                step1.Errors.Add(new Error(e));
            }
            step1.Done();
            response.AddStep(step1);


            if (ids.Count > 0)
            {
                var dts = new List<DataTable>();
                var glassTypeTable = new SpecGlassTypeTable();
                var glassProcessingTable = new SpecGlassProcessingTable();
                var unitTable = new SpecUnitTable();
                var unitXrefTable = new SpecUnitXrefTable();
                var unitShapeTable = new SpecUnitShapeTable();
                var unitAirspaceTable = new SpecUnitAirspaceTable();
                var unitGridTable = new SpecUnitGridTable();
                var unitSurfaceTable = new SpecUnitSurfaceTable();
                var unitFlexTable = new SpecUnitFlexTable();

                var step2 = new ProcessStep("Load Tables");
                try
                {
                    foreach (SpecUnit item in ids)
                    {
                        glassTypeTable.Add(item);
                        glassProcessingTable.Add(item);
                        unitTable.Add(item);
                        unitXrefTable.Add(item);
                        unitShapeTable.Add(item);
                        unitAirspaceTable.Add(item);
                        unitGridTable.Add(item);
                        unitSurfaceTable.Add(item);
                        unitFlexTable.Add(item);
                    }

                    dts.Add(glassTypeTable);
                    dts.Add(glassProcessingTable);
                    dts.Add(unitTable);
                    dts.Add(unitXrefTable);
                    dts.Add(unitShapeTable);
                    dts.Add(unitAirspaceTable);
                    dts.Add(unitGridTable);
                    dts.Add(unitSurfaceTable);
                    dts.Add(unitFlexTable);
                }
                catch (Exception e)
                {
                    step2.Errors.Add(new Error(e));
                }
                step2.Done();
                response.AddStep(step2);

                var step3 = new ProcessStep("Bulk Copy");
                try
                {
                    foreach (DataTable dt in dts)
                    {
                        step3.SqlBulkCopyResponses.Add(
                            BulkCopyManager.BulkTable(dt.ExtendedProperties["Target"].ToString(), dt, cn));
                    }
                }
                catch (Exception e)
                {
                    step3.Errors.Add(new Error(e));
                }
                step3.Done();
                response.AddStep(step3);

                var step4 = new ProcessStep("Set High Water Mark");
                try
                {
                    if (span.HighWaterMark.Exists)
                    {
                        span.HighWaterMark.EnterpriseDateTime = unitTable.MaxDate;
                        response.SqlDmlResponses.Add(HighWaterMarkManager.SaveHighWaterMark(cn, span.HighWaterMark));
                    }
                }
                catch (Exception e)
                {
                    step4.Errors.Add(new Error(e));
                }
                step4.Done();
                response.AddStep(step4);
            }

            return response;
        }

        public ProcessResponse ProcessMts(BusinessSystemSession session, SqlConnection cn, Plant plant, ProcessRunSpan span, decimal itemNum)
        {
            var response = new ProcessResponse("MTS Unit Download");
            response.Messages.Add(new Message("RunSpan", span.Serialize()));
            response.Plant = plant;
            response.ConnectionStrings.Add(new ConnectionString("E1", session.DbConnection.ConnectionString));
            response.ConnectionStrings.Add(new ConnectionString("SF", cn.ConnectionString));

            // Get Enterprise Objects
            var ids = new SpecUnits();
            var step1 = new ProcessStep("Get MTS Units");
            try
            {
                var specUnitMgr = new SpecUnitManager(session);
                ids = specUnitMgr.GetMtsUnits(session, plant, span, itemNum);
                step1.RowsAffected = ids.Count;
            }
            catch (Exception e)
            {
                step1.Errors.Add(new Error(e));
            }
            step1.Done();
            response.AddStep(step1);

            if (ids.Count > 0)
            {
                var dts = new List<DataTable>();
                var glassTypeTable = new SpecGlassTypeTable();
                var glassProcessingTable = new SpecGlassProcessingTable();
                var unitTable = new SpecUnitTable();
                var unitXrefTable = new SpecUnitXrefTable();
                var unitShapeTable = new SpecUnitShapeTable();
                var unitAirspaceTable = new SpecUnitAirspaceTable();
                var unitGridTable = new SpecUnitGridTable();
                var unitSurfaceTable = new SpecUnitSurfaceTable();
                var unitFlexTable = new SpecUnitFlexTable();

                var step2 = new ProcessStep("Load Tables");
                try
                {
                    foreach (SpecUnit item in ids)
                    {
                        glassTypeTable.Add(item);
                        glassProcessingTable.Add(item);
                        unitTable.Add(item);
                        unitXrefTable.Add(item);
                        unitShapeTable.Add(item);
                        unitAirspaceTable.Add(item);
                        unitGridTable.Add(item);
                        unitSurfaceTable.Add(item);
                        unitFlexTable.Add(item);
                    }

                    dts.Add(glassTypeTable);
                    dts.Add(glassProcessingTable);
                    dts.Add(unitTable);
                    dts.Add(unitXrefTable);
                    dts.Add(unitShapeTable);
                    dts.Add(unitAirspaceTable);
                    dts.Add(unitGridTable);
                    dts.Add(unitSurfaceTable);
                    dts.Add(unitFlexTable);
                }
                catch (Exception e)
                {
                    step2.Errors.Add(new Error(e));
                }
                step2.Done();
                response.AddStep(step2);

                var step3 = new ProcessStep("Bulk Copy");
                try
                {
                    foreach (DataTable dt in dts)
                    {
                        step3.SqlBulkCopyResponses.Add(
                            BulkCopyManager.BulkTable(dt.ExtendedProperties["Target"].ToString(), dt, cn));
                    }
                }
                catch (Exception e)
                {
                    step3.Errors.Add(new Error(e));
                }
                step3.Done();
                response.AddStep(step3);

                var step4 = new ProcessStep("Set High Water Mark");
                try
                {
                    if (span.HighWaterMark.Exists)
                    {
                        span.HighWaterMark.EnterpriseDateTime = unitTable.MaxDate;
                        response.SqlDmlResponses.Add(HighWaterMarkManager.SaveHighWaterMark(cn, span.HighWaterMark));
                    }
                }
                catch (Exception e)
                {
                    step4.Errors.Add(new Error(e));
                }
                step4.Done();
                response.AddStep(step4);
            }

            return response;
        }
    }
}
