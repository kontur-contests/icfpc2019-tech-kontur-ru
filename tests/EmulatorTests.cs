using FluentAssertions;
using lib.Solvers.Postprocess;
using lib.Solvers.RandomWalk;
using NUnit.Framework;
using tests.Solvers;

namespace tests
{
    [TestFixture]
    public class EmulatorTests : SolverTestsBase
    {
        [TestCase("WSADZEQB(10,20)B(-10,-20)FL", "FL")]
        public void ParseSolved(string sol, string buy)
        {
            var solved = Emulator.ParseSolved(sol, buy);
            solved.FormatSolution().Should().Be(sol);
            solved.FormatBuy().Should().Be(buy);
        }

        [Test]
        public void Emulate()
        {
            var solver = new DeepWalkSolver(2, new Estimator(true, true, collectDrill:true), usePalka: true, useWheels:true, useDrill:true);
            
            var state = ReadFromFile(2);
            var result = solver.Solve(state);

            var readFromFile = ReadFromFile(2);
            Emulator.Emulate(readFromFile, result);
            
            readFromFile.History.Should().BeEquivalentTo(state.History);
        }
    }
}