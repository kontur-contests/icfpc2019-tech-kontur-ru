using System;
using System.Collections.Generic;
using System.Linq;

namespace lib.Models
{
    public static class ProblemConverter
    {
        public static State ToState(this Problem problem)
        {
            var state = new State(
                new Worker
                {
                    Position = problem.Point,
                    Manipulators = new List<V> {new V(1, 0), new V(1, 1), new V(1, -1)}
                },
                ConvertMap(problem.Map, problem.Obstacles),
                problem.Boosters,
                0,
                null);
            state.Wrap();
            return state;
        }

        private static Map ConvertMap(List<V> problemMap, List<List<V>> problemObstacles)
        {
            var map = new Map(problemMap.Max(x => x.X), problemMap.Max(x => x.Y));

            var segments = new Dictionary<int, List<(int min, int max)>>();
            foreach (var polygon in problemObstacles.Concat(new[] {problemMap}))
            {
                for (int i = 0; i < polygon.Count; i++)
                {
                    var a = polygon[i];
                    var b = polygon[(i + 1) % polygon.Count];
                    if (a.X == b.X)
                    {
                        var list = segments.GetOrAdd(a.X, x => new List<(int min, int max)>());
                        list.Add((Math.Min(a.Y, b.Y), Math.Max(a.Y, b.Y)));
                    }
                }
            }

            for (int y = 0; y < map.SizeY; y++)
            {
                bool inside = false;
                for (int x = 0; x < map.SizeX; x++)
                {
                    if (segments.TryGetValue(x, out var segs) && segs.Any(s => s.min <= y && s.max > y))
                        inside = !inside;

                    map[new V(x, y)] = inside ? CellState.Void : CellState.Obstacle;
                }
            }
            
            return map;
        }
    }
}