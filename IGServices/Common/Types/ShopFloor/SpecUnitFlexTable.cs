using System;
using System.Collections.Generic;
using System.Data;
using System.ServiceModel;
using System.Text;
using IGServices.Common.Types.Enterprise;
using IGServices.Common.Utilities;
using NLog;

namespace IGServices.Common.Types.ShopFloor
{

    [System.ComponentModel.DesignerCategory("Code")]
    public class SpecUnitFlexTable : DataTable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public SpecUnitFlexTable()
        {
            ExtendedProperties.Add("Target", "[ITM].[SPEC_UNIT_FLEX_LEDGER]");
            Columns.Add("ITEM_NUM", typeof(decimal));
            Columns.Add("CONFIGURATION_ID", typeof(decimal));
            Columns.Add("CONSTRUCTION_CD", typeof(string));
            Columns.Add("SHAPE_CD", typeof(string));
            Columns.Add("AIRSPACE_CD", typeof(string));
            Columns.Add("PRESERVE_CD", typeof(string));
            Columns.Add("PLACEMENTS_CD", typeof(string));
            Columns.Add("GLASS_PROCESSING_CD", typeof(string));
            Columns.Add("VGROOVE_CD", typeof(string));
            Columns.Add("ROUTING_CD", typeof(string));
            Columns.Add("RELATED_ORDERS", typeof(string));
        }

        public void Add(SpecUnit item)
        {
            try
            {
                DataRow dr = NewRow();
                dr["ITEM_NUM"] = item.ItemNum;
                dr["CONFIGURATION_ID"] = item.Id;
                dr["CONSTRUCTION_CD"] = item.FlexDescriptions[0];
                dr["SHAPE_CD"] = item.FlexDescriptions[1];
                dr["AIRSPACE_CD"] = item.FlexDescriptions[2];
                dr["PRESERVE_CD"] = item.FlexDescriptions[3];
                dr["PLACEMENTS_CD"] = item.FlexDescriptions[4];
                dr["GLASS_PROCESSING_CD"] = item.FlexDescriptions[5];
                dr["VGROOVE_CD"] = item.FlexDescriptions[6];
                dr["ROUTING_CD"] = item.FlexDescriptions[7];
                if (item.OrderIds?.Count > 0)
                {
                    var orderIdList = new List<String>();
                    foreach (OrderId orderId in item.OrderIds)
                    {
                        orderIdList.Add(orderId.Description);
                    }
                    //dr["RELATED_ORDERS"] = string.Join(" | ", Array.ConvertAll(item.OrderIds.ToArray(), i => i.ToString()));
                    dr["RELATED_ORDERS"] = string.Join(" | ", Array.ConvertAll(orderIdList.ToArray(), i => i.ToString()));
                }
                else
                {
                    dr["RELATED_ORDERS"] = "";
                }
                Rows.Add(dr);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

    }
}
