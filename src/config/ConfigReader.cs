using System;
using System.IO;
using System.Reflection;
using Enable_Now_Konnektor_Bibliothek.src.misc;
using Enable_Now_Konnektor_Bibliothek.src.service;
using log4net;
using Newtonsoft.Json;

namespace Enable_Now_Konnektor_Bibliothek.src.config
{
    public class ConfigReader :ConfigIO
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static Config config;

        private ConfigReader() { }

        public static void Initialize()
        {
            if (config == null)
            {
                try
                {
                    string jsonString = ReadFile();
                    config = JsonConvert.DeserializeObject<Config>(jsonString);
                    new Validator().ValidateConfig(config);
                }
                catch (Exception e)
                {
                    _log.Error(LocalizationService.GetFormattedResource("ConfigReaderMessage02", FilePath));
                    throw new Exception(LocalizationService.GetFormattedResource("ConfigReaderMessage02", FilePath), e);
                }
            }
        }

        public static Config LoadConnectorConfig()
        {
            return config;
        }

        private static string ReadFile()
        {
            try
            {
                return File.ReadAllText(FilePath);
            }
            catch (Exception e)
            {
                _log.Error(LocalizationService.GetFormattedResource("ConfigReaderMessage01", FilePath));
                throw new Exception(LocalizationService.GetFormattedResource("ConfigReaderMessage01", FilePath), e);
            }
        }
    }
}
