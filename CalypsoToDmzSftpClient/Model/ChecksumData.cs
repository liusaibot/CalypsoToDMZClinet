using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalypsoToDmzSftpClient.Model
{
    public class ChecksumData
    {
        public int Id { get; set; }

        public string FileName { get; set; }

        public string FileCategory { get; set; }

        public string Checksum { get; set; }

        public string DateUploaded { get; set; }
    }
}
