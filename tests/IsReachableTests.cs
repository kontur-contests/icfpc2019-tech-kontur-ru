using System;
using FluentAssertions;
using lib;
using lib.Models;
using NUnit.Framework;

namespace tests
{
    [TestFixture]
    public class IsReachableTests
    {
        [TestCase("0,0", "1,1", true)]
        [TestCase("5,0", "6,1", true)]
        [TestCase("5,0", "7,1", false)]
        [TestCase("5,2", "7,1", false)]
        [TestCase("7,1", "5,0", false)]
        [TestCase("7,1", "5,2", false)]
        [TestCase("4,0", "7,1", true)]
        [TestCase("7,1", "4,0", true)]
        public void IsReachableOnFirstMap(string from, string to, bool expected)
        {
            var problem = new ProblemReader(ProblemReader.PART_1_INITIAL).Read(1);
            problem.ToState().Map.IsReachable(from, to).Should().Be(expected);
        }

        [TestCase("1,1", "2,2", true)]
        [TestCase("1,1", "3,3", true)]
        [TestCase("1,1", "2,3", false)]
        [TestCase("1,1", "3,2", false)]
        [TestCase("1,0", "2,2", false)]
        [TestCase("1,0", "3,3", false)]
        [TestCase("1,0", "2,3", true)]
        [TestCase("1,0", "3,2", false)]
        [TestCase("0,1", "2,2", false)]
        [TestCase("0,1", "3,3", false)]
        [TestCase("0,1", "2,3", false)]
        [TestCase("0,1", "3,2", true)]
        [TestCase("0,0", "2,2", true)]
        [TestCase("0,0", "3,3", true)]
        [TestCase("0,0", "2,3", false)]
        [TestCase("0,0", "3,2", false)]
        public void IsReachable(string from, string to, bool expected)
        {
            var map = new Map(4, 4);
            map["0,0"] = map["1,0"] = map["0,1"] = map["1,1"] = CellState.Void;
            map["2,2"] = map["3,2"] = map["2,3"] = map["3,3"] = CellState.Void;
            map.IsReachable(from, to).Should().Be(expected);
            map.IsReachable(to, from).Should().Be(expected);
        }

        [Test]
        public void BadTest()
        {
            var map = CreateEmptyMap(7);
            map.IsReachable("0,0", "2,6").Should().BeTrue();
        }

        [TestCase(6)]
        [TestCase(7)]
        [TestCase(10)]
        //[TestCase(20)]
        public void DontLoop(int size)
        {
            var map = CreateEmptyMap(size);
            for (int x1 = 0; x1 < map.SizeX; x1++)
            for (int y1 = 0; y1 < map.SizeY; y1++)
            for (int x2 = 0; x2 < map.SizeX; x2++)
            for (int y2 = 0; y2 < map.SizeY; y2++)
            {
                Console.WriteLine($"{new V(x1, y1)} {new V(x2, y2)}");
                map.IsReachable(new V(x1, y1), new V(x2, y2)).Should().Be(true);
            }
        }

        private static Map CreateEmptyMap(int size)
        {
            var map = new Map(size, size);
            for (int x = 0; x < map.SizeX; x++)
            for (int y = 0; y < map.SizeY; y++)
            {
                map[new V(x, y)] = CellState.Void;
            }

            return map;
        }
    }
}