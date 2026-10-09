// Decompiled with JetBrains decompiler
// Type: IGServices.Common.Managers.Enterprise.PartListManager
// Assembly: IGServices, Version=0.1.7340.16006, Culture=neutral, PublicKeyToken=null
// MVID: DC8EC0DC-64DA-4A3E-B3DA-A4691D2D5367
// Assembly location: \\cgd1intsvcpd\c$\inetpub\wwwroot\IGServices\bin\IGServices.dll

using IBM.Data.DB2.iSeries;
using IGServices.Common.Types.E1;
using IGServices.Common.Types.Enterprise;
using IGServices.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Data;

namespace IGServices.Common.Managers.Enterprise
{
  public class PartListManager
  {
    public PartListManager(BusinessSystemSession e1Session)
    {
      Dictionary<string, iDB2Command> dictionary = new Dictionary<string, iDB2Command>();
      dictionary.Add("PartListManager.GetGlassParts", this.GetGlassParts());
      foreach (string key in dictionary.Keys)
      {
        if (!e1Session.DbCommands.ContainsKey(key))
          e1Session.DbCommands.Add(key, dictionary[key]);
      }
    }

    public iDB2Command GetGlassParts()
    {
      iDB2Command iDb2Command = new iDB2Command();
      iDb2Command.CommandType = CommandType.Text;
      iDb2Command.CommandText = "SELECT\r\n                    WMDOCO,\r\n                    WMOPSQ,\r\n                    WMCPIT,\r\n                    WMDSC1,\r\n                    WMDSC2,\r\n                    WMLOCN,\r\n                    WMUORG,\r\n                    WMTRQT,\r\n                    WMUM,\r\n                    WMCPNB,\r\n                    WMMCU\r\n                FROM\r\n                    F3111\r\n                    INNER JOIN F4102 \r\n                    ON  IBITM = WMCPIT AND\r\n                        IBMCU = WMMCU\r\n                WHERE\r\n                    WMDOCO = ? AND\r\n                    IBGLPT IN ('GLAS','GLAF','GLAC','GLAX','GLAZ')";
      iDb2Command.Parameters.Add(new iDB2Parameter("DOCO", iDB2DbType.iDB2Decimal));
      iDb2Command.Parameters["DOCO"].Precision = (byte) 8;
      iDb2Command.Parameters["DOCO"].Scale = (byte) 0;
      return iDb2Command;
    }

    public GlassParts GetGlassParts(BusinessSystemSession e1Session, Decimal orderNum)
    {
      GlassParts glassParts = new GlassParts();
      try
      {
        iDB2Command dbCommand = e1Session.DbCommands["PartListManager.GetGlassParts"];
        dbCommand.Connection = e1Session.DbConnection;
        dbCommand.Parameters[0].Value = (object) orderNum;
        IDataReader dataReader = (IDataReader) dbCommand.ExecuteReader(CommandBehavior.SingleResult);
        while (dataReader.Read())
          glassParts.Add(new GlassPart()
          {
            ItemNum = CastDbNull.To<Decimal>(dataReader["WMCPIT"], Decimal.Zero),
            SequenceNum = CastDbNull.To<Decimal>(dataReader["WMOPSQ"], Decimal.Zero) / new Decimal(100),
            OrderQty = CastDbNull.To<Decimal>(dataReader["WMUORG"], Decimal.Zero) / new Decimal(1000),
            TargetQty = CastDbNull.To<Decimal>(dataReader["WMTRQT"], Decimal.Zero) / new Decimal(1000)
          });
        dataReader.Close();
      }
      catch (Exception ex)
      {
        throw new Exception(ex.Message);
      }
      return glassParts;
    }
  }
}
