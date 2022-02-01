using Microsoft.VisualStudio.TestTools.UnitTesting;
using DataMigration;
using System;

namespace DataMigrationTests
{
    [TestClass]
    public class ExtensionTests
    {
        [TestMethod]
        public void TestGetXml()
        {
            var transaction = new PolicyQuoteTransaction()
            {
                InstanceId = Guid.NewGuid().ToString(),
                PolicyQuoteNumber = "12345",
                IsQuote = false,
                TransactionNumber = "1",
                TransactionType = "NBIS",
                AlternateTransactionType = "ERRC",
                LoadedToWips = true,
                MigratedToDatabase = false,
                DeletedFromWips = null,
                RetryCount = 1
            };

            var result = transaction.GetXml();
            Assert.IsNotNull(result);

        }
    }
}
