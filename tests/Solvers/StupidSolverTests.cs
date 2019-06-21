using System;
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