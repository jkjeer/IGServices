using System;
using IGServices.Common.Types.Response;

namespace IGServices.Common.Managers.Response
{
    public static class ConnectionStringManager
    {
        public static ConnectionStrings AddConnectionString(ConnectionStrings connectionStrings, string name, string value)
        {
            try
            {
                if (connectionStrings == null)
                {
                    connectionStrings = new ConnectionStrings();
                }
                var connectionString = new ConnectionString
                {
                    Name = name,
                    Value = value
                };
                connectionStrings.Add(connectionString);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.ToString());
            }
            return connectionStrings;
        }
    }
}
