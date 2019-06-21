using lib.Models;
using lib.Solvers;
using NUnit.Framework;

namespace tests.Solvers
{
    [TestFixture]
    internal class StupidSolverTests
    {
        [Test]
        public void Test()
        {
            var state = ReadFromFile(1);
            var solver = new StupidSolver(state);
            var result = solver.Solve();
        }
        
        public State ReadFromFile(int id)
        {
            var reader = new ProblemReader(ProblemReader.PART_1_INITIAL);
            var problem = reader.Read(id);
            return problem.ToState();
        }
    }
}