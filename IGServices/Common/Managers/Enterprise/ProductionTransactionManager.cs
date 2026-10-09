using System;
using System.Collections.Generic;
using System.Data;
using IBM.Data.DB2.iSeries;
using IGServices.Common.Types.E1;
using IGServices.Common.Types.Enterprise;
using IGServices.Common.Utilities;

namespace IGServices.Common.Managers.Enterprise
{
    public class ProductionTransactionManager
    {
        public void LoadCommands(BusinessSystemSession e1Session)
        {
            var hash = new Dictionary<string, iDB2Command>
            {
                {"GetProductionTransactionRejectsByWorkOrder", GetProductionTransactionRejectsByWorkOrder()}
            };
            foreach (var keyName in hash.Keys)
            {
                if (!e1Session.DbCommands.ContainsKey(keyName))
                {
                    e1Session.DbCommands.Add(keyName, hash[keyName]);
                }
            }
        }

        public iDB2Command GetProductionTransactionRejectsByWorkOrder()
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = @"
                SELECT 
                    SFUKID,
                    SFMMCU, 
                    SFMCU,
                    MCDL01,
                    SFDOCO,
                    SFDCTO,
                    SFEDCT,
                    SFTYTN, 
                    SFSTTM, 
                    SFPNID, 
                    SFLOTN, 
                    SFZ58SN, 
                    SFZ58SCN,
                    SFUORG, 
                    SFSOCN, 
                    SFZ58RID, 
                    SFZ58UID, 
                    SFZ58RCP, 
                    SFZ58RCC, 
                    SFZ58L1CLN 
                FROM 
	                F580030
                    INNER JOIN F0006
                    ON MCMCU = SFMCU
                WHERE 
                    SFMMCU = ? AND
                    SFDOCO = ? AND
                    SFSOCN > 0"
            };
            cmd.Parameters.Add(new iDB2Parameter("PlantNum", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("OrderNum", iDB2DbType.iDB2Decimal));
            cmd.Parameters["OrderNum"].Precision = 8;
            cmd.Parameters["OrderNum"].Scale = 0;
            return cmd;
        }

        public ProductionTransactions GetProductionTransactionRejectsByWorkOrder(BusinessSystemSession e1Session, string plantNum, decimal orderNum)
        {
            var transactions = new ProductionTransactions();
            try
            {
                var cmd = (e1Session.DbCommands.ContainsKey("GetProductionTransactionRejectsByWorkOrder")) ? e1Session.DbCommands["GetProductionTransactionRejectsByWorkOrder"] : GetProductionTransactionRejectsByWorkOrder();
                cmd.Parameters[0].Value = plantNum.PadLeft(12);
                cmd.Parameters[1].Value = orderNum;
                cmd.Connection = e1Session.DbConnection;
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
                while (reader.Read())
                {
                    var tran = new ProductionTransaction
                    {
                        TransactionId = CastDbNull.To<decimal>(reader["SFUKID"], 0),
                        TransactionDate = Utilities.Common.CastSttmToDateTime(CastDbNull.To(reader["SFSTTM"], string.Empty).Trim()),
                        PlantNum = CastDbNull.To(reader["SFMMCU"], string.Empty).Trim(),
                        WorkCenter = CastDbNull.To(reader["SFMCU"], string.Empty).Trim(),
                        WorkCenterDesc = CastDbNull.To(reader["MCDL01"], string.Empty).Trim(),
                        OrderNum = CastDbNull.To<decimal>(reader["SFDOCO"], 0),
                        OrderType = CastDbNull.To(reader["SFDCTO"], string.Empty).Trim(),
                        UnitId = CastDbNull.To(reader["SFLOTN"], string.Empty).Trim(),
                        LiteNum = CastDbNull.To<decimal>(reader["SFZ58L1CLN"], 0),
                        RackId = CastDbNull.To<decimal>(reader["SFZ58RID"], 0),
                        DocumentType = CastDbNull.To(reader["SFEDCT"], string.Empty).Trim(),
                        TransactionType = CastDbNull.To(reader["SFTYTN"], string.Empty).Trim(),
                        RejectReason = CastDbNull.To(reader["SFZ58RCC"], string.Empty).Trim(),
                        RejectQuantity = CastDbNull.To<decimal>(reader["SFSOCN"], 0) / 1000
                    };
                    transactions.Add(tran);
                }
                reader.Close();
            }
            catch (Exception err)
            {
                throw new ApplicationException(err.ToString());
            }
            return transactions;
        }
    }
}
