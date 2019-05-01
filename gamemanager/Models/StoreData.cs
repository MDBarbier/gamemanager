using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace gamemanager.Models
{
    public class StoreData
    {
        public long Id { get; set; }
        public string StoreUrl { get; set; }
        public string StoreName { get; set; }
        public int AppId { get; set; }
        public int ParentId { get; set; }
    }
}
