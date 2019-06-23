using System;
using lib.Models;
using lib.Solvers;
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
            var solver = new RandomWalkSolver(2, new Estimator(), new Random(seed), 100, usePalka: true, false);
            SolveOneProblem(solver, 5);
        }

        [Test]
        public void SolveOneWithWheels()
        {
            //var seed = Guid.NewGuid().GetHashCode();
            var seed = -1635707027;
            Console.Out.WriteLine($"Seed: {seed}");
            var solver = new RandomWalkSolver(2, new Estimator(true), new Random(seed), 100, usePalka: true, useWheels:true);
            SolveOneProblem(solver, 22);
        }

        [Test]
        public void SolvePuzzleOneWithWheels()
        {
            //var seed = Guid.NewGuid().GetHashCode();
            var seed = -1635707027;
            Console.Out.WriteLine($"Seed: {seed}");
            var solver0 = new StupidSolver(true);
            var solver1 = new ParallelDeepWalkSolver(2, new Estimator(true), usePalka: true, new BoosterType[0]);
            var solver = new RandomWalkSolver(2, new Estimator(true), new Random(seed), 100, usePalka: true, useWheels: true);
            SolvePuzzleProblem(solver1, 2);
        }

        [Test]
        public void SolvePuzzleOneWithWheelsRandom()
        {
            //var seed = Guid.NewGuid().GetHashCode();
            var seed = -1635707027;
            Console.Out.WriteLine($"Seed: {seed}");
            var solver0 = new StupidSolver(true);
            var solver1 = new ParallelDeepWalkSolver(2, new Estimator(), usePalka: false, new BoosterType[0]);
            var solver = new RandomWalkSolver(2, new Estimator(true), new Random(seed), 100, usePalka: true, useWheels: true);
            SolvePuzzleProblem(solver, 2);
        }

        [Test]
        public void SolveOneZakoulochki()
        {
            //var seed = Guid.NewGuid().GetHashCode();
            var seed = -1635707027;
            Console.Out.WriteLine($"Seed: {seed}");
            var solver = new RandomWalkSolver(2, new EstimatorZakoulocki(), new Random(seed), 100, usePalka: true, false);
            SolveOneProblem(solver, 5);
        }
    }
}