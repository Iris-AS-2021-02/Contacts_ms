using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Support.Dtos
{
    public class StorageModel
    {
        public int SubFolder { get; set; }

        public int ObjectId { get; set; }

        public byte[] Bytes { get; set; }

        public string MediaType { get; set; }

        public string Extension { get; set; }
    }
}
