using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;

namespace Enable_Now_Konnektor_Bibliothek.src.jobs
{
    internal class RecentJobsReader : RecentJobsIO
    {

        internal JObject ReadJson()
        {
            try
            {
                if (!File.Exists(recentJobsFilePath))
                {
                    
                    File.WriteAllText(recentJobsFilePath, defaultJson);
                }
                string jsonString = File.ReadAllText(recentJobsFilePath);
                return JsonConvert.DeserializeObject<JObject>(jsonString);
            }
            catch
            {
                return null;
            }
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        internal JArray ReadJArray(JObject json)
        {
            return json["recent-jobs"].Value<JArray>();
        }



        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal List<string> ReadRecentJobs()
        {
            try
            {
                JObject json = ReadJson();
                if (json == null) return new List<string>();

                JArray jobs = ReadJArray(json);
                List<string> recentJobList = new(jobs.Count);
                foreach (var job in jobs)
                {
                    string jobId = job.Value<string>() ?? "N/A";
                    recentJobList.Add(jobId);
                }
                return recentJobList;
            }
            catch
            {
                return new List<string>();
            } 
        }
    }
}
