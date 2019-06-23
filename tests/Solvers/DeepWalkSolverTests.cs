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
            var solver = new DeepWalkSolver(2, new Estimator(true, true, collectDrill:true), usePalka: true, useWheels:true, useDrill:true);
            SolveOneProblem(solver, 2);
        }
    }
}