using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using IBM.Data.DB2.iSeries;
using IGServices.Common.Utilities;
using IGServices.Common.Types.E1;
using IGServices.Common.Types.Response;
using IGServices.Common.Managers.Response;
using IGServices.Common.Types.ShopFloor;
using NLog;

namespace IGServices.Common.Managers.E1
{
    public class GlassConsumptionInteropManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public GlassConsumptionInteropManager(BusinessSystemSession e1Session)
        {
            var hash = new Dictionary<string, iDB2Command>
            {
                {"GlassConsumptionInteropManager.InsertF580030", InsertF580030()},
                {"GlassConsumptionInteropManager.InsertF580030A", InsertF580030A()}
            };
            foreach (var keyName in hash.Keys.Where(keyName => !e1Session.DbCommands.ContainsKey(keyName)))
            {
                e1Session.DbCommands.Add(keyName, hash[keyName]);
            }
        }

        /*********************************************************************************
         * Prepared Commands
         ********************************************************************************/
        public iDB2Command InsertF580030A()
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

        public iDB2Command InsertF580030()
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = @"
                    INSERT INTO F580030 (
                        SFUKID,
                        SFEDUS,
                        SFEDBT,
                        SFEDTN,
                        SFEDLN,
                        SFEDCT,
                        SFTYTN,
                        SFEDFT,
                        SFEDDT,
                        SFDRIN,
                        SFEDDL,
                        SFEDSP,
                        SFPNID,
                        SFTNAC,
                        SFMMCU,
                        SFACDT,
                        SFSTTM,
                        SFDTMS,
                        SFLITM,
                        SFLOTN,
                        SFZ58SN,
                        SFDOCO,
                        SFDCTO,
                        SFMCU,
                        SFZ58SCN,
                        SFZ58SCSN,
                        SFANP,
                        SFANPA,
                        SFSFT,
                        SFZ58PCR,
                        SFZ58RCD,
                        SFUORG,
                        SFSOCN,
                        SFLOCN,
                        SFZ58RID,
                        SFZ58RT,
                        SFZ58GWT,
                        SFZ58GWTT,
                        SFZ58IGLN,
                        SFZ58UNID,
                        SFZ58UID,
                        SFTKNR,
                        SFZ58PT,
                        SFZ58L1CLN,
                        SFZ58BSF,
                        SFZ5850000,
                        SFLOCATION,
                        SFZ58LDOCO,
                        SFZ58LNID,
                        SFZ58FLAG,
                        SFZ58LWSTN,
                        SFJFLG,
                        SFFLAG,
                        SFUSER,
                        SFUPMJ,
                        SFUPMT,
                        SFPID,
                        SFZ58RCP,
                        SFZ58RCC,
                        SFZ58IP01,
                        SFZ58GT,
                        SFZ58HST)
                        VALUES(?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,
                                ?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,
                                ?,?,?,?,?,?,?,?,?,?,?,?)"
            };
            cmd.Parameters.Add(new iDB2Parameter("SFUKID", iDB2DbType.iDB2Decimal, 15));
            cmd.Parameters["SFUKID"].Precision = 15;
            cmd.Parameters["SFUKID"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("SFEDUS", iDB2DbType.iDB2Graphic, 10));
            cmd.Parameters.Add(new iDB2Parameter("SFEDBT", iDB2DbType.iDB2Graphic, 15));
            cmd.Parameters.Add(new iDB2Parameter("SFEDTN", iDB2DbType.iDB2Graphic, 22));
            cmd.Parameters.Add(new iDB2Parameter("SFEDLN", iDB2DbType.iDB2Decimal, 7));
            cmd.Parameters["SFEDLN"].Precision = 7;
            cmd.Parameters["SFEDLN"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("SFEDCT", iDB2DbType.iDB2Graphic, 2));
            cmd.Parameters.Add(new iDB2Parameter("SFTYTN", iDB2DbType.iDB2Graphic, 8));
            cmd.Parameters.Add(new iDB2Parameter("SFEDFT", iDB2DbType.iDB2Graphic, 10));
            cmd.Parameters.Add(new iDB2Parameter("SFEDDT", iDB2DbType.iDB2Decimal, 6));
            cmd.Parameters["SFEDDT"].Precision = 6;
            cmd.Parameters["SFEDDT"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("SFDRIN", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("SFEDDL", iDB2DbType.iDB2Decimal, 5));
            cmd.Parameters["SFEDDL"].Precision = 5;
            cmd.Parameters["SFEDDL"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("SFEDSP", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("SFPNID", iDB2DbType.iDB2Graphic, 15));
            cmd.Parameters.Add(new iDB2Parameter("SFTNAC", iDB2DbType.iDB2Graphic, 2));
            cmd.Parameters.Add(new iDB2Parameter("SFMMCU", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("SFACDT", iDB2DbType.iDB2Decimal, 6));
            cmd.Parameters["SFACDT"].Precision = 6;
            cmd.Parameters["SFACDT"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("SFSTTM", iDB2DbType.iDB2Graphic, 24));
            cmd.Parameters.Add(new iDB2Parameter("SFDTMS", iDB2DbType.iDB2Graphic, 24));
            cmd.Parameters.Add(new iDB2Parameter("SFLITM", iDB2DbType.iDB2Graphic, 25));
            cmd.Parameters.Add(new iDB2Parameter("SFLOTN", iDB2DbType.iDB2Graphic, 30));
            cmd.Parameters.Add(new iDB2Parameter("SFZ58SN", iDB2DbType.iDB2Graphic, 30));
            cmd.Parameters.Add(new iDB2Parameter("SFDOCO", iDB2DbType.iDB2Decimal, 8));
            cmd.Parameters["SFDOCO"].Precision = 8;
            cmd.Parameters["SFDOCO"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("SFDCTO", iDB2DbType.iDB2Graphic, 2));
            cmd.Parameters.Add(new iDB2Parameter("SFMCU", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("SFZ58SCN", iDB2DbType.iDB2Decimal, 5));
            cmd.Parameters["SFZ58SCN"].Precision = 5;
            cmd.Parameters["SFZ58SCN"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("SFZ58SCSN", iDB2DbType.iDB2Decimal, 5));
            cmd.Parameters["SFZ58SCSN"].Precision = 5;
            cmd.Parameters["SFZ58SCSN"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("SFANP", iDB2DbType.iDB2Decimal, 8));
            cmd.Parameters["SFANP"].Precision = 8;
            cmd.Parameters["SFANP"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("SFANPA", iDB2DbType.iDB2Decimal, 8));
            cmd.Parameters["SFANPA"].Precision = 8;
            cmd.Parameters["SFANPA"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("SFSFT", iDB2DbType.iDB2Graphic, 5));
            cmd.Parameters.Add(new iDB2Parameter("SFZ58PCR", iDB2DbType.iDB2Graphic, 2));
            cmd.Parameters.Add(new iDB2Parameter("SFZ58RCD", iDB2DbType.iDB2Graphic, 2));
            cmd.Parameters.Add(new iDB2Parameter("SFUORG", iDB2DbType.iDB2Decimal, 15));
            cmd.Parameters["SFUORG"].Precision = 15;
            cmd.Parameters["SFUORG"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("SFSOCN", iDB2DbType.iDB2Decimal, 15));
            cmd.Parameters["SFSOCN"].Precision = 15;
            cmd.Parameters["SFSOCN"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("SFLOCN", iDB2DbType.iDB2Graphic, 20));
            cmd.Parameters.Add(new iDB2Parameter("SFZ58RID", iDB2DbType.iDB2Decimal, 15));
            cmd.Parameters["SFZ58RID"].Precision = 15;
            cmd.Parameters["SFZ58RID"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("SFZ58RT", iDB2DbType.iDB2Graphic, 5));
            cmd.Parameters.Add(new iDB2Parameter("SFZ58GWT", iDB2DbType.iDB2Decimal, 15));
            cmd.Parameters["SFZ58GWT"].Precision = 15;
            cmd.Parameters["SFZ58GWT"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("SFZ58GWTT", iDB2DbType.iDB2Decimal, 15));
            cmd.Parameters["SFZ58GWTT"].Precision = 15;
            cmd.Parameters["SFZ58GWTT"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("SFZ58IGLN", iDB2DbType.iDB2Graphic, 2));
            cmd.Parameters.Add(new iDB2Parameter("SFZ58UNID", iDB2DbType.iDB2Graphic, 6));
            cmd.Parameters.Add(new iDB2Parameter("SFZ58UID", iDB2DbType.iDB2Graphic, 14));
            cmd.Parameters.Add(new iDB2Parameter("SFTKNR", iDB2DbType.iDB2Graphic, 20));
            cmd.Parameters.Add(new iDB2Parameter("SFZ58PT", iDB2DbType.iDB2Graphic, 30));
            cmd.Parameters.Add(new iDB2Parameter("SFZ58L1CLN", iDB2DbType.iDB2Decimal, 1));
            cmd.Parameters["SFZ58L1CLN"].Precision = 1;
            cmd.Parameters["SFZ58L1CLN"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("SFZ58BSF", iDB2DbType.iDB2Decimal, 15));
            cmd.Parameters["SFZ58BSF"].Precision = 15;
            cmd.Parameters["SFZ58BSF"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("SFZ5850000", iDB2DbType.iDB2Decimal, 7));
            cmd.Parameters["SFZ5850000"].Precision = 7;
            cmd.Parameters["SFZ5850000"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("SFLOCATION", iDB2DbType.iDB2Graphic, 51));
            cmd.Parameters.Add(new iDB2Parameter("SFZ58LDOCO", iDB2DbType.iDB2Decimal, 8));
            cmd.Parameters["SFZ58LDOCO"].Precision = 8;
            cmd.Parameters["SFZ58LDOCO"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("SFZ58LNID", iDB2DbType.iDB2Decimal, 6));
            cmd.Parameters["SFZ58LNID"].Precision = 6;
            cmd.Parameters["SFZ58LNID"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("SFZ58FLAG", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("SFZ58LWSTN", iDB2DbType.iDB2Graphic, 5));
            cmd.Parameters.Add(new iDB2Parameter("SFJFLG", iDB2DbType.iDB2Graphic, 2));
            cmd.Parameters.Add(new iDB2Parameter("SFFLAG", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("SFUSER", iDB2DbType.iDB2Graphic, 10));
            cmd.Parameters.Add(new iDB2Parameter("SFUPMJ", iDB2DbType.iDB2Decimal, 6));
            cmd.Parameters["SFUPMJ"].Precision = 6;
            cmd.Parameters["SFUPMJ"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("SFUPMT", iDB2DbType.iDB2Decimal, 6));
            cmd.Parameters["SFUPMT"].Precision = 6;
            cmd.Parameters["SFUPMT"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("SFPID", iDB2DbType.iDB2Graphic, 10));
            cmd.Parameters.Add(new iDB2Parameter("SFZ58RCP", iDB2DbType.iDB2Graphic, 3));
            cmd.Parameters.Add(new iDB2Parameter("SFZ58RCC", iDB2DbType.iDB2Graphic, 25));
            cmd.Parameters.Add(new iDB2Parameter("SFZ58IP01", iDB2DbType.iDB2Graphic, 15));
            cmd.Parameters.Add(new iDB2Parameter("SFZ58GT", iDB2DbType.iDB2Graphic, 25));
            cmd.Parameters.Add(new iDB2Parameter("SFZ58HST", iDB2DbType.iDB2Graphic, 1));
            return cmd;
        }

        public decimal GetNextQAUKID(SqlConnection cnQAUKID)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info("BEGIN: " + DateTime.Now.ToLocalTime());
            long uniqueID = 0;
            decimal dUniqueID;
            try
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.Text,
                    CommandText = @"SELECT NEXT VALUE FOR [E1].[QAUKID] AS 'QAUKID'",
                    Connection = cnQAUKID
                };
                Logger.Trace("CN: " + cnQAUKID.ConnectionString);
                Logger.Trace("SQL: " + Environment.NewLine + cmd.CommandText);
                Logger.Trace("PARMS: " + string.Join(", ", cmd.Parameters.Cast<SqlParameter>().AsQueryable().Select(parm => parm.Value)));
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
                var counter = 0;
                while (reader.Read())
                {
                    uniqueID = CastDbNull.To<long>(reader["QAUKID"], 0);
                }
                reader.Close();
                dUniqueID = Convert.ToDecimal(uniqueID);
                Logger.Trace("ROWS: " + counter);
            }
            catch (Exception e)
            {
                throw new ApplicationException(cnQAUKID.ConnectionString + "  " + e);
            }
            Logger.Info("END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            return dUniqueID;
        }

        public void SuccessE1(GlassConsumptionResponse response)
        {
            try
            {
                response.Transferred = true;
                response.TransferStatus = "Y";
                foreach (var glassConsumption in response.GlassConsumptions.Where(glassConsumption => glassConsumption.IsValid))
                {
                    glassConsumption.Transferred = true;
                    glassConsumption.TransferStatus = "Y";
                    glassConsumption.TransferDate = response.TransferDate;
                }
            }
            catch (Exception e)
            {
                response.Errors = ErrorManager.AddError(response.Errors, e);
            }
        }

        public void FailE1(GlassConsumptionResponse response)
        {
            try
            {
                response.Transferred = false;
                response.TransferStatus = "N";
                foreach (var glassConsumption in response.GlassConsumptions.Where(glassConsumption => glassConsumption.IsValid))
                {
                    glassConsumption.Transferred = false;
                    glassConsumption.TransferStatus = "E";
                    glassConsumption.TransferDate = response.TransferDate;
                }
            }
            catch (Exception e)
            {
                response.Errors = ErrorManager.AddError(response.Errors, e);
            }
        }

        public bool InsertE1GlassConsumptionInterops(BusinessSystemSession e1Session, iDB2Transaction e1Trans, GlassConsumptionInterops glassConsumptionInterops)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info("BEGIN: " + DateTime.Now.ToLocalTime());
            var retval = false;
            var cmd = (e1Session.DbCommands.ContainsKey("GlassConsumptionInteropManager.InsertF580030")) ? e1Session.DbCommands["GlassConsumptionInteropManager.InsertF580030"] : InsertF580030();
            cmd.Connection = e1Session.DbConnection;
            cmd.Transaction = e1Trans;
            try
            {
                foreach (var glassConsumptionInterop in glassConsumptionInterops)
                {
                    cmd.Parameters["SFUKID"].Value = glassConsumptionInterop.SFUKID;
                    cmd.Parameters["SFEDUS"].Value = glassConsumptionInterop.SFEDUS;
                    cmd.Parameters["SFEDBT"].Value = glassConsumptionInterop.SFEDBT;
                    cmd.Parameters["SFEDTN"].Value = glassConsumptionInterop.SFEDTN;
                    cmd.Parameters["SFEDLN"].Value = glassConsumptionInterop.SFEDLN;
                    cmd.Parameters["SFEDCT"].Value = glassConsumptionInterop.SFEDCT;
                    cmd.Parameters["SFTYTN"].Value = glassConsumptionInterop.SFTYTN;
                    cmd.Parameters["SFEDFT"].Value = glassConsumptionInterop.SFEDFT;
                    cmd.Parameters["SFEDDT"].Value = glassConsumptionInterop.SFEDDT;
                    cmd.Parameters["SFDRIN"].Value = glassConsumptionInterop.SFDRIN;
                    cmd.Parameters["SFEDDL"].Value = glassConsumptionInterop.SFEDDL;
                    cmd.Parameters["SFEDSP"].Value = glassConsumptionInterop.SFEDSP;
                    cmd.Parameters["SFPNID"].Value = glassConsumptionInterop.SFPNID;
                    cmd.Parameters["SFTNAC"].Value = glassConsumptionInterop.SFTNAC;
                    cmd.Parameters["SFMMCU"].Value = glassConsumptionInterop.SFMMCU;
                    cmd.Parameters["SFACDT"].Value = glassConsumptionInterop.SFACDT;
                    cmd.Parameters["SFSTTM"].Value = glassConsumptionInterop.SFSTTM;
                    cmd.Parameters["SFDTMS"].Value = glassConsumptionInterop.SFDTMS;
                    cmd.Parameters["SFLITM"].Value = glassConsumptionInterop.SFLITM;
                    cmd.Parameters["SFLOTN"].Value = glassConsumptionInterop.SFLOTN;
                    cmd.Parameters["SFZ58SN"].Value = glassConsumptionInterop.SFZ58SN;
                    cmd.Parameters["SFDOCO"].Value = glassConsumptionInterop.SFDOCO;
                    cmd.Parameters["SFDCTO"].Value = glassConsumptionInterop.SFDCTO;
                    cmd.Parameters["SFMCU"].Value = glassConsumptionInterop.SFMCU;
                    cmd.Parameters["SFZ58SCN"].Value = glassConsumptionInterop.SFZ58SCN;
                    cmd.Parameters["SFZ58SCSN"].Value = glassConsumptionInterop.SFZ58SCSN;
                    cmd.Parameters["SFANP"].Value = glassConsumptionInterop.SFANP;
                    cmd.Parameters["SFANPA"].Value = glassConsumptionInterop.SFANPA;
                    cmd.Parameters["SFSFT"].Value = glassConsumptionInterop.SFSFT;
                    cmd.Parameters["SFZ58PCR"].Value = glassConsumptionInterop.SFZ58PCR;
                    cmd.Parameters["SFZ58RCD"].Value = glassConsumptionInterop.SFZ58RCD;
                    cmd.Parameters["SFUORG"].Value = glassConsumptionInterop.SFUORG;
                    cmd.Parameters["SFSOCN"].Value = glassConsumptionInterop.SFSOCN;
                    cmd.Parameters["SFLOCN"].Value = glassConsumptionInterop.SFLOCN;
                    cmd.Parameters["SFZ58RID"].Value = glassConsumptionInterop.SFZ58RID;
                    cmd.Parameters["SFZ58RT"].Value = glassConsumptionInterop.SFZ58RT;
                    cmd.Parameters["SFZ58GWT"].Value = glassConsumptionInterop.SFZ58GWT;
                    cmd.Parameters["SFZ58GWTT"].Value = glassConsumptionInterop.SFZ58GWTT;
                    cmd.Parameters["SFZ58IGLN"].Value = glassConsumptionInterop.SFZ58IGLN;
                    cmd.Parameters["SFZ58UNID"].Value = glassConsumptionInterop.SFZ58UNID;
                    cmd.Parameters["SFZ58UID"].Value = glassConsumptionInterop.SFZ58UID;
                    cmd.Parameters["SFTKNR"].Value = glassConsumptionInterop.SFTKNR;
                    cmd.Parameters["SFZ58PT"].Value = glassConsumptionInterop.SFZ58PT;
                    cmd.Parameters["SFZ58L1CLN"].Value = glassConsumptionInterop.SFZ58L1CLN;
                    cmd.Parameters["SFZ58BSF"].Value = glassConsumptionInterop.SFZ58BSF;
                    cmd.Parameters["SFZ5850000"].Value = glassConsumptionInterop.SFZ5850000;
                    cmd.Parameters["SFLOCATION"].Value = glassConsumptionInterop.SFLOCATION;
                    cmd.Parameters["SFZ58LDOCO"].Value = glassConsumptionInterop.SFZ58LDOCO;
                    cmd.Parameters["SFZ58LNID"].Value = glassConsumptionInterop.SFZ58LNID;
                    cmd.Parameters["SFZ58FLAG"].Value = glassConsumptionInterop.SFZ58FLAG;
                    cmd.Parameters["SFZ58LWSTN"].Value = glassConsumptionInterop.SFZ58LWSTN;
                    cmd.Parameters["SFJFLG"].Value = glassConsumptionInterop.SFJFLG;
                    cmd.Parameters["SFFLAG"].Value = glassConsumptionInterop.SFFLAG;
                    cmd.Parameters["SFUSER"].Value = glassConsumptionInterop.SFUSER;
                    cmd.Parameters["SFUPMJ"].Value = glassConsumptionInterop.SFUPMJ;
                    cmd.Parameters["SFUPMT"].Value = glassConsumptionInterop.SFUPMT;
                    cmd.Parameters["SFPID"].Value = glassConsumptionInterop.SFPID;
                    cmd.Parameters["SFZ58RCP"].Value = glassConsumptionInterop.SFZ58RCP;
                    cmd.Parameters["SFZ58RCC"].Value = glassConsumptionInterop.SFZ58RCC;
                    cmd.Parameters["SFZ58IP01"].Value = glassConsumptionInterop.SFZ58IP01;
                    cmd.Parameters["SFZ58GT"].Value = glassConsumptionInterop.SFZ58GT;
                    cmd.Parameters["SFZ58HST"].Value = glassConsumptionInterop.SFZ58HST;
                    var rows = cmd.ExecuteNonQuery();
                    Logger.Trace("ROWS: " + rows);
                }
                retval = true;
            }
            catch (Exception e)
            {
                throw new Exception("ERROR: " + e1Session.DbConnection.ConnectionString + " JobName: " + e1Session.DbConnection.JobName + "  " + e);
            }
            Logger.Info("END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            return retval;
        }

        public bool InsertGlassConsumptionTrigger(BusinessSystemSession e1Session, iDB2Transaction e1Trans, GlassConsumptionControlTrigger glassConsumptionControlTrigger)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info("BEGIN: " + DateTime.Now.ToLocalTime());
            var retval = false;
            var cmd = (e1Session.DbCommands.ContainsKey("GlassConsumptionInteropManager.InsertF580030A")) ? e1Session.DbCommands["GlassConsumptionInteropManager.InsertF580030A"] : InsertF580030A();
            cmd.Connection = e1Session.DbConnection;
            cmd.Transaction = e1Trans;
            try
            {
                cmd.Parameters["QAUKID"].Value = glassConsumptionControlTrigger.QAUKID;
                cmd.Parameters["QAEDSP"].Value = glassConsumptionControlTrigger.QAEDSP;
                cmd.Parameters["QAEDCT"].Value = glassConsumptionControlTrigger.QAEDCT;
                cmd.Parameters["QAMCU"].Value = glassConsumptionControlTrigger.QAMCU;
                cmd.Parameters["QAEDBT"].Value = glassConsumptionControlTrigger.QAEDBT;
                cmd.Parameters["QAZ58SCN"].Value = glassConsumptionControlTrigger.QAZ58SCN;
                cmd.Parameters["QACRTU"].Value = glassConsumptionControlTrigger.QACRTU;
                cmd.Parameters["QACPGM"].Value = glassConsumptionControlTrigger.QACPGM;
                cmd.Parameters["QAJOBN"].Value = glassConsumptionControlTrigger.QAJOBN;
                cmd.Parameters["QACRTDT"].Value = glassConsumptionControlTrigger.QACRTDT;
                cmd.Parameters["QACRTM"].Value = glassConsumptionControlTrigger.QACRTM;
                cmd.Parameters["QAUPMB"].Value = glassConsumptionControlTrigger.QAUPMB;
                cmd.Parameters["QAUPGM"].Value = glassConsumptionControlTrigger.QAUPGM;
                cmd.Parameters["QAMJOB"].Value = glassConsumptionControlTrigger.QAMJOB;
                Logger.Trace("CN: " + e1Session.DbConnection.ConnectionString);
                Logger.Trace("SQL: " + Environment.NewLine + cmd.CommandText);
                Logger.Trace("PARMS: " + string.Join(", ", cmd.Parameters.Cast<iDB2Parameter>().AsQueryable().Select(parm => parm.Value)));
                var rows = cmd.ExecuteNonQuery();
                Logger.Trace("ROWS: " + rows);
                retval = true;
            }
            catch (Exception e)
            {
                throw new Exception("ERROR: " + e1Session.DbConnection.ConnectionString + " JobName: " + e1Session.DbConnection.JobName + "  " + e);
            }
            Logger.Info("END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            return retval;
        }

        public GlassConsumptionInterops BuildGlassConsumptionInterops(GlassConsumptions glassConsumptions)
        {
            var glassConsumptionInterops = new GlassConsumptionInterops();
            try
            {
                var iTransactionNum = 0;
                foreach (var sfTag in glassConsumptions.Where(glassConsumption => glassConsumption.IsValid).Select(glassConsumption => new GlassConsumptionInterop
                {
                    SFUKID = glassConsumption.TransferUniqueID,
                    SFEDUS = "SVCSOA",
                    SFEDBT = Convert.ToString(glassConsumption.TransferBatchNum),
                    SFEDCT = "GS",
                    SFTYTN = "IGMTO",
                    SFEDFT = "IGGS",
                    SFEDDT = Utilities.Common.CastToE1Date(CastDbNull.To(glassConsumption.TransferDate, default(DateTime))),
                    SFDRIN = "1",
                    SFEDDL = 0,
                    SFEDSP = "",
                    SFPNID = "GlassStock",
                    SFTNAC = "02",
                    SFMMCU = glassConsumption.PlantNum.PadLeft(2, '0').PadLeft(12, ' '),
                    SFACDT = Utilities.Common.CastToE1Date(CastDbNull.To(glassConsumption.TransferDate, default(DateTime))),
                    SFSTTM = glassConsumption.ProductionDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    SFZ58SN = glassConsumption.RawCrateNumber,
                    SFSFT = glassConsumption.Shift,
                    SFUORG = glassConsumption.Quantity * 1000,
                    SFSOCN = glassConsumption.SheetsRejected,
                    SFZ58IGLN = glassConsumption.CuttingTable,
                    SFZ58UID = Convert.ToString(glassConsumption.ConsumptionId),
                    SFZ58PCR = glassConsumption.Crew,
                    SFMCU = glassConsumption.E1WorkCenter,
                    SFZ58LWSTN = glassConsumption.E1Operation,
                    SFDTMS = glassConsumption.ProductionDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    SFZ58GT = glassConsumption.RawCTCFCode,

                    SFUPMJ = Utilities.Common.CastToE1Date(CastDbNull.To(glassConsumption.TransferDate, default(DateTime))),
                    SFUPMT = Utilities.Common.CastToE1Time(CastDbNull.To(glassConsumption.TransferDate, default(DateTime))),
                }))
                {
                    iTransactionNum += 1;  // break this for key violation testing
                    var transactionNum = Convert.ToString(iTransactionNum);
                    sfTag.SFEDTN = transactionNum.PadLeft(10, '0');
                    glassConsumptionInterops.Add(sfTag);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
            return glassConsumptionInterops;
        }

        public GlassConsumptionControlTrigger BuildGlassConsumptionTrigger(string plantNum, long transferBatchNum, SqlConnection cnQAUKID = null)
        {
            try
            {
                var glassConsumptionControlTrigger = new GlassConsumptionControlTrigger();
                if (cnQAUKID != null)
                {
                    glassConsumptionControlTrigger.QAUKID = GetNextQAUKID(cnQAUKID);
                }
                glassConsumptionControlTrigger.QAEDSP = "";
                glassConsumptionControlTrigger.QAEDCT = "WC";
                glassConsumptionControlTrigger.QAMCU = plantNum.PadLeft(2, '0').PadLeft(12, ' ');
                glassConsumptionControlTrigger.QAEDBT = Convert.ToString(transferBatchNum);
                glassConsumptionControlTrigger.QAZ58SCN = 0;
                glassConsumptionControlTrigger.QACRTU = "SVCSOA";
                glassConsumptionControlTrigger.QACPGM = "SVCSOA";
                glassConsumptionControlTrigger.QAJOBN = "SVCSOA";
                glassConsumptionControlTrigger.QACRTDT = Utilities.Common.CastToE1Date(DateTime.Now.ToLocalTime());
                glassConsumptionControlTrigger.QACRTM = Utilities.Common.CastToE1Time(DateTime.Now.ToLocalTime());
                glassConsumptionControlTrigger.QAUPMB = "";
                glassConsumptionControlTrigger.QAUPGM = "";
                glassConsumptionControlTrigger.QAMJOB = "";
                glassConsumptionControlTrigger.QAUPMJ = 0;
                glassConsumptionControlTrigger.QAUPMT = 0;
                return glassConsumptionControlTrigger;
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }
    }
}
