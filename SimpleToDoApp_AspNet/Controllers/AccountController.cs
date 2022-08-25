using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Security.Claims;
using SimpleToDoApp_AspNet.Models;
using System.Diagnostics;

namespace SimpleToDoApp_AspNet.Controllers
{
    public class AccountController : Controller
    {
        //Create connection to MongoClient object //will be added to appsettings.json later when using an actual database
        public static MongoClient dbClient = new("mongodb://localhost:27017");
        public IMongoDatabase database = dbClient.GetDatabase("AspEmailLoginTests"); //name of database where credentials are stored. 

        //the login page
        public IActionResult Login()
        {
            return View("Login");
        }
        public async Task<IActionResult> SignInButton(IFormCollection form)
        {
            //null checker
            if (String.IsNullOrWhiteSpace(form["email"].ToString()) || String.IsNullOrWhiteSpace(form["password"].ToString()))
            {
                ViewData["Message"] = "Please fill both fields";
                return View("Login");
            }
            string email = form["email"].ToString();
            string inputpassword = form["password"].ToString();
            //checking if email exists in the system
            if (DoesCollectionExist(email) != true)
            {
                ViewData["Message"] = "Email does not exist.";
                return View("Login");
            }

            var collection = database.GetCollection<User>(email);
            //how to target password
            var documents = await collection.FindAsync(_ => true);
            var password = String.Empty;
            var firstname = String.Empty;
            foreach (var document in documents.ToList())
            {
                password = $"{document.Password}";
                firstname = $"{document.FirstName}";
            }
            //password checker
            if (password == inputpassword)
            {
                //if all worked then login and authenticate 

                //creating security context using System.Security.Claims and ClaimsPrincipal 
                var claims = new List<Claim>
                {
                    new Claim("name", firstname),
                    new Claim("email", email),
                };
                var identity = new ClaimsIdentity(claims, "MyCookieAuth");
                ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(identity); //security context is created incide principal

                await HttpContext.SignInAsync("MyCookieAuth", claimsPrincipal); //serealize the claimsprinciple and encrypt it and then save it in cookie as HttpContext

                return RedirectToAction("Index", "Home"); //if it works, send user to the Index action of the Home controller
            }
            //else return Login and give message password incorrect
            ViewData["Message"] = "Password is incorrect";
            return View("Login");
        }
        public IActionResult Signup()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> NewSignUp(IFormCollection form)
        {
            if (String.IsNullOrWhiteSpace(form["email"].ToString()) || String.IsNullOrWhiteSpace(form["firstname"].ToString()) || String.IsNullOrWhiteSpace(form["password"].ToString()))
            {
                ViewData["Message"] = "One or more required fields are empty.";
                return View("Signup");
            }
            if (form["password"] != form["confirmpassword"])
            {
                ViewData["Message"] = "Passwords do not match, please try again.";
                return View("Signup");
            }

            var newUserEmail = form["email"].ToString();
            var collection = database.GetCollection<User>(newUserEmail);

            if (DoesCollectionExist(newUserEmail) == false)
            {
                User newUser = new User
                {
                    Email = form["email"].ToString(),
                    FirstName = form["firstname"].ToString(),
                    Password = (form["password"].ToString())
                };
                await collection.InsertOneAsync(newUser);
                ViewData["Message"] = "Successfully registered - you may now login.";
                return View("Login");
            }

            ViewData["Message"] = "Email already exists.";
            return View("Signup");

        }
        //lists the collections in the database, since each collection name is an email, this is used for validation to check if a user already exists or not
        public List<string> GetCollections()
        {
            List<string> collections = new List<string>();

            foreach (BsonDocument collection in database.ListCollectionsAsync().Result.ToListAsync<BsonDocument>().Result)
            {
                string name = collection["name"].AsString;
                collections.Add(name);
            }
            return collections;
        }
        //function to see if collection (email address) exists. if it exists that means a user has already signed up with that email.
        public bool DoesCollectionExist(string collectionName)
        {

            List<string> collections = GetCollections();

            if (collections.Contains(collectionName))
            {
                return true;
            }

            return false;
        }
        public async Task<IActionResult> LogoutButton()
        {
            await HttpContext.SignOutAsync("MyCookieAuth"); //removes cookie from browser
            return RedirectToAction("Login");
        }
        public IActionResult ForgotCredentials()
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
