using log4net;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Enable_Now_Konnektor_Bibliothek.src.config
{
    public class ConfigManager : INotifyPropertyChanged
    {
        private static readonly ILog s_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private protected static ConfigManager s_manager;

        public event PropertyChangedEventHandler PropertyChanged;

        private Config _connectorConfig;
        public Config ConnectorConfig
        {
            get
            {
                return _connectorConfig;
            }
            private set
            {
                if (value != _connectorConfig)
                {
                    _connectorConfig = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private protected ConfigManager()
        {
            ConnectorConfig = ConfigReader.ReadConfig();
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
            s_manager ??= new ConfigManager();
            return s_manager;
        }
    }
}
