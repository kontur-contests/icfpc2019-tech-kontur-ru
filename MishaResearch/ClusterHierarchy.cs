using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using lib;
using MongoDB.Driver.Core.Clusters.ServerSelectors;

namespace MishaResearch
{
    class ClusterHierarchy
    {
        public int Id;
        public int Level;
        public List<V> Points = new List<V>();
        public Dictionary<int, ClusterHierarchy> Childs = new Dictionary<int, ClusterHierarchy>();
        public Dictionary<int, ClusterHierarchy> Edges = new Dictionary<int, ClusterHierarchy>();
        public ClusterHierarchy Parent;

        private ClusterHierarchy(int id, int level)
        {
            Id = id;
            Level = level;
        }

        public ClusterHierarchy(List<ClusterRecord> records)
        {
            Level = records[0].cluster_hierarchy.Length;

            var near = new V[] { new V(0, 1), new V(0, -1), new V(-1, 0), new V(1, 0) };
            var points = records.ToDictionary(r => new V(r.X, r.Y));
            var levelIdToCluster = new ConcurrentDictionary<(int, int), ClusterHierarchy>();

            foreach (var record in records)
            {
                ClusterHierarchy parentCluster = this;
                var point = new V(record.X, record.Y);
                for (int i = record.cluster_hierarchy.Length - 1; i >= 0; i--)
                {
                    var cluster = levelIdToCluster.GetOrAdd((i, record.cluster_hierarchy[i]), _ => new ClusterHierarchy(record.cluster_hierarchy[i], i));
                    cluster.Parent = parentCluster;
                    cluster.Points.Add(point);
                    parentCluster.Childs[cluster.Id] = cluster;
                    foreach (var n in near)
                    {
                        var adjancent = point + n;
                        if(!points.TryGetValue(adjancent, out var adjNode))
                            continue;
                        var adjCluster = levelIdToCluster.GetOrAdd(
                            (i, adjNode.cluster_hierarchy[i]), 
                            _ => new ClusterHierarchy(adjNode.cluster_hierarchy[i], i));
                        cluster.Edges[adjCluster.Id] = adjCluster;
                    }
                    parentCluster = cluster;
                }
            }
        }

        public Dictionary<(int, int), int> DistanceBetweenClusters = new Dictionary<(int, int), int>();

        public void CalculateDistancesBetweenChilds()
        {
            if(Childs.Count == 0)
                return;
            var inComponentHierarchy = new HashSet<ClusterHierarchy>();
            var queue = new Queue<ClusterHierarchy>();
            queue.Enqueue(Childs.First().Value);
            inComponentHierarchy.Add(queue.Peek());
            while (queue.Count != 0)
            {
                var node = queue.Dequeue();
                foreach (var pair in node.Edges
                    .Where(edge => Childs.ContainsKey(edge.Key) && !inComponentHierarchy.Contains(edge.Value)))
                {
                    queue.Enqueue(pair.Value);
                    inComponentHierarchy.Add(node);
                }
            }

            foreach (var pair in Childs.Where(pair => !inComponentHierarchy.Contains(pair.Value)))
                BuildDistances(pair.Value, Childs.Values.Where(c => c != pair.Value).ToHashSet());

            var component = inComponentHierarchy.ToList();

            for (var k = 0; k < component.Count; k++)
            {
                for (var i = 0; i < component.Count; i++)
                {
                    for (var j = 0; j < component.Count; j++)
                    {
                        if (component[i].Edges.ContainsKey(component[j].Id))
                            DistanceBetweenClusters[(component[i].Id, component[j].Id)] = 1;
                        else
                            DistanceBetweenClusters[(component[i].Id, component[j].Id)] = Math.Min(
                                DistanceBetweenClusters.GetValueOrDefault((component[i].Id, component[j].Id), i == j ? 0 : 10000),
                                DistanceBetweenClusters.GetValueOrDefault((component[i].Id, component[k].Id), i == k ? 0 : 10000) +
                                DistanceBetweenClusters.GetValueOrDefault((component[k].Id, component[j].Id), j == k ? 0 : 10000));
                    }
                }
            }

            foreach (var pair in DistanceBetweenClusters.Keys.ToList())
            {
                if (pair.Item1 == pair.Item2)
                    DistanceBetweenClusters.Remove(pair);
            }

            foreach (var child in Childs)
            {
                child.Value.CalculateDistancesBetweenChilds();
            }
            if(DistanceBetweenClusters.Count == 4 )
                Console.WriteLine("");
        }

