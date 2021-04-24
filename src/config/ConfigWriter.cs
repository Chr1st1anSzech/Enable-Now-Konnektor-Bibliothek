using Enable_Now_Konnektor_Bibliothek.src.misc;
using Newtonsoft.Json;
using System;
using System.IO;

namespace Enable_Now_Konnektor_Bibliothek.src.config
{
    public class ConfigWriter : ConfigIO, IJsonWriter
    {
        public ConfigWriter()
        {
        }

        public string Write(object obj, string path = null)
        {
            if(path == null) { path = s_filePath; }
            string jsonString = JsonConvert.SerializeObject(obj);
            WriteFile(jsonString, path);
            return path;
        }

        private void WriteFile(string jsonString, string path)
        {
            try
            {
                File.WriteAllText(path, jsonString);
            }
            catch (Exception e)
            {
                throw new Exception("Fehler beim Schreiben der Konfigurationsdatei", e);
            }
        }
    }
}
