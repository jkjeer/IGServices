using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.ServiceModel;
using IGServices.Common.Managers.E1;
using IGServices.Common.Managers.Enterprise;
using IGServices.Common.Managers.Process;
using IGServices.Common.Types.E1;
using IGServices.Common.Types.Enterprise;
using IGServices.Common.Types.Response;
using IGServices.Common.Utilities;
using NLog;
namespace IGServices.ItemSvc
{
    public class ItemProcessor
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static ProcessResponse DownloadItems(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var response = new ProcessResponse($"{plant.PlantCode} Item Download Manager");
            try
            {
                var timeSpan = Data.UnboxParm.To(parms, "TimeSpan", new TimeSpan());
                var useHighWaterMark = Data.UnboxParm.To(parms, "HighWaterMark", false);
                var itemNum = Data.UnboxParm.To<decimal>(parms, "ItemNum", 0);
                var stockTypes = Data.UnboxParm.To(parms, "StockTypes", new List<string>());
                var mgr = new ItemDownloadManager(session);
                response = mgr.Process(session, sqlCns, plant, timeSpan, useHighWaterMark, itemNum, stockTypes);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
            }
            response.Done();
            return response;
        }

        public static ProcessResponse DownloadGlassItems(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var response = new ProcessResponse($"{plant.PlantCode} Glass Item Download Manager");
            try
            {
                var timeSpan = Data.UnboxParm.To(parms, "TimeSpan", new TimeSpan());
                var useHighWaterMark = Data.UnboxParm.To(parms, "HighWaterMark", false);
                var itemNum = Data.UnboxParm.To<decimal>(parms, "ItemNum", 0);
                var stockTypes = Data.UnboxParm.To(parms, "StockTypes", new List<string>());
                var mgr = new GlassItemDownloadManager(session);
                response = mgr.Process(session, sqlCns, plant, timeSpan, useHighWaterMark, itemNum, stockTypes);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
            }
            response.Done();
            return response;
        }

        public static ProcessResponse DownloadSpacerItems(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var response = new ProcessResponse($"{plant.PlantCode} Glass Item Download Manager");
            try
            {
                var timeSpan = Data.UnboxParm.To(parms, "TimeSpan", new TimeSpan());
                var useHighWaterMark = Data.UnboxParm.To(parms, "HighWaterMark", false);
                var itemNum = Data.UnboxParm.To<decimal>(parms, "ItemNum", 0);
                var mgr = new SpacerItemDownloadManager(session);
                response = mgr.Process(session, sqlCns, plant, timeSpan, useHighWaterMark, itemNum);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
            }
            response.Done();
            return response;
        }

        public static ProcessResponse DownloadMuntinBarItems(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var response = new ProcessResponse($"{plant.PlantCode} Glass Item Download Manager");
            try
            {
                var timeSpan = Data.UnboxParm.To(parms, "TimeSpan", new TimeSpan());
                var useHighWaterMark = Data.UnboxParm.To(parms, "HighWaterMark", false);
                var itemNum = Data.UnboxParm.To<decimal>(parms, "ItemNum", 0);
                var mgr = new MuntinDownloadManager(session);
                response = mgr.Process(session, sqlCns, plant, timeSpan, useHighWaterMark, itemNum);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
            }
            response.Done();
            return response;
        }

        public static ProcessResponse DownloadMuntinCodes(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var response = new ProcessResponse($"{plant.PlantCode} Glass Item Download Manager");
            try
            {
                var timeSpan = Data.UnboxParm.To(parms, "TimeSpan", new TimeSpan());
                var useHighWaterMark = Data.UnboxParm.To(parms, "HighWaterMark", false);
                var mgr = new MuntinCodeDownloadManager(session);
                response = mgr.Process(session, sqlCns, plant, timeSpan, useHighWaterMark);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
            }
            response.Done();
            return response;
        }

        public static ProcessResponse DownloadItemXref(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var response = new ProcessResponse($"{plant.PlantCode} Item Download Manager");
            try
            {
                var timeSpan = Data.UnboxParm.To(parms, "TimeSpan", new TimeSpan());
                var useHighWaterMark = Data.UnboxParm.To(parms, "HighWaterMark", false);
                var xrefTypes = Data.UnboxParm.To(parms, "XrefTypes", new List<string>());
                var mgr = new ItemXrefDownloadManager(session);
                response = mgr.Process(session, sqlCns, plant, timeSpan, useHighWaterMark, xrefTypes);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
            }
            response.Done();
            return response;
        }

        public static ProcessResponse DownloadMtoSpecUnits(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var response = new ProcessResponse($"{plant.PlantCode} MTO Spec Unit Download Manager");
            try
            {
                var timeSpan = Data.UnboxParm.To(parms, "TimeSpan", new TimeSpan());
                var useHighWaterMark = Data.UnboxParm.To(parms, "HighWaterMark", false);
                var documentTypes = Data.UnboxParm.To(parms, "DocumentTypes", new List<string>());
                var documentNum = Data.UnboxParm.To<decimal>(parms, "DocumentNum", 0);
                var mgr = new SpecUnitMtoDownloadManager(session);
                response = mgr.Process(session, sqlCns, plant, timeSpan, useHighWaterMark, documentTypes, documentNum);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
            }
            response.Done();
            return response;
        }

