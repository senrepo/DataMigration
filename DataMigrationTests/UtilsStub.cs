using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataMigrationTests
{
    public class UtilsStub
    {

        public static string CreateZipFile(string fileName)
        {
            string zipFileName = string.Empty;
            try
            {
                var fileParts = fileName.Split('.');
                zipFileName = fileParts[0] + ".zip";

                if (string.IsNullOrWhiteSpace(zipFileName) == false) File.Delete(zipFileName);

                using (var archive = ZipFile.Open(zipFileName, ZipArchiveMode.Create))
                {
                    archive.CreateEntryFromFile(fileName, Path.GetFileName(fileName),CompressionLevel.Optimal);
                }
            } catch (Exception ex)
            {
                zipFileName = string.Empty;
            }
            return zipFileName;
        }

    }
}
