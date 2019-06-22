using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using lib;
using lib.Models;
using lib.Puzzles;
using NUnit.Framework;

namespace tests
{
    [TestFixture]
    public class PuzzleTests
    {
        [Test]
        public void Read()
        {
            var source = "0,1,150,400,1200,6,10,5,1,3,4#(73,61),(49,125),(73,110),(98,49),(126,89),(68,102),(51,132),(101,123),(22,132),(71,120),(97,129),(118,76),(85,100),(88,22),(84,144),(93,110),(96,93),(113,138),(91,52),(27,128),(84,140),(93,143),(83,17),(123,85),(50,74),(139,97),(101,110),(77,56),(86,23),(117,59),(133,126),(83,135),(76,90),(70,12),(12,141),(116,87),(102,76),(19,138),(86,129),(86,128),(83,60),(100,98),(60,105),(61,103),(94,99),(130,124),(141,132),(68,84),(86,143),(72,119)#(145,82),(20,65),(138,99),(38,137),(85,8),(125,104),(117,48),(57,48),(64,119),(3,25),(40,22),(82,54),(121,119),(1,34),(43,98),(97,120),(10,90),(15,32),(41,13),(86,40),(3,83),(2,127),(4,40),(139,18),(96,49),(53,22),(5,103),(112,33),(38,47),(16,121),(133,99),(113,45),(50,5),(94,144),(16,0),(93,113),(18,141),(36,25),(56,120),(3,126),(143,144),(99,62),(144,117),(48,97),(69,9),(0,9),(141,16),(55,68),(81,3),(47,53)";
            var puzzle = new Puzzle(source);
            puzzle.ToString().Should().Be(source);
        }

        [Test]
        public void TestSolve()
        {
            var puzzle = PuzzleReader.ReadFromFile(1);
            var problem = new MstPuzzleSolver().Solve(puzzle);
            problem.IsValidForPuzzle(puzzle).Should().BeTrue();
        }

        [Test]
        public void TestMapConvert([Range(1, 300)] int id)
        {
            var problem = ProblemReader.Read(id);
            problem.Obstacles = new List<List<V>>();

            var state = problem.ToState().Map;
            var map = new Map<Cell>(state.SizeX, state.SizeY);

            for (int x = 0; x < map.SizeX; x++)
            for (int y = 0; y < map.SizeY; y++)
                map[new V(x, y)] = state[new V(x, y)] != CellState.Obstacle ? Cell.Inside : Cell.Outside;

            var converted = PuzzleConverter.ConvertMapToPoints(map);

            var expected = problem.Map;
            var i = expected.IndexOf(converted[0]);
            if (i != 0)
                expected = expected.Skip(i).Concat(expected.Take(i)).ToList();

            converted.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
        }
    }
}