using Enable_Now_Konnektor_Bibliothek.src.misc;
using Enable_Now_Konnektor_Bibliothek.src.service;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Enable_Now_Konnektor_Bibliothek.src.jobs
{
    public class JobReader : JobIO
    {
        private readonly Validator jobValidator = new Validator();


        public IEnumerable<string> ReadJobPaths()
        {
            try
            {
                return Directory.EnumerateFiles(JobDirectory);
            }
            catch (Exception e)
            {
                log.Error(LocalizationService.GetFormattedResource("JobReaderMessage01", JobDirectory), e);
                return null;
            }
        }

        public List<JobConfig> ReadAllJobConfigs()
        {
            IEnumerable<string> fullFileNames = ReadJobPaths();
            if (fullFileNames == null) return null;

            int jobCount = fullFileNames.Count();
            List<JobConfig> jobsConfigs = new List<JobConfig>();
            for (int i = 0; i < jobCount; i++)
            {
                JobConfig config = ReadJob(fullFileNames.ElementAt(i));
                if(config != null)
                {
                    jobsConfigs.Add(config);
                }
            }
            return jobsConfigs;
        }

        public JobConfig ReadJob(string filePath)
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
                log.Error(LocalizationService.GetFormattedResource("JobReaderMessage02", filePath), e);
                return null;
            }


            if (jobValidator.ValidateJobConfig(jobConfig))
            {
                return jobConfig;
            }
            else
            {
                log.Error(LocalizationService.GetFormattedResource("JobReaderMessage03", filePath));
                return null;
            }
        }

        private string ReadFile(string filePath)
        {
            try
            {
                return File.ReadAllText(filePath);
            }
            catch (Exception e)
            {
                log.Error(LocalizationService.GetFormattedResource("JobReaderMessage04", filePath), e );
                return null;
            }
        }
    }
}
