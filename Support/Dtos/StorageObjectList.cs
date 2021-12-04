using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Support.Dtos
{
    public class StorageObjectList
    {
        public string Kind { get; set; }

        public List<StorageObject> Items { get; set; }
    }
}
