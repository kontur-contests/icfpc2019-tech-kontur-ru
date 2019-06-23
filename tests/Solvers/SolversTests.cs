using System;
using System.Linq;
using lib.Solvers;
using lib.Solvers.Postprocess;
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

        [Test]
        public void StupidOne1()
        {
            var id = 220;
            
            var solver = new StupidSolver(palka:false);
            var state = ReadFromFile(id);
            var result = solver.Solve(state);
            Console.WriteLine($"Original: {result.CalculateTime()}");
            Save(result, id, "original");

            var postprocessor = new Postprocessor(state, result);
            postprocessor.TransferSmall();

            var modified = state.History.BuildSolved();
            Console.WriteLine($"Modified: {modified.CalculateTime()}");
            Save(modified, id);
        }
    }
}