using System;
using System.Collections.Generic;

namespace DataMigration
{
    class Program
    {
        static void Main(string[] args)
        {
            var logger = new DbLogger();
            try
            {
                var config = new ConfigReader();
                var getPolicyDb = new GetPolicyDatabase();
                var getPolicyRepo = new GetPolicyRepository(getPolicyDb);
                var setPolicyDb = new SetPolicyDatabase();
                var setPolicyRepo = new SetPolicyRespository(setPolicyDb);

                var resultWritter = new CsvFileWritter();
                var progress = new ConsoleProgress();
                var notification = new EmailNotification(progress, config, resultWritter);

                var ppsDataMigrationUtil = new DataMigrationUtility(config, getPolicyRepo, setPolicyRepo, logger, resultWritter, progress, notification);

                List<PolicyQuoteType> runList = new List<PolicyQuoteType>();
                if (string.IsNullOrWhiteSpace(config.GetDataLoadType()))
                {
                    runList.Add(PolicyQuoteType.Quote);
                    runList.Add(PolicyQuoteType.Policy);
                }
                else
                {
                    runList.Add(config.GetDataLoadType() == "Policy" ? PolicyQuoteType.Policy : PolicyQuoteType.Quote);
                }

                runList.ForEach(runType =>
                {
                    ppsDataMigrationUtil.LoadPolicyQuoteData(runType);
                    ppsDataMigrationUtil.MigrateToDb();
                });
                ppsDataMigrationUtil.SendNotification();

            }
            catch (Exception ex)
            {
                logger.LogException(ex);
            }
            

            Console.ReadLine();
        }

    }
}
