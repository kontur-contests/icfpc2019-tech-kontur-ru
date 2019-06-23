using System.Collections.Generic;
using System.Linq;
using lib.Models;

namespace lib.Solvers.RandomWalk
{
    public class SingleStateEstimator : ISingleStateEstimator, IEstimator
    {
        public double Estimate(State state)
        {
            if (state.UnwrappedLeft == 0)
                return 1_000_000_000 - state.Time;

            var distScore = GetDistanceToClosestVoid(state.Map, state.SingleWorker.Position);

            int fastWheelsBonus = state.Workers.Sum(w => w.FastWheelsTimeLeft) + state.FastWheelsCount * Constants.FastWheelsTime * 100_000;
            return 100_000_000 - distScore - state.UnwrappedLeft * 100_000 + fastWheelsBonus;
        }

        private int GetDistanceToClosestVoid(Map map, V start)
        {
            
            //Bfs
            var queue = new Queue<(V, int)>();
            queue.Enqueue((start, 0));

            var used = new HashSet<V>();
            used.Add(start);
            while (queue.Any())
            {
                var (v, dist) = queue.Dequeue();
                for (var direction = 0; direction < 4; direction++)
                {
                    var u = v.Shift(direction);
                    if (!u.Inside(map) || used.Contains(u) || map[u] == CellState.Obstacle)
                        continue;
                    if (map[u] == CellState.Void) return dist + 1;
                    used.Add(u);
                    queue.Enqueue((u, dist + 1));
                }
            }
            return int.MaxValue;
        }

        public double Estimate(State state, State prevState) => Estimate(state);

        public string GetName() => "simple";
    }
}