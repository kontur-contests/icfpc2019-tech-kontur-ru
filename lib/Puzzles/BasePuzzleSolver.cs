using System;
using System.Collections.Generic;
using System.Linq;
using lib.Models;

namespace lib.Puzzles
{
    public class BasePuzzleSolver
    {
        [ThreadStatic] private static Random random = new Random();

        protected Problem Complete(Map<PuzzleCell> map, Puzzle puzzle)
        {
            while (true)
            {
                var countIn = MarkedInside(map).Count;
                var countMin = (int)(0.2 * puzzle.TaskSize * puzzle.TaskSize + 10);

                if (countIn < countMin)
                {
                    AddCells(map, countMin - countIn);
                    continue;
                }

                var verts = VerticesCount(map);
                if (verts < puzzle.MinVertices)
                {
                    AddVertices(map, puzzle.MinVertices);
                    continue;
                }

                break;
            }

            //Print(map);

            var inside = MarkedInside(map);

            var boosters = new List<Booster>();
            var boosterCellIndex = 1;

            var needBoosters = new List<(BoosterType, int)>
            {
                (BoosterType.Cloning, puzzle.ClonesCount),
                (BoosterType.Drill, puzzle.DrillsCount),
                (BoosterType.Extension, puzzle.ManipulatorsCount),
                (BoosterType.FastWheels, puzzle.FastwheelsCount),
                (BoosterType.MysteriousPoint, puzzle.SpawnsCount),
                (BoosterType.Teleport, puzzle.TeleportsCount)
            };

            foreach (var (type, cnt) in needBoosters)
                for (var i = 0; i < cnt; i++)
                    boosters.Add(new Booster(type, inside[boosterCellIndex++ % inside.Count]));

            var problem = new Problem
            {
                Map = PuzzleConverter.ConvertMapToPoints(map),
                Boosters = boosters,
                Obstacles = new List<List<V>>(),
                Point = inside.First()
            };

            return problem;
        }

        protected void Print(Map<PuzzleCell> map, V mark = null)
        {
            var enumerable = Enumerable
                .Range(0, map.SizeY)
                .Select(
                    y =>
                    {
                        var strings = Enumerable
                            .Range(0, map.SizeX)
                            .Select(x => x == mark?.X && y == mark?.Y ? '@' : (char)map[new V(x, map.SizeY - y - 1)])
                            .ToArray();
                        return string.Join("", strings);
                    });
            Console.WriteLine(string.Join("\n", enumerable));
        }

        private void AddVertices(Map<PuzzleCell> map, int puzzleMinVertices)
        {
            int was = VerticesCount(map);
            for (int x = 0; x < map.SizeX; x++)
                for (int y = 0; y < map.SizeY; y++)
                {
                    var v = new V(x, y);
                    if (OnBound(map, v, out var u))
                    {
                        var dirs = new[]
                        {
                            Direction.Up, Direction.Right, Direction.Up, Direction.Up, Direction.Left, Direction.Up
                        };

                        var path = new List<V> {v};
                        for (int step = 0; step < Math.Max(10, puzzleMinVertices - was); step++)
                        {
                            v = v.Shift((int)dirs[step % dirs.Length]);
                            if (!v.Inside(map) || map[v] != PuzzleCell.Unknown || !DeepOutside(map, v))
                                break;
                            path.Add(v);
                        }

                        if (path.Count < 5)
                            continue;
                        foreach (var v1 in path)
                            map[v1] = PuzzleCell.Inside;

                        was = VerticesCount(map);

                        if (was >= puzzleMinVertices)
                           return;
                    }
                }
        }

        private int VerticesCount(Map<PuzzleCell> map)
        {
            var pts = PuzzleConverter.ConvertMapToPoints(map);
            return pts.Count;
        }

        private void AddCells(Map<PuzzleCell> map, int need)
        {
            for (int x = 0; x < map.SizeX && need > 0; x++)
                for (int y = 0; y < map.SizeY && need > 0; y++)
                {
                    var v = new V(x, y);
                    if (OnBound(map, v, out _))
                    {
                        map[v] = PuzzleCell.Inside;
                        need--;
                    }
                }
        }

        private bool OnBound(Map<PuzzleCell> map, V v, out V u)
        {
            u = null;
            if (!v.Inside(map) || map[v] != PuzzleCell.Unknown)
                return false;

            for (int d = 0; d < 4; d++)
            {
                u = v.Shift(d);
                if (u.Inside(map) && map[u] == PuzzleCell.Inside)
                    return true;
            }

            return false;
        }

        private bool DeepOutside(Map<PuzzleCell> map, V v)
        {
            if (!v.Inside(map) || map[v] != PuzzleCell.Unknown)
                return false;

            for (int dx = -1; dx <= 1; dx++)
            for (int dy = -1; dy <= 1; dy++)
            {
                var u = new V(v.X + dx, v.Y + dy);
                if (u.Inside(map) && map[u] != PuzzleCell.Unknown)
                    return false;
            }

            return true;
        }

        private List<V> MarkedInside(Map<PuzzleCell> map)
        {
            var result = new List<V>();
            for (int x = 0; x < map.SizeX; x++)
                for (int y = 0; y < map.SizeY; y++)
                    if (map[new V(x, y)] == PuzzleCell.Inside)
                        result.Add(new V(x, y));
            return result;
        }
    }
}