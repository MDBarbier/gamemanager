﻿using gamemanager.Models;
using gamemanager.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Threading.Tasks;

namespace gamemanager.Controllers
{
    public class DlcController : Controller
    {
        public async Task<IActionResult> Index()
        {
            List<Dlc> data;

            //This row uses setup in the Startup.cs file
            DataContext dc = HttpContext.RequestServices.GetService(typeof(DataContext)) as DataContext;

            //Check for OnlyOwned variables            
            bool ShowOwned = string.IsNullOrEmpty(HttpContext.Session.GetString("ShowOwned")) ? false : (HttpContext.Session.GetString("ShowOwned").ToLower() == "true") ? true : false;

            //Get a list of dlc data
            if (ShowOwned)
            {
                data = dc.GetAllDlc().Where(a => a.Owned == true).ToList();
                data = data.OrderBy(a => a.Name).ToList();
                ViewBag.ShowOwned = "checked";
            }
            else
            {
                data = dc.GetAllDlc().Where(a => a.Owned == false).ToList();
                data = data.OrderBy(a => a.Ranking).ToList();
                ViewBag.ShowOwned = "";
            }

            //get owned games to populate parent game list
            List<GameEntry> games = dc.GetAllGames().Where(a => a.Owned == true).ToList();
            List<DlcViewModel> dlcViewModel = new List<DlcViewModel>();


            //Update the prices of the games in the list from Steam data
            Code.SteamPriceChecker spc = new Code.SteamPriceChecker();

            foreach (var dlc in data)
            {
                //TODO add support for price checking other stores
                if (dlc.Store != "steam")
                    continue;

                //get current price from steam
                var appid = dc.GetDlcAppId(dlc.Name);
                if (appid == 0) continue;
                var price = await spc.GetPrice(appid.ToString());
                dlc.Price = price;

                //update price in db and throw error if cannot
                if (!dc.EditDlc(dlc))
                {
                    throw new Exception("There was a problem editing the game");
                }
            }

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
                    Rating = dlc.Rating,
                    Store = dlc.Store
                };

                dlcViewModel.Add(dlcvm);
            }
            
            //sort list by ranking
            dlcViewModel = dlcViewModel.OrderBy(a => a.Ranking).ToList();

            //Assign any message to the viewbag      
            string message = HttpContext.Session.GetString("Message");
            ViewBag.Message = message;

            //Clear message out so it's not shown multiple times
            HttpContext.Session.Remove("Message");            

