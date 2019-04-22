using gamemanager.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace gamemanager.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            //This row uses setup in the Startup.cs file
            DataContext dc = HttpContext.RequestServices.GetService(typeof(DataContext)) as DataContext;

            //Check for OnlyOwned variables            
            bool ShowOwned = string.IsNullOrEmpty(HttpContext.Session.GetString("ShowOwned")) ? false : (HttpContext.Session.GetString("ShowOwned").ToLower() == "true") ? true : false;

            //Get a list of data 
            List<GameEntry> data = new List<GameEntry>();
            if (ShowOwned)
            {
                data = dc.GetAllGames();
                ViewBag.ShowOwned = "checked";
            }
            else
            {
                data =  dc.GetAllGames().Where(a => a.Owned == false).ToList();
                ViewBag.ShowOwned = "";
            }

            //Assign any message to the viewbag      
            string message = HttpContext.Session.GetString("Message");
            ViewBag.Message = message;

            //Clear message out so it's not shown multiple times
            HttpContext.Session.Remove("Message");

            //sort list by ranking
            data = data.OrderBy(a => a.Ranking).ToList();

            //Pass list to the view as Model
            return View(data);

        }

        public IActionResult OnlyOwned(bool status)
        {
            HttpContext.Session.SetString("ShowOwned", status.ToString());            
            
            return RedirectToAction("Index");
        }

        public IActionResult New()
        {
            return View();
        }
        
        public IActionResult Edit(int id)
        {
            //This row uses setup in the Startup.cs file
            DataContext dc = HttpContext.RequestServices.GetService(typeof(DataContext)) as DataContext;

            //Get the game that relates to the id provided
            var data = dc.GetGame(id);

            //Pass list to the view as Model
            return View(data);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Save(GameEntry game)
        {
            //This row uses setup in the Startup.cs file
            DataContext dc = HttpContext.RequestServices.GetService(typeof(DataContext)) as DataContext;

            bool outcomeOfSave = dc.EditGame(game);
            
            if (outcomeOfSave) HttpContext.Session.SetString("Message", "Record Saved");            

            return RedirectToAction("Index");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult SaveNew(GameEntry game)
        {
            //This row uses setup in the Startup.cs file
            DataContext dc = HttpContext.RequestServices.GetService(typeof(DataContext)) as DataContext;

            bool outcomeOfSave = dc.InsertGame(game);

            if (outcomeOfSave) HttpContext.Session.SetString("Message", "Record Saved");

            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
