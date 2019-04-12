using gamemanager.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace gamemanager.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {

#if RELEASE
            //This row uses setup in the Startup.cs file
            DataContext dc = HttpContext.RequestServices.GetService(typeof(DataContext)) as DataContext;

            //Get a list of data
            var data = dc.GetAllGames();

            //Pass list to the view as Model
            return View(data);
#endif

#if DEBUG
            var data = new List<GameEntry>() {
                new GameEntry() { Id = 1, Name = "Skyrim", Genre = "RPG", Owned = true, Price = 9.99M},
                new GameEntry() { Id = 2, Name = "Wargroove", Genre = "Strategy", Owned = true, Price = 12.99M},
                new GameEntry() { Id = 3, Name = "MTGA", Genre = "CCG", Owned = true, Price = 0.00M},
            };

            return View(data);
#endif
        }     
        
        public IActionResult Edit(int id)
        {
            //Get the game that relates to the id provided
#if RELEASE
            //This row uses setup in the Startup.cs file
            DataContext dc = HttpContext.RequestServices.GetService(typeof(DataContext)) as DataContext;

            //Get a list of data
            var data = dc.GetGame(id);

            //Pass list to the view as Model
            return View(data);
#endif

#if DEBUG
            switch (id)
            {
                case 1:
                    return View(new GameEntry() { Id = 1, Name = "Skyrim", Genre = "RPG", Owned = true, Price = 9.99M });
                    
                case 2:
                    return View(new GameEntry() { Id = 2, Name = "Wargroove", Genre = "Strategy", Owned = true, Price = 12.99M });
                    
                case 3:
                    return View(new GameEntry() { Id = 3, Name = "MTGA", Genre = "CCG", Owned = true, Price = 0.00M });

                default:
                    throw new Exception("Unrecoginised Id");
            }            
#endif

            //Load the Edit view passing in that game object
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
