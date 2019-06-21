using FluentAssertions;
using lib.Models;
using NUnit.Framework;

namespace tests
{
    [TestFixture]
    public class SolutionFormatterTests
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
    }
}