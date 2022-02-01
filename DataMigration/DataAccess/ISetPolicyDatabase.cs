namespace DataMigration
{
    public interface ISetPolicyDatabase
    {
        bool SetPolicy(IPolicyQuoteTransaction transaction);
    }
}
