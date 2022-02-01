namespace DataMigration
{
    public interface IPolicyQuoteTransaction
    {
        string InstanceId { get; set; }
        string PolicyQuoteNumber { get; set; }
        bool? IsQuote { get; set; }
        string TransactionNumber { get; set; }
        string TransactionType { get; set; }
        string AlternateTransactionType { get; set; }
        bool? LoadedToWips { get; set; }
        bool? MigratedToDatabase { get; set; }
        bool? DeletedFromWips { get; set; }
        int RetryCount { get; set; }
        string ErrorMessage { get; set; }
    }
}
