// Decompiled with JetBrains decompiler
// Type: IGServices.Common.Managers.E1.ItemAttributeManager
// Assembly: IGServices, Version=0.1.7340.16006, Culture=neutral, PublicKeyToken=null
// MVID: DC8EC0DC-64DA-4A3E-B3DA-A4691D2D5367
// Assembly location: \\cgd1intsvcpd\c$\inetpub\wwwroot\IGServices\bin\IGServices.dll

using IBM.Data.DB2.iSeries;
using IGServices.Common.Types.E1;
using IGServices.Common.Types.Enterprise;
using IGServices.Common.Utilities;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;

namespace IGServices.Common.Managers.E1
{
  public class ItemAttributeManager
  {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public ItemAttributeManager(BusinessSystemSession session)
    {
      Dictionary<string, iDB2Command> dictionary = new Dictionary<string, iDB2Command>()
      {
        {
          "ItemAttributeManager.GetDiscreteItemAttributes",
          this.GetDiscreteItemAttributes()
        },
        {
          "ItemAttributeManager.GetConfiguredItemAttributes",
          this.GetConfiguredItemAttributes()
        },
        {
          "ItemAttributeManager.GetConfiguredItemAttributesById",
          this.GetConfiguredItemAttributesById()
        },
        {
          "ItemAttributeManager.GetArchivedItemAttributesById",
          this.GetArchivedItemAttributesById()
        }
      };
      foreach (string key in dictionary.Keys.Where<string>((Func<string, bool>) (keyName => !session.DbCommands.ContainsKey(keyName))))
        session.DbCommands.Add(key, dictionary[key]);
    }

    public iDB2Command GetDiscreteItemAttributes()
    {
      iDB2Command iDb2Command1 = new iDB2Command();
      iDb2Command1.CommandType = CommandType.Text;
      iDb2Command1.CommandText = "\r\n                    SELECT\r\n\t                    IAZ58ATNM AS ATNM,\r\n\t                    IADVAL AS SGVL,\r\n                        A.DRDL01 AS DL01,\r\n                        A.DRDL02 AS DL02,\r\n                        A.DRSPHD AS SPHD\r\n                    FROM\r\n                        F580002\r\n                        LEFT OUTER JOIN " + ConfigurationManager.AppSettings["ControlLib"] + ".F0005 A\r\n                        ON A.DRSY = IASY AND\r\n                            A.DRRT = IART AND\r\n                            TRIM(A.DRKY) = IADVAL  \r\n                    WHERE\r\n\t                    IAITM = ?";
      iDB2Command iDb2Command2 = iDb2Command1;
      iDb2Command2.Parameters.Add(new iDB2Parameter("ItemNum", iDB2DbType.iDB2Decimal));
      iDb2Command2.Parameters["ItemNum"].Precision = (byte) 8;
      iDb2Command2.Parameters["ItemNum"].Scale = (byte) 0;
      return iDb2Command2;
    }

