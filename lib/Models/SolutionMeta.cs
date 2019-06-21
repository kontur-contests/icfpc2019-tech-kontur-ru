using System;
using System.Xml.Linq;
using MongoDB.Bson;
using MongoDB.Driver;

namespace lib.Models
{
    public class SolutionMeta
    {
        public SolutionMeta(string taskId, string solutionBlob, int ourTime, string algorithmId, int algorithmVersion, double calculationTookMs)
        {
            TaskId = taskId;
            SolutionBlob = solutionBlob;
            OurTime = ourTime;
            AlgorithmId = algorithmId;
            AlgorithmVersion = algorithmVersion;
            CalculationTookMs = calculationTookMs;
        }

        public void SaveToDb()
        {
            var client = new MongoClient("mongodb://icfpc19-mongo1:27017");
            var database = client.GetDatabase("icfpc");
            var collection = database.GetCollection<SolutionMeta>("solution_metas");

            SavedAt = DateTimeOffset.Now.ToUnixTimeSeconds();
            
            collection.InsertOne(this);
        }

        public string TaskId;
        public string SolutionBlob;
        public int OurTime;
        // public int OnlineTime;
        // public bool OnlineCorrectness;
        public string AlgorithmId;
        public int AlgorithmVersion;
        public long SavedAt;
        public double CalculationTookMs;
    }
}