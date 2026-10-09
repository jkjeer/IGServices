using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using IBM.Data.DB2.iSeries;
using IGServices.Common.Managers.E1;
using IGServices.Common.Types.E1;
using IGServices.Common.Types.Enterprise;
using IGServices.Common.Types.Response;
using IGServices.Common.Types.ShopFloor;
using IGServices.Common.Utilities;
using NLog;

namespace IGServices.Common.Managers.ShopFloor
{
    public class WorkOrderTableManager
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public WorkOrderTableManager(BusinessSystemSession session)
        {
            var hash = new Dictionary<string, iDB2Command>
            {
                //{"WorkOrdManager.GetRelatedOrders", GetRelatedOrders()}
            };
            foreach (var keyName in hash.Keys.Where(keyName => !session.DbCommands.ContainsKey(keyName)))
            {
                session.DbCommands.Add(keyName, hash[keyName]);
            }
        }

        public iDB2Command GetReader(List<string> filters)
        {
            var filterString = new StringBuilder();
            foreach (string filter in filters)
            {
                filterString.Append(filter);
            }

            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = String.Format(@"
                SELECT 
	                WAUKID,
	                WADOCO, 
	                WADCTO, 
                    WALOTN,
	                WAMCU,
	                WASRST,
	                WAAN8, 
	                WATRDJ,
	                WADRQJ,
                    WADCG,
	                WAITM,
	                IBSTKT,
	                WAUORG,
	                WAMUPM,
	                WAUPMT
                FROM 
	                F584801
	                INNER JOIN F4102
	                ON	IBMCU = WAMCU AND
		                IBITM = WAITM
	            WHERE 
                    {0}
                    1=1", filterString.ToString());
            return cmd;
        }

        public IDataReader GetReader(BusinessSystemSession session, SqlConnection cn, Plant plant, TimeSpan lookbackSpan, HighWaterMark highWaterMark, decimal orderNum)
        {
            var cmd = new iDB2Command();
            List<string> filters = new List<string>();
            List<iDB2Parameter> parameters = new List<iDB2Parameter>();
            try
            {
                // Plant
                if (!String.IsNullOrEmpty(plant.PlantCode))
                {
                    filters.Add("WAMCU = ? AND\r\n");
                    parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
                    parameters.Last().Value = plant.PlantNum.PadLeft(12);
                }

                // Order Num
                if (orderNum > 0)
                {
                    filters.Add("WADOCO = ? AND \r\n");
                    parameters.Add(new iDB2Parameter("DOCO", iDB2DbType.iDB2Decimal));
                    parameters.Last().Precision = 8;
                    parameters.Last().Scale = 0;
                    parameters.Last().Value = orderNum;
                }

                // Min Id
                if (highWaterMark != null)
                {
                    parameters.Add(new iDB2Parameter("ID", iDB2DbType.iDB2Decimal));
                    parameters.Last().Precision = 15;
                    parameters.Last().Scale = 0;
                    parameters.Last().Value = highWaterMark.UniqueID;
                }

                cmd = GetReader(filters);
                foreach (iDB2Parameter parameter in parameters)
                {
                    cmd.Parameters.Add(parameter);
                }
                cmd.Connection = session.DbConnection;
                _logger.Debug(Logging.DbCmdString(cmd));

                _logger.Trace("Start Get Orders");
                return cmd.ExecuteReader(CommandBehavior.SingleResult);
            }
            catch (Exception e)
            {
                throw new FaultException(e.Message);
            }
        }

        /******************************************************************
         * High Water Mark
         ******************************************************************/
        public HighWaterMark GetHighWaterMark(BusinessSystemSession session, SqlConnection cn, Plant plant)
        {
            var mark = new HighWaterMark();
            try
            {
                var hwMgr = new E1DownloadHighWaterMarkManager();
                mark = hwMgr.GetHighWaterMark(session, cn, plant.PlantNum, "E1_F584801");
            }
            catch (Exception e)
            {
                throw new FaultException(e.Message);
            }
            return mark;
        }

        public SqlDmlResponse SetHighWaterMark(SqlConnection cn, HighWaterMark mark)
        {
            var response = new SqlDmlResponse();
            try
            {
                var hwMgr = new E1DownloadHighWaterMarkManager();
                response = hwMgr.SaveHighWaterMark(cn, mark);
            }
            catch (Exception e)
            {
                throw new FaultException(e.Message);
            }
            return response;
        }
    }
}
