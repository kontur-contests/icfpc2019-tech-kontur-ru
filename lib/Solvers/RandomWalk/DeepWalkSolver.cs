using System;
using System.Collections.Generic;
using System.Linq;
using lib.Models;

namespace lib.Solvers.RandomWalk
{
    public class DeepWalkSolver : ISolver
    {
        public string GetName()
        {
            return "deep-walk";
        }

        public int GetVersion()
        {
            return 1;
        }

        private readonly IEstimator estimator;

        private readonly ActionBase[] availableActions =
        {
            new Rotate(true),
            new Rotate(false),
            new Move("0,1"),
            new Move("0,-1"),
            new Move("1,0"),
            new Move("-1,0")
        };
        private readonly List<List<ActionBase>> chains;

        public DeepWalkSolver(int depth, IEstimator estimator)
        {
            this.estimator = estimator;

            chains = availableActions.Select(x => new List<ActionBase> {x}).ToList();
            for (int i = 1; i < depth; i++)
            {
                chains = chains.SelectMany(c => availableActions.Select(a => c.Concat(new[] {a}).ToList())).ToList();
            }
        }

        public List<ActionBase> Solve(State state)
        {
            var solution = new List<ActionBase>();
            
            CollectManipulators(state, solution);

            while (state.UnwrappedLeft > 0)
            {
                var part = SolvePart(state);
                solution.AddRange(part);
                state.Apply(part);
            }

            return solution;
        }
        
        private void CollectManipulators(State state, List<ActionBase> result)
        {
            var k = 0;

            while (true)
            {
                var boosters = state.Boosters.Where(b => b.Type == BoosterType.Extension).ToList();

                if (!boosters.Any())
                    return;

                var map = state.Map;
                var me = state.Worker;
                var pathBuilder = new PathBuilder(map, me.Position, false);

                var best = boosters.OrderBy(b => pathBuilder.Distance(b.Position)).First();

                var actions = pathBuilder.GetActions(best.Position);

                var y = k / 2 + 2;
                y = k % 2 == 0 ? -y : y;
                var add = new UseExtension(new V(1, y));
                k++;

                actions.Add(add);

                state.Apply(actions);
                result.AddRange(actions);
            }
        }
        
        private class PathBuilder
        {
            private readonly V start;
            private Queue<V> queue;
            private Map<int> distance;
            private Map<V> parent;

            public PathBuilder(Map map, V start, bool stop)
            {
                this.start = start;
                queue = new Queue<V>();
                queue.Enqueue(start);

                distance = new Map<int>(map.SizeX, map.SizeY);
                parent = new Map<V>(map.SizeX, map.SizeY);

                while (queue.Any())
                {
                    var v = queue.Dequeue();
                    if (map[v] == CellState.Void && stop)
                        break;

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

            public int Distance(V v) => parent[v] == null ? int.MaxValue : distance[v];

            public List<ActionBase> GetActions(V to)
            {
                var result = new List<ActionBase>();

                while (to != start)
                {
                    var from = parent[to];

                    result.Add(new Move(to - from));

                    to = from;
                }

                result.Reverse();
                return result;
            }
        }


        public List<ActionBase> SolvePart(State state)
        {
            // Console.Out.WriteLine("START PART");

            var bestEstimation = double.MinValue;
            List<ActionBase> bestSolution = null;

            foreach (var chain in chains)
            {
                var clone = state.Clone();

                var solution = new List<ActionBase>();
                foreach (var action in chain)
                {
                    if (action is Move moveAction)
                    {
                        var nextPosition = clone.Worker.Position + moveAction.Shift;
                        if (!nextPosition.Inside(clone.Map) || clone.Map[nextPosition] == CellState.Obstacle)
                            continue;
                    }

                    clone.Apply(action);
                    solution.Add(action);
                    if (clone.UnwrappedLeft == 0)
                        break;
                }

                var estimation = estimator.Estimate(clone, state);
                // Console.Out.Write($"  {estimation} {solution.Format()}");
                if (estimation > bestEstimation)
                {
                    bestEstimation = estimation;
                    bestSolution = solution;
                    // Console.Out.WriteLine(" -- better");
                }
                // else
                //     Console.Out.WriteLine();
            }

            return bestSolution;
        }
    }
}