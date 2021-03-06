﻿using gamemanager.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace gamemanager.ViewModels
{
    public class GameViewModel
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Genre { get; set; }
        public bool Owned { get; set; }
        public decimal Price { get; set; }
        public string Notes { get; set; }
        public short Rating { get; set; }
        public short Ranking { get; set; }
        public string Store { get; set; }
        public Dictionary<int, Dlc> Dlc { get; set; }
        public string StoreUrl { get; set; }

        public GameViewModel()
        {

        }

        public GameViewModel(GameEntry game)
        {
            Id = game.Id;           
            Name = game.Name;
            Owned = game.Owned;
            Price = game.Price;
            Notes = game.Notes;
            Rating = game.Rating;
            Ranking = game.Ranking;
            Store = game.Store;
            Genre = game.Genre;
        }
    }
}
