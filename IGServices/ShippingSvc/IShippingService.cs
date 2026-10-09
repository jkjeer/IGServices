using System;
using System.Data;
using System.ServiceModel;
using System.ServiceModel.Web;
using IGServices.Common.Types.Response;
using SoaCore.Wcf;

namespace IGServices.ShippingSvc
{
    [ServiceContract]
    public interface IShippingService
    {
        [OperationContract, OperationInvokerLogger]
        ProcessResponse DownloadShippableRacks(string plant);
    }

    [ServiceContract]
    public interface IShippingRestService
    {
        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/DownloadShippableRacks?Plant={plant}")]
        ProcessResponse DownloadShippableRacks(string plant);
    }
}


