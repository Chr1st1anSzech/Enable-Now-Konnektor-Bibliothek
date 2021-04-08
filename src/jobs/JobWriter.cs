using Enable_Now_Konnektor_Bibliothek.src.misc;
using Enable_Now_Konnektor_Bibliothek.src.service;
using log4net;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Reflection;

namespace Enable_Now_Konnektor_Bibliothek.src.jobs
{
    public class JobWriter : JobIO, IJsonWriter
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly string fileName;

        public JobWriter(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                _log.Error(LocalizationService.GetFormattedResource("JobWriterMessage01"));
                throw new ArgumentException("JobWriterMessage01");
            }
            this.fileName = fileName;
        }

        public void Write(object obj)
        {
            string jsonString = JsonConvert.SerializeObject(obj);
            WriteFile(jsonString, fileName);
        }

        private void WriteFile(string jsonString, string fileName)
        {
            string path = Path.Combine(JobDirectory, $"{fileName}.json");
            int i = 2;
            while (File.Exists(path))
            {
                path = Path.Combine(JobDirectory, $"{fileName}-{i}.json");
                i++;
            }
            File.WriteAllText(path, jsonString);
        }
    }
}
