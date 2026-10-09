using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IBM.Data.DB2.iSeries;
using IGServices.Common.Types.E1;
using IGServices.Common.Types.Enterprise;
using IGServices.Common.Utilities;
using NLog;

namespace IGServices.Common.Managers.Enterprise
{
    public class SpacerManager
    {
        Logger _logger = LogManager.GetCurrentClassLogger();

        public SpacerManager(BusinessSystemSession e1Session)
        {
            var hash = new Dictionary<string, iDB2Command>
            {
                {"SpacerManager.GetSpacerTypeFromC", GetSpacerTypeFromC()}
            };
            foreach (var keyName in hash.Keys.Where(keyName => !e1Session.DbCommands.ContainsKey(keyName)))
            {
                e1Session.DbCommands.Add(keyName, hash[keyName]);
            }
        }

        /**********************************************************************************
        *   Glass Item properties
        **********************************************************************************/
        public Spacer GetSpacerAttributes(BusinessSystemSession e1Session, string plantNum, ItemAttributes attributes, string tableName)
        {
            var spacer = new Spacer();
            try
            {
                if (tableName == "IGSPACER")
                {
                    spacer.SpacerType = attributes.GetStringAttributeValue("Z58L1STP");
                    spacer.Width = attributes.GetDecimalAttributeValue("Z58L1SSZ");
                }
                else
                {
                    spacer.SpacerType = attributes.GetStringAttributeValue("Z58L2STP");
                    spacer.Width = attributes.GetDecimalAttributeValue("Z58L2SSZ");
                }
                //spacer.NarrowMargin = attributes.GetStringAttributeValue("Z58IGNMC");
                //spacer.NarrowMarginDesc = attributes.GetStringAttributeDesc1("Z58IGNMC");
                var dict = GetSpacerTypeProperties(e1Session, spacer.SpacerType);
                spacer.Material = dict[1];
                spacer.Color = dict[2];
                spacer.Bead = dict[3];
                spacer.Endur = (spacer.SpacerType.Substring(1, 1) == "E");
            }
            catch (Exception err)
            {
                throw new ApplicationException(err.ToString());
            }
            return spacer;
        }

        private Dictionary<decimal, string> GetSpacerTypeProperties(BusinessSystemSession e1Session, string spacerType)
        {
            var ret = new Dictionary<decimal, string>();
            try
            {
                var cmd = (e1Session.DbCommands.ContainsKey("SpacerManager.GetSpacerTypeFromC")) ? e1Session.DbCommands["SpacerManager.GetSpacerTypeFromC"] : GetSpacerTypeFromC();
                cmd.Parameters[0].Value = spacerType;
                cmd.Connection = e1Session.DbConnection;
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
                while (reader.Read())
                {
                    ret.Add(CastDbNull.To<decimal>(reader["RDBSEQ"], 0), CastDbNull.To(reader["RDTVAL"], string.Empty).Trim());
                }
                reader.Close();
            }
            catch (Exception err)
            {
                throw new ApplicationException(err.ToString());
            }
            return ret;
        }

        public iDB2Command GetSpacerItemFromQ()
        {
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText =
                @"SELECT 
	                    RDITM               
                    FROM 
	                    F3283
                    WHERE 
                        RDRTBT = 'Q' AND
                        RDTBLC = ? AND
                        RDMCU = ? AND
                        RDTSV1 = ? AND
                        RDTSV2 = ?";
            cmd.Parameters.Add(new iDB2Parameter("TableName", iDB2DbType.iDB2Graphic, 10));
            cmd.Parameters.Add(new iDB2Parameter("Plant", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("ChannelWidth", iDB2DbType.iDB2Graphic, 40));
            cmd.Parameters.Add(new iDB2Parameter("ChannelType", iDB2DbType.iDB2Graphic, 40));
            return cmd;
        }

        public iDB2Command GetSpacerTypeFromC()
        {
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText =
                @"SELECT 
	                    RDBSEQ,
                        RDTVAL               
                    FROM 
	                    F3283
                    WHERE 
                        RDRTBT = 'C' AND
                        RDTBLC = 'IGSPTY' AND
                        RDTSV1 = ?";
            cmd.Parameters.Add(new iDB2Parameter("ChannelType", iDB2DbType.iDB2Graphic, 40));
            return cmd;
        }

        public iDB2Command GetSpacerWallFromC()
        {
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText =
                @"SELECT 
	                    RDBSEQ,
                        RDTVAL               
                    FROM 
	                    F3283
                    WHERE 
                        RDRTBT = 'C' AND
                        RDTBLC = 'IGSPWL' AND
                        RDMCU = ? AND
                        RDTSV1 = ? AND
                        RDTSV2 = ?";
            cmd.Parameters.Add(new iDB2Parameter("Plant", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("ChannelWidth", iDB2DbType.iDB2Graphic, 40));
            cmd.Parameters.Add(new iDB2Parameter("ChannelType", iDB2DbType.iDB2Graphic, 40));
            return cmd;
        }
    }
}
