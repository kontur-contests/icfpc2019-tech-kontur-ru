using System;
using System.Collections.Generic;
using System.Linq;
using lib.Models;
using lib.Models.Actions;
using lib.Solvers.RandomWalk;

namespace lib.Solvers
{
    public class MiningSolver : ISolver
    {
        private List<List<ActionBase>> result;
        private Random random = new Random();
        private bool palka;
        private readonly int limit;

        public MiningSolver(bool palka = true, int limit = 10)
        {
            this.palka = palka;
            this.limit = limit;
        }

        public string GetName()
        {
            return $"mining_{palka}_{limit}";
        }

        public int GetVersion()
        {
            return 1;
        }

        public List<List<ActionBase>> Solve(State state)
        {
            result = new List<List<ActionBase>> { new List<ActionBase>() };

            if (palka)
                BoosterMaster.CreatePalka2(state, result[0]);

            BoosterMaster.CloneAttack(state, result);

            while (state.UnwrappedLeft > 0)
            {
                var workerActions = new List<(Worker worker, ActionBase action)>();

                for (int i = 0; i < state.Workers.Count; i++)
                {
                    var map = state.Map;
                    var me = state.Workers[i];

                    var pathBuilder = new PathBuilder(map, me.Position, state.Workers.Take(i).Select(w => w.Position).ToList(), limit);

                    V best = null;
                    var bestDist = int.MaxValue;

                    for (int y = 0; y < map.SizeY; y++)
                        for (int x = 0; x < map.SizeX; x++)
                        {
                            if (map[new V(x, y)] != CellState.Void)
                                continue;

                            var dist = pathBuilder.Distance(new V(x, y));
                            if (dist == int.MaxValue)
                                continue;

                            if (dist < bestDist)
                            {
                                bestDist = dist;
                                best = new V(x, y);
                            }
                        }

                    var action = best == null ? new Wait() : pathBuilder.GetActions(best).First();
                    workerActions.Add((me, action));
                    result[i].Add(action);
                }

                state.Apply(workerActions);
            }

            return result;
        }

        private class PathBuilder
        {
            private readonly V start;
            private readonly List<V> other;
            private readonly int limit;
            private Queue<V> queue;
            private Map<int> distance;
            private Map<V> parent;

            public PathBuilder(Map map, V start, List<V> other, int limit)
            {
                this.start = start;
                this.other = other;
                this.limit = limit;
                queue = new Queue<V>();
                queue.Enqueue(start);

                distance = new Map<int>(map.SizeX, map.SizeY);
                parent = new Map<V>(map.SizeX, map.SizeY);

                while (queue.Any())
                {
                    var v = queue.Dequeue();
                    if (map[v] == CellState.Void && !TooClose(v))
                        break;

                    foreach (var direction in new[] {Direction.Up, Direction.Left, Direction.Right, Direction.Down})
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

            public int Distance(V v) => parent[v] == null || TooClose(v) ? int.MaxValue : distance[v];

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

            private bool TooClose(V v)
            {
                return other.Any(o => (o - v).MLen() < limit);
            }
        }
    }

}