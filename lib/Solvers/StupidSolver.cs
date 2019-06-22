using System.Collections.Generic;
using System.Linq;
using lib.Models;
using lib.Models.Actions;
using lib.Solvers.RandomWalk;

namespace lib.Solvers
{
    public class StupidSolver : ISolver
    {
        private List<ActionBase> result;

        public string GetName()
        {
            return "stupid";
        }

        public int GetVersion()
        {
            return 1;
        }

        public List<List<ActionBase>> Solve(State state)
        {
            result = new List<ActionBase>();

            BoosterMaster.CreatePalka(state, result);

            while (true)
            {
                var map = state.Map;
                var me = state.SingleWorker;

                var pathBuilder = new PathBuilder(map, me.Position, true);

                V best = null;
                var bestDist = int.MaxValue;
                var bestSize = double.MaxValue;

                var (comp, csize) = ComponentBuilder.Build(map, me.Position);
                
                for (int x = 0; x < map.SizeX; x++)
                for (int y = 0; y < map.SizeY; y++)
                {
                    var p = new V(x, y);
                    if (map[p] != CellState.Void)
                        continue;

                    var dist = pathBuilder.Distance(p);
                    var size = csize[comp[p]];
                    if (size < bestSize || size == bestSize && dist < bestDist)
                    {
                        bestDist = dist;
                        bestSize = size;
                        best = p;
                    }
                }

                if (best == null)
                    break;

                var actions = pathBuilder.GetActions(best).ToList();
                state.ApplyRange(actions);
                result.AddRange(actions);
            }

            return new List<List<ActionBase>> {result};
        }

        private static class ComponentBuilder
        {
            public static (Map<int> comp, Dictionary<int, double> size) Build(Map map, V me)
            {
                var dists = new Dictionary<int, List<double>>();
                var comp = new Map<int>(map.SizeX, map.SizeY);

                int id = 0;

                for (int x = 0; x < map.SizeX; x++)
                for (int y = 0; y < map.SizeY; y++)
                {
                    var v = new V(x, y);
                    if (map[v] != CellState.Void || comp[v] != 0)
                        continue;

                    id++;
                    dists[id] = new List<double>();

                    var queue = new Queue<V>();
                    queue.Enqueue(v);

                    while (queue.Any())
                    {
                        v = queue.Dequeue();
                        comp[v] = id;
                        dists[id].Add((me - v).MLen());

                        for (var direction = 0; direction < 4; direction++)
                        {
                            var u = v.Shift(direction);
                            if (!u.Inside(map) || map[u] != CellState.Void || comp[u] != 0)
                                continue;

                            comp[u] = id;
                            queue.Enqueue(u);
                        }
                    }
                }

                var size = new Dictionary<int, double>();
                foreach (var k in dists.Keys)
                {
                    var top = dists[k].OrderBy(x => x).Take(10).ToList();
                    size[k] = top.Average();
                }
                return (comp, size);
            }
        }

        private class PathBuilder
        {
            private readonly V start;
            private Queue<V> queue;
            private Map<int> distance;
            private Map<V> parent;

            public PathBuilder(Map map, V start, bool stop)
            {
                this.start = start;
                queue = new Queue<V>();
                queue.Enqueue(start);

                distance = new Map<int>(map.SizeX, map.SizeY);
                parent = new Map<V>(map.SizeX, map.SizeY);

                while (queue.Any())
                {
                    var v = queue.Dequeue();
                    //if (map[v] == CellState.Void && stop)
                    //    break;

                    for (var direction = 0; direction < 4; direction++)
                    {
                        var u = v.Shift(direction);
                        if (!u.Inside(map) || parent[u] != null || map[u] == CellState.Obstacle)
                            continue;

                        parent[u] = v;
                        distance[u] = distance[v] + 1;
                        queue.Enqueue(u);
                    }
                }
            }

            public int Distance(V v) => parent[v] == null ? int.MaxValue : distance[v];

            public List<ActionBase> GetActions(V to)
            {
                var result = new List<ActionBase>();

                while (to != start)
                {
                    var from = parent[to];

                    result.Add(new Move(to - from));

                    to = from;
                }

                result.Reverse();
                return result;
            }
        }
    }
}