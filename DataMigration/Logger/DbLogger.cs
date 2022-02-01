using System;
using System.Diagnostics;

namespace DataMigration
{
    public class DbLogger : ILogger
    {

        static DbLogger()
        {
        }

        public void LogDebug(string message)
        {
            //no implementation intended
        }

        public void LogException(Exception exception)
        {
            //logManager.LogException($"Message: {exception?.Message}, StackTrace: {exception?.StackTrace}, TransactionXml: {transaction?.GetXml()}");
        }

        public void LogInfo(string message)
        {
            //logManager.LogMessage(message, EventLogEntryType.Information, transaction?.GetXml());
        }
    }
}
