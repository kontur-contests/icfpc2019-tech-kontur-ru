using System;
using lib.Models;
using lib.Solvers.RandomWalk;
using NUnit.Framework;

namespace tests.Solvers
{
    [TestFixture]
    internal class ParallelDeepWalkSolverTests : SolverTestsBase
    {
        [TestCase(100, 2853, "C")]
        [TestCase(214, 20066, "C")]
        [TestCase(214, 20066, "CC")]
        [TestCase(214, 20066, "CCC")]
        public void SolveOne(int problemId, int prevBestTime, string buy)
        {
            var solver = new ParallelDeepWalkSolver(2, new Estimator(), usePalka: false, buy.ToBuyBoosters());

            var solved = SolveOneProblem(solver, problemId);
            
            var nextTime = solved.CalculateTime();
            var map = ProblemReader.Read(problemId).ToState().Map;

            var mapScore = Math.Log(map.SizeX * map.SizeY, 2) * 1000;

            var prevScore = Math.Ceiling(mapScore * nextTime / prevBestTime);
            var nextScore = Math.Ceiling(mapScore);

            var cost = solved.BuyCost();
            var nextScoreWithCost = nextScore - cost;

            Console.Out.WriteLine($"{(nextScoreWithCost - prevScore > 0 ? "WIN" : "---")} Delta={nextScoreWithCost - prevScore}; PrevScore={prevScore};" +
                                  $"NextScore={nextScore}; Cost: {cost}; NextScoreWithCost={nextScoreWithCost}; " +
                                  $"PrevBestTime={prevBestTime}; NextTime={nextTime}");
        }
    }
}