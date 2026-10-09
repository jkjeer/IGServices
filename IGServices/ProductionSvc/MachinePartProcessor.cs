using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IGServices.Common.Managers.E1;
using IGServices.Common.Managers.Enterprise;
using IGServices.Common.Managers.Process;
using IGServices.Common.Types.E1;
using IGServices.Common.Types.Enterprise;
using IGServices.Common.Types.Response;
using IGServices.Common.Utilities;

namespace IGServices.ProductionSvc
{
    public class MachinePartProcessor
    {

        //public ProcessResponse ConsumptionUpload(string plantCode, string status)
        //{
        //    var session = new BusinessSystemSession();
        //    var cn = new SqlConnection();
        //    var cnInt = new SqlConnection();
        //    try
        //    {
        //        // resolve Plant
        //        var plant = PlantManager.GetPlant(plantCode);

        //        // Biz Session
        //        session = BusinessSystemSessionManager.GetBusinessSession();

        //        // Shop Floor Connection
        //        cn = ConnectionManager.GetSqlConnectionFromKey(plant.PlantCode + ".INTEGRATION");
        //        cn.Open();

        //        // Integration Connection
        //        cnInt = ConnectionManager.GetSqlConnectionFromKey("CORP.INTEGRATION");
        //        cnInt.Open();

        //        // Process
        //        var mgr = new MachinePartUploadManager(session);
        //        return mgr.Process(session, cn, cnInt, plant, status);
        //    }
        //    catch (Exception err)
        //    {
        //        throw new ApplicationException(err.ToString());
        //    }
        //    finally
        //    {
        //        session.Close();
        //        cn.Close();
        //        cnInt.Close();
        //    }
        //}

        public static ProcessResponse UploadMachinePartConsumption(Plant plant, BusinessSystemSession session, Dictionary<string, SqlConnection> sqlCns, Dictionary<string, object> parms)
        {
            var response = new ProcessResponse($"{plant.PlantCode} Machine Part Upload Manager");
            try
            {
                var timeSpan = Data.UnboxParm.To(parms, "TimeSpan", new TimeSpan());
                var useHighWaterMark = Data.UnboxParm.To(parms, "HighWaterMark", false);
                var statusCode = Data.UnboxParm.To(parms, "StatusCode", string.Empty);
                response.ProcessResponses.Add(MachinePartUploadManager.Process(session, sqlCns, plant, statusCode));
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
            }
            response.Done();
            return response;
        }
    }
}
