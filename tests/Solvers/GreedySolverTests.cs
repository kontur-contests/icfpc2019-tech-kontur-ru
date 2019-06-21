using System;
using lib.Solvers;
using lib.Solvers.RandomWalk;
using NUnit.Framework;

namespace tests.Solvers
{
    [TestFixture]
    internal class GreedySolverTests : SolverTestsBase
    {
        [Test]
        public void SolveOne()
        {
            //var seed = Guid.NewGuid().GetHashCode();
            var seed = -1635707027;
            Console.Out.WriteLine($"Seed: {seed}");
            var solver = new GreedySolver(new SingleStateEstimator());
            SolveOneProblem(solver, 2);
        }
    }
}