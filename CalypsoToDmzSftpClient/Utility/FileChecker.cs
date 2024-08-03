using CalypsoToDmzSftpClient.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CalypsoToDmzSftpClient.Utility
{
    public class FileChecker
    {
        public static object _locked = new object();
        public static string path = ConfigurationManager.AppSettings["jsonFolder"] + "checksum.json";


        public static void createJson()
        {
            string checksumDirectory = ConfigurationManager.AppSettings["jsonFolder"];
            Directory.CreateDirectory(checksumDirectory);
            string filename = "checksum.json";
            string targetFile = Path.Combine(checksumDirectory, filename);

            ChecksumItem productItem = new ChecksumItem
            {
                dataKey = DataKey.Maturity,
                dataChecksum = "",
                filename = ""
            };

            ChecksumItem holidayItem = new ChecksumItem
            {
                dataKey = DataKey.Holiday,
                dataChecksum = "",
                filename = ""
            };

            ChecksumItem fxItem = new ChecksumItem
            {
                dataKey = DataKey.Fx,
                dataChecksum = "",
                filename = ""
            };
            List<ChecksumItem> checksumItems = new List<ChecksumItem>();
            checksumItems.Add(productItem);
            checksumItems.Add(holidayItem);
            checksumItems.Add(fxItem);

            string jsonData = JsonConvert.SerializeObject(checksumItems);

            if (!File.Exists(targetFile))
            {
                using (StreamWriter sw = File.CreateText(targetFile))
                {
                    sw.WriteLine(jsonData);
                }
            }
            //        using (StreamWriter writer = new StreamWriter(path, true))
            //{
            //    writer.WriteLine(jsonData);
            //    writer.Close();
            //}
            //UpdateJson(jsonData);


        }

        public static void UpdateJson(string text)
        {
            Directory.CreateDirectory(ConfigurationManager.AppSettings["jsonFolder"]);
            //lock (_locked)
            //{
            using (StreamWriter writer = new StreamWriter(path, true))
            {
                writer.WriteLine(text);
                writer.Close();
            }

            //}
        }

        public static string LoadJson()
        {
            createJson();
            using (StreamReader reader = new StreamReader(path))
            {
                string json = reader.ReadToEnd();
                return json;
            }

        }

        public static bool IsSameFile(ChecksumItem checksumItem, ChecksumItem dbItem)
        {
            try
            {
                
               

                var dbItemJson = JsonConvert.SerializeObject(dbItem);
                var currentItemJson = JsonConvert.SerializeObject(checksumItem);

                bool sameFile = dbItemJson == currentItemJson;

                Console.WriteLine($"DB Item = {dbItemJson}");
                Console.WriteLine($"Current Item = {currentItemJson}");
                if (sameFile)
                {

                    Console.WriteLine("Data Already Exists");
                }
                else
                {
                    Console.WriteLine("Data Doesn't Match");
                }
                
                //if (!string.IsNullOrEmpty(jsonData))
                //{
                //    List<ChecksumItem> checksumItems = JsonConvert.DeserializeObject<List<ChecksumItem>>(jsonData);
                //    ChecksumItem currentItem = checksumItems.Where(x => x.dataKey == checksumItem.dataKey).SingleOrDefault();
                //    if (currentItem != null)
                //    {
                //        sameFile = checksumItem.Equals(currentItem);
                //        return sameFile;
                //    }
                //}


                return sameFile;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return true;
            }
        }

        public static string EncryptFile(string filename)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filename))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }
    }
}
