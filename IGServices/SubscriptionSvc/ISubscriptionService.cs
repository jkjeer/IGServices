using System;
using System.ServiceModel;
using System.ServiceModel.Web;
using IGServices.Common.Types.Enterprise;
using IGServices.Common.Types.Response;
using SoaCore.Wcf;

namespace IGServices.SubscriptionSvc
{
    [ServiceContract]
    public interface ISubscriptionService
    {
        [OperationContract, OperationInvokerLogger]
        Plants GetPlants(string code);

        [OperationContract, OperationInvokerLogger]
        PlantGroups GetPlantGroups();

        [OperationContract, OperationInvokerLogger]
        SubscriptionTypes GetSubscriptionTypes(string code);

        [OperationContract, OperationInvokerLogger]
        SubscriptionTypeItems RefreshSubscriptionTypeCache();

        [OperationContract, OperationInvokerLogger]
        ProcessResponse RunSubscriptionType(string plant, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string subscriptionType);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse InitHighWaterMarks(string plant, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string markName);
    }

    [ServiceContract]
    public interface ISubscriptionRestService
    {
        [OperationContract, OperationInvokerLogger]
        [WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "/GetPlants?Plant={plant}")]
        Plants GetPlants(string plant);

        [OperationContract, OperationInvokerLogger]
        [WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "/GetPlantGroups")]
        PlantGroups GetPlantGroups();

        [OperationContract, OperationInvokerLogger]
        [WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "/GetSubscriptionTypes?SubscriptionType={subscriptionType}")]
        SubscriptionTypes GetSubscriptionTypes(string subscriptionType);

        [OperationContract, OperationInvokerLogger]
        [WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "/RefreshSubscriptionTypeCache")]
        SubscriptionTypeItems RefreshSubscriptionTypeCache();

        [OperationContract, OperationInvokerLogger]
        [WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "/RunSubscriptionType?Plant={plant}&TimeSpan={timeSpan}&HighWaterMark={useHighWaterMark}&Parallel={parallel}&SubscriptionType={subscriptionType}")]
        ProcessResponse RunSubscriptionType(string plant, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string subscriptionType);

        [OperationContract, OperationInvokerLogger]
        [WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "/InitHighWaterMarks?Plant={plant}&TimeSpan={timeSpan}&HighWaterMark={useHighWaterMark}&Parallel={parallel}&MarkName={markName}")]
        ProcessResponse InitHighWaterMarks(string plant, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string markName);
    }
}
