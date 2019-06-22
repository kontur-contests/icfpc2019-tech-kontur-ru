using System;
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
            var solver = new RandomWalkSolver(2, new Estimator(), new Random(seed), 100, usePalka: true);
            SolveOneProblem(solver, 5);
        }

        [Test]
        public void SolveOneWithWheels()
        {
            //var seed = Guid.NewGuid().GetHashCode();
            var seed = -1635707027;
            Console.Out.WriteLine($"Seed: {seed}");
            var solver = new RandomWalkSolver(2, new Estimator(true), new Random(seed), 100, usePalka: true, useWheels:true);
            SolveOneProblem(solver, 5);
        }

        [Test]
        public void SolveOneZakoulochki()
        {
            //var seed = Guid.NewGuid().GetHashCode();
            var seed = -1635707027;
            Console.Out.WriteLine($"Seed: {seed}");
            var solver = new RandomWalkSolver(2, new EstimatorZakoulocki(), new Random(seed), 100, usePalka: true);
            SolveOneProblem(solver, 25);
        }
    }
}