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
            var solver = new BlockDeepWalkSolver(5, 2, new Estimator(), usePalka: true);
            SolveOneProblem(solver, 2);
        }
    }
}