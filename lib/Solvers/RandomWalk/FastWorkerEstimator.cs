using System;
using System.Collections.Generic;
using System.Linq;
using lib.Models;

namespace lib.Solvers.RandomWalk
{
    public class FastWorkerEstimator : IFastWorkerEstimator
    {
        public double Estimate(State state, Worker worker)
        {
            if (state.UnwrappedLeft == 0)
                return 1_000_000_000.0 - state.Time;

            var distScore = DistanceToVoid(state.Map, worker.Position);

            return 100_000_000.0 - distScore - state.UnwrappedLeft * 1_000_000.0;
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