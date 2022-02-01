using DataMigration;
using System.Text;

namespace DataMigrationTests
{
    public class GetPolicyDatabaseStub : GetPolicyDatabase
    {
        protected override string GetPolicyLoadQuery(string startDate, string endDate)
        {
            StringBuilder resultStirng = new StringBuilder();
            resultStirng.Append(GetPolicyQuery(startDate, endDate));
            resultStirng.Append($" WHERE Policy_Number = 'S  1234567890'"); //good policy
            return resultStirng.ToString();
        }

        protected override string GetQuoteLoadQuery(string startDate, string endDate)
        {
            StringBuilder resultStirng = new StringBuilder();
            resultStirng.Append(GetQuoteQuery(startDate, endDate));
            resultStirng.Append($" WHERE Policy_Number = 'S  1234567890'"); //good policy
            return resultStirng.ToString();
        }
    }
}
