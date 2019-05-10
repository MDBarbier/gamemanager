using gamemanager.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace gamemanager.ViewModels
{
    public class DlcViewModel
    {
        public int Id { get; set; }
        public long ParentGameId { get; set; }
        public string ParentGameName { get; set; }
        [Required]
        public string Name { get; set; }
        public bool Owned { get; set; }
        public decimal Price { get; set; }
        public string Notes { get; set; }
        public short Rating { get; set; }
        public short Ranking { get; set; }
        public string Store { get; set; }
        public SelectListItem ParentGame { get; set; }
        public List<SelectListItem> PotentialParentGames { get; set; } = new List<SelectListItem>();
        public string StoreUrl { get; set; }

        public DlcViewModel()
        {

        }

        public DlcViewModel(Dlc dlc)
        {
            Id = dlc.Id;
            ParentGameId = dlc.ParentGameId;
            Name = dlc.Name;
            Owned = dlc.Owned;
            Price = dlc.Price;
            Notes = dlc.Notes;
            Rating = dlc.Rating;
            Ranking = dlc.Ranking;
            Store = dlc.Store;            
        }

    }
}
