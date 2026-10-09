using System;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using IGServices.Common.Managers.E1;
using IGServices.Common.Managers.Enterprise;
using IGServices.Common.Types.E1;
using IGServices.Common.Types.Response;
using IGServices.Common.Types.ShopFloor;
using IGServices.Common.Utilities;

namespace IGServices.WOStatusSvc
{
    public class WorkOrderStatusProcessor
    {
        public SqlDmlResponse SetPendingUpdates()
        {
            var e1Session = GetE1Session();
            try
            {
                var manager = new WorkOrderStatusManager(e1Session);
                return manager.SetPendingUpdates(e1Session);
            }
            catch (Exception e)
            {
                throw new FaultException(e.Message);
            }
            finally
            {
                CloseE1Session(e1Session);
            }
        }

        public PendingUpdatesResults GetPlantsWithPendingUpdates()
        {
            var e1Session = GetE1Session();
            PendingUpdatesResults results;
            try
            {
                var manager = new WorkOrderStatusManager(e1Session);
                results = manager.GetPlantsWithPendingUpdates(e1Session);
            }
            catch (Exception e)
            {
                throw new FaultException(e.Message);
            }
            finally
            {
                CloseE1Session(e1Session);
            }
            return results;
        }

        public WorkOrderStatuses GetPendingUpdatesByPlant(string plantCode)
        {
            var e1Session = GetE1Session();
            WorkOrderStatuses workOrderStatuses;
            try
            {
                var manager = new WorkOrderStatusManager(e1Session);
                workOrderStatuses = manager.GetPendingUpdatesByPlant(e1Session, plantCode);
            }
            catch (Exception e)
            {
                throw new FaultException(e.Message);
            }
            finally
            {
                CloseE1Session(e1Session);
            }
            return workOrderStatuses;
        }

        public PlantOperationResponses ProcessAll()
        {
            var responses = new PlantOperationResponses();
            var e1Session = GetE1Session();
            var activePlants = ConfigurationManager.AppSettings["ActivePlants"].Split(',');
            var suspendedPlants = ConfigurationManager.AppSettings["SuspendedPlants"].Split(',');
            try
            {
                var manager = new WorkOrderStatusManager(e1Session);
                var results = GetPlantsWithPendingUpdates();
                foreach (PendingUpdatesResult result in results)
                {
                    var bulkErrors = false;

                    var operationResponse = new PlantOperationResponse
                    {
                        StartDate = DateTime.Now,
                        PlantNum = result.PlantNum
                    };

                    var isSuspended = suspendedPlants.Contains(result.PlantNum);
                    operationResponse.IsSuspended = isSuspended;
                    var isActive = activePlants.Contains(result.PlantNum);
                    operationResponse.IsActive = isActive;
                    operationResponse.PendingUpdates = result.Updates;
 
                    if (!isSuspended)
                    {
                        operationResponse.IsSuspended = false;
                        if (isActive)
                        {
                            operationResponse.IsActive = true;
                            var updates = GetPendingUpdatesByPlant(result.PlantNum);
                            var bulkResponse = BulkToPlant(result.PlantNum, updates);
                            operationResponse.DestinationPersistence = bulkResponse;
                            bulkErrors = bulkResponse.HasErrors();
                        }
                        if (!bulkErrors)
                        {
                            var response = manager.SetCompletedUpdatesByPlant(e1Session, result.PlantNum);
                            operationResponse.SourcePersistence = response;
                        }              
                    }
                    operationResponse.EndDate = DateTime.Now;
                    responses.Add(operationResponse);
                }
            }
            catch (Exception e)
            {
                throw new FaultException(e.Message);
            }
            finally
            {
                CloseE1Session(e1Session);
            }
            return responses;
        }

        public SqlBulkCopyResponse BulkToPlant(string plantNum, WorkOrderStatuses updates)
        {
            var response = new SqlBulkCopyResponse();
            var bulkCopy = new SqlBulkCopyManager();
            try
            {
                var destCn = ConnectionManager.GetSqlConnectionFromKey(plantNum);
                ConnectionManager.IsPingable(destCn.DataSource);
                bulkCopy.DbConnectionString = destCn.ConnectionString;
                response = bulkCopy.BulkTable("E1WorkOrderStatusLedger", updates.SerializeToDataTable());  
            }
            catch (Exception e)
            {
                response.Message = e.Message;
            }
            return response;
        }

        private static BusinessSystemSession GetE1Session()
        {
            var e1Session = BusinessSystemSessionManager.GetBusinessSession();
            return e1Session;
        }

        private static void CloseE1Session(BusinessSystemSession e1Session)
        {
            var e1SessionMgr = new BusinessSystemSessionManager();
            e1SessionMgr.CloseBusinessSession(e1Session);
        }
    }
}
