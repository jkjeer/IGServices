using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;
using IBM.Data.DB2.iSeries;
using IGServices.Common.Types.E1;
using IGServices.Common.Types.Enterprise;
using IGServices.Common.Utilities;
using NLog;

namespace IGServices.Common.Managers.Enterprise
{


    public class RackTagItemManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private GlassItemManager _glassItemMgr;
        private E1.ItemAttributeManager _itemAttributeMgr;
        private MuntinBarCodeManager _muntinBarCodeManager;

        /*********************************************************
        * INIT
        *********************************************************/
        public RackTagItemManager(BusinessSystemSession e1Session)
        {
            var hash = new Dictionary<string, iDB2Command>();
            hash.Add("GetRackTagItemsByOrder", GetRackTagItemsByOrder());
            hash.Add("GetRackTagItemsByUnitId", GetRackTagItemsByUnitId());
            hash.Add("GetRackFooterMessages", GetRackFooterMessages());
            hash.Add("GetEdiMessages", GetEdiMessages());
            foreach (string keyName in hash.Keys)
            {
                if (!e1Session.DbCommands.ContainsKey(keyName))
                {
                    e1Session.DbCommands.Add(keyName, hash[keyName]);
                }
            }
            _glassItemMgr = new GlassItemManager(e1Session);
            _itemAttributeMgr = new E1.ItemAttributeManager(e1Session);
            _muntinBarCodeManager = new MuntinBarCodeManager(e1Session);
        }




