using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Schema;
using lib.Models;
using lib.Models.Actions;

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

            CollectManipulators(state);

            while (true)
            {
                var map = state.Map;
                var me = state.SingleWorker;

                var pathBuilder = new PathBuilder(map, me.Position, true);

                V best = null;
                var bestDist = int.MaxValue;

                for (int x = 0; x < map.SizeX; x++)
                for (int y = 0; y < map.SizeY; y++)
                {
                    if (map[new V(x, y)] != CellState.Void)
                        continue;

                    var dist = pathBuilder.Distance(new V(x, y));
                    if (dist < bestDist)
                    {
                        bestDist = dist;
                        best = new V(x, y);
                    }
                }

                if (best == null)
                    break;

                var actions = pathBuilder.GetActions(best).Take(1).ToList();
                state.ApplyRange(actions);
                result.AddRange(actions);
            }

            return new List<List<ActionBase>> {result};
        }

        private void CollectManipulators(State state)
        {
            var k = 0;

            while (true)
            {
                var boosters = state.Boosters.Where(b => b.Type == BoosterType.Extension).ToList();

                if (!boosters.Any())
                    return;

                var map = state.Map;
                var me = state.SingleWorker;
                var pathBuilder = new PathBuilder(map, me.Position, false);

                var best = boosters.OrderBy(b => pathBuilder.Distance(b.Position)).First();

                var actions = pathBuilder.GetActions(best.Position);

                var y = k / 2 + 2;
                y = k % 2 == 0 ? -y : y;
                var add = new UseExtension(new V(1, y));
                k++;

                actions.Add(add);

                state.ApplyRange(actions);
                result.AddRange(actions);
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
                    if (map[v] == CellState.Void && stop)
                        break;

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