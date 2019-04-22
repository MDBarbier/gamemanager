﻿using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
namespace gamemanager.ViewModels
{
    public class DlcViewModel
    {
        public int Id { get; set; }
        public long ParentGameId { get; set; }
        public string ParentGameName { get; set; }
        public string Name { get; set; }
        public bool Owned { get; set; }
        public decimal Price { get; set; }
        public string Notes { get; set; }
        public short Rating { get; set; }
        public short Ranking { get; set; }
        public SelectListItem ParentGame { get; set; }
        public List<SelectListItem> PotentialParentGames { get; set; }

    }
}