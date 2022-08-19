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
        //Create connection to MongoClient object
        public static MongoClient dbClient = new("mongodb://localhost:27017");

        private readonly IMongoDatabase _database;

        private IMongoCollection<ToDoModel> _collection;


        public HomeController()
        {
             _database = dbClient.GetDatabase("ToDoDB");
            _collection = _database.GetCollection<ToDoModel>("ToDoItems");
        }

        //private readonly ILogger<HomeController> _logger;

        //public HomeController(ILogger<HomeController> logger)
        //{
        //    _logger = logger;
        //}

        public async Task<IActionResult> Index()
        {

            return View(await GetToDoModels());
            
        }
        public async static Task<List<ToDoModel>> GetToDoModels()
        {
            var database = dbClient.GetDatabase("ToDoDB");
            var collection = database.GetCollection<ToDoModel>("ToDoItems");
            var results = await collection.FindAsync(_ => true);
            return results.ToList();
        }

        //Function to add a new todomodel via input
        [HttpPost]
        public async Task<IActionResult> AddNewToDo(IFormCollection form)
        {
            string addNewTaskInput = form["addNewTaskInput"].ToString();
            
            _collection = _database.GetCollection<ToDoModel>("ToDoItems");
            ToDoModel newToDo = new () { ToDoTask = addNewTaskInput};
            await _collection.InsertOneAsync(newToDo);

            return RedirectToAction("Index");
        }

        //Function to delete todomodels
        [HttpPost]
        public async Task<ActionResult<ToDoModel>> DeleteToDo(string id)
        {
            _collection = _database.GetCollection<ToDoModel>("ToDoItems");

            var builder = Builders<ToDoModel>.Filter;
            var filter = builder.Eq(x => x.Id, id);
            
            await _collection.DeleteOneAsync(filter);

            return RedirectToAction(nameof(Index));
        }
        


        public IActionResult Documentation()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
 

    }
}