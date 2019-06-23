using System;
using System.Collections;
using System.Xml.Linq;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Driver;

namespace lib.Models
{
    [BsonIgnoreExtraElements]
    public class SolutionMeta
    {
        public SolutionMeta(int problemId, string solutionBlob, int ourTime, string algorithmId, int algorithmVersion, double calculationTookMs, string buyBlob = null, int moneySpent = 0)
        {
            ProblemId = problemId;
            SolutionBlob = solutionBlob;
            BuyBlob = buyBlob;
            OurTime = ourTime;
            MoneySpent = moneySpent;
            AlgorithmId = algorithmId;
            AlgorithmVersion = algorithmVersion;
            CalculationTookMs = calculationTookMs;
        }

        public ObjectId Id;
        public int ProblemId;
        public string SolutionBlob;
        public string BuyBlob;
        public int OurTime;
        public int MoneySpent;
        public bool IsOnlineChecked;
        public int? OnlineTime;
        public bool? IsOnlineCorrect;
        public string AlgorithmId;
        public int AlgorithmVersion;
        public long SavedAt;
        public double CalculationTookMs;
    }
}