using System;
using System.Collections.Generic;

namespace Enable_Now_Konnektor_Bibliothek.src.jobs
{
    public class JobConfig
    {
        public string Id { get; set; }
        public string StartId { get; set; } = "";

        public bool IsTemplateJob { get; set; } = false;
        public string TemplateJobId { get; set; } = "";
        public List<string> TemplateOverwriteProperties { get; set; } = new();

        public bool UseTodayWhenDateEmpty { get; set; } = true;

        public bool AutostartMetaMapping { get; set; } = true;
        public bool AutostartChildOverwrite { get; set; } = false;
        public string[] AutoStartMappingBlacklist { get; set; } = Array.Empty<string>();

        public bool AttachementUrlOverwrite { get; set; } = true;

        public string IndexerType { get; set; } = "Json";

        public bool IndexAttachements { get; set; } = true;
        public bool IndexSlides { get; set; } = true;
        public bool IndexGroups { get; set; } = true;
        public bool IndexProjects { get; set; } = true;
        public bool IndexBooks { get; set; } = true;
        public bool IndexText { get; set; } = true;

        public string PublicationSource { get; set; } = "Website";

        public string EntityPath { get; set; } = "";
        public string EntityUrl { get; set; } = "";
        public string ContentPath { get; set; } = "";
        public string ContentUrl { get; set; } = "";
        public string DemoUrl { get; set; } = "";

        public string ProxyUrl { get; set; } = "http://proxy.de";
        public int ProxyPort { get; set; } = 0;
        public bool UseProxy { get; set; } = false;
        public bool UseSystemProxy { get; set; } = true;
        public bool UseCredentials { get; set; } = false;
        public string ProxyUsername { get; set; } = "";
        public string ProxyPassword { get; set; } = "";

        public bool EmailSend { get; set; } = false;
        public string EmailRecipient { get; set; } = "max.mustermann@muster.de";
        public string EmailSender { get; set; } = "crawler@muster.de";
        public string EmailSubject { get; set; } = "Enable Now Konnektor Bericht";
        public string EmailSmtpServer { get; set; } = "smtp.firma.de";
        public int EmailPort { get; set; } = 0;

        public bool IsAuthRequired { get; set; } = false;
        public string AuthType { get; set; } = "Formular";
        public string AuthFormId { get; set; } = "loginform";
        public string AuthFormUrl { get; set; } = "http://server.de/login";
        public string AuthFormMethod { get; set; } = "Get";
        public Dictionary<string, string> AuthFormAdditionalParameters { get; set; } = new();
        public string AuthFormActionUrl { get; set; } = "http://server.de/send";
        public Dictionary<string, string> AuthFormActionAdditionalParameters { get; set; } = new();
        public string AuthUserControl { get; set; } = "os_username";
        public string AuthUser { get; set; } = "Max Mustermann";
        public string AuthPasswordControl { get; set; } = "os_password";
        public string AuthPassword { get; set; } = "123456789";

        public Dictionary<string, string> BlacklistFields { get; set; } = new();
        public string[] MustHaveFields { get; set; } = Array.Empty<string>();

        public Dictionary<string, string[]> GlobalMappings { get; set; } = new();
        public Dictionary<string, string[]> ProjectMappings { get; set; } = new();
        public Dictionary<string, string[]> SlideMappings { get; set; } = new();
        public Dictionary<string, string[]> GroupMappings { get; set; } = new();
        public Dictionary<string, string[]> TextMappings { get; set; } = new();
        public Dictionary<string, string[]> BookMappings { get; set; } = new();

        public int ThreadCount { get; set; } = 2;

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (!(obj is JobConfig))
            {
                return false;
            }
            return (Id == ((JobConfig)obj).Id);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}