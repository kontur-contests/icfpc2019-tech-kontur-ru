using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;
using lib.Models;
using lib.Solvers;
using MongoDB.Bson;
using MongoDB.Driver;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;

namespace pipeline
{
    public static class Storage
    {
        private const string dbHost = "mongodb://icfpc19-mongo1:27017";
        private const string dbName = "icfpc";
        private const string metaCollectionName = "solution_metas";
        private const string blockMetaCollectionName = "block_solution_metas";
        private const string solutionInProgressCollectionName = "solution_inprogress";
        private const string submissionSummaryCollectionName = "submission_summary";

        private static readonly MongoClient client = new MongoClient(dbHost);
        private static readonly IMongoDatabase database = client.GetDatabase(dbName);

        internal static readonly IMongoCollection<SolutionMeta> MetaCollection = database.GetCollection<SolutionMeta>(metaCollectionName);
        internal static readonly IMongoCollection<SolutionMeta> BlockMetaCollection = database.GetCollection<SolutionMeta>(blockMetaCollectionName);
        internal static readonly IMongoCollection<SolutionInProgress> SolutionInProgressCollection = database.GetCollection<SolutionInProgress>(solutionInProgressCollectionName);
        internal static readonly IMongoCollection<SubmissionSummary> SubmissionSummaryCollection = database.GetCollection<SubmissionSummary>(submissionSummaryCollectionName);

        public static List<SolutionMeta> EnumerateUnchecked()
        {
            return MetaCollection.FindSync(meta => !meta.IsOnlineChecked).ToList();
        }

        public static List<SolutionMeta> EnumerateSolved(ISolver solver)
        {
            return MetaCollection.FindSync(x => x.AlgorithmId == solver.GetName() && x.AlgorithmVersion == solver.GetVersion()).ToList();
        }

        public static List<SolutionMeta> EnumerateBestSolutions(int balance, int minDelta)
        {
            var metas = new List<SolutionMeta>();

            var tuples = EnumerateBestSolutionTuples()
                .OrderByDescending(t => t.delta / 
                                        (t.best.MoneySpent == 0 ? int.MaxValue : t.best.MoneySpent))
                .ToList();
            foreach (var tuple in tuples)
            {
                if (balance >= tuple.best.MoneySpent && tuple.delta >= minDelta)
                    metas.Add(tuple.best);
                else
                    metas.Add(tuple.@base);

                balance -= metas.Last().MoneySpent;
            }

            return metas;
        }

        private static List<(SolutionMeta @base, SolutionMeta best, int delta)> EnumerateBestSolutionTuples()
        {
            var metas = new List<(SolutionMeta @base, SolutionMeta best, int delta)>();
            var problemIds = MetaCollection.Distinct<int>("ProblemId", new BsonDocument()).ToList();

            foreach (var problemId in problemIds)
            {
                var map = ProblemReader.Read(problemId).ToState().Map;
                var mapScore = Math.Log(map.SizeX * map.SizeY, 2) * 1000;
                
                var pipeline = new[]
                {
                    new BsonDocument
                    {
                        {
                            "$match", new BsonDocument(
                                new Dictionary<string, object>
                                {
                                    {"ProblemId", problemId}
                                })
                        }
                    },
                    new BsonDocument()
                    {
                        {
                            "$group", new BsonDocument(
                                new Dictionary<string, object>
                                {
                                    {"_id", "$MoneySpent"},
                                    {
                                        "time", new BsonDocument(
                                            new Dictionary<string, string>
                                            {
                                                {"$min", "$OurTime"},
                                            })
                                    },
                                })
                        }
                    }
                };
                var minScoresForProblem = MetaCollection
                    .Aggregate<MinTimeResult>(pipeline)
                    .ToList();
                var baselineSolution = minScoresForProblem.First(s => s._id == 0);

                var estimatedSolutions = minScoresForProblem.Select(
                    s =>
                    {
                        var prevScore = (int) Math.Ceiling(mapScore * s.time / baselineSolution.time);
                        var nextScore = (int) Math.Ceiling(mapScore);

                        var nextScoreWithCost = nextScore - s._id;
                        return new {s, delta = nextScoreWithCost - prevScore};
                    })
                    .ToList();
                
                var optimalSolution = estimatedSolutions
                    .OrderByDescending(s => s.delta / (s.s._id == 0 ? int.MaxValue : s.s._id))
                    .First();

                var best = MetaCollection.FindSync(
                        y => y.ProblemId == problemId &&
                             y.OurTime == optimalSolution.s.time &&
                             y.MoneySpent == optimalSolution.s._id)
                    .First();
                var @base = MetaCollection.FindSync(
                        y => y.ProblemId == problemId &&
                             y.OurTime == baselineSolution.time &&
                             y.MoneySpent == baselineSolution._id)
                    .First();
                
                metas.Add((@base, best, optimalSolution.delta));
            }

            return metas;
        }
    }

    internal class MinTimeResult
    {
#pragma warning disable 649
        public int _id;
        public int time;
#pragma warning restore 649
    }

    public static class StorageExtensions
    {
        public static void SaveToDb(this SolutionMeta meta, bool isBlockSolution = false)
        {
            if (meta.Id == ObjectId.Empty)
            {
                meta.Id = ObjectId.GenerateNewId();
            }

            meta.SavedAt = DateTimeOffset.Now.ToUnixTimeSeconds();
            var collection = isBlockSolution ? Storage.BlockMetaCollection : Storage.MetaCollection;
            collection.ReplaceOne(x => x.Id == meta.Id, meta, new UpdateOptions {IsUpsert = true});
        }

        public static void SaveToDb(this SolutionInProgress inProgress)
        {
            inProgress.StartedAt = DateTime.Now;
            inProgress.HostName = Environment.MachineName;
            Storage.SolutionInProgressCollection.InsertOne(inProgress);
        }

        public static void SaveToDb(this SubmissionSummary submissionSummary)
        {
            submissionSummary.GeneratedByHost = Environment.MachineName;
            submissionSummary.GeneratedAt = DateTime.Now;
            Storage.SubmissionSummaryCollection.ReplaceOne(
                s => s.ProblemId == submissionSummary.ProblemId,
                submissionSummary,
                new UpdateOptions {IsUpsert = true});
        }
    }
}