using System;
using System.Linq;
using lib.Models;
using lib.Solvers.RandomWalk;
using NUnit.Framework;
using pipeline;

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


        [TestCase("22,47,50,70,130")]
        [TestCase("22,100,150")]
        [TestCase("30,60,90")]
        //[TestCase("218")]
        public void Compare(string problemIds)
        {
            var parallelSolver = new ParallelDeepWalkSolver(2, new Estimator(collectFastWheels: true, zakoulochki: true, collectDrill: false), usePalka: false, useWheels: true, useDrill: false, new BoosterType[0]);
            var solver = new DeepWalkSolver(2, new Estimator(collectFastWheels: true, zakoulochki: true, collectDrill: true), usePalka: true, useWheels: true, useDrill: true);

            foreach (var problemId in problemIds.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse))
            {
                var solved = SolveOneProblem(solver, problemId);
                var parallelSolved = SolveOneProblem(parallelSolver, problemId);
                var win = parallelSolved.CalculateTime() < solved.CalculateTime() ? "P" : "S";
                Console.Out.WriteLine($"{problemId:000}: {win} - solvedTime: {solved.CalculateTime()}; parallelSolvedTime: {parallelSolved.CalculateTime()};");
            }
        }


        [TestCase("22,47,50,70,130")]
        [TestCase("22,100,150")]
        [TestCase("30,60,90")]
        //[TestCase("218")]
        public void Sequential(string problemIds)
        {
            var solver = new DeepWalkSolver(2, new Estimator(collectFastWheels: true, zakoulochki: true, collectDrill: true), usePalka: true, useWheels: true, useDrill: true);
            
            foreach (var problemId in problemIds.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse))
            {
                var solved = SolveOneProblem(solver, problemId);
                Console.Out.WriteLine($"{problemId:000}: solvedTime: {solved.CalculateTime()}");
            }
        }


        [TestCase("221")]
        [TestCase("230,241")]
        [TestCase("270")]
        [TestCase("243")]
        [TestCase("255")]
        [TestCase("299")]
        public void CompareParallel(string problemIds)
        {
            var parallelSolver = new ParallelDeepWalkSolver(2, new Estimator(collectFastWheels: false, zakoulochki: true, collectDrill: false), usePalka: false, useWheels: false, useDrill: false, new BoosterType[0]);
            
            foreach (var problemId in problemIds.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse))
            {
                var parallelSolved = SolveOneProblem(parallelSolver, problemId);
                Console.Out.WriteLine($"{problemId:000}: parallelSolvedTime: {parallelSolved.CalculateTime()};");
            }
        }

        [TestCase(250, 2575)]
        [TestCase(251, 3397)]
        [TestCase(265, 3397)]
        [TestCase(266, 3397)]
        public void RemoveTrash(int problemId, int ourTime)
        {
            Storage.Remove(problemId, "parallel-deep-2-False-True-True-wheels-zako-drrr-", ourTime);
        }
    }
}