using System;
using System.Collections.Generic;
using IBM.Data.DB2.iSeries;
using IGServices.Common.Types.E1;
using IGServices.Common.Types.Enterprise;

namespace IGServices.Common.Managers.Enterprise
{
    public class MuntinBarManager
    {
        private MuntinBarCodeManager _barCodeManager;

        public MuntinBarManager(BusinessSystemSession e1Session)
        {
            var hash = new Dictionary<string, iDB2Command>();
            foreach (string keyName in hash.Keys)
            {
                if (!e1Session.DbCommands.ContainsKey(keyName))
                {
                    e1Session.DbCommands.Add(keyName, hash[keyName]);
                }
            }
            _barCodeManager = new MuntinBarCodeManager(e1Session);
        }

        /**********************************************************************************
        *   Muntin Bar properties
        **********************************************************************************/

        public MuntinBar GetMuntinBarByCode(BusinessSystemSession e1Session, string mCode)
        {
            var bar = new MuntinBar();
            try
            {
                var code = _barCodeManager.GetBarCode(e1Session, mCode.PadLeft(5, '0'));
                bar.MCode = code.MCode;
                bar.MCodeNum = code.MCodeNum;
                bar.Description = code.Description;
                bar.ExtColor = code.ExtColor;
                bar.IntColor = code.IntColor;
                bar.IsTwoTone = code.IsTwoTone;
                bar.Type = code.Type;
                bar.Thickness = code.Thickness;
                bar.Width = code.Width;
            }
            catch (Exception err)
            {
                throw new ApplicationException(err.ToString());
            }
            return bar;
        }

    }
}
