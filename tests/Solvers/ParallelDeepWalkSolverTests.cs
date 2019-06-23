using System;
using lib.Models;
using lib.Solvers.RandomWalk;
using NUnit.Framework;

namespace tests.Solvers
{
    [TestFixture]
    internal class ParallelDeepWalkSolverTests : SolverTestsBase
    {
        [TestCase(2, 350, "", false, false)]
        [TestCase(2, 350, "", false, true)]
        [TestCase(2, 350, "", true, false)]
        [TestCase(2, 350, "", true, true)]
        [TestCase(2, 350, "C", false, false)]
        [TestCase(2, 350, "C", false, true)]
        [TestCase(2, 350, "C", true, false)]
        [TestCase(2, 350, "C", true, true)]
        [TestCase(2, 350, "CC", false, false)]
        [TestCase(2, 350, "CC", false, true)]
        [TestCase(2, 350, "CC", true, false)]
        [TestCase(2, 350, "CC", true, true)]
        [TestCase(2, 350, "F", true, false)]
        [TestCase(2, 350, "F", true, true)]
        [TestCase(2, 350, "FC", true, false)]
        [TestCase(2, 350, "FC", true, true)]
        [TestCase(2, 350, "FFC", true, false)]
        [TestCase(2, 350, "FFC", true, true)]
        [TestCase(2, 350, "FCC", true, false)]
        [TestCase(2, 350, "FCC", true, true)]
        [TestCase(2, 350, "FFCC", true, false)]
        [TestCase(2, 350, "FFCC", true, true)]
        [TestCase(214, 20066, "FC", true, false)]
        [TestCase(214, 20066, "FC", true, true)]
        // [TestCase(214, 20066, "CC")]
        // [TestCase(214, 20066, "CCC")]
        public void SolveOne(int problemId, int prevBestTime, string buy, bool useWheels, bool useDrill)
        {
            var solver = new ParallelDeepWalkSolver(2, new Estimator(useWheels, false, false), usePalka: false, useWheels: useWheels, useDrill: useDrill, buy.ToBuyBoosters());

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
            var solver = new ParallelDeepWalkSolver(2, new Estimator(false, zakoulochki, false), usePalka: false, useWheels: false, useDrill: true, new []{BoosterType.Cloning});

            var solved = SolveOneProblem(solver, problemId);

            var nextTime = solved.CalculateTime();
            Console.WriteLine(nextTime);
        }

    }
}