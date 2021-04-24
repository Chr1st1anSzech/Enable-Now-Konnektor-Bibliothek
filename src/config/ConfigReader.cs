using System;
using System.IO;
using System.Reflection;
using Enable_Now_Konnektor_Bibliothek.src.service;
using log4net;
using Newtonsoft.Json;

namespace Enable_Now_Konnektor_Bibliothek.src.config
{
    internal class ConfigReader : ConfigIO
    {
        private static readonly ILog s_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        internal ConfigReader() { }

        internal static Config ReadConfig()
        {
            Config config;
            try
            {
                string jsonString = ReadFile();
                config = JsonConvert.DeserializeObject<Config>(jsonString);
            }
            catch (Exception e)
            {
                s_log.Error(LocalizationService.FormatResourceString("ConfigReaderMessage02", FilePath));
                throw new Exception(LocalizationService.FormatResourceString("ConfigReaderMessage02", FilePath), e);
            }
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
                s_log.Error(LocalizationService.FormatResourceString("ConfigReaderMessage01", FilePath));
                throw new Exception(LocalizationService.FormatResourceString("ConfigReaderMessage01", FilePath), e);
            }
        }
    }
}
