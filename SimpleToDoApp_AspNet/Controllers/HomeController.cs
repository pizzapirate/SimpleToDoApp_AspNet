using Microsoft.AspNetCore.Mvc;
using SimpleToDoApp_AspNet.Models;
using System.Diagnostics;
using SimpleToDoApp_AspNet;
using MongoDB.Driver;
using Microsoft.Extensions.Options;
using MongoDB.Bson;


namespace SimpleToDoApp_AspNet.Controllers
{

    public class HomeController : Controller
    {
        //Create connection to MongoClient object //will be added to appsettings.json later when using an actual database
        public static MongoClient dbClient = new("mongodb+srv://admin:dkJwnJrnTk2jIdx2@simpletodoappusers.erwsunc.mongodb.net/?retryWrites=true&w=majority");

        public async Task<IActionResult> Index()
        {
            try
            {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                bool isAuthenticated = HttpContext.User.Identity.IsAuthenticated; //Check if user is authenticated via the cookies
                var name = HttpContext.User.Claims.First(c => c.Type == "name").Value; //store the name of the user in a variable, extracted from claims of cookie

                if (isAuthenticated == true)
                {
                    ViewData["Name"] = name;
                    //if user is authenticated, then we want to return the view and get the to do models. 
                    return View(await GetToDoModels());
                }
            }
            catch
            {
                //if the user is not authenticated, send them to the login page
                return RedirectToAction("Login", "Account"); 
            }
            return RedirectToAction("Login", "Account");

        }
        public async Task<List<ToDoModel>> GetToDoModels()
        {
            var email = HttpContext.User.Claims.First(c => c.Type == "email").Value;

            var database = dbClient.GetDatabase("SimpleToDoAppLists");
            var collection = database.GetCollection<ToDoModel>(email);
            var results = await collection.FindAsync(_ => true);
            return results.ToList();
        }

        //Function to add a new todomodel via input
        [HttpPost]
        public async Task<IActionResult> AddNewToDo(IFormCollection form)
        {
            var email = HttpContext.User.Claims.First(c => c.Type == "email").Value;

            string addNewTaskInput = form["addNewTaskInput"].ToString();
            var database = dbClient.GetDatabase("SimpleToDoAppLists");
            var collection = database.GetCollection<ToDoModel>(email);
            ToDoModel newToDo = new () { ToDoTask = addNewTaskInput};
            await collection.InsertOneAsync(newToDo);

            return RedirectToAction("Index");
        }

        //Function to delete todomodels
        [HttpPost]
        public async Task<ActionResult<ToDoModel>> DeleteToDo(string id)
        {
            var email = HttpContext.User.Claims.First(c => c.Type == "email").Value;

            var database = dbClient.GetDatabase("SimpleToDoAppLists");
            var collection = database.GetCollection<ToDoModel>(email);

            var builder = Builders<ToDoModel>.Filter;
            var filter = builder.Eq(x => x.Id, id);
            
            await collection.DeleteOneAsync(filter);

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Documentation()
        {
            try
            {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                bool isAuthenticated = HttpContext.User.Identity.IsAuthenticated;
                if (isAuthenticated == true)
                {
                    ViewData["isAuthenticated"] = true;
                    return View();
                }
            }
            catch
            {
                ViewData["isAuthenticated"] = false;
                return View();
            }
            ViewData["isAuthenticated"] = false;
            return View();
        }
        public IActionResult Privacy()
        {
            try
            {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                bool isAuthenticated = HttpContext.User.Identity.IsAuthenticated;
                if (isAuthenticated == true)
                {
                    ViewData["isAuthenticated"] = true;
                    return View();
                }
            }
            catch
            {
                ViewData["isAuthenticated"] = false;
                return View();
            }
            ViewData["isAuthenticated"] = false;
            return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
 

    }
}