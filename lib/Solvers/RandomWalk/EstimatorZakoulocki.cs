using System;
using System.Collections.Generic;
using System.Linq;
using lib.Models;

namespace lib.Solvers.RandomWalk
{
    public class EstimatorZakoulocki : IEstimator
    {
        public string Name => "zako";
        public double Estimate(State state, Worker worker)
        {
            if (state.UnwrappedLeft == 0)
                return 1_000_000_000 - state.Time;
            var distScore = DistanceToVoid(state.Map, worker.Position);

            var unwrappedCost = state.Map.EnumerateCells().Where(c => c.state == CellState.Void).Sum(c => CellCost(c.pos, state.Map));

            return -distScore - unwrappedCost * 100_000;
        }
        private static V[] dirs = new V[] { "1,0", "-1,0", "0,1", "0,-1"};//, "2,0", "-2,0", "0,2", "0,-2" };
        private static double[] weights = new[] {4, 3, 2, 1, 1.0};
        private double CellCost(V pos, Map map)
        {
            var freeNs = 0;
            foreach (var dir in dirs)
            {
                var n = dir + pos;
                if (n.Inside(map) && map[n] == CellState.Void) freeNs++;
            }
            return weights[freeNs];
        }

        private int DistanceToVoid(Map map, V start)
        {
            var queue = new Queue<V>();
            queue.Enqueue(start);

            var distance = new Map<int>(map.SizeX, map.SizeY);
            var parent = new Map<V>(map.SizeX, map.SizeY);

            while (queue.Any())
            {
                var v = queue.Dequeue();

                for (var direction = 0; direction < 4; direction++)
                {
                    var u = v.Shift(direction);
                    if (!u.Inside(map) || parent[u] != null || map[u] == CellState.Obstacle)
                        continue;

                    parent[u] = v;
                    distance[u] = distance[v] + 1;
                    if (map[u] == CellState.Void)
                        return distance[u];

                    queue.Enqueue(u);
                }
            }
            throw new InvalidOperationException();
        }
    }
}