using System.Collections.Generic;
using System.Linq;
using lib.Models;
using lib.Models.Actions;
using MongoDB.Driver;

namespace lib.Solvers.Postprocess
{
    public class PostprocessorSolver : ISolver
    {
        public string GetName() => "postprocess";

        public int GetVersion() => 1;

        public Solved Solve(State state2)
        {
            var list = Storage.GetSingleMeta(state2.ProblemId).Select(
                    solutionMeta =>
                    {
                        if (!string.IsNullOrEmpty(solutionMeta.BuyBlob))
                            return null;
                        var solved = Emulator.ParseSolved(solutionMeta.SolutionBlob, solutionMeta.BuyBlob);
                        if (solved.Actions.Any(aa => aa.Any(a => a is UseDrill || a is UseFastWheels || a is UseCloning)))
                            return null;

                        return new {solutionMeta, solved};
                    })
                .Where(x => x != null)
                .ToList();

            var selected = list.OrderBy(x => x.solutionMeta.OurTime).Take(10).ToList();

            var bestTime = int.MaxValue;
            Solved bestSolved = null;
            foreach (var sss in selected)
            {
                var state = ProblemReader.Read(sss.solutionMeta.ProblemId).ToState();
                Emulator.Emulate(state, sss.solved);
                var postprocessor = new Postprocessor(state, sss.solved);
                postprocessor.TransferSmall();

                var buildSolved = state.History.BuildSolved();
                var time = buildSolved.CalculateTime();
                if (time < bestTime)
                {
                    bestTime = time;
                    bestSolved = buildSolved;
                }
            }

            var bestState = ProblemReader.Read(state2.ProblemId).ToState();
            Emulator.Emulate(bestState, bestSolved);
            return bestSolved;
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