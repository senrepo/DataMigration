using DataMigration;

namespace DataMigrationTests
{
    public class SetPolicyDatabaseStub : SetPolicyDatabase
    {
        public int GetPolicyTermStub(string policyTerm)
        {
            return base.GetPolicyTerm(policyTerm);
        }
    }
}
