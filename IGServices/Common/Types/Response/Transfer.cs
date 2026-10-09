using System;
using System.Runtime.Serialization;

namespace IGServices.Common.Types.Response
{
    [DataContract(Namespace = "")]
    public class Transfer
    {
        public Transfer()
        {
            TransferDate = DateTime.Now.ToLocalTime();
            SqlBulkCopyResponses = new SqlBulkCopyResponses();
        }

        [DataMember(Order = 50, EmitDefaultValue = false)]
        public string ValidationStatus { get; set; }
        [DataMember(Order = 100, EmitDefaultValue = false)]
        public bool Transferred { get; set; }
        [DataMember(Order = 102, EmitDefaultValue = false)]
        public string TransferStatus { get; set; }
        [DataMember(Order = 103, EmitDefaultValue = false)]
        public string TransferType { get; set; }
        [DataMember(Order = 104, EmitDefaultValue = false)]
        public DateTime TransferDate { get; set; }
        [DataMember(Order = 106, EmitDefaultValue = false)]
        public long TransferBatchNum { get; set; }
        [DataMember(Order = 108, EmitDefaultValue = false)]
        public string TransferMessage { get; set; }
        [DataMember(Order = 108, EmitDefaultValue = false)]
        public DateTime BatchDate { get; set; }
        [DataMember(Order = 115, EmitDefaultValue = false)]
        public short SFUpdateBatchSize { get; set; }

        [DataMember(Order = 116, EmitDefaultValue = false)]
        public short LockExpireMinutes { get; set; }

        [DataMember(Order = 122, EmitDefaultValue = false)]
        public int Write { get; set; }

        [DataMember(Order = 300, EmitDefaultValue = false)]
        public SqlBulkCopyResponses SqlBulkCopyResponses { get; set; }
    }
}
