using lib.Solvers.RandomWalk;
using NUnit.Framework;

namespace tests.Solvers
{
    [TestFixture]
    internal class ParallelPlanSolverTests : SolverTestsBase
    {
        [Test]
        public void SolveOne()
        {
            var solver = new ParallelPlanSolver(2);
            SolveOneProblemWithCluster(solver, 221);
        }
    }
}