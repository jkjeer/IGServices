using System;
using System.Collections.Generic;
using System.Data;
using IBM.Data.DB2.iSeries;
using IGServices.Common.Types.E1;
using IGServices.Common.Types.Enterprise;
using IGServices.Common.Utilities;

namespace IGServices.Common.Managers.Enterprise
{
    public class RackManager1
    {
        private GlassItemManager _glassItemMgr;
        private CustomerManager _customerMgr;

        public RackManager1(BusinessSystemSession e1Session)
        {
            var hash = new Dictionary<string, iDB2Command>();
            hash.Add("RackManager1.GetRackItemReceiving", GetRackItemReceiving());
            foreach (string keyName in hash.Keys)
            {
                if (!e1Session.DbCommands.ContainsKey(keyName))
                {
                    e1Session.DbCommands.Add(keyName, hash[keyName]);
                }
            }
            _glassItemMgr = new GlassItemManager(e1Session);
            _customerMgr = new CustomerManager(e1Session);
        }


        /******************************************************************
        * Racks
        ******************************************************************/
        public iDB2Command GetPurchasedRacks(string plantNum)
        {
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = 300;
            cmd.CommandText = @"
            SELECT 
                RHZ58RID,
                RHMCU,
                RHLOCN,
                RHZ58RT,
                RHZ58HST,
                RHZ58RSDT,
                RHZ58GWTT / 100 AS RHZ58GWTT,
                RHZ58RTWT / 100 AS RHZ58RTWT,
                RHGWEI,
                RHUWUM,
                RHZ58RCD,
                RHZ58SCN,
                RHZ58BOX,
                RHPSN,
                RHAN8,
                RHUPMJ,
                RHTDAY,
                RDMCU,
                RDKCOO,
                RDZ58SD,
                RDZ58PS,
                RDZ58STK,
                RDZ58ROW,
                RDZ58SL,
                RDITM,
                RDLOTN,
                RDZ58DST,
                RDTRDJ,
                RDZ58MDT,
                RDZ58PDT,
                RDPQOH,
                RDUOM1,
                RDZ58GWT,
                RDUWUM, 
                RDZ58SN,
                RDUPMJ,
                RDTDAY,
                RDDOCO,
                RDDCTO,
                RDLNID,
                RDDOC1,
                RDDOC2,
                RDXCTO,
                RDLNIC,
                RDPKCO,
                RDPID,
                IBVEND AS SUPPAN8,
                0 AS CUSTAN8,
                IBGLPT,
                'PUR' AS TYPE
            FROM 
	            F580055
	            INNER JOIN F580056
	            ON RHZ58RID = RDZ58RID 
	            INNER JOIN F4102
	            ON  IBMCU = RDMCU AND
                    IBITM = RDITM
            WHERE 
	            RHMCU = ? AND 
                RHZ58HST NOT IN ('0') AND
	            RDDOC2 > 0
            UNION ALL
            SELECT 
                RHZ58RID,
                RHMCU,
                RHLOCN,
                RHZ58RT,
                RHZ58HST,
                RHZ58RSDT,
                RHZ58GWTT / 100 AS RHZ58GWTT,
                RHZ58RTWT / 100 AS RHZ58RTWT,
                RHGWEI,
                RHUWUM,
                RHZ58RCD,
                RHZ58SCN,
                RHZ58BOX,
                RHPSN,
                RHAN8,
                RHUPMJ,
                RHTDAY,
                RDMCU,
                RDKCOO,
                RDZ58SD,
                RDZ58PS,
                RDZ58STK,
                RDZ58ROW,
                RDZ58SL,
                RDITM,
                RDLOTN,
                RDZ58DST,
                RDTRDJ,
                RDZ58MDT,
                RDZ58PDT,
                RDPQOH,
                RDUOM1,
                RDZ58GWT,
                RDUWUM, 
                RDZ58SN,
                RDUPMJ,
                RDTDAY,
                A.DLDOCO AS RDDOCO,
                A.DLDCTO AS RDDCTO,
                A.DLLNID AS RDLNID,
                A.DLDOC1 AS RDDOC1,
                A.DLDOC2 AS RDDOC2,
                A.DLXCTO AS RDXCTO,
                A.DLLNIC AS RDLNIC,
                A.DLPKCO AS RDPKCO,
                RDPID,
                IBVEND AS SUPPAN8,
                0 AS CUSTAN8,
                IBGLPT,
                'PURA' AS TYPE
            FROM 
	            F580055
	            INNER JOIN F580056
	            ON RHZ58RID = RDZ58RID 
	            INNER JOIN (SELECT DLZ58RID, DLZ58SN, DLMCU, MAX(DLUKID) AS DLUKID FROM F580569 WHERE DLDOC2 > 0 GROUP BY DLZ58RID, DLZ58SN, DLMCU) Z
	            ON Z.DLZ58RID = RDZ58RID AND Z.DLZ58SN = RDZ58SN AND Z.DLMCU = RDMCU
	            INNER JOIN F580569 A
	            ON A.DLUKID = Z.DLUKID
                INNER JOIN F4102
                ON  IBMCU = RDMCU AND
                    IBITM = RDITM
            WHERE 
	            RHMCU = ? AND 
                RHZ58HST NOT IN ('0') AND
	            RDDOCO = 0 AND
	            RDDOC1 = 0 AND
	            RDDOC2 = 0 AND
	            RDPQOH > 0 AND
	            RDLOTN = ''
            ORDER BY 1,2,3";
            cmd.Parameters.Add(new iDB2Parameter("PlantNum1", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("PlantNum2", iDB2DbType.iDB2Graphic, 12));
            return cmd;
        }

        public iDB2Command GetRacks(string plantNum, string statusClause)
        {
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = 300;
            cmd.CommandText = string.Format(@"
            SELECT 
                RHZ58RID,
                RHMCU,
                RHLOCN,
                RHZ58RT,
                RHZ58HST,
                RHZ58RSDT,
                RHZ58GWTT / 100 AS RHZ58GWTT,
                RHZ58RTWT / 100 AS RHZ58RTWT,
                RHGWEI,
                RHUWUM,
                RHZ58RCD,
                RHZ58SCN,
                RHZ58BOX,
                RHPSN,
                RHAN8,
                RHUPMJ,
                RHTDAY,
                RDMCU,
                RDZ58RID,
                RDMCU,
                RDKCOO,
                RDZ58SD,
                RDZ58PS,
                RDZ58STK,
                RDZ58ROW,
                RDZ58SL,
                RDITM,
                RDLOTN,
                RDZ58DST,
                RDTRDJ,
                RDZ58MDT,
                RDZ58PDT,
                RDPQOH,
                RDUOM1,
                RDZ58GWT,
                RDUWUM, 
                RDZ58SN,
                RDUPMJ,
                RDTDAY,
                RDDOCO,
                RDDCTO,
                RDLNID,
                RDDOC1,
                RDDOC2,
                RDXCTO,
                RDLNIC,
                RDPKCO,
                RDPID,
                IBGLPT,
                IBVEND AS SUPPAN8,
                0 AS CUSTAN8,
	            'PUR' AS TYPE
            FROM 
	            F580055
	            INNER JOIN F580056
	            ON RHZ58RID = RDZ58RID
                INNER JOIN F4102
                ON  IBMCU = RDMCU AND
                    IBITM = RDITM              
            WHERE 
	            RHMCU = '{0}' AND 
	            RHZ58HST IN ({1}) AND 
	            RDDOC2 > 0
            UNION ALL
            SELECT 
                RHZ58RID,
                RHMCU,
                RHLOCN,
                RHZ58RT,
                RHZ58HST,
                RHZ58RSDT,
                RHZ58GWTT / 100 AS RHZ58GWTT,
                RHZ58RTWT / 100 AS RHZ58RTWT,
                RHGWEI,
                RHUWUM,
                RHZ58RCD,
                RHZ58SCN,
                RHZ58BOX,
                RHPSN,
                RHAN8,
                RHUPMJ,
                RHTDAY,
                RDMCU,
                RDZ58RID,
                RDMCU,
                RDKCOO,
                RDZ58SD,
                RDZ58PS,
                RDZ58STK,
                RDZ58ROW,
                RDZ58SL,
                RDITM,
                RDLOTN,
                RDZ58DST,
                RDTRDJ,
                RDZ58MDT,
                RDZ58PDT,
                RDPQOH,
                RDUOM1,
                RDZ58GWT,
                RDUWUM, 
                RDZ58SN,
                RDUPMJ,
                RDTDAY,
                RDDOCO,
                RDDCTO,
                RDLNID,
                RDDOC1,
                RDDOC2,
                RDXCTO,
                RDLNIC,
                RDPKCO,
                RDPID,
                IBGLPT,
                0 AS SUPPAN8,
                RHAN8 AS CUSTAN8,
	            'MTO' AS TYPE
            FROM 
	            F580055
	            INNER JOIN F580056
	            ON RHZ58RID = RDZ58RID
                INNER JOIN F4102
                ON  IBMCU = RDMCU AND
                    IBITM = RDITM              
            WHERE 
	            RHMCU = '{0}' AND 
	            RHZ58HST IN ({1}) AND 
	            RDDOC2 = 0 AND
                RDDOCO > 0
            UNION ALL
            SELECT 
                RHZ58RID,
                RHMCU,
                RHLOCN,
                RHZ58RT,
                RHZ58HST,
                RHZ58RSDT,
                RHZ58GWTT / 100 AS RHZ58GWTT,
                RHZ58RTWT / 100 AS RHZ58RTWT,
                RHGWEI,
                RHUWUM,
                RHZ58RCD,
                RHZ58SCN,
                RHZ58BOX,
                RHPSN,
                RHAN8,
                RHUPMJ,
                RHTDAY,
                RDMCU,
                RDZ58RID,
                RDMCU,
                RDKCOO,
                RDZ58SD,
                RDZ58PS,
                RDZ58STK,
                RDZ58ROW,
                RDZ58SL,
                RDITM,
                RDLOTN,
                RDZ58DST,
                RDTRDJ,
                RDZ58MDT,
                RDZ58PDT,
                RDPQOH,
                RDUOM1,
                RDZ58GWT,
                RDUWUM, 
                RDZ58SN,
                RDUPMJ,
                RDTDAY,
                RDDOCO,
                RDDCTO,
                RDLNID,
                RDDOC1,
                RDDOC2,
                RDXCTO,
                RDLNIC,
                RDPKCO,
                RDPID,
                IBGLPT,
                0 AS SUPPAN8,
                IBANPL AS CUSTAN8,
	            'MTS' AS TYPE
            FROM 
	            F580055
	            INNER JOIN F580056
	            ON RHZ58RID = RDZ58RID
                INNER JOIN F4102
                ON  IBMCU = RDMCU AND
                    IBITM = RDITM              
            WHERE 
	            RHMCU = '{0}' AND 
	            RHZ58HST IN ({1}) AND 
	            RDDOC2 = 0 AND
                RDDOCO = 0 AND
                RDDOC1 > 0
            UNION ALL    
            SELECT 
                RHZ58RID,
                RHMCU,
                RHLOCN,
                RHZ58RT,
                RHZ58HST,
                RHZ58RSDT,
                RHZ58GWTT / 100 AS RHZ58GWTT,
                RHZ58RTWT / 100 AS RHZ58RTWT,
                RHGWEI,
                RHUWUM,
                RHZ58RCD,
                RHZ58SCN,
                RHZ58BOX,
                RHPSN,
                RHAN8,
                RHUPMJ,
                RHTDAY,
                RDMCU,
                RDZ58RID,
                RDMCU,
                RDKCOO,
                RDZ58SD,
                RDZ58PS,
                RDZ58STK,
                RDZ58ROW,
                RDZ58SL,
                RDITM,
                RDLOTN,
                RDZ58DST,
                RDTRDJ,
                RDZ58MDT,
                RDZ58PDT,
                RDPQOH,
                RDUOM1,
                RDZ58GWT,
                RDUWUM, 
                RDZ58SN,
                RDUPMJ,
                RDTDAY,
                RDDOCO,
                RDDCTO,
                RDLNID,
                RDDOC1,
                RDDOC2,
                RDXCTO,
                RDLNIC,
                RDPKCO,
                RDPID,
                IBGLPT,
                IBVEND AS SUPPAN8,
                0 AS CUSTAN8,
	            'CYCL' AS TYPE
            FROM 
	            F580055 
	            INNER JOIN F580056
	            ON RHZ58RID = RDZ58RID 
                INNER JOIN F4102
                ON  IBMCU = RDMCU AND
                    IBITM = RDITM  
	            INNER JOIN (SELECT DLZ58RID, DLZ58SN, DLMCU, MAX(DLUKID) AS DLUKID FROM F580569 WHERE DLPID = 'R41413' GROUP BY DLZ58RID, DLZ58SN, DLMCU) Z
	            ON Z.DLZ58RID = RDZ58RID AND Z.DLZ58SN = RDZ58SN AND Z.DLMCU = RDMCU
            WHERE 
	            RHMCU = '{0}' AND 
	            RHZ58HST IN ({1}) AND 
	            RDDOCO = 0 AND
	            RDDOC1 = 0 AND
	            RDDOC2 = 0 
            UNION ALL
            SELECT 
                RHZ58RID,
                RHMCU,
                RHLOCN,
                RHZ58RT,
                RHZ58HST,
                RHZ58RSDT,
                RHZ58GWTT / 100 AS RHZ58GWTT,
                RHZ58RTWT / 100 AS RHZ58RTWT,
                RHGWEI,
                RHUWUM,
                RHZ58RCD,
                RHZ58SCN,
                RHZ58BOX,
                RHPSN,
                RHAN8,
                RHUPMJ,
                RHTDAY,
                RDMCU,
                RDZ58RID,
                RDMCU,
                RDKCOO,
                RDZ58SD,
                RDZ58PS,
                RDZ58STK,
                RDZ58ROW,
                RDZ58SL,
                RDITM,
                RDLOTN,
                RDZ58DST,
                RDTRDJ,
                RDZ58MDT,
                RDZ58PDT,
                RDPQOH,
                RDUOM1,
                RDZ58GWT,
                RDUWUM, 
                RDZ58SN,
                RDUPMJ,
                RDTDAY,
                A.DLDOCO,
                A.DLDCTO,
                A.DLLNID,
                A.DLDOC1,
                A.DLDOC2,
                A.DLXCTO,
                A.DLLNIC,
                A.DLPKCO,
                RDPID,
                IBGLPT,
                0 AS SUPPAN8,
                IBANPL AS CUSTAN8,
	            'MTSA' AS TYPE
            FROM 
	            F580055
	            INNER JOIN F580056 
	            ON RHZ58RID = RDZ58RID 
                INNER JOIN F4102
                ON  IBMCU = RDMCU AND
                    IBITM = RDITM  
	            INNER JOIN (SELECT DLZ58RID, DLZ58SN, DLMCU, MAX(DLUKID) AS DLUKID FROM F580569 WHERE DLDOC1 > 0 GROUP BY DLZ58RID, DLZ58SN, DLMCU) Z
	            ON Z.DLZ58RID = RDZ58RID AND Z.DLZ58SN = RDZ58SN AND Z.DLMCU = RDMCU
	            INNER JOIN F580569 A
	            ON A.DLUKID = Z.DLUKID
            WHERE 
	            RHMCU = '{0}' AND 
	            RHZ58HST IN ({1}) AND 
	            RDDOCO = 0 AND
	            RDDOC1 = 0 AND
	            RDDOC2 = 0 AND
	            RDPQOH > 0 AND
	            RDLOTN = '' AND
	            NOT EXISTS (SELECT 1 FROM F580569 WHERE DLPID = 'R41413' and DLZ58RID = RDZ58RID AND DLZ58SN = RDZ58SN AND DLMCU = RDMCU)
            UNION ALL
            SELECT 
                RHZ58RID,
                RHMCU,
                RHLOCN,
                RHZ58RT,
                RHZ58HST,
                RHZ58RSDT,
                RHZ58GWTT / 100 AS RHZ58GWTT,
                RHZ58RTWT / 100 AS RHZ58RTWT,
                RHGWEI,
                RHUWUM,
                RHZ58RCD,
                RHZ58SCN,
                RHZ58BOX,
                RHPSN,
                RHAN8,
                RHUPMJ,
                RHTDAY,
                RDMCU,
                RDZ58RID,
                RDMCU,
                RDKCOO,
                RDZ58SD,
                RDZ58PS,
                RDZ58STK,
                RDZ58ROW,
                RDZ58SL,
                RDITM,
                RDLOTN,
                RDZ58DST,
                RDTRDJ,
                RDZ58MDT,
                RDZ58PDT,
                RDPQOH,
                RDUOM1,
                RDZ58GWT,
                RDUWUM, 
                RDZ58SN,
                RDUPMJ,
                RDTDAY,
                A.DLDOCO,
                A.DLDCTO,
                A.DLLNID,
                A.DLDOC1,
                A.DLDOC2,
                A.DLXCTO,
                A.DLLNIC,
                A.DLPKCO,
                RDPID,
                IBGLPT,
                IBVEND AS SUPPAN8,
                0 AS CUSTAN8,
	            'PURA' AS TYPE
            FROM 
	            F580055
	            INNER JOIN F580056 
	            ON RHZ58RID = RDZ58RID 
                INNER JOIN F4102
                ON  IBMCU = RDMCU AND
                    IBITM = RDITM  
	            INNER JOIN (SELECT DLZ58RID, DLZ58SN, DLMCU, MAX(DLUKID) AS DLUKID FROM F580569 WHERE DLDOC2 > 0 GROUP BY DLZ58RID, DLZ58SN, DLMCU) Z
	            ON Z.DLZ58RID = RDZ58RID AND Z.DLZ58SN = RDZ58SN AND Z.DLMCU = RDMCU
	            INNER JOIN F580569 A
	            ON A.DLUKID = Z.DLUKID
            WHERE 
	            RHMCU = '{0}' AND 
	            RHZ58HST IN ({1}) AND 
	            RDDOCO = 0 AND
	            RDDOC1 = 0 AND
	            RDDOC2 = 0 AND
	            RDPQOH > 0 AND
	            RDLOTN = '' AND
	            NOT EXISTS (SELECT 1 FROM F580569 WHERE DLPID = 'R41413' and DLZ58RID = RDZ58RID AND DLZ58SN = RDZ58SN AND DLMCU = RDMCU)
            ORDER BY 1,2,3", plantNum, statusClause);
            return cmd;
        }

        public iDB2Command GetArchivedRacks(string plantNum, string statusClause)
        {
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = string.Format(@"
            SELECT 
                RHZ58RID,
                RHMCU,
                RHLOCN,
                RHZ58RT,
                RHZ58HST,
                RHZ58RSDT,
                RHZ58GWTT / 100 AS RHZ58GWTT,
                RHZ58RTWT / 100 AS RHZ58RTWT,
                RHGWEI,
                RHUWUM,
                RHZ58RCD,
                RHZ58SCN,
                RHZ58BOX,
                RHPSN,
                RHAN8,
                RHUPMJ,
                RHTDAY,
                RDMCU,
                RDZ58RID,
                RDMCU,
                RDKCOO,
                RDZ58SD,
                RDZ58PS,
                RDZ58STK,
                RDZ58ROW,
                RDZ58SL,
                RDITM,
                RDLOTN,
                RDZ58DST,
                RDTRDJ,
                RDZ58MDT,
                RDZ58PDT,
                RDPQOH,
                RDUOM1,
                RDZ58GWT,
                RDUWUM, 
                RDZ58SN,
                RDUPMJ,
                RDTDAY,
                A.DLDOCO,
                A.DLDCTO,
                A.DLLNID,
                A.DLDOC1,
                A.DLDOC2,
                A.DLXCTO,
                A.DLLNIC,
                A.DLPKCO,
                RDPID,
	            'MTSA' AS TYPE
            FROM 
	            F580056 
	            INNER JOIN F580055 
	            ON RHZ58RID = RDZ58RID 
	            INNER JOIN (SELECT DLZ58RID, DLZ58SN, DLMCU, MAX(DLUKID) AS DLUKID FROM F580569 WHERE DLDOCO > 0 OR DLDOC1 > 0 OR DLDOC2 > 0 GROUP BY DLZ58RID, DLZ58SN, DLMCU) Z
	            ON Z.DLZ58RID = RDZ58RID AND Z.DLZ58SN = RDZ58SN AND Z.DLMCU = RDMCU
	            INNER JOIN F580569 A
	            ON A.DLUKID = Z.DLUKID
            WHERE 
	            RDMCU = '{0}' AND 
	            RDZ58DST IN ({1}) AND 
	            RDDOCO = 0 AND
	            RDDOC1 = 0 AND
	            RDDOC2 = 0 AND
	            RDPQOH > 0 AND
	            RDLOTN = '' AND
	            A.DLDOC1 > 0 AND
	            NOT EXISTS (SELECT 1 FROM F580569 WHERE DLPID = 'R41413' and DLZ58RID = RDZ58RID AND DLZ58SN = RDZ58SN AND DLMCU = RDMCU)
            UNION ALL
            SELECT 
                RHZ58RID,
                RHMCU,
                RHLOCN,
                RHZ58RT,
                RHZ58HST,
                RHZ58RSDT,
                RHZ58GWTT / 100 AS RHZ58GWTT,
                RHZ58RTWT / 100 AS RHZ58RTWT,
                RHGWEI,
                RHUWUM,
                RHZ58RCD,
                RHZ58SCN,
                RHZ58BOX,
                RHPSN,
                RHAN8,
                RHUPMJ,
                RHTDAY,
                RDMCU,
                RDZ58RID,
                RDMCU,
                RDKCOO,
                RDZ58SD,
                RDZ58PS,
                RDZ58STK,
                RDZ58ROW,
                RDZ58SL,
                RDITM,
                RDLOTN,
                RDZ58DST,
                RDTRDJ,
                RDZ58MDT,
                RDZ58PDT,
                RDPQOH,
                RDUOM1,
                RDZ58GWT,
                RDUWUM, 
                RDZ58SN,
                RDUPMJ,
                RDTDAY,
                A.DLDOCO,
                A.DLDCTO,
                A.DLLNID,
                A.DLDOC1,
                A.DLDOC2,
                A.DLXCTO,
                A.DLLNIC,
                A.DLPKCO,
                RDPID,
	            'PURA' AS TYPE
            FROM 
	            F580056 
	            INNER JOIN F580055 
	            ON RHZ58RID = RDZ58RID 
	            INNER JOIN (SELECT DLZ58RID, DLZ58SN, DLMCU, MAX(DLUKID) AS DLUKID FROM F580569 WHERE DLDOCO > 0 OR DLDOC1 > 0 OR DLDOC2 > 0 GROUP BY DLZ58RID, DLZ58SN, DLMCU) Z
	            ON Z.DLZ58RID = RDZ58RID AND Z.DLZ58SN = RDZ58SN AND Z.DLMCU = RDMCU
	            INNER JOIN F580569 A
	            ON A.DLUKID = Z.DLUKID
            WHERE 
	            RDMCU = '{0}' AND 
	            RDZ58DST IN ({1}) AND 
	            RDDOCO = 0 AND
	            RDDOC1 = 0 AND
	            RDDOC2 = 0 AND
	            RDPQOH > 0 AND
	            RDLOTN = '' AND
	            A.DLDOC2 > 0 AND
	            NOT EXISTS (SELECT 1 FROM F580569 WHERE DLPID = 'R41413' and DLZ58RID = RDZ58RID AND DLZ58SN = RDZ58SN AND DLMCU = RDMCU)
            ORDER BY 1,2,3", plantNum, statusClause);
            return cmd;
        }

        public Racks GetPurchasedRacks(BusinessSystemSession e1Session, string plantCode = null, bool attributes = false)
        {
            var racks = new Racks();
            try
            {
                var plantNum = PlantManager.GetPlant(plantCode).PlantNum.PadLeft(12);
                decimal prevRackId = 0;
                var cmd = GetPurchasedRacks(plantNum);
                cmd.Parameters[0].Value = plantNum;
                cmd.Connection = e1Session.DbConnection;
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
                while (reader.Read())
                {
                    var rackId = CastDbNull.To<decimal>(reader["RHZ58RID"], 0);
                    if (rackId != prevRackId)
                    {
                        AddRack(e1Session, racks, reader);
                    }
                    else
                    {
                        AddRackItem(e1Session, racks[racks.Count - 1], reader);
                    }
                    prevRackId = rackId;
                }
                reader.Close();

                AddRackItemAttributes(e1Session, racks);
                AddRackReceiving(e1Session, racks);
            }
            catch (Exception err)
            {
                throw new ApplicationException(err.ToString());
            }
            return racks;
        }

        public Racks GetRacks(BusinessSystemSession e1Session, decimal rackId1, string serialNum = null, string plantCode = null, string glCategory = null, double days = 0, bool attributes = false)
        {
            var racks = new Racks();
            try
            {
                var plantNum = PlantManager.GetPlant(plantCode).PlantNum.PadLeft(12);
                var cmd = GetRacks(plantNum, "'7'");
                cmd.Connection = e1Session.DbConnection;
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
                decimal prevRackId = 0;
                while (reader.Read())
                {
                    var rackId = CastDbNull.To<decimal>(reader["RHZ58RID"], 0);
                    if (rackId != prevRackId)
                    {
                        AddRack(e1Session, racks, reader);
                    }
                    else
                    {
                        AddRackItem(e1Session, racks[racks.Count - 1], reader);
                    }
                    prevRackId = rackId;
                }
                reader.Close();
                AddRackItemAttributes(e1Session, racks);
                //AddRackReceiving(e1Session, racks);
            }
            catch (Exception err)
            {
                throw new ApplicationException(err.ToString());
            }
            return racks;
        }

        public void AddRack(BusinessSystemSession e1Session, Racks racks, IDataReader reader)
        {
            try
            {
                var rack = new Rack();
                rack.RackId = CastDbNull.To<decimal>(reader["RHZ58RID"], 0);
                rack.CostCenter = CastDbNull.To(reader["RHMCU"], string.Empty).Trim();
                rack.Location = CastDbNull.To(reader["RHLOCN"], string.Empty).Trim();
                rack.RackType = CastDbNull.To(reader["RHZ58RT"], string.Empty).Trim();
                rack.HeaderStatus = CastDbNull.To(reader["RHZ58HST"], string.Empty).Trim();
                var statusDateId = CastDbNull.To<decimal>(reader["RHZ58RSDT"], 0);
                rack.HeaderStatusDate = Utilities.Common.CastToDateTime(statusDateId);
                rack.GlassWeight = Math.Round(CastDbNull.To<decimal>(reader["RHZ58GWTT"], 0), 4);
                rack.TareWeight = Math.Round(CastDbNull.To<decimal>(reader["RHZ58RTWT"], 0), 4);
                rack.GrossWeight = CastDbNull.To<decimal>(reader["RHGWEI"], 0);
                rack.ReasonCode = CastDbNull.To(reader["RHZ58RCD"], string.Empty).Trim();
                rack.LoadNum = CastDbNull.To<decimal>(reader["RHPSN"], 0);
                rack.ScheduleNum = CastDbNull.To<decimal>(reader["RHZ58SCN"], 0);
                rack.BoxNum = CastDbNull.To<decimal>(reader["RHZ58BOX"], 0);
                rack.SupplierNum = CastDbNull.To<decimal>(reader["SUPPAN8"], 0);
                var customerNum =  CastDbNull.To<decimal>(reader["CUSTAN8"], 0);
                rack.Customer = _customerMgr.GetCustomer(e1Session, customerNum);
                var updateDateId = CastDbNull.To<decimal>(reader["RHUPMJ"], 0);
                var updateTimeId = CastDbNull.To<decimal>(reader["RHTDAY"], 0);
                rack.UpdateDateTime = Utilities.Common.CastToDateTime(updateDateId, updateTimeId);
                AddRackItem(e1Session, rack, reader);
                racks.Add(rack);
            }
            catch (Exception err)
            {
                throw new ApplicationException(err.ToString());
            }
        }

        public void AddRackItem(BusinessSystemSession e1Session, Rack rack, IDataReader reader)
        {
            try
            {
                var rackItem = new RackItem();
                rackItem.RackId = rack.RackId;
                rackItem.SerialNum = CastDbNull.To(reader["RDZ58SN"], string.Empty).Trim();
                rackItem.ItemNum = CastDbNull.To<decimal>(reader["RDITM"], 0);
                rackItem.GLCategory = CastDbNull.To(reader["IBGLPT"], string.Empty).Trim();
                rackItem.PlantNum = CastDbNull.To(reader["RDMCU"], string.Empty).Trim();
                rackItem.Side = CastDbNull.To(reader["RDZ58SD"], string.Empty).Trim();
                rackItem.Position = CastDbNull.To<decimal>(reader["RDZ58PS"], 0);
                rackItem.StackTier = CastDbNull.To<decimal>(reader["RDZ58STK"], 0);
                rackItem.Row = CastDbNull.To<decimal>(reader["RDZ58ROW"], 0);
                rackItem.Slot = CastDbNull.To<decimal>(reader["RDZ58SL"], 0);
                rackItem.LotNum = CastDbNull.To(reader["RDLOTN"], string.Empty).Trim();
                rackItem.DetailStatus = CastDbNull.To(reader["RDZ58DST"], string.Empty).Trim();
                var transactionDateId = CastDbNull.To<decimal>(reader["RDTRDJ"], 0);
                rackItem.TransactionDate = Utilities.Common.CastToDateTime(transactionDateId);
                var manufacturedDateId = CastDbNull.To<decimal>(reader["RDZ58MDT"], 0);
                rackItem.ManufacturedDate = Utilities.Common.CastToDateTime(manufacturedDateId);
                //rackItem.ManufacturedPlant = CastDbNull.To<String>(reader["WAMCU"], String.Empty).Trim();
                var productionDateId = CastDbNull.To<decimal>(reader["RDZ58PDT"], 0);
                rackItem.ProductionDate = Utilities.Common.CastToDateTime(productionDateId);
                var updateDateId = CastDbNull.To<decimal>(reader["RDUPMJ"], 0);
                var updateTimeId = CastDbNull.To<decimal>(reader["RDTDAY"], 0);
                rackItem.UpdateDateTime = Utilities.Common.CastToDateTime(updateDateId, updateTimeId);
                rackItem.Quantity = CastDbNull.To<decimal>(reader["RDPQOH"], 0) / 1000;
                rackItem.QuantityUom = CastDbNull.To(reader["RDUOM1"], string.Empty).Trim();
                rackItem.GlassWeight = CastDbNull.To<decimal>(reader["RDZ58GWT"], 0) / 100;
                rackItem.GlassWeightUom = CastDbNull.To(reader["RDUWUM"], string.Empty).Trim();
                rackItem.ProcessId = CastDbNull.To(reader["RDPID"], string.Empty).Trim();
                rackItem.WorkOrderNum = CastDbNull.To<decimal>(reader["RDDOC1"], 0);
                //rackItem.WorkOrderPlantNum = CastDbNull.To<String>(reader["WAMCU"], String.Empty).Trim();
                rackItem.SalesOrderNum = CastDbNull.To<decimal>(reader["RDDOCO"], 0);
                rackItem.SalesOrderType = CastDbNull.To(reader["RDDCTO"], string.Empty).Trim();
                rackItem.SalesOrderLineNum = CastDbNull.To<decimal>(reader["RDLNID"], 0) / 1000;
                rackItem.SalesOrderCompany = CastDbNull.To(reader["RDKCOO"], string.Empty).Trim();
                rackItem.PurchaseOrderNum = CastDbNull.To<decimal>(reader["RDDOC2"], 0);
                rackItem.PurchaseOrderType = CastDbNull.To(reader["RDXCTO"], string.Empty).Trim();
                rackItem.PurchaseOrderLineNum = CastDbNull.To<decimal>(reader["RDLNIC"], 0) / 1000;
                rackItem.PurchaseOrderCompany = CastDbNull.To(reader["RDPKCO"], string.Empty).Trim();

                // ADD TO COLLECTION
                if (rack.RackItems == null)
                {
                    rack.RackItems = new RackItems();
                    rack.RackItems.Add(rackItem);
                }
                else
                {
                    rack.RackItems.Add(rackItem);
                }
            }
            catch (Exception err)
            {
                throw new ApplicationException(err.ToString());
            }
        }

        public void AddRackItemAttributes(BusinessSystemSession e1Session, Racks racks)
        {
            foreach (Rack rack in racks)
            {
                foreach (RackItem rackItem in rack.RackItems)
                {
                    if (rackItem.LotNum == "")
                    {
                        rackItem.GlassItem = _glassItemMgr.GetGlassItem(e1Session, rackItem.ItemNum, rackItem.PlantNum);
                    }
                    else
                    {
                        if (rackItem.PurchaseOrderNum > 0)
                        {
                            rackItem.GlassItem = _glassItemMgr.GetGlassItem(e1Session, rackItem.ItemNum, rackItem.PlantNum,
                                                                            rackItem.PurchaseOrderNum, rackItem.PurchaseOrderType, rackItem.PurchaseOrderLineNum * 1000, rackItem.PurchaseOrderCompany);
                        }
                        else
                        {
                            if (rackItem.SalesOrderNum > 0)
                            {
                                rackItem.GlassItem = _glassItemMgr.GetGlassItem(e1Session, rackItem.ItemNum,
                                                                                rackItem.PlantNum,
                                                                                rackItem.SalesOrderNum, rackItem.SalesOrderType, rackItem.SalesOrderLineNum * 1000, rackItem.SalesOrderCompany);
                            }
                            else
                            {
                                if (rackItem.WorkOrderNum > 0)
                                {
                                    rackItem.GlassItem = _glassItemMgr.GetGlassItem(e1Session, rackItem.ItemNum,
                                                                                    rackItem.PlantNum, rackItem.WorkOrderNum, "WO", 0,
                                                                                    "*");
                                }
                            }
                        }
                    }
                    try
                    {
                        rackItem.HasGlassAttributes = (rackItem.GlassItem.ItemAttributes.Count > 0);
                    }
                    catch (Exception)
                    {
                        rackItem.HasGlassAttributes = false;
                    }
                }
            }
        }

        public iDB2Command GetRackItemReceiving()
        {
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText =
            @"SELECT 
                DLUPMJ,
                DLTDAY,
                DLPID,
                DLUSER,
                DLPQOH,
                DLDOC1,
                PHAN8        
            FROM 
	            F580569
                INNER JOIN F4301
                ON  PHDOCO = DLDOC2 AND
                    PHDCTO = DLXCTO AND
                    PHKCOO = DLPKCO AND
                    DLDOC2 > 0
            WHERE 
                DLZ58RID = ? AND
                DLZ58SN = ? AND
                DLMCU = ? AND
                (
                    (DLPID IN ('N580060') AND DLZ58DST = '8' AND DLPSN > 0) OR
                    (DLPID IN ('P4312') AND DLPSN = 0) OR
                    (DLPID IN ('R47071') AND DLZ58DST = '4' AND DLPSN = 0)
                )";
            cmd.Parameters.Add(new iDB2Parameter("RackId", iDB2DbType.iDB2Decimal));
            cmd.Parameters["RackId"].Precision = 15;
            cmd.Parameters["RackId"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("SerialNum", iDB2DbType.iDB2Graphic, 30));
            cmd.Parameters.Add(new iDB2Parameter("PlantNum", iDB2DbType.iDB2Graphic, 12));
            return cmd;
        }

        public void AddRackReceiving(BusinessSystemSession e1Session, Racks racks)
        {
            try
            {
                foreach (Rack rack in racks)
                {
                    decimal tot = 0;
                    foreach (RackItem rackItem in rack.RackItems)
                    {
                        if (rackItem.PurchaseOrderNum > 0)
                        {
                            var cmd = (e1Session.DbCommands.ContainsKey("RackManger.GetRackItemReceiving"))
                                          ? e1Session.DbCommands["RackManger.GetRackItemReceiving"]
                                          : GetRackItemReceiving();
                            cmd.Parameters[0].Value = rack.RackId;
                            cmd.Parameters[1].Value = rackItem.SerialNum;
                            cmd.Parameters[2].Value = rackItem.PlantNum.PadLeft(12);
                            cmd.Connection = e1Session.DbConnection;
                            IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
                            while (reader.Read())
                            {
                                var recdDateId = CastDbNull.To<decimal>(reader["DLUPMJ"], 0);
                                rackItem.ReceivedDate = Utilities.Common.CastToDateTime(recdDateId);
                                rackItem.SupplierNum = CastDbNull.To<decimal>(reader["PHAN8"], 0);
                                rackItem.ReceivedQty = CastDbNull.To<decimal>(reader["DLPQOH"], 0)/1000;
                                rack.ReceivedDate = rackItem.ReceivedDate;
                                rack.SupplierNum = rackItem.SupplierNum;
                                tot = tot + rackItem.ReceivedQty;
                            }
                            reader.Close();
                        }
                    }
                    rack.ReceivedQty = tot;
                }
            }
            catch (Exception err)
            {
                throw new ApplicationException(err.ToString());
            }
        }



    }
}