        public static ProcessResponse DownloadMtsSpecUnits(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var response = new ProcessResponse($"{plant.PlantCode} MTO Spec Unit Download Manager");
            try
            {
                var timeSpan = Data.UnboxParm.To(parms, "TimeSpan", new TimeSpan());
                var useHighWaterMark = Data.UnboxParm.To(parms, "HighWaterMark", false);
                var itemNum = Data.UnboxParm.To<decimal>(parms, "ItemNum", 0);
                var mgr = new SpecUnitMtsDownloadManager(session);
                response = mgr.Process(session, sqlCns, plant, timeSpan, useHighWaterMark, itemNum);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
            }
            response.Done();
            return response;
        }

        public ItemAttributes GetGlassItemAttributes(decimal itemNum = 0, decimal documentNum = 0, string documentType = null, decimal documentLineNum = 0, string companyCode = null)
        {
            var session = BusinessSystemSessionManager.GetBusinessSession();
            try
            {
                var manager = new Common.Managers.E1.ItemAttributeManager(session);
                return manager.GetItemAttributes(session, itemNum, documentNum, documentType, documentLineNum, companyCode);
            }
            catch (Exception e)
            {
                throw new FaultException(e.Message);
            }
            finally
            {
                session.Close();
            }
        }

        public GlassItem GetGlassItem(decimal itemNum, string plantCode, decimal documentNum = 0, string documentType = null, decimal documentLineNum = 0, string companyCode = null)
        {
            var session = BusinessSystemSessionManager.GetBusinessSession();
            try
            {
                var manager = new GlassItemManager(session);
                return manager.GetGlassItem(session, itemNum, plantCode, documentNum, documentType, documentLineNum, companyCode);
            }
            catch (Exception e)
            {
                throw new FaultException(e.Message);
            }
            finally
            {
                session.Close();
            }
        }

        public GlassItems GetDiscreteGlassItems(string plantCode, decimal itemNum = 0, TimeSpan? timeSpan = null)
        {
            var session = BusinessSystemSessionManager.GetBusinessSession();
            try
            {
                var manager = new GlassItemManager(session);
                return manager.GetDiscreteItems(session, plantCode, itemNum, timeSpan);
            }
            catch (Exception e)
            {
                throw new FaultException(e.Message);
            }
            finally
            {
                session.Close();
            }
        }

        public GlassItems GetDiscreteGlassItems(string plantCode, decimal itemNum = 0, List<string> stockingTypes = null, List<string> categoryCodes = null, TimeSpan? timeSpan = null)
        {
            var session = BusinessSystemSessionManager.GetBusinessSession();
            try
            {
                Plant plant = PlantManager.GetPlant(plantCode);
                var manager = new GlassItemManager(session);
                return manager.GetDiscreteItems(session, plant.PlantNum, itemNum, timeSpan);
            }
            catch (Exception e)
            {
                throw new FaultException(e.Message);
            }
            finally
            {
                session.Close();
            }
        }

        public SpacerItems GetSpacerItems(string plantCode, TimeSpan span, bool useHighWaterMark, decimal itemNum)
        {
            var items = new SpacerItems();
            var session = BusinessSystemSessionManager.GetBusinessSession();
            var cn = new SqlConnection();
            try
            {
                // Plant Assignment
                var plant = PlantManager.GetPlant(plantCode);

                // Shop Floor Connection
                cn = ConnectionManager.GetSqlConnectionFromKey(plant.PlantCode + ".INTEGRATION");
                cn.Open();

                // Process Run Span
                var mark = (useHighWaterMark) ? HighWaterMarkManager.GetHighWaterMark(cn, plant, "E1_SPACER_ITEM") : null;
                var processRunSpan = ProcessRunSpanManager.GetRunSpan(mark, span, E1TimeManager.GetE1TimeStamp(session));

                // Manager
                var manager = new SpacerItemManager(session);
                items = manager.GetSpacerItems(session, plant, processRunSpan, itemNum);
            }
            catch (Exception e)
            {
                throw new FaultException(e.Message);
            }
            finally
            {
                session.Close();
            }
            return items;
        }

        public MuntinBarItems GetMuntinBarItems(string plantCode, TimeSpan span, bool useHighWaterMark, decimal itemNum)
        {
            var items = new MuntinBarItems();
            var session = BusinessSystemSessionManager.GetBusinessSession();
            var cn = new SqlConnection();
            try
            {
                // Plant Assignment
                var plant = PlantManager.GetPlant(plantCode);

                // Shop Floor Connection
                cn = ConnectionManager.GetSqlConnectionFromKey(plant.PlantCode + ".INTEGRATION");
                cn.Open();

                // Process Run Span
                var mark = (useHighWaterMark) ? HighWaterMarkManager.GetHighWaterMark(cn, plant, "E1_MUNTIN_ITEM") : null;
                var processRunSpan = ProcessRunSpanManager.GetRunSpan(mark, span, E1TimeManager.GetE1TimeStamp(session));

                // Manager
                var manager = new MuntinBarItemManager(session);
                items = manager.GetBarItems(session, plant, processRunSpan, itemNum);
            }
            catch (Exception e)
            {
                throw new FaultException(e.Message);
            }
            finally
            {
                session.Close();
            }
            return items;
        }




    }
}
