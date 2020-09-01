using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalypsoToDmzSftpClient.Utility
{
    public class ConfigHelper
    {
        public static string getSftpHost()
        {
            string sftpHost = "";
            if (ConfigurationManager.AppSettings["sftpHost"] == null)
            {
                XLogger.AppendToLog("SFTP HOST CONFIG NOT AVAILABLE");
            }
            else
            {
                sftpHost = ConfigurationManager.AppSettings["sftpHost"].ToString();
            }


            return sftpHost;
        }

        public static int getSftpPort()
        {
            int sftpPort = 21;
            if (ConfigurationManager.AppSettings["sftpPort"] == null)
            {
                XLogger.AppendToLog("SFTP PORT CONFIG NOT AVAILABLE");
            }
            else
            {
                sftpPort = int.Parse(ConfigurationManager.AppSettings["sftpPort"].ToString());
            }


            return sftpPort;
        }

        public static string getSftpUsername()
        {
            string host = "";
            if (ConfigurationManager.AppSettings["sftpUsername"] == null)
            {
                XLogger.AppendToLog("SFTP HOST PATH NOT AVAILABLE");
            }
            else
            {
                host = ConfigurationManager.AppSettings["sftpUsername"].ToString();
            }


            return host;
        }

        public static string getSftpPassword()
        {
            string sftpPassword = "";
            if (ConfigurationManager.AppSettings["sftpPassword"] == null)
            {
                XLogger.AppendToLog("SFTP PASSWORD CONFIG NOT AVAILABLE");
            }
            else
            {
                //sftpPassword = ConfigurationManager.AppSettings["sftpPassword"].ToString();
                sftpPassword = MiServeAPI.Utilities.StringExtensions.GetDecryptedValue("sftpPassword");
            }


            return sftpPassword;
        }

        public static string getSourceFilePath()
        {
            string sourceFilePath = "";
            if (ConfigurationManager.AppSettings["sourceFilePath"] == null)
            {
                XLogger.AppendToLog("SOURCE FILE PATH NOT AVAILABLE");
            }
            else
            {
                sourceFilePath = ConfigurationManager.AppSettings["sourceFilePath"].ToString();
            }


            return sourceFilePath;
        }

        public static string getDestinationFilePath()
        {
            string destinationFilePath = "";
            if (ConfigurationManager.AppSettings["destinationFilePath"] == null)
            {
                XLogger.AppendToLog("SOURCE FILE PATH NOT AVAILABLE");
            }
            else
            {
                destinationFilePath = ConfigurationManager.AppSettings["destinationFilePath"].ToString();
            }


            return destinationFilePath;
        }

        public static string getArchiveFolder()
        {
            string archiveFolder = "";
            if (ConfigurationManager.AppSettings["archiveFolder"] == null)
            {
                XLogger.AppendToLog("SOURCE FILE PATH NOT AVAILABLE");
            }
            else
            {
                archiveFolder = ConfigurationManager.AppSettings["archiveFolder"].ToString();
            }


            return archiveFolder;
        }

    }
}
