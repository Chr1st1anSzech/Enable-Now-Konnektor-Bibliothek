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
        private readonly string path;

        public JobWriter()
        {
        }

        public JobWriter(string path)
        {
            this.path = path;
        }

        public string Write(object obj, string path2 = null)
        {
            if(path2== null)
            {
                path2 = path;
            }
            CheckPath(path2);
            string absolutePath = MakeAbsolutePath(path2);
            string jsonString = JsonConvert.SerializeObject(obj);
            File.WriteAllText(absolutePath, jsonString);
            return absolutePath;
        }

        private void CheckPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                _log.Error(LocalizationService.FormatResourceString("JobWriterMessage01"));
                throw new ArgumentException("JobWriterMessage01");
            }
        }

        private string MakeAbsolutePath(string path)
        {
            if (!path.EndsWith(".json"))
            {
                path = $"{path}.json";
            }
            if (Path.IsPathFullyQualified(path))
            {
                return path;
            }
            else
            {
                return Path.Combine(JobDirectory, path);
            }
        }
    }
}
