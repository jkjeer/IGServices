using System.Collections.Generic;

namespace IGServices.Common.Types.Enterprise
{
    public class PlantNumMap
    {

        public Dictionary<string, string> Values { get; set; }

        public PlantNumMap()
        {
            Values = new Dictionary<string, string>();
            Values.Add("01", "MPIG");
            Values.Add("02", "GFIG");
            Values.Add("03", "SGIG");
            Values.Add("04", "NFCG");
            Values.Add("05", "SGCG");
            Values.Add("06", "TOIG");
            Values.Add("07", "TOTG");
            Values.Add("08", "AMLG");
            Values.Add("09", "MNFG");
            Values.Add("10", "BFCG");
            Values.Add("11", "TWCG");
            Values.Add("12", "PTFG");
            Values.Add("14", "FMIG");
            Values.Add("15", "MVCG");
            Values.Add("16", "FRIG");
            Values.Add("17", "OCIG");
            Values.Add("18", "MRFG");
            Values.Add("19", "WXIG");
            Values.Add("20", "GTCG");
            Values.Add("21", "WXCG");
            Values.Add("22", "HRIG");
            Values.Add("23", "DRFG");
            Values.Add("24", "CGCG");
            Values.Add("25", "ROIG");
            Values.Add("26", "LLCG");
            Values.Add("27", "WLFG");
            Values.Add("28", "WBIG");
            Values.Add("29", "OCLG");
            Values.Add("30", "CHTG");
            Values.Add("31", "DRML");
            Values.Add("32", "MZST");
            Values.Add("33", "SGSC");
            Values.Add("34", "WKIG");
            Values.Add("35", "SGAG");
            Values.Add("36", "SGRT");
            Values.Add("77", "BFIG");
        }

        public string GetPlantCode(string plantNum)
        {
            return this.Values[plantNum];
        }

    }
}
