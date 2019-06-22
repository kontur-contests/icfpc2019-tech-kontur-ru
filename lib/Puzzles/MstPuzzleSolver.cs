using System.Collections.Generic;
using System.Linq;
using lib.Models;
using lib.Models.Actions;

namespace lib.Puzzles
{
    internal class MstPuzzleSolver : IPuzzleSolver
    {
        public Map<bool> Solve(Puzzle puzzle)
        {
            var outside = puzzle.MustContainPoints.ToList();
            var map = new Map<bool>(puzzle.TaskSize, puzzle.TaskSize);

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