using gamemanager.Models;
using gamemanager.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;

namespace gamemanager.Controllers
{
    public class DlcController : Controller
    {
        public IActionResult Index()
        {
            List<Dlc> data;

            //This row uses setup in the Startup.cs file
            DataContext dc = HttpContext.RequestServices.GetService(typeof(DataContext)) as DataContext;

            //Check for OnlyOwned variables            
            bool ShowOwned = string.IsNullOrEmpty(HttpContext.Session.GetString("ShowOwned")) ? false : (HttpContext.Session.GetString("ShowOwned").ToLower() == "true") ? true : false;

            //Get a list of data
            if (ShowOwned)
            {
                data = dc.GetAllDlc();
                ViewBag.ShowOwned = "checked";
            }
            else
            {
                data = dc.GetAllDlc().Where(a => a.Owned == false).ToList();
                ViewBag.ShowOwned = "";
            }
            
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

        public IActionResult OnlyOwned(bool status)
        {
            HttpContext.Session.SetString("ShowOwned", status.ToString());

            return RedirectToAction("Index");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Save(Dlc dlc)
        {
            //This row uses setup in the Startup.cs file
            DataContext dc = HttpContext.RequestServices.GetService(typeof(DataContext)) as DataContext;

            bool outcomeOfSave = dc.EditDlc(dlc, true);

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
                if (outcomeOfSave) HttpContext.Session.SetString("Message", "Dlc Saved");
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

            //Pass list to the view as Model
            return View(data);
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