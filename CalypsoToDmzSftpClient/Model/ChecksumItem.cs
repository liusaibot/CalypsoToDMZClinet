using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalypsoToDmzSftpClient.Model
{
    public class ChecksumItem
    {
        //public int ID { get; set; }

        public string dataKey { get; set; }

        public string dataChecksum { get; set; }

        public string filename { get; set; }
    }
}
