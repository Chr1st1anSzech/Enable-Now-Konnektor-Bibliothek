using Enable_Now_Konnektor_Bibliothek.locals;
using log4net;
using System.Resources;

namespace Enable_Now_Konnektor_Bibliothek.src.service
{
    public class LocalizationService
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(LocalizationService));
        private static ResourceManager res;

        public static string GetFormattedResource(string key, params object[] parameters)
        {
            if (res == null)
            {
                InitializeResourceManager();
            }
            var txt = res.GetString(key) ?? "";
            if( parameters != null)
            {
                txt = string.Format(txt, parameters);
            }
            return txt;
        }

        private static void InitializeResourceManager()
        {
            res = de_DE.ResourceManager;
        }
    }
}
