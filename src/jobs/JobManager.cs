using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;
using Enable_Now_Konnektor_Bibliothek.src.service;
using log4net;
using Microsoft.Extensions.Caching.Memory;

namespace Enable_Now_Konnektor_Bibliothek.src.jobs
{
    public class JobManager : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;



        private static readonly ILog s_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private protected static JobManager s_manager;
        private readonly MemoryCache _jobConfigCache = new(new MemoryCacheOptions()
        {
            SizeLimit = 3
        });
        private readonly ConcurrentDictionary<string, SemaphoreSlim> _cacheLocks = new();
        private readonly Dictionary<string, string> _jobIdPathDictionary = new();
        private readonly ObservableCollection<string> _jobIds = new();



        private JobConfig _selectedJobConfig;



        public JobConfig SelectedJobConfig
        {
            get
            {
                return _selectedJobConfig;
            }
            set
            {
                if (value != _selectedJobConfig)
                {
                    _selectedJobConfig = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public ObservableCollection<string> JobIds
        {
            get
            {
                return _jobIds;
            }
        }



        /// <summary>
        /// 
        /// </summary>
        private protected JobManager()
        {
            new JobReader().ReadAllJobConfigs(ref _jobIds, ref _jobIdPathDictionary);
        }



        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        #region public-methods
        /// <summary>
        /// Erstellt gegebenenfalls das JobManager-Objekt und gibt es zurück.
        /// </summary>
        /// <returns></returns>
        public static JobManager GetJobManager()
        {
            s_manager ??= new JobManager();
            return s_manager;
        }



        /// <summary>
        /// Holt das JobConfig-Objekt entweder aus dem Cache oder liest es aus dem Dateisystem, speichert es im Cache und gibt
        /// es zurück.
        /// </summary>
        /// <param name="jobId">Die ID des geforderten JobConfig-Objekts.</param>
        /// <returns>Das JobConfig-Objekt mit der geforderten ID.</returns>
        public JobConfig GetJobConfig(string jobId)
        {
            if (!_jobConfigCache.TryGetValue(jobId, out JobConfig jobConfig))
            {
                SemaphoreSlim chacheLock = _cacheLocks.GetOrAdd(jobId, k => new SemaphoreSlim(1, 1));

                chacheLock.Wait();
                try
                {
                    bool isJobConfigInCache = _jobConfigCache.TryGetValue(jobId, out jobConfig);
                    if (!isJobConfigInCache)
                    {
                        s_log.Debug(LocalizationService.FormatResourceString("JobManagerMessage02",jobId));
                        string jobPath = GetJobConfigPath(jobId);
                        jobConfig = new JobReader().ReadJobConfig(jobPath);
                        MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions()
                            .SetSize(1)
                            .SetSlidingExpiration(TimeSpan.FromSeconds(30))
                            .SetAbsoluteExpiration(TimeSpan.FromSeconds(120));
                        _jobConfigCache.Set<JobConfig>(jobId, jobConfig, cacheEntryOptions);
                    }
                }
                finally
                {
                    chacheLock.Release();
                }
            }
            return jobConfig;
        }



        /// <summary>
        /// Erstellt ein JobConfig-Objekt mit der übergebenen ID.
        /// <para>
        /// Es wird eine gültige und nicht vorhandene ID gesucht. Das Objekt wird serialisiert und geschrieben.
        /// </para>
        /// </summary>
        /// <param name="jobName">Die ID, die das JobConfig-Objekt besitzen soll.</param>
        /// <returns>Das erstellte JobConfig-Objekt.</returns>
        public JobConfig CreateJob(string jobName)
        {
            s_log.Debug(LocalizationService.FormatResourceString("JobManagerMessage03",jobName));
            if (!Regex.IsMatch(jobName, @"^[a-zA-Z0-9\-_]+$"))
            {
                s_log.Error(LocalizationService.FormatResourceString("JobManagerMessage01", jobName));
                return null;
            }
            else
            {
                jobName = SearchFreeJobId(jobName);

                JobConfig jobConfig = new()
                {
                    Id = jobName
                };

                string path = new JobWriter().Write(jobConfig, jobName);
                AddNewJob(jobName, path);

                s_log.Info(LocalizationService.FormatResourceString("JobManagerMessage04", jobName));

                return jobConfig;
            }

        }



        /// <summary>
        /// Gibt den Pfad des JobConfig-Objekt mit der übergebenen ID zurück.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string GetJobConfigPath(string id)
        {
            if (_jobIdPathDictionary.ContainsKey(id))
            {
                return _jobIdPathDictionary[id];
            }
            else
            {
                return null;
            }
        }



        /// <summary>
        /// Gibt den Pfad des JobConfig-Objekt zurück.
        /// </summary>
        /// <param name="jobConfig"></param>
        /// <returns></returns>
        public string GetJobConfigPath(JobConfig jobConfig)
        {
            if (_jobIdPathDictionary.ContainsKey(jobConfig.Id))
            {
                return _jobIdPathDictionary[jobConfig.Id];
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region private-methods
        /// <summary>
        /// Sucht nach einer nicht vorhandenen ID.
        /// <para>
        /// Falls Sie schon existiert, wird ein numerischer Zusatz hinzugefügt, bis kein JobConfig-Objekt mit dieser ID existiert.
        /// </para>
        /// </summary>
        /// <param name="jobName">Die ID, die als Vorlage dient.</param>
        /// <returns>Gibt eine ID anhand der Vorlage zurück, die noch nicht vorhanden ist.</returns>
        private string SearchFreeJobId(string jobName)
        {
            string tmpJobName = jobName;
            int i = 2;
            while (GetJobConfig(tmpJobName) != null)
            {
                tmpJobName = $"{jobName}-{i}";
                i++;
            }
            jobName = tmpJobName;
            return jobName;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="jobId"></param>
        /// <param name="path"></param>
        private void AddNewJob(string jobId, string path)
        {
            JobIds.Add(jobId);
            _jobIdPathDictionary.Add(jobId, path);
        }
        #endregion
    }
}
