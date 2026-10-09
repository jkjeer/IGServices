using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.Text;
using IBM.Data.DB2.iSeries;
using IGServices.Common.Managers.E1;
using IGServices.Common.Types.E1;
using IGServices.Common.Types.Enterprise;
using IGServices.Common.Types.ShopFloor;
using IGServices.Common.Utilities;
using NLog;
using SpacerItem = IGServices.Common.Types.Enterprise.SpacerItem;
using SpacerItems = IGServices.Common.Types.Enterprise.SpacerItems;
using SpacerTokens = IGServices.Common.Types.Configuration.SpacerTokens;

namespace IGServices.Common.Managers.Enterprise
{
    public class SpacerItemManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        readonly SpacerTokens _spacerTokenConfig;
        private readonly SpacerTypeManager _spacerTypeManager;

        public SpacerItemManager(BusinessSystemSession session)
        {
            var hash = new Dictionary<string, iDB2Command>
            {
                {"SpacerItemManager.GetSpacerItem", GetSpacerItem()},
                {"SpacerItemManager.GetSpacerLength", GetSpacerLength()},
                {"SpacerItemManager.GetCostPerLinealFoot", GetCostPerLinealFoot()},
                {"SpacerItemManager.GetPrimaryLocation", GetPrimaryLocation()},
                {"SpacerItemManager.GetQtyOnHand", GetQtyOnHand()}
            };
            foreach (var keyName in hash.Keys.Where(keyName => !session.DbCommands.ContainsKey(keyName)))
            {
                session.DbCommands.Add(keyName, hash[keyName]);
            }
            var spacerConfiguration = (Types.Configuration.SpacerConfiguration)ConfigurationManager.GetSection("SpacerSection");
            _spacerTokenConfig = spacerConfiguration.SpacerTokens;
            _spacerTypeManager = new SpacerTypeManager(session);
        }

