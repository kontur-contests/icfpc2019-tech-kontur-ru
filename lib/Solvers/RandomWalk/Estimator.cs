using System;
using System.Collections.Generic;
using System.Linq;
using lib.Models;

namespace lib.Solvers.RandomWalk
{
    public class Estimator : IEstimator
    {
        private readonly bool collectFastWheels;
        private readonly bool zakoulochki;
        private readonly bool collectDrill;
        private Map<(int value, int version)> distance;
        private Map<(V value, int version)> parent;
        private int currentVersion;

        public Estimator(bool collectFastWheels, bool zakoulochki, bool collectDrill)
        {
            this.collectFastWheels = collectFastWheels;
            this.zakoulochki = zakoulochki;
            this.collectDrill = collectDrill;
        }

        public string Name
        {
            get
            {
                var name = new List<string>();
                if (collectFastWheels) name.Add("wheels");
                if (zakoulochki) name.Add("zako");
                if (collectDrill) name.Add("drrr");
                name.Add("spread-clone");
                return string.Join("-", name);
            }
        }

        public double Estimate(State state, Worker worker)
        {
            if (zakoulochki && state.CellCostCalculator == null)
                state.CellCostCalculator = new CellCostCalculator(state);

            if (state.UnwrappedLeft == 0)
                return 10_000_000_000.0 - state.Time * 1_000.0;

            if (ReferenceEquals(worker, state.Workers[0]))
            {
                if (state.CloningCount > 0)
                {
                    if (state.Boosters.Any(b => b.Type == BoosterType.MysteriousPoint && b.Position == worker.Position))
                        return 5_000_000_000.0;

                    var distToMyst = DistanceToBooster(state.Map, worker.Position, BoosterType.MysteriousPoint, state.Boosters);
                    return 3_000_000_000.0 - distToMyst;
                }

                if (state.CloningCount == 0 && state.Boosters.Any(b => b.Type == BoosterType.Cloning))
                {
                    var distToClone = DistanceToBooster(state.Map, worker.Position, BoosterType.Cloning, state.Boosters);
                    return 1_000_000_000.0 - distToClone;
                }
            }

            var distScore = DistanceToVoid(state.Map, worker.Position);

            var unwrappedCost = zakoulochki ? state.CellCostCalculator.Cost : 0;

            var fastWheelsBonus = collectFastWheels ? state.Workers.Sum(w => w.FastWheelsTimeLeft) + state.FastWheelsCount * Constants.FastWheelsTime * 50_000_000.0 : 0;
            var drillBonus = collectDrill ? state.Workers.Sum(w => w.DrillTimeLeft) + state.DrillCount * Constants.DrillTime * 50_000_000.0 : 0;

            var spreadBonus = state.Workers.Count > 1 && state.Time % 100 < 11 ? GetWorkersBBSq(state.Workers) : 0;

            return 100_000_000.0 + fastWheelsBonus + drillBonus + spreadBonus * 10.0 - distScore - state.UnwrappedLeft * 1_000_000.0 - unwrappedCost * 1_000_000.0;
        }

        private double GetWorkersBBSq(List<Worker> workers)
        {
            /*var result = 0;
            for (int i = 0; i < workers.Count - 1; i++)
            for (int j = i + 1; j < workers.Count; j++)
            {
                var dx = Math.Abs(workers[j].Position.X - workers[i].Position.X);
                var dy = Math.Abs(workers[j].Position.Y - workers[i].Position.Y);
                result += dx + dy;
            }
            return result;*/

            var minx = workers.Select(w => w.Position.X).Min();
            var maxx = workers.Select(w => w.Position.X).Max();
            var miny = workers.Select(w => w.Position.Y).Min();
            var maxy = workers.Select(w => w.Position.Y).Max();
            return (maxy - miny) * (maxx - minx);
        }

        public int DistanceToVoid(Map map, V start)
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

        private double DistanceToBooster(Map map, V start, BoosterType targetBoosterType, List<Booster> allBoosters)
        {
            return DistanceToTargets(map, start, new HashSet<V>(allBoosters.Where(b => b.Type == targetBoosterType).Select(b => b.Position)));
        }

        public int DistanceToTargets(Map map, V start, HashSet<V> targets)
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

                    if (targets.Contains(u))
                        return distance[u].value;

                    queue.Enqueue(u);
                }
            }

            throw new InvalidOperationException();
        }

        public int DistanceToTarget(Map map, V start, V target)
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
                    if (u == target)
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