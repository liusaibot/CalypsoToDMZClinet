using CalypsoToDmzSftpClient.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalypsoToDmzSftpClient
{
    class Program
    {
        static void Main(string[] args)
        {
            XLogger.AppendToLog("Process Started at " + DateTime.Now.ToLongTimeString());
            MainAsync(args).GetAwaiter().GetResult();
            XLogger.AppendToLog("Process Ended at " + DateTime.Now.ToLongTimeString());
        }

        static async Task MainAsync(string[] args)
        {
            SftpUtility sftpUtility = new SftpUtility();
            await sftpUtility.CopyFile();
            

        }
    }
}