    public iDB2Command GetConfiguredItemAttributes()
    {
      iDB2Command iDb2Command1 = new iDB2Command();
      iDb2Command1.CommandType = CommandType.Text;
      iDb2Command1.CommandText = "\r\n                    SELECT\r\n\t                    C.AMZ58ATNM AS ATNM,\r\n                        A.KZSGVL AS SGVL,\r\n                        U.DRDL01 AS DL01,\r\n                        U.DRDL02 AS DL02,\r\n                        U.DRSPHD AS SPHD\r\n                    FROM\r\n                        F3201 CM\r\n                        INNER JOIN F3211 A\r\n                        ON CM.CMCFGID = A.KZCFGID\r\n                        INNER JOIN ( SELECT\r\n                                    KZCFGID ,\r\n                                    KZPEL ,\r\n                                    MAX(KZCFGCID) AS KZCFGCID\r\n                                    FROM\r\n                                    F3211\r\n                                    GROUP BY\r\n                                    KZCFGID ,\r\n                                    KZPEL\r\n                                    ) B\r\n                        ON B.KZCFGID = A.KZCFGID AND\r\n                            A.KZPEL = B.KZPEL AND\r\n                            A.KZCFGCID = B.KZCFGCID\r\n                        INNER JOIN F580001 C\r\n                        ON C.AMPEL = A.KZPEL  \r\n                        LEFT OUTER JOIN " + ConfigurationManager.AppSettings["ControlLib"] + ".F0005 U\r\n                        ON U.DRSY = C.AMSY AND\r\n                            U.DRRT = C.AMRT AND\r\n                            TRIM(U.DRKY) = A.KZSGVL  \r\n                    WHERE\r\n                        A.KZSGVL <> '' AND\r\n                        NOT (C.AMCNUM = 'Y' and A.KZSGVL IN ('0', '0.0', '0.00', '0.000','0.0000','.0','.00','.000','.0000')) AND\r\n                        CM.CMKCOO = ? AND\r\n                        CM.CMDOCO = ? AND\r\n                        CM.CMDCTO = ? AND\r\n                        CM.CMLNID = ?";
      iDB2Command iDb2Command2 = iDb2Command1;
      iDb2Command2.Parameters.Add(new iDB2Parameter("CompanyNum", iDB2DbType.iDB2Graphic, 5));
      iDb2Command2.Parameters.Add(new iDB2Parameter("OrderNum", iDB2DbType.iDB2Decimal));
      iDb2Command2.Parameters["OrderNum"].Precision = (byte) 8;
      iDb2Command2.Parameters["OrderNum"].Scale = (byte) 0;
      iDb2Command2.Parameters.Add(new iDB2Parameter("OrderType", iDB2DbType.iDB2Graphic, 2));
      iDb2Command2.Parameters.Add(new iDB2Parameter("OrderLineNum", iDB2DbType.iDB2Decimal));
      iDb2Command2.Parameters["OrderLineNum"].Precision = (byte) 6;
      iDb2Command2.Parameters["OrderLineNum"].Scale = (byte) 0;
      return iDb2Command2;
    }

    public iDB2Command GetConfiguredItemAttributesById()
    {
      iDB2Command iDb2Command = new iDB2Command();
      iDb2Command.CommandType = CommandType.Text;
      iDb2Command.CommandText = string.Format("\r\n\t        SELECT\r\n\t\t        A.AMZ58ATNM AS ATNM,\r\n\t\t        A.AMSY,\r\n\t\t        A.AMRT,\r\n\t\t        A.KZSGVL AS SGVL,\r\n                U1.DRDL01 AS DL01,\r\n                U1.DRDL02 AS DL02,\r\n                U1.DRSPHD AS SPHD\r\n            FROM (\r\n                SELECT\r\n                    KZPEL,\r\n\t\t            KZSGVL,\r\n                    AMZ58ATNM,\r\n                    AMSY,\r\n                    AMRT,\r\n                    ROW_NUMBER() OVER (PARTITION BY KZPEL ORDER BY KZCFGCID DESC) AS RN\r\n\t            FROM\r\n\t\t            F3211\t\t    \r\n                    INNER JOIN F580001\r\n\t\t                ON AMPEL = KZPEL \r\n\r\n\t            WHERE\r\n\t\t            KZCFGID = ? AND\r\n\t\t            (KZSGVL != '' OR KZPEL IN (390,2070,3660)) AND\r\n\t\t            NOT (AMCNUM = 'Y' and KZSGVL IN ('0', '0.0', '0.00', '0.000','0.0000','.0','.00','.000','.0000'))\r\n                ) A\r\n            LEFT OUTER JOIN {0}.F0005 U1\r\n            ON  U1.DRSY = AMSY AND\r\n                U1.DRRT = AMRT AND\r\n                TRIM(U1.DRKY) = TRIM(A.KZSGVL)          \r\n            WHERE\r\n                A.RN = 1\r\n            ORDER BY 1\r\n            FOR READ ONLY", (object) ConfigurationManager.AppSettings["ControlLib"]);
      iDb2Command.Parameters.Add(new iDB2Parameter("CFGID", iDB2DbType.iDB2Decimal));
      iDb2Command.Parameters["CFGID"].Precision = (byte) 15;
      iDb2Command.Parameters["CFGID"].Scale = (byte) 0;
      return iDb2Command;
    }

