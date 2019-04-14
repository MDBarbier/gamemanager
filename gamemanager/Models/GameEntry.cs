using System.Collections.Generic;   

namespace gamemanager.Models
{
    public class GameEntry
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Genre { get; set; }
        public bool Owned { get; set; }
        public decimal Price { get; set; }
        public string Notes { get; set; }
        public short Rating { get; set; }
        public short Ranking { get; set; }
        public Dictionary<int, Dlc> Dlc { get; set; }
    }

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
