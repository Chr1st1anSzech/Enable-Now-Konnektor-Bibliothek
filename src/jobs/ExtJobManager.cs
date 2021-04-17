using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enable_Now_Konnektor_Bibliothek.src.jobs
{
    public class ExtJobManager :JobManager
    {
        private protected static new ExtJobManager Manager;

        private string recentJobsFilter;
        public string RecentJobsFilter { 
            get 
            { 
                return recentJobsFilter; 
            } 
            set
            {
                if (value != recentJobsFilter)
                {
                    recentJobsFilter = value;
                    NotifyPropertyChanged();
                    NotifyPropertyChanged(nameof(RecentJobs));
                }
            } 
        }

        private ObservableCollection<JobConfig> recentJobs = new();
        public ObservableCollection<JobConfig> RecentJobs { 
            get 
            {
                if (string.IsNullOrEmpty(RecentJobsFilter)) return recentJobs;
                else
                {
                    return new ObservableCollection<JobConfig>( recentJobs.Where(jobConfig => jobConfig.Id.StartsWith(RecentJobsFilter)) );
                }
                
            }
            private set
            {
                if (value != recentJobs)
                {
                    recentJobs = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private ObservableCollection<JobConfig> openedJobs = new();
        public ObservableCollection<JobConfig> OpenedJobs
        {
            get
            {
                return openedJobs;

            }
            private set
            {
                if (value != openedJobs)
                {
                    openedJobs = value;
                    NotifyPropertyChanged();
                }
            }
        }



        /// <summary>
        /// 
        /// </summary>
        private protected ExtJobManager() : base()
        {
            ReadRecentJobs();
        }



        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static new ExtJobManager GetJobManager()
        {
            Manager ??= new ExtJobManager();
            return Manager;
        }



        /// <summary>
        /// 
        /// </summary>
        private void ReadRecentJobs()
        {
            List<string> jobIds = new RecentJobsReader().ReadRecentJobs();
            RecentJobs = new ObservableCollection<JobConfig>(
                AllJobs.Where(job => jobIds.Contains(job.Id))
            );
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="jobConfig"></param>
        public bool AddRecentJob(JobConfig jobConfig)
        {
            if (jobConfig == null) return false;
            if (RecentJobs.FirstOrDefault(jobConfig2 => jobConfig.Equals(jobConfig2)) != null) return false;

            RecentJobs.Add(jobConfig);
            new RecentJobsWriter().AddRecentJob(jobConfig.Id);
            return true;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="jobConfig"></param>
        public bool AddOpenedJob(JobConfig jobConfig)
        {
            JobConfig jobConfigInList = OpenedJobs.FirstOrDefault(jobConfig2 => jobConfig2.Equals(jobConfig));
            if (jobConfig == null || jobConfigInList != null) return false;

            OpenedJobs.Add(jobConfig);
            return true;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="jobConfig"></param>
        public bool RemoveOpenedJob(JobConfig jobConfig)
        {
            JobConfig jobConfigInList = OpenedJobs.FirstOrDefault(jobConfig2 => jobConfig2.Equals(jobConfig));
            if (jobConfig == null || jobConfigInList == null) return true;

            OpenedJobs.Remove(jobConfigInList);
            return true;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="jobConfig"></param>
        public bool RemoveOpenedJob(string id)
        {
            JobConfig jobConfigInList = OpenedJobs.FirstOrDefault(jobConfig => jobConfig.Id.Equals(id));
            if (jobConfigInList == null) return true;

            OpenedJobs.Remove(jobConfigInList);
            return true;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="jobConfig"></param>
        public bool IsJobOpened(JobConfig jobConfig)
        {
            return OpenedJobs.FirstOrDefault(jobConfig2 => jobConfig2.Equals(jobConfig)) != null;
        }
    }
}
