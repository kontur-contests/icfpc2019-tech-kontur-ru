using System;
using System.Collections.Generic;
using System.Diagnostics;
using lib.Models;
using lib.Solvers.RandomWalk;

namespace lib.Solvers
{
    public static class RunnableSolvers
    {
        // This and only this solvers would be periodically re-run
        public static List<Func<ISolver>> Enumerate()
        {
            return new List<Func<ISolver>>
            {
                () => new StupidSolver(),
                () => new RandomWalkSolver(3, new Estimator(), new Random(-1635707027), 100),
                () => new RandomWalkSolver(10, new Estimator(), new Random(Guid.NewGuid().GetHashCode()), 10),
            };
        }

        public static SolutionMeta Solve(ISolver solver, ProblemMeta problemMeta)
        {
            var stopwatch = Stopwatch.StartNew();
                        
            var actions = solver.Solve(problemMeta.Problem.ToState());
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