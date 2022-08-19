using MongoDB.Driver;

namespace SimpleToDoApp_AspNet
{
    public class MongoDBCruds
    {
        //public async static Main()
        //{
        //    //Create connection to MongoClient object
        //    MongoClient dbClient = new MongoClient("mongodb://localhost:27017");

        //    string databaseName = "ToDoDB";
        //    string collectionName = "ToDoItems";

        //    //Read function / get items
        //    var database = dbClient.GetDatabase(databaseName);
        //    var collection = database.GetCollection<ToDoModel>(collectionName);
        //    var results = await collection.FindAsync(_ => true);

        //    //this is the add new to do part / create function
        //    var newToDo = new ToDoModel { ToDoTask = "Learn MongoDB" };
        //    await collection.InsertOneAsync(newToDo);

        //    //How to target the ToDoTasks for the get list
        //    foreach (var result in results.ToList())
        //    {
        //        //Console.WriteLine(result.ToDoTask);
        //        Console.WriteLine($"{result.Id} : {result.ToDoTask}");
        //    }
        //}

    }
}
