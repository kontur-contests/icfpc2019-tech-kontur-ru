using System.Collections.Generic;
using FluentAssertions;
using lib;
using lib.Models;
using NUnit.Framework;

namespace tests
{
    [TestFixture]
    public class WorkerTests
    {
        [Test]
        public void GetManipulators()
        {
            var worker = new Worker
            {
                Direction = Direction.Up,
                Manipulators = new List<V>
                {
                    new V(0, 1),
                    new V(-1, 1),
                    new V(1, 1),
                }
            };
            var points = worker.GetManipulators(Direction.Left);
            points.Should().BeEquivalentTo(new V[] {new V(-1, 1), new V(-1, -1), new V(-1, 0),});
        }

        [TestCase("1,1", Direction.Left, Direction.Left, "1,1")]
        [TestCase("2,1", Direction.Up, Direction.Right, "1,-2")]
        [TestCase("2,1", Direction.Up, Direction.Left, "-1,2")]
        public void GetManipulators(string initial, Direction initialDir, Direction finalDirection, string expected)
        {
            var worker = new Worker
            {
                Direction = initialDir,
                Manipulators = new List<V>
                {
                    initial,
                }
            };
            var points = worker.GetManipulators(finalDirection);
            points.Should().BeEquivalentTo(new V[] {expected});
        }
    }
}