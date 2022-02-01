using System;
using System.IO;
using System.Reflection;

namespace DataMigration
{
    public class FileLogger : ILogger
    {
        private static readonly string folder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\";
        private static readonly string fileName = string.Empty;
        private static readonly string filePath = string.Empty;


        static FileLogger()
        {
            fileName = $"log.txt";
            filePath = folder + fileName;
            filePath = folder + fileName;
        }

        public void LogDebug(string message)
        {
            using (StreamWriter writer = new StreamWriter(path: filePath, append: true))
            {
                writer.WriteLine(message);
            }
        }


        public void LogException(Exception exception)
        {
            using (StreamWriter writer = new StreamWriter(path: filePath, append: true))
            {
                var message = $"ERROR: Exception: {exception}, StackTrace: {exception.StackTrace}";
                writer.WriteLine(message);
            }
        }

        public void LogInfo(string message)
        {
            using (StreamWriter writer = new StreamWriter(path: filePath, append: true))
            {
                var msg = $"INFO: Message: {message}";
                writer.WriteLine(msg);
            }
        }
    }
}
