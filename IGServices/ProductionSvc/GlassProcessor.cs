using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using IGServices.Common.Managers.E1;
using IGServices.Common.Managers.Response;
using IGServices.Common.Managers.ShopFloor;
using IGServices.Common.Types.E1;
using IGServices.Common.Types.Response;
using NLog;

namespace IGServices.ProductionSvc
{
    public class GlassProcessor
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public GlassConsumptionResponse GetUnprocessedGlassConsumption(string plantNum, int verbosityMode, string queryString)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info("BEGIN: " + DateTime.Now.ToLocalTime());
            var response = new GlassConsumptionResponse("GetGlassConsumption", queryString);
            var responseMgr = new GlassConsumptionResponseManager();
            var cnSF = new SqlConnection();
            try
            {
                cnSF = GetSFSqlConnection(plantNum, response.ConnectionStrings);
                responseMgr.Init(response, plantNum, verbosityMode, false);
                var glassConsumptionMgr = new GlassConsumptionManager();
                response.GlassConsumptions = glassConsumptionMgr.GetUnprocessedGlassConsumption(plantNum, cnSF);
            }
            catch (Exception e)
            {
                response.Errors = ErrorManager.AddError(response.Errors, e);
                throw new ApplicationException(cnSF.ConnectionString + "  " + e);
            }
            finally
            {
                cnSF.Close();
                responseMgr.Close(response);
            }
            Logger.Info("PLANT: " + plantNum + " END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            return response;
        }

        public GlassConsumptionResponse GetLockedGlassConsumption(string plantNum, int verbosityMode, string queryString)
        {
            var response = new GlassConsumptionResponse("GetGlassConsumption", queryString);
            var responseMgr = new GlassConsumptionResponseManager();
            var cnSFInt = new SqlConnection();
            var cnSF = new SqlConnection();
            var responseTimesMgr = new ResponseTimesManager();
            try
            {
                cnSFInt = GetSFIntSqlConnection(plantNum, response.ConnectionStrings);
                cnSF = GetSFSqlConnection(plantNum, response.ConnectionStrings);
                responseMgr.Init(response, plantNum, verbosityMode, false);
                var glassConsumptionMgr = new GlassConsumptionManager();

                responseTimesMgr.OpenResponseTime(response.ResponseTimes, "GetLockedGlassConsumption", "SF", cnSF.ConnectionString);
                glassConsumptionMgr.Lock(cnSFInt);
                response.GlassConsumptions = glassConsumptionMgr.GetLockedGlassConsumption(plantNum, cnSF);
                responseTimesMgr.CloseResponseTime(response.ResponseTimes, "GetLockedGlassConsumption", "SF", cnSF.ConnectionString);
            }
            catch (Exception e)
            {
                response.Errors = ErrorManager.AddError(response.Errors, e);
            }
            finally
            {
                cnSFInt.Close();
                cnSF.Close();
                responseMgr.Close(response);
            }
            return response;
        }

