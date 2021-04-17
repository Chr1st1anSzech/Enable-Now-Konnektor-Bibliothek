using log4net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Enable_Now_Konnektor_Bibliothek.src.config
{
    public class ConfigManager : INotifyPropertyChanged
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private protected static ConfigManager Manager;

        public event PropertyChangedEventHandler PropertyChanged;

        private Config connectorConfig;
        public Config ConnectorConfig
        {
            get
            {
                return connectorConfig;
            }
            private set
            {
                if (value != connectorConfig)
                {
                    connectorConfig = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private protected ConfigManager()
        {
            ConnectorConfig = new ConfigReader().ReadConfig();
        }

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }



        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static ConfigManager GetConfigManager()
        {
            Manager ??= new ConfigManager();
            return Manager;
        }
    }
}
