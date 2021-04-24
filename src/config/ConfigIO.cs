using Enable_Now_Konnektor_Bibliothek.src.misc;
using log4net;
using System.IO;
using System.Reflection;

namespace Enable_Now_Konnektor_Bibliothek.src.config
{
    public class ConfigIO
    {
        protected static readonly string s_filePath = Path.Combine(Util.GetApplicationRoot(), "config.json");
        protected static readonly ILog s_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    }
}
