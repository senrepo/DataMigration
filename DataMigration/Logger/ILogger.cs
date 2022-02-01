using System;

namespace DataMigration
{
    public interface ILogger
    {
        void LogException(Exception exception);
        void LogInfo(string message);
        void LogDebug(string message);
    }
}
