using System.Collections.Generic;
using System.Linq;
using lib.Models;

namespace lib.Strategies
{
    internal class Stupid
    {
        private Map colored;
        private Map map;

        public List<ActionBase> Solve(State state)
        {
            map = state.Map;
            colored = new Map(map.SizeX, map.SizeY);

            while (true)
            {
                
            }
        }

        private class PathBuilder
        {
            private Map map;
            private V start;
            private Queue<V> queue;
            private Map<int> distance;

            public PathBuilder(Map map, V start)
            {
                this.map = map;
                this.start = start;

                queue = new Queue<V>();
                queue.Enqueue(start);

                while (queue.Any())
                {
                    var v = queue.Dequeue();

                }
            }
        }
    }
}