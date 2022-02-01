using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataMigration
{
    public class EmailNotification : INotification
    {
        private readonly IProgress result = null;
        private readonly IConfigReader config = null;
        private readonly IFileWritter csvFile = null;

        public EmailNotification(IProgress result, IConfigReader config, IFileWritter csvFile)
        {
            this.result = result;
            this.config = config;
            this.csvFile = csvFile;
        }
        
        public virtual void Send( )
        {
            var body = $"Total Policy/Quoute Count: {result.TotalPolicyQuotes}" + Environment.NewLine +
                       $"Passed: {result.Passed}" + Environment.NewLine +
                       $"Failed: {result.Failed}" + Environment.NewLine +
                       $"Process Start: {result.ProcessStart}" + Environment.NewLine +
                       $"Process End: {result.ProcessEnd}";

            //MailMessage mail = new MailMessage() { From = config.GetFromEmailAddress(), Body = body, Subject = "DATA LOAD NOTIFICATION" };
            //mail.Attachments.Add(Attachment.CreateFromFile(csvFile.GetFileName()));
            //mail.To.Add(config.GetToEmailAddress());
            //SmtpClient.SendMail(mail);
        }
    }
}