        /*********************************************************************************
        * Spacer Item
        ********************************************************************************/
        public iDB2Command GetSpacerItem()
        {
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = @"
            SELECT 
                IBMCU,
                IBITM,
                IBGLPT,
                IBSTKT,
                IBSRP4,
                IMUOM1,
                IMDSC1,
                IMDSC2,
                IBUPMJ,
                IBTDAY    
            FROM
                F4102
                INNER JOIN F4101
                ON IMITM = IBITM              
            WHERE 
                IBPRP1 = 'SPC' AND                        
                IBPRP2 IN ('ALU','STS') AND
                IBMCU = ? AND
                IBITM = ?";
            cmd.Parameters.Add(new iDB2Parameter("MCU", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("ITM", iDB2DbType.iDB2Decimal));
            cmd.Parameters["ITM"].Precision = 8;
            cmd.Parameters["ITM"].Scale = 0;
            return cmd;
        }

        public SpacerItem GetSpacerItem(BusinessSystemSession session, string plantNum, decimal itemNum)
        {
            var item = new SpacerItem();
            try
            {
                var cmd = (session.DbCommands.ContainsKey("SpacerItemManager.GetSpacerItem")) ? session.DbCommands["SpacerItemManager.GetSpacerItem"] : GetSpacerItem();
                cmd.Parameters["MCU"].Value = plantNum.PadLeft(12);
                cmd.Parameters["ITM"].Value = itemNum;
                cmd.Connection = session.DbConnection;
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
                while (reader.Read())
                {
                    var dateId = CastDbNull.To<decimal>(reader["IBUPMJ"], 0);
                    var timeId = CastDbNull.To<decimal>(reader["IBTDAY"], 0);
                    item.ItemNum = CastDbNull.To<decimal>(reader["IBITM"], 0);
                    item.StockingType = CastDbNull.To(reader["IBSTKT"], string.Empty).Trim();
                    item.GLCategory = CastDbNull.To(reader["IBGLPT"], string.Empty).Trim();
                    item.Description = CastDbNull.To(reader["IMDSC1"], string.Empty).Trim();
                    item.Construction = CastDbNull.To(reader["IMDSC2"], string.Empty).Trim();
                    item.PrimaryUom = CastDbNull.To(reader["IMUOM1"], string.Empty).Trim();
                    item.UpdateDate = Utilities.Common.CastToDateTime(dateId, timeId);
                    item.Length = GetSpacerLength(session, plantNum, item.ItemNum);
                    item.CostPerLinealFoot = GetCostPerLinealFoot(session, plantNum, item.ItemNum);
                    item.PrimaryLocation = GetPrimaryLocation(session, plantNum, item.ItemNum);
                    item.QtyOnHand = GetQtyOnHand(session, plantNum, item.ItemNum);
                    GetSpacerItemAttributes(item);
                }
                reader.Close();
            }
            catch (Exception err)
            {
                throw new ApplicationException(err.ToString());
            }
            return item;
        }

        /*********************************************************************************
        * Spacer Items
        ********************************************************************************/
        public iDB2Command GetSpacerItems(List<string> filters)
        {
            var filterString = new StringBuilder();
            foreach (var filter in filters)
            {
                filterString.Append(filter);
            }
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = $@"
            SELECT 
                IBMCU,
                IBITM,
                IBGLPT,
                IBSTKT,
                IBSRP4,
                IMUOM1,
                IMDSC1,
                IMDSC2,
                IBUPMJ,
                IBTDAY    
            FROM
                F4102
                INNER JOIN F4101
                ON IMITM = IBITM              
            WHERE 
                {filterString}
                IBPRP1 = 'SPC' AND                        
                IBPRP2 IN ('ALU','STS')
            ORDER BY 
                IBITM";
            return cmd;
        }

        public SpacerItems GetSpacerItems(BusinessSystemSession session, Plant plant, ProcessRunSpan span, decimal itemNum)
        {
            var items = new SpacerItems();
            var filters = new List<string>();
            var parameters = new List<iDB2Parameter>();
            try
            {
                // Plant
                if (plant != null)
                {
                    filters.Add("IBMCU = ? AND\r\n");
                    parameters.Add(new iDB2Parameter("IBMCU", iDB2DbType.iDB2Graphic, 12));
                    parameters.Last().Value = plant.PlantNum.PadLeft(12);
                }

                // Item 
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
                    var queryString = E1TimeManager.GetE1DateTimeQueryString("IBUPMJ", "IBTDAY", span.E1TimeIdSpan);
                    filters.Add(queryString + " AND \r\n");
                }

                var cmd = GetSpacerItems(filters);
                foreach (var parameter in parameters)
                {
                    cmd.Parameters.Add(parameter);
                }

                cmd.Connection = session.DbConnection;
                Logger.Trace(Logging.DbCmdString(cmd));
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
                while (reader.Read())
                {
                    var dateId = CastDbNull.To<decimal>(reader["IBUPMJ"], 0);
                    var timeId = CastDbNull.To<decimal>(reader["IBTDAY"], 0);
                    var item = new SpacerItem
                    {
                        ItemNum = CastDbNull.To<decimal>(reader["IBITM"], 0),
                        StockingType = CastDbNull.To(reader["IBSTKT"], string.Empty).Trim(),
                        GLCategory = CastDbNull.To(reader["IBGLPT"], string.Empty).Trim(),
                        Description = CastDbNull.To(reader["IMDSC1"], string.Empty).Trim(),
                        Construction = CastDbNull.To(reader["IMDSC2"], string.Empty).Trim(),
                        PrimaryUom = CastDbNull.To(reader["IMUOM1"], string.Empty).Trim(),
                        UpdateDate = Utilities.Common.CastToDateTime(dateId, timeId)
                    };
                    items.Add(item);
                }
                reader.Close();
                foreach (var item in items)
                {
                    item.Length = GetSpacerLength(session, plant.PlantNum, item.ItemNum);
                    item.CostPerLinealFoot = GetCostPerLinealFoot(session, plant.PlantNum, item.ItemNum);
                    item.PrimaryLocation = GetPrimaryLocation(session, plant.PlantNum, item.ItemNum);
                    item.QtyOnHand = GetQtyOnHand(session, plant.PlantNum, item.ItemNum);
                }
                GetSpacerItemAttributes(items);
            }
            catch (Exception e)
            {
                throw new FaultException(e.Message);
            }
            return items;
        }

