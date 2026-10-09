using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

// ReSharper disable InconsistentNaming

namespace IGServices.Common.Types.ShopFloor
{
    [CollectionDataContract(Namespace = "")]
    public class ScheduleInfoMTSLines : List<ScheduleInfoMTSLine>
    {
    }

    [DataContract(Namespace = "")]
    public class ScheduleInfoMTSLine
    {
        public ScheduleInfoMTSLine()
        {
            PlantNum = "";
            ScheduleNum = 0;
            ScheduleSeq = 0;
            ScheduleDate = new DateTime(1900, 1, 1);
            ScheduleDateE1 = 0;
            ScheduledQty = 0;
            WOrderNum = 0;
            OrderLine = 0;
            WOrderType = "";
            LineID = 0;
            ItemNumber = "";
            Dimensions = "";
            Construction = "";
            Customer = "";
            Lite1_Inventory = 0;
            Lite2_Inventory = 0;
            Lite3_Inventory = 0;
            Finished_Inventory = 0;
            Lite1_ReleaseNote = "";
            Lite1_ReleaseTimeNote = "";
            Lite2_ReleaseNote = "";
            Lite2_ReleaseTimeNote = "";
            Lite3_ReleaseNote = "";
            Lite3_ReleaseTimeNote = "";
            Completed = false;
            MuntinComplete = false;
            ChannelComplete = false;
            GlassComplete = false;
            PackingComplete = false;
            InterLayerComplete = false;
            AutoClaveComplete = false;
            ChangedNote = "";
            SalesMsg1 = "";
            SalesMsg2 = "";
            SalesMsg3 = "";
            SalesMsg4 = "";
            ProcessMsg1 = "";
            ProcessMsg2 = "";
            ShapeNum = "";
            RackType = "";
            LitesPerRack = 0;
            E1BatchID = "";
            PlantID = "";
            WrittenDate = new DateTime(1900, 1, 1);
        }

        [DataMember(Order = 0, EmitDefaultValue = false)]
        public string PlantNum { get; set; }

        [DataMember(Order = 1, EmitDefaultValue = false)]
        public decimal ScheduleNum { get; set; }

        [DataMember(Order = 2, EmitDefaultValue = false)]
        public decimal ScheduleSeq { get; set; }

        [DataMember(Order = 3, EmitDefaultValue = false)]
        public DateTime ScheduleDate { get; set; }

        [DataMember(Order = 4, EmitDefaultValue = false)]
        public decimal ScheduleDateE1 { get; set; }

        [DataMember(Order = 5, EmitDefaultValue = false)]
        public decimal ScheduledQty { get; set; }

        [DataMember(Order = 6, EmitDefaultValue = false)]
        public decimal WOrderNum { get; set; }

        [DataMember(Order = 7, EmitDefaultValue = false)]
        public decimal OrderLine { get; set; }

        [DataMember(Order = 8, EmitDefaultValue = false)]
        public string WOrderType { get; set; }

        [DataMember(Order = 9, EmitDefaultValue = false)]
        public int LineID { get; set; }

        [DataMember(Order = 91, EmitDefaultValue = false)]
        public string ItemNumber { get; set; }

        [DataMember(Order = 10, EmitDefaultValue = false)]
        public string Dimensions { get; set; }

        [DataMember(Order = 11, EmitDefaultValue = false)]
        public string Construction { get; set; }

        [DataMember(Order = 12, EmitDefaultValue = false)]
        public string Customer { get; set; }

        [DataMember(Order = 13, EmitDefaultValue = false)]
        public int Lite1_Inventory { get; set; }

        [DataMember(Order = 14, EmitDefaultValue = false)]
        public int Lite2_Inventory { get; set; }

        [DataMember(Order = 15, EmitDefaultValue = false)]
        public int Lite3_Inventory { get; set; }

        [DataMember(Order = 16, EmitDefaultValue = false)]
        public int Finished_Inventory { get; set; }

        [DataMember(Order = 17, EmitDefaultValue = false)]
        public string Lite1_ReleaseNote { get; set; }

        [DataMember(Order = 18, EmitDefaultValue = false)]
        public string Lite1_ReleaseTimeNote { get; set; }

        [DataMember(Order = 19, EmitDefaultValue = false)]
        public string Lite2_ReleaseNote { get; set; }

        [DataMember(Order = 20, EmitDefaultValue = false)]
        public string Lite2_ReleaseTimeNote { get; set; }

        [DataMember(Order = 21, EmitDefaultValue = false)]
        public string Lite3_ReleaseNote { get; set; }

        [DataMember(Order = 22, EmitDefaultValue = false)]
        public string Lite3_ReleaseTimeNote { get; set; }

        [DataMember(Order = 23, EmitDefaultValue = false)]
        public bool Completed { get; set; }

        [DataMember(Order = 24, EmitDefaultValue = false)]
        public bool MuntinComplete { get; set; }

        [DataMember(Order = 25, EmitDefaultValue = false)]
        public bool ChannelComplete { get; set; }

        [DataMember(Order = 26, EmitDefaultValue = false)]
        public bool GlassComplete { get; set; }

        [DataMember(Order = 27, EmitDefaultValue = false)]
        public bool PackingComplete { get; set; }

        [DataMember(Order = 28, EmitDefaultValue = false)]
        public bool InterLayerComplete { get; set; }

        [DataMember(Order = 29, EmitDefaultValue = false)]
        public bool AutoClaveComplete { get; set; }

        [DataMember(Order = 30, EmitDefaultValue = false)]
        public string ChangedNote { get; set; }

        [DataMember(Order = 31, EmitDefaultValue = false)]
        public string SalesMsg1 { get; set; }

        [DataMember(Order = 32, EmitDefaultValue = false)]
        public string SalesMsg2 { get; set; }

        [DataMember(Order = 33, EmitDefaultValue = false)]
        public string SalesMsg3 { get; set; }

        [DataMember(Order = 34, EmitDefaultValue = false)]
        public string SalesMsg4 { get; set; }

        [DataMember(Order = 35, EmitDefaultValue = false)]
        public string ProcessMsg1 { get; set; }

        [DataMember(Order = 36, EmitDefaultValue = false)]
        public string ProcessMsg2 { get; set; }

        [DataMember(Order = 37, EmitDefaultValue = false)]
        public string ShapeNum { get; set; }

        [DataMember(Order = 38, EmitDefaultValue = false)]
        public string RackType { get; set; }

        [DataMember(Order = 39, EmitDefaultValue = false)]
        public decimal LitesPerRack { get; set; }

        [DataMember(Order = 40, EmitDefaultValue = false)]
        public string E1BatchID { get; set; }

        [DataMember(Order = 41, EmitDefaultValue = false)]
        public string PlantID { get; set; }

        [DataMember(Order = 42, EmitDefaultValue = false)]
        public DateTime WrittenDate { get; set; }
    }
}