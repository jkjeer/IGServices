using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using IGServices.Common.Managers.Response;
using IGServices.Common.Types.Enterprise;
using IGServices.Common.Types.Response;
//using NLog;

namespace IGServices.Common.Managers.ShopFloor
{
    public class ConfiguratorRulesManager
    {
        //private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /******************************************************************
        * GET ShopFloor ConfiguratorRules
        ******************************************************************/
        public Types.ShopFloor.ConfiguratorRules GetShopFloorConfiguratorRules(ConfiguratorRules configuratorRules)
        {
            var sfConfiguratorRules = new Types.ShopFloor.ConfiguratorRules();
            sfConfiguratorRules.AddRange(configuratorRules.Select(configuratorRule => new Types.ShopFloor.ConfiguratorRule
            {
                RulesTableType = configuratorRule.RulesTableType,
                RulesTableName = configuratorRule.RulesTableName,
                CostCenter = configuratorRule.CostCenter,
                ConfiguredTableKeyID = configuratorRule.ConfiguredTableKeyID,
                TableSegmentValue1 = configuratorRule.TableSegmentValue1,
                TableSegmentValue2 = configuratorRule.TableSegmentValue2,
                TableSegmentValue3 = configuratorRule.TableSegmentValue3,
                TableSegmentValue4 = configuratorRule.TableSegmentValue4,
                TableSegmentValue5 = configuratorRule.TableSegmentValue5,
                TableSegmentValue6 = configuratorRule.TableSegmentValue6,
                TableSegmentValue7 = configuratorRule.TableSegmentValue7,
                TableSegmentValue8 = configuratorRule.TableSegmentValue8,
                TableSegmentValue9 = configuratorRule.TableSegmentValue9,
                TableSegmentValue0 = configuratorRule.TableSegmentValue0,
                TableSegmentValue11 = configuratorRule.TableSegmentValue11,
                TableSegmentValue12 = configuratorRule.TableSegmentValue12,
                TableSegmentValue13 = configuratorRule.TableSegmentValue13,
                TableSegmentValue14 = configuratorRule.TableSegmentValue14,
                TableSegmentValue15 = configuratorRule.TableSegmentValue15,
                TableSegmentValue16 = configuratorRule.TableSegmentValue16,
                TableSegmentValue17 = configuratorRule.TableSegmentValue17,
                TableSegmentValue18 = configuratorRule.TableSegmentValue18,
                TableSegmentValue19 = configuratorRule.TableSegmentValue19,
                TableSegmentValue20 = configuratorRule.TableSegmentValue20,
                BubbleSequence = configuratorRule.BubbleSequence,
                EffectiveFromDate = configuratorRule.EffectiveFromDate,
                EffectiveThruDate = configuratorRule.EffectiveThruDate,
                RulesTableValue = configuratorRule.RulesTableValue,
                UnitPriceEntered = configuratorRule.UnitPriceEntered,
                IdentifierShortItem = configuratorRule.IdentifierShortItem,
                QtyRequiredStandard = configuratorRule.QtyRequiredStandard,
                UnitOfMeasure = configuratorRule.UnitOfMeasure,
                UserReservedCode = configuratorRule.UserReservedCode,
                UserReservedDate = configuratorRule.UserReservedDate,
                UserReservedAmount = configuratorRule.UserReservedAmount,
                UserReservedNumber = configuratorRule.UserReservedNumber,
                UserReservedReference = configuratorRule.UserReservedReference,
                UserId = configuratorRule.UserId,
                ProgramId = configuratorRule.ProgramId,
                WorkStationId = configuratorRule.WorkStationId,
                UpdateDate = configuratorRule.UpdateDateTime
            }));
            return sfConfiguratorRules;
        }

        /******************************************************************
        * Download 
        ******************************************************************/
        public Transfer DownloadConfiguratorRules(ConfiguratorRules configuratorRules, SqlConnection cnSFInt)
        {
            var transfer = new Transfer
            {
                TransferType = "Download"
            };
            var bcMgr = new SqlBulkCopyManager { DbConnectionString = cnSFInt.ConnectionString };
            var configuratorRulesSF = GetShopFloorConfiguratorRules(configuratorRules);
            IDataReader dr = configuratorRulesSF.GetDataReader();
            var sqlBulkCopyResponse = bcMgr.BulkReader("ConfiguratorRulesLedger", dr);
            transfer.SqlBulkCopyResponses.Add(sqlBulkCopyResponse);
            transfer.Write = 1;
            transfer.Transferred = true;
            transfer.TransferStatus = "Y";
            transfer.TransferDate = DateTime.Now.ToLocalTime();
            if (sqlBulkCopyResponse.HasErrors())
            {
                throw new ApplicationException(sqlBulkCopyResponse.Message);
            }
            return transfer;
        }
    }
}
