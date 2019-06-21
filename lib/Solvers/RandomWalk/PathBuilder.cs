using System.Collections.Generic;
using System.Linq;
using lib.Models;

namespace lib.Solvers.RandomWalk
{
    public class PathBuilder
    {
        private readonly V start;
        private Queue<V> queue;
        private Map<int> distance;
        private Map<V> parent;

        public PathBuilder(Map map, V start)
        {
            this.start = start;
            queue = new Queue<V>();
            queue.Enqueue(start);

            distance = new Map<int>(map.SizeX, map.SizeY);
            parent = new Map<V>(map.SizeX, map.SizeY);

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
                    queue.Enqueue(u);
                }
            }
        }

        public int Distance(V v) => distance[v];

        public List<ActionBase> GetActions(V to)
        {
            var result = new List<ActionBase>();

            while (to != start)
            {
                var from = parent[to];

                result.Add(new Move(to - @from));

                to = @from;
            }

            result.Reverse();
            return result;
        }
    }
}