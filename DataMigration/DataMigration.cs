using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace DataMigration
{
    public abstract class DataMigration : IDataMigration
    {
        protected IConfigReader config = null;
        protected IGetPolicyDatabase database = null;
        protected ILogger logger = null;
        protected IFileWritter resultWritter = null;
        protected IProgress progress = null;
        protected INotification notification = null;

        protected IGetPolicyRepository getPolicyRepo = null;
        protected ISetPolicyRepository setPolicyRepo = null;

        protected DataTable dataTable = null;
        protected List<IPolicyQuote> policyList = null;

        public abstract void LoadPolicyQuoteData(PolicyQuoteType type);

        protected abstract void LoadTransaction(IPolicyQuoteTransaction transaction);
        protected abstract void DeleteTransactionDataFromWips(IPolicyQuoteTransaction transaction);
        protected abstract void MigrateTransctionData(IPolicyQuoteTransaction transaction);
        protected abstract List<IPolicyQuote> GetPolicyQuoteList();
        protected abstract void LoadTranscationDetails(IPolicyQuote policy);

        protected PolicyQuoteType currentRunType;
        private int batchCounter = 0;

        public DataMigration()
        {
        }

        public virtual void MigrateToDb()
        {
            try
            {
                var timer = new Stopwatch();
                timer.Start();
                progress.ProcessStart = string.IsNullOrWhiteSpace(progress.ProcessStart) ? DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") : progress.ProcessStart;
                logger.LogInfo($"{currentRunType} Data Load Start: {DateTime.Now}");
                progress.Status = $"Processing the {currentRunType} list";

                policyList = GetPolicyQuoteList();
                logger.LogInfo($"Toal retrieved {currentRunType} count from database: {policyList.Count()}");

                var policyChuncks = policyList.SplitIntoChunks(config.GetDataPartitionLimt());
                logger.LogInfo($"Batch count: {policyChuncks.Count()}");

                progress.TotalPolicyQuotes += policyList.Count();
                progress.TotalBatches += policyChuncks.Count();

                int counter = 0;
                foreach (var policies in policyChuncks)
                {
                    var batchTimer = new Stopwatch();
                    batchTimer.Start();
                    progress.CurrentBatch = ++batchCounter;
                    logger.LogInfo($"Batch: {++counter} Start: {DateTime.Now}");

                    Migrate(policies);

                    var retryTransactions = new List<IPolicyQuoteTransaction>();
                    policies.ForEach(policy =>
                    {
                        var transactions = policy.Details.Where(transaction => transaction.RetryCount > 0);
                        retryTransactions.AddRange(transactions);
                    });
                    RetryTranscations(retryTransactions);

                    GetBatchResults(policies);
                    progress.Passed += policies.Where(p => p.MigrationStatus == true).Count();
                    progress.Failed += policies.Where(p => p.MigrationStatus != true).Count();

                    policies.ForEach(policy =>
                    {
                        if (policy.Details?.Count > 0)
                        {
                            DeleteTransactionDataFromWips(policy.Details.First());
                            policy.Details.ForEach(policyDetails => policyDetails.DeletedFromWips = true);
                        }
                    });
                    resultWritter.Write(policies);

                    logger.LogInfo($"Batch: {counter} data cleared from Wips");
                    batchTimer.Stop();
                    TimeSpan batchTimeTaken = batchTimer.Elapsed;
                    logger.LogInfo($"Batch: {counter} End: {DateTime.Now}, Time Time taken: {batchTimeTaken}");

                }

                progress.Status = $"Processing Completed";
                timer.Stop();
                TimeSpan timeTaken = timer.Elapsed;
                logger.LogInfo($"{currentRunType} Data Load End: {DateTime.Now}, Time Time taken: {timeTaken}");
                progress.ProcessEnd = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            }
            catch(Exception ex)
            {
                progress.Status = $"Error while processing the {currentRunType} data...";
                logger.LogException(ex);
            }
        }

        private void Migrate(List<IPolicyQuote> policies)
        {
            var taskList = new List<Task>();
            foreach (var policy in policies)
            {
                taskList.AddRange(ProcessPolicy(policy));
             }   
            
            Task.WaitAll(taskList.ToArray());
        }

        private List<Task> ProcessPolicy(IPolicyQuote policy)
        {
            var taskList = new List<Task>();
            try
            {
                LoadTranscationDetails(policy);

                foreach (var transaction in policy.Details)
                {
                    progress.Status = $"Processing {currentRunType} - {transaction.PolicyQuoteNumber.Trim()}, Transaction: {transaction.TransactionNumber.Trim()}, {transaction.TransactionType.Trim()}, {transaction.AlternateTransactionType.Trim()}";

                    var task = RunTask(transaction);
                    if (task != null)
                    {
                        taskList.Add(task);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
            }

            return taskList;

        }

        private void RetryTranscations(List<IPolicyQuoteTransaction> transactions)
        {
            var taskList = new List<Task>();
            foreach (var transaction in transactions)
            {
                progress.Status = $"Retrying {currentRunType} - {transaction.PolicyQuoteNumber.Trim()}, Transaction: {transaction.TransactionNumber.Trim()}, {transaction.TransactionType.Trim()}, {transaction.AlternateTransactionType.Trim()}";

                var task = RunTask(transaction, true);
                if (task != null)
                {
                    taskList.Add(task);
                }
            }
            Task.WaitAll(taskList.ToArray());
        }

        private Task RunTask(IPolicyQuoteTransaction transaction, bool isRetry = false)
        {
            Task task = Task.Factory.StartNew((obj) =>
            {
                LoadTransaction(transaction);
                if (transaction.LoadedToWips == true)
                {
                    //logger.LogInfo(GetLogDetailMessage("Load Wips Success", transaction));
                    MigrateTransctionData(transaction);
                    if (transaction.MigratedToDatabase == true)
                    {
                        // logger.LogInfo(GetLogDetailMessage("Database Migration Success", transaction));
                    }
                    else
                    {
                        logger.LogInfo(GetLogDetailMessage("Database Migration Failure", transaction));
                        transaction.RetryCount++;
                    }
                }
                else
                {
                    logger.LogInfo(GetLogDetailMessage("Load Wips Failure", transaction));
                    transaction.RetryCount++;
                }

                if (isRetry == true) Task.Delay(1);  //try to avoid any cuncurreny issues because of two threads are attempting to insert some record in the database at the the exact same time

            }, transaction, TaskCreationOptions.LongRunning);

            return task;
        }

        public List<IPolicyQuote> GetBatchResults(List<IPolicyQuote> batch)
        {
            batch.ForEach(policy =>
            {
                policy.MigrationStatus = policy.Details?.All(transaction => transaction.MigratedToDatabase == true);
            });

            return batch;

        }

        public virtual List<IPolicyQuote> GetResults()
        {
            policyList.ForEach(policy =>
            {
                policy.MigrationStatus = policy.Details?.All(transaction => transaction.MigratedToDatabase == true);
            });

            return policyList;
        }

        public virtual void SendNotification()
        {
            try
            {
                notification.Send();
                logger.LogInfo($"Data Load results sent in email successfully");
            }
            catch(Exception ex)
            {
                logger.LogException(ex);
            }
            
        }

        private string GetLogDetailMessage(string message, IPolicyQuoteTransaction transaction)
        {
            message += $", {currentRunType}Number: {transaction.PolicyQuoteNumber.Trim()}, TransNumber: { transaction.TransactionNumber.Trim()}, TransType: { transaction.TransactionType.Trim()}, AltTransType: {transaction.AlternateTransactionType.Trim()}";
            return string.IsNullOrWhiteSpace(transaction.ErrorMessage) ? message: message + $", Error Message: {transaction.ErrorMessage}";
            
        }


    }
}
