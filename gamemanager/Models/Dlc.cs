using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using gamemanager.ViewModels;

namespace gamemanager.Models
{
    public class Dlc
    {
        public int Id { get; set; }
        public long ParentGameId { get; set; }
        [Required]
        public string Name { get; set; }
        public bool Owned { get; set; }
        public decimal Price { get; set; }
        public string Notes { get; set; }
        public short Rating { get; set; }
        public short Ranking { get; set; }
        public string Store { get; set; }

        public static explicit operator Dlc(DlcViewModel v)
        {
            return new Dlc()
            {
                Id = v.Id,
                Name = v.Name,
                Notes = v.Notes,
                Owned = v.Owned,
                ParentGameId = v.ParentGameId,
                Price = v.Price,
                Ranking = v.Ranking,
                Rating = v.Rating,
                Store = v.Store
            };
        }
    }
}
