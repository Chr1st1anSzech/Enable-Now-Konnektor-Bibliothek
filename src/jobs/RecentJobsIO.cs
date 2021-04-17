using Enable_Now_Konnektor_Bibliothek.src.misc;
using System.IO;

namespace Enable_Now_Konnektor_Bibliothek.src.jobs
{
    internal class RecentJobsIO
    {
        protected readonly string recentJobsFilePath = Path.Combine(Util.GetApplicationRoot(), "recent-jobs.json");
        protected readonly string defaultJson = "{ \"recent-jobs\" : [] }";
    }
}
