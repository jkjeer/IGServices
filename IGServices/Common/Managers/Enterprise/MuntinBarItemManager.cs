using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IBM.Data.DB2.iSeries;
using IGServices.Common.Managers.E1;
using IGServices.Common.Types.E1;
using IGServices.Common.Types.Enterprise;
using IGServices.Common.Types.ShopFloor;
using IGServices.Common.Utilities;
using MuntinBarCode = IGServices.Common.Types.Enterprise.MuntinBarCode;
using MuntinBarCodes = IGServices.Common.Types.Enterprise.MuntinBarCodes;

namespace IGServices.Common.Managers.Enterprise
{
    public class MuntinBarItemManager
    {
        private MuntinBarCodeManager _barCodeManager;

        public MuntinBarItemManager(BusinessSystemSession session)
        {
            var hash = new Dictionary<string, iDB2Command>();
            //hash.Add("MuntinBarItemManager.GetBarItems", GetBarItems());
            hash.Add("MuntinBarItemManager.GetBarLength", GetBarLength());
            hash.Add("MuntinBarItemManager.GetPrimaryLocation", GetPrimaryLocation());
            hash.Add("MuntinBarItemManager.GetCostPerLinealFoot", GetCostPerLinealFoot());
            hash.Add("MuntinBarItemManager.GetQtyOnHand", GetQtyOnHand());
            foreach (string keyName in hash.Keys)
            {
                if (!session.DbCommands.ContainsKey(keyName))
                {
                    session.DbCommands.Add(keyName, hash[keyName]);
                }
            }
            _barCodeManager = new MuntinBarCodeManager(session);
        }
        /**********************************************************************************
        *   Get Items
        **********************************************************************************/
        public iDB2Command GetBarItems(List<string> filters )
        {
            var filterListString = String.Join(" ", filters);
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = String.Format(@"
            SELECT 
                IBMCU,
                IBITM,
                IBGLPT,
                IBSTKT,
                IBSRP4,
                IBUPMJ,
                IBTDAY,
                IMUOM1,
                IMDSC1,
                IMDSC2               
            FROM 
                F4102
                INNER JOIN F4101
                ON  IMITM = IBITM 
            WHERE
                {0}
                IBPRP1 = 'BAR' AND
                IBPRP2 NOT IN ('BUM')
            GROUP BY
                IBMCU,
                IBITM,
                IBGLPT,
                IBSTKT,
                IBSRP4,
                IBUPMJ,
                IBTDAY,
                IMUOM1,
                IMDSC1,
                IMDSC2   
            ORDER BY 1,2", filterListString);
            return cmd;
        }

        public MuntinBarItems GetBarItems(BusinessSystemSession session, Plant plant, ProcessRunSpan span, decimal itemNum)
        {
            var barItems = new MuntinBarItems();
            List<String> filters = new List<string>();
            List<iDB2Parameter> parameters = new List<iDB2Parameter>();
            var cmd = new iDB2Command();
            try
            {
                // Plant Num
                filters.Add("IBMCU = ? AND\r\n");
                parameters.Add(new iDB2Parameter("PlantNum", iDB2DbType.iDB2Graphic, 12));
                parameters.Last().Value = plant.PlantNum.PadLeft(12);

                // Item Num
                if (itemNum > 0)
                {
                    filters.Add("IBITM = ? AND\r\n");
                    parameters.Add(new iDB2Parameter("ItemNum", iDB2DbType.iDB2Decimal));
                    parameters.Last().Precision = 8;
                    parameters.Last().Scale = 0;
                    parameters.Last().Value = itemNum;
                }

                // Process Run Span
                if (!span.IsEmpty)
                {
                    var queryString1 = E1TimeManager.GetE1DateTimeQueryString("IBUPMJ", "IBTDAY", span.E1TimeIdSpan);
                    var queryString2 = E1TimeManager.GetE1DateTimeQueryString("LIUPMJ", "LITDAY", span.E1TimeIdSpan);
                    filters.Add($@"( 
                    (
                        {queryString1}
                    ) OR
                    (
                        EXISTS(SELECT 1
                                FROM F41021
                                WHERE
                                    LIMCU = IBMCU AND
                                    LIITM = IBITM AND
                                    (
                                        {queryString2}
                                    )
                                )
                    )
                ) AND ");
                }

                //if (!span.IsEmpty)
                //{
                //    var queryString = E1TimeManager.GetE1DateTimeQueryString("IBUPMJ", "IBTDAY", span.E1TimeIdSpan);

                //    filters.Add(queryString + " AND \r\n");
                //}

                cmd = GetBarItems(filters);
                foreach (iDB2Parameter parameter in parameters)
                {
                    cmd.Parameters.Add(parameter);
                }
                foreach (iDB2Parameter parameter in parameters)
                {
                    cmd.Parameters.Add(parameter);
                }
                cmd.Connection = session.DbConnection;
                cmd.CommandTimeout = 0;

                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
                while (reader.Read())
                {
                    var barItem = new MuntinBarItem();
                    barItem.ItemNum = CastDbNull.To<Decimal>(reader["IBITM"], 0);
                    barItem.Description = CastDbNull.To<String>(reader["IMDSC1"], String.Empty).Trim();
                    barItem.StockingType = CastDbNull.To<String>(reader["IBSTKT"], String.Empty).Trim();
                    barItem.GLCategory = CastDbNull.To<String>(reader["IBGLPT"], String.Empty).Trim();
                    barItem.PrimaryUom = CastDbNull.To<String>(reader["IMUOM1"], String.Empty).Trim();
                    var updateDateId = CastDbNull.To<Decimal>(reader["IBUPMJ"], 0);
                    var updateTimeId = CastDbNull.To<Decimal>(reader["IBTDAY"], 0);
                    barItem.UpdateDate = Utilities.Common.CastToDateTime(updateDateId, updateTimeId);
                    var code = CastDbNull.To<String>(reader["IMDSC2"], String.Empty).Trim();
                    var bars = new MuntinBarCodes();
                    foreach (string c in code.Split(','))
                    {
                        var bar = new MuntinBarCode();
                        bar = _barCodeManager.GetBarCode(session, c.Trim().PadLeft(5, '0'));
                        if (bar.MCode != null)
                        {
                            bars.Add(bar);
                        }
                    }
                    barItem.MuntinBarCodes = bars;
                    barItem.Length = GetBarLength(session, plant.PlantNum, barItem.ItemNum);
                    barItem.PrimaryLocation = GetPrimaryLocation(session, plant.PlantNum, barItem.ItemNum);
                    barItem.CostPerLinealFoot = GetCostPerLinealFoot(session, plant.PlantNum, barItem.ItemNum);
                    barItem.QtyOnHand = GetQtyOnHand(session, plant.PlantNum, barItem.ItemNum);
                    barItems.Add(barItem);
                }
                reader.Close();
            }
            catch (Exception err)
            {
                throw new ApplicationException(err.ToString());
            }
            return barItems;
        }



        /**********************************************************************************
        *   Length
        **********************************************************************************/
        public iDB2Command GetBarLength()
        {
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText =
                @"SELECT
                      IBMCU, 
                      IBITM,
                      ROUND((UMCONV * 12 / 10000000), 4) AS UMCONV
                    FROM
                      F4102
                      INNER JOIN F41002
                      ON IBITM = UMITM AND 
	                    IBMCU = UMMCU
                      INNER JOIN F4101
                      ON IMITM = IBITM
                    WHERE
                      --IBGLPT = 'CHAN' AND
                      UMUM = 'EA' AND
                      UMRUM = 'LF' AND
                      IBMCU = ? AND
                      IBITM = ?";
            cmd.Parameters.Add(new iDB2Parameter("PlantNum", iDB2DbType.iDB2Graphic));
            cmd.Parameters.Add(new iDB2Parameter("ItemNum", iDB2DbType.iDB2Decimal));
            cmd.Parameters["ItemNum"].Precision = 8;
            cmd.Parameters["ItemNum"].Scale = 0;
            return cmd;
        }

        public decimal GetBarLength(BusinessSystemSession session, string plantNum, decimal itemNum)
        {
            decimal length = 0;
            try
            {
                var cmd = (session.DbCommands.ContainsKey("MuntinBarItemManager.GetBarLength")) ? session.DbCommands["MuntinBarItemManager.GetBarLength"] : GetBarLength();
                cmd.Parameters[0].Value = plantNum.PadLeft(12);
                cmd.Parameters[1].Value = itemNum;
                cmd.Connection = session.DbConnection;
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
                while (reader.Read())
                {
                    length = CastDbNull.To<decimal>(reader["UMCONV"], 0);
                }
                reader.Close();
            }
            catch (Exception err)
            {
                throw new ApplicationException(err.ToString());
            }
            return Math.Round(length, 2);
        }

        /**********************************************************************************
        *   Primary Location
        **********************************************************************************/
        public iDB2Command GetPrimaryLocation()
        {
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText =
                @"SELECT
                      LILOCN
                    FROM
                      F41021
                    WHERE
                      LIMCU = ? AND
                      LIITM = ? AND
                      LIPBIN = 'P'";
            cmd.Parameters.Add(new iDB2Parameter("PlantNum", iDB2DbType.iDB2Graphic));
            cmd.Parameters.Add(new iDB2Parameter("ItemNum", iDB2DbType.iDB2Decimal));
            cmd.Parameters["ItemNum"].Precision = 8;
            cmd.Parameters["ItemNum"].Scale = 0;
            return cmd;
        }

        public string GetPrimaryLocation(BusinessSystemSession session, string plantNum, decimal itemNum)
        {
            var location = "";
            try
            {
                var cmd = (session.DbCommands.ContainsKey("MuntinBarItemManager.GetPrimaryLocation")) ? session.DbCommands["MuntinBarItemManager.GetPrimaryLocation"] : GetPrimaryLocation();
                cmd.Parameters[0].Value = plantNum.PadLeft(12);
                cmd.Parameters[1].Value = itemNum;
                cmd.Connection = session.DbConnection;
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
                while (reader.Read())
                {
                    location = CastDbNull.To(reader["LILOCN"], string.Empty).Trim();
                }
                reader.Close();
            }
            catch (Exception err)
            {
                throw new ApplicationException(err.ToString());
            }
            return location;
        }

        /**********************************************************************************
        *   Cost per lineal foot
        **********************************************************************************/
        public iDB2Command GetCostPerLinealFoot()
        {
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText =
                @"SELECT
                      IECSL / 10000 AS IECSL
                    FROM
                      F30026
                      INNER JOIN F4101
                      ON IMITM = IEITM
                    WHERE
                      IEMMCU = ? AND
                      IEITM = ? AND
                      IMUOM1 = 'LF'";
            cmd.Parameters.Add(new iDB2Parameter("PlantNum", iDB2DbType.iDB2Graphic));
            cmd.Parameters.Add(new iDB2Parameter("ItemNum", iDB2DbType.iDB2Decimal));
            cmd.Parameters["ItemNum"].Precision = 8;
            cmd.Parameters["ItemNum"].Scale = 0;
            return cmd;
        }

        public decimal GetCostPerLinealFoot(BusinessSystemSession session, string plantNum, decimal itemNum)
        {
            decimal cost = 0;
            try
            {
                var cmd = (session.DbCommands.ContainsKey("MuntinBarItemManager.GetCostPerLinealFoot")) ? session.DbCommands["MuntinBarItemManager.GetCostPerLinealFoot"] : GetCostPerLinealFoot();
                cmd.Parameters[0].Value = plantNum.PadLeft(12);
                cmd.Parameters[1].Value = itemNum;
                cmd.Connection = session.DbConnection;
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
                while (reader.Read())
                {
                    cost = CastDbNull.To<decimal>(reader["IECSL"], 0);
                }
                reader.Close();
            }
            catch (Exception err)
            {
                throw new ApplicationException(err.ToString());
            }
            return Math.Round(cost, 6);
        }

        /**********************************************************************************
        *   Qty on Hand
        **********************************************************************************/
        public iDB2Command GetQtyOnHand()
        {
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText =
                @"SELECT
                      SUM(LIPQOH / 1000) AS LIPQOH
                    FROM
                      F41021
                    WHERE
                      LIMCU = ? AND
                      LIITM = ?";
            cmd.Parameters.Add(new iDB2Parameter("PlantNum", iDB2DbType.iDB2Graphic));
            cmd.Parameters.Add(new iDB2Parameter("ItemNum", iDB2DbType.iDB2Decimal));
            cmd.Parameters["ItemNum"].Precision = 8;
            cmd.Parameters["ItemNum"].Scale = 0;
            return cmd;
        }

        public decimal GetQtyOnHand(BusinessSystemSession session, string plantNum, decimal itemNum)
        {
            decimal qty = 0;
            try
            {
                var cmd = (session.DbCommands.ContainsKey("MuntinBarItemManager.GetQtyOnHand")) ? session.DbCommands["MuntinBarItemManager.GetQtyOnHand"] : GetQtyOnHand();
                cmd.Parameters[0].Value = plantNum.PadLeft(12);
                cmd.Parameters[1].Value = itemNum;
                cmd.Connection = session.DbConnection;
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
                while (reader.Read())
                {
                    qty = CastDbNull.To<decimal>(reader["LIPQOH"], 0);
                }
                reader.Close();
            }
            catch (Exception err)
            {
                throw new ApplicationException(err.ToString());
            }
            return Math.Round(qty, 2);
        }

    }

}
