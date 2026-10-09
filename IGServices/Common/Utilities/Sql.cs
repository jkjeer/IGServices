using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Text;
using IBM.Data.DB2.iSeries;
using IGServices.Common.Types.Response;

namespace IGServices.Common.Utilities
{
    public static class Sql
    {
        public static SqlDmlResponses SqlBatchExecution(List<string> statements, short batchSize, SqlConnection cn, SqlTransaction txn = null)
        {
            var responses = new SqlDmlResponses();
            var cmd = new SqlCommand();
            cmd.Connection = cn;
            if (txn != null)
            {
                cmd.Transaction = txn;
            }

            var sb = new StringBuilder();
            int i = 0;
            try
            {
                foreach (var statement in statements)
                {
                    i++;
                    sb.Append(statement);
                    if (i % batchSize == 0 || i == statements.Count)
                    {
                        var dmlResponse = new SqlDmlResponse();
                        cmd.CommandText = sb.ToString();
                        dmlResponse.Init(cmd);
                        int rows = cmd.ExecuteNonQuery();
                        dmlResponse.RowsAffected = rows;
                        responses.Add(dmlResponse);
                        sb.Clear();
                    }
                }
                txn?.Commit();
            }
            catch (Exception e)
            {
                txn?.Rollback();
                throw new ApplicationException(cn.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }
            return responses;
        }

        public class SqlCommandFilterSet
        {
            public List<string> Filters { get; set; }
            public List<SqlParameter> Parameters { get; set; }

            public override string ToString()
            {
                var filterString = new StringBuilder();
                foreach (string filter in Filters)
                {
                    filterString.Append(filter);
                }
                return filterString.ToString();
            }
        }


        public class Db2CommandFilterSet
        {
            public List<string> Filters { get; set; }
            public List<iDB2Parameter> Parameters { get; set; }

            public override string ToString()
            {
                var filterString = new StringBuilder();
                foreach (string filter in Filters)
                {
                    filterString.Append(filter);
                }
                return filterString.ToString();
            }
        }

    }
}
