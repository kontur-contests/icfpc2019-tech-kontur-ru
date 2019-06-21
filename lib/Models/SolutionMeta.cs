using System;
using System.Collections;
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

        public string TaskId;
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