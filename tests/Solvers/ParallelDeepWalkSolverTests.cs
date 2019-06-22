using System;
using lib.Models;
using lib.Solvers.RandomWalk;
using NUnit.Framework;

namespace tests.Solvers
{
    [TestFixture]
    internal class ParallelDeepWalkSolverTests : SolverTestsBase
    {
        [Test]
        public void SolveOne()
        {
            var solver = new ParallelDeepWalkSolver(2, new WorkerEstimator(), usePalka: false);
            SolveOneProblem(solver, 221);

            // var state = ReadFromFile(221);
            // var result = solver.Solve(state);
            // Console.Out.WriteLine(result.Format());
            // Console.Out.WriteLine(state.Print());
        }
    }
}