using Enable_Now_Konnektor_Bibliothek.src.misc;
using log4net;
using System.IO;
using System.Reflection;

namespace Enable_Now_Konnektor_Bibliothek.src.jobs
{
    public class JobIO
    {
        protected static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        protected static string JobDirectory = Path.Combine(Util.GetApplicationRoot(), "jobs");
    }
}
