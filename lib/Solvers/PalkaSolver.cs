using System;
using System.Collections.Generic;
using System.Linq;
using lib.Models;
using lib.Models.Actions;
using lib.Solvers.RandomWalk;

namespace lib.Solvers
{
    public class PalkaSolver : ISolver
    {
        private List<ActionBase> result;
        private int[] MinFreeY;

        public string GetName()
        {
            return "palka";
        }

        public int GetVersion()
        {
            return 1;
        }

        public List<List<ActionBase>> Solve(State state)
        {
            result = new List<ActionBase>();

            BoosterMaster.CreatePalka(state, result, 1);
            var map = state.Map;

            MinFreeY = Enumerable.Repeat(int.MaxValue, state.Map.SizeX).ToArray();
            for (int x = 0; x < map.SizeX; x++)
            for (int y = 0; y < map.SizeY; y++)
            {
                if (map[new V(x, y)] == CellState.Void)
                    MinFreeY[x] = Math.Min(MinFreeY[x], y);
            }
            state.OnWrap = v =>
            {
                if (v.Y == MinFreeY[v.X])
                {
                    MinFreeY[v.X] = int.MaxValue;
                    for (int y = v.Y + 1; y < map.SizeY; y++)
                    {
                        if (map[new V(v.X, y)] == CellState.Void)
                        {
                            MinFreeY[v.X] = y;
                            break;
                        }
                    }
                }
            };

            int palkaDown = -state.SingleWorker.Manipulators.Min(m => m.Y);

            while (state.UnwrappedLeft > 0)
            {
                if (result.Count == 213)
                    palkaDown = palkaDown;

                for (int check = 0; check < 2; check++)
                {
                    var me = state.SingleWorker;

                    var pathBuilder = new PathBuilder(map, me.Position, MinFreeY, palkaDown, check == 0);

                    V best = null;
                    var bestDist = int.MaxValue;
                    var bestY = int.MaxValue;

                    for (int y = 0; y < map.SizeY; y++)
                    for (int x = 0; x < map.SizeX; x++)
                    {
                        if (map[new V(x, y)] != CellState.Void)
                            continue;

                        var dist = pathBuilder.Distance(new V(x, y));
                        if (dist == int.MaxValue)
                            continue;

                        if (check == 0 && (dist < bestDist /*&& bestY >= y || bestY > y*/)
                            || check == 1 && (dist < bestDist && bestY >= y || bestY > y))
                        {
                            bestDist = dist;
                            bestY = y;
                            best = new V(x, y);
                        }
                    }

                    if (best == null)
                        continue;

                    var actions = pathBuilder.GetActions(best).Take(1).ToList();
                    state.ApplyRange(actions);
                    result.AddRange(actions);
                    break;
                }
            }

            return new List<List<ActionBase>> { result };
        }

        private class PathBuilder
        {
            private readonly V start;
            private Queue<V> queue;
            private Map<int> distance;
            private Map<V> parent;

            public PathBuilder(Map map, V start, int[] minFreeY, int palkaDown, bool check)
            {
                this.start = start;
                queue = new Queue<V>();
                queue.Enqueue(start);

                distance = new Map<int>(map.SizeX, map.SizeY);
                parent = new Map<V>(map.SizeX, map.SizeY);

                while (queue.Any())
                {
                    var v = queue.Dequeue();
                    //if (map[v] == CellState.Void && check)
                    //    break;

                    for (var direction = 0; direction < 4; direction++)
                    {
                        var u = v.Shift(direction);
                        if (!u.Inside(map) || parent[u] != null || map[u] == CellState.Obstacle)
                            continue;
                        if (check && (Direction)direction == Direction.Up && !AllowedPosition(map, u, minFreeY, palkaDown))
                            continue;

                        parent[u] = v;
                        distance[u] = distance[v] + 1;
                        queue.Enqueue(u);
                    }
                }
            }

            private bool AllowedPosition(Map map, V v, int[] minFreeY, int palkaDown)
            {
                int x = v.X, y = v.Y;
                for (int i = 0; i < palkaDown; i++)
                {
                    var to = new V(x, y - 1);
                    if (!to.Inside(map) || map[to] == CellState.Obstacle)
                        break;
                    y--;
                }

                return y <= minFreeY[x];
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