using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Enable_Now_Konnektor_Bibliothek.src.service;
using Newtonsoft.Json;

namespace Enable_Now_Konnektor_Bibliothek.src.jobs
{
    public class JobReader : JobIO
    {
        #region internal-methods
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal IEnumerable<string> ReadJobPaths()
        {
            try
            {
                return Directory.EnumerateFiles(JobDirectory);
            }
            catch (Exception e)
            {
                log.Error(LocalizationService.FormatResourceString("JobReaderMessage01", JobDirectory), e);
                return null;
            }
        }



        /// <summary>
        /// Liest alle Dateien im Job-Verzeichnis. Speichert die ID in der übergebenen Liste. Speichert die ID und den
        /// Pfad des Jobs in einer Liste.
        /// </summary>
        /// <param name="jobIds">Eine Liste, in der die IDs eingetragen werden sollen.</param>
        /// <param name="jobIdPathDictionary">Ein Wörterbuch, in der die IDs und Pfade eingetragen werden sollen.</param>
        internal void ReadAllJobConfigs(ref ObservableCollection<string> jobIds, ref Dictionary<string, string> jobIdPathDictionary)
        {
            IEnumerable<string> fullFileNames = ReadJobPaths();
            if (fullFileNames == null) return;

            int jobCount = fullFileNames.Count();
            for (int fileNamesIndex = 0; fileNamesIndex < jobCount; fileNamesIndex++)
            {
                string fullFileName = fullFileNames.ElementAt(fileNamesIndex);
                JobConfig jobConfig = ReadJobConfig(fullFileName);
                if (jobConfig != null)
                {
                    jobIds.Add(jobConfig.Id);
                    jobIdPathDictionary.Add(jobConfig.Id, fullFileName);
                }
            }
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        internal JobConfig ReadJobConfig(string filePath)
        {
            string jsonString = ReadFile(filePath);
            if (jsonString == null) { return null; }

            JobConfig jobConfig;
            try
            {
                jobConfig = JsonConvert.DeserializeObject<JobConfig>(jsonString);
            }
            catch (Exception e)
            {
                log.Error(LocalizationService.FormatResourceString("JobReaderMessage02", filePath), e);
                return null;
            }

            return jobConfig;
        }
#endregion

        #region private-methods
        /// <summary>
        /// Liest den Text aus der Datei aus.
        /// </summary>
        /// <param name="filePath">Pfad zu der Datei.</param>
        /// <returns>Text der Datei.</returns>
        private string ReadFile(string filePath)
        {
            try
            {
                return File.ReadAllText(filePath);
            }
            catch (Exception e)
            {
                log.Error(LocalizationService.FormatResourceString("JobReaderMessage04", filePath), e);
                return null;
            }
        }
        #endregion
    }
}
