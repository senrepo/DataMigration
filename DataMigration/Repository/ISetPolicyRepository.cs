namespace DataMigration
{
    public interface ISetPolicyRepository
    {
        bool SetPolicyQuote(IPolicyQuoteTransaction tranaction);
    }
}
