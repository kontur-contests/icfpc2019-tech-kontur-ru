using System.Collections.Generic;
using System.Linq;
using lib.Models;
using lib.Models.Actions;

namespace lib.Solvers.RandomWalk
{
    public static class BoosterMaster
    {
        public static void CloneAttack(State state, List<ActionBase> result)
        {
            if (state.Boosters.All(b => b.Type != BoosterType.MysteriousPoint))
                return;

            while (true)
            {
                var boosters = state.Boosters.Where(b => b.Type == BoosterType.Cloning).ToList();

                if (!boosters.Any())
                    break;

                var map = state.Map;
                var me = state.SingleWorker;
                var pathBuilder = new PathBuilder(map, me.Position, false);
                var best = boosters.OrderBy(b => pathBuilder.Distance(b.Position)).First();

                var actions = pathBuilder.GetActions(best.Position);

                state.ApplyRange(actions);
                result.AddRange(actions);
            }

            if (state.CloningCount == 0)
                return;

            var mboosters = state.Boosters.Where(b => b.Type == BoosterType.MysteriousPoint).ToList();
            var mpathBuilder = new PathBuilder(state.Map, state.SingleWorker.Position, false);
            var mbest = mboosters.OrderBy(b => mpathBuilder.Distance(b.Position)).First();
            var mactions = mpathBuilder.GetActions(mbest.Position);

            state.ApplyRange(mactions);
            result.AddRange(mactions);

            while (state.CloningCount > 0)
            {
                var cactions = new List<(Worker w, ActionBase action)>();
                foreach (var w in state.Workers)
                {
                    if (cactions.Count < state.CloningCount)
                        cactions.Add((w, new UseCloning()));
                    else
                        cactions.Add((w, new Wait()));
                }

                state.Apply(cactions);
                result.AddRange(cactions.Select(x => x.action));
            }
        }

        public static void CreatePalka(State state, List<ActionBase> result, int type = 0)
        {
            var k = type == 0 ? 0 : 1;

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

                if (type == 0)
                    k++;
                else
                    k += 2;

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