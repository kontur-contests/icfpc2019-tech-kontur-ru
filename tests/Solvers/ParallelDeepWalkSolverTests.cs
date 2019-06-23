using System;
using lib.Models;
using lib.Solvers.RandomWalk;
using NUnit.Framework;

namespace tests.Solvers
{
    [TestFixture]
    internal class ParallelDeepWalkSolverTests : SolverTestsBase
    {
        [TestCase(2, 350, "", false)]
        [TestCase(2, 350, "", true)]
        [TestCase(2, 350, "C", false)]
        [TestCase(2, 350, "C", true)]
        [TestCase(2, 350, "CC", false)]
        [TestCase(2, 350, "CC", true)]
        [TestCase(2, 350, "F", true)]
        [TestCase(2, 350, "FC", true)]
        [TestCase(2, 350, "FFC", true)]
        [TestCase(2, 350, "FCC", true)]
        [TestCase(2, 350, "FFCC", true)]
        [TestCase(214, 20066, "FC", true)]
        // [TestCase(214, 20066, "CC")]
        // [TestCase(214, 20066, "CCC")]
        public void SolveOne(int problemId, int prevBestTime, string buy, bool useWheels)
        {
            var solver = new ParallelDeepWalkSolver(2, new Estimator(useWheels, false, false), usePalka: false, useWheels: useWheels, buy.ToBuyBoosters());

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



        [TestCase(5, false)]
        [TestCase(5, true)]
        [TestCase(22, false)]
        [TestCase(22, true)]
        [TestCase(100, false)]
        [TestCase(100, true)]
        public void Zakoulochki(int problemId, bool zakoulochki)
        {
            var solver = new ParallelDeepWalkSolver(2, new Estimator(false, zakoulochki, false), usePalka: false, useWheels: false, new []{BoosterType.Cloning});

            var solved = SolveOneProblem(solver, problemId);

            var nextTime = solved.CalculateTime();
            Console.WriteLine(nextTime);
        }

    }
}