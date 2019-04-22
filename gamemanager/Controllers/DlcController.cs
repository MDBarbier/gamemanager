﻿using gamemanager.Models;
using gamemanager.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace gamemanager.Controllers
{
    public class DlcController : Controller
    {
        public IActionResult Index()
        {
            //This row uses setup in the Startup.cs file
            DataContext dc = HttpContext.RequestServices.GetService(typeof(DataContext)) as DataContext;

            //Get a list of data
            List<Dlc> data = dc.GetAllDlc();
            List<GameEntry> games = dc.GetAllGames();
            List<DlcViewModel> dlcViewModel = new List<DlcViewModel>();

            //assign to list of view models and get the parent game names
            foreach (var dlc in data)
            {
                var parentGame = games.Where(a => a.Id == dlc.ParentGameId).FirstOrDefault();

                DlcViewModel dlcvm = new DlcViewModel()
                {
                    Id = dlc.Id,
                    Name = dlc.Name,
                    Notes = dlc.Notes,
                    Owned = dlc.Owned,
                    ParentGameId = dlc.ParentGameId,
                    ParentGameName = parentGame.Name,
                    Price = dlc.Price,
                    Ranking = dlc.Ranking,
                    Rating = dlc.Rating
                };

                dlcViewModel.Add(dlcvm);
            }

            //Assign any message to the viewbag      
            string message = HttpContext.Session.GetString("Message");
            ViewBag.Message = message;

            //Clear message out so it's not shown multiple times
            HttpContext.Session.Remove("Message");

            //Pass list to the view as Model
            return View(dlcViewModel);
        }

        public IActionResult AddDlc(int Id)
        {
            Dlc dlc = new Dlc() { ParentGameId = Id };
            return View(dlc);
        }

        public IActionResult ViewDlc(int Id)
        {
            //This row uses setup in the Startup.cs file
            DataContext dc = HttpContext.RequestServices.GetService(typeof(DataContext)) as DataContext;

            GameEntry g = dc.GetGame(Id);
            g.Dlc = dc.GetDlcForGame(Id);

            return View(g);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Save(Dlc dlc)
        {
            //This row uses setup in the Startup.cs file
            DataContext dc = HttpContext.RequestServices.GetService(typeof(DataContext)) as DataContext;

            bool outcomeOfSave = dc.EditDlc(dlc);

            if (outcomeOfSave) HttpContext.Session.SetString("Message", "Record Saved");

            return RedirectToAction("Index");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult SaveNewDlc(DlcViewModel dlc)
        {
            //This row uses setup in the Startup.cs file
            DataContext dc = HttpContext.RequestServices.GetService(typeof(DataContext)) as DataContext;

            bool outcomeOfSave = dc.InsertDlc(dlc);

            if (outcomeOfSave) HttpContext.Session.SetString("Message", "Record Saved");

            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            //This row uses setup in the Startup.cs file
            DataContext dc = HttpContext.RequestServices.GetService(typeof(DataContext)) as DataContext;

            //Get the game that relates to the id provided
            var data = dc.GetDlc(id);

            //Pass list to the view as Model
            return View(data);
        }

        public IActionResult New()
        {
            //This row uses setup in the Startup.cs file
            DataContext dc = HttpContext.RequestServices.GetService(typeof(DataContext)) as DataContext;

            //Get list of games to populate the parent game dropdown list
            var games = dc.GetAllGames().Select(g => (g.Id, g.Name)).ToList();

            DlcViewModel dlcvm = new DlcViewModel();
            dlcvm.PotentialParentGames = new List<SelectListItem>();

            //assign the list of possible parent games to the games property of the vm
            foreach (var (Id, Name) in games)
            {
                dlcvm.PotentialParentGames.Add(new SelectListItem(Name, Id.ToString()));
            }

            return View(dlcvm);
        }       

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}