using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using SoaCore.Wcf;

namespace IGServices.ConfiguratorSvc
{
    [ServiceContract]
    public interface IConfiguratorService
    {
        [OperationContract, OperationInvokerLogger]
        int GetLogoCorner(string customerName, int shapeNum, decimal baseDim, decimal leftDim);
    }

    [ServiceContract]
    public interface IConfiguratorRestService
    {
        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/GetLogoCorner?Customer={customerName}&Shape={shapeNum}&Base={baseDim}&Left={leftDim}")]
        int GetLogoCorner(string customerName, string shapeNum, string baseDim, string leftDim);
    }
}
