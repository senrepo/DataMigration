using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using DataMigration;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;

namespace DataMigrationTests
{
    [TestClass]
    public class DataMigrationUtilityTests
    {
        private static readonly string currentFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        private Mock<IConfigReader> _configMock = null;
        private Mock<IGetPolicyRepository> _getPolicyRepoMock = null;
        private Mock<ISetPolicyRepository> _setPolicyRepoMock = null;
        private Mock<ILogger> _logger = null;
        private Mock<IFileWritter> _resultWritter = null;
        private Mock<IProgress> _progress = null;
        private Mock<INotification> _notification = null;

        [TestInitialize]
        public void TestInitialize()
        {
            _configMock = new Mock<IConfigReader>();
            _getPolicyRepoMock = new Mock<IGetPolicyRepository>();
            _setPolicyRepoMock = new Mock<ISetPolicyRepository>();
            _logger = new Mock<ILogger>();
            _resultWritter = new Mock<IFileWritter>();
            _progress = new Mock<IProgress>();
            _notification = new Mock<INotification>();

        }

        [TestMethod]
        public void TestMigrationFlow()
        {
            var dataMigrationUtil = new DataMigrationUtility(_configMock.Object, _getPolicyRepoMock.Object, _setPolicyRepoMock.Object, _logger.Object, _resultWritter.Object, _progress.Object, _notification.Object);
            _configMock.Setup(x => x.GetDataPartitionLimt()).Returns(2);
            _getPolicyRepoMock.Setup(x => x.GetPolicyData(It.IsAny<string>(), It.IsAny<string>())).Returns(() =>
              {
                  var dataTable = CreateDataTableAndLoadData("Data.csv");
                  return dataTable;
              });
            _getPolicyRepoMock.Setup(x => x.GetPolicyQuote(It.IsAny<IPolicyQuoteTransaction>())).Returns(true);
            _getPolicyRepoMock.Setup(x => x.DeleteTranscation(It.IsAny<IPolicyQuoteTransaction>())).Returns(true);
            _setPolicyRepoMock.Setup(x => x.SetPolicyQuote(It.IsAny<IPolicyQuoteTransaction>())).Returns(true);

            dataMigrationUtil.LoadPolicyQuoteData(PolicyQuoteType.Policy);
            dataMigrationUtil.MigrateToDb();

            var results = dataMigrationUtil.GetResults();

            //check transactions loaded into wips
            Assert.IsTrue(results.All(p =>
            {
                return p.Details.All(r => r.LoadedToWips == true);
            }));

            //checkk all Policy/Quote transactions migrated
            Assert.AreEqual(results.Count(), results.Where(policy => policy.MigrationStatus == true).Count());

            //All transactions removed from wips
            Assert.IsTrue(results.All(p =>
            {
                return p.Details.All(r => r.DeletedFromWips == true);
            }));
        }

        [TestMethod]
        public void TestMigrationRetry()
        {
            var dataMigrationUtil = new DataMigrationUtility(_configMock.Object, _getPolicyRepoMock.Object, _setPolicyRepoMock.Object, _logger.Object, _resultWritter.Object, _progress.Object, _notification.Object);
            _configMock.Setup(x => x.GetDataPartitionLimt()).Returns(25);
            _getPolicyRepoMock.Setup(x => x.GetPolicyData(It.IsAny<string>(), It.IsAny<string>())).Returns(() =>
            {
                var dataTable = CreateDataTableAndLoadData("DataRetry.csv");
                return dataTable;

            });
            _getPolicyRepoMock.SetupSequence(x => x.GetPolicyQuote(It.IsAny<IPolicyQuoteTransaction>()))
                               .Returns(true).Returns(false).Returns(true).Returns(false).Returns(true).Returns(false)
                               .Returns(true).Returns(true).Returns(true);

            _getPolicyRepoMock.Setup(x => x.DeleteTranscation(It.IsAny<IPolicyQuoteTransaction>())).Returns(true);
            _setPolicyRepoMock.Setup(x => x.SetPolicyQuote(It.IsAny<IPolicyQuoteTransaction>())).Returns(true);

            dataMigrationUtil.LoadPolicyQuoteData(PolicyQuoteType.Policy);
            dataMigrationUtil.MigrateToDb();

            var results = dataMigrationUtil.GetResults();

            //checkk all Policy/Quote transactions migrated
            Assert.IsTrue(results.Where(policy => policy.MigrationStatus == true).Count() > 0);
        }

