using System;
using System.Collections.Generic;
using System.Linq;
using lib.Models;
using lib.Models.Actions;

namespace lib.Solvers.RandomWalk
{
    public class ParallelPlanSolver : ISolver
    {
        public string GetName()
        {
            return $"paraplan-{depth}";
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
        private readonly int depth;

        public ParallelPlanSolver(int depth)
        {
            this.depth = depth;
            chains = availableActions.Select(x => new List<ActionBase> {x}).ToList();
            for (int i = 1; i < depth; i++)
            {
                chains = chains.SelectMany(c => availableActions.Select(a => c.Concat(new[] {a}).ToList())).ToList();
            }

            estimator = new PlanWorkerEstimator();
        }

        public Solved Solve(State state)
        {
            var solution = new List<List<ActionBase>> {new List<ActionBase>()};

            BoosterMaster.CloneAttack(state, solution);

            var offsets = Enumerable.Range(0, state.Workers.Count).Select(i => i * state.ClustersState.Path.Count / state.Workers.Count).ToList();
            while (state.UnwrappedLeft > 0)
            {
                //Console.Out.WriteLine($"--BEFORE:\n{state.Print()}");
                
                var partialSolution = new List<List<ActionBase>>();

                while (partialSolution.Count < solution.Count)
                {
                    var clusterId = state.ClustersState.Path[offsets[partialSolution.Count]];
                    while (state.ClustersState.Unwrapped[(0, clusterId)] == 0)
                    {
                        offsets[partialSolution.Count] = (offsets[partialSolution.Count] + 1) % state.ClustersState.Path.Count;
                        clusterId = state.ClustersState.Path[offsets[partialSolution.Count]];
                    }

                    var part = SolvePart(state, partialSolution, clusterId);
                    partialSolution.Add(part);
                    //Console.Out.WriteLine($"  PART:\n{part.Format()}");
                }

                for (int i = 0; i < partialSolution[0].Count; i++)
                {
                    for (int j = 0; j < partialSolution.Count; j++)
                        solution[j].Add(partialSolution[j][i]);
                    state.Apply(state.Workers.Select((w, wi) => (w, partialSolution[wi][i])).ToList());
                }
            }

            return new Solved {Actions = solution};
        }

        public List<ActionBase> SolvePart(State state, List<List<ActionBase>> partialSolution, int clusterId)
        {
            var bestEstimation = double.MinValue;
            List<ActionBase> bestSolution = null;

            foreach (var chain in chains)
            {
                var solution = new List<ActionBase>();
                var undos = new List<Action>();
                for (var c = 0; c < chain.Count; c++)
                {
                    var action = chain[c];
                    if (action is Move moveAction)
                    {
                        var nextPosition = state.Workers[partialSolution.Count].Position + moveAction.Shift;
                        if (!nextPosition.Inside(state.Map) || state.Map[nextPosition] == CellState.Obstacle)
                            break;
                    }

                    undos.Add(
                        state.Apply(
                            state
                                .Workers
                                .Select(
                                    (w, i) => (w, i < partialSolution.Count ? partialSolution[i][c]
                                        : i == partialSolution.Count ? action
                                        : new Wait()))
                                .ToList()));
                    solution.Add(action);
                    if (state.UnwrappedLeft == 0 || state.ClustersState.Unwrapped[(0, clusterId)] == 0)
                        break;
                }

                while (solution.Count < depth)
                {
                    var wait = new Wait();
                    undos.Add(
                        state.Apply(
                            state
                                .Workers
                                .Select((w, i) => (w, i < partialSolution.Count ? partialSolution[i][solution.Count] : wait))
                                .ToList()));
                    solution.Add(wait);
                }

                var estimation = estimator.Estimate(state, state.Workers[partialSolution.Count], clusterId);
                //Console.Out.Write($"  w{partialSolution.Count} {estimation} {solution.Format()}");
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