    public iDB2Command GetArchivedItemAttributesById()
    {
      iDB2Command iDb2Command = new iDB2Command();
      iDb2Command.CommandType = CommandType.Text;
      iDb2Command.CommandText = string.Format("\r\n            SELECT \r\n\t            ATNM, \r\n\t            AMSY,\r\n\t            AMRT,\r\n\t            SGVL,\r\n                COALESCE(U1.DRDL01, U2.DRDL01) AS DL01,\r\n                COALESCE(U1.DRDL02, U2.DRDL02) AS DL02,\r\n                COALESCE(U1.DRSPHD, U2.DRSPHD) AS SPHD\r\n            FROM (\r\n\t            SELECT\r\n\t\t            C.AMZ58ATNM AS ATNM,\r\n\t\t            C.AMSY,\r\n\t\t            C.AMRT,\r\n\t\t            A.KZSGVL AS SGVL,\r\n\t\t            ROW_NUMBER() OVER (PARTITION BY A.KZCFGID, A.KZPEL ORDER BY A.KZCFGCID ASC) AS ROWNUM\r\n\t            FROM\r\n\t\t            F5832119 A\r\n\t\t            INNER JOIN F580001 C\r\n\t\t            ON C.AMPEL = A.KZPEL  \r\n\t            WHERE\r\n\t\t            A.KZCFGID = ? AND\r\n\t\t            (A.KZSGVL != '' OR KZPEL IN (390,2070,3660)) AND\r\n\t\t            NOT (A.KZCNUM = 'Y' and A.KZSGVL IN ('0', '0.0', '0.00', '0.000','0.0000','.0','.00','.000','.0000'))\r\n\t\t            ) A\r\n                LEFT OUTER JOIN {0}.F0005 U1\r\n                ON  U1.DRSY = A.AMSY AND\r\n                    U1.DRRT = A.AMRT AND\r\n                    CAST(U1.DRKY AS INT) = CAST(A.SGVL AS INT) AND\r\n                    A.AMSY = '57' AND\r\n                    A.AMRT = 'BR'\r\n                LEFT OUTER JOIN {0}.F0005 U2\r\n                ON  U2.DRSY = A.AMSY AND\r\n                    U2.DRRT = A.AMRT AND\r\n                    U2.DRKY = A.SGVL\r\n            WHERE \r\n\t            A.ROWNUM = 1 \r\n            ORDER BY 1", (object) ConfigurationManager.AppSettings["ControlLib"]);
      iDb2Command.Parameters.Add(new iDB2Parameter("CFGID", iDB2DbType.iDB2Decimal));
      iDb2Command.Parameters["CFGID"].Precision = (byte) 15;
      iDb2Command.Parameters["CFGID"].Scale = (byte) 0;
      return iDb2Command;
    }

    public ItemAttributes GetItemAttributes(
      BusinessSystemSession session,
      Decimal itemNum,
      Decimal configId)
    {
      ItemAttributes itemAttributes1 = new ItemAttributes();
      ItemAttributes itemAttributes2;
      try
      {
        itemAttributes2 = configId > Decimal.Zero ? this.GetConfiguredItemAttributes(session, configId) : this.GetDiscreteItemAttributes(session, itemNum);
      }
      catch (Exception ex)
      {
        throw new Exception(session.DbConnection.ConnectionString + "  " + ex.Message + ": " + ex.StackTrace);
      }
      return itemAttributes2;
    }

    public ItemAttributes GetItemAttributes(
      BusinessSystemSession session,
      Decimal itemNum,
      Decimal documentNum,
      string documentType,
      Decimal documentLine,
      string companyCode)
    {
      ItemAttributes itemAttributes1 = new ItemAttributes();
      ItemAttributes itemAttributes2;
      try
      {
        itemAttributes2 = documentNum > Decimal.Zero ? this.GetConfiguredItemAttributes(session, documentNum, documentType, documentLine, companyCode) : this.GetDiscreteItemAttributes(session, itemNum);
      }
      catch (Exception ex)
      {
        throw new Exception(session.DbConnection.ConnectionString + "  " + ex.Message + ": " + ex.StackTrace);
      }
      return itemAttributes2;
    }

    public ItemAttributes GetDiscreteItemAttributes(
      BusinessSystemSession session,
      Decimal itemNum)
    {
      ItemAttributes itemAttributes = new ItemAttributes();
      try
      {
        iDB2Command dbCommand = session.DbCommands["ItemAttributeManager.GetDiscreteItemAttributes"];
        dbCommand.Parameters["ItemNum"].Value = (object) itemNum;
        dbCommand.Connection = session.DbConnection;
        ItemAttributeManager.Logger.Trace(Logging.DbCmdString(dbCommand));
        IDataReader dataReader = (IDataReader) dbCommand.ExecuteReader(CommandBehavior.SingleResult);
        while (dataReader.Read())
        {
          ItemAttribute itemAttribute = new ItemAttribute()
          {
            Name = dataReader["ATNM"].ToString().Trim(),
            Value = dataReader["SGVL"].ToString().Trim(),
            Desc1 = dataReader["DL01"].ToString().Trim(),
            Desc2 = dataReader["DL02"].ToString().Trim(),
            Desc3 = dataReader["SPHD"].ToString().Trim()
          };
          if (!itemAttributes.Contains(itemAttribute.Name))
          {
              itemAttributes.Add(itemAttribute);
          }
        }
        dataReader.Close();
      }
      catch (Exception ex)
      {
        ItemAttributeManager.Logger.Error("ERROR: " + session.DbConnection.ConnectionString + "  " + ex.Message + ": " + ex.StackTrace);
        throw new Exception(session.DbConnection.ConnectionString + "  " + ex.Message + ": " + ex.StackTrace);
      }
      return itemAttributes;
    }

