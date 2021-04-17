using System.Collections.ObjectModel;
using log4net;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Enable_Now_Konnektor_Bibliothek.src.jobs
{
    public class JobManager : INotifyPropertyChanged
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private protected static JobManager Manager;

        public event PropertyChangedEventHandler PropertyChanged;
        
        private JobConfig selectedJobConfig;
        public JobConfig SelectedJobConfig
        {
            get
            {
                return selectedJobConfig;
            }
            set
            {
                if (value != selectedJobConfig)
                {
                    selectedJobConfig = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public ObservableCollection<JobConfig> AllJobs { get; set; }

        internal Dictionary<string, string> jobIdPathDictionary;
        internal Dictionary<string, string> JobIdPathDictionary {
            get
            {
                return jobIdPathDictionary;
            }
            set
            {
                if (value != jobIdPathDictionary)
                {
                    jobIdPathDictionary = value;
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private protected JobManager()
        {
            ReadAllJobs();
        }

        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }



        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static JobManager GetJobManager()
        {
            Manager ??= new JobManager();
            return Manager;
        }



        /// <summary>
        /// 
        /// </summary>
        private void ReadAllJobs()
        {
            AllJobs = new ObservableCollection<JobConfig>(
                new JobReader().ReadAllJobConfigs(ref jobIdPathDictionary)
            );
        }

        private void AddNewJob(JobConfig jobConfig, string path)
        {
            if (AllJobs == null)
            {
                AllJobs = new ObservableCollection<JobConfig>();
            }
            AllJobs.Add(jobConfig);
            JobIdPathDictionary.Add(jobConfig.Id, path);
        }



        /// <summary>
        /// 
        /// </summary>
        public JobConfig CreateJob(string jobName)
        {
            
            if (!Regex.IsMatch(jobName, @"^[a-zA-Z0-9\-_]+$"))
            {
                return null;
            }
            else
            {
                string tmpJobName = jobName;
                int i = 2;
                while (GetJobConfig(tmpJobName) != null)
                {
                    tmpJobName = $"{jobName}-{i}";
                    i++;
                }
                jobName = tmpJobName;
                JobConfig jobConfig = new()
                {
                    Id = jobName
                };
                string path = new JobWriter().Write(jobConfig, jobName);
                AddNewJob(jobConfig, path);
                return jobConfig;
            }

        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JobConfig GetJobConfig(string id)
        {
            return AllJobs.FirstOrDefault(job => job.Id.Equals(id));
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string GetJobConfigPath(string id)
        {
            if( JobIdPathDictionary.ContainsKey(id))
            {
                return JobIdPathDictionary[id];
            }
            else
            {
                return null;
            }
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string GetJobConfigPath(JobConfig jobConfig)
        {
            if (JobIdPathDictionary.ContainsKey(jobConfig.Id))
            {
                return JobIdPathDictionary[jobConfig.Id];
            }
            else
            {
                return null;
            }
        }
    }
}
