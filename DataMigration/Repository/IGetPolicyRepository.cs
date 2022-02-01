using System.Collections.Generic;
using System.Data;

namespace DataMigration
{
    public interface IGetPolicyRepository
    {
        DataTable GetPolicyData(string startDate, string endDate);
        DataTable GetQuoteData(string startDate, string endDate);
        bool GetPolicyQuote(IPolicyQuoteTransaction tranaction);
        bool DeleteTranscation(IPolicyQuoteTransaction tranaction);
        bool DeletePolicies(List<IPolicyQuote> policyList);
    }
}
