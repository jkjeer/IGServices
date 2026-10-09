using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.ServiceModel;
using IBM.Data.DB2.iSeries;
using IGServices.Common.Types.E1;
using IGServices.Common.Types.Response;
using IGServices.Common.Utilities;

namespace IGServices.Common.Managers.E1
{
    public class F580022Manager
    {
        public F580022Manager(BusinessSystemSession e1Session)
        {
            var hash = new Dictionary<string, iDB2Command>
            {
                {"F580022Manager.Insert", Insert()}
            };
            foreach (var keyName in hash.Keys.Where(keyName => !e1Session.DbCommands.ContainsKey(keyName)))
            {
                e1Session.DbCommands.Add(keyName, hash[keyName]);
            }
        }

        /*********************************************************************************
         * INSERT
         ********************************************************************************/

        public iDB2Command Insert()
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = @"
                INSERT INTO F580022 (
                CAEDUS,
                CAEDBT,
                CAEDTN,
                CAEDLN,
                CAEDCT,
                CATYTN,
                CAEDFT,
                CAEDDT,
                CADRIN,
                CAEDDL,
                CAEDSP,
                CAPNID,
                CATNAC,
                CAYST,
                CAMMCU,
                CADOCO,
                CADCTO,
                CALOTN,
                CAMCU,
                CAZ58RT,
                CAFRMPAK,
                CA75TLID,
                CAANPA,
                CASFT,
                CASOQS,
                CAUOM,
                CAZ58SCN,
                CAURCD,
                CAURDT,
                CAURAT,
                CAURAB,
                CAURRF,
                CAUSER,
                CAPID,
                CAJOBN,
                CAUPMJ,
                CATDAY,
                CAZ58IP01)
                VALUES(
                ?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,
                ?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?)"
            };
            cmd.Parameters.Add(new iDB2Parameter("CAEDUS", iDB2DbType.iDB2Graphic, 10));
            cmd.Parameters.Add(new iDB2Parameter("CAEDBT", iDB2DbType.iDB2Graphic, 15));
            cmd.Parameters.Add(new iDB2Parameter("CAEDTN", iDB2DbType.iDB2Graphic, 22));
            cmd.Parameters.Add(new iDB2Parameter("CAEDLN", iDB2DbType.iDB2Decimal, 7));
            cmd.Parameters["CAEDLN"].Precision = 7;
            cmd.Parameters["CAEDLN"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("CAEDCT", iDB2DbType.iDB2Graphic, 2));
            cmd.Parameters.Add(new iDB2Parameter("CATYTN", iDB2DbType.iDB2Graphic, 8));
            cmd.Parameters.Add(new iDB2Parameter("CAEDFT", iDB2DbType.iDB2Graphic, 10));
            cmd.Parameters.Add(new iDB2Parameter("CAEDDT", iDB2DbType.iDB2Decimal, 6));
            cmd.Parameters["CAEDDT"].Precision = 6;
            cmd.Parameters["CAEDDT"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("CADRIN", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("CAEDDL", iDB2DbType.iDB2Decimal, 5));
            cmd.Parameters["CAEDDL"].Precision = 5;
            cmd.Parameters["CAEDDL"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("CAEDSP", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("CAPNID", iDB2DbType.iDB2Graphic, 15));
            cmd.Parameters.Add(new iDB2Parameter("CATNAC", iDB2DbType.iDB2Graphic, 2));
            cmd.Parameters.Add(new iDB2Parameter("CAYST", iDB2DbType.iDB2Graphic, 1));
            cmd.Parameters.Add(new iDB2Parameter("CAMMCU", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("CADOCO", iDB2DbType.iDB2Decimal, 8));
            cmd.Parameters["CADOCO"].Precision = 8;
            cmd.Parameters["CADOCO"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("CADCTO", iDB2DbType.iDB2Graphic, 2));
            cmd.Parameters.Add(new iDB2Parameter("CALOTN", iDB2DbType.iDB2Graphic, 30));
            cmd.Parameters.Add(new iDB2Parameter("CAMCU", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("CAZ58RT", iDB2DbType.iDB2Graphic, 5));
            cmd.Parameters.Add(new iDB2Parameter("CAFRMPAK", iDB2DbType.iDB2Decimal, 3));
            cmd.Parameters["CAFRMPAK"].Precision = 18;
            cmd.Parameters["CAFRMPAK"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("CA75TLID", iDB2DbType.iDB2Decimal, 3));
            cmd.Parameters["CA75TLID"].Precision = 3;
            cmd.Parameters["CA75TLID"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("CAANPA", iDB2DbType.iDB2Decimal, 8));
            cmd.Parameters["CAANPA"].Precision = 8;
            cmd.Parameters["CAANPA"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("CASFT", iDB2DbType.iDB2Graphic, 5));
            cmd.Parameters.Add(new iDB2Parameter("CASOQS", iDB2DbType.iDB2Decimal, 15));
            cmd.Parameters["CASOQS"].Precision = 15;
            cmd.Parameters["CASOQS"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("CAUOM", iDB2DbType.iDB2Graphic, 2));
            cmd.Parameters.Add(new iDB2Parameter("CAZ58SCN", iDB2DbType.iDB2Decimal, 5));
            cmd.Parameters["CAZ58SCN"].Precision = 5;
            cmd.Parameters["CAZ58SCN"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("CAURCD", iDB2DbType.iDB2Graphic, 2));
            cmd.Parameters.Add(new iDB2Parameter("CAURDT", iDB2DbType.iDB2Decimal, 6));
            cmd.Parameters["CAURDT"].Precision = 6;
            cmd.Parameters["CAURDT"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("CAURAT", iDB2DbType.iDB2Decimal, 15));
            cmd.Parameters["CAURAT"].Precision = 15;
            cmd.Parameters["CAURAT"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("CAURAB", iDB2DbType.iDB2Decimal, 8));
            cmd.Parameters["CAURAB"].Precision = 8;
            cmd.Parameters["CAURAB"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("CAURRF", iDB2DbType.iDB2Graphic, 15));
            cmd.Parameters.Add(new iDB2Parameter("CAUSER", iDB2DbType.iDB2Graphic, 10));
            cmd.Parameters.Add(new iDB2Parameter("CAPID", iDB2DbType.iDB2Graphic, 10));
            cmd.Parameters.Add(new iDB2Parameter("CAJOBN", iDB2DbType.iDB2Graphic, 10));
            cmd.Parameters.Add(new iDB2Parameter("CAUPMJ", iDB2DbType.iDB2Decimal, 6));
            cmd.Parameters["CAUPMJ"].Precision = 6;
            cmd.Parameters["CAUPMJ"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("CATDAY", iDB2DbType.iDB2Decimal, 6));
            cmd.Parameters["CATDAY"].Precision = 6;
            cmd.Parameters["CATDAY"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("CAZ58IP01", iDB2DbType.iDB2Graphic, 15));
            return cmd;
        }

        public SqlDmlResponse Save(BusinessSystemSession session, iDB2Transaction txn, F580022 rec)
        {
            var response = new SqlDmlResponse();
            var cmd = (session.DbCommands.ContainsKey("F580030Manager.Insert"))
                ? session.DbCommands["F580030Manager.Insert"]
                : Insert();
            cmd.Connection = session.DbConnection;
            cmd.Transaction = txn;
            try
            {
                cmd.Parameters["CAEDUS"].Value = rec.CAEDUS;
                cmd.Parameters["CAEDBT"].Value = rec.CAEDBT;
                cmd.Parameters["CAEDTN"].Value = rec.CAEDTN;
                cmd.Parameters["CAEDLN"].Value = rec.CAEDLN;
                cmd.Parameters["CAEDCT"].Value = rec.CAEDCT;
                cmd.Parameters["CATYTN"].Value = rec.CATYTN;
                cmd.Parameters["CAEDFT"].Value = rec.CAEDFT;
                cmd.Parameters["CAEDDT"].Value = rec.CAEDDT;
                cmd.Parameters["CADRIN"].Value = rec.CADRIN;
                cmd.Parameters["CAEDDL"].Value = rec.CAEDDL;
                cmd.Parameters["CAEDSP"].Value = rec.CAEDSP;
                cmd.Parameters["CAPNID"].Value = rec.CAPNID;
                cmd.Parameters["CATNAC"].Value = rec.CATNAC;
                cmd.Parameters["CAYST"].Value = rec.CAYST;
                cmd.Parameters["CAMMCU"].Value = rec.CAMMCU;
                cmd.Parameters["CADOCO"].Value = rec.CADOCO;
                cmd.Parameters["CADCTO"].Value = rec.CADCTO;
                cmd.Parameters["CALOTN"].Value = rec.CALOTN;
                cmd.Parameters["CAMCU"].Value = rec.CAMCU;
                cmd.Parameters["CAZ58RT"].Value = rec.CAZ58RT;
                cmd.Parameters["CAFRMPAK"].Value = rec.CAFRMPAK;
                cmd.Parameters["CA75TLID"].Value = rec.CA75TLID;
                cmd.Parameters["CAANPA"].Value = rec.CAANPA;
                cmd.Parameters["CASFT"].Value = rec.CASFT;
                cmd.Parameters["CASOQS"].Value = rec.CASOQS;
                cmd.Parameters["CAUOM"].Value = rec.CAUOM;
                cmd.Parameters["CAZ58SCN"].Value = rec.CAZ58SCN;
                cmd.Parameters["CAURCD"].Value = rec.CAURCD;
                cmd.Parameters["CAURDT"].Value = rec.CAURDT;
                cmd.Parameters["CAURAT"].Value = rec.CAURAT;
                cmd.Parameters["CAURAB"].Value = rec.CAURAB;
                cmd.Parameters["CAURRF"].Value = rec.CAURRF;
                cmd.Parameters["CAUSER"].Value = rec.CAUSER;
                cmd.Parameters["CAPID"].Value = rec.CAPID;
                cmd.Parameters["CAJOBN"].Value = rec.CAJOBN;
                cmd.Parameters["CAUPMJ"].Value = rec.CAUPMJ;
                cmd.Parameters["CATDAY"].Value = rec.CATDAY;
                cmd.Parameters["CAZ58IP01"].Value = rec.CAZ58IP01;
                response = new SqlDmlResponse();
                response.Init(cmd);
                int rows = cmd.ExecuteNonQuery();
                response.RowsAffected = rows;
                response.EndTime = DateTime.Now.ToLocalTime();
                txn.Commit();
            }
            catch (Exception e)
            {
                txn.Rollback();
                throw new Exception(e.Message);
            }
            return response;
        }

        public SqlDmlResponses Save(BusinessSystemSession session, iDB2Transaction txn, F580022s recs)
        {
            var responses = new SqlDmlResponses();
            var cmd = session.DbCommands["F580022Manager.Insert"];
            cmd.Connection = session.DbConnection;
            cmd.Transaction = txn;
            try
            {
                foreach (F580022 rec in recs)
                {
                    var response = new SqlDmlResponse();
                    try
                    {
                        cmd.Parameters["CAEDUS"].Value = rec.CAEDUS;
                        cmd.Parameters["CAEDBT"].Value = rec.CAEDBT;
                        cmd.Parameters["CAEDTN"].Value = rec.CAEDTN;
                        cmd.Parameters["CAEDLN"].Value = rec.CAEDLN;
                        cmd.Parameters["CAEDCT"].Value = rec.CAEDCT;
                        cmd.Parameters["CATYTN"].Value = rec.CATYTN;
                        cmd.Parameters["CAEDFT"].Value = rec.CAEDFT;
                        cmd.Parameters["CAEDDT"].Value = rec.CAEDDT;
                        cmd.Parameters["CADRIN"].Value = rec.CADRIN;
                        cmd.Parameters["CAEDDL"].Value = rec.CAEDDL;
                        cmd.Parameters["CAEDSP"].Value = rec.CAEDSP;
                        cmd.Parameters["CAPNID"].Value = rec.CAPNID;
                        cmd.Parameters["CATNAC"].Value = rec.CATNAC;
                        cmd.Parameters["CAYST"].Value = rec.CAYST;
                        cmd.Parameters["CAMMCU"].Value = rec.CAMMCU;
                        cmd.Parameters["CADOCO"].Value = rec.CADOCO;
                        cmd.Parameters["CADCTO"].Value = rec.CADCTO;
                        cmd.Parameters["CALOTN"].Value = rec.CALOTN;
                        cmd.Parameters["CAMCU"].Value = rec.CAMCU;
                        cmd.Parameters["CAZ58RT"].Value = rec.CAZ58RT;
                        cmd.Parameters["CAFRMPAK"].Value = rec.CAFRMPAK;
                        cmd.Parameters["CA75TLID"].Value = rec.CA75TLID;
                        cmd.Parameters["CAANPA"].Value = rec.CAANPA;
                        cmd.Parameters["CASFT"].Value = rec.CASFT;
                        cmd.Parameters["CASOQS"].Value = rec.CASOQS;
                        cmd.Parameters["CAUOM"].Value = rec.CAUOM;
                        cmd.Parameters["CAZ58SCN"].Value = rec.CAZ58SCN;
                        cmd.Parameters["CAURCD"].Value = rec.CAURCD;
                        cmd.Parameters["CAURDT"].Value = rec.CAURDT;
                        cmd.Parameters["CAURAT"].Value = rec.CAURAT;
                        cmd.Parameters["CAURAB"].Value = rec.CAURAB;
                        cmd.Parameters["CAURRF"].Value = rec.CAURRF;
                        cmd.Parameters["CAUSER"].Value = rec.CAUSER;
                        cmd.Parameters["CAPID"].Value = rec.CAPID;
                        cmd.Parameters["CAJOBN"].Value = rec.CAJOBN;
                        cmd.Parameters["CAUPMJ"].Value = rec.CAUPMJ;
                        cmd.Parameters["CATDAY"].Value = rec.CATDAY;
                        cmd.Parameters["CAZ58IP01"].Value = rec.CAZ58IP01;
                        response.Init(cmd);
                        int rows = cmd.ExecuteNonQuery();
                        response.RowsAffected = rows;
                        response.EndTime = DateTime.Now.ToLocalTime();
                        responses.Add(response);
                    }
                    catch (Exception e)
                    {
                        response.Message = e.Message;
                        throw new Exception($"CADOCO: {rec.CADOCO} TRACE: {e.Message}");
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return responses;
        }


    }
}
