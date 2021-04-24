using System;
using System.IO;
using Enable_Now_Konnektor_Bibliothek.src.service;
using Newtonsoft.Json;

namespace Enable_Now_Konnektor_Bibliothek.src.config
{
    internal class ConfigReader : ConfigIO
    {
        internal ConfigReader() { }



        /// <summary>
        /// Liest die Konnektor-Konfiguration aus der config.json heraus.
        /// </summary>
        /// <returns></returns>
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
                string message = LocalizationService.FormatResourceString("ConfigReaderMessage02", s_filePath);
                s_log.Error(message);
                throw new Exception(message, e);
            }
            return config;
        }



        /// <summary>
        /// Liest den Text aus einer Datei.
        /// </summary>
        /// <returns></returns>
        private static string ReadFile()
        {
            try
            {
                return File.ReadAllText(s_filePath);
            }
            catch (Exception e)
            {
                string message = LocalizationService.FormatResourceString("ConfigReaderMessage01", s_filePath);
                s_log.Error(message);
                throw new Exception(message, e);
            }
        }
    }
}