        [TestMethod]
        public void TestCsvFileWritterIntegration()
        {
            var csvFileWritter = new CsvFileWritter();
            var consoleProgress = new ConsoleProgress();
            var dataMigrationUtil = new DataMigrationUtility(_configMock.Object, _getPolicyRepoMock.Object, _setPolicyRepoMock.Object, _logger.Object, csvFileWritter, consoleProgress, _notification.Object);
            _configMock.Setup(x => x.GetDataPartitionLimt()).Returns(25);
            _getPolicyRepoMock.Setup(x => x.GetPolicyData(It.IsAny<string>(), It.IsAny<string>())).Returns(() =>
            {
                var dataTable = CreateDataTableAndLoadData("DataRetry.csv");
                return dataTable;
            });
            _getPolicyRepoMock.SetupSequence(x => x.GetPolicyQuote(It.IsAny<IPolicyQuoteTransaction>()))
                               .Returns(true).Returns(false).Returns(true).Returns(false).Returns(true).Returns(false)
                               .Returns(true).Returns(true).Returns(true);

            _getPolicyRepoMock.Setup(x => x.DeleteTranscation(It.IsAny<IPolicyQuoteTransaction>())).Returns(true);
            _setPolicyRepoMock.Setup(x => x.SetPolicyQuote(It.IsAny<IPolicyQuoteTransaction>())).Returns(true);

            dataMigrationUtil.LoadPolicyQuoteData(PolicyQuoteType.Policy);
            dataMigrationUtil.MigrateToDb();

            var results = dataMigrationUtil.GetResults();
            var filename = csvFileWritter.GetFileName();
            Assert.IsTrue(!string.IsNullOrEmpty(filename));
        }

        //[TestMethod]
        public void TestHappyPathForPolicy()
        {
            var config = new ConfigReader();
            var getPolicyDb = new GetPolicyDatabaseStub();
            var getPolicyRepo = new GetPolicyRepository(getPolicyDb);
            var setPolicyDb = new SetPolicyDatabaseStub();
            var setPolicyRepo = new SetPolicyRespository(setPolicyDb);
            var logger = new FileLogger();
            var resultWritter = new CsvFileWritter();
            var progress = new ConsoleProgress();
            var emailStub = new EmailNotificationStub(progress, config, resultWritter);

            var dataMigrationUtil = new DataMigrationUtility(config, getPolicyRepo, setPolicyRepo, logger, resultWritter, progress, emailStub);
            dataMigrationUtil.LoadPolicyQuoteData(PolicyQuoteType.Policy);
            dataMigrationUtil.MigrateToDb();
            var results = dataMigrationUtil.GetResults();
            Assert.AreEqual(results.Count(), results.Where(policy => policy.MigrationStatus == true).Count());
        }

        //[TestMethod]
        public void TestHappyPathForQuote()
        {
            var config = new ConfigReader();
            var getPolicyDb = new GetPolicyDatabaseStub();
            var getPolicyRepo = new GetPolicyRepository(getPolicyDb);
            var setPolicyDb = new SetPolicyDatabaseStub();
            var setPolicyRepo = new SetPolicyRespository(setPolicyDb);
            var logger = new FileLogger();
            var resultWritter = new CsvFileWritter();
            var progress = new ConsoleProgress();
            var emailStub = new EmailNotificationStub(progress, config, resultWritter);


            var dataMigrationUtil = new DataMigrationUtility(config, getPolicyRepo, setPolicyRepo, logger, resultWritter, progress, emailStub);
            dataMigrationUtil.LoadPolicyQuoteData(PolicyQuoteType.Quote);
            dataMigrationUtil.MigrateToDb();
            var results = dataMigrationUtil.GetResults();
            Assert.AreEqual(results.Count(), results.Where(policy => policy.MigrationStatus == true).Count());
        }

        //[TestMethod]
        public void TestHappyPathForBothQuoteAndPolicy()
        {
            var config = new ConfigReader();
            var getPolicyDb = new GetPolicyDatabaseStub();
            var getPolicyRepo = new GetPolicyRepository(getPolicyDb);
            var setPolicyDb = new SetPolicyDatabaseStub();
            var setPolicyRepo = new SetPolicyRespository(setPolicyDb);
            var logger = new FileLogger();
            var resultWritter = new CsvFileWritter();
            var progress = new ConsoleProgress();
            var emailStub = new EmailNotificationStub(progress, config, resultWritter);


            var dataMigrationUtil = new DataMigrationUtility(config, getPolicyRepo, setPolicyRepo, logger, resultWritter, progress, emailStub);

            List<PolicyQuoteType> runList = new List<PolicyQuoteType>() { PolicyQuoteType.Quote, PolicyQuoteType.Policy };
            runList.ForEach(runType =>
            {
                dataMigrationUtil.LoadPolicyQuoteData(runType);
                dataMigrationUtil.MigrateToDb();
                var results = dataMigrationUtil.GetResults();
                Assert.AreEqual(results.Count(), results.Where(policy => policy.MigrationStatus == true).Count());
            });

            dataMigrationUtil.SendNotification();

        }

