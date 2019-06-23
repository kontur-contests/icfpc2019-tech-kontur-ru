using System;
using System.Collections.Generic;
using System.Linq;
using lib.Models;

namespace lib.Solvers.RandomWalk
{
    public class FastWorkerEstimator : IFastWorkerEstimator
    {
        private readonly bool collectFastWheels;
        private Map<(int value, int version)> distance;
        private Map<(V value, int version)> parent;
        private int currentVersion;

        public FastWorkerEstimator(bool collectFastWheels = false)
        {
            this.collectFastWheels = collectFastWheels;
        }

        public double Estimate(State state, Worker worker)
        {
            if (state.UnwrappedLeft == 0)
                return 1_000_000_000.0 - state.Time;

            var distScore = DistanceToVoid(state.Map, worker.Position);

            var fastWheelsBonus = collectFastWheels ? state.Workers.Sum(w => w.FastWheelsTimeLeft) + state.FastWheelsCount * Constants.FastWheelsTime * 1_000_000.0 : 0;
            return 100_000_000.0 + fastWheelsBonus- distScore - state.UnwrappedLeft * 1_000_000.0;
        }

        private int DistanceToVoid(Map map, V start)
        {
            var queue = new Queue<V>();
            queue.Enqueue(start);

            Init(map);

            while (queue.Count > 0)
            {
                var v = queue.Dequeue();

                for (var direction = 0; direction < 4; direction++)
                {
                    var u = v.Shift(direction);
                    if (!u.Inside(map) || parent[u].version == currentVersion || map[u] == CellState.Obstacle)
                        continue;

                    parent[u] = (v, currentVersion);
                    var dv = distance[v];
                    distance[u] = (dv.version == currentVersion ? dv.value + 1 : 1, currentVersion);
                    if (map[u] == CellState.Void)
                        return distance[u].value;

                    queue.Enqueue(u);
                }
            }

            throw new InvalidOperationException();
        }

        private void Init(Map map)
        {
            currentVersion++;
            if (distance == null || distance.SizeX != map.SizeX || distance.SizeY != map.SizeY)
            {
                distance = new Map<(int, int)>(map.SizeX, map.SizeY);
                parent = new Map<(V, int)>(map.SizeX, map.SizeY);
            }
        }
    }
}