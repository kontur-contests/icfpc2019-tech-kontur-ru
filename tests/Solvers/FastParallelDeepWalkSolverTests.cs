using lib.Solvers.RandomWalk;
using NUnit.Framework;

namespace tests.Solvers
{
    [TestFixture]
    internal class FastParallelDeepWalkSolverTests : SolverTestsBase
    {
        [Test]
        public void SolveOne()
        {
            var solver = new FastParallelDeepWalkSolver(2, new FastWorkerEstimator(), usePalka: false);
            SolveOneProblem(solver, 296);
        }
    }
}