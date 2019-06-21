using System;
using System.Linq;
using FluentAssertions.Extensions;
using lib.Solvers.RandomWalk;
using NUnit.Framework;

namespace tests.Solvers
{
    [TestFixture]
    internal class RandomWalkSolverTests : SolverTestsBase
    {
        [Test]
        public void SolveOne()
        {
            //var seed = Guid.NewGuid().GetHashCode();
            var seed = -1635707027;
            Console.Out.WriteLine($"Seed: {seed}");
            var solver = new RandomWalkSolver(2, new Estimator(), new Random(seed), 100);
            SolveOneProblem(solver, 2);
        }
        
        [Test]
        public void SolveSome()
        {
            var seed = Guid.NewGuid().GetHashCode();
            Console.Out.WriteLine($"Seed: {seed}");
            SolveSomeProblems(() => new RandomWalkSolver(10, new Estimator(), new Random(seed), 10), 
                Enumerable.Range(1, 150).Take(30).ToList());
        }
    }
}