        /*********************************************************************************
        * Spacer Items
        ********************************************************************************/
        public iDB2Command GetSpacerItemKeys(List<string> filters)
        {
            var filterString = new StringBuilder();
            foreach (var filter in filters)
            {
                filterString.Append(filter);
            }
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = $@"
            SELECT
				IBMCU,
                IBITM,
                IBGLPT,
                IBSTKT,
                IBSRP4,
                IMUOM1,
                IMDSC1,
                IMDSC2,
                IBUPMJ,
                IBTDAY    
            FROM
                F4102  
				INNER JOIN F4101
				ON	IMITM = IBITM       
            WHERE 
                {filterString}
                IBPRP1 = 'SPC' AND                        
                IBPRP2 IN ('ALU','STS','SS','BLA')
			GROUP BY
				IBMCU,
                IBITM,
                IBGLPT,
                IBSTKT,
                IBSRP4,
                IMUOM1,
                IMDSC1,
                IMDSC2,
                IBUPMJ,
                IBTDAY 
            ORDER BY 
                IBITM";
            return cmd;
        }

        public SpacerItems GetSpacerItemKeys(BusinessSystemSession session, Plant plant, ProcessRunSpan span, decimal itemNum)
        {
            var items = new SpacerItems();
            var filters = new List<string>();
            var parameters = new List<iDB2Parameter>();
            try
            {
                // Plant
                if (plant != null)
                {
                    filters.Add("IBMCU = ? AND\r\n");
                    parameters.Add(new iDB2Parameter("IBMCU", iDB2DbType.iDB2Graphic, 12));
                    parameters.Last().Value = plant.PlantNum.PadLeft(12);
                }

                // Item 
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

                var cmd = GetSpacerItemKeys(filters);
                foreach (var parameter in parameters)
                {
                    cmd.Parameters.Add(parameter);
                }

                cmd.Connection = session.DbConnection;
                Logger.Trace(Logging.DbCmdString(cmd));
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
                while (reader.Read())
                {
                    var dateId = CastDbNull.To<decimal>(reader["IBUPMJ"], 0);
                    var timeId = CastDbNull.To<decimal>(reader["IBTDAY"], 0);
                    var item = new SpacerItem
                    {
                        ItemNum = CastDbNull.To<decimal>(reader["IBITM"], 0),
                        StockingType = CastDbNull.To(reader["IBSTKT"], string.Empty).Trim(),
                        GLCategory = CastDbNull.To(reader["IBGLPT"], string.Empty).Trim(),
                        Description = CastDbNull.To(reader["IMDSC1"], string.Empty).Trim(),
                        Construction = CastDbNull.To(reader["IMDSC2"], string.Empty).Trim(),
                        PrimaryUom = CastDbNull.To(reader["IMUOM1"], string.Empty).Trim(),
                        UpdateDate = Utilities.Common.CastToDateTime(dateId, timeId)
                    };
                    items.Add(item);
                }
                reader.Close();
                foreach (var item in items)
                {
                    item.Length = GetSpacerLength(session, plant.PlantNum, item.ItemNum);
                    item.CostPerLinealFoot = GetCostPerLinealFoot(session, plant.PlantNum, item.ItemNum);
                    item.PrimaryLocation = GetPrimaryLocation(session, plant.PlantNum, item.ItemNum);
                    item.QtyOnHand = GetQtyOnHand(session, plant.PlantNum, item.ItemNum);
                }
                GetSpacerItemAttributes(items);
            }
            catch (Exception e)
            {
                throw new FaultException(e.Message);
            }
            return items;
        }

