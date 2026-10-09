using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
// ReSharper disable InconsistentNaming

namespace IGServices.Common.Types.ShopFloor
{
    [CollectionDataContract(Namespace = "")]
    public class QCInspections : List<QCInspection>
    {
    }

    [DataContract(Namespace = "")]
    public class QCInspection
    {
        public QCInspection()
        {
            //TransferDate = DateTime.Now.ToLocalTime();
            //IsValid = true;
        }

        [DataMember(Order = 10)]
        public string PlantNum { get; set; }
    }
}
