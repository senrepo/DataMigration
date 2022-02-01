using System.Collections.Generic;

namespace DataMigration
{
    public interface IDataMigration
    {
        void LoadPolicyQuoteData(PolicyQuoteType type);
        void MigrateToDb();
        List<IPolicyQuote> GetResults();

    }
}
