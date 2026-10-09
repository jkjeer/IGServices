using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using IBM.Data.DB2.iSeries;
using IGServices.Common.Managers.Enterprise;
using IGServices.Common.Utilities;
using IGServices.Common.Types.E1;
using IGServices.Common.Types.ShopFloor;
using NLog;

namespace IGServices.Common.Managers.E1
{
    public class SpacerConsumptionInteropManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public SpacerConsumptionInteropManager(BusinessSystemSession e1Session)
        {
            var hash = new Dictionary<string, iDB2Command>
            {
                {"SpacerConsumptionInteropManager.GetExistingF580030", GetExistingF580030()},
                {"SpacerConsumptionInteropManager.InsertF580030", InsertF580030()},
                {"SpacerConsumptionInteropManager.InsertShopFloorF580030", InsertShopFloorF580030()},
                {"SpacerConsumptionInteropManager.InsertF580030A", InsertF580030A()}
            };
            foreach (var keyName in hash.Keys.Where(keyName => !e1Session.DbCommands.ContainsKey(keyName)))
            {
                e1Session.DbCommands.Add(keyName, hash[keyName]);
            }
        }

        /*********************************************************************************
         * Prepared Commands
         ********************************************************************************/
        public iDB2Command GetExistingF580030()
        {
            var cmd = new iDB2Command();
            try
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"
                    SELECT
                        SFUKID
		                ,SFEDBT
		                ,SFSTTM
		                ,SFZ58UID
                    FROM
                        F580030
                    WHERE
                        SFMMCU = ?
                        AND SFEDCT = ?
                        AND SFZ58UID = ? 
                    FOR READ ONLY";
                cmd.Parameters.Add(new iDB2Parameter("SFMMCU", iDB2DbType.iDB2Graphic, 12));
                cmd.Parameters.Add(new iDB2Parameter("SFEDCT", iDB2DbType.iDB2Graphic, 2));
                cmd.Parameters.Add(new iDB2Parameter("SFZ58UID", iDB2DbType.iDB2Decimal));
                cmd.Parameters["SFZ58UID"].Precision = 14;
                cmd.Parameters["SFZ58UID"].Scale = 0;
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
            return cmd;
        }

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

