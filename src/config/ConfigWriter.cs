﻿using Enable_Now_Konnektor_Bibliothek.src.misc;
using Newtonsoft.Json;
using System;
using System.IO;

namespace Enable_Now_Konnektor_Bibliothek.src.config
{
    public class ConfigWriter :ConfigIO, IJsonWriter
    {
        private readonly string path;

        public ConfigWriter(string path)
        {
            this.path = path;
        }

        public void Write(object obj)
        {
            string jsonString = JsonConvert.SerializeObject(obj);
            WriteFile(jsonString, path);
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
