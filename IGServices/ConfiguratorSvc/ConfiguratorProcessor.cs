using System;
using System.Collections.Specialized;
using System.ServiceModel;

namespace IGServices.ConfiguratorSvc
{
    public class ConfiguratorProcessor
    {
        public int GetLogoCorner(string customerName, int shapeNum, decimal baseDim, decimal leftDim)
        {
            try
            {
                //var values = new StringDictionary();
                //values.Add("Customer", customerName);
                //values.Add("Shape", shapeNum.ToString());
                //values.Add("Base", baseDim.ToString());
                //values.Add("Left", leftDim.ToString());
                //var apms = new ApmsLogic.ApmsResults("CGD1SQL8DV01", "APMS_Config");
                //return apms.GetLogoCorner(values);
                return 0;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
