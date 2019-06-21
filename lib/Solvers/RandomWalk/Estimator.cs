using System;
using System.Collections.Generic;
using System.Linq;
using lib.Models;

namespace lib.Solvers.RandomWalk
{
    public class Estimator : IEstimator
    {
        public double Estimate(State state, State prevState)
        {
            if (state.UnwrappedLeft == 0)
                return 1_000_000_000 - state.Time;

            var distScore = DistanceToVoid(state.Map, state.Worker.Position);
            
            if (state.UnwrappedLeft == prevState.UnwrappedLeft)
                return -distScore;

            //var components = GetComponents(state.Map);
            //var minComponent = components.OrderBy(x => x.Count).First();
            
            // var distToMinComponent = minComponent
            //     .Select(x => pathBuilder.Distance(x))
            //     .Min();
            
            return 100_000_000 - /*components.Count * 100_000*/ distScore - (state.UnwrappedLeft - prevState.UnwrappedLeft) * 100_000;
        }

        private readonly V[] shifts = {"0,1", "1,0", "0,-1", "-1,0"};

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

        private List<List<V>> GetComponents(Map map)
        {
            var result = new List<List<V>>();
            var used = new HashSet<V>();
            for (int x = 0; x < map.SizeX; x++)
            for (int y = 0; y < map.SizeY; y++)
            {
                var start = new V(x, y);
                if (map[start] == CellState.Void && !used.Contains(start))
                {
                    var component = new List<V>();
                    result.Add(component);
                    var queue = new Queue<V>();
                    queue.Enqueue(start);
                    used.Add(start);
                    component.Add(start);
                    while (queue.Any())
                    {
                        var cur = queue.Dequeue();
                        foreach (var shift in shifts)
                        {
                            var next = cur + shift;
                            if (next.Inside(map) && map[next] == CellState.Void)
                            {
                                if (used.Add(next))
                                {
                                    queue.Enqueue(next);
                                    component.Add(next);
                                }
                            }
                        }
                    }
                }
            }

            return result;
        }
    }
}
