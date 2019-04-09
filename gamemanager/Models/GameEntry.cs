using System.Collections.Generic;   

namespace gamemanager.Models
{
    public class GameEntry
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Genre { get; set; }
        public bool Owned { get; set; }
        public decimal Price { get; set; }
        public Dictionary<int, Dlc> Dlc { get; set; }
    }

    public class Dlc
    {
        public int Id { get; set; }
        public int ParentGameId { get; set; }
        public string Name { get; set; }
        public bool Owned { get; set; }
        public float Price { get; set; }
    }
}
