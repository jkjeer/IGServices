using System;
using System.Collections.Generic;

namespace IGServices.Common.Types.Enterprise
{
    public class PlantCodeMap
    {
        public Dictionary<string, string> Values { get; set; }

        public PlantCodeMap()
        {
            Values = new Dictionary<string, string>
            {
                {"MPIG", "01"},
                {"GFIG", "02"},
                {"SGIG", "03"},
                {"NFCG", "04"},
                {"SGCG", "05"},
                {"TOIG", "06"},
                {"TOTG", "07"},
                {"AMLG", "08"},
                {"MNFG", "09"},
                {"BFCG", "10"},
                {"TWCG", "11"},
                {"PTFG", "12"},
                {"FMIG", "14"},
                {"MVCG", "15"},
                {"FRIG", "16"},
                {"OCIG", "17"},
                {"MRFG", "18"},
                {"WXIG", "19"},
                {"GTCG", "20"},
                {"WXCG", "21"},
                {"HRIG", "22"},
                {"DRFG", "23"},
                {"CGCG", "24"},
                {"ROIG", "25"},
                {"LLCG", "26"},
                {"WLFG", "27"},
                {"WBIG", "28"},
                {"OCLG", "29"},
                {"CHTG", "30"},
                {"DRML", "31"},
                {"MZST", "32"},
                {"SGSC", "33"},
                {"WKIG", "34"},
                {"SGAG", "35"},
                {"SGRT", "36"}
            };
        }

        public string GetPlantNum(string plantCode)
        {
            try
            {
                return Values[plantCode];
            } 
            catch (Exception)
            {
                throw new ApplicationException(plantCode + " does not EXIST in Plant Map");
            }
        }
    }
}