        [TestMethod]
        public void TestTransactionExceptionScenario()
        {
            var dataMigrationUtil = new DataMigrationUtility(_configMock.Object, _getPolicyRepoMock.Object, _setPolicyRepoMock.Object, _logger.Object, _resultWritter.Object, _progress.Object, _notification.Object);
            _configMock.Setup(x => x.GetDataPartitionLimt()).Returns(25);
            _getPolicyRepoMock.Setup(x => x.GetPolicyData(It.IsAny<string>(), It.IsAny<string>())).Returns(() =>
            {
                var dataTable = CreateDataTableAndLoadData("DataRetry.csv");

                dataTable.Rows[0]["TransactionNumber"] = DBNull.Value;

                return dataTable;

            });
            _getPolicyRepoMock.Setup(x => x.GetPolicyQuote(It.IsAny<IPolicyQuoteTransaction>())).Returns(true);
            _getPolicyRepoMock.Setup(x => x.DeleteTranscation(It.IsAny<IPolicyQuoteTransaction>())).Returns(true);
            _setPolicyRepoMock.Setup(x => x.SetPolicyQuote(It.IsAny<IPolicyQuoteTransaction>())).Returns(true);

            dataMigrationUtil.LoadPolicyQuoteData(PolicyQuoteType.Policy);
            dataMigrationUtil.MigrateToDb();

            var results = dataMigrationUtil.GetResults();
            Assert.AreEqual(3, results.Where(policy => policy.MigrationStatus == true).Count());

        }


        [TestMethod]
        public void TestDbLogger()
        {
            var logger = new DbLogger();
            logger.LogException(new Exception("test exception"));
            logger.LogInfo("test log");
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void TestZip()
        {
            var folder =  Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\TestData";
            var fileName = folder + @"\Results.csv";
            var zipFileName = UtilsStub.CreateZipFile(fileName);
            Assert.IsTrue(!string.IsNullOrWhiteSpace(zipFileName));
        }

        [TestCleanup]
        public void TestCleanup()
        {

        }

        private DataTable CreateDataTableAndLoadData(string filename)
        {
            var dataTable = new DataTable();

            DataColumn instanceId = new DataColumn("InstanceId", Type.GetType("System.Guid"));
            DataColumn policyQuoteNumber = new DataColumn("PolicyQuoteNumber", Type.GetType("System.String"));
            DataColumn transactionNumber = new DataColumn("TransactionNumber", Type.GetType("System.Int64"));
            DataColumn transactionType = new DataColumn("TransactionType", Type.GetType("System.String"));
            DataColumn alternateTransactionType = new DataColumn("AlternateTransactionType", Type.GetType("System.String"));
            DataColumn quotePolicyIndicator = new DataColumn("QuotePolicyIndicator", Type.GetType("System.String"));


            dataTable.Columns.Add(instanceId);
            dataTable.Columns.Add(policyQuoteNumber);
            dataTable.Columns.Add(transactionNumber);
            dataTable.Columns.Add(transactionType);
            dataTable.Columns.Add(alternateTransactionType);
            dataTable.Columns.Add(quotePolicyIndicator);

            using (StreamReader reader = new StreamReader($"{currentFolder}/TestData/{filename}"))
            {
                reader.ReadLine(); //skip the first row
                while (!reader.EndOfStream)
                {
                    var row = reader.ReadLine();
                    var rowData = row.Split(',');
                    AddRow(dataTable, rowData[0], rowData[1], Convert.ToInt32(rowData[2]), rowData[3], rowData[4], rowData[5]);
                }
            }
            return dataTable;
        }

        private void AddRow(DataTable table, string instanceId, string policyQuoteNumber, int transactionNumber, string transactionType, string alternateTransactionType, string quotePolicyIndicator)
        {
            DataRow dr = table.NewRow();
            dr["InstanceId"] = Guid.Parse(instanceId);
            dr["PolicyQuoteNumber"] = policyQuoteNumber;
            dr["TransactionNumber"] = transactionNumber;
            dr["TransactionType"] = transactionType;
            dr["AlternateTransactionType"] = alternateTransactionType;
            dr["QuotePolicyIndicator"] = quotePolicyIndicator;

            table.Rows.Add(dr);
        }

    }
}
