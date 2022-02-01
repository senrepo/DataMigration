namespace DataMigration
{
    public interface IConfigReader
    {
        int GetDataPartitionLimt();
        string GetStartDate();
        string GetEndDate();
        string GetDataLoadType();
        string GetFromEmailAddress();
        string GetToEmailAddress();

    }
}
