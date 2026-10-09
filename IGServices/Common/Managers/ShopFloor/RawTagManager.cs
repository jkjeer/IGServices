using System;
using System.Data;
using Microsoft.Data.SqlClient;
using IGServices.Common.Managers.Enterprise;
using IGServices.Common.Types.ShopFloor;
using IGServices.Common.Utilities;

namespace IGServices.Common.Managers.ShopFloor
{
    public class RawTagManager
    {
        
        public RawTagManager()
        {
        }

        /*********************************************************************************
         * Prepared Commands
         ********************************************************************************/
        private string rawTagsSelectClause =
            @"ItemNum,
                SerialNum,
                Writtendate,
                VenderID,
                BaseL,
                BaseN,
                BaseD,
                LeftL,
                LeftN,
                LeftD,
                RightL,
                RightN,
                RightD,
                TopL,
                TopN,
                TopD,
                ShapeNum,
                Lite1_Thick,
                Lite1_Color,
                Lite1_Heat,
                Lite1_1stSrfCoating,
                Lite1_2ndSrfCoating,
                QtyOnHand,
                ReceivedDate,
                ExpirationDate,
                SqftActual,
                SqftBillable,
                Side1L,
                Side1N,
                Side1D,
                Side2L,
                Side2N,
                Side2D,
                Location,
                Pattern,
                RackId,
                RawCTCFCode,
                RawTagsID,
                ReceivedQty,
                Status,
                Lite1_ProdQualityLevel,
                ProductionDate,
                ProcessID,
                Lite1_Lami,
                RackType,
                LitesPerRack,
                SequenceNum";

