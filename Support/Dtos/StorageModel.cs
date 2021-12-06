using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Support.Dtos
{
    public class StorageModel
    {
        public string SubFolder { get; set; }

        public Guid ObjectId { get; set; }

        public byte[] Bytes { get; set; }

        public string MediaType { get; set; }

        public string Extension { get; set; }
    }
}
