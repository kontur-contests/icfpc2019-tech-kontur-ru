using System;
using System.Collections.Generic;
using System.Linq;

namespace lib.Models
{
    public class ClustersState
    {
        public ClustersState(ClusterSourceLine[] lines, State state)
        {
            ClusterIds = new Map<int[]>(state.Map.SizeX, state.Map.SizeY);
            Unwrapped = new Dictionary<(int level, int clusterId), int>();
            Wrapped = new Dictionary<(int level, int clusterId), int>();
            WrappedSq = new Dictionary<(int level, int clusterId), int>();
            ChildIds = new Dictionary<(int level, int clusterId), HashSet<int>>();
            RootIds = new HashSet<int>();
            Path = null;
            
            RootLevel = lines[0].cluster_hierarchy.Length - 1;

            foreach (var line in lines)
            {
                var p = new V(line.X, line.Y);
                ClusterIds[p] = line.cluster_hierarchy;
                if (state.Map[p] != CellState.Obstacle)
                {
                    for (int level = 0; level < line.cluster_hierarchy.Length; level++)
                    {
                        var clusterId = line.cluster_hierarchy[level];
                        var clusterKey = (level, clusterId);

                        if (!Unwrapped.ContainsKey(clusterKey))
                            Unwrapped[clusterKey] = 0;
                        if (!Wrapped.ContainsKey(clusterKey))
                            Wrapped[clusterKey] = 0;
                        if (!WrappedSq.ContainsKey(clusterKey))
                            WrappedSq[clusterKey] = 0;

                        if (level == 0)
                        {
                            if (state.Map[p] == CellState.Void)
                                Unwrapped[clusterKey]++;
                            else
                                Wrapped[clusterKey]++;
                        }
                        
                        if (state.Map[p] == CellState.Wrapped)
                            WrappedSq[clusterKey]++;

                        if (level == line.cluster_hierarchy.Length - 1)
                            RootIds.Add(clusterId);
                        else
                        {
                            var parentKey = (level + 1, line.cluster_hierarchy[level + 1]);
                            if (!ChildIds.ContainsKey(parentKey))
                                ChildIds[parentKey] = new HashSet<int>();
                            ChildIds[parentKey].Add(clusterId);
                        }
                    }
                }
            }

            for (int level = 1; level < lines[0].cluster_hierarchy.Length; level++)
            {
                foreach (var kvp in Unwrapped.Where(x => x.Key.level == level).ToList())
                    Unwrapped[kvp.Key] = ChildIds[kvp.Key].Count(childId => Unwrapped[(level - 1, childId)] > 0);

                foreach (var kvp in Wrapped.Where(x => x.Key.level == level).ToList())
                    Wrapped[kvp.Key] = ChildIds[kvp.Key].Count - Unwrapped[kvp.Key];
            }
        }

        public HashSet<int> RootIds { get; set; }
        public int RootLevel { get; set; }
        public Dictionary<(int level, int clusterId), HashSet<int>> ChildIds { get; set; }

        public Map<int[]> ClusterIds { get; set; }
        public Dictionary<(int level, int clusterId), int> Unwrapped { get; set; }
        public Dictionary<(int level, int clusterId), int> Wrapped { get; set; }
        public List<int> Path { get; set; }
        
        public Dictionary<(int level, int clusterId), int> WrappedSq { get; set; }

        public void Wrap(V p)
        {
            var clusterIds = ClusterIds[p];
            for (int level = 0; level < clusterIds.Length; level++)
                WrappedSq[(level, clusterIds[level])]++;
            
            for (int level = 0; level < clusterIds.Length; level++)
            {
                Unwrapped[(level, clusterIds[level])]--;
                Wrapped[(level, clusterIds[level])]++;
                if (Unwrapped[(level, clusterIds[level])] == 0)
                    continue;
                break;
            }
        }

        public void Unwrap(V p)
        {
            var clusterIds = ClusterIds[p];
            for (int level = 0; level < clusterIds.Length; level++)
                WrappedSq[(level, clusterIds[level])]--;
            
            for (int level = 0; level < clusterIds.Length; level++)
            {
                Unwrapped[(level, clusterIds[level])]++;
                Wrapped[(level, clusterIds[level])]--;
                if (Unwrapped[(level, clusterIds[level])] == 1)
                    continue;
                break;
            }
        }
    }
}