        public SqlCommand GetRawTagsByRackId()
        {
            var cmd = new SqlCommand();
            try
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = String.Format(
                    @"SELECT
                        {0}
                    FROM 
                        dbo.RawTags
                    WHERE 
                        RackId = @RackId", rawTagsSelectClause); 
                cmd.Parameters.Add(new SqlParameter("@RackId", SqlDbType.Decimal));
                cmd.Parameters["@RackId"].Precision = 15;
                cmd.Parameters["@RackId"].Scale = 0;
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
            return cmd;
        }

        public SqlCommand GetRawTagsBySerialNum()
        {
            var cmd = new SqlCommand();
            try
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = String.Format(
                    @"SELECT
                        {0}
                    FROM 
                        dbo.RawTags
                    WHERE 
                        SerialNum = @SerialNum", rawTagsSelectClause);
                cmd.Parameters.Add(new SqlParameter("@SerialNum", SqlDbType.VarChar, 30));
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
            return cmd;
        }

        public SqlCommand GetRawTagsByRackSerial()
        {
            var cmd = new SqlCommand();
            try
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = String.Format(
                    @"SELECT
                        {0}
                    FROM 
                        dbo.RawTags
                    WHERE 
                        RackId = @RackId AND
                        SerialNum = @SerialNum", rawTagsSelectClause);
                cmd.Parameters.Add(new SqlParameter("@RackId", SqlDbType.Decimal));
                cmd.Parameters["@RackId"].Precision = 15;
                cmd.Parameters["@RackId"].Scale = 0;
                cmd.Parameters.Add(new SqlParameter("@SerialNum", SqlDbType.VarChar, 30));
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
            return cmd;
        }

/*********************************************************************************
 * Manager methods
 ********************************************************************************/
        public RawTags GetRawTags(string plantNum, decimal rackId, string serialNum)
        {
            var tags = new RawTags();
            var cn = new SqlConnection();
            try
            {
                var cmd = new SqlCommand();
                if (rackId == 0 & serialNum != "")
                {
                    cmd = GetRawTagsBySerialNum();
                    cmd.Parameters["@SerialNum"].Value = serialNum;
                }
                else
                {
                    if (rackId != 0 & serialNum == "")
                    {
                        cmd = GetRawTagsByRackId();
                        cmd.Parameters["@RackId"].Value = rackId;
                    }
                    else
                    {
                        if (rackId != 0 & serialNum != "")
                        {
                            cmd = GetRawTagsByRackSerial();
                            cmd.Parameters["@RackId"].Value = rackId;
                            cmd.Parameters["@SerialNum"].Value = serialNum;
                        } 
                        else
                        {
                            throw new ApplicationException("Invalid RawTags Query ... Rack Id and/or Serial Num must be Provided");
                        }
                    }
                }

                cn = ConnectionManager.GetSqlConnectionFromKey(plantNum + ".SHOPFLOOR");
                cn.Open();
                cmd.Connection = cn;
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
                while (reader.Read())
                {
                    var tag = new RawTag();
                    tag.ItemNum = CastDbNull.To<string>(reader["ItemNum"], String.Empty).Trim();
                    tag.SerialNum = CastDbNull.To<string>(reader["SerialNum"], String.Empty).Trim();
                    tag.Writtendate = CastDbNull.To<DateTime>(reader["Writtendate"], DateTime.MinValue);
                    tag.VenderID = CastDbNull.To<string>(reader["VenderID"], String.Empty).Trim();
                    tag.BaseL = CastDbNull.To<decimal>(reader["BaseL"], 0);
                    tag.BaseN = CastDbNull.To<decimal>(reader["BaseN"], 0);
                    tag.BaseD = CastDbNull.To<decimal>(reader["BaseD"], 0);
                    tag.LeftL = CastDbNull.To<decimal>(reader["LeftL"], 0);
                    tag.LeftN = CastDbNull.To<decimal>(reader["LeftN"], 0);
                    tag.LeftD = CastDbNull.To<decimal>(reader["LeftD"], 0);
                    tag.RightL = CastDbNull.To<decimal>(reader["RightL"], 0);
                    tag.RightN = CastDbNull.To<decimal>(reader["RightN"], 0);
                    tag.RightD = CastDbNull.To<decimal>(reader["RightD"], 0);
                    tag.TopL = CastDbNull.To<decimal>(reader["TopL"], 0);
                    tag.TopN = CastDbNull.To<decimal>(reader["TopN"], 0);
                    tag.TopD = CastDbNull.To<decimal>(reader["TopD"], 0);
                    tag.ShapeNum = CastDbNull.To<decimal>(reader["ShapeNum"], 0);
                    tag.Lite1_Thick = CastDbNull.To<decimal>(reader["Lite1_Thick"], 0);
                    tag.Lite1_Color = CastDbNull.To<string>(reader["Lite1_Color"], String.Empty).Trim();
                    tag.Lite1_Heat = CastDbNull.To<string>(reader["Lite1_Heat"], String.Empty).Trim();
                    tag.Lite1_1stSrfCoating = CastDbNull.To<string>(reader["Lite1_1stSrfCoating"], String.Empty).Trim();
                    tag.Lite1_2ndSrfCoating = CastDbNull.To<string>(reader["Lite1_2ndSrfCoating"], String.Empty).Trim();
                    tag.QtyOnHand = CastDbNull.To<decimal>(reader["QtyOnHand"], 0);
                    tag.ReceivedDate = CastDbNull.To<DateTime>(reader["ReceivedDate"], DateTime.MinValue);
                    tag.ExpirationDate = CastDbNull.To<DateTime>(reader["ExpirationDate"], DateTime.MinValue);
                    tag.SqftActual = CastDbNull.To<decimal>(reader["SqftActual"], 0);
                    tag.SqftBillable = CastDbNull.To<decimal>(reader["SqftBillable"], 0);
                    tag.Side1L = CastDbNull.To<decimal>(reader["Side1L"], 0);
                    tag.Side1N = CastDbNull.To<decimal>(reader["Side1N"], 0);
                    tag.Side1D = CastDbNull.To<decimal>(reader["Side1D"], 0);
                    tag.Side2L = CastDbNull.To<decimal>(reader["Side2L"], 0);
                    tag.Side2N = CastDbNull.To<decimal>(reader["Side2N"], 0);
                    tag.Side2D = CastDbNull.To<decimal>(reader["Side2D"], 0);
                    tag.Location = CastDbNull.To<string>(reader["Location"], String.Empty).Trim();
                    tag.Pattern = CastDbNull.To<string>(reader["Pattern"], String.Empty).Trim();
                    tag.RackId = CastDbNull.To<decimal>(reader["RackId"], 0);
                    tag.RawCTCFCode = CastDbNull.To<string>(reader["RawCTCFCode"], String.Empty).Trim();
                    tag.RawTagsID = CastDbNull.To<int>(reader["RawTagsID"], 0);
                    tag.ReceivedQty = CastDbNull.To<decimal>(reader["ReceivedQty"], 0);
                    tag.Status = CastDbNull.To<string>(reader["Status"], String.Empty).Trim();
                    tag.Lite1_ProdQualityLevel = CastDbNull.To<string>(reader["Lite1_ProdQualityLevel"], String.Empty).Trim();
                    tag.ProductionDate = CastDbNull.To<DateTime>(reader["ProductionDate"], DateTime.MinValue);
                    tag.ProcessID = CastDbNull.To<string>(reader["ProcessID"], String.Empty).Trim();
                    tag.Lite1_Lami = CastDbNull.To<string>(reader["Lite1_Lami"], String.Empty).Trim();
                    tag.RackType = CastDbNull.To<string>(reader["RackType"], String.Empty).Trim();
                    tag.LitesPerRack = CastDbNull.To<decimal>(reader["LitesPerRack"], 0);
                    tag.SequenceNum = CastDbNull.To<decimal>(reader["SequenceNum"], 0);
                    tags.Add(tag);
                }
                reader.Close();
                return tags;
            }
            catch (Exception err)
            {
                throw new ApplicationException(err.ToString());
            }
            finally
            {
                cn.Close();
            }
        }

    }
}
