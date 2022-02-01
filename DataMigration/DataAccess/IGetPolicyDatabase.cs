using System.Data;

namespace DataMigration
{
    public interface IGetPolicyDatabase
    {
        DataTable GetPolicyList(string startDate, string endDate);
        DataTable GetQuoteList(string startDate, string endDate);
    }
}
