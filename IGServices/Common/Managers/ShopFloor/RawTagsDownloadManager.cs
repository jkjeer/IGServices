using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.ServiceModel;
using System.Text;
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
    public class RawTagsDownloadManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        //public RawTagsDownloadManager(BusinessSystemSession session)
        //{
        //    var hash = new Dictionary<string, iDB2Command>
        //    {
        //        //{"WorkOrdManager.GetRelatedOrders", GetRelatedOrders()}
        //    };
        //    foreach (var keyName in hash.Keys.Where(keyName => !session.DbCommands.ContainsKey(keyName)))
        //    {
        //        session.DbCommands.Add(keyName, hash[keyName]);
        //    }
        //}

        public ProcessResponse Process(BusinessSystemSession session, SqlConnection cn, Plant plant, decimal rackId)
        {
            if (rackId > 0)
            {
                return ProcessRack(session, cn, plant, rackId);
            }
            else
            {
                return ProcessPlant(session, cn, plant);
            }
        }

        public ProcessResponse ProcessPlant(BusinessSystemSession session, SqlConnection cn, Plant plant)
        {
            var response = new ProcessResponse("Raw Tags Plant Download");
            var table = new RawTagsTable();
            // Set Domain
            //var rt1 = new ResponseTime("Mark Selected From E1");
            response.SqlDmlResponses.Add(SetDomainProgress(session, plant.PlantNum));
            //response.ResponseTimes.Add(rt1.Done());
            // Get Domain
            //var rt2 = new ResponseTime("Get From E1");
            var reader = GetReader(session, cn, plant);
            //response.ResponseTimes.Add(rt2.Done());
            table.Add(reader);
            if (table.Rows.Count > 0)
            {
                // Bulk Copy
                //var rt3 = new ResponseTime("Bulk Copy");
                var bulkResponse = BulkCopyManager.BulkTable(table.ExtendedProperties["Target"].ToString(), table, cn);
                response.SqlBulkCopyResponses.Add(bulkResponse);
                //response.ResponseTimes.Add(rt3.Done());
                if (!bulkResponse.HasErrors())
                {
                    // Complete Domain
                    //var rt4 = new ResponseTime("Mark Completed From E1");
                    response.SqlDmlResponses.Add(SetDomainComplete(session, plant.PlantNum));
                    //response.ResponseTimes.Add(rt4.Done());
                }
            }
            return response;
        }

        public ProcessResponse ProcessRack(BusinessSystemSession session, SqlConnection cn, Plant plant, decimal rackId)
        {
            var response = new ProcessResponse("Raw Tags Rack Download");
            var table = new RawTagsTable();
            // Get Rack
            //var rt2 = new ResponseTime("Get From E1");
            var reader = GetLastRackReader(session, cn, plant, rackId);
            //response.ResponseTimes.Add(rt2.Done());
            table.Add(reader);
            if (table.Rows.Count > 0)
            {
                // Bulk Copy
                //var rt3 = new ResponseTime("Bulk Copy");
                var bulkResponse = BulkCopyManager.BulkTable(table.ExtendedProperties["Target"].ToString(), table, cn);
                response.SqlBulkCopyResponses.Add(bulkResponse);
                //response.ResponseTimes.Add(rt3.Done());
            }
            return response;
        }

        /******************************************************************
         * Generic
         ******************************************************************/
        public iDB2Command GetReader(List<string> filters)
        {
            var filterString = new StringBuilder();
            foreach (var filter in filters)
            {
                filterString.Append(filter);
            }
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandTimeout = 0,
                CommandText = $@"
                SELECT 
                    REMCU,
                    RELITM,
                    REZ58SN,
                    REZ58RID,
                    RESEQNO,
                    REVEND,
                    RELOCN,
                    REEDSP,
                    REZ58DST,
                    REZ58L1TK,
                    REZ58L11C,
                    REZ58L1CL,
                    REZ58L1HT,
                    REZ58L12C,
                    REZ58L1PX,
                    REZ58SHAPE,
                    REZ58L1BDM,
                    REZ58L1BNM,
                    REZ58L1BDN,
                    REZ58L1LDM,
                    REZ58L1LNM,
                    REZ58L1LDN,
                    REZ58L1RDM,
                    REZ58L1RNM,
                    REZ58L1RDN,
                    REZ58L1TDM,
                    REZ58L1TNM,
                    REZ58L1TDN,
                    REZ58L11DM,
                    REZ58L11NM,
                    REZ58L11DN,
                    REZ58L12DM,
                    REZ58L12NM,
                    REZ58L12DN,
                    REZ58MDT,
                    REPQOH,
                    REZ58BSF,
                    REZ58USF,
                    REUSER,
                    REPID,
                    REJOBN,
                    REUPMJ,
                    RETDAY,
                    REZ58L1LPT,
                    REZ58L1PQL,
                    REZ58RT,
                    REZ58LPR,
                    REGLPT,
                    REZ581BMIM,
                    REZ581LMIM,
                    REZ58L1ALT,
                    REZ58RCD,
                    REZ58AID
                FROM 
	                F580003A
	            WHERE 
                    {filterString}
                    REEDSP = 'U'"
            };
            return cmd;
        }

        public IDataReader GetReader(BusinessSystemSession session, SqlConnection cn, Plant plant)
        {
            var filters = new List<string>();
            var parameters = new List<iDB2Parameter>();
            try
            {
                // Plant
                filters.Add("REMCU = ? AND\r\n");
                parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
                parameters.Last().Value = plant.PlantNum.PadLeft(12);
                var cmd = GetReader(filters);
                foreach (var parameter in parameters)
                {
                    cmd.Parameters.Add(parameter);
                }
                cmd.Connection = session.DbConnection;
                Logger.Debug(Logging.DbCmdString(cmd));
                Logger.Trace("Start Get Orders");
                return cmd.ExecuteReader(CommandBehavior.SingleResult);
            }
            catch (Exception e)
            {
                throw new FaultException(e.Message);
            }
        }

        /******************************************************************
         * Generic
         ******************************************************************/
        public iDB2Command GetLastRackReader()
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandTimeout = 0,
                CommandText = @"
                    SELECT
                        * 
                    FROM 
                        (SELECT 
                            REMCU,
                            RELITM,
                            REZ58SN,
                            REZ58RID,
                            RESEQNO,
                            REVEND,
                            RELOCN,
                            REEDSP,
                            REZ58DST,
                            REZ58L1TK,
                            REZ58L11C,
                            REZ58L1CL,
                            REZ58L1HT,
                            REZ58L12C,
                            REZ58L1PX,
                            REZ58SHAPE,
                            REZ58L1BDM,
                            REZ58L1BNM,
                            REZ58L1BDN,
                            REZ58L1LDM,
                            REZ58L1LNM,
                            REZ58L1LDN,
                            REZ58L1RDM,
                            REZ58L1RNM,
                            REZ58L1RDN,
                            REZ58L1TDM,
                            REZ58L1TNM,
                            REZ58L1TDN,
                            REZ58L11DM,
                            REZ58L11NM,
                            REZ58L11DN,
                            REZ58L12DM,
                            REZ58L12NM,
                            REZ58L12DN,
                            REZ58MDT,
                            REPQOH,
                            REZ58BSF,
                            REZ58USF,
                            REUSER,
                            REPID,
                            REJOBN,
                            REUPMJ,
                            RETDAY,
                            REZ58L1LPT,
                            REZ58L1PQL,
                            REZ58RT,
                            REZ58LPR,
                            REGLPT,
                            REZ581BMIM,
                            REZ581LMIM,
                            REZ58L1ALT,
                            REZ58RCD,
                            REZ58AID,
		                    ROW_NUMBER() OVER (PARTITION BY REMCU, REZ58RID, REZ58SN ORDER BY RESEQNO DESC) AS ROWNUM
                        FROM 
	                        F580003A
	                    WHERE 
                            REMCU = ? AND
                            REZ58RID = ?) A
                    WHERE
                        A.ROWNUM = 1"
            };
            cmd.Parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("Z58RID", iDB2DbType.iDB2Decimal));
            cmd.Parameters["Z58RID"].Precision = 15;
            cmd.Parameters["Z58RID"].Scale = 0;
            return cmd;
        }

        public IDataReader GetLastRackReader(BusinessSystemSession session, SqlConnection cn, Plant plant, decimal rackId)
        {
            try
            {
                var cmd = GetLastRackReader();
                cmd.Connection = session.DbConnection;
                cmd.Parameters["MCU"].Value = plant.PlantNum.PadLeft(12);
                cmd.Parameters["Z58RID"].Value = rackId;
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
        public iDB2Command SetDomainProgress()
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandTimeout = 0,
                CommandText = (@"
                    UPDATE  
	                    F580003A
                    SET
                        REEDSP = 'U'
	                WHERE 
                        REMCU = ? AND
                        REEDSP = 'N'")
            };
            cmd.Parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
            return cmd;
        }

        public iDB2Command SetDomainComplete()
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandTimeout = 0,
                CommandText = (@"
                    UPDATE  
	                    F580003A
                    SET
                        REEDSP = 'Y'
	                WHERE 
                        REMCU = ? AND
                        REEDSP = 'U'")
            };
            cmd.Parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
            return cmd;
        }

        public SqlDmlResponse SetDomainProgress(BusinessSystemSession session, string plantNum)
        {
            var response = new SqlDmlResponse();
            try
            {
                var cmd = (session.DbCommands.ContainsKey("RawTagsDownloadManager.SetDomainProgress")) ? session.DbCommands["RawTagsDownloadManager.SetDomainProgress"] : SetDomainProgress();
                cmd.Connection = session.DbConnection;
                cmd.Parameters["MCU"].Value = plantNum.PadLeft(12);
                response.Init(cmd);
                response.RowsAffected = cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw new ApplicationException(session.DbConnection.ConnectionString + "  " + e);
            }
            return response;
        }

        public SqlDmlResponse SetDomainComplete(BusinessSystemSession session, string plantNum)
        {
            var response = new SqlDmlResponse();
            try
            {
                var cmd = (session.DbCommands.ContainsKey("RawTagsDownloadManager.SetDomainComplete")) ? session.DbCommands["RawTagsDownloadManager.SetDomainComplete"] : SetDomainComplete();
                cmd.Connection = session.DbConnection;
                cmd.Parameters["MCU"].Value = plantNum.PadLeft(12);
                response.Init(cmd);
                response.RowsAffected = cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw new ApplicationException(session.DbConnection.ConnectionString + "  " + e);
            }
            return response;
        }
    }
}
