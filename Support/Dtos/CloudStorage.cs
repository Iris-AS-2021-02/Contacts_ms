using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Support.Dtos
{
    public class CloudStorage
    {
        public string BaseAddress { get; set; }

        public string Bucket { get; set; }
        
        public string Folder { get; set; }
    }
}
