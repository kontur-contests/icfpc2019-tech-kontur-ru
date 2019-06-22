using lib.Solvers.RandomWalk;
using NUnit.Framework;

namespace tests.Solvers
{
    [TestFixture]
    internal class PlanSolverTests : SolverTestsBase
    {
        [Test]
        public void SolveOne()
        {
            var solver = new PlanSolver(2);
            SolveOneProblemWithCluster(solver, 2);
        }
    }
}