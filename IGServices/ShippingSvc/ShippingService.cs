using System;
using System.Data;
using IGServices.Common.Types.Response;

namespace IGServices.ShippingSvc
{
    public class ShippingService : IShippingService
    {
        public ProcessResponse DownloadShippableRacks(string plantCode)
        {
            var processor = new ShippingProcessor();
            try
            {
                return processor.DownloadShippableRacks(plantCode);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }

    public class ShippingRestService : IShippingRestService
    {
        public ProcessResponse DownloadShippableRacks(string plantCode)
        {
            var processor = new ShippingProcessor();
            try
            {
                return processor.DownloadShippableRacks(plantCode);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
