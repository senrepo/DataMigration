using System.Collections.Generic;

namespace DataMigration
{
    public interface IPolicyQuote
    {
        string PolicyQuoteNumber { get; set; }
        bool? MigrationStatus { get; set; }
        List<IPolicyQuoteTransaction> Details { get; set; }
    }
}
