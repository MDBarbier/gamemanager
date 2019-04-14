using gamemanager.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace gamemanager.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {            
            //This row uses setup in the Startup.cs file
            DataContext dc = HttpContext.RequestServices.GetService(typeof(DataContext)) as DataContext;

            //Get a list of data
            var data = dc.GetAllGames();

            //Assign any message to the viewbag      
           string message = HttpContext.Session.GetString("Message");
            ViewBag.Message = message;

            //Clear message out so it's not shown multiple times
            HttpContext.Session.Remove("Message");

            //Pass list to the view as Model
            return View(data);

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

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult SaveNewDlc(Dlc dlc)
        {
            //This row uses setup in the Startup.cs file
            DataContext dc = HttpContext.RequestServices.GetService(typeof(DataContext)) as DataContext;

            bool outcomeOfSave = dc.InsertDlc(dlc);

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