            //Pass list to the view as Model
            return View(dlcViewModel);
        }

        [HttpGet]
        public IActionResult RankChange(int id, bool increase)
        {
            //This row uses setup in the Startup.cs file
            DataContext dc = HttpContext.RequestServices.GetService(typeof(DataContext)) as DataContext;

            var dlc = dc.GetDlc(id);
            bool outcomeOfEdit = false;
            bool outcomeOfOtherEdit = false;

            if (dlc != null)
            {
                if (increase)
                {
                    dlc.Ranking--;

                    //Get the game that is already in the new position, and move it the opposite way
                    var otherDlc = dc.GetDlcByRanking(dlc.Ranking);

                    if (otherDlc != null)
                    {
                        otherDlc.Ranking++;
                        outcomeOfOtherEdit = dc.EditDlc(otherDlc);

                        if (!outcomeOfOtherEdit)
                        {
                            throw new Exception("Error editing other item");
                        }
                    }
                }
                else
                {
                    dlc.Ranking++;

                    //Get the game that is already in the new position, and move it the opposite way
                    var otherDlc = dc.GetDlcByRanking(dlc.Ranking);

                    if (otherDlc != null)
                    {
                        otherDlc.Ranking--;
                        outcomeOfOtherEdit = dc.EditDlc(otherDlc);

                        if (!outcomeOfOtherEdit)
                        {
                            throw new Exception("Error editing other item");
                        }
                    }
                }

                outcomeOfEdit = dc.EditDlc(dlc, true);
            }

            if (!outcomeOfEdit) HttpContext.Session.SetString("Message", "Problem changing rank");
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Delete(int Id)
        {
            //This row uses setup in the Startup.cs file
            DataContext dc = HttpContext.RequestServices.GetService(typeof(DataContext)) as DataContext;

            bool outcomeOfDelete = dc.DeleteDlc(Id);

            if (outcomeOfDelete) HttpContext.Session.SetString("Message", "Dlc deleted");

            return RedirectToAction("Index");
        }

        public IActionResult AddDlc(int Id)
        {
            DlcViewModel dlc = new DlcViewModel() { ParentGameId = Id };
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

        public IActionResult OnlyOwned(bool status)
        {
            HttpContext.Session.SetString("ShowOwned", status.ToString());

            return RedirectToAction("Index");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Save(DlcViewModel dlc)
        {
            //This row uses setup in the Startup.cs file
            DataContext dc = HttpContext.RequestServices.GetService(typeof(DataContext)) as DataContext;

            bool outcomeOfSave = dc.EditDlc((Dlc)dlc, true);

            if (outcomeOfSave) HttpContext.Session.SetString("Message", "Record Saved");

            return RedirectToAction("Index");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult SaveNewDlc(DlcViewModel dlc)
        {
            //This row uses setup in the Startup.cs file
            DataContext dc = HttpContext.RequestServices.GetService(typeof(DataContext)) as DataContext;

            if (ModelState.IsValid)
            {
                if (dlc.Ranking == 0)
                {
                    try
                    {
                        dlc.Ranking = short.Parse(dc.GetNextRanking(Code.ItemType.Dlc));
                    }
                    catch (System.Exception ex)
                    {
                        HttpContext.Session.SetString("Message", "Error trying to get next available rank: " + ex.Message);
                        return RedirectToAction("Index");
                    }
                }

                bool outcomeOfSave = dc.InsertDlc(dlc);

                if (outcomeOfSave)
                {
                    HttpContext.Session.SetString("Message", "Record Saved");

                    //get game id
                    var dbGame = dc.GetDlcByName(dlc.Name);

                    //add a new store data entry for this game
                    if (!dc.InsertStoreDataDlcEntry(dbGame.Id, dlc.StoreUrl))
                    {
                        HttpContext.Session.SetString("Error", "There was a problem entering store data for game");
                    }
                }

                return RedirectToAction("Index");
            }
            else
            {
                //repopulate list of games to populate the parent game dropdown list
                var games = dc.GetAllGames().Select(g => (g.Id, g.Name)).ToList();

                //assign the list of possible parent games to the games property of the vm
                foreach (var (Id, Name) in games)
                {
                    dlc.PotentialParentGames.Add(new SelectListItem(Name, Id.ToString()));
                }
                return View("New", dlc);
            }
        }

        public IActionResult Edit(int id)
        {
            //This row uses setup in the Startup.cs file
            DataContext dc = HttpContext.RequestServices.GetService(typeof(DataContext)) as DataContext;

            //Get the game that relates to the id provided
            var data = dc.GetDlc(id);

            DlcViewModel dlcvm = new DlcViewModel(data);

            var storeData = dc.GetDlcStoreData(id);

            if (storeData == null)
            {
                HttpContext.Session.SetString("Error", "There was a problem entering store data for game");
                return RedirectToAction("Index");
            }

            dlcvm.StoreUrl = storeData.StoreUrl;
            dlcvm.Store = storeData.StoreName;
            
            //Pass list to the view as Model
            return View(dlcvm);
        }

        public IActionResult New()
        {
            //This row uses setup in the Startup.cs file
            DataContext dc = HttpContext.RequestServices.GetService(typeof(DataContext)) as DataContext;

            //Get list of games to populate the parent game dropdown list
            var games = dc.GetAllGames().Select(g => (g.Id, g.Name)).ToList();

            //sort by name
            games = games.OrderBy(g => g.Name).ToList();

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