using System;
using System.Collections.Generic;
using IBM.Data.DB2.iSeries;
using IGServices.Common.Types.E1;
using IGServices.Common.Utilities;
using NLog;

namespace IGServices.Common.Managers.E1
{
    public class BusinessSystemSessionManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static BusinessSystemSession GetBusinessSession(string environment = null, bool ping = false)
        {
            var e1Session = new BusinessSystemSession();
            try
            {
                var dbConnectionString = ConnectionManager.GetDb2ConnectionString(environment);
                e1Session.DbConnection = ConnectionManager.GetDb2Connection(dbConnectionString);
                e1Session.DbCommands = new Dictionary<string, iDB2Command>();
                e1Session.DbConnection.Open();
                e1Session.IsOpen = true;
                Logger.Info($@"{e1Session.DbConnection.ConnectionString} JobName: {e1Session.DbConnection.JobName}");
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                throw new Exception($"{e.Message}: {e.StackTrace}");
            }
            return e1Session;
        }

        public void CloseBusinessSession(BusinessSystemSession e1Session)
        {
            try
            {
                if (e1Session.IsOpen)
                {
                    e1Session.DbConnection.Close();
                    e1Session.IsOpen = false;
                }
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {e1Session.DbConnection.ConnectionString}  {e.Message}: {e.StackTrace}");
                throw new ApplicationException($"{e1Session.DbConnection.ConnectionString}  {e.Message}: {e.StackTrace}");
            }
        }
    }
}
