using System;
using System.Data;
using System.Linq;
using IGServices.Common.Managers.E1;
using IGServices.Common.Types.Enterprise;
using IGServices.Common.Utilities;

namespace IGServices.Common.Types.ShopFloor
{
    [System.ComponentModel.DesignerCategory("Code")]
    public class RackAssetTable : DataTable
    {

        public DateTime MaxDate
        {
            get { return (from a in this.AsEnumerable() select a.Field<DateTime>("TransactionDate")).Max(); }
            set { }
        }

        public RackAssetTable()
        {
            ExtendedProperties.Add("Target", "[EBS].[RackAssetLedger]");
            Columns.Add("AssetId", typeof(string));
            Columns.Add("Plant", typeof(string));
            Columns.Add("Location", typeof(string));
            Columns.Add("Status", typeof(string));
            Columns.Add("CustomerSoldNum", typeof(decimal));
            Columns.Add("CustomerShipNum", typeof(decimal));
            Columns.Add("AssetType", typeof(string));
            Columns.Add("RackType", typeof(string));
            Columns.Add("TransactionDate", typeof(DateTime));
        }

        public void Add(IDataReader reader, Plant plant)
        {
            try
            {
                while (reader.Read())
                {
                    DataRow dr = this.NewRow();
                    dr["AssetId"] = Data.CastDbNull.To<string>(reader["AMZ58AID"], String.Empty).Trim();
                    dr["Plant"] = CastDbNull.To<string>(reader["AMMCU"], String.Empty).Trim();
                    dr["Location"] = CastDbNull.To<string>(reader["AMLOCN"], String.Empty).Trim();
                    dr["Status"] = CastDbNull.To<string>(reader["AMZ58AST"], String.Empty).Trim();
                    dr["CustomerSoldNum"] = CastDbNull.To<decimal>(reader["AMAN8"], 0);
                    dr["CustomerShipNum"] = CastDbNull.To<decimal>(reader["AMSHAN"], 0);
                    dr["AssetType"] = CastDbNull.To<string>(reader["AMZ58ATY"], String.Empty).Trim();
                    dr["RackType"] = CastDbNull.To<string>(reader["AMZ58RT"], String.Empty).Trim();
                    var updateDateId = Data.CastDbNull.To<decimal>(reader["AMUPMJ"], 0);
                    var updateTimeId = Data.CastDbNull.To<decimal>(reader["AMUPMT"], 0);
                    dr["TransactionDate"] = E1TimeManager.CastToDateTime(updateDateId, updateTimeId).AddHours(plant.TimeOffset);
                    this.Rows.Add(dr);
                }
                reader.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
