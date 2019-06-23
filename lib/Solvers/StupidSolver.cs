using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Schema;
using lib.Models;
using lib.Models.Actions;
using lib.Solvers.RandomWalk;

namespace lib.Solvers
{
    public class StupidSolver : ISolver
    {
        private List<List<ActionBase>> result;
        private Random random = new Random();
        private bool palka;

        public StupidSolver(bool palka = true)
        {
            this.palka = palka;
        }

        public string GetName()
        {
            return $"stupid-{palka}";
        }

        public int GetVersion()
        {
            return 3;
        }

        public Solved Solve(State state)
        {
            result = new List<List<ActionBase>> { new List<ActionBase>() };

            if (palka)
                BoosterMaster.CreatePalka(state, result[0], 1);

            BoosterMaster.CloneAttack(state, result);

            var pathBuilder = new PathBuilder(state.Map);

            while (state.UnwrappedLeft > 0)
            {
                var workerActions = new List<(Worker worker, ActionBase action)>();

                for (int i = 0; i < state.Workers.Count; i++)
                {
                    var map = state.Map;
                    var me = state.Workers[i];

                    pathBuilder.Build(map, me.Position, state.Workers.Take(i).Select(w => w.Position).ToList());

                    V best = null;
                    var bestDist = int.MaxValue;

                    for (int x = 0; x < map.SizeX; x++)
                    for (int y = 0; y < map.SizeY; y++)
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

            return new Solved {Actions = result};
        }

        private class PathBuilder
        {
            private V start;
            private List<V> other;
            private Queue<V> queue;
            private Map<int> distance;
            private Map<(V, int)> parent;
            private int timer;

            public PathBuilder(Map map)
            {
                distance = new Map<int>(map.SizeX, map.SizeY);
                parent = new Map<(V, int)>(map.SizeX, map.SizeY);
            }

            private V Parent(V v)
            {
                return parent[v].Item2 == timer ? parent[v].Item1 : null;
            }

            public void Build(Map map, V start, List<V> other)
            {
                this.start = start;
                this.other = other;
                queue = new Queue<V>();
                queue.Enqueue(start);
                timer++;

                while (queue.Any())
                {
                    var v = queue.Dequeue();
                    if (map[v] == CellState.Void && !TooClose(v))
                        break;

                    for (var direction = 0; direction < 4; direction++)
                    {
                        var u = v.Shift(direction);
                        if (!u.Inside(map) || Parent(u) != null || map[u] == CellState.Obstacle)
                            continue;

                        parent[u] = (v, timer);
                        distance[u] = distance[v] + 1;
                        queue.Enqueue(u);
                    }
                }
            }

            public int Distance(V v) => Parent(v) == null || TooClose(v) ? int.MaxValue : distance[v];

            public List<ActionBase> GetActions(V to)
            {
                var result = new List<ActionBase>();

                while (to != start)
                {
                    var from = Parent(to);

                    result.Add(new Move(to - from));

                    to = from;
                }

                result.Reverse();
                return result;
            }

            private bool TooClose(V v)
            {
                return other.Any(o => (o - v).MLen() < 3);
            }
        }
    }
}