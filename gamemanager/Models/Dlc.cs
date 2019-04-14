using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace gamemanager.Models
{
    public class Dlc
    {
        public int Id { get; set; }
        public long ParentGameId { get; set; }
        public string Name { get; set; }
        public bool Owned { get; set; }
        public decimal Price { get; set; }
        public string Notes { get; set; }
        public short Rating { get; set; }
        public short Ranking { get; set; }
    }
}
