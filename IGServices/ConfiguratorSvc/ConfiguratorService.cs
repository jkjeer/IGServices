using System;
using SoaCore.Wcf;

namespace IGServices.ConfiguratorSvc
{
    [ParameterInspectorLogger, CGIErrorHandler]
    public class ConfiguratorService : IConfiguratorService
    {
        public int GetLogoCorner(string customerName, int shapeNum, decimal baseDim, decimal leftDim)
        {
            var processor = new ConfiguratorProcessor();
            return processor.GetLogoCorner(customerName, shapeNum, baseDim, leftDim);
        }
    }

    public class ConfiguratorRestService : IConfiguratorRestService
    {
        public int GetLogoCorner(string customerName, string shapeNum, string baseDim, string leftDim)
        {
            var processor = new ConfiguratorProcessor();
            return processor.GetLogoCorner(customerName, Convert.ToInt32(shapeNum), Convert.ToDecimal(baseDim), Convert.ToDecimal(leftDim));
        }
    }
}
