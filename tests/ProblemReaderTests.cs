using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using lib;
using lib.Models;
using NUnit.Framework;

namespace tests
{
    [TestFixture]
    public class ProblemReaderTests
    {
        [Test]
        public void Read()
        {
            var source = "(0,0),(10,0),(10,10),(0,10)#(0,0)#(4,2),(6,2),(6,7),(4,7);(5,8),(6,8),(6,9),(5,9)#B(0,1);B(1,1);F(0,2);F(1,2);L(0,3);X(0,9)";
            var problem = ProblemReader.Read(source);
            problem.ToString().Should().Be(source);
        }

        [Test]
        public void ReadFromFile([Range(1, 300)] int problem)
        {
            var fileName = Path.Combine(FileHelper.PatchDirectoryName("problems"), "all", $"prob-{problem:000}.desc");
            ProblemReader.Read(problem).ToString().Should().Be(File.ReadAllText(fileName));
        }
        
        [Test]
        public async Task ReadCurrentFromApi()
        {
            var problem = await ProblemReader.ReadCurrentFromApiAsync();
            problem.Should().NotBeNull();
        }

        [Test]
        public void ToState()
        {
            var state = ProblemReader.Read(1).ToState();
            var expectedMap = new Map(8, 3)
            {
                [new V(0, 0)] = CellState.Void, [new V(1, 0)] = CellState.Void, [new V(2, 0)] = CellState.Void, [new V(3, 0)] = CellState.Void, [new V(4, 0)] = CellState.Void, [new V(5, 0)] = CellState.Void, [new V(6, 0)] = CellState.Obstacle, [new V(7, 0)] = CellState.Obstacle,
                [new V(0, 1)] = CellState.Void, [new V(1, 1)] = CellState.Void, [new V(2, 1)] = CellState.Void, [new V(3, 1)] = CellState.Void, [new V(4, 1)] = CellState.Void, [new V(5, 1)] = CellState.Void, [new V(6, 1)] = CellState.Void, [new V(7, 1)] = CellState.Void,
                [new V(0, 2)] = CellState.Void, [new V(1, 2)] = CellState.Void, [new V(2, 2)] = CellState.Void, [new V(3, 2)] = CellState.Void, [new V(4, 2)] = CellState.Void, [new V(5, 2)] = CellState.Void, [new V(6, 2)] = CellState.Obstacle, [new V(7, 2)] = CellState.Obstacle,
            };
            state.Should()
                .BeEquivalentTo(
                    new State(
                        new Worker
                        {
                            Position = new V(0, 0),
                            Manipulators = new List<V> {new V(1, 0), new V(1, 1), new V(1, -1)}
                        },
                        expectedMap,
                        new List<Booster>()
                    ));
            state.Map.ToString()
                .Should()
                .Be(
                    "......##\n" +
                    ".*......\n" +
                    "**....##");
        }

        [Test]
        public void ToState2()
        {
            var problem = ProblemReader.Read(9);
            var state = problem.ToState();
            state.Map.ToString()
                .Should()
                .Be(
                    "###...################\n" +
                    "###...################\n" +
                    "####...###############\n" +
                    "####...###############\n" +
                    "####...###############\n" +
                    "####...####......#####\n" +
                    "####...#.........#####\n" +
                    "#####............#####\n" +
                    "##.........###########\n" +
                    "........##############\n" +
                    ".*......#.............\n" +
                    "**###...#.............\n" +
                    "######.......#########\n" +
                    "######.......#########\n" +
                    "######...#...#########\n" +
                    "##########...#########\n" +
                    "######.......#########\n" +
                    "######.......#########\n" +
                    "######.......#....####\n" +
                    "######....#.......####\n" +
                    "###.......#.......####\n" +
                    "###.......#...########\n" +
                    "###...#####...########\n" +
                    "#######.......########\n" +
                    "#######.......########\n" +
                    "#######....###########");
        }
    }
}