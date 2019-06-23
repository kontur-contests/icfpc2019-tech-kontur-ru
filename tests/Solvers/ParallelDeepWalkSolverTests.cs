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
            var solver = new ParallelDeepWalkSolver(2, new Estimator(), usePalka: false);
            SolveOneProblem(solver, 250);
        }
    }
}