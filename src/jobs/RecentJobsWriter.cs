using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Linq;

namespace Enable_Now_Konnektor_Bibliothek.src.jobs
{
    internal class RecentJobsWriter : RecentJobsIO
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="json"></param>
        private void WriteRecentJobs(JObject json)
        {
            try
            {
                string outputJson = JsonConvert.SerializeObject(json);
                File.WriteAllText(recentJobsFilePath, outputJson);
            }
            catch
            {

            }

        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        internal void AddRecentJob(string jobId)
        {
            try
            {
                RecentJobsReader reader = new();
                JObject recentJobsJson = reader.ReadJson();
                JArray recentJobs = reader.ReadJArray(recentJobsJson);
                if (recentJobs == null) return;

                JToken token = recentJobs.FirstOrDefault(token => token.Value<string>().Equals(jobId));
                if (token == null)
                {
                    recentJobs.Add(jobId);
                    WriteRecentJobs(recentJobsJson);
                }
            }
            catch
            {

            }
        }
    }
}
