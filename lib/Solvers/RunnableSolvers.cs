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
            return new List<Func<ISolver>>
            {
                () => new StupidSolver(false),
                () => new StupidSolver(true),
                () => new ParallelDeepWalkSolver(2, new Estimator(), usePalka: false),
                //() => new PalkaSolver()
                //() => new RandomWalkSolver(depth: 2, new Estimator(), new Random(Guid.NewGuid().GetHashCode()), 100, usePalka: true),
                //() => new DeepWalkSolver(depth: 2, new Estimator()),
            };
        }

        // This and only this solvers would be periodically re-run
        public static List<Func<ISolver>> Enumerate()
        {
            return new List<Func<ISolver>>
            {
                () => new RandomWalkSolver(depth: 2, new Estimator(true), new Random(Guid.NewGuid().GetHashCode()), 100, usePalka: true, true),
                () => new DeepWalkSolver(depth: 2, new Estimator(true), true, true),
                //() => new StupidSolver(),
                //() => new RandomWalkSolver(depth: 2, new Estimator(), new Random(Guid.NewGuid().GetHashCode()), 100, usePalka: true),
                //() => new DeepWalkSolver(depth: 2, new Estimator()),
                //() => new BlockDeepWalkSolver(blockSize: 25, depth: 2, new Estimator(), usePalka: true),
                //() => new BlockDeepWalkSolver(blockSize: 50, depth: 2, new Estimator(), usePalka: true),
                //() => new BlockDeepWalkSolver(blockSize: 50, depth: 3, new Estimator(), usePalka: true),
                // () => new FastParallelDeepWalkSolver(2, new FastWorkerEstimator(), usePalka: false),
                // () => new FastParallelDeepWalkSolver(2, new FastWorkerEstimator(), usePalka: true),
                //() => new ParallelPlanSolver(2),
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
            var solutionBlob = actions.Format();

            stopwatch.Stop();
            var calculationTime = stopwatch.ElapsedMilliseconds;
                        
            return new SolutionMeta(
                problemMeta.ProblemId,
                solutionBlob,
                actions.CalculateTime(),
                solver.GetName(),
                solver.GetVersion(),
                calculationTime
            );
        }
    }
}