    public ItemAttributes GetConfiguredItemAttributes(
      BusinessSystemSession session,
      Decimal orderNum,
      string orderType,
      Decimal orderLineNum,
      string companyCode)
    {
      ItemAttributes itemAttributes = new ItemAttributes();
      try
      {
        iDB2Command cmd = session.DbCommands.ContainsKey("ItemAttributeManager.GetConfiguredItemAttributes") ? session.DbCommands["ItemAttributeManager.GetConfiguredItemAttributes"] : this.GetConfiguredItemAttributes();
        cmd.Parameters["CompanyNum"].Value = (object) companyCode;
        cmd.Parameters["OrderNum"].Value = (object) orderNum;
        cmd.Parameters["OrderType"].Value = (object) orderType;
        cmd.Parameters["OrderLineNum"].Value = (object) orderLineNum;
        cmd.Connection = session.DbConnection;
        ItemAttributeManager.Logger.Trace(Logging.DbCmdString(cmd));
        IDataReader dataReader = (IDataReader) cmd.ExecuteReader(CommandBehavior.SingleResult);
        while (dataReader.Read())
        {
          ItemAttribute itemAttribute = new ItemAttribute()
          {
            Name = dataReader["ATNM"].ToString().Trim(),
            Value = dataReader["SGVL"].ToString().Trim(),
            Desc1 = dataReader["DL01"].ToString().Trim(),
            Desc2 = dataReader["DL02"].ToString().Trim(),
            Desc3 = dataReader["SPHD"].ToString().Trim()
          };
          if (!itemAttributes.Contains(itemAttribute.Name))
          {
              itemAttributes.Add(itemAttribute);
          }
        }
                dataReader.Close();
        ItemAttributeManager.Logger.Trace("ROWS: " + (object) itemAttributes.Count);
      }
      catch (Exception ex)
      {
        ItemAttributeManager.Logger.Error("ERROR: " + session.DbConnection.ConnectionString + "  " + ex.Message + ": " + ex.StackTrace);
        throw new ApplicationException(session.DbConnection.ConnectionString + "  " + ex.Message + ": " + ex.StackTrace);
      }
      return itemAttributes;
    }

