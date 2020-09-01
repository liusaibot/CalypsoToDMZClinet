using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalypsoToDmzSftpClient.Utility
{
    class XLogger
    {
        public static object _locked = new object();
        public static string path = ConfigurationManager.AppSettings["logFolder"] + DateTime.Now.ToString("yyyyMMdd") + ".txt";

        public static void AppendToLog(string text)
        {
            Directory.CreateDirectory(ConfigurationManager.AppSettings["logFolder"]);
            lock (_locked)
            {
                using (StreamWriter writer = new StreamWriter(path, true))
                {
                    writer.WriteLine(text);
                    writer.Close();
                }

            }
        }
    }
}
