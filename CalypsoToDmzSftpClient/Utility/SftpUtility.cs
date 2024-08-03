using CalypsoToDmzSftpClient.Enums;
using CalypsoToDmzSftpClient.Model;
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

        public async Task<bool> CopyFile()
        {
            try
            {
                string host = ConfigHelper.getSftpHost();
                int port = ConfigHelper.getSftpPort();
                string username = ConfigHelper.getSftpUsername();
                string password = ConfigHelper.getSftpPassword();
                //int countCopy = 0;
                //host = "localhost";
                //username = "sheriff";
                //password = "Shinratensei!1";

                using (var client = new SftpClient(host, port, username, password))
                {
                    client.Connect();
                    if (client.IsConnected)
                    {
                        Console.WriteLine("SFTP Client Connected");
                        XLogger.AppendToLog("SFTP Client Connected");

                        var files = Directory.GetFiles(@ConfigHelper.getSourceFilePath()).Where(s => s.EndsWith(".csv", StringComparison.OrdinalIgnoreCase));

                        if (files.Count() == 0)
                        {
                            XLogger.AppendToLog("No Files Found");
                            return false;
                        }

                        Console.WriteLine($"{files.Count()} Files Found");
                        XLogger.AppendToLog($"{files.Count()} Files Found");

                        foreach (var dFile in files)
                        {
                            var filename = Path.GetFileName(dFile);

                            XLogger.AppendToLog("Current File" + filename);
                            Console.WriteLine("Current File" + filename);


                            FileType fileType = GetFileType(filename);
                            string fileCategory = GetFileCategory(fileType);
                            string jsonKey = GetJsonKey(fileType);
                            string checksum = FileChecker.EncryptFile(dFile);
                            string dateUploaded = DateTime.Now.ToString("dd-MM-yyyy");

                            ChecksumData checksumData = new ChecksumData()
                            {
                                Checksum = checksum,
                                FileName = filename,
                                FileCategory = fileCategory,
                                DateUploaded = dateUploaded
                            };

                            SqliteClient sqliteClient = new SqliteClient();
                            var currentDataChecksum = await sqliteClient.SelectDataByKey(checksumData.FileName, ChecksumDataColumn.FileName);


                            if (currentDataChecksum == null)
                            {
                                await sqliteClient.CreatetData(checksumData);
                                MemoryStream myCSVDataInMemory = new MemoryStream(File.ReadAllBytes(dFile));
                                //client.BufferSize = (uint)myCSVDataInMemory.Length;
                                client.UploadFile(myCSVDataInMemory, getFileDestination(filename));
                                Console.WriteLine($"{filename} has been uploaded to {host}");
                                XLogger.AppendToLog($"{filename} has been uploaded  to {host}");
                                archiveFile(dFile);
                            }
                            else
                            {
                                bool isSameFile = currentDataChecksum.Checksum == checksumData.Checksum;

                                if (isSameFile == true)
                                {
                                    Console.WriteLine($"{filename} already exists on {host}");
                                    XLogger.AppendToLog($"{filename} already exists on {host}");

                                    archiveFile(dFile);
                                }
                                else
                                {
                                    MemoryStream myCSVDataInMemory = new MemoryStream(File.ReadAllBytes(dFile));
                                    client.BufferSize = (uint)myCSVDataInMemory.Length;
                                    client.UploadFile(myCSVDataInMemory, getFileDestination(filename));
                                    Console.WriteLine($"{filename} has been uploaded to {host}");
                                    XLogger.AppendToLog($"{filename} has been uploaded  to {host}");
                                    await sqliteClient.UpdateDataByKey(checksumData, ChecksumDataColumn.FileName);
                                    archiveFile(dFile);
                                }

                            }




                        }

                    }
                    else
                    {
                        Console.WriteLine($"Could Not connect to Host at {host}");
                        XLogger.AppendToLog($"Could Not connect to Host at {host}");
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                XLogger.AppendToLog(ex.ToString());
                return false;
            }
        }

        public static string GetFileCategory(FileType fileType)
        {
            string fileCategory = string.Empty;
            if (fileType == FileType.Product)
            {
                fileCategory = DataKey.Maturity;
            }
            else if (fileType == FileType.FxRate)
            {
                fileCategory = DataKey.Fx;
            }
            else if (fileType == FileType.HolidayCalendar)
            {
                fileCategory = DataKey.Holiday;
            }
            else if (fileType == FileType.NotFound)
            {
                fileCategory = string.Empty;
            }
            return fileCategory;
        }

        public static FileType GetFileType(string fileName)
        {
            string productPrefix = "Maturity_";
            string holidayPrefix = "Holiday_";
            string fxPrefix = "FX_";


            if (fileName.StartsWith(productPrefix, StringComparison.OrdinalIgnoreCase))
            {
                return FileType.Product;
            }
            else if (fileName.StartsWith(holidayPrefix, StringComparison.OrdinalIgnoreCase))
            {
                return FileType.HolidayCalendar;
            }
            else if (fileName.StartsWith(fxPrefix, StringComparison.OrdinalIgnoreCase))
            {
                return FileType.FxRate;
            }
            else
            {
                return FileType.NotFound;
            }
        }

        public static string GetJsonKey(FileType fileType)
        {
            string key = string.Empty;
            if (fileType == FileType.Product)
            {
                key = DataKey.Maturity;
            }
            else if (fileType == FileType.FxRate)
            {
                key = DataKey.Fx;
            }
            else if (fileType == FileType.HolidayCalendar)
            {
                key = DataKey.Holiday;
            }
            else if (fileType == FileType.NotFound)
            {
                key = "";
            }
            return key;
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
            }
            catch (Exception ex)
            {
                XLogger.AppendToLog(ex.ToString());
                return false;
            }
        }

    }
}
