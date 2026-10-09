using System.Runtime.Serialization;

namespace IGServices.Common.Types.Response
{
    public class SpacerItemsResponse : BaseResponse
    {
        public SpacerItemsResponse()
        {
            ProcessName = "SpacerItems";
            SpacerItems = new Enterprise.SpacerItems();
            SpacerItemsSummary = new SpacerItemsSummary();
        }

        public SpacerItemsResponse(string arguments)
        {
            ProcessName = "SpacerItems";
            SpacerItems = new Enterprise.SpacerItems();
            SpacerItemsSummary = new SpacerItemsSummary();
        }

        [DataMember(Order = 10)]
        public decimal ItemNum { get; set; }

        [DataMember(Order = 20)]
        public Enterprise.SpacerItems SpacerItems { get; set; }

        [DataMember(Order = 30)]
        public SpacerItemsSummary SpacerItemsSummary { get; set; }
    }

    [DataContract(Namespace = "")]
    public class SpacerItemsSummary
    {
        public SpacerItemsSummary()
        {
            Items = 0;
            SourceRows = 0;
            Quantity = 0;
        }

        [DataMember(Order = 10)]
        public short Items { get; set; }
        [DataMember(Order = 20)]
        public short SourceRows { get; set; }
        [DataMember(Order = 30)]
        public short Quantity { get; set; }
    }
}
