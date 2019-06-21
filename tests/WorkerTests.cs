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
            points.Should().BeEquivalentTo(new V[] {new V(-1, 1), new V(-1, -1), new V(-1, 1),});
        }
    }
}