    public ItemAttributes GetConfiguredItemAttributes(
      BusinessSystemSession session,
      Decimal configId)
    {
      DateTime.Now.ToLocalTime();
      ItemAttributes itemAttributes = new ItemAttributes();
      try
      {
        iDB2Command cmd1 = session.DbCommands.ContainsKey("ItemAttributeManager.GetConfiguredItemAttributesById") ? session.DbCommands["ItemAttributeManager.GetConfiguredItemAttributesById"] : this.GetConfiguredItemAttributesById();
        cmd1.Parameters[0].Value = (object) configId;
        cmd1.Connection = session.DbConnection;
        ItemAttributeManager.Logger.Trace(Logging.DbCmdString(cmd1));
        IDataReader dataReader1 = (IDataReader) cmd1.ExecuteReader(CommandBehavior.SingleResult);
        while (dataReader1.Read())
        {
          ItemAttribute itemAttribute = new ItemAttribute()
          {
            Name = dataReader1["ATNM"].ToString().Trim(),
            Value = dataReader1["SGVL"].ToString().Trim(),
            Desc1 = dataReader1["DL01"].ToString().Trim(),
            Desc2 = dataReader1["DL02"].ToString().Trim(),
            Desc3 = dataReader1["SPHD"].ToString().Trim()
          };
          if (!itemAttributes.Contains(itemAttribute.Name))
          {
              itemAttributes.Add(itemAttribute);
          }
        }
                dataReader1.Close();
        if (itemAttributes.Count == 0)
        {
          iDB2Command cmd2 = session.DbCommands.ContainsKey("ItemAttributeManager.GetArchivedItemAttributesById") ? session.DbCommands["ItemAttributeManager.GetArchivedItemAttributesById"] : this.GetArchivedItemAttributesById();
          cmd2.Parameters[0].Value = (object) configId;
          cmd2.Connection = session.DbConnection;
          ItemAttributeManager.Logger.Trace(Logging.DbCmdString(cmd2));
          IDataReader dataReader2 = (IDataReader) cmd2.ExecuteReader(CommandBehavior.SingleResult);
          while (dataReader2.Read())
          {
            ItemAttribute itemAttribute = new ItemAttribute()
            {
              Name = dataReader2["ATNM"].ToString().Trim(),
              Value = dataReader2["SGVL"].ToString().Trim(),
              Desc1 = dataReader2["DL01"].ToString().Trim(),
              Desc2 = dataReader2["DL02"].ToString().Trim(),
              Desc3 = dataReader2["SPHD"].ToString().Trim()
            };
            if (!itemAttributes.Contains(itemAttribute.Name))
            {
                itemAttributes.Add(itemAttribute);
            }
          }
                    dataReader2.Close();
        }
      }
      catch (Exception ex)
      {
        ItemAttributeManager.Logger.Error("ERROR: " + session.DbConnection.ConnectionString + "  " + ex.Message + ": " + ex.StackTrace);
        throw new ApplicationException(session.DbConnection.ConnectionString + "  " + ex.Message + ": " + ex.StackTrace);
      }
      return itemAttributes;
    }

    public Decimal GetConfiguredId(
      BusinessSystemSession session,
      string kcoo,
      Decimal doco,
      string dcto,
      Decimal lnid)
    {
      Decimal num = new Decimal();
      try
      {
        iDB2Command cmd = session.DbCommands.ContainsKey("ItemAttributeManager.GetConfiguredId") ? session.DbCommands["ItemAttributeManager.GetConfiguredId"] : this.GetConfiguredId();
        cmd.Parameters[0].Value = (object) kcoo;
        cmd.Parameters[1].Value = (object) doco;
        cmd.Parameters[2].Value = (object) dcto;
        cmd.Parameters[3].Value = (object) lnid;
        cmd.Connection = session.DbConnection;
        ItemAttributeManager.Logger.Trace(Logging.DbCmdString(cmd));
        IDataReader dataReader = (IDataReader) cmd.ExecuteReader(CommandBehavior.SingleRow);
        while (dataReader.Read())
          num = CastDbNull.To<Decimal>(dataReader["CMCFGID"], Decimal.Zero);
        dataReader.Close();
      }
      catch (Exception ex)
      {
        throw new Exception(session.DbConnection.ConnectionString + "  " + ex.Message + ": " + ex.StackTrace);
      }
      return num;
    }

    public iDB2Command GetConfiguredId()
    {
      iDB2Command iDb2Command1 = new iDB2Command();
      iDb2Command1.CommandType = CommandType.Text;
      iDb2Command1.CommandText = "\r\n                    SELECT\r\n\t                    MAX(CMCFGID) AS CMCFGID\r\n                    FROM\r\n                        F3201\r\n                    WHERE\r\n\t                    CMKCOO = ? AND\r\n                        CMDOCO = ? AND\r\n                        CMDCTO = ? AND\r\n                        CMLNID = ?";
      iDB2Command iDb2Command2 = iDb2Command1;
      iDb2Command2.Parameters.Add(new iDB2Parameter("CompanyNum", iDB2DbType.iDB2Graphic, 5));
      iDb2Command2.Parameters.Add(new iDB2Parameter("OrderNum", iDB2DbType.iDB2Decimal));
      iDb2Command2.Parameters["OrderNum"].Precision = (byte) 8;
      iDb2Command2.Parameters["OrderNum"].Scale = (byte) 0;
      iDb2Command2.Parameters.Add(new iDB2Parameter("OrderType", iDB2DbType.iDB2Graphic, 2));
      iDb2Command2.Parameters.Add(new iDB2Parameter("OrderLineNum", iDB2DbType.iDB2Decimal));
      iDb2Command2.Parameters["OrderLineNum"].Precision = (byte) 6;
      iDb2Command2.Parameters["OrderLineNum"].Scale = (byte) 0;
      return iDb2Command2;
    }
  }
}
