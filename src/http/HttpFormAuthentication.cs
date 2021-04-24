using Enable_Now_Konnektor_Bibliothek.src.jobs;
using Enable_Now_Konnektor_Bibliothek.src.service;
using HtmlAgilityPack;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Enable_Now_Konnektor_Bibliothek.src.http
{
    internal class HttpFormAuthentication : HttpAuthentication
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly HttpClient client;
        private readonly JobConfig jobConfig;

        public HttpFormAuthentication(HttpClient client)
        {
            this.client = client;
            jobConfig = JobManager.GetJobManager().SelectedJobConfig;
        }

        internal async Task AuthenticateAsync()
        {
            string content = await RequestFormAsync(jobConfig.AuthFormUrl);
            content = await FollowFormAsync(jobConfig.AuthFormUrl, content);
            content = await SendFormAsync(jobConfig.AuthFormActionUrl, content);
            await FollowFormAsync(jobConfig.AuthFormActionUrl, content);
        }



        /// <summary>
        /// Die Webseite des Formulars aufrufen.
        /// </summary>
        /// <returns>Den Inhalt der Serverantwort.</returns>
        private async Task<string> RequestFormAsync(string url)
        {
            Log.Info(LocalizationService.FormatResourceString("HttpFormAuthenticationMessage04", url));
            string formMethod = jobConfig.AuthFormMethod.ToLower();
            HttpResponseMessage response;
            if ("post".Equals(formMethod))
            {
                var body = new List<KeyValuePair<string, string>>();
                foreach (var param in jobConfig.AuthFormAdditionalParameters)
                {
                    body.Add(new KeyValuePair<string, string>(param.Key, param.Value));
                }
                response = await client.PostAsync(url, new FormUrlEncodedContent(body));
            }
            else
            {
                response = await client.GetAsync(url);
            }

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }



        /// <summary>
        /// Die Webseite des Formulars aufrufen.
        /// </summary>
        /// <returns>Den Inhalt der Serverantwort.</returns>
        private async Task<string> FollowFormAsync(string sourceUrl, string content)
        {
            if (content == null) return null;

            HtmlDocument htmlDoc = new();
            htmlDoc.LoadHtml(content);
            HtmlNode htmlForm = htmlDoc.DocumentNode.SelectSingleNode("//form");
            if (htmlForm == null) return content;

            string formId = htmlForm.Id ?? "";
            if (!formId.Equals(jobConfig.AuthFormId))
            {

                string action = htmlForm.GetAttributeValue("action", "");
                string actionUrl;
                if (Regex.IsMatch(action, "^/.*^$"))
                {
                    string baseUrl = Regex.Match(sourceUrl, @"https?:\/\/[^^/]+").Value;
                    actionUrl = baseUrl + "/" + action;
                }
                else if (Regex.IsMatch(action, "^https?://.+$"))
                {
                    actionUrl = action;
                }
                else
                {
                    string baseUrlPath = Regex.Match(sourceUrl, @"https?:\/\/(.*\/)*").Value;
                    actionUrl = baseUrlPath + action;
                }

                List<KeyValuePair<string, string>> body = new();
                AddFormParameters(htmlForm, body);

                Log.Info(LocalizationService.FormatResourceString("HttpFormAuthenticationMessage03", actionUrl));
                var response = await client.PostAsync(actionUrl, new FormUrlEncodedContent(body));
                string responseContent = await response.Content.ReadAsStringAsync();
                return await FollowFormAsync(actionUrl, responseContent);
            }
            return content;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        private async Task<string> SendFormAsync(string url, string content)
        {
            Log.Info(LocalizationService.FormatResourceString("HttpFormAuthenticationMessage02", url));
            List<KeyValuePair<string, string>> body = new()
            {
                new KeyValuePair<string, string>(jobConfig.AuthUserControl, jobConfig.AuthUser),
                new KeyValuePair<string, string>(jobConfig.AuthPasswordControl, jobConfig.AuthPassword)
            };
            foreach (var param in jobConfig.AuthFormActionAdditionalParameters)
            {
                body.Add(new KeyValuePair<string, string>(param.Key, param.Value));
            }
            AddFormParameters(content, body);
            HttpResponseMessage response = await client.PostAsync(url, new FormUrlEncodedContent(body));
            return await response.Content.ReadAsStringAsync();
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="content"></param>
        /// <param name="formData"></param>
        private void AddFormParameters(string content, List<KeyValuePair<string, string>> formData)
        {
            if (content == null) return;

            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(content);
            var htmlForm = htmlDoc.DocumentNode.SelectSingleNode("//form");
            AddFormParameters(htmlForm, formData);
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="form"></param>
        /// <param name="formData"></param>
        private void AddFormParameters(HtmlNode form, List<KeyValuePair<string, string>> formData)
        {
            if (form == null) return;

            var inputElements = form.SelectNodes("//input");
            foreach (var inputElement in inputElements)
            {
                string name = inputElement.GetAttributeValue("name", "");
                if (!jobConfig.AuthFormActionAdditionalParameters.ContainsKey(name) &&
                        !name.Equals(jobConfig.AuthUserControl) &&
                        !name.Equals(jobConfig.AuthPasswordControl) &&
                        name.Length > 0)
                {
                    string value = inputElement.GetAttributeValue("value", "");
                    Log.Debug(LocalizationService.FormatResourceString("HttpFormAuthenticationMessage01", name, value));
                    formData.Add(new KeyValuePair<string, string>(name, value));
                }
            }
        }
    }
}
