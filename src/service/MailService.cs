using Enable_Now_Konnektor_Bibliothek.src.jobs;
using log4net;
using System;
using System.Net.Mail;

namespace Enable_Now_Konnektor_Bibliothek.src.service
{
    public class MailService
    {
        private readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly JobConfig jobConfig;

        public MailService(JobConfig jobConfig)
        {
            this.jobConfig = jobConfig;
        }

        public void SendMail(string text)
        {
            SmtpClient smtpClient = new SmtpClient(jobConfig.EmailSmtpServer, jobConfig.EmailPort);
            if (!jobConfig.EmailSend) { return; }

            try
            {
                smtpClient.SendAsync(
                    jobConfig.EmailSender,
                    jobConfig.EmailRecipient,
                    jobConfig.EmailSubject,
                    text,
                    jobConfig.Id);
            }
            catch (Exception e)
            {
                log.Error(LocalizationService.FormatResourceString("MailClientMessage02"), e);
            }
        }
    }
}
