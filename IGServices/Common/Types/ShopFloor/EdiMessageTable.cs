using System;
using System.Data;
using System.Linq;
using System.ServiceModel;
using IGServices.Common.Utilities;

namespace IGServices.Common.Types.ShopFloor
{
    [System.ComponentModel.DesignerCategory("Code")]
    public class EdiMessageTable : DataTable
    {

        public DateTime MaxDate
        {
            get { return (from a in this.AsEnumerable() select a.Field<DateTime>("UPDATE_DATE")).Max(); }
            set { }
        }

        public EdiMessageTable()
        {
            ExtendedProperties.Add("Target", "[ORD].[EDI_MESSAGE_LEDGER]");
            Columns.Add("ORDER_NUM", typeof(decimal));
            Columns.Add("ORDER_TYPE_CD", typeof(string));
            Columns.Add("ORDER_LINE_NUM", typeof(decimal));
            Columns.Add("MESSAGE_LINE_NUM", typeof(decimal));
            Columns.Add("MESSAGE_TYPE_CD", typeof(string));
            Columns.Add("MESSAGE_SEQ_NUM", typeof(decimal));
            Columns.Add("MESSAGE_TEXT", typeof(string));
            Columns.Add("UPDATE_DATE", typeof(DateTime));
        }

        public void Add(IDataReader reader)
        {
            try
            {
                while (reader.Read())
                {
                    DataRow dr = this.NewRow();
                    dr["ORDER_NUM"] = CastDbNull.To<decimal>(reader["ZDDOCO"], 0);
                    dr["ORDER_TYPE_CD"] = CastDbNull.To<string>(reader["ZDDCTO"], String.Empty).Trim();
                    dr["ORDER_LINE_NUM"] = CastDbNull.To<decimal>(reader["ZDLNID"], 0) / 1000;
                    dr["MESSAGE_LINE_NUM"] = CastDbNull.To<decimal>(reader["ZDLINS"], 0) / 100;
                    dr["MESSAGE_TYPE_CD"] = CastDbNull.To<string>(reader["ZDEDTY"], String.Empty).Trim();
                    dr["MESSAGE_SEQ_NUM"] = CastDbNull.To<decimal>(reader["ZDEDSQ"], 0);
                    dr["MESSAGE_TEXT"] = CastDbNull.To<string>(reader["ZDTXLN"], String.Empty).Trim();
                    dr["UPDATE_DATE"] = Timestamp.CastToDateTime(Data.CastDbNull.To<decimal>(reader["ZDUPMJ"], 0),
                        Data.CastDbNull.To<decimal>(reader["ZDTDAY"], 0));
                    this.Rows.Add(dr);
                }
                reader.Close();
            }
            catch (Exception e)
            {
                throw new FaultException(e.Message);
            }
        }
    }
}
