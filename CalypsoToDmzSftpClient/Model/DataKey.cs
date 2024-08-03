using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalypsoToDmzSftpClient.Model
{
    class DataKey
    {
        public static string Maturity { get; set; } = "Maturity";

        public static string Fx { get; set; } = "Fx";

        public static string Holiday { get; set; } = "Holiday";
    }

    class ChecksumDataColumn
    {
        public static string FileName { get; set; } = "FileName";

        public static string FileCategory { get; set; } = "FileCategory";

        public static string Checksum { get; set; } = "Checksum";

        public static string DateUploaded { get; set; } = "DateUploaded";
    }
}
