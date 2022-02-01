namespace DataMigration
{
    public class SetPolicyRespository : ISetPolicyRepository
    {
        private readonly ISetPolicyDatabase policyDb = null;

        public SetPolicyRespository(ISetPolicyDatabase database)
        {
            policyDb = database;
        }

        public bool SetPolicyQuote(IPolicyQuoteTransaction tranaction)
        {
            return policyDb.SetPolicy(tranaction);
        }
    }
}
