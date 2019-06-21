using System.Collections.Generic;
using FluentAssertions;
using lib;
using lib.Models;
using lib.Strategies;
using NUnit.Framework;

namespace tests.Strategies
{
    [TestFixture, Explicit]
    internal class BruteForceTests
    {
        [Test]
        public void TestMap()
        {
            var a = new Map(1, 1);
            var b = new Map(1, 1);
            
            Equals(a, b).Should().BeTrue();
        }

        [Test]
        public void Test()
        {
            var bruteForce = new BruteForce();
            var x = bruteForce.Solve(new State(new Worker()
            {
                Direction = 0,
                Position = V.Zero
            }, new Map(4, 4), new List<Booster>()));

        }
    }
}