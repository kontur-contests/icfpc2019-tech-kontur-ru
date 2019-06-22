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
            var solver = new DeepWalkSolver(2, new Estimator());
            SolveOneProblem(solver, 5);
        }

        [Test]
        public void SolveOneWithFastWheels()
        {
            var solver = new DeepWalkSolver(2, new Estimator(collectFastWheels:true), useWheels:true);
            SolveOneProblem(solver, 5);
        }

        [Test]
        public void SolveOneZakoulochki()
        {
            var solver = new DeepWalkSolver(2, new EstimatorZakoulocki());
            SolveOneProblem(solver, 10);
        }
    }
}