using System;
using System.Collections.Generic;
using System.Linq;
using lib.Models;

namespace lib.Solvers.RandomWalk
{
    public class ClusterWorkerEstimator : IEstimator
    {
        private Map<(int value, int version)> distance;
        private Map<(V value, int version)> parent;
        private int currentVersion;

        public string Name => "cluster";

        public double Estimate(State state, Worker worker)
        {
            if (state.UnwrappedLeft == 0)
                return 100_000_000_000.0 - state.Time * 1000.0;

            var level = state.ClustersState.RootLevel;
            var clusterIds = state.ClustersState.RootIds;

            var estimation = 0.0;

            while (true)
            {
                var bestWrapped = int.MinValue;
                var bestClusterIds = new List<int>();
                var sumWrappedSq = 0;
                foreach (var clusterId in clusterIds)
                {
                    var clusterKey = (level, clusterId);
                    var unwrapped = state.ClustersState.Unwrapped[clusterKey];
                    if (unwrapped == 0)
                    {
                        sumWrappedSq += state.ClustersState.WrappedSq[clusterKey];
                        continue;
                    }

                    var wrapped = state.ClustersState.Wrapped[clusterKey];
                    if (wrapped > bestWrapped)
                    {
                        bestWrapped = wrapped;
                        bestClusterIds = new List<int> {clusterId};
                    }
                    else if (wrapped == bestWrapped)
                    {
                        bestClusterIds.Add(clusterId);
                    }
                }

                if (bestClusterIds.Count == 0)
                    throw new InvalidOperationException($"bestClusterIds.Count == 0 at level {level} between clusters {string.Join(", ", clusterIds)}");
                (int clusterId, int dist) bestCluster;
                if (bestClusterIds.Count > 1 || level == 0)
                    bestCluster = FindClosestCluster(state, worker.Position, level, bestClusterIds);
                else
                    bestCluster = (bestClusterIds[0], 0);

                estimation = estimation * 10 + sumWrappedSq;

                if (level == 0)
                {
                    estimation = (estimation + bestWrapped) * 1000.0 - bestCluster.dist;
                    break;
                }

                clusterIds = state.ClustersState.ChildIds[(level, bestCluster.clusterId)];
                level--;
            }

            return estimation;
        }

        private (int clusterId, int dist) FindClosestCluster(State state, V start, int level, List<int> clusterIds)
        {
            if (level != 0 && clusterIds.Contains(state.ClustersState.ClusterIds[start][level]))
                return (state.ClustersState.ClusterIds[start][level], 0);

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
                    if ((level != 0 || map[u] == CellState.Void) && clusterIds.Contains(clusterIdsAtPos[level]))
                        return (clusterIdsAtPos[level], distance[u].value);

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