using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DataMigration
{
    public class GetPolicyRepository : IGetPolicyRepository
    {

        private readonly IGetPolicyDatabase getPolicydatabase = null;

        public GetPolicyRepository(IGetPolicyDatabase database)
        {
            getPolicydatabase = database;
        }

        public DataTable GetQuoteData(string startDate, string endDate)
        {
            return getPolicydatabase.GetQuoteList(startDate, endDate);
        }

        public DataTable GetPolicyData(string startDate, string endDate)
        {
            return getPolicydatabase.GetPolicyList(startDate, endDate);
        }

        public bool GetPolicyQuote(IPolicyQuoteTransaction tranaction)
        {
            return true;
        }

        public bool DeleteTranscation(IPolicyQuoteTransaction transaction)
        {
            return true;
        }

        public bool DeletePolicies(List<IPolicyQuote> policyList)
        {
            return true;
        }
    }
}
