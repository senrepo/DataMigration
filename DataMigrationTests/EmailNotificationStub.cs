using DataMigration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataMigrationTests
{
    public class EmailNotificationStub : EmailNotification
    {
        private readonly IProgress result = null;
        private readonly IConfigReader config = null;
        private readonly IFileWritter csvFile = null;

        public EmailNotificationStub(IProgress result, IConfigReader config, IFileWritter csvFile) : base(result,config,csvFile)
        {
            this.result = result;
            this.config = config;
            this.csvFile = csvFile;

        }

        public override void Send()
        {
            var body = $"Total Policy/Quoute Count: {result.TotalPolicyQuotes}" + Environment.NewLine +
                       $"Passed: {result.Passed}" + Environment.NewLine +
                       $"Failed: {result.Failed}" + Environment.NewLine +
                       $"Process Start: {result.ProcessStart}" + Environment.NewLine +
                       $"Process End: {result.ProcessEnd}";
            var fromEmail = config.GetFromEmailAddress();
            var toEmail = config.GetToEmailAddress();
            var fileName = csvFile.GetFileName();

            Console.WriteLine($"Body: {body}");
            Console.WriteLine($"FromEmail: {fromEmail}");
            Console.WriteLine($"ToEmail: {toEmail}");
            Console.WriteLine($"FileName: {fileName}");
        }
    }
}
