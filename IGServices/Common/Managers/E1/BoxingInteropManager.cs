using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using IBM.Data.DB2.iSeries;
using IGServices.Common.Managers.Enterprise;
using IGServices.Common.Types.E1;
using IGServices.Common.Types.Enterprise;
using IGServices.Common.Types.Response;
using IGServices.Common.Utilities;
using NLog;

namespace IGServices.Common.Managers.E1
{
    public class BoxingInteropManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public BoxingInteropManager(BusinessSystemSession e1Session)
        {
            var hash = new Dictionary<string, iDB2Command>
            {
                {"BoxingInteropManager.InsertF580200", InsertF580200()},
                {"BoxingInteropManager.InsertF580030A", InsertF580030A()}
            };
            foreach (var keyName in hash.Keys.Where(keyName => !e1Session.DbCommands.ContainsKey(keyName)))
            {
                e1Session.DbCommands.Add(keyName, hash[keyName]);
            }
        }

        /*********************************************************************************
         * Prepared Commands
         ********************************************************************************/
        private static iDB2Command GetF580200sCountDynamic(List<string> filters)
        {
            var filterString = new StringBuilder();
            if (filters.Any()) filterString.Append("WHERE ");
            foreach (var filter in filters)
            {
                filterString.Append(filter);
            }
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = $@"
                    SELECT 
                        BIEDDT
                        ,BIEDBT
                        ,BIZ58SCN
                        ,CONCAT(CONCAT(CONCAT(CONCAT('BIZ58SCN=', CAST(BIZ58SCN AS VARCHAR(4))), ' and '),CONCAT(CONCAT('BIEDBT=', CAST(BIEDBT AS VARCHAR(10))), ' and ')), CONCAT('BIEDDT=', CAST(BIEDDT AS VARCHAR(6)))) AS MSG
                        ,COUNT(*) AS RECS 
                    FROM
                        F580200
                    {filterString}
	                GROUP BY 
                        BIEDDT
                        ,BIEDBT
                        ,BIZ58SCN
                        ,CONCAT(CONCAT(CONCAT(CONCAT('BIZ58SCN=', CAST(BIZ58SCN AS VARCHAR(4))), ' and '),CONCAT(CONCAT('BIEDBT=', CAST(BIEDBT AS VARCHAR(10))), ' and ')), CONCAT('BIEDDT=', CAST(BIEDDT AS VARCHAR(6))))
	                ORDER BY BIEDDT DESC, BIZ58SCN DESC
	                FETCH FIRST 1 ROWS ONLY"
            };
            return cmd;
        }


        private static iDB2Command InsertF580200()
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = @"
                    INSERT INTO F580200 (
                        BIEDUS,
                        BIEDBT,
                        BIEDTN,
                        BIEDLN,
                        BIEDDT,
                        BIEDSP,
                        BIDOCO,
                        BIDCTO,
                        BILNID,
                        BIZ58UNID,
                        BIZ58BXCR,
                        BIZ58SNUM,
                        BIZ58RT,
                        BIZ58BOX,
                        BIZ58BXRO,
                        BIZ58SD,
                        BIZ58STK,
                        BIZ58ROW,
                        BIZ58SL,
                        BIZ58IGLN,
                        BIZ58SCN,
                        BIZ58RNSQ,
                        BIZ58ONSK,
                        BIZ58L1R,
                        BIZ58L1SLN,
                        BIZ58L2R,
                        BIZ58L2SLN,
                        BIZ58L3SLN,
                        BIZ58L3R,
                        BIURCD,
                        BIURDT,
                        BIURAT,
                        BIURAB,
                        BIURRF,
                        BIUSER,
                        BIPID,
                        BIJOBN,
                        BIUPMJ,
                        BITDAY,
                        BIZ58RID,
                        BIZ58SCHDJ,
                        BIMCU,
                        BIKCOO,
                        BIAN8,
                        BISHAN,
                        BIITM,
                        BILITM,
                        BIAITM,
                        BIRKCO,
                        BITRDJ,
                        BIRORN,
                        BIRCTO,
                        BIRLLN,
                        BIITWT,
                        BIWTUM,
                        BITRQTY,
                        BIUM,
                        BICFGID,
                        BIUKID)
                        VALUES(?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,
                                ?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,
                                ?,?,?,?,?,?,?,?,?)"
            };
            cmd.Parameters.Add(new iDB2Parameter("BIEDUS", iDB2DbType.iDB2Graphic, 10));
            cmd.Parameters.Add(new iDB2Parameter("BIEDBT", iDB2DbType.iDB2Graphic, 15));
            cmd.Parameters.Add(new iDB2Parameter("BIEDTN", iDB2DbType.iDB2Graphic, 22));
            cmd.Parameters.Add(new iDB2Parameter("BIEDLN", iDB2DbType.iDB2Decimal, 7));
            cmd.Parameters["BIEDLN"].Precision = 7;
            cmd.Parameters["BIEDLN"].Scale = 3;
            cmd.Parameters.Add(new iDB2Parameter("BIEDDT", iDB2DbType.iDB2Decimal, 6));
            cmd.Parameters["BIEDDT"].Precision = 6;
            cmd.Parameters["BIEDDT"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("BIEDSP", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("BIDOCO", iDB2DbType.iDB2Decimal, 8));
            cmd.Parameters["BIDOCO"].Precision = 8;
            cmd.Parameters["BIDOCO"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("BIDCTO", iDB2DbType.iDB2Graphic, 2));
            cmd.Parameters.Add(new iDB2Parameter("BILNID", iDB2DbType.iDB2Decimal, 6));
            cmd.Parameters["BILNID"].Precision = 6;
            cmd.Parameters["BILNID"].Scale = 3;
            cmd.Parameters.Add(new iDB2Parameter("BIZ58UNID", iDB2DbType.iDB2Graphic, 6));
            cmd.Parameters.Add(new iDB2Parameter("BIZ58BXCR", iDB2DbType.iDB2Decimal, 1));
            cmd.Parameters["BIZ58BXCR"].Precision = 1;
            cmd.Parameters["BIZ58BXCR"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("BIZ58SNUM", iDB2DbType.iDB2Graphic, 14));
            cmd.Parameters.Add(new iDB2Parameter("BIZ58RT", iDB2DbType.iDB2Graphic, 5));
            cmd.Parameters.Add(new iDB2Parameter("BIZ58BOX", iDB2DbType.iDB2Decimal, 5));
            cmd.Parameters["BIZ58BOX"].Precision = 5;
            cmd.Parameters["BIZ58BOX"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("BIZ58BXRO", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("BIZ58SD", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("BIZ58STK", iDB2DbType.iDB2Decimal, 3));
            cmd.Parameters["BIZ58STK"].Precision = 3;
            cmd.Parameters["BIZ58STK"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("BIZ58ROW", iDB2DbType.iDB2Decimal, 3));
            cmd.Parameters["BIZ58ROW"].Precision = 3;
            cmd.Parameters["BIZ58ROW"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("BIZ58SL", iDB2DbType.iDB2Decimal, 3));
            cmd.Parameters["BIZ58SL"].Precision = 3;
            cmd.Parameters["BIZ58SL"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("BIZ58IGLN", iDB2DbType.iDB2Graphic, 2));
            cmd.Parameters.Add(new iDB2Parameter("BIZ58SCN", iDB2DbType.iDB2Decimal, 5));
            cmd.Parameters["BIZ58SCN"].Precision = 5;
            cmd.Parameters["BIZ58SCN"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("BIZ58RNSQ", iDB2DbType.iDB2Decimal, 5));
            cmd.Parameters["BIZ58RNSQ"].Precision = 5;
            cmd.Parameters["BIZ58RNSQ"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("BIZ58ONSK", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("BIZ58L1R", iDB2DbType.iDB2Graphic, 2));
            cmd.Parameters.Add(new iDB2Parameter("BIZ58L1SLN", iDB2DbType.iDB2Decimal, 3));
            cmd.Parameters["BIZ58L1SLN"].Precision = 3;
            cmd.Parameters["BIZ58L1SLN"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("BIZ58L2R", iDB2DbType.iDB2Graphic, 2));
            cmd.Parameters.Add(new iDB2Parameter("BIZ58L2SLN", iDB2DbType.iDB2Decimal, 3));
            cmd.Parameters["BIZ58L2SLN"].Precision = 3;
            cmd.Parameters["BIZ58L2SLN"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("BIZ58L3SLN", iDB2DbType.iDB2Decimal, 3));
            cmd.Parameters["BIZ58L3SLN"].Precision = 3;
            cmd.Parameters["BIZ58L3SLN"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("BIZ58L3R", iDB2DbType.iDB2Graphic, 2));
            cmd.Parameters.Add(new iDB2Parameter("BIURCD", iDB2DbType.iDB2Graphic, 2));
            cmd.Parameters.Add(new iDB2Parameter("BIURDT", iDB2DbType.iDB2Decimal, 6));
            cmd.Parameters["BIURDT"].Precision = 6;
            cmd.Parameters["BIURDT"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("BIURAT", iDB2DbType.iDB2Decimal, 15));
            cmd.Parameters["BIURAT"].Precision = 15;
            cmd.Parameters["BIURAT"].Scale = 2;
            cmd.Parameters.Add(new iDB2Parameter("BIURAB", iDB2DbType.iDB2Decimal, 8));
            cmd.Parameters["BIURAB"].Precision = 8;
            cmd.Parameters["BIURAB"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("BIURRF", iDB2DbType.iDB2Graphic, 15));
            cmd.Parameters.Add(new iDB2Parameter("BIUSER", iDB2DbType.iDB2Graphic, 10));
            cmd.Parameters.Add(new iDB2Parameter("BIPID", iDB2DbType.iDB2Graphic, 10));
            cmd.Parameters.Add(new iDB2Parameter("BIJOBN", iDB2DbType.iDB2Graphic, 10));
            cmd.Parameters.Add(new iDB2Parameter("BIUPMJ", iDB2DbType.iDB2Decimal, 6));
            cmd.Parameters["BIUPMJ"].Precision = 6;
            cmd.Parameters["BIUPMJ"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("BITDAY", iDB2DbType.iDB2Decimal, 6));
            cmd.Parameters["BITDAY"].Precision = 6;
            cmd.Parameters["BITDAY"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("BIZ58RID", iDB2DbType.iDB2Decimal, 15));
            cmd.Parameters["BIZ58RID"].Precision = 15;
            cmd.Parameters["BIZ58RID"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("BIZ58SCHDJ", iDB2DbType.iDB2Decimal, 6));
            cmd.Parameters["BIZ58SCHDJ"].Precision = 6;
            cmd.Parameters["BIZ58SCHDJ"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("BIMCU", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("BIKCOO", iDB2DbType.iDB2Graphic, 5));
            cmd.Parameters.Add(new iDB2Parameter("BIAN8", iDB2DbType.iDB2Decimal, 8));
            cmd.Parameters["BIAN8"].Precision = 8;
            cmd.Parameters["BIAN8"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("BISHAN", iDB2DbType.iDB2Decimal, 8));
            cmd.Parameters["BISHAN"].Precision = 8;
            cmd.Parameters["BISHAN"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("BIITM", iDB2DbType.iDB2Decimal, 8));
            cmd.Parameters["BIITM"].Precision = 8;
            cmd.Parameters["BIITM"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("BILITM", iDB2DbType.iDB2Graphic, 25));
            cmd.Parameters.Add(new iDB2Parameter("BIAITM", iDB2DbType.iDB2Graphic, 25));
            cmd.Parameters.Add(new iDB2Parameter("BIRKCO", iDB2DbType.iDB2Graphic, 5));
            cmd.Parameters.Add(new iDB2Parameter("BITRDJ", iDB2DbType.iDB2Decimal, 6));
            cmd.Parameters["BITRDJ"].Precision = 6;
            cmd.Parameters["BITRDJ"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("BIRORN", iDB2DbType.iDB2Graphic, 8));
            cmd.Parameters.Add(new iDB2Parameter("BIRCTO", iDB2DbType.iDB2Graphic, 2));
            cmd.Parameters.Add(new iDB2Parameter("BIRLLN", iDB2DbType.iDB2Decimal, 7));
            cmd.Parameters["BIRLLN"].Precision = 7;
            cmd.Parameters["BIRLLN"].Scale = 3;
            cmd.Parameters.Add(new iDB2Parameter("BIITWT", iDB2DbType.iDB2Decimal, 15));
            cmd.Parameters["BIITWT"].Precision = 15;
            cmd.Parameters["BIITWT"].Scale = 4;
            cmd.Parameters.Add(new iDB2Parameter("BIWTUM", iDB2DbType.iDB2Graphic, 2));
            cmd.Parameters.Add(new iDB2Parameter("BITRQTY", iDB2DbType.iDB2Decimal, 15));
            cmd.Parameters["BITRQTY"].Precision = 15;
            cmd.Parameters["BITRQTY"].Scale = 3;
            cmd.Parameters.Add(new iDB2Parameter("BIUM", iDB2DbType.iDB2Graphic, 2));
            cmd.Parameters.Add(new iDB2Parameter("BICFGID", iDB2DbType.iDB2Decimal, 15));
            cmd.Parameters["BICFGID"].Precision = 15;
            cmd.Parameters["BICFGID"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("BIUKID", iDB2DbType.iDB2Decimal, 15));
            cmd.Parameters["BIUKID"].Precision = 15;
            cmd.Parameters["BIUKID"].Scale = 0;
            return cmd;
        }

        private static iDB2Command InsertF580030A()
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = @"
                    INSERT INTO F580030A (
                        QAUKID,
                        QAEDSP,
	                    QAEDCT,
	                    QAMCU,
	                    QAEDBT,
	                    QAZ58SCN,
	                    QACRTU,
	                    QACPGM,
	                    QAJOBN,
	                    QACRTDT,
	                    QACRTM,
	                    QAUPMB,
	                    QAUPGM,
	                    QAMJOB)
                    VALUES(?,?,?,?,?,?,?,?,?,?,?,?,?,?)"
            };
            cmd.Parameters.Add(new iDB2Parameter("QAUKID", iDB2DbType.iDB2Decimal, 15));
            cmd.Parameters["QAUKID"].Precision = 15;
            cmd.Parameters["QAUKID"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("QAEDSP", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("QAEDCT", iDB2DbType.iDB2Graphic, 2));
            cmd.Parameters.Add(new iDB2Parameter("QAMCU", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("QAEDBT", iDB2DbType.iDB2Graphic, 15));
            cmd.Parameters.Add(new iDB2Parameter("QAZ58SCN", iDB2DbType.iDB2Decimal, 5));
            cmd.Parameters["QAZ58SCN"].Precision = 5;
            cmd.Parameters["QAZ58SCN"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("QACRTU", iDB2DbType.iDB2Graphic, 10));
            cmd.Parameters.Add(new iDB2Parameter("QACPGM", iDB2DbType.iDB2Graphic, 10));
            cmd.Parameters.Add(new iDB2Parameter("QAJOBN", iDB2DbType.iDB2Graphic, 10));
            cmd.Parameters.Add(new iDB2Parameter("QACRTDT", iDB2DbType.iDB2Decimal, 6));
            cmd.Parameters["QACRTDT"].Precision = 6;
            cmd.Parameters["QACRTDT"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("QACRTM", iDB2DbType.iDB2Decimal, 6));
            cmd.Parameters["QACRTM"].Precision = 6;
            cmd.Parameters["QACRTM"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("QAUPMB", iDB2DbType.iDB2Graphic, 10));
            cmd.Parameters.Add(new iDB2Parameter("QAUPGM", iDB2DbType.iDB2Graphic, 10));
            cmd.Parameters.Add(new iDB2Parameter("QAMJOB", iDB2DbType.iDB2Graphic, 10));
            return cmd;
        }

        public void InsertE1BoxingInterops(BusinessSystemSession e1Session, iDB2Transaction e1Trans, BoxingSchedule boxingSchedule)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info("BEGIN: " + DateTime.Now.ToLocalTime());
            try
            {
                var cmd = (e1Session.DbCommands.ContainsKey("BoxingInteropManager.InsertF580200")) ? e1Session.DbCommands["BoxingInteropManager.InsertF580200"] : InsertF580200();
                cmd.Connection = e1Session.DbConnection;
                cmd.Transaction = e1Trans;
                cmd.CommandTimeout = int.Parse(ConfigurationManager.AppSettings["BoxingQueryTimeout"]);
                var rowCount = 0;
                foreach (var boxInterop in boxingSchedule.BoxF580200.BoxInterops)
                {
                    cmd.Parameters["BIEDUS"].Value = boxInterop.BIEDUS;
                    cmd.Parameters["BIEDBT"].Value = boxInterop.BIEDBT;
                    cmd.Parameters["BIEDTN"].Value = boxInterop.BIEDTN;
                    cmd.Parameters["BIEDLN"].Value = boxInterop.BIEDLN;
                    cmd.Parameters["BIEDDT"].Value = boxInterop.BIEDDT;
                    cmd.Parameters["BIEDSP"].Value = boxInterop.BIEDSP;
                    cmd.Parameters["BIDOCO"].Value = boxInterop.BIDOCO;
                    cmd.Parameters["BIDCTO"].Value = boxInterop.BIDCTO;
                    cmd.Parameters["BILNID"].Value = boxInterop.BILNID;
                    cmd.Parameters["BIZ58UNID"].Value = boxInterop.BIZ58UNID;
                    cmd.Parameters["BIZ58BXCR"].Value = boxInterop.BIZ58BXCR;
                    cmd.Parameters["BIZ58SNUM"].Value = boxInterop.BIZ58SNUM;
                    cmd.Parameters["BIZ58RT"].Value = boxInterop.BIZ58RT;
                    cmd.Parameters["BIZ58BOX"].Value = boxInterop.BIZ58BOX;
                    cmd.Parameters["BIZ58BXRO"].Value = boxInterop.BIZ58BXRO;
                    cmd.Parameters["BIZ58SD"].Value = boxInterop.BIZ58SD;
                    cmd.Parameters["BIZ58STK"].Value = boxInterop.BIZ58STK;
                    cmd.Parameters["BIZ58ROW"].Value = boxInterop.BIZ58ROW;
                    cmd.Parameters["BIZ58SL"].Value = boxInterop.BIZ58SL;
                    cmd.Parameters["BIZ58IGLN"].Value = boxInterop.BIZ58IGLN;
                    cmd.Parameters["BIZ58SCN"].Value = boxInterop.BIZ58SCN;
                    cmd.Parameters["BIZ58RNSQ"].Value = boxInterop.BIZ58RNSQ;
                    cmd.Parameters["BIZ58ONSK"].Value = boxInterop.BIZ58ONSK;
                    cmd.Parameters["BIZ58L1R"].Value = boxInterop.BIZ58L1R;
                    cmd.Parameters["BIZ58L1SLN"].Value = boxInterop.BIZ58L1SLN;
                    cmd.Parameters["BIZ58L2R"].Value = boxInterop.BIZ58L2R;
                    cmd.Parameters["BIZ58L2SLN"].Value = boxInterop.BIZ58L2SLN;
                    cmd.Parameters["BIZ58L3SLN"].Value = boxInterop.BIZ58L3SLN;
                    cmd.Parameters["BIZ58L3R"].Value = boxInterop.BIZ58L3R;
                    cmd.Parameters["BIURCD"].Value = boxInterop.BIURCD;
                    cmd.Parameters["BIURDT"].Value = boxInterop.BIURDT;
                    cmd.Parameters["BIURAT"].Value = boxInterop.BIURAT;
                    cmd.Parameters["BIURAB"].Value = boxInterop.BIURAB;
                    cmd.Parameters["BIURRF"].Value = boxInterop.BIURRF;
                    cmd.Parameters["BIUSER"].Value = boxInterop.BIUSER;
                    cmd.Parameters["BIPID"].Value = boxInterop.BIPID;
                    cmd.Parameters["BIJOBN"].Value = boxInterop.BIJOBN;
                    cmd.Parameters["BIUPMJ"].Value = boxInterop.BIUPMJ;
                    cmd.Parameters["BITDAY"].Value = boxInterop.BITDAY;
                    cmd.Parameters["BIZ58RID"].Value = boxInterop.BIZ58RID;
                    cmd.Parameters["BIZ58SCHDJ"].Value = boxInterop.BIZ58SCHDJ;
                    cmd.Parameters["BIMCU"].Value = boxInterop.BIMCU;
                    cmd.Parameters["BIKCOO"].Value = boxInterop.BIKCOO;
                    cmd.Parameters["BIAN8"].Value = boxInterop.BIAN8;
                    cmd.Parameters["BISHAN"].Value = boxInterop.BISHAN;
                    cmd.Parameters["BIITM"].Value = boxInterop.BIITM;
                    cmd.Parameters["BILITM"].Value = boxInterop.BILITM;
                    cmd.Parameters["BIAITM"].Value = boxInterop.BIAITM;
                    cmd.Parameters["BIRKCO"].Value = boxInterop.BIRKCO;
                    cmd.Parameters["BITRDJ"].Value = boxInterop.BITRDJ;
                    cmd.Parameters["BIRORN"].Value = boxInterop.BIRORN;
                    cmd.Parameters["BIRCTO"].Value = boxInterop.BIRCTO;
                    cmd.Parameters["BIRLLN"].Value = boxInterop.BIRLLN;
                    cmd.Parameters["BIITWT"].Value = boxInterop.BIITWT;
                    cmd.Parameters["BIWTUM"].Value = boxInterop.BIWTUM;
                    cmd.Parameters["BITRQTY"].Value = boxInterop.BITRQTY;
                    cmd.Parameters["BIUM"].Value = boxInterop.BIUM;
                    cmd.Parameters["BICFGID"].Value = boxInterop.BICFGID;
                    cmd.Parameters["BIUKID"].Value = boxInterop.BIUKID;
                    Logger.Trace(Logging.DbCmdString(cmd));
                    rowCount += cmd.ExecuteNonQuery();                    
                }
                Logger.Trace("ROWS: " + rowCount);
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + e1Session.DbConnection.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
                throw new ApplicationException(e1Session.DbConnection.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }
            finally
            {
                Logger.Info($"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }
        }

        public void InsertE1BoxingTrigger(BusinessSystemSession e1Session, iDB2Transaction trans, BoxingSchedule boxingSchedule)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"BEGIN: {DateTime.Now.ToLocalTime()}");
            try
            {
                var cmd = InsertF580030A();
                cmd.Connection = e1Session.DbConnection;
                cmd.Transaction = trans;
                cmd.Parameters["QAUKID"].Value = boxingSchedule.BoxF580200.BoxControlTrigger.QAUKID;
                cmd.Parameters["QAEDSP"].Value = boxingSchedule.BoxF580200.BoxControlTrigger.QAEDSP;
                cmd.Parameters["QAEDCT"].Value = boxingSchedule.BoxF580200.BoxControlTrigger.QAEDCT;
                cmd.Parameters["QAMCU"].Value = boxingSchedule.BoxF580200.BoxControlTrigger.QAMCU;
                cmd.Parameters["QAEDBT"].Value = boxingSchedule.BoxF580200.BoxControlTrigger.QAEDBT;
                cmd.Parameters["QAZ58SCN"].Value = boxingSchedule.BoxF580200.BoxControlTrigger.QAZ58SCN;
                cmd.Parameters["QACRTU"].Value = boxingSchedule.BoxF580200.BoxControlTrigger.QACRTU;
                cmd.Parameters["QACPGM"].Value = boxingSchedule.BoxF580200.BoxControlTrigger.QACPGM;
                cmd.Parameters["QAJOBN"].Value = boxingSchedule.BoxF580200.BoxControlTrigger.QAJOBN;
                cmd.Parameters["QACRTDT"].Value = boxingSchedule.BoxF580200.BoxControlTrigger.QACRTDT;
                cmd.Parameters["QACRTM"].Value = boxingSchedule.BoxF580200.BoxControlTrigger.QACRTM;
                cmd.Parameters["QAUPMB"].Value = boxingSchedule.BoxF580200.BoxControlTrigger.QAUPMB;
                cmd.Parameters["QAUPGM"].Value = boxingSchedule.BoxF580200.BoxControlTrigger.QAUPGM;
                cmd.Parameters["QAMJOB"].Value = boxingSchedule.BoxF580200.BoxControlTrigger.QAMJOB;
                cmd.CommandTimeout = int.Parse(ConfigurationManager.AppSettings["BoxingQueryTimeout"]);
                Logger.Trace(Logging.DbCmdString(cmd));
                var rowCount = cmd.ExecuteNonQuery();
                Logger.Trace($"ROWS: {rowCount}");
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {e1Session.DbConnection.ConnectionString}  {e.Message}: {e.StackTrace}");
                throw new ApplicationException($"{e1Session.DbConnection.ConnectionString}  {e.Message}: {e.StackTrace}");
            }
            finally
            {
                Logger.Info($"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }
        }

        public void BuildBoxingInterops(BoxingSchedule boxingSchedule)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"BEGIN: {DateTime.Now.ToLocalTime()}");
            try
            {
                var boxInterops = new BoxInterops();
                var iTransactionNum = 0;
                foreach (var boxInterop in boxingSchedule.BoxItems.Select(boxItem => new BoxingInterop
                {
                    BIEDUS = "SVCSOA",
                    BIEDBT = boxItem.TransferBatchNum,
                    BIUKID = boxItem.UniqueID,
                    BIEDLN = 1000,
                    BIEDDT = Utilities.Common.CastToE1Date(DateTime.Now),
                    BIEDSP = "",
                    BIDOCO = boxItem.WorkOrderNum,
                    BIDCTO = boxItem.WorkOrderNum > 0 ? "WO" : boxItem.SalesOrderType,
                    BILNID = boxItem.WorkOrderNum > 0 ? 0 : boxItem.SalesOrderLine,
                    BIZ58UNID = boxItem.UnitId,
                    BIZ58BXCR = Convert.ToInt16(boxItem.BoxCrate),
                    BIZ58SNUM = boxItem.SerialNum,
                    BIZ58RT = boxItem.RackType,
                    BIZ58BOX = boxItem.BoxNum,
                    BIZ58BXRO = boxItem.BoxRotated ? "1" : "",
                    BIZ58SD = boxItem.Side,
                    BIZ58STK = boxItem.Stack,
                    BIZ58ROW = boxItem.Row,
                    BIZ58SL = boxItem.Slot,
                    BIZ58IGLN = boxItem.IGLine,
                    BIZ58SCN = boxItem.ScheduleNum,
                    BIZ58RNSQ = boxItem.RunSequence,
                    BIZ58L1R = boxItem.Lite1Rack,
                    BIZ58L1SLN = boxItem.Lite1Slot,
                    BIZ58L2R = boxItem.Lite2Rack,
                    BIZ58L2SLN = boxItem.Lite2Slot,
                    BIZ58L3R = boxItem.Lite3Rack,
                    BIZ58L3SLN = boxItem.Lite3Slot,
                    BIURCD = "",
                    BIURDT = Utilities.Common.CastToE1Date(boxItem.UpdateDate),
                    BIURAT = Utilities.Common.CastToE1Time(boxItem.UpdateDate),
                    BIUSER = "SVCSOA",
                    BIPID = ConfigurationManager.AppSettings["BIPID"],
                    BIUPMJ = Utilities.Common.CastToE1Date(DateTime.Now),
                    BITDAY = Utilities.Common.CastToE1Time(DateTime.Now),
                    BIZ58ONSK = boxItem.OnSkid,
                    BIZ58RID = boxItem.RackId,
                    BIMCU = boxItem.PlantNum.PadLeft(12),
                    BoxHoldId = boxItem.BoxHoldId
                }))
                {
                    iTransactionNum++;  // break this for key violation testing
                    var transactionNum = Convert.ToString(iTransactionNum);
                    boxInterop.BIEDTN = transactionNum.PadLeft(10, '0');
                    boxInterops.Add(boxInterop);
                }
                boxingSchedule.BoxF580200.BoxInterops = boxInterops;
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                throw new ApplicationException($"{e.Message}: {e.StackTrace}");
            }
            finally
            {
                Logger.Info($"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }
        }

        public void BuildBoxingTrigger(BoxingSchedule boxSchedule, SqlConnection cnCorpInt = null)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"BEGIN: {DateTime.Now.ToLocalTime()}");
            try
            {
                boxSchedule.BoxF580200.BoxControlTrigger = new ShopFloorControlTrigger();
                if (cnCorpInt != null)
                {
                    boxSchedule.BoxF580200.BoxControlTrigger.QAUKID = BatchNumManager.GetNextQAUKID(cnCorpInt);
                }
                boxSchedule.BoxF580200.BoxControlTrigger.QAEDSP = "";
                boxSchedule.BoxF580200.BoxControlTrigger.QAEDCT = "BI";
                boxSchedule.BoxF580200.BoxControlTrigger.QAMCU = boxSchedule.PlantNum.PadLeft(12);
                boxSchedule.BoxF580200.BoxControlTrigger.QAEDBT = boxSchedule.TransferBatchNum;
                boxSchedule.BoxF580200.BoxControlTrigger.QAZ58SCN = boxSchedule.ScheduleNum;
                boxSchedule.BoxF580200.BoxControlTrigger.QACRTU = "SVCSOA";
                boxSchedule.BoxF580200.BoxControlTrigger.QACPGM = "SVCSOA";
                boxSchedule.BoxF580200.BoxControlTrigger.QAJOBN = "SVCSOA";
                boxSchedule.BoxF580200.BoxControlTrigger.QACRTDT = Utilities.Common.CastToE1Date(DateTime.Now.ToLocalTime());
                boxSchedule.BoxF580200.BoxControlTrigger.QACRTM = Utilities.Common.CastToE1Time(DateTime.Now.ToLocalTime());
                boxSchedule.BoxF580200.BoxControlTrigger.QAUPMB = "";
                boxSchedule.BoxF580200.BoxControlTrigger.QAUPGM = "";
                boxSchedule.BoxF580200.BoxControlTrigger.QAMJOB = "";
                boxSchedule.BoxF580200.BoxControlTrigger.QAUPMJ = 0;
                boxSchedule.BoxF580200.BoxControlTrigger.QAUPMT = 0;
            }
            catch (Exception e)
            {
                if (cnCorpInt != null)
                {
                    Logger.Error($"ERROR: {cnCorpInt.ConnectionString}  {e.Message}: {e.StackTrace}");
                    throw new ApplicationException($"{cnCorpInt.ConnectionString}  {e.Message}: {e.StackTrace}");
                }
                else
                {
                    Logger.Error($"ERROR:  {e.Message}: {e.StackTrace}");
                    throw new ApplicationException($"{e.Message}: {e.StackTrace}");
                }
            }
            finally
            {
                Logger.Info($"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }
        }

        public static Message GetF580200sCountDynamic(BusinessSystemSession e1Session, string plantNum, decimal scheduleNum, BoxItems boxItems)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"BEGIN: {DateTime.Now.ToLocalTime()}");
            var workOrdersCount = 0;
            var msg = new Message("0", "0");
            var filters = new List<string>();
            var parameters = new List<iDB2Parameter>();
            try
            {
                // Plant Num
                filters.Add("BIMCU = ? AND \r\n");
                parameters.Add(new iDB2Parameter("PlantNum", iDB2DbType.iDB2Graphic, 12));
                parameters.Last().Value = plantNum.PadLeft(12);
                // MTOSchedule Num
                filters.Add("BIZ58SCN = ? AND \r\n");
                parameters.Add(new iDB2Parameter("BIZ58SCN", iDB2DbType.iDB2Decimal));
                parameters.Last().Precision = 5;
                parameters.Last().Scale = 0;
                parameters.Last().Value = scheduleNum;
                // Work Orders
                var workOrders = boxItems.Select(b => b.WorkOrderNum).Distinct();
                var inString = new StringBuilder();
                foreach (var workOrder in workOrders)
                {
                    inString.Append(workOrder + ",");
                }
                if (inString.Length > 0) inString.Length -= 1; //Strip off trailing comma.
                filters.Add("BIDOCO IN (" + inString + ") \r\n");
                var cmd = GetF580200sCountDynamic(filters);
                foreach (var parameter in parameters)
                {
                    cmd.Parameters.Add(parameter);
                }
                cmd.Connection = e1Session.DbConnection;
                cmd.CommandTimeout = int.Parse(ConfigurationManager.AppSettings["BoxingQueryTimeout"]);
                Logger.Trace(Logging.DbCmdString(cmd));
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
                while (reader.Read())
                {
                    var oldScheduleNumber = CastDbNull.To(reader["MSG"], "");
                    workOrdersCount = CastDbNull.To(reader["RECS"], 0);
                    msg = new Message(oldScheduleNumber.ToString(CultureInfo.InvariantCulture), workOrdersCount.ToString());
                }
                reader.Close();
                Logger.Trace($"ROWS: {workOrdersCount}");
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {e1Session.DbConnection.ConnectionString}  {e.Message}: {e.StackTrace}");
                throw new ApplicationException($"{e1Session.DbConnection.ConnectionString }  { e.Message}: { e.StackTrace}");
            }
            finally
            {
                Logger.Info($"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }
            return msg;
        }

        public bool RollBack(iDB2Transaction e1Trans)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"BEGIN: {DateTime.Now.ToLocalTime()}");
            var isRolledBack = false;
            try
            {
                e1Trans.Rollback();
                isRolledBack = true;
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {e1Trans.Connection.ConnectionString}  {e.Message}: {e.StackTrace}");
                throw new ApplicationException($"{e1Trans.Connection.ConnectionString}  {e.Message}: {e.StackTrace}");
            }
            finally
            {
                Logger.Info($"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }
            return isRolledBack;
        }
    }
}
