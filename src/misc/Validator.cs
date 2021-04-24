using Enable_Now_Konnektor_Bibliothek.src.config;
using Enable_Now_Konnektor_Bibliothek.src.jobs;
using log4net;
using System.Text.RegularExpressions;

namespace Enable_Now_Konnektor_Bibliothek.src.misc
{
    public class Validator
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        
        public readonly static string EmailPattern = "[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?";
        public readonly static string ServerNamePattern = @"[A-Za-z0-9-]*[A-Za-z0-9]+(\.[A-Za-z0-9-]*[A-Za-z0-9]+)+";
        public readonly static string EnableNowIdPattern = @"^(CD|BO|PR|GR|SL|M)_[0-9A-Za-z]+$";
        public readonly static string UrlPattern = @"(ht|f)tp(s?)\:\/\/[0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*(:(0-9)*)*(\/?)([a-zA-Z0-9\-\.\?\,\'\/\\\+&%\$#_]*)?";


        /// <summary>
        /// Prüft, ob ein Wert mit einem regulären Ausdruck übereinstimmt.
        /// </summary>
        /// <param name="value">Der Wert, der geprüft werden soll.</param>
        /// <param name="pattern">Der reguläre Ausdruck.</param>
        /// <returns>Falls der Wert dem regulären Ausdruck entspricht wird wahr zurückgegeben. Falls der Wert oder das Muster null ist oder
        /// der Wert nicht übereinstimmt, wird falsch zurückgegeben.</returns>
        public static bool Validate( string value, string pattern)
        {
            if( value == null ) { return false; }
            return Regex.Match(value, pattern).Success;
        }
    }
}
