using System;
using IGServices.Common.Types.E1;
using IGServices.Common.Types.ShopFloor;
//using NLog;

namespace IGServices.Common.Managers.ShopFloor
{
    public class ShopFloorRawGlassManager
    {
        //private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static class Tmp<T>
        {
            [ThreadStatic]
            public static T Value;
        }

        public ShopFloorRawGlassItems GetShopFloorRawGlassitems(RawGlassItems rawGlassItems)
        {
            var shopFloorRawGlassItems = new ShopFloorRawGlassItems();
            try
            {
                // ReSharper disable once LoopCanBeConvertedToQuery
                foreach (var rawGlassItem in rawGlassItems)
                {
                    var shopFloorRawGlassItem = new ShopFloorRawGlassItem();
                    shopFloorRawGlassItem.ItemNumber = rawGlassItem.WMCPIT.ToString();
                    shopFloorRawGlassItem.BusinessUnit = rawGlassItem.WMCMCU;
                    shopFloorRawGlassItem.PartType = rawGlassItem.IBGLPT;
                    shopFloorRawGlassItem.WorderNum = rawGlassItem.WMDOCO;
                    shopFloorRawGlassItem.DocType = rawGlassItem.WMDCTO;
                    shopFloorRawGlassItem.Desc1 = rawGlassItem.WMDSC1;
                    shopFloorRawGlassItem.Desc2 = rawGlassItem.WMDSC2;
                    shopFloorRawGlassItem.Location = rawGlassItem.WMLOCN;
                    shopFloorRawGlassItem.OrderQty = rawGlassItem.WMUORG;
                    shopFloorRawGlassItem.QtyAvail = rawGlassItem.WMTRQT;
                    shopFloorRawGlassItem.LiteThickness = rawGlassItem.CRZ58L1TK;
                    shopFloorRawGlassItem.LiteHeat = rawGlassItem.CRZ58L1HT;
                    shopFloorRawGlassItem.LiteColor = rawGlassItem.CRZ58L1CL;
                    shopFloorRawGlassItem.FirstSrfCoating = rawGlassItem.CRZ58L11C;
                    shopFloorRawGlassItem.SecondSrfCoating = rawGlassItem.CRZ58L12C;
                    shopFloorRawGlassItem.LitePattern = rawGlassItem.CRZ58L1PX;
                    shopFloorRawGlassItem.BaseDim = rawGlassItem.CRZ58L1BDM;
                    shopFloorRawGlassItem.BaseN = rawGlassItem.CRZ58L1BNM;
                    shopFloorRawGlassItem.BaseD = rawGlassItem.CRZ58L1BDN;
                    shopFloorRawGlassItem.LeftDim = rawGlassItem.CRZ58L1LDM;
                    shopFloorRawGlassItem.LeftN = rawGlassItem.CRZ58L1LNM;
                    shopFloorRawGlassItem.LeftD = rawGlassItem.CRZ58L1LDN;
                    shopFloorRawGlassItem.RightDim = rawGlassItem.CRZ58L1RDM;
                    shopFloorRawGlassItem.RightN = rawGlassItem.CRZ58L1BNM;
                    shopFloorRawGlassItem.RightD = rawGlassItem.CRZ58L1RDN;
                    shopFloorRawGlassItem.TopDim = rawGlassItem.CRZ58L1TDM;
                    shopFloorRawGlassItem.TopN = rawGlassItem.CRZ58L1TNM;
                    shopFloorRawGlassItem.TopD = rawGlassItem.CRZ58L1TDN;
                    shopFloorRawGlassItem.ShapeNum = rawGlassItem.CRZ58SHAPE;
                    shopFloorRawGlassItem.E1BatchID = rawGlassItem.S7EDBT;
                    shopFloorRawGlassItem.S7ITM = rawGlassItem.S7ITM;
                    shopFloorRawGlassItem.S7Z58SCN = rawGlassItem.S7Z58SCN;
                    shopFloorRawGlassItem.WMOPSQ = rawGlassItem.WMOPSQ;

                    shopFloorRawGlassItems.Add(shopFloorRawGlassItem);
                }
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.ToString());
            }
            return shopFloorRawGlassItems;
        }
    }
}
