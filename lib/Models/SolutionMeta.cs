using System;
using System.Collections;
using System.Xml.Linq;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Driver;

namespace lib.Models
{
    public class SolutionMeta
    {
        public SolutionMeta(string problemPack, int problemId, string solutionBlob, int ourTime, string algorithmId, int algorithmVersion, double calculationTookMs)
        {
            ProblemPack = problemPack;
            ProblemId = problemId;
            SolutionBlob = solutionBlob;
            OurTime = ourTime;
            AlgorithmId = algorithmId;
            AlgorithmVersion = algorithmVersion;
            CalculationTookMs = calculationTookMs;
        }

        public ObjectId Id;
        public string ProblemPack;
        public int ProblemId;
        public string SolutionBlob;
        public int OurTime;
        public bool IsOnlineChecked;
        public int? OnlineTime;
        public bool? IsOnlineCorrect;
        public string AlgorithmId;
        public int AlgorithmVersion;
        public long SavedAt;
        public double CalculationTookMs;
    }
}