using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Runtime.Serialization;

namespace IGServices.Common.Types.Enterprise
{

    [CollectionDataContract(Namespace = "")]
    public class RackTagItems : List<RackTagItem>
    {
        public DataTable GetDataTable()
        {
            DataTable dt = new DataTable("RackTagItems");

            PropertyInfo[] propertyInfos;
            propertyInfos = typeof(RackTagItem).GetProperties();
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                dt.Columns.Add(propertyInfo.Name, propertyInfo.PropertyType);
                if (dt.Columns[propertyInfo.Name].DataType == typeof(DateTime))
                {
                    dt.Columns[propertyInfo.Name].DateTimeMode = DataSetDateTime.Unspecified;
                }
            }

            foreach (RackTagItem rackTag in this)
            {
                DataRow dr = dt.NewRow();
                foreach (PropertyInfo propertyInfo in propertyInfos)
                {
                    dr[propertyInfo.Name] = propertyInfo.GetValue(rackTag, null);
                }
                dt.Rows.Add(dr);
            }

            return dt;
        }
    }

    [DataContract(Namespace = "")]
    public class RackTagItem
    {

        public RackTagItem()
        {
            RackId = 0;
            RackSide = "";
            RackRow = 0;
            RackStack = 0;
            RackSlot = 0;
            RackPosition = 0;
            //RackSlotPosition = 0;
            //GroupingKey = "";
            BeginSerialNum = "";
            EndSerialNum = "";
            CompanyNum = "";
            PlantNum = "";
            SalesOrderNum = 0;
            SalesOrderLineNum = 0;
            SalesOrderType = "";
            WorkOrderNum = 0;
            ScheduleNum = 0;
            ScheduleType = "";
            CustSerialNum = "";
            ItemNum = 0;
            Location = "";
            SoldToNum = 0;
            SoldToName = "";
            ShipToNum = 0;
            ShipToName = "";
            //OrigSoldToNum = 0;
            //OrigSoldToName = "";
            //OrigShipToNum = 0;
            //OrigShipToName = "";
            CustPartNum1 = "";
            CustPartNum2 = "";
            CustPartNum3 = "";
            CustSeqNum1 = "";
            GenericSeqNum = "";
            IGLine = "";
            //ReasonCode = "";
            //RemakeFlag = "";
            RackType = "";
            RackTypeDesc = "";
            Status = "";
            StatusDate = DateTime.Now.ToLocalTime();
            BoxRotated = "";
            BoxNum = 0;
            //RackSlotsTotal = 0;
            //IsEmptySlot = false;
            //Lite1Rack = "";
            //Lite1Slot = 0;
            //Lite2Rack = "";
            //Lite2Slot = 0;
            //Lite3Rack = "";
            //Lite3Slot = 0;
            RackGlassWeight = 0;
            RackTareWeight = 0;
            Quantity = 0;
            UnitThickness = 0;
            CustomerPO = "";
            //OrigCustomerPO = "";
            ProductType = "";
            CardinalProductType = "";
            Comment1 = "";
            //ConfigId = 0;
            //CustomerLiteNumber = 0;
            Lite1Heat = "";
            Lite1Thickness = 0;
            Lite1FirstSrfCoat = "";
            Lite1SecondSrfCoat = "";
            //Lite1PatTextSrf = "";
            Lite1Color = "";
            Lite1Pattern = "";
            Lite1LamiType = "";
            Lite1LamiDesc = "";
            Lite1LamiShopFloorDesc = "";
            //Lite1TempLogo = "";
            Lite1Routing = "";
            Lite1VGroove = "";
            //Lite1EdgeWork = "";
            //Lite1AnsiLogo = "";
            //IsCoated = false;
            Lite2Heat = "";
            Lite2Thickness = 0;
            Lite2ThirdSrfCoat = "";
            Lite2FourthSrfCoat = "";
            Lite2Color = "";
            Lite2Pattern = "";
            Lite2LamiType = "";
            Lite2LamiDesc = "";
            Lite2LamiShopFloorDesc = "";
            //Lite2Routing = "";
            Lite2VGroove = "";
            Lite3Heat = "";
            Lite3Thickness = 0;
            Lite3FifthSrfCoat = "";
            Lite3SixthSrfCoat = "";
            Lite3Color = "";
            Lite3Pattern = "";
            Lite3LamiType = "";
            Lite3LamiDesc = "";
            Lite3LamiShopFloorDesc = "";
            Lite3VGroove = "";
            //Lite3Routing = "";
            //EdgeDeleteWidth = "";
            Spacer1Size = 0;
            Spacer1Type = "";
            Spacer2Size = 0;
            Spacer2Type = "";
            InteriorScoring = "";
            ExteriorScoring = "";
            InteriorPreserveCutback = "";
            InteriorPreserveColor = "";
            ExteriorPreserveCutback = "";
            ExteriorPreserveColor = "";
            BreatherTube1 = "";
            BreatherTubeUnitsCount = 0;
            ScoredUnitsCount = 0;
            PickItemNum = 0;
            PickedUnitsCount = 0;
            CrateType = "";
            ShopFloorMessage1 = "";
            ShopFloorMessage2 = "";
            PickNotes = "";
            PackNotes = "";
            //CustomerDate = DateTime.Now.ToLocalTime();
            DueDate = DateTime.Now.ToLocalTime();
            //DueDateString = "";
            ShapeNum = 0;
            Base = 0;
            Left = 0;
            Right = 0;
            Top = 0;
            S1 = 0;
            S2 = 0;
            FooterMsg1 = "";
            FooterMsg2 = "";
            FooterMsg3 = "";
            FooterMsg4 = "";
            FooterMsg5 = "";
            FooterMsg6 = "";
            LabelMsg1 = "";
            LabelMsg2 = "";
            LabelMsg3 = "";
            LabelMsg4 = "";
            LabelMsg5 = "";
            UnitWeight = 0;
            UnitActualSqft = 0;
            UnitBillableSqft = 0;

            RackIDMessage1 = "";
            DunnageMessage = "";
            //DockLocal = "";
            //HasMate = "";
            //BaseMM = 0;
            //LeftMM = 0;
            //RightMM = 0;
            //TopMM = 0;
            //TinOut = "";
        }

        [DataMember(EmitDefaultValue = false)]
        public decimal RackId { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string RackSide { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public decimal RackRow { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public decimal RackStack { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public decimal RackSlot { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public decimal RackPosition { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string BeginSerialNum { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string EndSerialNum { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string CompanyNum { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string PlantNum { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public decimal SalesOrderNum { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public decimal SalesOrderLineNum { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string SalesOrderType { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public decimal WorkOrderNum { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public decimal ScheduleNum { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string ScheduleType { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string CustSerialNum { get; set; }
        
        [DataMember(EmitDefaultValue = false)]
        public decimal ItemNum { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string Location { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public decimal SoldToNum { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string SoldToName { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public decimal ShipToNum { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string ShipToName { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string CustPartNum1 { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string CustPartNum2 { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string CustPartNum3 { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string CustSeqNum1 { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string RackFirstCustSeqNum { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string RackLastCustSeqNum { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string GenericSeqNum { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string IGLine { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string RackType { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string RackTypeDesc { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string Status { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public DateTime StatusDate { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string BoxRotated { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public decimal BoxNum { get; set; }


        [DataMember(EmitDefaultValue = false)]
        public decimal RackGlassWeight { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public decimal RackTareWeight { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public int Quantity { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public decimal UnitThickness { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string CustomerPO { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string ProductType { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string CardinalProductType { get; set; }
        
        [DataMember(EmitDefaultValue = false)]
        public string OriginalBranchPlant { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Comment1 { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Lite1Heat { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public decimal Lite1Thickness { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string Lite1FirstSrfCoat { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string Lite1SecondSrfCoat { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string Lite1Color { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string Lite1Pattern { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string Lite1LamiType { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string Lite1LamiDesc { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string Lite1LamiShopFloorDesc { get; set; }


        [DataMember(EmitDefaultValue = false)]
        public string Lite1VGroove { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Lite1Routing { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Lite2Heat { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public decimal Lite2Thickness { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string Lite2ThirdSrfCoat { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string Lite2FourthSrfCoat { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string Lite2Color { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string Lite2Pattern { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string Lite2LamiType { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string Lite2LamiDesc { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string Lite2LamiShopFloorDesc { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Lite2VGroove { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string Lite3Heat { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public decimal Lite3Thickness { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string Lite3FifthSrfCoat { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string Lite3SixthSrfCoat { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string Lite3Color { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string Lite3Pattern { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string Lite3LamiType { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string Lite3LamiDesc { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string Lite3LamiShopFloorDesc { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Lite3VGroove { get; set; }

        // Spacer
        [DataMember(EmitDefaultValue = false)]
        public decimal Spacer1Size { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string Spacer1Type { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public decimal Spacer2Size { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string Spacer2Type { get; set; }

        // Bars
        [DataMember(EmitDefaultValue = false)]
        public decimal Bars1HorzNum { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public decimal Bars1VertNum { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public decimal Air1MCode1 { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string Air1MCode1Desc { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public decimal Air1MCode2 { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string Air1MCode2Desc { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public decimal Air2MCode1 { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string Air2MCode1Desc { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public decimal Air2MCode2 { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string Air2MCode2Desc { get; set; }

        // Preserve Scoring
        [DataMember(EmitDefaultValue = false)]
        public string InteriorScoring { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string ExteriorScoring { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string InteriorPreserveCutback { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string InteriorPreserveColor { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string ExteriorPreserveCutback { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string ExteriorPreserveColor { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string BreatherTube1 { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public int BreatherTubeUnitsCount { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public int ScoredUnitsCount { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public decimal PickItemNum { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public int PickedUnitsCount { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string CrateType { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string ShopFloorMessage1 { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string ShopFloorMessage2 { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string PickNotes { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string PackNotes { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public DateTime DueDate { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public DateTime ScheduleDueDate { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public DateTime CustomerDueDate { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public decimal ShapeNum { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public decimal Base { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public decimal Left { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public decimal Right { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public decimal Top { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public decimal S1 { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public decimal S2 { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string FooterMsg1 { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string FooterMsg2 { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string FooterMsg3 { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string FooterMsg4 { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string FooterMsg5 { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string FooterMsg6 { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string LabelMsg1 { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string LabelMsg2 { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string LabelMsg3 { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string LabelMsg4 { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string LabelMsg5 { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string LabelMsg6 { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string LabelMsg7 { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string LabelMsg8 { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string LabelMsg9 { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string LabelMsg10 { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string LabelMsg11 { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string LabelMsg12 { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string LabelMsg13 { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string LabelMsg14 { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string LabelMsg15 { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string LabelMsg16 { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string LabelMsg17 { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string LabelMsg18 { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string LabelMsg19 { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string LabelMsg20 { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public decimal UnitWeight { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public decimal UnitActualSqft { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public decimal UnitBillableSqft { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string RackIDMessage1 { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string DunnageMessage { get; set; }
        
        //[DataMember(EmitDefaultValue = false)]
        //public string QcInspectFlag { get; set; }

        //[DataMember(EmitDefaultValue = false)]
        //public string ReasonCode { get; set; }
        //[DataMember(EmitDefaultValue = false)]
        //public string EdgeDeleteWidth { get; set; }
        //[DataMember(EmitDefaultValue = false)]
        //public string OrigCustomerPO { get; set; }
        //[DataMember(EmitDefaultValue = false)]
        //public string RemakeFlag { get; set; }
        //[DataMember(EmitDefaultValue = false)]
        //public bool IsCoated { get; set; }

        //[DataMember(EmitDefaultValue = false)]
        //public string DockLocal { get; set; }
    }
}
