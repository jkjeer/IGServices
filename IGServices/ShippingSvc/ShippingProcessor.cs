using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using IGServices.Common.Managers.E1;
using IGServices.Common.Managers.Enterprise;
using IGServices.Common.Managers.Process;
using IGServices.Common.Managers.ShopFloor;
using IGServices.Common.Types.E1;
using IGServices.Common.Types.Enterprise;
using IGServices.Common.Types.Response;
using IGServices.Common.Utilities;

namespace IGServices.ShippingSvc
{
    public class ShippingProcessor
    {
        public ProcessResponse DownloadShippableRacks(string plantCode)
        {
            var response = new ProcessResponse("Shippable Racks Download");
            var session = BusinessSystemSessionManager.GetBusinessSession();
            var cn = new SqlConnection();
            try
            {
                // resolve Plant
                var plant = PlantManager.GetPlant(plantCode);

                // Shop Floor Connection
                cn = ConnectionManager.GetSqlConnectionFromKey(plant.PlantCode + ".INTEGRATION");
                cn.Open();

                // Response Properties
                response.Plant = plant;
                response.ConnectionStrings.Add(new ConnectionString("E1", session.DbConnection.ConnectionString));
                response.ConnectionStrings.Add(new ConnectionString("INT", cn.ConnectionString));

                // Download Manager
                var step1 = new ProcessStep("Shippable Rack Download Manager");
                try
                {
                    var rtMgr = new ShippableRackDownloadManager(session);
                    step1.ProcessResponses.Add(rtMgr.Process(session, cn, plant));
                    response.RowsAffected = step1.ProcessResponses[0].RowsAffected;
                }
                catch (Exception e)
                {
                    step1.Errors.Add(new Error(e));
                }
                step1.Done();
                response.AddStep(step1);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
            }
            finally
            {
                cn.Close();
                session.Close();
            }
            response.Close();
            return response;
        }



    }
}
