using System.Linq;
using lib.Solvers;
using NUnit.Framework;

namespace tests.Solvers
{
    [TestFixture]
    internal class StupidSolverTests : SolverTestsBase
    {
        [Test]
        public void SolveOne()
        {
            //Solved 96 problem in 3881 steps. (stupid)
            //Solved 96 problem in 3477 steps. (collect manipulators)
            SolveOneProblem(new StupidSolver(), 96);
        }
        
        [Test]
        public void SolveSome()
        {
            SolveSomeProblems(() => new StupidSolver(), 
                Enumerable.Range(1, 150).Take(30).ToList());
        }
    }
}