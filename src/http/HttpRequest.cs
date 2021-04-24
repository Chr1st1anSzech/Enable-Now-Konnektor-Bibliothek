using Enable_Now_Konnektor_Bibliothek.src.config;
using Enable_Now_Konnektor_Bibliothek.src.jobs;
using Enable_Now_Konnektor_Bibliothek.src.service;
using log4net;
using System;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Enable_Now_Konnektor_Bibliothek.src.http
{
    public class HttpRequest
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static HttpClient HttpClient;
        private static bool IsAuthenticated = false;
        private static readonly Semaphore semaphore = new(1, 1);

        private readonly JobConfig jobConfig;

        public HttpRequest(JobConfig jobConfig)
        {
            this.jobConfig = jobConfig;
        }



        /// <summary>
        /// Konfiguration des HttpClients. CookieContainer hinzufügen und Proxy setzen.
        /// Zuletzt eine Authentifizierung durchführen.
        /// </summary>
        /// <returns></returns>
        private void ThreadSafeInitClient()
        {
            if (HttpClient == null)
            {
                semaphore.WaitOne();
                if (HttpClient == null)
                {
                    InitClient();
                }
                semaphore.Release();
            }
        }



        /// <summary>
        /// Konfiguration des HttpClients. CookieContainer hinzufügen und Proxy setzen.
        /// Zuletzt eine Authentifizierung durchführen.
        /// </summary>
        /// <returns></returns>
        private void InitClient()
        {
            JobConfig jobConfig = JobManager.GetJobManager().SelectedJobConfig;

            WebProxy proxy = null;
            if (!jobConfig.UseSystemProxy)
            {
                if (jobConfig.UseCredentials)
                {
                    if (string.IsNullOrWhiteSpace(jobConfig.ProxyUsername) || string.IsNullOrWhiteSpace(jobConfig.ProxyPassword))
                    {
                        string message = LocalizationService.FormatResourceString("HttpRequestMessage4");
                        Log.Error(message);
                        throw new ArgumentException(message);
                    }
                    proxy = new WebProxy(jobConfig.ProxyUrl, jobConfig.ProxyPort)
                    {
                        Credentials = new NetworkCredential(jobConfig.ProxyUsername, jobConfig.ProxyPassword)
                    };
                }
                else
                {
                    proxy = new WebProxy(jobConfig.ProxyUrl, jobConfig.ProxyPort);
                }
            }

            SocketsHttpHandler handler = new()
            {
                AllowAutoRedirect = true,
                MaxAutomaticRedirections = 10,
                CookieContainer = new CookieContainer(),
                UseCookies = true,
                ConnectTimeout = TimeSpan.FromSeconds(3d),
                UseProxy = jobConfig.UseProxy,
                Proxy = proxy
            };

            HttpClient = new(handler);
            try
            {
                AuthenticateAsync(HttpClient).Wait();
            }
            catch
            {

            }
        }



        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private async Task AuthenticateAsync(HttpClient client)
        {
            if (!IsAuthenticated && jobConfig.IsAuthRequired)
            {
                Log.Info(LocalizationService.FormatResourceString("HttpRequestMessage05"));

                if ("Formular".Equals(jobConfig.AuthType))
                {
                    await new HttpFormAuthentication(client).AuthenticateAsync();
                }
                else if ("Basic".Equals(jobConfig.AuthType))
                {

                }
                else
                {
                    throw new();
                }

                IsAuthenticated = true;
            }
        }




        internal async Task<HttpResponseMessage> SendRequestAsync(string url, string method, HttpContent content = null)
        {
            ThreadSafeInitClient();
            if ("post".Equals(method.ToLower()))
            {
                return await HttpClient.PostAsync(url, content);
            }
            else
            {
                return await HttpClient.GetAsync(url);
            }
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<string> SendRequestAsync(string url)
        {
            ThreadSafeInitClient();
            Log.Debug(LocalizationService.FormatResourceString("HttpRequestMessage01", url));
            try
            {
                var response = await HttpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                return responseBody;
            }
            catch (TaskCanceledException timeoutException)
            {
                Log.Error(LocalizationService.FormatResourceString("HttpRequestMessage02"), timeoutException);
                throw;
            }
            catch (Exception e)
            {
                Log.Error(LocalizationService.FormatResourceString("HttpRequestMessage03"), e);
                throw;
            }
        }
    }
}
