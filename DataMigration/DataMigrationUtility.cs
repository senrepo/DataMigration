using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DataMigration
{
    public class DataMigrationUtility : DataMigration
    {
        public DataMigrationUtility()
        {
            config = new ConfigReader();

            var getPolicyDb = new GetPolicyDatabase();
            getPolicyRepo = new GetPolicyRepository(getPolicyDb);

            var setPolicyDb = new SetPolicyDatabase();
            setPolicyRepo = new SetPolicyRespository(setPolicyDb);

            logger = new DbLogger();

            resultWritter = new CsvFileWritter();
            progress = new ConsoleProgress();

            notification = new EmailNotification(progress, config, resultWritter);
        }

        public DataMigrationUtility(IConfigReader config, IGetPolicyRepository getPolicyRepo, ISetPolicyRepository setPolicyRepo, ILogger logger, IFileWritter resultWritter, IProgress progress, INotification email)
        {
            this.config = config;
            this.getPolicyRepo = getPolicyRepo;
            this.setPolicyRepo = setPolicyRepo;
            this.logger = logger;
            this.resultWritter = resultWritter;
            this.progress = progress;
            this.notification = email;
        }

        public override void LoadPolicyQuoteData(PolicyQuoteType type)
        {
            try
            {
                this.currentRunType = type;
                progress.Status = $"Loading the {currentRunType} data...";
                this.dataTable = type == PolicyQuoteType.Quote ?
                                    getPolicyRepo.GetQuoteData(config.GetStartDate(), config.GetEndDate()) :
                                    getPolicyRepo.GetPolicyData(config.GetStartDate(), config.GetEndDate());
            }
            catch (Exception ex)
            {
                progress.Status = $"Error while loading the {currentRunType} data...";
                logger.LogException(ex);
            }
        }

        protected override List<IPolicyQuote> GetPolicyQuoteList()
        {
            var policies = dataTable.AsEnumerable().Select(row => new PolicyQuote { PolicyQuoteNumber = row.Field<string>("PolicyQuoteNumber") });
            var results = policies.GroupBy(policy => policy.PolicyQuoteNumber).Select(g => g.First());
            return results?.ToList<IPolicyQuote>();
        }

        protected override void LoadTransaction(IPolicyQuoteTransaction transaction)
        {
            try
            {
                transaction.LoadedToWips = getPolicyRepo.GetPolicyQuote(transaction);
            }
            catch (Exception ex)
            {
                transaction.LoadedToWips = false;
                logger.LogException(ex);
            }
        }

        protected override void MigrateTransctionData(IPolicyQuoteTransaction transaction)
        {
            try
            {
                transaction.MigratedToDatabase = setPolicyRepo.SetPolicyQuote(transaction);
            }
            catch (Exception ex)
            {
                transaction.MigratedToDatabase = false;
                logger.LogException(ex);
            }
        }

        protected override void LoadTranscationDetails(IPolicyQuote policy)
        {
            var details = dataTable.AsEnumerable().Where(row => row.Field<string>("PolicyQuoteNumber") == policy.PolicyQuoteNumber)
                          .Select(row =>
                          {
                              var transaction = new PolicyQuoteTransaction();
                              transaction.IsQuote = row.Field<string>("QuotePolicyIndicator") == "Q";
                              transaction.InstanceId = row.Field<Guid>("InstanceId").ToString();
                              transaction.PolicyQuoteNumber = row.Field<string>("PolicyQuoteNumber");
                              transaction.TransactionNumber = transaction.IsQuote == true ? row.Field<string>("TransactionNumber").ToString(): row.Field<Int64>("TransactionNumber").ToString();
                              transaction.TransactionType = row.Field<string>("TransactionType");
                              transaction.AlternateTransactionType = string.IsNullOrWhiteSpace(row.Field<string>("AlternateTransactionType")) ? row.Field<string>("TransactionType") : row.Field<string>("AlternateTransactionType");
                              return transaction;
                          });

            policy.Details = details.ToList<IPolicyQuoteTransaction>();
        }

        protected override void DeleteTransactionDataFromWips(IPolicyQuoteTransaction transaction)
        {
            try
            {
                transaction.DeletedFromWips = getPolicyRepo.DeleteTranscation(transaction);
            }
            catch(Exception ex)
            {
                transaction.DeletedFromWips = false;
                logger.LogException(ex);
            }
        }
    }
}
