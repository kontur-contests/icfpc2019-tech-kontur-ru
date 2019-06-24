using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using lib.Models;
using lib.Models.Actions;
using lib.Solvers;
using lib.Solvers.Postprocess;
using lib.Solvers.RandomWalk;
using MongoDB.Bson;
using MongoDB.Driver;
using MoreLinq;
using NUnit.Framework;
using tests.Solvers;

namespace tests
{
    [TestFixture]
    public class EmulatorTests : SolverTestsBase
    {
        [TestCase("WSADZEQB(10,20)B(-10,-20)FL", "FL")]
        public void ParseSolved(string sol, string buy)
        {
            var solved = Emulator.ParseSolved(sol, buy);
            solved.FormatSolution().Should().Be(sol);
            solved.FormatBuy().Should().Be(buy);
        }

        [Test]
        public void Emulate()
        {
            var solver = new DeepWalkSolver(2, new Estimator(true, true, collectDrill: true), usePalka: true, useWheels: true, useDrill: true);

            var state = ReadFromFile(2);
            var result = solver.Solve(state);

            var readFromFile = ReadFromFile(2);
            Emulator.Emulate(readFromFile, result);

            readFromFile.History.Should().BeEquivalentTo(state.History);
        }

        [Test]
        public void METHOD()
        {
            var list = Storage.GetSingleMeta(236).Select(
                    solutionMeta =>
                    {
                        if (!string.IsNullOrEmpty(solutionMeta.BuyBlob))
                            return null;
                        var solved = Emulator.ParseSolved(solutionMeta.SolutionBlob, solutionMeta.BuyBlob);
                        if (solved.Actions.Any(aa => aa.Any(a => a is UseDrill || a is UseFastWheels)))
                            return null;

                        return new {solutionMeta, solved};
                    })
                .Where(x => x != null)
                .ToList();

            var selected = list.OrderBy(x => x.solutionMeta.OurTime).DistinctBy(x => x.solutionMeta.OurTime).Take(10).ToList();

            foreach (var sss in selected.Where(x => x.solutionMeta.OurTime == 1629))
            {
                Save(sss.solved, sss.solutionMeta.ProblemId, "-original" + sss.solved.CalculateTime());
                
                var state = ProblemReader.Read(sss.solutionMeta.ProblemId).ToState();
                Emulator.Emulate(state, sss.solved);
                var postprocessor = new PostprocessorSimple(state, sss.solved);
                postprocessor.TransferSmall();

                var buildSolved = state.History.BuildSolved();
                Console.Out.WriteLine($"current: {sss.solved.CalculateTime()}, processed: {buildSolved.CalculateTime()}");
                Save(buildSolved, sss.solutionMeta.ProblemId, "-fixed" + buildSolved.CalculateTime());
                
                state = ProblemReader.Read(sss.solutionMeta.ProblemId).ToState();
                Emulator.Emulate(state, buildSolved);
                if (state.UnwrappedLeft > 0)
                    throw new InvalidOperationException("Bad mother fucker!");
            }
        }

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

            public static List<SolutionMeta> GetSingleMeta(int problemId)
            {
                return MetaCollection.FindSync(meta => meta.ProblemId == problemId).ToList();
            }
        }
    }
}