        /*********************************************************
        * SQL
        *********************************************************/
        public iDB2Command GetRackTagItemsByUnitId()
        {
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = String.Format(
                @"SELECT
                *
                FROM (
                    SELECT
                    RHZ58RID,
                    RHMCU,
                    RHAN8,
                    RHZ58HST,
                    RHZ58RSDT,
                    AB1.ABALPH AS AN8ALPH,
                    RHSHAN,
                    AB2.ABALPH AS SHANALPH,
                    RHZ58RT,
                    UD1.DRDL01 AS RHZ58RT_DESC,
                    RHZ58BOX,
                    RHZ58GWTT,
                    RHZ58RTWT,
                    RHZ58SCN,
                    RHLOCN,
                    RDZ58SN AS MIN_RDZ58SN,
                    RDZ58SN AS MAX_RDZ58SN,
					RDZ58UID,
                    RDITM,
                    RDKCOO,
                    RDDOCO,
                    RDDCTO,
                    RDLNID,
                    RDDOC1,
                    RDXDCTO,
                    RDDOC2,
                    RDXCTO,
                    RDPKCO,
                    RDLNIC,
                    RDZ58SD,
                    RDZ58ROW,
                    RDZ58STK,
                    RDZ58BXRO,
                    RDZ58PS,
                    RDZ58SL,
                    1 AS RDPQOH,
                    ATZ5899003,
                    ATZ58CP2,
                    ATZ58CP3,
                    ATZ58CP4,
                    ATZ58CSQNO,
                    ATZ58GSQNO,
                    ATZ5899009,
                    ATZ5899010,
                    ATZ5899011,
                    ATZ5899012,
                    SHDRQJ,
                    SDDRQJ,
                    SDUORG,
                    SDVR01,
                    S1Z58SCN,
                    S1Z58SCDDJ,
                    COALESCE(S1Z58SCTYP, '') AS S1Z58SCTYP,
                    COALESCE(BIZ58IGLN, '0') AS BIZ58IGLN,
                    ROW_NUMBER() OVER(PARTITION BY RHZ58RID, RDLOTN, RDZ58SN ORDER BY S5UPMJ DESC, S5TDAY DESC) AS RN
                FROM
                    F580055
                    LEFT OUTER JOIN F0101 AB1
                    ON AB1.ABAN8 = RHAN8
                    LEFT OUTER JOIN F0101 AB2
                    ON  AB2.ABAN8 = RHSHAN
                    LEFT OUTER JOIN F580056
                    ON RDZ58RID = RHZ58RID
                    LEFT OUTER JOIN F4201
                    ON  SHDOCO = RDDOCO AND
                        SHDCTO = RDDCTO AND
                        SHKCOO = RDKCOO
                    LEFT OUTER JOIN F4211
                    ON  SDDOCO = RDDOCO AND
                        SDLNID = RDLNID AND
                        SDDCTO = RDDCTO AND
                        SDKCOO = RDKCOO
                    LEFT OUTER JOIN F580032
                    ON  ATDOCO = RDDOCO AND
                        ATLNID = RDLNID AND
                        ATDCTO = RDDCTO AND
			            ATKCOO = RDKCOO AND
                        ATDOCO > 0
                    LEFT OUTER JOIN F5834A11
                    ON  S5MCU = RDMCU AND
                        S5DOCO = RDDOC1
                    LEFT OUTER JOIN F5834A01
                    ON S1MCU = S5MCU AND
                        S1Z58SCN = S5Z58SCN AND
                        S1Z58SCD = S5Z58SCD
                    LEFT OUTER JOIN F580200
                    ON BIMCU = RDMCU AND
                        BIDOCO = RDDOC1 AND
                        BIZ58UNID = RDZ58SN
                    LEFT OUTER JOIN {0}.F0005 UD1
                        ON UD1.DRSY = '57' AND
                        UD1.DRRT = 'RK' AND
                        TRIM(UD1.DRKY) = TRIM(RHZ58RT)
                    WHERE
                        RHZ58RID = ?) A
                WHERE 
                    A.RN = 1
                  ORDER BY
                    RDZ58SD,
                    RDZ58ROW,
                    RDZ58STK,
                    RDZ58SL,
                    RDZ58PS", ConfigurationManager.AppSettings["ControlLib"]);
            cmd.Parameters.Add(new iDB2Parameter("RackId", iDB2DbType.iDB2Decimal));
            cmd.Parameters["RackId"].Precision = 8;
            cmd.Parameters["RackId"].Scale = 0;
            return cmd;
        }

        public iDB2Command GetRackTagItemsByUnitId0()
        {
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = String.Format(
            @"SELECT
                RHZ58RID,
                RHMCU,
                RHAN8,
                RHZ58HST,
                RHZ58RSDT,
                AB1.ABALPH AS AN8ALPH,
                RHSHAN,
                AB2.ABALPH AS SHANALPH,  
                RHZ58RT,
                UD1.DRDL01 AS RHZ58RT_DESC,
                RHZ58BOX,
                RHZ58GWTT,
                RHZ58RTWT,
                RHZ58SCN,
                RHLOCN,
                RDZ58SN AS MIN_RDZ58SN,
                RDZ58SN AS MAX_RDZ58SN,
					RDZ58UID,
                RDITM,
                RDKCOO,
                RDDOCO,
                RDDCTO,
                RDLNID,
                RDDOC1,
                RDXDCTO,
                RDDOC2,
                RDXCTO,
                RDPKCO,
                RDLNIC,
                RDZ58SD,
                RDZ58ROW,
                RDZ58STK,
                RDZ58BXRO,
                RDZ58PS,
                RDZ58SL,
                1 AS RDPQOH,  
                ATZ5899003,
                ATZ58CP2,
                ATZ58CP3,
                ATZ58CP4,
                ATZ58CSQNO,
                ATZ58GSQNO,
                ATZ5899009,
                ATZ5899010,
                ATZ5899011,
                ATZ5899012,
                SHDRQJ,
                SDDRQJ,
                SDUORG,
                SDVR01,
                S1Z58SCDDJ,
                COALESCE(S1Z58SCTYP, '') AS S1Z58SCTYP,
                COALESCE(BIZ58IGLN, '0') AS BIZ58IGLN         
            FROM
                F580055
                LEFT OUTER JOIN F0101 AB1
                ON  AB1.ABAN8 = RHAN8
                LEFT OUTER JOIN F0101 AB2
                ON  AB2.ABAN8 = RHSHAN
                LEFT OUTER JOIN F580056
                ON RDZ58RID = RHZ58RID
                LEFT OUTER JOIN F4201
                ON  SHDOCO = RDDOCO AND
                    SHDCTO = RDDCTO AND
                    SHKCOO = RDKCOO
                LEFT OUTER JOIN F4211
                ON  SDDOCO = RDDOCO AND
                    SDLNID = RDLNID AND
                    SDDCTO = RDDCTO AND
                    SDKCOO = RDKCOO
                LEFT OUTER JOIN F580032
                ON  ATDOCO = RDDOCO AND
                    ATLNID = RDLNID AND
                    ATDCTO = RDDCTO AND
			            ATKCOO = RDKCOO AND
                        ATDOCO > 0
                LEFT OUTER JOIN F5834A11
                ON  S5MCU = RDMCU AND
                    S5DOCO = RDDOC1
                LEFT OUTER JOIN F5834A01
                ON  S1MCU = S5MCU AND
                    S1Z58SCN = S5Z58SCN AND
                    S1Z58SCD = S5Z58SCD
                LEFT OUTER JOIN F580200 
                ON  BIMCU = RDMCU AND
                    BIDOCO = RDDOC1 AND
                    BIZ58UNID = RDZ58SN
                LEFT OUTER JOIN {0}.F0005 UD1
                ON  UD1.DRSY = '57' AND
                    UD1.DRRT = 'RK' AND
                    TRIM(UD1.DRKY) = TRIM(RHZ58RT)
            WHERE
                RHZ58RID = ?
            ORDER BY
                RDZ58SD,
                RDZ58ROW,
                RDZ58STK,
                RDZ58SL,
                RDZ58PS", ConfigurationManager.AppSettings["ControlLib"]);
            cmd.Parameters.Add(new iDB2Parameter("RackId", iDB2DbType.iDB2Decimal));
            cmd.Parameters["RackId"].Precision = 8;
            cmd.Parameters["RackId"].Scale = 0;
            return cmd;
        }

        public iDB2Command GetRackTagItemsByOrder()
        {
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = String.Format(@"
            SELECT
	            RHZ58RID,
	            RHMCU,
	            RHAN8,
	            RHZ58HST,
	            RHZ58RSDT,
	            AN8ALPH,
	            RHSHAN,
	            SHANALPH,  
	            RHZ58RT,
	            RHZ58RT_DESC,
	            RHZ58BOX,
	            RHZ58GWTT,
	            RHZ58RTWT,
	            RHZ58SCN,
	            RHLOCN,
	            MIN(RDZ58SN) AS MIN_RDZ58SN,
	            MAX(RDZ58SN) AS MAX_RDZ58SN,
					MAX(RDZ58UID) AS RDZ58UID,
	            MAX(RDITM) AS RDITM,
	            RDKCOO,
	            RDDOCO,
	            RDDCTO,
	            RDLNID,
	            RDDOC1,
	            RDXDCTO,
	            RDDOC2,
	            RDXCTO,
	            RDPKCO,
	            RDLNIC,
	            RDZ58SD,
	            RDZ58ROW,
	            RDZ58STK,
	            MAX(RDZ58BXRO) AS RDZ58BXRO,
	            MAX(RDZ58PS) AS RDZ58PS,
	            MAX(RDZ58SL) AS RDZ58SL,
	            SUM(RDPQOH) AS RDPQOH,
	            ATZ5899003,
	            ATZ58CP2,
	            ATZ58CP3,
	            ATZ58CP4,
	            ATZ58CSQNO,
	            ATZ58GSQNO,
	            ATZ5899009,
	            ATZ5899010,
	            ATZ5899011,
	            ATZ5899012,
	            SHDRQJ,
	            SDDRQJ,
	            SDUORG,
	            SDVR01,
	            MAX(S1Z58SCDDJ) AS S1Z58SCDDJ,
	            MAX(S1Z58SCTYP) AS S1Z58SCTYP,
	            BIZ58IGLN   
            FROM (
	            SELECT
		            RHZ58RID,
		            RHMCU,
		            RHAN8,
		            RHZ58HST,
		            RHZ58RSDT,
		            AB1.ABALPH AS AN8ALPH,
		            RHSHAN,
		            AB2.ABALPH AS SHANALPH,
		            RHZ58RT,
		            UD1.DRDL01 AS RHZ58RT_DESC,
		            RHZ58BOX,
		            RHZ58GWTT,
		            RHZ58RTWT,
		            RHZ58SCN,
		            RHLOCN,
		            RDZ58SN,
					RDZ58UID,
		            RDITM,
		            RDKCOO,
		            RDDOCO,
		            RDDCTO,
		            RDLNID,
		            RDDOC1,
		            RDXDCTO,
		            RDDOC2,
		            RDXCTO,
		            RDPKCO,
		            RDLNIC,
		            RDZ58SD,
		            RDZ58ROW,
		            RDZ58STK,
		            RDZ58BXRO,
		            RDZ58PS,
		            RDZ58SL,
		            1 AS RDPQOH,
		            ATZ5899003,
		            ATZ58CP2,
		            ATZ58CP3,
		            ATZ58CP4,
		            ATZ58CSQNO,
		            ATZ58GSQNO,
		            ATZ5899009,
		            ATZ5899010,
		            ATZ5899011,
		            ATZ5899012,
		            SHDRQJ,
		            SDDRQJ,
		            SDUORG,
		            SDVR01,
		            S1Z58SCN,
		            S1Z58SCDDJ,
		            COALESCE(S1Z58SCTYP, '') AS S1Z58SCTYP,
		            COALESCE(BIZ58IGLN, '0') AS BIZ58IGLN,
		            ROW_NUMBER() OVER(PARTITION BY RHZ58RID, RDLOTN, RDZ58SN ORDER BY S5UPMJ DESC, S5TDAY DESC) AS RN
	            FROM
		            F580055
		            LEFT OUTER JOIN F0101 AB1
		            ON AB1.ABAN8 = RHAN8
		            LEFT OUTER JOIN F0101 AB2
		            ON  AB2.ABAN8 = RHSHAN
		            LEFT OUTER JOIN F580056
		            ON RDZ58RID = RHZ58RID
		            LEFT OUTER JOIN F4201
		            ON  SHDOCO = RDDOCO AND
		            SHDCTO = RDDCTO AND
		            SHKCOO = RDKCOO
		            LEFT OUTER JOIN F4211
		            ON  SDDOCO = RDDOCO AND
			            SDLNID = RDLNID AND
			            SDDCTO = RDDCTO AND
			            SDKCOO = RDKCOO
		            LEFT OUTER JOIN F580032
		            ON  ATDOCO = RDDOCO AND
			            ATLNID = RDLNID AND
			            ATDCTO = RDDCTO AND
			            ATKCOO = RDKCOO AND
                        ATDOCO > 0
		            LEFT OUTER JOIN F5834A11
		            ON  S5MCU = RDMCU AND
			            S5DOCO = RDDOC1
		            LEFT OUTER JOIN F5834A01
		            ON S1MCU = S5MCU AND
			            S1Z58SCN = S5Z58SCN AND
			            S1Z58SCD = S5Z58SCD
		            LEFT OUTER JOIN F580200
		            ON BIMCU = RDMCU AND
			            BIDOCO = RDDOC1 AND
			            BIZ58UNID = RDZ58SN
		            LEFT OUTER JOIN {0}.F0005 UD1
			            ON UD1.DRSY = '57' AND
			            UD1.DRRT = 'RK' AND
			            TRIM(UD1.DRKY) = TRIM(RHZ58RT)
	            WHERE
		            RHZ58RID = ?) A
            WHERE 
	            A.RN = 1
            GROUP BY
	            RHZ58RID,
	            RHMCU,
	            RHAN8,
	            RHZ58HST,
	            RHZ58RSDT,
	            AN8ALPH,
	            RHSHAN,
	            SHANALPH,  
	            RHZ58RT,
	            RHZ58RT_DESC,
	            RHZ58BOX,
	            RHZ58GWTT,
	            RHZ58RTWT,
	            RHZ58SCN,
	            RHLOCN,
	            RDKCOO,
	            RDDOCO,
	            RDDCTO,
	            RDLNID,
	            RDKCOO,
	            RDDOC1,
	            RDXDCTO,
	            RDPKCO,
	            RDDOC2,
	            RDXCTO,
	            RDLNIC,
	            RDZ58SD,
	            RDZ58ROW,
	            RDZ58STK,
	            ATZ5899003,
	            ATZ58CP2,
	            ATZ58CP3,
	            ATZ58CP4,
	            ATZ58CSQNO,
	            ATZ58GSQNO,
	            ATZ5899009,
	            ATZ5899010,
	            ATZ5899011,
	            ATZ5899012,
	            SHDRQJ,
	            SDDRQJ,
	            SDUORG,
	            SDVR01,
	            BIZ58IGLN
            ORDER BY
                RDZ58SD,
                RDZ58ROW,
                RDZ58STK,
                RDZ58SL,
                RDZ58PS", ConfigurationManager.AppSettings["ControlLib"]);
            cmd.Parameters.Add(new iDB2Parameter("RackId", iDB2DbType.iDB2Decimal));
            cmd.Parameters["RackId"].Precision = 8;
            cmd.Parameters["RackId"].Scale = 0;
            return cmd;
        }


        public iDB2Command GetRackTagItemsByOrder0()
        {
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = String.Format(@"
            SELECT
                RHZ58RID,
                RHMCU,
                RHAN8,
                RHZ58HST,
                RHZ58RSDT,
                AB1.ABALPH AS AN8ALPH,
                RHSHAN,
                AB2.ABALPH AS SHANALPH,  
                RHZ58RT,
                UD1.DRDL01 AS RHZ58RT_DESC,
                RHZ58BOX,
                RHZ58GWTT,
                RHZ58RTWT,
                RHZ58SCN,
                RHLOCN,
                MIN(RDZ58SN) AS MIN_RDZ58SN,
                MAX(RDZ58SN) AS MAX_RDZ58SN,
                MAX(RDITM) AS RDITM,
                RDKCOO,
                RDDOCO,
                RDDCTO,
                RDLNID,
                RDDOC1,
                RDXDCTO,
                RDDOC2,
                RDXCTO,
                RDPKCO,
                RDLNIC,
                RDZ58SD,
                RDZ58ROW,
                RDZ58STK,
                MAX(RDZ58BXRO) AS RDZ58BXRO,
                MAX(RDZ58PS) AS RDZ58PS,
                MAX(RDZ58SL) AS RDZ58SL,
                COUNT(*) AS RDPQOH,
                ATZ5899003,
                ATZ58CP2,
                ATZ58CP3,
                ATZ58CP4,
                ATZ58CSQNO,
                ATZ58GSQNO,
                ATZ5899009,
                ATZ5899010,
                ATZ5899011,
                ATZ5899012,
                SHDRQJ,
                SDDRQJ,
                SDUORG,
                SDVR01,
                S1Z58SCDDJ,
                COALESCE(S1Z58SCTYP, '') AS S1Z58SCTYP,
                COALESCE(BIZ58IGLN, '0') AS BIZ58IGLN   
            FROM
                F580055
                INNER JOIN F0101 AB1
                ON  AB1.ABAN8 = RHAN8
                INNER JOIN F0101 AB2
                ON  AB2.ABAN8 = RHSHAN
                INNER JOIN F580056
                ON RDZ58RID = RHZ58RID
                INNER JOIN F4201
                ON  SHDOCO = RDDOCO AND
                    SHDCTO = RDDCTO AND
                    SHKCOO = RDKCOO
                INNER JOIN F4211
                ON  SDDOCO = RDDOCO AND
                    SDLNID = RDLNID AND
                    SDDCTO = RDDCTO AND
                    SDKCOO = RDKCOO
                INNER JOIN F580032
                ON  ATDOCO = RDDOCO AND
                    ATLNID = RDLNID AND
                    ATDCTO = RDDCTO AND
			            ATKCOO = RDKCOO AND
                        ATDOCO > 0
                LEFT OUTER JOIN F5834A11
                ON  S5MCU = RDMCU AND
                    S5DOCO = RDDOC1
                LEFT OUTER JOIN F5834A01
                ON  S1MCU = S5MCU AND
                    S1Z58SCN = S5Z58SCN AND
                    S1Z58SCD = S5Z58SCD
                LEFT OUTER JOIN F580200 
                ON  BIMCU = RDMCU AND
                    BIDOCO = RDDOC1 AND
                    BIZ58UNID = RDZ58SN
                LEFT OUTER JOIN {0}.F0005 UD1
                ON  UD1.DRSY = '57' AND
                    UD1.DRRT = 'RK' AND
                    TRIM(UD1.DRKY) = TRIM(RHZ58RT)
            WHERE
                RHZ58RID = ?
            GROUP BY
                RHZ58RID,
                RHMCU,
                RHAN8,
                RHZ58HST,
                RHZ58RSDT,
                AB1.ABALPH,
                RHSHAN,
                AB2.ABALPH,  
                RHZ58RT,
                UD1.DRDL01,
                RHZ58BOX,
                RHZ58GWTT,
                RHZ58RTWT,
                RHZ58SCN,
                RHLOCN,
                RDKCOO,
                RDDOCO,
                RDDCTO,
                RDLNID,
                RDKCOO,
                RDDOC1,
                RDXDCTO,
                RDPKCO,
                RDDOC2,
                RDXCTO,
                RDLNIC,
                RDZ58SD,
                RDZ58ROW,
                RDZ58STK,
                ATZ5899003,
                ATZ58CP2,
                ATZ58CP3,
                ATZ58CP4,
                ATZ58CSQNO,
                ATZ58GSQNO,
                ATZ5899009,
                ATZ5899010,
                ATZ5899011,
                ATZ5899012,
                SHDRQJ,
                SDDRQJ,
                SDUORG,
                SDVR01,
                S1Z58SCDDJ,
                COALESCE(S1Z58SCTYP, ''),
                COALESCE(BIZ58IGLN,'0')
            ORDER BY
                RDZ58SD,
                RDZ58ROW,
                RDZ58STK,
                RDZ58SL,
                RDZ58PS", ConfigurationManager.AppSettings["ControlLib"]);
            cmd.Parameters.Add(new iDB2Parameter("RackId", iDB2DbType.iDB2Decimal));
            cmd.Parameters["RackId"].Precision = 8;
            cmd.Parameters["RackId"].Scale = 0;
            return cmd;
        }

        /**************************************************************
         *  EDI Messages
         * ************************************************************/
        public iDB2Command GetEdiMessages()
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = @"
                    SELECT
                        ZDEDTY,
                        ZDEDSQ,
                        ZDLINS,
                        ZDTXLN
                    FROM
                        F4715 
                    WHERE
                        ZDKCOO = ? AND
                        ZDDOCO = ? AND
                        ZDDCTO = ? AND
                        ZDLNID = ?"
            };
            cmd.Parameters.Add(new iDB2Parameter("CompanyNum", iDB2DbType.iDB2Graphic, 5));
            cmd.Parameters.Add(new iDB2Parameter("OrderNum", iDB2DbType.iDB2Decimal));
            cmd.Parameters["OrderNum"].Precision = 9;
            cmd.Parameters["OrderNum"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("OrderType", iDB2DbType.iDB2Graphic, 2));
            cmd.Parameters.Add(new iDB2Parameter("OrderLineNum", iDB2DbType.iDB2Decimal));
            cmd.Parameters["OrderLineNum"].Precision = 6;
            cmd.Parameters["OrderLineNum"].Scale = 0;
            return cmd;
        }

        public iDB2Command GetRackFooterMessages()
        {
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText =
                @"SELECT
                        ZDEDSQ,
                        ZDTXLN
                    FROM
                        F4715 
                    WHERE
                        ZDEDTY = 'R' AND
                        ZDKCOO = ? AND
                        ZDDOCO = ? AND
                        ZDDCTO = ? AND
                        ZDLNID = ?";
            cmd.Parameters.Add(new iDB2Parameter("CompanyNum", iDB2DbType.iDB2Graphic, 5));
            cmd.Parameters.Add(new iDB2Parameter("OrderNum", iDB2DbType.iDB2Decimal));
            cmd.Parameters["OrderNum"].Precision = 9;
            cmd.Parameters["OrderNum"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("OrderType", iDB2DbType.iDB2Graphic, 2));
            cmd.Parameters.Add(new iDB2Parameter("OrderLineNum", iDB2DbType.iDB2Decimal));
            cmd.Parameters["OrderLineNum"].Precision = 6;
            cmd.Parameters["OrderLineNum"].Scale = 0;
            return cmd;
        }

        /*********************************************************
        * Rack Tag Items
        *********************************************************/
        public RackTagItems GetRackTagItems(BusinessSystemSession e1Session, decimal rackId, bool byUnitId)
        {
            string rackTagPlantNum = "";
            var rackTags = new RackTagItems();
            try
            {
                var cmd = new iDB2Command();
                if (byUnitId)
                {
                    cmd = (e1Session.DbCommands.ContainsKey("GetRackTagItemsByUnitId"))
                        ? e1Session.DbCommands["GetRackTagItemsByUnitId"]
                        : GetRackTagItemsByUnitId();
                }
                else
                {
                    cmd = (e1Session.DbCommands.ContainsKey("GetRackTagItemsByOrder"))
                        ? e1Session.DbCommands["GetRackTagItemsByOrder"]
                        : GetRackTagItemsByOrder();
                }

                cmd.Parameters[0].Value = rackId;
                cmd.Connection = e1Session.DbConnection;
                Logger.Info(Logging.DbCmdString(cmd));
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
                while (reader.Read())
                {
                    var rackTag = new RackTagItem();

                    rackTag.RackId = CastDbNull.To<Decimal>(reader["RHZ58RID"], 0);
                    rackTag.CompanyNum = CastDbNull.To<String>(reader["RDKCOO"], String.Empty).Trim();
                    rackTag.PlantNum = CastDbNull.To<String>(reader["RHMCU"], String.Empty).Trim();
                    rackTagPlantNum = rackTag.PlantNum;
                    rackTag.SalesOrderNum = CastDbNull.To<Decimal>(reader["RDDOCO"], 0);
                    rackTag.SalesOrderLineNum = Math.Round(CastDbNull.To<Decimal>(reader["RDLNID"], 0) / 1000, 3);
                    rackTag.SalesOrderType = CastDbNull.To<String>(reader["RDDCTO"], String.Empty).Trim();
                    rackTag.WorkOrderNum = CastDbNull.To<Decimal>(reader["RDDOC1"], 0);
                    rackTag.BeginSerialNum = CastDbNull.To<String>(reader["MIN_RDZ58SN"], String.Empty).Trim();
                    rackTag.EndSerialNum = CastDbNull.To<String>(reader["MAX_RDZ58SN"], String.Empty).Trim();
                    rackTag.CustSerialNum = CastDbNull.To<string>(reader["RDZ58UID"], "");
                    rackTag.ItemNum = CastDbNull.To<Decimal>(reader["RDITM"], 0);

                    rackTag.SoldToNum = CastDbNull.To<Decimal>(reader["RHAN8"], 0);
                    rackTag.SoldToName = CastDbNull.To<String>(reader["AN8ALPH"], String.Empty).Trim();
                    rackTag.ShipToNum = CastDbNull.To<Decimal>(reader["RHSHAN"], 0);
                    rackTag.ShipToName = CastDbNull.To<String>(reader["SHANALPH"], String.Empty).Trim();
                    rackTag.CustPartNum1 = CastDbNull.To<String>(reader["ATZ58CP2"], String.Empty).Trim();
                    rackTag.CustPartNum2 = CastDbNull.To<String>(reader["ATZ58CP3"], String.Empty).Trim();
                    rackTag.CustPartNum3 = CastDbNull.To<String>(reader["ATZ58CP4"], String.Empty).Trim();
                    rackTag.CustSeqNum1 = CastDbNull.To<String>(reader["ATZ58CSQNO"], String.Empty).Trim();
                    rackTag.GenericSeqNum = CastDbNull.To<String>(reader["ATZ58GSQNO"], String.Empty).Trim();
                    rackTag.IGLine = CastDbNull.To<String>(reader["BIZ58IGLN"], String.Empty).Trim();

                    rackTag.RackType = CastDbNull.To<String>(reader["RHZ58RT"], String.Empty).Trim();
                    rackTag.RackTypeDesc = CastDbNull.To<String>(reader["RHZ58RT_DESC"], String.Empty).Trim();
                    rackTag.Location = CastDbNull.To<String>(reader["RHLOCN"], String.Empty).Trim();
                    rackTag.Status = CastDbNull.To<String>(reader["RHZ58HST"], String.Empty).Trim();
                    rackTag.StatusDate = Utilities.Common.ConvertE1Date(CastDbNull.To<Decimal>(reader["RHZ58RSDT"], 0));
                    rackTag.BoxNum = CastDbNull.To<Decimal>(reader["RHZ58BOX"], 0);
                    rackTag.RackGlassWeight = CastDbNull.To<Decimal>(reader["RHZ58GWTT"], 0) / 100;
                    rackTag.RackTareWeight = CastDbNull.To<Decimal>(reader["RHZ58RTWT"], 0) / 100;
                    rackTag.ScheduleNum = CastDbNull.To<Decimal>(reader["RHZ58SCN"], 0);
                    rackTag.ScheduleType = CastDbNull.To<String>(reader["S1Z58SCTYP"], String.Empty).Trim();
                    rackTag.Quantity = CastDbNull.To<int>(reader["RDPQOH"], 0);
                    rackTag.CustomerPO = CastDbNull.To<String>(reader["SDVR01"], String.Empty).Trim();
                    rackTag.ShopFloorMessage1 = CastDbNull.To<String>(reader["ATZ5899009"], String.Empty).Trim();
                    rackTag.ShopFloorMessage2 = CastDbNull.To<String>(reader["ATZ5899010"], String.Empty).Trim();
                    rackTag.PickNotes = CastDbNull.To<String>(reader["ATZ5899011"], String.Empty).Trim();
                    rackTag.PackNotes = CastDbNull.To<String>(reader["ATZ5899012"], String.Empty).Trim();
                    rackTag.DueDate = Utilities.Common.ConvertE1Date(CastDbNull.To<Decimal>(reader["S1Z58SCDDJ"], 0));
                    rackTag.ScheduleDueDate = Utilities.Common.ConvertE1Date(CastDbNull.To<Decimal>(reader["S1Z58SCDDJ"], 0));
                    rackTag.CustomerDueDate = Utilities.Common.ConvertE1Date(CastDbNull.To<Decimal>(reader["SDDRQJ"], 0));

                    rackTag.RackSide = CastDbNull.To<String>(reader["RDZ58SD"], String.Empty).Trim();
                    rackTag.RackPosition = CastDbNull.To<Decimal>(reader["RDZ58PS"], 0);
                    rackTag.RackStack = CastDbNull.To<Decimal>(reader["RDZ58STK"], 0);
                    rackTag.RackRow = CastDbNull.To<Decimal>(reader["RDZ58ROW"], 0);
                    rackTag.RackSlot = CastDbNull.To<Decimal>(reader["RDZ58SL"], 0);
                    rackTag.BoxRotated = CastDbNull.To<String>(reader["RDZ58BXRO"], String.Empty).Trim();

                    // Item by Purchase
                    var segments = new ItemAttributes();
                    var rddoc2 = CastDbNull.To<Decimal>(reader["RDDOC2"], 0);
                    var rdpkco = CastDbNull.To<String>(reader["RDPKCO"], String.Empty).Trim();
                    if (rddoc2 > 0)
                    {
                        var rdxcto = CastDbNull.To<String>(reader["RDXCTO"], String.Empty).Trim();
                        var rdlnic = CastDbNull.To<Decimal>(reader["RDLNIC"], 0);
                        segments = _itemAttributeMgr.GetItemAttributes(e1Session, rackTag.ItemNum, rddoc2, rdxcto, rdlnic, rdpkco);
                    }

                    // Item by Sale
                    var rddoco = CastDbNull.To<Decimal>(reader["RDDOCO"], 0);
                    var rdkcoo = CastDbNull.To<String>(reader["RDKCOO"], String.Empty).Trim();
                    if (rddoc2 == 0 && rddoco > 0)
                    {
                        var rddcto = CastDbNull.To<String>(reader["RDDCTO"], String.Empty).Trim();
                        var rdlnid = CastDbNull.To<Decimal>(reader["RDLNID"], 0);
                        segments = _itemAttributeMgr.GetItemAttributes(e1Session, rackTag.ItemNum, rddoco, rddcto, rdlnid, rdkcoo);
                    }

                    // Item by Work
                    var rddoc1 = CastDbNull.To<Decimal>(reader["RDDOC1"], 0);
                    if (rddoco == 0 && rddoc2 == 0 && rddoc1 > 0)
                    {
                        segments = _itemAttributeMgr.GetItemAttributes(e1Session, rackTag.ItemNum, rddoc1, "WO", 0, "*");
                    }

                    // Item by Item
                    if (rddoco == 0 && rddoc2 == 0 && rddoc1 == 0)
                    {
                        segments = _itemAttributeMgr.GetDiscreteItemAttributes(e1Session, rackTag.ItemNum);
                    }

                    // Glass Type
                    rackTag.Lite1Thickness = segments.GetDecimalAttributeValue("Z58L1TK");
                    rackTag.Lite1Heat = segments.GetStringAttributeValue("Z58L1HT");
                    rackTag.Lite1FirstSrfCoat = segments.GetStringAttributeValue("Z58L11C");
                    rackTag.Lite1SecondSrfCoat = segments.GetStringAttributeValue("Z58L12C");
                    rackTag.Lite1Color = segments.GetStringAttributeDesc3("Z58L1CL");
                    rackTag.Lite1Pattern = segments.GetStringAttributeValue("Z58L1PX");
                    rackTag.Lite1LamiType = segments.GetStringAttributeValue("Z58L1LPT");
                    rackTag.Lite1LamiDesc = segments.GetStringAttributeDesc1("Z58L1LPT");
                    rackTag.Lite1LamiShopFloorDesc = segments.GetStringAttributeDesc3("Z58L1LPT");
                    rackTag.Lite1VGroove = segments.GetStringAttributeDesc1("Z58L1VG");
                    rackTag.Lite1Routing = segments.GetStringAttributeDesc1("Z58L1RTG");

                    rackTag.Lite2Thickness = segments.GetDecimalAttributeValue("Z58L2TK");
                    rackTag.Lite2Heat = segments.GetStringAttributeValue("Z58L2HT");
                    rackTag.Lite2ThirdSrfCoat = segments.GetStringAttributeValue("Z58L23C");
                    rackTag.Lite2FourthSrfCoat = segments.GetStringAttributeValue("Z58L24C");
                    rackTag.Lite2Color = segments.GetStringAttributeDesc3("Z58L2CL");
                    rackTag.Lite2Pattern = segments.GetStringAttributeValue("Z58L2PX");
                    rackTag.Lite2LamiType = segments.GetStringAttributeValue("Z58L2LPT");
                    rackTag.Lite2LamiDesc = segments.GetStringAttributeDesc1("Z58L2LPT");
                    rackTag.Lite2LamiShopFloorDesc = segments.GetStringAttributeDesc3("Z58L2LPT");
                    rackTag.Lite2VGroove = segments.GetStringAttributeDesc2("Z58L2VG");
                    rackTag.Lite3Thickness = segments.GetDecimalAttributeValue("Z58L3TK");

                    rackTag.Lite3Heat = segments.GetStringAttributeValue("Z58L3HT");
                    rackTag.Lite3FifthSrfCoat = segments.GetStringAttributeValue("Z58L35C");
                    rackTag.Lite3SixthSrfCoat = segments.GetStringAttributeValue("Z58L36C");
                    rackTag.Lite3Color = segments.GetStringAttributeDesc3("Z58L3CL");
                    rackTag.Lite3Pattern = segments.GetStringAttributeValue("Z58L3PX");
                    rackTag.Lite3LamiType = segments.GetStringAttributeValue("Z58L3LPT");
                    rackTag.Lite3LamiDesc = segments.GetStringAttributeDesc1("Z58L3LPT");
                    rackTag.Lite3LamiShopFloorDesc = segments.GetStringAttributeDesc3("Z58L3LPT");
                    rackTag.Lite3VGroove = segments.GetStringAttributeDesc3("Z58L3VG");

                    // Shapes
                    rackTag.ShapeNum = segments.GetDecimalAttributeValue("Z58SHAPE");
                    rackTag.Base = segments.GetDecimalAttributeValue("Z581BDEC");
                    rackTag.Left = segments.GetDecimalAttributeValue("Z581LDEC");
                    rackTag.Right = segments.GetDecimalAttributeValue("Z581RDEC");
                    rackTag.Top = segments.GetDecimalAttributeValue("Z581TDEC");
                    rackTag.S1 = segments.GetDecimalAttributeValue("Z5811DEC");
                    rackTag.S2 = segments.GetDecimalAttributeValue("Z5812DEC");

                    // Unit Weight
                    var qty = segments.GetDecimalAttributeValue("Z5840360");
                    var totalWeight = segments.GetDecimalAttributeValue("Z58GLWT") / 1000;
                    rackTag.UnitWeight = (qty == 0) ? 0 : Math.Round(qty / totalWeight, 3);

                    // Sqft
                    rackTag.UnitActualSqft = segments.GetDecimalAttributeValue("Z58USF");
                    rackTag.UnitBillableSqft = segments.GetDecimalAttributeValue("Z58BSF");

                    // Spacer
                    rackTag.Spacer1Size = segments.GetDecimalAttributeValue("Z58L1SSZ");
                    rackTag.Spacer1Type = segments.GetStringAttributeValue("Z58L1STP");
                    rackTag.Spacer2Size = segments.GetDecimalAttributeValue("Z58L2SSZ");
                    rackTag.Spacer2Type = segments.GetStringAttributeValue("Z58L2STP");

                    // Bars
                    rackTag.Bars1HorzNum = segments.GetDecimalAttributeValue("Z58GHA1");
                    rackTag.Bars1VertNum = segments.GetDecimalAttributeValue("Z58GVA1");
                    rackTag.Air1MCode1 = segments.GetDecimalAttributeValue("Z58M1A1");
                    rackTag.Air1MCode1Desc = _muntinBarCodeManager.GetBarCode(e1Session, rackTag.Air1MCode1.ToString().PadLeft(5, '0')).Description;
                    rackTag.Air1MCode2 = segments.GetDecimalAttributeValue("Z58M2A1");
                    rackTag.Air1MCode2Desc = _muntinBarCodeManager.GetBarCode(e1Session, rackTag.Air1MCode2.ToString().PadLeft(5, '0')).Description;
                    rackTag.Air2MCode1 = segments.GetDecimalAttributeValue("Z58M1A2");
                    rackTag.Air2MCode1Desc = _muntinBarCodeManager.GetBarCode(e1Session, rackTag.Air2MCode1.ToString().PadLeft(5, '0')).Description;
                    rackTag.Air2MCode2 = segments.GetDecimalAttributeValue("Z58M2A2");
                    rackTag.Air2MCode2Desc = _muntinBarCodeManager.GetBarCode(e1Session, rackTag.Air2MCode2.ToString().PadLeft(5, '0')).Description;

                    // Preserve / Scoring
                    rackTag.ExteriorPreserveColor = segments.GetStringAttributeValue("Z587566");
                    rackTag.ExteriorPreserveCutback = segments.GetStringAttributeValue("Z587563");
                    rackTag.InteriorPreserveColor = segments.GetStringAttributeValue("Z587578");
                    rackTag.InteriorPreserveCutback = segments.GetStringAttributeValue("Z587575");
                    rackTag.InteriorScoring = segments.GetStringAttributeValue("Z587581");
                    rackTag.ExteriorScoring = segments.GetStringAttributeValue("Z587569");
                    rackTag.BreatherTube1 = segments.GetStringAttributeValue("Z58TA1");
                    rackTag.BreatherTubeUnitsCount = (rackTag.BreatherTube1 == "Y") ? rackTag.Quantity : 0;
                    rackTag.ScoredUnitsCount = (String.IsNullOrEmpty(rackTag.InteriorScoring) &
                                                String.IsNullOrEmpty(rackTag.ExteriorScoring))
                                                   ? 0
                                                   : rackTag.Quantity;

                    // Other
                    rackTag.ProductType = segments.GetStringAttributeValue("Z58PT");
                    rackTag.CardinalProductType = segments.GetStringAttributeValue("Z5840310");
                    rackTag.CrateType = segments.GetStringAttributeValue("Z58CRATE");
                    rackTag.OriginalBranchPlant = segments.GetStringAttributeValue("Z5840415");

                    // Picked Units
                    rackTag.PickItemNum = segments.GetDecimalAttributeValue("Z5880230");
                    rackTag.PickedUnitsCount = rackTag.PickItemNum != 0 ? rackTag.Quantity : 0;

                    rackTags.Add(rackTag);
                }
                reader.Close();

                // Aggregate values
                if (rackTags.Any<RackTagItem>())
                {
                    string custSeqNum11 = rackTags.First<RackTagItem>().CustSeqNum1;
                    string custSeqNum12 = rackTags.Last<RackTagItem>().CustSeqNum1;
                    foreach (RackTagItem rackTagItem in (List<RackTagItem>)rackTags)
                    {
                        rackTagItem.RackFirstCustSeqNum = custSeqNum11;
                        rackTagItem.RackLastCustSeqNum = custSeqNum12;
                    }
                }

                //// Rack Footer Messages
                //foreach (RackTagItem rackTag in rackTags)
                //{
                //    var cmd1 = (e1Session.DbCommands.ContainsKey("GetRackFooterMessages")) ? e1Session.DbCommands["GetRackFooterMessages"] : GetRackFooterMessages();
                //    cmd1.Parameters[0].Value = rackTag.CompanyNum.PadLeft(5);
                //    cmd1.Parameters[1].Value = rackTag.SalesOrderNum;
                //    cmd1.Parameters[2].Value = rackTag.SalesOrderType.PadLeft(2);
                //    cmd1.Parameters[3].Value = rackTag.SalesOrderLineNum;
                //    cmd1.Connection = e1Session.DbConnection;
                //    IDataReader reader2 = cmd1.ExecuteReader(CommandBehavior.SingleResult);
                //    while (reader2.Read())
                //    {
                //        switch (Convert.ToInt32(CastDbNull.To<decimal>(reader2["ZDEDSQ"], 0)))
                //        {
                //            case 1:
                //                rackTag.FooterMsg1 = CastDbNull.To<String>(reader2["ZDTXLN"], String.Empty).Trim();
                //                break;
                //            case 2:
                //                rackTag.FooterMsg2 = CastDbNull.To<String>(reader2["ZDTXLN"], String.Empty).Trim();
                //                break;
                //            case 3:
                //                rackTag.FooterMsg3 = CastDbNull.To<String>(reader2["ZDTXLN"], String.Empty).Trim();
                //                break;
                //            case 4:
                //                rackTag.FooterMsg4 = CastDbNull.To<String>(reader2["ZDTXLN"], String.Empty).Trim();
                //                break;
                //            case 5:
                //                rackTag.FooterMsg5 = CastDbNull.To<String>(reader2["ZDTXLN"], String.Empty).Trim();
                //                break;
                //            case 6:
                //                rackTag.FooterMsg6 = CastDbNull.To<String>(reader2["ZDTXLN"], String.Empty).Trim();
                //                break;
                //        }
                //    }
                //    reader2.Close();
                //}

                // Edi Messages
                foreach (var rackTag in rackTags)
                {
                    var cmd1 = (e1Session.DbCommands.ContainsKey("RackTagItemManager.GetEdiMessages")) ? e1Session.DbCommands["RackTagItemManager.GetEdiMessages"] : GetEdiMessages();
                    cmd1.Parameters[0].Value = rackTag.CompanyNum.PadLeft(5);
                    cmd1.Parameters[1].Value = rackTag.SalesOrderNum;
                    cmd1.Parameters[2].Value = rackTag.SalesOrderType.PadLeft(2);
                    cmd1.Parameters[3].Value = rackTag.SalesOrderLineNum * 1000;
                    cmd1.Connection = e1Session.DbConnection;
                    Logger.Info(Logging.DbCmdString(cmd1));
                    using (IDataReader reader2 = cmd1.ExecuteReader(CommandBehavior.SingleResult))
                    {
                        while (reader2.Read())
                        {
                            var msgType = CastDbNull.To(reader2["ZDEDTY"], string.Empty).Trim();
                            if (msgType == "R")
                            {
                                switch (Convert.ToInt32(CastDbNull.To<decimal>(reader2["ZDEDSQ"], 0)))
                                {
                                    case 1:
                                        rackTag.FooterMsg1 = CastDbNull.To(reader2["ZDTXLN"], string.Empty).Trim();
                                        break;
                                    case 2:
                                        rackTag.FooterMsg2 = CastDbNull.To(reader2["ZDTXLN"], string.Empty).Trim();
                                        break;
                                    case 3:
                                        rackTag.FooterMsg3 = CastDbNull.To(reader2["ZDTXLN"], string.Empty).Trim();
                                        break;
                                    case 4:
                                        rackTag.FooterMsg4 = CastDbNull.To(reader2["ZDTXLN"], string.Empty).Trim();
                                        break;
                                    case 5:
                                        rackTag.FooterMsg5 = CastDbNull.To(reader2["ZDTXLN"], string.Empty).Trim();
                                        break;
                                    case 6:
                                        rackTag.FooterMsg6 = CastDbNull.To(reader2["ZDTXLN"], string.Empty).Trim();
                                        break;
                                }
                            }
                            else
                            {
                                if (msgType == "L")
                                {
                                    switch (Convert.ToInt32(CastDbNull.To<decimal>(reader2["ZDEDSQ"], 0)))
                                    {
                                        case 1:
                                            rackTag.LabelMsg1 = CastDbNull.To(reader2["ZDTXLN"], string.Empty).Trim();
                                            break;
                                        case 2:
                                            rackTag.LabelMsg2 = CastDbNull.To(reader2["ZDTXLN"], string.Empty).Trim();
                                            break;
                                        case 3:
                                            rackTag.LabelMsg3 = CastDbNull.To(reader2["ZDTXLN"], string.Empty).Trim();
                                            break;
                                        case 4:
                                            rackTag.LabelMsg4 = CastDbNull.To(reader2["ZDTXLN"], string.Empty).Trim();
                                            break;
                                        case 5:
                                            rackTag.LabelMsg5 = CastDbNull.To(reader2["ZDTXLN"], string.Empty).Trim();
                                            break;
                                        case 6:
                                            rackTag.LabelMsg6 = CastDbNull.To(reader2["ZDTXLN"], string.Empty).Trim();
                                            break;
                                        case 7:
                                            rackTag.LabelMsg7 = CastDbNull.To(reader2["ZDTXLN"], string.Empty).Trim();
                                            break;
                                        case 8:
                                            rackTag.LabelMsg8 = CastDbNull.To(reader2["ZDTXLN"], string.Empty).Trim();
                                            break;
                                        case 9:
                                            rackTag.LabelMsg9 = CastDbNull.To(reader2["ZDTXLN"], string.Empty).Trim();
                                            break;
                                        case 10:
                                            rackTag.LabelMsg10 = CastDbNull.To(reader2["ZDTXLN"], string.Empty).Trim();
                                            break;
                                        case 11:
                                            rackTag.LabelMsg11 = CastDbNull.To(reader2["ZDTXLN"], string.Empty).Trim();
                                            break;
                                        case 12:
                                            rackTag.LabelMsg12 = CastDbNull.To(reader2["ZDTXLN"], string.Empty).Trim();
                                            break;
                                        case 13:
                                            rackTag.LabelMsg13 = CastDbNull.To(reader2["ZDTXLN"], string.Empty).Trim();
                                            break;
                                        case 14:
                                            rackTag.LabelMsg14 = CastDbNull.To(reader2["ZDTXLN"], string.Empty).Trim();
                                            break;
                                        case 15:
                                            rackTag.LabelMsg15 = CastDbNull.To(reader2["ZDTXLN"], string.Empty).Trim();
                                            break;
                                        case 16:
                                            rackTag.LabelMsg16 = CastDbNull.To(reader2["ZDTXLN"], string.Empty).Trim();
                                            break;
                                        case 17:
                                            rackTag.LabelMsg17 = CastDbNull.To(reader2["ZDTXLN"], string.Empty).Trim();
                                            break;
                                        case 18:
                                            rackTag.LabelMsg18 = CastDbNull.To(reader2["ZDTXLN"], string.Empty).Trim();
                                            break;
                                        case 19:
                                            rackTag.LabelMsg19 = CastDbNull.To(reader2["ZDTXLN"], string.Empty).Trim();
                                            break;
                                        case 20:
                                            rackTag.LabelMsg20 = CastDbNull.To(reader2["ZDTXLN"], string.Empty).Trim();
                                            break;
                                    }
                                }
                            }
                        }

                        reader2.Close();
                    }
                }

                var cnSF = GetSFSqlConnection(PlantManager.GetPlant(rackTagPlantNum).PlantCode);
                var schedBoxes = rackTags.Select(sb => new { sb.ScheduleNum, sb.BoxNum }).Distinct();
                foreach (var schedBox in schedBoxes)
                {
                    // Get flexi boxing RackID Messages
                    var scheduleBoxRackIDMsg = GetScheduleBoxRackIDMsg(cnSF, schedBox.ScheduleNum, schedBox.BoxNum);
                    foreach (var rackTag in rackTags)
                    {
                        if (rackTag.ScheduleNum == schedBox.ScheduleNum && rackTag.BoxNum == schedBox.BoxNum)
                        {
                            rackTag.RackIDMessage1 = scheduleBoxRackIDMsg;
                        }
                    }
                    // Get Dunnage Messages
                    var scheduleBoxDunnageMsg = GetScheduleBoxDunnageMsg(cnSF, schedBox.ScheduleNum, schedBox.BoxNum);
                    foreach (var rackTag in rackTags)
                    {
                        if (rackTag.ScheduleNum == schedBox.ScheduleNum && rackTag.BoxNum == schedBox.BoxNum)
                        {
                            rackTag.DunnageMessage = scheduleBoxDunnageMsg;
                        }
                    }
                }

                Logger.Trace(XmlSerializationUtility.SerializeObject(Encoding.UTF8, rackTags));
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return rackTags;
        }

        private static SqlConnection GetSFSqlConnection(string plantCode)
        {
            var cnSF = new SqlConnection();
            try
            {
                cnSF = ConnectionManager.GetSqlConnectionFromKey(plantCode + ".SHOPFLOOR", false);
                cnSF.Open();
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {cnSF.ConnectionString}  {e.Message}: {e.StackTrace}");
                throw new ApplicationException(cnSF.ConnectionString + "  " + e.Message);
            }
            return cnSF;
        }

        public SqlCommand GetScheduleBoxeRackIDMessages()
        {
            var cmd = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = $@"
                    SELECT [RackIDMessage]
                    FROM [ShopFloor].[dbo].[ScheduleBoxes]
                    WHERE [SchedNum] = @ScheduleNum
                    AND [BoxNum] = @BoxNum
                "
            };
            cmd.Parameters.Add(new SqlParameter("@ScheduleNum", SqlDbType.Decimal));
            cmd.Parameters.Add(new SqlParameter("@BoxNum", SqlDbType.Decimal));
            return cmd;
        }

        public string GetScheduleBoxRackIDMsg(SqlConnection cnSF, decimal scheduleNum, decimal boxNum)
        {
            var scheduleBoxRackIDMsg = "";
            try
            {
                var cmd = GetScheduleBoxeRackIDMessages();
                cmd.Parameters["@ScheduleNum"].Value = scheduleNum;
                cmd.Parameters["@BoxNum"].Value = boxNum;
                cmd.Connection = cnSF;
                Logger.Trace(Logging.DbCmdString(cmd));
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
                {
                    while (reader.Read())
                    {
                        scheduleBoxRackIDMsg = CastDbNull.To(reader["RackIDMessage"], string.Empty).Trim();

                        Logger.Trace($"Rack Id Msg [{scheduleBoxRackIDMsg}] 1 row found");
                    }

                    reader.Close();
                }
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + cnSF.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
                throw new Exception(cnSF.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }
            finally
            {
            }
            return scheduleBoxRackIDMsg;
        }

        public SqlCommand GetScheduleDunnageMessages()
        {
            var cmd = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = $@"
                    SELECT DISTINCT [DunnageMessage]
                    FROM [ShopFloor].[dbo].[BoxHold]
                    WHERE [ScheduleNum] = @ScheduleNum
                    AND [BoxNumber] = @BoxNum
                "
            };
            cmd.Parameters.Add(new SqlParameter("@ScheduleNum", SqlDbType.Decimal));
            cmd.Parameters.Add(new SqlParameter("@BoxNum", SqlDbType.Decimal));
            return cmd;
        }

        public string GetScheduleBoxDunnageMsg(SqlConnection cnSF, decimal scheduleNum, decimal boxNum)
        {
            var scheduleBoxDunnageMsg = "";
            try
            {
                var cmd = GetScheduleDunnageMessages();
                cmd.Parameters["@ScheduleNum"].Value = scheduleNum;
                cmd.Parameters["@BoxNum"].Value = boxNum;
                cmd.Connection = cnSF;
                Logger.Trace(Logging.DbCmdString(cmd));
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
                {
                    while (reader.Read())
                    {
                        scheduleBoxDunnageMsg = CastDbNull.To(reader["DunnageMessage"], string.Empty).Trim();

                        Logger.Trace($"Dunnage Msg [{scheduleBoxDunnageMsg}] 1 row found");
                    }

                    reader.Close();
                }
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + cnSF.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
                throw new Exception(cnSF.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }
            finally
            {
            }
            return scheduleBoxDunnageMsg;
        }
    }
}
