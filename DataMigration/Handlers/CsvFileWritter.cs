using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace DataMigration
{
    public class CsvFileWritter : IFileWritter
    {
        private readonly string folder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\";
        private readonly string fileName = string.Empty;
        private readonly string filePath = string.Empty;

        public CsvFileWritter()
        {
            fileName = DateTime.Now.ToString("MM-dd-yyyy-HH-mm-ss");
            fileName = $"Results-{fileName}.csv";
            filePath = folder + fileName;
            //AddHeader();
        }

        public string GetFileName()
        {
            return filePath;
        }

        public void Write(List<IPolicyQuote> policyQuoteList)
        {
            //using (StreamWriter writer = new StreamWriter(path: filePath, append: true))
            //{
            //    foreach (var policy in policyQuoteList)
            //    {
            //        foreach (var transaction in policy.Details)
            //        {
            //            writer.WriteLine($"{transaction.InstanceId}," +
            //                $"{transaction.UserID}," +
            //                $"{transaction.PolicyQuoteNumber}," +
            //                $"{transaction.TransactionNumber}," +
            //                $"{transaction.TransactionType}," +
            //                $"{transaction.AlternateTransactionType}," +
            //                $"{transaction.LoadedToWips}," +
            //                $"{transaction.MigratedToDatabase}," +
            //                $"{transaction.DeletedFromWips}"
            //                );
            //        }
            //    }
            //}

            // Append to the file.
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                // Don't write the header again.
                HasHeaderRecord = true,
                
            };
            using (var stream = File.Open(filePath, FileMode.Append))
            using (var writer = new StreamWriter(stream))
            using (var csv = new CsvWriter(writer, config))
            {
                foreach (var policy in policyQuoteList)
                {
                    if(policy.Details?.Count > 0)
                    {
                        csv.WriteRecords(policy.Details);
                    }
                }
            }

        }

        private void AddHeader()
        {
            using (StreamWriter writer = new StreamWriter(path: filePath, append: true))
            {
                writer.WriteLine($"InstanceId,UserID,PolicyQuoteNumber,TransactionNumber,TransactionType,AlternateTransactionType,LoadedToWips,MigratedToDatabase,DeletedFromWips");
            }

        }
    }
}
