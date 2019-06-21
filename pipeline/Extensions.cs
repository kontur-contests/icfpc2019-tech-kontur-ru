using System;
using lib.Models;
using MongoDB.Driver;

namespace pipeline
{
    public static class Extensions
    {
        private const string dbHost = "mongodb://icfpc19-mongo1:27017";
        private const string dbName = "icfpc";
        private const string metaCollectionName = "solution_metas";

        public static void SaveToDb(this SolutionMeta meta)
        {
            var client = new MongoClient(dbHost);
            var database = client.GetDatabase(dbName);
            var collection = database.GetCollection<SolutionMeta>(metaCollectionName);

            meta.SavedAt = DateTimeOffset.Now.ToUnixTimeSeconds();
            
            collection.InsertOne(meta);
        }
    }
}