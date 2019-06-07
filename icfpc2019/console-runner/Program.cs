using System;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace console_runner
{
    class Program
    {
        public static async Task Main()
        {
            var client = new MongoClient("mongodb://icfpc19-mongo1:27017");
            var database = client.GetDatabase("icfpc");
            var collection = database.GetCollection<BsonDocument>("tmp");
            
            var document = new BsonDocument
            {
                { "name", "SomeDocument" },
            };
            await collection.InsertOneAsync(document);
            
            await collection.Find(new BsonDocument()).ForEachAsync(d => Console.WriteLine(d));

            await database.DropCollectionAsync("tmp");
        }
    }
}