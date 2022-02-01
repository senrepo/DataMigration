namespace DataMigration
{
    public class PolicyQuoteTransaction : IPolicyQuoteTransaction
    {
        public string InstanceId { get; set; }
        public string PolicyQuoteNumber { get; set; }
        public bool? IsQuote { get; set; }
        public string TransactionNumber { get; set; }
        public string TransactionType { get; set; }
        public string AlternateTransactionType { get; set; }
        public bool? LoadedToWips { get; set; }
        public bool? MigratedToDatabase { get; set; }
        public bool? DeletedFromWips { get; set; }
        public int RetryCount { get; set; }
        public string ErrorMessage { get; set; }
    }
}
