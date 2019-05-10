using gamemanager.Models;
using gamemanager.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace gamemanager.Controllers
{
    public class HomeController : Controller
    {
        public async Task<IActionResult> Index()
        {
            //This row uses setup in the Startup.cs file
            DataContext dc = HttpContext.RequestServices.GetService(typeof(DataContext)) as DataContext;

            //Check for OnlyOwned variables            
            bool ShowOwned = string.IsNullOrEmpty(HttpContext.Session.GetString("ShowOwned")) ? false : (HttpContext.Session.GetString("ShowOwned").ToLower() == "true") ? true : false;

            //Get a list of data 
            List<GameEntry> data = new List<GameEntry>();
            if (ShowOwned)
            {
                data = dc.GetAllGames().Where(a => a.Owned == true).ToList();
                data = data.OrderBy(a => a.Name).ToList();
                ViewBag.ShowOwned = "checked";
            }
            else
            {
                data = dc.GetAllGames().Where(a => a.Owned == false).ToList();
                data = data.OrderBy(a => a.Ranking).ToList();
                ViewBag.ShowOwned = "";
            }

            //Assign any message to the viewbag      
            string message = HttpContext.Session.GetString("Message");
            ViewBag.Message = message;

            //Clear message out so it's not shown multiple times
            HttpContext.Session.Remove("Message");

            //Update the prices of the games in the list from Steam data
            Code.SteamPriceChecker spc = new Code.SteamPriceChecker();

            foreach (var game in data)
            {
                //TODO add support for price checking other stores
                if (game.Store != "steam")
                    continue;

                //get current price from steam
                var appid = dc.GetGameAppId(game.Name);
                if (appid == 0) continue;
                var price = await spc.GetPrice(appid.ToString());
                game.Price = price;

                //update price in db and throw error if cannot
                if (!dc.EditGame(game))
                {
                    throw new Exception("There was a problem editing the game");
                }
            }

            //Pass list to the view as Model
            return View(data);

        }

        [HttpGet]
        public IActionResult Delete(int Id)
        {
            //This row uses setup in the Startup.cs file
            DataContext dc = HttpContext.RequestServices.GetService(typeof(DataContext)) as DataContext;

            bool outcomeOfDelete = dc.DeleteGame(Id);

            if (outcomeOfDelete) HttpContext.Session.SetString("Message", "Game deleted");

            return RedirectToAction("Index");
        }

        public IActionResult OnlyOwned(bool status)
        {
            HttpContext.Session.SetString("ShowOwned", status.ToString());

            return RedirectToAction("Index");
        }

        public IActionResult New()
        {
            return View(new GameViewModel());
        }

        public IActionResult Edit(int id)
        {
            //This row uses setup in the Startup.cs file
            DataContext dc = HttpContext.RequestServices.GetService(typeof(DataContext)) as DataContext;

            //Get the game that relates to the id provided
            var data = dc.GetGame(id);

            GameViewModel vm = new GameViewModel(data);

            var storeData = dc.GetStoreData(id);

            if (storeData == null)
            {
                HttpContext.Session.SetString("Error", "There was a problem entering store data for game");
                return RedirectToAction("Index");
            }

            vm.StoreUrl = storeData.StoreUrl;
            vm.Store = storeData.StoreName;

            //Pass list to the view as Model
            return View(vm);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Save(GameViewModel game)
        {
            //This row uses setup in the Startup.cs file
            DataContext dc = HttpContext.RequestServices.GetService(typeof(DataContext)) as DataContext;

            bool outcomeOfSave = dc.EditGame((GameEntry)game, true);

            if (outcomeOfSave) HttpContext.Session.SetString("Message", "Record Saved");

            return RedirectToAction("Index");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult SaveNew(GameViewModel game)
        {
            //This row uses setup in the Startup.cs file
            DataContext dc = HttpContext.RequestServices.GetService(typeof(DataContext)) as DataContext;

            if (ModelState.IsValid)
            {
                if (game.Ranking == 0)
                {
                    try
                    {
                        game.Ranking = short.Parse(dc.GetNextRanking(Code.ItemType.Game));
                    }
                    catch (System.Exception ex)
                    {
                        HttpContext.Session.SetString("Message", "Error trying to get next available rank: " + ex.Message);
                        return RedirectToAction("Index");
                    }
                }

                bool outcomeOfSave = dc.InsertGame((GameEntry)game);

                if (outcomeOfSave)
                {
                    HttpContext.Session.SetString("Message", "Record Saved");

                    //get game id
                    var dbGame = dc.GetGameByName(game.Name);
                    
                    //add a new store data entry for this game
                    if (!dc.InsertStoreDataEntry(dbGame.Id, game.StoreUrl))
                    {
                        HttpContext.Session.SetString("Error", "There was a problem entering store data for game");
                    }
                }

                return RedirectToAction("Index");
            }
            else
            {
                return View("New", game);
            }

        }

        [HttpGet]
        public IActionResult RankChange(int id, bool increase)
        {
            //This row uses setup in the Startup.cs file
            DataContext dc = HttpContext.RequestServices.GetService(typeof(DataContext)) as DataContext;

            var game = dc.GetGame(id);
            bool outcomeOfEdit = false;
            bool outcomeOfOtherEdit = false;

            if (game != null)
            {
                if (increase)
                {
                    game.Ranking--;

                    //Get the game that is already in the new position, and move it the opposite way
                    var otherGame = dc.GetGameByRanking(game.Ranking);

                    if (otherGame != null)
                    {
                        otherGame.Ranking++;
                        outcomeOfOtherEdit = dc.EditGame(otherGame);

                        if (!outcomeOfOtherEdit)
                        {
                            throw new Exception("Error editing other item");
                        }
                    }
                }
                else
                {
                    game.Ranking++;

                    //Get the game that is already in the new position, and move it the opposite way
                    var otherGame = dc.GetGameByRanking(game.Ranking);

                    if (otherGame != null)
                    {
                        otherGame.Ranking--;
                        outcomeOfOtherEdit = dc.EditGame(otherGame);

                        if (!outcomeOfOtherEdit)
                        {
                            throw new Exception("Error editing other item");
                        }
                    }
                }

                outcomeOfEdit = dc.EditGame(game, true);
            }

            if (!outcomeOfEdit) HttpContext.Session.SetString("Message", "Problem changing rank");
            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