        public iDB2Command InsertShopFloorF580030()
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
                        SFZ58HST,
                        SFUM)
                        VALUES(?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,
                                ?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,
                                ?,?,?,?,?,?,?,?,?,?,?,?,?)"
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
            cmd.Parameters.Add(new iDB2Parameter("SFUM", iDB2DbType.iDB2Graphic, 2));
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

        public List<string> ValidateE1(BusinessSystemSession e1Session, SpacerConsumptions spacerConsumptions)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"BEGIN: {DateTime.Now.ToLocalTime()}");
            var validationErrors = new List<string>();
            foreach (var spacerConsumption in spacerConsumptions)
            {
                var validationMessage = IsExistingF580030(e1Session, spacerConsumption.PlantNum, spacerConsumption.ConsumptionAggregateId);
                if (validationMessage.Length > 0)
                {
                    spacerConsumption.IsValid = false;
                    spacerConsumption.Transferred = true;
                    spacerConsumption.TransferStatus = "Y";
                    spacerConsumption.ValidationStatus = "Previously Uploaded";
                    spacerConsumption.TransferMessage = "Previously Uploaded to E1 F580030";
                    validationErrors.Add(validationMessage);
                }
            }
            Logger.Info($"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            return validationErrors;
        }

        private static string IsExistingF580030(BusinessSystemSession e1Session, string plantNum, int consumptionAggregateId)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"BEGIN: {DateTime.Now.ToLocalTime()}");
            var validationMessage = "";
            try
            {
                var cmd = e1Session.DbCommands["SpacerConsumptionInteropManager.GetExistingF580030"];
                //var cmd = (e1Session.DbCommands.ContainsKey("SpacerConsumptionInteropManager.GetExistingF580030")) ? e1Session.DbCommands["SpacerConsumptionInteropManager.GetExistingF580030"] : GetExistingF580030();
                cmd.Parameters["SFMMCU"].Value = plantNum.PadLeft(12);
                cmd.Parameters["SFEDCT"].Value = "SC";
                cmd.Parameters["SFZ58UID"].Value = consumptionAggregateId;
                cmd.Connection = e1Session.DbConnection;
                Logger.Trace(Logging.DbCmdString(cmd));
                var rowCount = 0;
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
                {
                    while (reader.Read())
                    {
                        rowCount++;
                        // ReSharper disable InconsistentNaming
                        var SFUKID = CastDbNull.To<decimal>(reader["SFUKID"], 0);
                        var SFEDBT = CastDbNull.To<string>(reader["SFEDBT"], "");
                        var SFSTTM = CastDbNull.To<string>(reader["SFSTTM"], "");
                        //var SFZ58UID = CastDbNull.To(reader["SFZ58UID"], 0);
                        // ReSharper restore InconsistentNaming
                        validationMessage = "Previously Uploaded to E1 F580030 with SFZ58UID=" + consumptionAggregateId + " SFUKID=" + SFUKID + " SFEDBT=" + SFEDBT + " on " + SFSTTM;
                    }

                    reader.Close();
                }

                Logger.Trace($"ROWS: {rowCount}");
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
            return validationMessage;
        }

        public void InsertE1SpacerConsumptionInterops(BusinessSystemSession e1Session, iDB2Transaction e1Trans, SpacerConsumptionInterops spacerConsumptionInterops)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"BEGIN: {DateTime.Now.ToLocalTime()}");
            try
            {
                var cmd = (e1Session.DbCommands.ContainsKey("SpacerConsumptionInteropManager.InsertF580030")) ? e1Session.DbCommands["SpacerConsumptionInteropManager.InsertF580030"] : InsertF580030();
                cmd.Connection = e1Session.DbConnection;
                cmd.Transaction = e1Trans;
                var rowCount = 0;
                foreach (var spacerConsumptionInterop in spacerConsumptionInterops)
                {
                    cmd.Parameters["SFUKID"].Value = spacerConsumptionInterop.SFUKID;
                    cmd.Parameters["SFEDUS"].Value = spacerConsumptionInterop.SFEDUS;
                    cmd.Parameters["SFEDBT"].Value = spacerConsumptionInterop.SFEDBT;
                    cmd.Parameters["SFEDTN"].Value = spacerConsumptionInterop.SFEDTN;
                    cmd.Parameters["SFEDLN"].Value = spacerConsumptionInterop.SFEDLN;
                    cmd.Parameters["SFEDCT"].Value = spacerConsumptionInterop.SFEDCT;
                    cmd.Parameters["SFTYTN"].Value = spacerConsumptionInterop.SFTYTN;
                    cmd.Parameters["SFEDFT"].Value = spacerConsumptionInterop.SFEDFT;
                    cmd.Parameters["SFEDDT"].Value = spacerConsumptionInterop.SFEDDT;
                    cmd.Parameters["SFDRIN"].Value = spacerConsumptionInterop.SFDRIN;
                    cmd.Parameters["SFEDDL"].Value = spacerConsumptionInterop.SFEDDL;
                    cmd.Parameters["SFEDSP"].Value = spacerConsumptionInterop.SFEDSP;
                    cmd.Parameters["SFPNID"].Value = spacerConsumptionInterop.SFPNID;
                    cmd.Parameters["SFTNAC"].Value = spacerConsumptionInterop.SFTNAC;
                    cmd.Parameters["SFMMCU"].Value = spacerConsumptionInterop.SFMMCU;
                    cmd.Parameters["SFACDT"].Value = spacerConsumptionInterop.SFACDT;
                    cmd.Parameters["SFSTTM"].Value = spacerConsumptionInterop.SFSTTM;
                    cmd.Parameters["SFDTMS"].Value = spacerConsumptionInterop.SFDTMS;
                    cmd.Parameters["SFLITM"].Value = spacerConsumptionInterop.SFLITM;
                    cmd.Parameters["SFLOTN"].Value = spacerConsumptionInterop.SFLOTN;
                    cmd.Parameters["SFZ58SN"].Value = spacerConsumptionInterop.SFZ58SN;
                    cmd.Parameters["SFDOCO"].Value = spacerConsumptionInterop.SFDOCO;
                    cmd.Parameters["SFDCTO"].Value = spacerConsumptionInterop.SFDCTO;
                    cmd.Parameters["SFMCU"].Value = spacerConsumptionInterop.SFMCU;
                    cmd.Parameters["SFZ58SCN"].Value = spacerConsumptionInterop.SFZ58SCN;
                    cmd.Parameters["SFZ58SCSN"].Value = spacerConsumptionInterop.SFZ58SCSN;
                    cmd.Parameters["SFANP"].Value = spacerConsumptionInterop.SFANP;
                    cmd.Parameters["SFANPA"].Value = spacerConsumptionInterop.SFANPA;
                    cmd.Parameters["SFSFT"].Value = spacerConsumptionInterop.SFSFT;
                    cmd.Parameters["SFZ58PCR"].Value = spacerConsumptionInterop.SFZ58PCR;
                    cmd.Parameters["SFZ58RCD"].Value = spacerConsumptionInterop.SFZ58RCD;
                    cmd.Parameters["SFUORG"].Value = spacerConsumptionInterop.SFUORG;
                    cmd.Parameters["SFSOCN"].Value = spacerConsumptionInterop.SFSOCN;
                    cmd.Parameters["SFLOCN"].Value = spacerConsumptionInterop.SFLOCN;
                    cmd.Parameters["SFZ58RID"].Value = spacerConsumptionInterop.SFZ58RID;
                    cmd.Parameters["SFZ58RT"].Value = spacerConsumptionInterop.SFZ58RT;
                    cmd.Parameters["SFZ58GWT"].Value = spacerConsumptionInterop.SFZ58GWT;
                    cmd.Parameters["SFZ58GWTT"].Value = spacerConsumptionInterop.SFZ58GWTT;
                    cmd.Parameters["SFZ58IGLN"].Value = spacerConsumptionInterop.SFZ58IGLN;
                    cmd.Parameters["SFZ58UNID"].Value = spacerConsumptionInterop.SFZ58UNID;
                    cmd.Parameters["SFZ58UID"].Value = spacerConsumptionInterop.SFZ58UID;
                    cmd.Parameters["SFTKNR"].Value = spacerConsumptionInterop.SFTKNR;
                    cmd.Parameters["SFZ58PT"].Value = spacerConsumptionInterop.SFZ58PT;
                    cmd.Parameters["SFZ58L1CLN"].Value = spacerConsumptionInterop.SFZ58L1CLN;
                    cmd.Parameters["SFZ58BSF"].Value = spacerConsumptionInterop.SFZ58BSF;
                    cmd.Parameters["SFZ5850000"].Value = spacerConsumptionInterop.SFZ5850000;
                    cmd.Parameters["SFLOCATION"].Value = spacerConsumptionInterop.SFLOCATION;
                    cmd.Parameters["SFZ58LDOCO"].Value = spacerConsumptionInterop.SFZ58LDOCO;
                    cmd.Parameters["SFZ58LNID"].Value = spacerConsumptionInterop.SFZ58LNID;
                    cmd.Parameters["SFZ58FLAG"].Value = spacerConsumptionInterop.SFZ58FLAG;
                    cmd.Parameters["SFZ58LWSTN"].Value = spacerConsumptionInterop.SFZ58LWSTN;
                    cmd.Parameters["SFJFLG"].Value = spacerConsumptionInterop.SFJFLG;
                    cmd.Parameters["SFFLAG"].Value = spacerConsumptionInterop.SFFLAG;
                    cmd.Parameters["SFUSER"].Value = spacerConsumptionInterop.SFUSER;
                    cmd.Parameters["SFUPMJ"].Value = spacerConsumptionInterop.SFUPMJ;
                    cmd.Parameters["SFUPMT"].Value = spacerConsumptionInterop.SFUPMT;
                    cmd.Parameters["SFPID"].Value = spacerConsumptionInterop.SFPID;
                    cmd.Parameters["SFZ58RCP"].Value = spacerConsumptionInterop.SFZ58RCP;
                    cmd.Parameters["SFZ58RCC"].Value = spacerConsumptionInterop.SFZ58RCC;
                    cmd.Parameters["SFZ58IP01"].Value = spacerConsumptionInterop.SFZ58IP01;
                    cmd.Parameters["SFZ58GT"].Value = spacerConsumptionInterop.SFZ58GT;
                    cmd.Parameters["SFZ58HST"].Value = spacerConsumptionInterop.SFZ58HST;
                    Logger.Trace(Logging.DbCmdString(cmd));
                    rowCount += cmd.ExecuteNonQuery();
                    Logger.Trace($"ROWS: {rowCount}");
                }
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
        }

        public void InsertE1ShopFloorSpacerConsumptionInterops(BusinessSystemSession e1Session, iDB2Transaction e1Trans, SpacerConsumptionInterops spacerConsumptionInterops)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"BEGIN: {DateTime.Now.ToLocalTime()}");
            try
            {
                var cmd = e1Session.DbCommands["SpacerConsumptionInteropManager.InsertShopFloorF580030"];
                cmd.Connection = e1Session.DbConnection;
                cmd.Transaction = e1Trans;
                var rowCount = 0;
                foreach (var spacerConsumptionInterop in spacerConsumptionInterops)
                {
                    cmd.Parameters["SFUKID"].Value = spacerConsumptionInterop.SFUKID;
                    cmd.Parameters["SFEDUS"].Value = spacerConsumptionInterop.SFEDUS;
                    cmd.Parameters["SFEDBT"].Value = spacerConsumptionInterop.SFEDBT;
                    cmd.Parameters["SFEDTN"].Value = spacerConsumptionInterop.SFEDTN;
                    cmd.Parameters["SFEDLN"].Value = spacerConsumptionInterop.SFEDLN;
                    cmd.Parameters["SFEDCT"].Value = spacerConsumptionInterop.SFEDCT;
                    cmd.Parameters["SFTYTN"].Value = spacerConsumptionInterop.SFTYTN;
                    cmd.Parameters["SFEDFT"].Value = spacerConsumptionInterop.SFEDFT;
                    cmd.Parameters["SFEDDT"].Value = spacerConsumptionInterop.SFEDDT;
                    cmd.Parameters["SFDRIN"].Value = spacerConsumptionInterop.SFDRIN;
                    cmd.Parameters["SFEDDL"].Value = spacerConsumptionInterop.SFEDDL;
                    cmd.Parameters["SFEDSP"].Value = spacerConsumptionInterop.SFEDSP;
                    cmd.Parameters["SFPNID"].Value = spacerConsumptionInterop.SFPNID;
                    cmd.Parameters["SFTNAC"].Value = spacerConsumptionInterop.SFTNAC;
                    cmd.Parameters["SFMMCU"].Value = spacerConsumptionInterop.SFMMCU;
                    cmd.Parameters["SFACDT"].Value = spacerConsumptionInterop.SFACDT;
                    cmd.Parameters["SFSTTM"].Value = spacerConsumptionInterop.SFSTTM;
                    cmd.Parameters["SFDTMS"].Value = spacerConsumptionInterop.SFDTMS;
                    cmd.Parameters["SFLITM"].Value = spacerConsumptionInterop.SFLITM;
                    cmd.Parameters["SFLOTN"].Value = spacerConsumptionInterop.SFLOTN;
                    cmd.Parameters["SFZ58SN"].Value = spacerConsumptionInterop.SFZ58SN;
                    cmd.Parameters["SFDOCO"].Value = spacerConsumptionInterop.SFDOCO;
                    cmd.Parameters["SFDCTO"].Value = spacerConsumptionInterop.SFDCTO;
                    cmd.Parameters["SFMCU"].Value = spacerConsumptionInterop.SFMCU;
                    cmd.Parameters["SFZ58SCN"].Value = spacerConsumptionInterop.SFZ58SCN;
                    cmd.Parameters["SFZ58SCSN"].Value = spacerConsumptionInterop.SFZ58SCSN;
                    cmd.Parameters["SFANP"].Value = spacerConsumptionInterop.SFANP;
                    cmd.Parameters["SFANPA"].Value = spacerConsumptionInterop.SFANPA;
                    cmd.Parameters["SFSFT"].Value = spacerConsumptionInterop.SFSFT;
                    cmd.Parameters["SFZ58PCR"].Value = spacerConsumptionInterop.SFZ58PCR;
                    cmd.Parameters["SFZ58RCD"].Value = spacerConsumptionInterop.SFZ58RCD;
                    cmd.Parameters["SFUORG"].Value = spacerConsumptionInterop.SFUORG;
                    cmd.Parameters["SFSOCN"].Value = spacerConsumptionInterop.SFSOCN;
                    cmd.Parameters["SFLOCN"].Value = spacerConsumptionInterop.SFLOCN;
                    cmd.Parameters["SFZ58RID"].Value = spacerConsumptionInterop.SFZ58RID;
                    cmd.Parameters["SFZ58RT"].Value = spacerConsumptionInterop.SFZ58RT;
                    cmd.Parameters["SFZ58GWT"].Value = spacerConsumptionInterop.SFZ58GWT;
                    cmd.Parameters["SFZ58GWTT"].Value = spacerConsumptionInterop.SFZ58GWTT;
                    cmd.Parameters["SFZ58IGLN"].Value = spacerConsumptionInterop.SFZ58IGLN;
                    cmd.Parameters["SFZ58UNID"].Value = spacerConsumptionInterop.SFZ58UNID;
                    cmd.Parameters["SFZ58UID"].Value = spacerConsumptionInterop.SFZ58UID;
                    cmd.Parameters["SFTKNR"].Value = spacerConsumptionInterop.SFTKNR;
                    cmd.Parameters["SFZ58PT"].Value = spacerConsumptionInterop.SFZ58PT;
                    cmd.Parameters["SFZ58L1CLN"].Value = spacerConsumptionInterop.SFZ58L1CLN;
                    cmd.Parameters["SFZ58BSF"].Value = spacerConsumptionInterop.SFZ58BSF;
                    cmd.Parameters["SFZ5850000"].Value = spacerConsumptionInterop.SFZ5850000;
                    cmd.Parameters["SFLOCATION"].Value = spacerConsumptionInterop.SFLOCATION;
                    cmd.Parameters["SFZ58LDOCO"].Value = spacerConsumptionInterop.SFZ58LDOCO;
                    cmd.Parameters["SFZ58LNID"].Value = spacerConsumptionInterop.SFZ58LNID;
                    cmd.Parameters["SFZ58FLAG"].Value = spacerConsumptionInterop.SFZ58FLAG;
                    cmd.Parameters["SFZ58LWSTN"].Value = spacerConsumptionInterop.SFZ58LWSTN;
                    cmd.Parameters["SFJFLG"].Value = spacerConsumptionInterop.SFJFLG;
                    cmd.Parameters["SFFLAG"].Value = spacerConsumptionInterop.SFFLAG;
                    cmd.Parameters["SFUSER"].Value = spacerConsumptionInterop.SFUSER;
                    cmd.Parameters["SFUPMJ"].Value = spacerConsumptionInterop.SFUPMJ;
                    cmd.Parameters["SFUPMT"].Value = spacerConsumptionInterop.SFUPMT;
                    cmd.Parameters["SFPID"].Value = spacerConsumptionInterop.SFPID;
                    cmd.Parameters["SFZ58RCP"].Value = spacerConsumptionInterop.SFZ58RCP;
                    cmd.Parameters["SFZ58RCC"].Value = spacerConsumptionInterop.SFZ58RCC;
                    cmd.Parameters["SFZ58IP01"].Value = spacerConsumptionInterop.SFZ58IP01;
                    cmd.Parameters["SFZ58GT"].Value = spacerConsumptionInterop.SFZ58GT;
                    cmd.Parameters["SFZ58HST"].Value = spacerConsumptionInterop.SFZ58HST;
                    cmd.Parameters["SFUM"].Value = spacerConsumptionInterop.SFUM;
                    rowCount += cmd.ExecuteNonQuery();
                }
                Logger.Trace(Logging.DbCmdString(cmd));
                Logger.Trace($"ROWS: {rowCount}");
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
        }

        public void InsertSpacerConsumptionTrigger(BusinessSystemSession e1Session, iDB2Transaction e1Trans, SpacerConsumptionControlTrigger spacerConsumptionControlTrigger)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"BEGIN: {DateTime.Now.ToLocalTime()}");
            try
            {
                var cmd = InsertF580030A();
                cmd.Connection = e1Session.DbConnection;
                cmd.Transaction = e1Trans;
                cmd.Parameters["QAUKID"].Value = spacerConsumptionControlTrigger.QAUKID;
                cmd.Parameters["QAEDSP"].Value = spacerConsumptionControlTrigger.QAEDSP;
                cmd.Parameters["QAEDCT"].Value = spacerConsumptionControlTrigger.QAEDCT;
                cmd.Parameters["QAMCU"].Value = spacerConsumptionControlTrigger.QAMCU;
                cmd.Parameters["QAEDBT"].Value = spacerConsumptionControlTrigger.QAEDBT;
                cmd.Parameters["QAZ58SCN"].Value = spacerConsumptionControlTrigger.QAZ58SCN;
                cmd.Parameters["QACRTU"].Value = spacerConsumptionControlTrigger.QACRTU;
                cmd.Parameters["QACPGM"].Value = spacerConsumptionControlTrigger.QACPGM;
                cmd.Parameters["QAJOBN"].Value = spacerConsumptionControlTrigger.QAJOBN;
                cmd.Parameters["QACRTDT"].Value = spacerConsumptionControlTrigger.QACRTDT;
                cmd.Parameters["QACRTM"].Value = spacerConsumptionControlTrigger.QACRTM;
                cmd.Parameters["QAUPMB"].Value = spacerConsumptionControlTrigger.QAUPMB;
                cmd.Parameters["QAUPGM"].Value = spacerConsumptionControlTrigger.QAUPGM;
                cmd.Parameters["QAMJOB"].Value = spacerConsumptionControlTrigger.QAMJOB;
                Logger.Trace(Logging.DbCmdString(cmd));
                var rowCount = cmd.ExecuteNonQuery();
                Logger.Trace($"ROWS: {rowCount}");
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
        }

        public SpacerConsumptionInterops BuildSpacerConsumptionInterops(SpacerConsumptions spacerConsumptions)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"BEGIN: {DateTime.Now.ToLocalTime()}");
            var spacerConsumptionInterops = new SpacerConsumptionInterops();
            try
            {
                var iTransactionNum = 0;
                foreach (var sfTag in spacerConsumptions.Where(spacerConsumption => spacerConsumption.IsValid).Select(spacerConsumption => new SpacerConsumptionInterop
                {
                    SFUKID = spacerConsumption.TransferUniqueID,
                    SFEDUS = "SVCSOA",
                    SFEDBT = Convert.ToString(spacerConsumption.TransferBatchNum),
                    SFEDCT = "SC",
                    SFTYTN = "IGMTO",
                    SFEDFT = "IGSC",
                    SFEDDT = Utilities.Common.CastToE1Date(CastDbNull.To(spacerConsumption.TransferDate, default(DateTime))),
                    SFDRIN = "1",
                    SFEDDL = 0,
                    SFEDSP = "",
                    SFPNID = "SpacerConsumption",
                    SFMMCU = spacerConsumption.PlantNum.PadLeft(2, '0').PadLeft(12, ' '),
                    SFACDT = Utilities.Common.CastToE1Date(CastDbNull.To(spacerConsumption.TransferDate, default(DateTime))),
                    SFSTTM = spacerConsumption.MinWrittenDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    SFDTMS = spacerConsumption.ProductionDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    SFLITM = spacerConsumption.ItemNum,
                    SFUORG = spacerConsumption.Quantity * 1000,
                    SFSOCN = 0,
                    SFZ58UID = Convert.ToString(spacerConsumption.ConsumptionAggregateId),
                    SFUPMJ = Utilities.Common.CastToE1Date(CastDbNull.To(spacerConsumption.MinWrittenDate, default(DateTime))),
                    SFUPMT = Utilities.Common.CastToE1Time(CastDbNull.To(spacerConsumption.TransferDate, default(DateTime)))
                }))
                {
                    iTransactionNum++;  // break this for key violation testing
                    var transactionNum = Convert.ToString(iTransactionNum);
                    sfTag.SFEDTN = transactionNum.PadLeft(10, '0');
                    spacerConsumptionInterops.Add(sfTag);
                }
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
            return spacerConsumptionInterops;
        }

        public SpacerConsumptionInterops BuildShopFloorSpacerConsumptionInterops(SpacerConsumptions spacerConsumptions)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"BEGIN: {DateTime.Now.ToLocalTime()}");
            var spacerConsumptionInterops = new SpacerConsumptionInterops();
            try
            {
                var iTransactionNum = 0;
                foreach (var sfTag in spacerConsumptions.Where(spacerConsumption => spacerConsumption.IsValid).Select(spacerConsumption => new SpacerConsumptionInterop
                {
                    SFUKID = spacerConsumption.TransferUniqueID,
                    SFEDUS = "SVCSOA",
                    SFEDBT = Convert.ToString(spacerConsumption.TransferBatchNum),
                    SFEDCT = "SC",
                    SFTYTN = "IGMTO",
                    SFEDFT = "IGSC",
                    SFEDDT = Utilities.Common.CastToE1Date(CastDbNull.To(spacerConsumption.TransferDate, default(DateTime))),
                    SFDRIN = "1",
                    SFEDDL = 0,
                    SFEDSP = "",
                    SFPNID = "SpacerConsumption",
                    SFMMCU = spacerConsumption.PlantNum.PadLeft(2, '0').PadLeft(12, ' '),
                    SFACDT = Utilities.Common.CastToE1Date(CastDbNull.To(spacerConsumption.TransferDate, default(DateTime))),
                    SFSTTM = spacerConsumption.MinWrittenDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    SFDTMS = spacerConsumption.ProductionDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    SFLITM = spacerConsumption.ItemNum,
                    SFUORG = spacerConsumption.Quantity * 1000,
                    SFSOCN = 0,
                    SFZ58UID = Convert.ToString(spacerConsumption.ConsumptionAggregateId),
                    SFUPMJ = Utilities.Common.CastToE1Date(CastDbNull.To(spacerConsumption.MinWrittenDate, default(DateTime))),
                    SFUPMT = Utilities.Common.CastToE1Time(CastDbNull.To(spacerConsumption.TransferDate, default(DateTime))),
                    SFUM = spacerConsumption.UnitOfMeasure
                }))
                {
                    iTransactionNum++;  // break this for key violation testing
                    var transactionNum = Convert.ToString(iTransactionNum);
                    sfTag.SFEDTN = transactionNum.PadLeft(10, '0');
                    spacerConsumptionInterops.Add(sfTag);
                }
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
            return spacerConsumptionInterops;
        }

        public SpacerConsumptionControlTrigger BuildSpacerConsumptionTrigger(string plantNum, long transferBatchNum, SqlConnection cnQAUKID = null)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"BEGIN: {DateTime.Now.ToLocalTime()}");
            var spacerConsumptionControlTrigger = new SpacerConsumptionControlTrigger();
            try
            {
                if (cnQAUKID != null)
                {
                    spacerConsumptionControlTrigger.QAUKID = BatchNumManager.GetNextQAUKID(cnQAUKID);
                }
                spacerConsumptionControlTrigger.QAEDSP = "";
                spacerConsumptionControlTrigger.QAEDCT = "WC";
                spacerConsumptionControlTrigger.QAMCU = plantNum.PadLeft(2, '0').PadLeft(12, ' ');
                spacerConsumptionControlTrigger.QAEDBT = Convert.ToString(transferBatchNum);
                spacerConsumptionControlTrigger.QAZ58SCN = 0;
                spacerConsumptionControlTrigger.QACRTU = "SVCSOA";
                spacerConsumptionControlTrigger.QACPGM = "SVCSOA";
                spacerConsumptionControlTrigger.QAJOBN = "SVCSOA";
                spacerConsumptionControlTrigger.QACRTDT = Utilities.Common.CastToE1Date(DateTime.Now.ToLocalTime());
                spacerConsumptionControlTrigger.QACRTM = Utilities.Common.CastToE1Time(DateTime.Now.ToLocalTime());
                spacerConsumptionControlTrigger.QAUPMB = "";
                spacerConsumptionControlTrigger.QAUPGM = "";
                spacerConsumptionControlTrigger.QAMJOB = "";
                spacerConsumptionControlTrigger.QAUPMJ = 0;
                spacerConsumptionControlTrigger.QAUPMT = 0;
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {cnQAUKID.ConnectionString}  {e.Message}: {e.StackTrace}");
                throw new ApplicationException($"{cnQAUKID.ConnectionString}  {e.Message}: {e.StackTrace}");
            }
            finally
            {
                Logger.Info($"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }
            return spacerConsumptionControlTrigger;
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
