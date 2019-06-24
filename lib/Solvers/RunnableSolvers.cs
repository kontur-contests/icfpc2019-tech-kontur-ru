using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using lib.Models;
using lib.Solvers.Postprocess;
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
                () => new ParallelDeepWalkSolver(2, new Estimator(false, false, false), usePalka: false, useWheels: false, useDrill: false, new BoosterType[0]),
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
                //() => new RandomWalkSolver(depth: 2, new Estimator(true), new Random(Guid.NewGuid().GetHashCode()), 100, usePalka: true, true),
                () => new DeepWalkSolver(2, new Estimator(collectFastWheels: true, zakoulochki: true, collectDrill: true), usePalka: true, useWheels: true, useDrill: true),
                //() => new StupidSolver(),
                //() => new RandomWalkSolver(depth: 2, new Estimator(), new Random(Guid.NewGuid().GetHashCode()), 100, usePalka: true),
                //() => new DeepWalkSolver(depth: 2, new Estimator()),
                //() => new ParallelDeepWalkSolver(2, new Estimator(collectFastWheels: false), usePalka: false, useWheels: false, new[]{BoosterType.Cloning, }),
                //() => new ParallelDeepWalkSolver(2, new Estimator(true, zakoulochki: true, false), usePalka: false, useWheels: true, useDrill: false, new BoosterType[0]),
                () => new ParallelDeepWalkSolver(2, new Estimator(collectFastWheels: true, zakoulochki: true, false), usePalka: false, useWheels: true, useDrill: false, new BoosterType[0]),
            };
        }

        public static SolutionMeta Solve(ISolver solver, ProblemMeta problemMeta)
        {
            var stopwatch = Stopwatch.StartNew();

            var problem = problemMeta.Problem;
            problem.ProblemId = problemMeta.ProblemId;
            var state = problem.ToState();
            state.ClustersState = new ClustersState(ClustersStateReader.Read(problemMeta.ProblemId), state);
            
            var pathFileName = Path.Combine(FileHelper.PatchDirectoryName("clusters.v2"), $"prob-{problemMeta.ProblemId:000}.path");
            state.ClustersState.Path = File.ReadAllLines(pathFileName).Select(int.Parse).ToList();
            
            var solved = solver.Solve(state);
            
            state = problem.ToState();
            Emulator.Emulate(state, solved);
            if (state.UnwrappedLeft > 0)
                throw new InvalidOperationException("Bad mother fucker!");
            
            var solutionBlob = solved.FormatSolution();
            var buyBlob = solved.FormatBuy();
            var moneyCost = solved.BuyCost();

            stopwatch.Stop();
            var calculationTime = stopwatch.ElapsedMilliseconds;
                        
            return new SolutionMeta(
                problemMeta.ProblemId,
                solutionBlob,
                solved.CalculateTime(),
                solver.GetName(),
                solver.GetVersion(),
                calculationTime,
                buyBlob,
                moneyCost
            );
        }
    }
}
