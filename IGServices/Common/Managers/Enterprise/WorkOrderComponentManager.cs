using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.ServiceModel;
using IBM.Data.DB2.iSeries;
using IGServices.Common.Types;
using IGServices.Common.Types.E1;
using IGServices.Common.Types.Enterprise;
using IGServices.Common.Utilities;

namespace IGServices.Common.Managers.Enterprise
{
    public class WorkOrderComponentManager
    {

        public WorkOrderComponentManager(BusinessSystemSession e1Session)
        {
            var hash = new Dictionary<string, iDB2Command>();
            hash.Add("WorkOrderComponentManager.GetWorkOrderComponent", GetWorkOrderComponent());
            hash.Add("WorkOrderComponentManager.GetWorkOrderChannelComponents", GetWorkOrderChannelComponents());
            foreach (string keyName in hash.Keys)
            {
                if (!e1Session.DbCommands.ContainsKey(keyName))
                {
                    e1Session.DbCommands.Add(keyName, hash[keyName]);
                }
            }
        }

        /*********************************************************************************
         * Prepared Commands
         *******************************************************************************/
        public iDB2Command GetWorkOrderComponent()
        {
            iDB2Command cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = String.Format(
                @"SELECT
                    WADOCO,
                    WAMCU,
                    WASRST,
                    DRDL01,
                    WAITM
                FROM
                    F4801
                    INNER JOIN {0}.F0005
                    ON  DRSY = '00' AND
                        DRRT = 'SS' AND
                        TRIM(DRKY) = WASRST
                WHERE
                    WADOCO = ?", ConfigurationManager.AppSettings["ControlLib"]);
            cmd.Parameters.Add(new iDB2Parameter("WorkOrderNum", iDB2DbType.iDB2Decimal));
            cmd.Parameters["WorkOrderNum"].Precision = 8;
            cmd.Parameters["WorkOrderNum"].Scale = 0;
            return cmd;
        }

        public iDB2Command GetWorkOrderChannelComponents()
        {
            iDB2Command cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = String.Format(
                @"SELECT
                    WMDOCO,
                    WMOPSQ,
                    WMCPIT,
                    WMDSC1,
                    WMDSC2,
                    WMLOCN,
                    WMUORG,
                    WMTRQT,
                    WMUM,
                    WMCPNB,
                    WMMCU
                FROM
                    F3111
                    INNER JOIN F4102 
                    ON  IBITM = WMCPIT AND
                        IBMCU = WMMCU
                WHERE
                    WMDOCO = ? AND
                    IBGLPT = 'CHAN'", ConfigurationManager.AppSettings["ControlLib"]);
            cmd.Parameters.Add(new iDB2Parameter("WorkOrderNum", iDB2DbType.iDB2Decimal));
            cmd.Parameters["WorkOrderNum"].Precision = 8;
            cmd.Parameters["WorkOrderNum"].Scale = 0;
            return cmd;
        }

        public WorkOrderComponents GetWorkOrderChannelComponents(BusinessSystemSession e1Session, Decimal orderNum)
        {
            var components = new WorkOrderComponents();
            try
            {
                var cmd = (e1Session.DbCommands.ContainsKey("GetWorkOrderChannelComponents")) ? e1Session.DbCommands["GetWorkOrderChannelComponents"] : GetWorkOrderChannelComponents();
                cmd.Connection = e1Session.DbConnection;
                cmd.Parameters[0].Value = orderNum;
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
                while (reader.Read())
                {
                    var component = new WorkOrderComponent();
                    component.WorkOrderNum = CastDbNull.To<Decimal>(reader["WMDOCO"], 0);
                    component.SequenceNum = CastDbNull.To<Decimal>(reader["WMOPSQ"], 0) / 100;
                    // Planned Quantity
                    component.Quantity = CastDbNull.To<Decimal>(reader["WMUORG"], 0) / 1000;
                    // Actual Quantity
                    component.InventoryQuantity = CastDbNull.To<Decimal>(reader["WMTRQT"], 0) / 1000;
                    component.ComponentNum = CastDbNull.To<Decimal>(reader["WMCPNB"], 0) / 100;
                    component.Location = CastDbNull.To<String>(reader["WMLOCN"], String.Empty).Trim();
                    component.UnitOfMeasure = CastDbNull.To<String>(reader["WMUM"], String.Empty).Trim();
                    var itemNum = CastDbNull.To<Decimal>(reader["WMCPIT"], 0);
                    var plantNum = CastDbNull.To<String>(reader["WMMCU"], String.Empty).Trim();
                    var itemMgr = new SpacerItemManager(e1Session);
                    component.Item = itemMgr.GetSpacerItem(e1Session, plantNum, itemNum);
                    components.Add(component);
                }
                reader.Close();
            }
            catch (Exception e)
            {
                throw new FaultException(e.Message);
            }
            return components;
        }

    }
}
