using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalypsoToDmzSftpClient.Utility
{
    public class SftpUtility
    {

        public SftpUtility() { }

        public bool CopyFile()
        {
            try
            {
                string host = ConfigHelper.getSftpHost();
                int port = ConfigHelper.getSftpPort();
                string username = ConfigHelper.getSftpUsername();
                string password = ConfigHelper.getSftpPassword();
                int countCopy = 0;

                using (var client = new SftpClient(host, port, username, password))
                {
                    client.Connect();
                    if (client.IsConnected)
                    {
                        Console.WriteLine("SFTP Client Connected");
                        XLogger.AppendToLog("SFTP Client Connected");

                        var files = Directory.GetFiles(@ConfigHelper.getSourceFilePath()).Where(s => s.EndsWith(".csv", StringComparison.OrdinalIgnoreCase));

                        if(files.Count() == 0)
                        {
                            XLogger.AppendToLog("No Files Found");
                            return false;
                        }

                        Console.WriteLine($"{files.Count()} Files Found");
                        XLogger.AppendToLog($"{files.Count()} Files Found");

                        foreach (var dFile in files)
                        {
                            var filename = Path.GetFileName(dFile);
                            MemoryStream myCSVDataInMemory = new MemoryStream(File.ReadAllBytes(dFile));
                            client.BufferSize = (uint)myCSVDataInMemory.Length;
                            client.UploadFile(myCSVDataInMemory, getFileDestination(filename));
                            Console.WriteLine($"{filename} has been uploaded to {host}");
                            XLogger.AppendToLog($"{filename} has been uploaded  to {host}");

                            archiveFile(dFile);
                        }
                    }
                    else
                    {
                        XLogger.AppendToLog("SFTP Client could not connect");
                        return false;
                    }
                }

                return true;
            }
            catch(Exception ex)
            {
                XLogger.AppendToLog(ex.ToString());
                return false;
            }
        }

        public static string getFileDestination(string filename)
        {
            string destinationFolder = ConfigHelper.getDestinationFilePath();
            string destination = destinationFolder + filename;

            return destination;
        }

        public static bool archiveFile(string file)
        {
            try
            {
                string archiveDirectory = ConfigHelper.getArchiveFolder() + DateTime.Now.ToString("yyyyMMdd");
                Directory.CreateDirectory(archiveDirectory);
                string filename = Path.GetFileName(file);
                string targetFile = Path.Combine(archiveDirectory, filename);

                if (File.Exists(targetFile)) File.Delete(targetFile);
                File.Move(file, targetFile);


                return true;
            }catch(Exception ex)
            {
                XLogger.AppendToLog(ex.ToString());
                return false;
            }
        }

    }
}
