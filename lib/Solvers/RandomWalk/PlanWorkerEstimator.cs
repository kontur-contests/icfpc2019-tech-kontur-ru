using System;
using System.Collections.Generic;
using lib.Models;

namespace lib.Solvers.RandomWalk
{
    public class PlanWorkerEstimator
    {
        private Map<(int value, int version)> distance;
        private Map<(V value, int version)> parent;
        private int currentVersion;

        public double Estimate(State state, Worker worker, int clusterId)
        {
            if (state.UnwrappedLeft == 0)
                return 100_000_000_000.0 - state.Time * 1000.0;

            if (state.ClustersState.Unwrapped[(0, clusterId)] == 0)
                return 100_000_000.0 - state.Time * 1000.0;
            
            var clusterFillScore = state.ClustersState.Wrapped[(0, clusterId)];
            var clusterDistScore = GetDistToCluster(state, worker.Position, clusterId);

            return clusterFillScore * 1_000.0
                   - clusterDistScore * 1.0
                   - state.UnwrappedLeft * 0.01;
        }

        private int GetDistToCluster(State state, V start, int clusterId)
        {
            var queue = new Queue<V>();
            queue.Enqueue(start);

            var map = state.Map;
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

                    var clusterIdsAtPos = state.ClustersState.ClusterIds[u];
                    if (map[u] == CellState.Void && clusterIdsAtPos[0] == clusterId)
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