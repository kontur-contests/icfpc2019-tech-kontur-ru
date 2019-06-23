using System.IO;
using System.Linq;
using lib;
using lib.Models;
using lib.Solvers;
using NUnit.Framework;

namespace tests.Solvers
{
    [TestFixture]
    internal class SolversTests : SolverTestsBase
    {
        [Test]
        public void MiningPuzzleOne()
        {
            SolvePuzzleProblem(new MiningSolver(), 33);
        }

        [Test]
        public void PalkaOne()
        {
            //Solved 96 problem in 3881 steps. (stupid)
            //Solved 96 problem in 3477 steps. (collect manipulators)
            SolveOneProblem(new PalkaSolver(), 96);
        }

        [Test]
        public void PalkaSome()
        {
            SolveSomeProblems(() => new PalkaSolver(),
                Enumerable.Range(1, 150).Take(30).ToList());
        }

        [Test]
        public void StupidOne()
        {
            //Solved 96 problem in 3881 steps. (stupid)
            //Solved 96 problem in 3477 steps. (collect manipulators)
            SolveOneProblem(new StupidSolver(), 96);
        }
        
        [Test]
        public void StupidSome()
        {
            SolveSomeProblems(() => new StupidSolver(), 
                Enumerable.Range(1, 150).Take(30).ToList());
        }
    }
}