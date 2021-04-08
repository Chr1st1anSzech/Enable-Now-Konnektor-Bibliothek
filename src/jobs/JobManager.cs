using System;
using System.Collections.Generic;
using System.Text;

namespace Enable_Now_Konnektor_Bibliothek.src.jobs
{
    internal class JobManager
    {
        private static List<JobConfig> jobs;

        internal JobManager()
        {
            Initialize();
        }

        internal void Initialize()
        {
            JobReader reader = new JobReader();
            jobs = reader.ReadAllJobConfigs();
        }

        private void AddNewJob(JobConfig jobConfig)
        {
            if( jobs == null)
            {
                jobs = new List<JobConfig>(1);
            }
            jobs.Add(jobConfig);
        }

        internal void CreateNewJob(string jobName)
        {
            JobConfig jobConfig = new JobConfig();
            jobConfig.Id = jobName;
            new JobWriter(jobName).Write(jobConfig);
            AddNewJob(jobConfig);
        }
    }
}
