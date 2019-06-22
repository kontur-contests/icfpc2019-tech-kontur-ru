using System;
using System.Collections.Generic;
using System.Linq;
using lib.Models;
using lib.Models.Actions;

namespace lib.Solvers.RandomWalk
{
    public class PlanSolver : ISolver
    {
        private readonly int depth;

        public string GetName()
        {
            return $"plan-{depth}";
        }

        public int GetVersion()
        {
            return 1;
        }

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
        private readonly PlanWorkerEstimator estimator;

        public PlanSolver(int depth)
        {
            this.depth = depth;
            chains = availableActions.Select(x => new List<ActionBase> {x}).ToList();
            for (int i = 1; i < depth; i++)
            {
                chains = chains.SelectMany(c => availableActions.Select(a => c.Concat(new[] {a}).ToList())).ToList();
            }

            estimator = new PlanWorkerEstimator();
        }

        public List<List<ActionBase>> Solve(State state)
        {
            var solution = new List<ActionBase>();
            foreach (var clusterId in state.ClustersState.Path)
            {
                while (state.ClustersState.Unwrapped[(0, clusterId)] != 0)
                {
                    var part = SolvePart(state, clusterId);
                    state.ApplyRange(part);
                    solution.AddRange(part);
                }
            }

            return new List<List<ActionBase>> {solution};
        }

        public List<ActionBase> SolvePart(State state, int clusterId)
        {
            var bestEstimation = double.MinValue;
            List<ActionBase> bestSolution = null;

            foreach (var chain in chains)
            {
                var undos = new List<Action>();
                var solution = new List<ActionBase>();
                foreach (var action in chain)
                {
                    if (action is Move moveAction)
                    {
                        var nextPosition = state.SingleWorker.Position + moveAction.Shift;
                        if (!nextPosition.Inside(state.Map) || state.Map[nextPosition] == CellState.Obstacle)
                            continue;
                    }

                    undos.Add(state.Apply(action));
                    solution.Add(action);
                    if (state.UnwrappedLeft == 0 || state.ClustersState.Unwrapped[(0, clusterId)] == 0)
                        break;
                }

                var estimation = estimator.Estimate(state, state.SingleWorker, clusterId);
                //Console.Out.Write($"  {estimation} {solution.Format()}");
                if (estimation > bestEstimation)
                {
                    bestEstimation = estimation;
                    bestSolution = solution;
                    //Console.Out.WriteLine(" -- better");
                }
                // else
                //     Console.Out.WriteLine();

                undos.Reverse();
                foreach (var undo in undos)
                    undo();
            }

            return bestSolution;
        }
    }
}