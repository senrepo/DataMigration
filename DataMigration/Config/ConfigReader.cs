using System;
using System.Configuration;

namespace DataMigration
{
    public class ConfigReader : IConfigReader
    {
        public ConfigReader()
        {
        }

        public int GetDataPartitionLimt()
        {
            return Convert.ToInt32(ConfigurationManager.AppSettings["BatchSize"]);
        }

        public string GetStartDate()
        {
            return ConfigurationManager.AppSettings["StartDate"];
        }
        public string GetEndDate()
        {
            return ConfigurationManager.AppSettings["EndDate"];
        }

        public string GetDataLoadType()
        {
            return ConfigurationManager.AppSettings["DataLoadType"];
        }
        public string GetFromEmailAddress()
        {
            return ConfigurationManager.AppSettings["FromEmailAddress"];
        }
        public string GetToEmailAddress()
        {
            return ConfigurationManager.AppSettings["ToEmailAddress"];
        }


    }
}
