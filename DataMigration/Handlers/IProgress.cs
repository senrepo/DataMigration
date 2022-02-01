namespace DataMigration
{
    public interface IProgress
    {
        int TotalPolicyQuotes { get; set; }
        int Passed { get; set; }
        int Failed { get; set; }

        int TotalBatches { get; set; }
        int CurrentBatch { get; set; }
        string Status { get; set; }
        string ProcessStart { get; set; }
        string ProcessEnd { get; set; }
    }
}
