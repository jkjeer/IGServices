using System;
using System.Collections.Generic;
using IBM.Data.DB2.iSeries;
using IGServices.Common.Managers.E1;
using IGServices.Common.Types.E1;
using IGServices.Common.Types.Enterprise;


namespace IGServices.Common.Managers.Enterprise
{
    public class RelatedSalesLineManager
    {

        private CustomerManager _customerMgr;

        public RelatedSalesLineManager(BusinessSystemSession session)
        {
            var hash = new Dictionary<string, iDB2Command>();
            //hash.Add("RelatedSalesLineManager.Get", GetWorkOrder());
            foreach (string keyName in hash.Keys)
            {
                if (!session.DbCommands.ContainsKey(keyName))
                {
                    session.DbCommands.Add(keyName, hash[keyName]);
                }
            }
            _customerMgr = new CustomerManager(session);
        }


        public RelatedSalesLine GetRelatedSalesLine(BusinessSystemSession session, ScheduleLine scheduleLine)
        {
            var line = new RelatedSalesLine();
            try
            {
                line.RequestedDate = E1TimeManager.CastToDateTime(scheduleLine.F580021.S7RSDJ, 0);

                line.CustomerPart2 = scheduleLine.F580021.S7Z58CP2;
                line.CustomerPart3 = scheduleLine.F580021.S7Z58CP3;
                line.CustomerPart4 = scheduleLine.F580021.S7Z58CP4;
                line.CustomerPO = scheduleLine.F580021.S7VR01;
                line.CustomerSequenceNum = scheduleLine.F580021.S7Z58CSQNO;
                line.GenericSequenceNum = scheduleLine.F580021.S7Z58GSQNO;
                line.ProductType = scheduleLine.F580021.S7Z58PT;
                line.PriorityCode = scheduleLine.F580021.S7PRIO;
                //line.ShopFloorMsg1 = CastDbNull.To<String>(reader["ATZ5899009"], String.Empty).Trim();
                //line.ShopFloorMsg2 = CastDbNull.To<String>(reader["ATZ5899010"], String.Empty).Trim();
                line.PickNotes = scheduleLine.F580024.S8Z5899011;
                line.PackNotes = scheduleLine.F580024.S8Z5899012;
                //line.VolumeIndicator = CastDbNull.To<String>(reader["ATZ5840460"], String.Empty).Trim();       

                line.SoldToCustomer = _customerMgr.GetCustomer(session, scheduleLine.F580021.S7AN8DR, false);
                line.ShipToCustomer = _customerMgr.GetCustomer(session, scheduleLine.F580021.S7SHAN, false);
                line.OriginalSoldToCustomer = _customerMgr.GetCustomer(session, scheduleLine.F580021.S7Z58CTL, false);
                line.OriginalShipToCustomer = _customerMgr.GetCustomer(session, scheduleLine.ItemAttributes.GetDecimalAttributeValue("Z5840425"), false);

                var messages = new EdiMessages();
                foreach (var f4715 in scheduleLine.F4715s)
                {
                    var message = new EdiMessage();
                    message.EdiType = f4715.ZDEDTY;
                    message.EdiSeqNum = f4715.ZDEDSQ;
                    message.Message = f4715.ZDTXLN;
                    messages.Add(message);
                }
                line.EdiMessages = messages;

                var crates = new Crates();
                foreach (var f580220 in scheduleLine.F580220s)
                {
                    var crate = new Crate();
                    crate.CrateId = f580220.CRZ58CRID;
                    crate.CrateType = f580220.CRZ58CRTY;
                    crate.Quantity = f580220.CRZ58CRQ;
                    crate.Message = f580220.CRZ58CRMSG;
                    crates.Add(crate);
                }
                line.Crates = crates;

            }
            catch (Exception err)
            {
                throw new Exception(err.ToString());
            }
            return line;
        }

    }
}
