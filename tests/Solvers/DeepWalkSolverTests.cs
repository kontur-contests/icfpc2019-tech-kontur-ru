using lib.Solvers.RandomWalk;
using NUnit.Framework;

namespace tests.Solvers
{
    [TestFixture]
    internal class DeepWalkSolverTests : SolverTestsBase
    {
        [Test]
        public void SolveOne()
        {
            var solver = new DeepWalkSolver(2, new ClusterWorkerEstimator(), usePalka: true, useWheels:false);
            SolveOneProblemWithCluster(solver, 1);
        }
    }
}