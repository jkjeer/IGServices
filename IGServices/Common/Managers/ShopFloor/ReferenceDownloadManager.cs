using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IBM.Data.DB2.iSeries;
using IGServices.Common.Managers.Enterprise;
using IGServices.Common.Types.E1;
using IGServices.Common.Types.Enterprise;
using IGServices.Common.Types.Response;
using IGServices.Common.Types.ShopFloor;
using IGServices.Common.Utilities;
using NLog;

namespace IGServices.Common.Managers.ShopFloor
{
    public class ReferenceDownloadManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public ReferenceDownloadManager(BusinessSystemSession e1Session)
        {
            var hash = new Dictionary<string, iDB2Command>
            {
                //{"WorkOrdManager.GetRelatedOrders", GetRelatedOrders()}
            };
            foreach (var keyName in hash.Keys.Where(keyName => !e1Session.DbCommands.ContainsKey(keyName)))
            {
                e1Session.DbCommands.Add(keyName, hash[keyName]);
            }
        }

        public ProcessResponse Process(BusinessSystemSession e1Session, SqlConnection cnSFInt, Plant plant, E1TimeSpan span, bool useHighWaterMark, decimal orderNum)
        {
            var response = new ProcessResponse("Reference Download");
            //var employeeTable = new EmployeeTable();
            //var workOrderTableManager = new WorkOrderDownloadManager(e1Session);
            //var workOrderHwMark = (useHighWaterMark) ? workOrderTableManager.GetHighWaterMark(e1Session, cnSFInt, plant) : null;
            //var workOrderReader = workOrderTableManager.GetReader(e1Session, cnSFInt, plant, span, workOrderHwMark, 0);
            //workOrderTable.Add(workOrderReader);
            //if (workOrderTable.Rows.Count > 0)
            //{
            //    // Bulk Copy
            //    var woBulkResponse = BulkCopyManager.BulkTable(workOrderTable.ExtendedProperties["Target"].ToString(), workOrderTable, cnSFInt);
            //    response.SqlBulkCopyResponses.Add(woBulkResponse);
            //    // Set High Water Mark
            //    if (useHighWaterMark)
            //    {
            //        workOrderHwMark.UniqueID = (long)workOrderTable.MaxId;
            //        workOrderTableManager.SetHighWaterMark(cnSFInt, workOrderHwMark);
            //    }
            //}
            return response;
        }
    }
}
