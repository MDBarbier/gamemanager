using gamemanager.ViewModels;
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


        public static explicit operator GameEntry(GameViewModel v)
        {
            return new GameEntry()
            {
                Id = v.Id,
                Name = v.Name,
                Notes = v.Notes,
                Owned = v.Owned,
                Price = v.Price,
                Ranking = v.Ranking,
                Rating = v.Rating,
                Dlc = v.Dlc,
                Genre = v.Genre
            };
        }
    }
}
