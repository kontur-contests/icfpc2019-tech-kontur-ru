using System.Collections.Generic;
using System.Linq;
using lib.Models;
using lib.Models.Actions;

namespace lib.Puzzles
{
    public class MstPuzzleSolver : IPuzzleSolver
    {
        public Problem Solve(Puzzle puzzle)
        {
            var map = SolveInner(puzzle);
            
            while (true)
            {
                var countIn = Inside(map).Count;
                var countMin = (int) 0.2 * puzzle.TaskSize * puzzle.TaskSize + 10;

                if (countIn < countMin)
                {
                    Add(map, countMin - countIn);
                    continue;
                }

                break;
            }

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

        private void Add(Map<bool> map, int need)
        {
            for (int x = 0; x < map.SizeX && need > 0; x++)
            for (int y = 0; y < map.SizeY && need > 0; y++)
            {
                var v = new V(x, y);
                if (OnBound(map, v))
                {
                    map[v] = true;
                    need--;
                }
            }
        }

        private bool OnBound(Map<bool> map, V v)
        {
            if (!v.Inside(map) || map[v])
                return false;

            for (int d = 0; d < 4; d++)
            {
                var u = v.Shift(d);
                if (u.Inside(map) && map[u])
                    return true;
            }

            return false;
        }

        private List<V> Inside(Map<bool> map)
        {
            var result = new List<V>();
            for (int x = 0; x < map.SizeX; x++)
                for (int y = 0; y < map.SizeY; y++)
                    if (map[new V(x, y)])
                        result.Add(new V(x, y));
            return result;
        }

        public Map<bool> SolveInner(Puzzle puzzle)
        {
            var outside = puzzle.MustContainPoints.ToList();
            var map = new Map<bool>(puzzle.TaskSize, puzzle.TaskSize);

            if (outside.Any())
            {
                map[outside[0]] = true;
                outside.RemoveAt(0);
            }

            while (outside.Any())
            {
                var pathBuilder = new PathBuilder(map, puzzle.MustNotContainPoints);

                var best = 0;
                for (int i = 1; i < outside.Count; i++)
                    if (pathBuilder.Distance(outside[i]) < pathBuilder.Distance(outside[best]))
                        best = i;

                var path = pathBuilder.GetPath(outside[best]);
                foreach (var x in path)
                    map[x] = true;

                outside.RemoveAt(best);
            }

            return map;
        }

        private class PathBuilder
        {
            private readonly Map<bool> map;
            private Map<int> distance;
            private Map<V> parent;
            
            public PathBuilder(Map<bool> map, List<V> outersList)
            {
                this.map = map;
                var queue = new LinkedList<V>();
                for (int x = 0; x < map.SizeX; x++)
                for (int y = 0; y < map.SizeY; y++)
                {
                    if (map[new V(x, y)])
                        queue.AddLast(new V(x, y));
                }

                distance = new Map<int>(map.SizeX, map.SizeY);
                parent = new Map<V>(map.SizeX, map.SizeY);
                var outers = new Map<bool>(map.SizeX, map.SizeY);
                foreach (var x in outersList)
                    outers[x] = true;

                while (queue.Any())
                {
                    var v = queue.First();
                    queue.RemoveFirst();
                    
                    for (var direction = 0; direction < 4; direction++)
                    {
                        var u = v.Shift(direction);
                        if (!u.Inside(map) || parent[u] != null || outers[u] || map[u])
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

                while (!map[to])
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