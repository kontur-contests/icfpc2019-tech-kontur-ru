using System;
using FluentAssertions;
using lib.Models;
using NUnit.Framework;

namespace tests
{
    [TestFixture]
    public class CellCostCalculatorTests
    {
        [Test]
        public void DoSomething_WhenSomething()
        {
            var problem = ProblemReader.Read("(0,0),(3,0),(3,1),(5,1),(5,2),(3,2),(3,3),(0,3)#(0,0)##");
            var state = problem.ToState();
            Console.WriteLine(state.Map);
            var calc = new CellCostCalculator(state);
            calc.Cost.Should().Be(10);
            //2 2 2 
            //1 * 3 2 1
            //* * 1
            //1 1 1 
            //2 0 0 1 2
            //0 0 2
            calc.BeforeWrapCell("2,1");
            //2 2 1 
            //1 * * 1 1
            //* * 0
            //1 1 2 
            //2 0 0 2 2
            //0 0 3
            calc.Cost.Should().Be(13);
            calc.AfterUnwrapCell("2,1");
            calc.Cost.Should().Be(10);
        }
    }
}