        public GlassConsumptionResponse UploadGlassConsumption(string plantNum, int verbosityMode, bool write, string queryString)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info("BEGIN: " + DateTime.Now.ToLocalTime());
            var response = new GlassConsumptionResponse("UploadGlassConsumption", queryString);
            var responseMgr = new GlassConsumptionResponseManager();
            var e1Session = new BusinessSystemSession();
            var cnSFInt = new SqlConnection();
            var cnSF = new SqlConnection();
            var cnEDBT = new SqlConnection();
            var cnSFUKID = new SqlConnection();
            var cnQAUKID = new SqlConnection();
            var responseTimesMgr = new ResponseTimesManager();
            try
            {
                e1Session = GetE1Session(response.ConnectionStrings);
                cnSFInt = GetSFIntSqlConnection(plantNum, response.ConnectionStrings);
                cnSF = GetSFSqlConnection(plantNum, response.ConnectionStrings);
                cnEDBT = GetEDBTSqlConnection(response.ConnectionStrings);
                cnSFUKID = GetSFUKIDSqlConnection(response.ConnectionStrings);
                cnQAUKID = GetQAUKIDSqlConnection(response.ConnectionStrings);
                responseTimesMgr.OpenResponseTime(response.ResponseTimes, "GetGlassConsumption", "SF", cnSF.ConnectionString);
                responseMgr.Init(response, plantNum, verbosityMode, write);
                var glassConsumptionMgr = new GlassConsumptionManager();
                glassConsumptionMgr.Lock(cnSFInt);
                response.GlassConsumptions = glassConsumptionMgr.GetLockedGlassConsumption(plantNum, cnSF);
                response.IsValid = glassConsumptionMgr.Validate(e1Session, cnSFInt, response.GlassConsumptions);
                responseTimesMgr.CloseResponseTime(response.ResponseTimes, "GetGlassConsumption", "SF", cnSF.ConnectionString);
                responseTimesMgr.OpenResponseTime(response.ResponseTimes, "InsertGlassConsumption", "E1", e1Session.DbConnection.ConnectionString);
                if (response.GlassConsumptions.Any(glassConsumption => glassConsumption.IsValid))
                {
                    if (response.Write)
                    {
                        response.TransferBatchNum = glassConsumptionMgr.GetNextSFEDBT(cnEDBT);
                        foreach (var glassConsumption in response.GlassConsumptions.Where(glassConsumption => glassConsumption.IsValid))
                        {
                            glassConsumption.TransferBatchNum = response.TransferBatchNum;
                        }
                        glassConsumptionMgr.GetSFUKIDs(response.GlassConsumptions, cnSFUKID);
                        var glassConsumptionInteropMgr = new GlassConsumptionInteropManager(e1Session);
                        response.GlassConsumptionInterops = glassConsumptionInteropMgr.BuildGlassConsumptionInterops(response.GlassConsumptions);
                        if (response.GlassConsumptionInterops.Any())
                        {
                            response.GlassConsumptionControlTrigger = glassConsumptionInteropMgr.BuildGlassConsumptionTrigger(response.PlantNum, response.TransferBatchNum, cnQAUKID);
                            var e1Trans = e1Session.DbConnection.BeginTransaction(IsolationLevel.RepeatableRead);
                            try
                            {
                                glassConsumptionInteropMgr.InsertE1GlassConsumptionInterops(e1Session, e1Trans, response.GlassConsumptionInterops);
                                if (response.Errors == null)
                                {
                                    glassConsumptionInteropMgr.InsertGlassConsumptionTrigger(e1Session, e1Trans, response.GlassConsumptionControlTrigger);
                                    if (response.Errors == null)
                                    {
                                        e1Trans.Commit();
                                        glassConsumptionInteropMgr.SuccessE1(response);
                                    }
                                    else
                                    {
                                        e1Trans.Rollback();
                                        response.Errors = ErrorManager.AddError(response.Errors, "Rolling back InsertGlassConsumptionTrigger");
                                        glassConsumptionMgr.Unlock(cnSFInt);
                                        glassConsumptionInteropMgr.FailE1(response);
                                    }
                                }
                                else
                                {
                                    e1Trans.Rollback();
                                    response.Errors = ErrorManager.AddError(response.Errors, "Rolling back InsertGlassConsumptionInterop");
                                    glassConsumptionInteropMgr.FailE1(response);
                                }
                            }
                            catch (Exception e)
                            {
                                e1Trans.Rollback();
                                response.Errors = ErrorManager.AddError(response.Errors, e);
                                glassConsumptionInteropMgr.FailE1(response);
                            }
                        }
                        try
                        {
                            glassConsumptionMgr.UpdateGlassConsumption(response.GlassConsumptions, response.SFUpdateBatchSize, cnSFInt);
                        }
                        catch (Exception e)
                        {
                            response.Errors = ErrorManager.AddError(response.Errors, e);
                        }
                    }
                    else
                    {
                        response.Message = "No valid Glass Consumptions available";
                    }
                }
            }
            catch (Exception e)
            {
                response.Errors = ErrorManager.AddError(response.Errors, e);
            }
            finally
            {
                cnSFInt.Close();
                cnSF.Close();
                cnEDBT.Close();
                cnSFUKID.Close();
                cnQAUKID.Close();
                CloseE1Session(e1Session);
                responseTimesMgr.CloseResponseTime(response.ResponseTimes, "InsertGlassConsumption", "E1", e1Session.DbConnection.ConnectionString);
                responseMgr.Close(response);
            }
            Logger.Info("PLANT: " + plantNum + " END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            return response;
        }