        private void BuildDistances(ClusterHierarchy node, HashSet<ClusterHierarchy> aims)
        {
            var visited = new HashSet<ClusterHierarchy>();
            var queue = new Queue<(ClusterHierarchy cluster, int dist)>();
            queue.Enqueue((node, 0));
            while (aims.Count != 0)
            {
                var next = queue.Dequeue();
                if (aims.Contains(next.cluster))
                {
                    aims.Remove(next.cluster);
                    DistanceBetweenClusters[(node.Id, next.cluster.Id)] = next.dist;
                    DistanceBetweenClusters[(next.cluster.Id, node.Id)] = next.dist;
                }
                foreach (var pair in next.cluster.Edges
                    .Where(edge => !visited.Contains(edge.Value)))
                {
                    queue.Enqueue((pair.Value, next.dist + 1));
                    visited.Add(node);
                }
            }
        }

        public List<int> Path;
        public int Distance;

        public List<ClusterHierarchy> BuildPath(int[] startPointClusters, ClusterHierarchy[] previousEndPointClusters, List<int> endClusters)
        {
            if (Childs.Count == 0)
                return new List<ClusterHierarchy> { this };

            var startCluster = Childs.ContainsKey(startPointClusters[Level-1]) 
                ? startPointClusters[Level-1] 
                : GetConnectingSubClusters(previousEndPointClusters[Level], this).First();
            if (DistanceBetweenClusters.Count <= 3)
            {
                Path = Childs.Keys.OrderByDescending(i => i == startCluster).ToList();
                Distance = Path.Count - 1;
            }
            else
                (Path, Distance) = AntAlgorithm.GetPath(DistanceBetweenClusters, startCluster, endClusters, 100);
            var result = new List<ClusterHierarchy>();
            for (var index = 0; index < Path.Count; index++)
            {
                var ends = index == Path.Count - 1 ? new List<int>() : GetConnectingSubClusters(Childs[Path[index + 1]], Childs[Path[index]]);

                var idx = Path[index];
                var subPath = Childs[idx]
                    .BuildPath(
                        startPointClusters,
                        previousEndPointClusters,
                        ends);
                result.AddRange(subPath);
                previousEndPointClusters = result[result.Count - 1].GetParentHierarchy();
            }

            return result;
        }

        public ClusterHierarchy[] GetParentHierarchy()
        {
            var res = new List<ClusterHierarchy> { this };
            var node = Parent;
            while (node != null)
            {
                res.Add(node);
                node = node.Parent;
            }

            return res.ToArray();
        }

        public List<int> GetConnectingSubClusters(ClusterHierarchy cluster1, ClusterHierarchy aim)
        {
            var queue = cluster1.Childs.Values.ToList();
            if(queue.Count == 0)
                return new List<int>();
            var visited = new HashSet<ClusterHierarchy>(queue);
            while(queue.All(c => c.Parent != aim))
            {
                queue = queue.SelectMany(c => c.Edges.Values).Where(c => visited.Add(c)).ToList();
            }

            return queue.Where(c => c.Parent == aim).Select(c => c.Id).ToList();
        }
    }

    class ClusterRecord
    {
        public int Id;
        public int X;
        public int Y;
        public int[] cluster_hierarchy;
    }
}
