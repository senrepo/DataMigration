using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;


namespace DataMigration
{
    public class GetPolicyDatabase : IGetPolicyDatabase
    {

        public GetPolicyDatabase()
        {
        }

        public DataTable GetPolicyList(string startDate, string endDate)
        {
            return null;
        }
        public DataTable GetQuoteList(string startDate, string endDate)
        {
            return null;
        }

        public bool DeletePolicy(string quotePolicyNumber)
        {
            return false;
        }

        protected virtual string GetPolicyLoadQuery(string startDate, string endDate)
        {
            return string.Empty;
        }
        protected virtual string GetPolicyQuery(string startDate, string endDate)
        {
            return string.Empty;
        }
        protected virtual string GetQuoteLoadQuery(string startDate, string endDate)
        {
            return string.Empty;
        }
        protected virtual string GetQuoteQuery(string startDate, string endDate)
        {
            return string.Empty;
        }

    }
}