        /**********************************************************************************
        *   Spacer Item Attributes - parse F4101 Description
        **********************************************************************************/
        public void GetSpacerItemAttributes(SpacerItems items)
        {
            try
            {
                string[] gaugeIndex = { "0045", "006", "008" };
                foreach (var item in items)
                {
                    var tokens = item.Description.Replace("  "," ").Split(' ');
                    var typeName = "";
                    switch (tokens.Length)
                    {
                        case 4:
                            typeName = tokens[2].Trim();
                            break;
                        case 5:
                            typeName = tokens[2].Trim() + " " + tokens[3].Trim();
                            break;
                        case 6:
                            typeName = tokens[2].Trim() + " " + tokens[3].Trim() + " " + tokens[4].Trim();
                            break;
                    }
                    try
                    {
                        decimal w;
                        decimal.TryParse(tokens[1], out w);
                        item.Width = w;
                        item.PreKeyed = (_spacerTokenConfig[typeName].prekeyed == "true");
                        var spacerTypeName = _spacerTokenConfig[typeName].type;
                        item.SpacerType = _spacerTypeManager.GetSpacerType(spacerTypeName);
                        item.Profile = _spacerTokenConfig[typeName].profile;
                    }
                    catch (Exception)
                    {
                        Logger.Warn("Spacer Type Lookup Failed ... Item " + item.ItemNum.ToString(CultureInfo.InvariantCulture) + " " + item.Description);
                    }

                    var tokens2 = item.Construction.Split(' ');
                    IEnumerable<string> inBoth = tokens2.Intersect(gaugeIndex).ToList();
                    if (inBoth.Any())
                        item.Gauge = inBoth.First();
                }
            }
            catch (Exception err)
            {
                throw new ApplicationException(err.ToString());
            }
        }

        public void GetSpacerItemAttributes(SpacerItem item)
        {
            try
            {
                string[] gaugeIndex = { "0045", "006", "008" };

                    var tokens = item.Description.Replace("  ", " ").Split(' ');
                    var typeName = "";
                    switch (tokens.Length)
                    {
                        case 4:
                            typeName = tokens[2].Trim();
                            break;
                        case 5:
                            typeName = tokens[2].Trim() + " " + tokens[3].Trim();
                            break;
                        case 6:
                            typeName = tokens[2].Trim() + " " + tokens[3].Trim() + " " + tokens[4].Trim();
                            break;
                    }
                    try
                    {
                        decimal w;
                        decimal.TryParse(tokens[1], out w);
                        item.Width = w;
                        item.PreKeyed = (_spacerTokenConfig[typeName].prekeyed == "true");
                        var spacerTypeName = _spacerTokenConfig[typeName].type;
                        item.SpacerType = _spacerTypeManager.GetSpacerType(spacerTypeName);
                        //item.SpacerType = _spacerTokenConfig[typeName].type;
                        item.Profile = _spacerTokenConfig[typeName].profile;
                        //item.Material = _spacerTypeConfig[item.SpacerType].material;
                        //item.Color = _spacerTypeConfig[item.SpacerType].color;
                        //item.Painted = _spacerTypeConfig[item.SpacerType].painted;
                    }
                    catch (Exception)
                    {
                        Logger.Warn("Spacer Type Lookup Failed ... Item " + item.ItemNum.ToString(CultureInfo.InvariantCulture) + " " + item.Description);
                    }

                    var tokens2 = item.Construction.Split(' ');
                    IEnumerable<string> inBoth = tokens2.Intersect(gaugeIndex).ToList();
                    if (inBoth.Any())
                        item.Gauge = inBoth.First();
            }
            catch (Exception err)
            {
                throw new ApplicationException(err.ToString());
            }
        }

