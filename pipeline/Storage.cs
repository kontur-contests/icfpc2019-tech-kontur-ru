﻿using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
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

        private static readonly MongoClient client = new MongoClient(dbHost);
        private static readonly IMongoDatabase database = client.GetDatabase(dbName);
        
        internal static readonly IMongoCollection<SolutionMeta> MetaCollection = database.GetCollection<SolutionMeta>(metaCollectionName);
        internal static readonly IMongoCollection<SolutionMeta> BlockMetaCollection = database.GetCollection<SolutionMeta>(blockMetaCollectionName);
        internal static readonly IMongoCollection<SolutionInProgress> SolutionInProgressCollection = database.GetCollection<SolutionInProgress>(solutionInProgressCollectionName);
        
        public static List<SolutionMeta> EnumerateUnchecked()
        {
            return MetaCollection.FindSync(meta => !meta.IsOnlineChecked).ToList();
        }
        
        public static List<SolutionMeta> EnumerateSolved(ISolver solver)
        {
            return MetaCollection.FindSync(x => x.AlgorithmId == solver.GetName() && x.AlgorithmVersion == solver.GetVersion()).ToList();            
        }
        
        public static List<SolutionMeta> EnumerateCheckedAndCorrect()
        {
            var metas = new List<SolutionMeta>();
            
            var pipeline = new[] 
            {
                // new BsonDocument { { "$match", new BsonDocument(new Dictionary<string, object>
                // {
                //     { "IsOnlineChecked", true },
                //     { "IsOnlineCorrect", true }
                // }) } },
                new BsonDocument { { "$group", new BsonDocument(new Dictionary<string, object>
                {
                    { "_id", "$ProblemId" },
                    { "time", new BsonDocument(new Dictionary<string, string>
                        {
                            { "$min", "$OurTime" },
                        })
                    },
                }) } }
            };
            var minScores = MetaCollection.Aggregate<MinTimeResult>(pipeline).ToList();
            
            minScores.ForEach(x =>
            {
                metas.Add(MetaCollection.FindSync(y => y.ProblemId == x._id && y.OurTime == x.time).First());
            });

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
            collection.ReplaceOne(x => x.Id == meta.Id, meta, new UpdateOptions { IsUpsert = true});
        }

        public static void SaveToDb(this SolutionInProgress inProgress)
        {
            inProgress.StartedAt = DateTime.Now;
            inProgress.HostName = Environment.MachineName;
            Storage.SolutionInProgressCollection.InsertOne(inProgress);
        }
    }
}
