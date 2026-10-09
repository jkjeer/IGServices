using System;
using System.ServiceModel;
using System.ServiceModel.Web;
using IGServices.Common.Managers.Process;
using IGServices.Common.Types.Enterprise;
using IGServices.Common.Types.Response;
using IGServices.Common.Types.ShopFloor;
using SoaCore.Wcf;
using SpacerItems = IGServices.Common.Types.Enterprise.SpacerItems;

namespace IGServices.ItemSvc
{
    [ServiceContract]
    public interface IItemService
    {

        // Get
        [OperationContract, OperationInvokerLogger]
        ItemAttributes GetGlassItemAttributes(decimal itemNum, decimal documentNum, string documentType, decimal documentLineNum, string companyCode);

        [OperationContract, OperationInvokerLogger]
        GlassItem GetGlassItem(decimal itemNum, string plantCode, decimal documentNum, string documentType, decimal documentLineNum, string companyCode);

        [OperationContract, OperationInvokerLogger]
        GlassItems GetDiscreteGlassItems(string plantCode, decimal itemNum = 0, string stockingTypes = null, string categoryCodes = null, TimeSpan? timeSpan = null);

        [OperationContract, OperationInvokerLogger]
        SpacerItems GetSpacerItems(string plantCode, TimeSpan timeSpan, bool useHighWaterMark = false, decimal itemNum = 0);

        [OperationContract, OperationInvokerLogger]
        MuntinBarItems GetMuntinBarItems(string plantCode, TimeSpan timeSpan, bool useHighWaterMark = false, decimal itemNum = 0);

        // Download
        [OperationContract, OperationInvokerLogger]
        ProcessResponse DownloadItems(string plants, TimeSpan timeSpan, bool useHighWaterMark = false, bool parallel = false);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse DownloadGlassItems(string plantCode, TimeSpan timeSpan, bool useHighWaterMark = false, bool parallel = false, decimal itemNum = 0, string stockTypes = null);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse DownloadSpacerItems(string plantCode, TimeSpan timeSpan, bool useHighWaterMark, bool parallel = false, decimal itemNum = 0);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse DownloadMuntinBarItems(string plantCode, TimeSpan timeSpan, bool useHighWaterMark, bool parallel = false, decimal itemNum = 0);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse DownloadMuntinCodes(string plantCode, TimeSpan timeSpan, bool useHighWaterMark = false, bool parallel = false);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse DownloadMtoSpecUnits(string plant, TimeSpan timeSpan, bool useHighWaterMark = false, bool parallel = false, string documentTypes = null, decimal documentNum = 0);

        [OperationContract, OperationInvokerLogger]
        ProcessResponse DownloadMtsSpecUnits(string plant, TimeSpan timeSpan, bool useHighWaterMark = false, bool parallel = false, decimal itemNum = 0);


    }

    [ServiceContract]
    public interface IItemRestService
    {
        // Get
        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/GetGlassItemAttributes?ItemNum={itemNum}&DocumentNum={documentNum}&DocumentType={documentType}&DocumentLineNum={documentLineNum}&CompanyCode={companyCode}")]
        ItemAttributes GetGlassItemAttributes(string itemNum, string documentNum, string documentType, string documentLineNum, string companyCode);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/GetGlassItem?ItemNum={itemNum}&Plant={plantCode}&DocumentNum={documentNum}&DocumentType={documentType}&DocumentLineNum={documentLineNum}&CompanyCode={companyCode}")]
        GlassItem GetGlassItem(string itemNum, string plantCode, string documentNum, string documentType, string documentLineNum, string companyCode);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/GetDiscreteGlassItems?Plant={plantCode}&ItemNum={itemNum}&StockingTypes={stockingTypes}&CategoryCodes={categoryCodes}&TimeSpan={timeSpan}")]
        GlassItems GetDiscreteGlassItems(string plantCode, decimal itemNum, string stockingTypes, string categoryCodes, TimeSpan timespan);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/GetSpacerItems?Plant={plantCode}&LookbackSpan={span}&UseHighWaterMark={useHighWaterMark}&ItemNum={itemNum}")]
        SpacerItems GetSpacerItems(string plantCode, TimeSpan span, bool useHighWaterMark, decimal itemNum);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/GetMuntinBarItems?Plant={plantCode}&LookbackSpan={span}&UseHighWaterMark={useHighWaterMark}&ItemNum={itemNum}")]
        MuntinBarItems GetMuntinBarItems(string plantCode, TimeSpan span, bool useHighWaterMark, decimal itemNum);

        // Downloads
        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/DownloadItems?Plant={plants}&TimeSpan={timeSpan}&HighWaterMark={useHighWaterMark}&Parallel={parallel}")]
        ProcessResponse DownloadItems(string plants, TimeSpan timeSpan, bool useHighWaterMark, bool parallel);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/DownloadGlassItems?Plant={plantCode}&TimeSpan={timeSpan}&HighWaterMark={useHighWaterMark}&Parallel={parallel}&ItemNum={itemNum}&StockTypes={stockTypes}")]
        ProcessResponse DownloadGlassItems(string plantCode, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal itemNum, string stockTypes);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/DownloadMuntinBarItems?Plant={plantCode}&TimeSpan={timeSpan}&HighWaterMark={useHighWaterMark}&Parallel={parallel}&ItemNum={itemNum}")]
        ProcessResponse DownloadMuntinBarItems(string plantCode, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal itemNum);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/DownloadSpacerItems?Plant={plantCode}&TimeSpan={timeSpan}&HighWaterMark={useHighWaterMark}&Parallel={parallel}&ItemNum={itemNum}")]
        ProcessResponse DownloadSpacerItems(string plantCode, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal itemNum);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/DownloadMuntinCodes?Plant={plantCode}&TimeSpan={timeSpan}&HighWaterMark={useHighWaterMark}&Parallel={parallel}")]
        ProcessResponse DownloadMuntinCodes(string plantCode, TimeSpan timeSpan, bool useHighWaterMark, bool parallel);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/DownloadMtoSpecUnits?Plant={plant}&TimeSpan={timeSpan}&HighWaterMark={useHighWaterMark}&Parallel={parallel}&DocumentTypes={documentTypes}&DocumentNum={documentNum}")]
        ProcessResponse DownloadMtoSpecUnits(string plant, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, string documentTypes, decimal documentNum);

        [OperationContract, OperationInvokerLogger]
        [WebGet(UriTemplate = "/DownloadMtsSpecUnits?Plant={plant}&TimeSpan={timeSpan}&HighWaterMark={useHighWaterMark}&Parallel={parallel}&ItemNum={itemNum}")]
        ProcessResponse DownloadMtsSpecUnits(string plant, TimeSpan timeSpan, bool useHighWaterMark, bool parallel, decimal itemNum);

    }
}
