using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using lib.Models;
using lib.Models.Actions;

namespace lib.Puzzles
{
    public enum Cell
    {
        Unknown = '.',
        Inside = 'i',
        Outside = 'o'
    }

    public class MstPuzzleSolver : IPuzzleSolver
    {
        public Problem Solve(Puzzle puzzle)
        {
            var map = SolveInner(puzzle);

            //Print(map);
            
            while (true)
            {
                var countIn = Inside(map).Count;
                var countMin = (int) (0.2 * puzzle.TaskSize * puzzle.TaskSize + 10);

                if (countIn < countMin)
                {
                    Add(map, countMin - countIn);
                    continue;
                }

                var verts = VertsCount(map);
                if (verts < puzzle.MinVertices)
                {
                    AddVerts(map, puzzle.MinVertices);
                    continue;
                }

                break;
            }

            //Print(map);
            
            var inside = Inside(map);

            var boosters = new List<Booster>();
            int bi = 1;
            
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
                    boosters.Add(new Booster(type, inside[(bi++) % inside.Count]));
            
            var problem = new Problem
            {
                Map = PuzzleConverter.ConvertMapToPoints(map),
                Boosters = boosters,
                Obstacles =  new List<List<V>>(),
                Point = inside.First()
            };

            return problem;
        }

        private void Print(Map<Cell> map)
        {
            var enumerable = Enumerable
                .Range(0, map.SizeY)
                .Select(
                    y =>
                    {
                        var strings = Enumerable
                            .Range(0, map.SizeX)
                            .Select(x => x == 56 && y == 120 ? '@' : (char)map[new V(x, map.SizeY - y - 1)])
                            .ToArray();
                        return string.Join("", strings);
                    });
            Console.WriteLine(string.Join("\n", enumerable));
        }

        private void AddVerts(Map<Cell> map, int puzzleMinVertices)
        {
            for (int x = 0; x < map.SizeX; x++)
            for (int y = 0; y < map.SizeY; y++)
            {
                var v = new V(x, y);
                if (OnBound(map, v))
                {
                    int was = VertsCount(map);
                    map[v] = Cell.Inside;
                    int cur = VertsCount(map);

                    if (cur <= was)
                    {
                        map[v] = Cell.Unknown;
                        continue;
                    }

                    if (cur >= puzzleMinVertices)
                        return;
                }
            }
        }

        private int VertsCount(Map<Cell> map)
        {
            var pts = PuzzleConverter.ConvertMapToPoints(map);
            return pts.Count;
        }

        private void Add(Map<Cell> map, int need)
        {
            for (int x = 0; x < map.SizeX && need > 0; x++)
            for (int y = 0; y < map.SizeY && need > 0; y++)
            {
                var v = new V(x, y);
                if (OnBound(map, v))
                {
                    map[v] = Cell.Inside;
                    need--;
                }
            }
        }

        private bool OnBound(Map<Cell> map, V v)
        {
            if (!v.Inside(map) || map[v] != Cell.Unknown)
                return false;

            for (int d = 0; d < 4; d++)
            {
                var u = v.Shift(d);
                if (u.Inside(map) && map[u] == Cell.Inside)
                    return true;
            }

            return false;
        }

        private List<V> Inside(Map<Cell> map)
        {
            var result = new List<V>();
            for (int x = 0; x < map.SizeX; x++)
                for (int y = 0; y < map.SizeY; y++)
                    if (map[new V(x, y)] == Cell.Inside)
                        result.Add(new V(x, y));
            return result;
        }

        public Map<Cell> SolveInner(Puzzle puzzle)
        {
            var map = new Map<Cell>(puzzle.TaskSize, puzzle.TaskSize);
            for (int x = 0; x < map.SizeX; x++)
            for (int y = 0; y < map.SizeY; y++)
                map[new V(x, y)] = Cell.Unknown;

            var not = puzzle.MustNotContainPoints.ToList();
            not.Insert(0, V.Zero);

            foreach (var n in not)
                map[n] = Cell.Outside;

            SolveInner(map, puzzle.MustContainPoints, Cell.Inside);

            foreach (var n in not)
                map[n] = Cell.Unknown;

            SolveInner(map, not, Cell.Outside);

            return map;
        }

        public void SolveInner(Map<Cell> map, List<V> points, Cell type)
        {
            var outside = points.ToList();

            if (outside.Any())
            {
                map[outside[0]] = type;
                outside.RemoveAt(0);
            }
            
            while (outside.Any())
            {
                var pathBuilder = new PathBuilder(map, type);

                var best = 0;
                for (int i = 1; i < outside.Count; i++)
                    if (pathBuilder.Distance(outside[i]) < pathBuilder.Distance(outside[best]))
                        best = i;

                var path = pathBuilder.GetPath(outside[best]);
                foreach (var x in path)
                    map[x] = type;

                outside.RemoveAt(best);
            }
        }

        private class PathBuilder
        {
            private readonly Map<Cell> map;
            private Map<int> distance;
            private Map<V> parent;
            
            public PathBuilder(Map<Cell> map, Cell type)
            {
                this.map = map;
                var queue = new LinkedList<V>();
                for (int x = 0; x < map.SizeX; x++)
                for (int y = 0; y < map.SizeY; y++)
                {
                    if (map[new V(x, y)] == type)
                        queue.AddLast(new V(x, y));
                }

                distance = new Map<int>(map.SizeX, map.SizeY);
                parent = new Map<V>(map.SizeX, map.SizeY);
                
                while (queue.Any())
                {
                    var v = queue.First();
                    queue.RemoveFirst();
                    
                    for (var direction = 0; direction < 4; direction++)
                    {
                        var u = v.Shift(direction);
                        if (!u.Inside(map) || parent[u] != null || map[u] != Cell.Unknown)
                            continue;

                        parent[u] = v;
                        distance[u] = distance[v] + 1;
                        queue.AddLast(u);
                    }
                }
            }

            public int Distance(V v) => parent[v] == null ? int.MaxValue : distance[v];

            public List<V> GetPath(V to)
            {
                var result = new List<V>();

                while (map[to] == Cell.Unknown)
                {
                    result.Add(to);
                    to = parent[to];
                }

                result.Reverse();
                return result;
            }
        }
    }
}