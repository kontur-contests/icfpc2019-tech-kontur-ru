using lib.Solvers.RandomWalk;
using NUnit.Framework;

namespace tests.Solvers
{
    [TestFixture]
    internal class FastDeepWalkSolverTests : SolverTestsBase
    {
        [Test]
        public void SolveOne()
        {
            var solver = new FastDeepWalkSolver(2, new ClusterWorkerEstimator(), usePalka: false);
            SolveOneProblemWithCluster(solver, 96);
        }
    }
}