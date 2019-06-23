using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using lib.Models;
using lib.Solvers.RandomWalk;

namespace lib.Solvers
{
    public static class RunnableSolvers
    {
        public static List<Func<ISolver>> PuzzleSolvers()
        {
            var result = new List<Func<ISolver>>
            {
                () => new StupidSolver(false),
                () => new StupidSolver(true),
                () => new ParallelDeepWalkSolver(2, new Estimator(false), usePalka: false, useWheels: false, new BoosterType[0]),
                //() => new ParallelDeepWalkSolver(2, new Estimator(false, true), usePalka: false, useWheels: false, new BoosterType[0]),
                //() => new PalkaSolver()
                //() => new RandomWalkSolver(depth: 2, new Estimator(), new Random(Guid.NewGuid().GetHashCode()), 100, usePalka: true),
                //() => new DeepWalkSolver(depth: 2, new Estimator()),
            };

            //result.Add(() => new MiningSolver(false, -1));
            //result.Add(() => new MiningSolver(true, -1));

            foreach (var limit in new[] { 5, 10, 15, 20, 25, 30 })
            {
                foreach (var use in new [] {true, false})
                {
                    result.Add(() => new MiningSolver(use,  limit));
                }
            }

            return result;
        }

        // This and only this solvers would be periodically re-run
        public static List<Func<ISolver>> Enumerate()
        {
            return new List<Func<ISolver>>
            {
                //() => new RandomWalkSolver(depth: 2, new Estimator(true), new Random(Guid.NewGuid().GetHashCode()), 100, usePalka: true, true),
                () => new DeepWalkSolver(depth: 2, new Estimator(true, true), true, true),
                //() => new StupidSolver(),
                //() => new RandomWalkSolver(depth: 2, new Estimator(), new Random(Guid.NewGuid().GetHashCode()), 100, usePalka: true),
                //() => new DeepWalkSolver(depth: 2, new Estimator()),
                () => new ParallelDeepWalkSolver(2, new Estimator(collectFastWheels: false), usePalka: false, useWheels: false, new[]{BoosterType.Cloning, }),
                () => new ParallelDeepWalkSolver(2, new Estimator(collectFastWheels: false), usePalka: false, useWheels: false, new[]{BoosterType.Cloning, BoosterType.Cloning, }),
            };
        }

        public static SolutionMeta Solve(ISolver solver, ProblemMeta problemMeta)
        {
            var stopwatch = Stopwatch.StartNew();

            var state = problemMeta.Problem.ToState();
            state.ClustersState = new ClustersState(ClustersStateReader.Read(problemMeta.ProblemId), state);
            
            var pathFileName = Path.Combine(FileHelper.PatchDirectoryName("clusters.v2"), $"prob-{problemMeta.ProblemId:000}.path");
            state.ClustersState.Path = File.ReadAllLines(pathFileName).Select(int.Parse).ToList();
            
            var actions = solver.Solve(state);
            var solutionBlob = actions.FormatSolution();
            var buyBlob = actions.FormatBuy();
            var moneyCost = actions.BuyCost();

            stopwatch.Stop();
            var calculationTime = stopwatch.ElapsedMilliseconds;
                        
            return new SolutionMeta(
                problemMeta.ProblemId,
                solutionBlob,
                actions.CalculateTime(),
                solver.GetName(),
                solver.GetVersion(),
                calculationTime,
                buyBlob,
                moneyCost
            );
        }
    }
}