        /**********************************************************************************
        *   Spacer Length
        **********************************************************************************/
        public iDB2Command GetSpacerLength()
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = @"
                    SELECT
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
                      IBGLPT = 'CHAN' AND
                      UMUM = 'EA' AND
                      UMRUM = 'LF' AND
                      IBMCU = ? AND
                      IBITM = ?"
            };
            cmd.Parameters.Add(new iDB2Parameter("PlantNum", iDB2DbType.iDB2Graphic));
            cmd.Parameters.Add(new iDB2Parameter("ItemNum", iDB2DbType.iDB2Decimal));
            cmd.Parameters["ItemNum"].Precision = 8;
            cmd.Parameters["ItemNum"].Scale = 0;
            return cmd;
        }

        public decimal GetSpacerLength(BusinessSystemSession session, string plantNum, decimal itemNum)
        {
            decimal length = 0;
            try
            {
                var cmd = (session.DbCommands.ContainsKey("SpacerItemManager.GetSpacerLength")) ? session.DbCommands["SpacerItemManager.GetSpacerLength"] : GetSpacerLength();
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
            return Math.Round(length,2);
        }

        /**********************************************************************************
        *   Primary Location
        **********************************************************************************/
        public iDB2Command GetPrimaryLocation()
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = @"
                    SELECT
                      LILOCN
                    FROM
                      F41021
                    WHERE
                      LIMCU = ? AND
                      LIITM = ? AND
                      LIPBIN = 'P'"
            };
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
                var cmd = (session.DbCommands.ContainsKey("SpacerItemManager.GetPrimaryLocation")) ? session.DbCommands["SpacerItemManager.GetPrimaryLocation"] : GetPrimaryLocation();
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
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = @"
                    SELECT
                      IECSL / 10000 AS IECSL
                    FROM
                      F30026
                      INNER JOIN F4101
                      ON IMITM = IEITM
                    WHERE
                      IEMMCU = ? AND
                      IEITM = ? AND
                      IMUOM1 = 'LF'"
            };
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
                var cmd = (session.DbCommands.ContainsKey("SpacerItemManager.GetCostPerLinealFoot")) ? session.DbCommands["SpacerItemManager.GetCostPerLinealFoot"] : GetCostPerLinealFoot();
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
            return Math.Round(cost,6);
        }

        /**********************************************************************************
        *   Qty on Hand
        **********************************************************************************/
        public iDB2Command GetQtyOnHand()
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = @"
                    SELECT
                      SUM(LIPQOH / 1000) AS LIPQOH
                    FROM
                      F41021
                    WHERE
                      LIMCU = ? AND
                      LIITM = ?"
            };
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
                var cmd = (session.DbCommands.ContainsKey("SpacerItemManager.GetQtyOnHand")) ? session.DbCommands["SpacerItemManager.GetQtyOnHand"] : GetQtyOnHand();
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

        public iDB2Command GetSpacerItemFromQ()
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = @"
                    SELECT 
	                    RDITM               
                    FROM 
	                    F3283
                    WHERE 
                        RDRTBT = 'Q' AND
                        RDTBLC = ? AND
                        RDMCU = ? AND
                        RDTSV1 = ? AND
                        RDTSV2 = ?"
            };
            cmd.Parameters.Add(new iDB2Parameter("TableName", iDB2DbType.iDB2Graphic, 10));
            cmd.Parameters.Add(new iDB2Parameter("Plant", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("ChannelWidth", iDB2DbType.iDB2Graphic, 40));
            cmd.Parameters.Add(new iDB2Parameter("ChannelType", iDB2DbType.iDB2Graphic, 40));
            return cmd;
        }

        private decimal GetSpacerItemFromAttributes(BusinessSystemSession session, string plantNum, string spacerType, decimal spacerWidth, string tableName)
        {
            decimal itemNum = 0;
            try
            {
                var cmd = (session.DbCommands.ContainsKey("SpacerItemManager.GetSpacerItemFromQ")) ? session.DbCommands["SpacerItemManager.GetSpacerItemFromQ"] : GetSpacerItemFromQ();
                cmd.Parameters[0].Value = tableName;
                cmd.Parameters[1].Value = plantNum.PadLeft(12);
                cmd.Parameters[2].Value = spacerWidth.ToString(CultureInfo.InvariantCulture);
                cmd.Parameters[3].Value = spacerType;
                cmd.Connection = session.DbConnection;
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
                while (reader.Read())
                {
                    itemNum = CastDbNull.To<decimal>(reader["RDITM"], 0);
                }
                reader.Close();
            }
            catch (Exception err)
            {
                throw new ApplicationException(err.ToString());
            }
            return itemNum;
        }


    }
}
