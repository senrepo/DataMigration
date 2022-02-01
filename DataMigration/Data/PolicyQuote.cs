using System.Collections.Generic;

namespace DataMigration
{
    public class PolicyQuote : IPolicyQuote
    {
        public string PolicyQuoteNumber { get; set; }
        public bool? MigrationStatus { get; set; }
        public List<IPolicyQuoteTransaction> Details { get; set; }
    }
}
