using System.Linq;
using FluentAssertions;
using lib.Models;
using lib.Models.Actions;
using NUnit.Framework;

namespace tests
{
    [TestFixture]
    public class SolutionExtensionsTests
    {
        [Test]
        public void Format()
        {
            new ActionBase[]
                {
                    new Move("0,1"), new Move("0,-1"), new Move("-1,0"), new Move("1,0"),
                    new Wait(),
                    new Rotate(true),
                    new Rotate(false),
                    new UseExtension("10,20"), new UseExtension("-10,-20"),
                    new UseFastWheels(),
                    new UseDrill(),
                }.Format()
                .Should()
                .Be("WSADZEQB(10,20)B(-10,-20)FL");
        }

        [TestCase(".....", 5)]
        [TestCase("..C|", 3)]
        [TestCase("..C..|.", 5)]
        [TestCase("..C..|..", 5)]
        [TestCase("..C..|...", 6)]
        [TestCase("..C..|C...|..", 7)]
        public void CalculateTime(string solution, int expected)
        {
            var list = solution.Split("|").Select(x => x.Select(c => c == 'C' ? (ActionBase)new UseCloning() : new Wait()).ToList()).ToList();
            list.CalculateTime().Should().Be(expected);
        }
    }
}