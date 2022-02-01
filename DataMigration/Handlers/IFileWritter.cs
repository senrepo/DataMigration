using System.Collections.Generic;

namespace DataMigration
{
    public interface IFileWritter
    {
        void Write(List<IPolicyQuote> policyQuoteList);
        string GetFileName();
    }
}
