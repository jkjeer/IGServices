using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using IBM.Data.DB2.iSeries;
using IGServices.Common.Types.Configuration;
using IGServices.Common.Types.E1;
using NLog;

namespace IGServices.Common.Managers.Enterprise
{
    public class SpacerTypeManager
    {
        private Logger _logger = LogManager.GetCurrentClassLogger();

        private Types.Configuration.SpacerTypes _spacerTypeConfig;
        private SpacerTokens _spacerTokenConfig;

        public SpacerTypeManager(BusinessSystemSession e1Session)
        {
            var hash = new Dictionary<string, iDB2Command>
            {
            };
            foreach (var keyName in hash.Keys.Where(keyName => !e1Session.DbCommands.ContainsKey(keyName)))
            {
                e1Session.DbCommands.Add(keyName, hash[keyName]);
            }
            var spacerConfiguration =
                (Types.Configuration.SpacerConfiguration) ConfigurationManager.GetSection("SpacerSection");
            _spacerTypeConfig = spacerConfiguration.SpacerTypes;
            _spacerTokenConfig = spacerConfiguration.SpacerTokens;
        }

        /*********************************************************************************
        * Spacer Type
        ********************************************************************************/
        public Common.Types.Enterprise.SpacerType GetSpacerType(string name)
        {
            var spacerType = new Common.Types.Enterprise.SpacerType();
            try
            {
                var spacerTypeConfig = _spacerTypeConfig[name];
                spacerType.Name = spacerTypeConfig.name;
                spacerType.Material = spacerTypeConfig.material;
                spacerType.Color = spacerTypeConfig.color;
                spacerType.Painted = (spacerTypeConfig.painted == "true");
                spacerType.Profile = spacerTypeConfig.profile;
                spacerType.Surface = spacerTypeConfig.surface;
                return spacerType;
            }
            catch (Exception e)
            {
                throw new FaultException(e.Message);
            }
        }

    }
}
