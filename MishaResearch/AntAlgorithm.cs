using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MishaResearch
{
    class AntAlgorithm
    {
        [ThreadStatic] private static Random random;
        public static (List<int>, int) GetPath(Dictionary<(int, int), int> distances, int start, List<int> end, int iterations)
        {
            var nodes = distances.Keys.Select(pair => pair.Item1).Distinct().ToList();
            {
                var queue = new PriorityQueue<(List<int> path, int distance)>();
                int it = 0;
                queue.Enqueue(0, (new List<int> { start }, 0));
                while (queue.Count > 0 && it < 100000)
                {
                    var next = queue.Dequeue();
                    if (next.path.Count == nodes.Count)
                        return (next.path, next.distance);
                    foreach (var node in nodes.Where(n => !next.path.Contains(n)))
                    {
                        var dist = next.distance + distances[(next.path[next.path.Count - 1], node)];
                        queue.Enqueue(-dist, (next.path.Concat(new[] { node }).ToList(), dist));
                    }

                    it++;
                }
            }

            if (random == null)
                random = new Random();
            List<int> bestPath = null;
            int bestDistance = int.MaxValue;
            var pheromones = distances.ToDictionary(pair => pair.Key, _ => 1.0);
            if (nodes.Count < 3)
                iterations = 1;
            
            for (int i = 0; i < iterations; i++)
            {
                HashSet<int> visited = new HashSet<int>();
                List<int> path = new List<int>();
                int score = 0;

                var pos = start;
                visited.Add(pos);
                path.Add(pos);
                while (nodes.Count != visited.Count)
                {
                    var notVisited = nodes.Where(node => !visited.Contains(node)).ToList();
                    var cumsum = 0.0;
                    var probas = notVisited.Select(node => cumsum += pheromones[(pos, node)]).ToArray();
                    var choosedIdx = Array.BinarySearch(probas, random.NextDouble()*probas[probas.Length - 1]);
                    var newPos = notVisited[choosedIdx < 0 ? ~choosedIdx : choosedIdx];
                    score += distances[(pos, newPos)];
                    pos = newPos;
                    visited.Add(pos);
                    path.Add(pos);
                }

                if (end.Count > 0 && !end.Contains(pos))
                {
                    var pathend = (end.OrderBy(p => distances[(pos, p)]).First());
                    score += distances[(pos, pathend)];
                }

                if (score < bestDistance)
                {
                    bestPath = path;
                    bestDistance = score;
                }

                foreach (var pair in pheromones.Keys.ToList())
                {
                    pheromones[pair] = Math.Max(pheromones[pair]*0.99, 0.01);
                }

                for (int j = 0; j < path.Count-1; j++)
                {
                    pheromones[(path[j], path[j+1])] += nodes.Count * 1.0 / path.Count;
                }
            }

            return (bestPath, bestDistance);
        }
    }
}
