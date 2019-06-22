using System;
using System.Linq;
using lib.Solvers.RandomWalk;
using NUnit.Framework;

namespace tests.Solvers
{
    [TestFixture]
    internal class BlockDeepWalkSolverTests : SolverTestsBase
    {
        [Test]
        public void SolveOne()
        {
            var solver = new BlockDeepWalkSolver(50, 2, new Estimator(), usePalka: true);
            SolveOneProblem(solver, 150);
        }
        
        [Test]
        public void SolveSome()
        {
            var seed = Guid.NewGuid().GetHashCode();
            Console.Out.WriteLine($"Seed: {seed}");
            SolveSomeProblems(() => new DeepWalkSolver(10, new Estimator()), 
                Enumerable.Range(1, 150).Take(30).ToList());
        }
    }
}