        private static BusinessSystemSession GetE1Session(ConnectionStrings connectionStrings = null)
        {
            var connectionString = new ConnectionString();
            var e1Session = BusinessSystemSessionManager.GetBusinessSession(connectionString);
            ConnectionStringManager.AddConnectionString(connectionStrings, "cnE1", e1Session.DbConnection.ConnectionString);
            return e1Session;
        }

        private static void CloseE1Session(BusinessSystemSession e1Session)
        {
            var e1SessionMgr = new BusinessSystemSessionManager();
            e1SessionMgr.CloseBusinessSession(e1Session);
        }

        private static SqlConnection GetQAUKIDSqlConnection(ConnectionStrings connectionStrings = null)
        {
            var cnQAUKID = new SqlConnection();
            try
            {
                cnQAUKID = ConnectionManager.GetSqlConnectionFromKey("INTEGRATION");
                cnQAUKID.Open();
            }
            catch (Exception e)
            {
                throw new ApplicationException(cnQAUKID.ConnectionString + "  " + e);
            }
            ConnectionStringManager.AddConnectionString(connectionStrings, "cnQAUKID", cnQAUKID.ConnectionString);
            return cnQAUKID;
        }

        private static SqlConnection GetSFUKIDSqlConnection(ConnectionStrings connectionStrings = null)
        {
            var cnSFUKID = new SqlConnection();
            try
            {
                cnSFUKID = ConnectionManager.GetSqlConnectionFromKey("INTEGRATION");
                cnSFUKID.Open();
            }
            catch (Exception e)
            {
                throw new ApplicationException(cnSFUKID.ConnectionString + "  " + e);
            }
            ConnectionStringManager.AddConnectionString(connectionStrings, "cnSFUKID", cnSFUKID.ConnectionString);
            return cnSFUKID;
        }

        private static SqlConnection GetEDBTSqlConnection(ConnectionStrings connectionStrings = null)
        {
            var cnEDBT = new SqlConnection();
            try
            {
                cnEDBT = ConnectionManager.GetSqlConnectionFromKey("INTEGRATION");
                cnEDBT.Open();
            }
            catch (Exception e)
            {
                throw new ApplicationException(cnEDBT.ConnectionString + "  " + e);
            }
            ConnectionStringManager.AddConnectionString(connectionStrings, "cnEDBT", cnEDBT.ConnectionString);
            return cnEDBT;
        }

        private static SqlConnection GetSFSqlConnection(string plantNum, ConnectionStrings connectionStrings = null)
        {
            var cnSF = new SqlConnection();
            if (plantNum != "0")
            {
                try
                {
                    cnSF = ConnectionManager.GetSqlConnectionFromKey(plantNum + ".SHOPFLOOR");
                    cnSF.Open();
                }
                catch (Exception e)
                {
                    throw new ApplicationException(cnSF.ConnectionString + "  " + e);
                }
            }
            ConnectionStringManager.AddConnectionString(connectionStrings, "cnSF", cnSF.ConnectionString);
            return cnSF;
        }

        private static SqlConnection GetSFIntSqlConnection(string plantNum, ConnectionStrings connectionStrings = null)
        {
            var cnSFInt = new SqlConnection();
            if (plantNum != "0")
            {
                try
                {
                    cnSFInt = ConnectionManager.GetSqlConnectionFromKey(plantNum + ".INTEGRATION");
                    cnSFInt.Open();
                }
                catch (Exception e)
                {
                    throw new ApplicationException(cnSFInt.ConnectionString + "  " + e);
                }
            }
            ConnectionStringManager.AddConnectionString(connectionStrings, "cnSFInt", cnSFInt.ConnectionString);
            return cnSFInt;
        }
    }
}
