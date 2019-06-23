using System;
using System.Collections.Generic;
using System.Linq;
using lib.Models;
using lib.Models.Actions;

namespace lib.Solvers.RandomWalk
{
    public class DeepWalkSolver : ISolver
    {
        public string GetName()
        {
            return $"deep-fast-{depth}-{usePalka}-{estimator.Name}";
        }

        public int GetVersion()
        {
            return 1;
        }

        private readonly int depth;
        private readonly IEstimator estimator;
        private readonly bool usePalka;
        private readonly bool useWheels;

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

        public DeepWalkSolver(int depth, IEstimator estimator, bool usePalka, bool useWheels)
        {
            this.depth = depth;
            this.estimator = estimator;
            this.usePalka = usePalka;
            this.useWheels = useWheels;

            chains = availableActions.Select(x => new List<ActionBase> {x}).ToList();
            for (int i = 1; i < depth; i++)
            {
                chains = chains.SelectMany(c => availableActions.Select(a => c.Concat(new[] {a}).ToList())).ToList();
            }
        }

        public List<List<ActionBase>> Solve(State state)
        {
            var solution = new List<ActionBase>();

            if (usePalka)
                BoosterMaster.CreatePalka(state, solution);

            //var tick = 0;

            var used = new HashSet<(V position, int unwrappedLeft)>();
            while (state.UnwrappedLeft > 0)
            {
                if (useWheels && state.FastWheelsCount > 0)
                {
                    var useFastWheels = new UseFastWheels();
                    solution.Add(useFastWheels);
                    state.Apply(useFastWheels);
                }
                //Console.Out.WriteLine($"--BEFORE:\n{state.Print()}");
                var part = SolvePart(state, used);
                if (part == null)
                    part = SolvePart(state, new HashSet<(V position, int unwrappedLeft)>());
                used.Add((state.SingleWorker.Position, state.UnwrappedLeft));
                solution.AddRange(part);
                state.ApplyRange(part);
                // Console.Out.WriteLine($"  PART:\n{part.Format()}");
                //
                // if (tick++ > 1000)
                //     break;
            }

            return new List<List<ActionBase>> {solution};
        }

        public List<ActionBase> SolvePart(State state, HashSet<(V position, int unwrappedLeft)> used)
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
                    if (state.UnwrappedLeft == 0)
                        break;
                }

                if (!used.Contains((state.SingleWorker.Position, state.UnwrappedLeft)))
                {
                    var estimation = estimator.Estimate(state, state.SingleWorker);
                    //Console.Out.Write($"  {estimation} {solution.Format()}");
                    if (estimation > bestEstimation)
                    {
                        bestEstimation = estimation;
                        bestSolution = solution;
                        //Console.Out.WriteLine(" -- better");
                    }
                    // else
                    //     Console.Out.WriteLine();
                }

                undos.Reverse();
                foreach (var undo in undos)
                    undo();
            }

            return bestSolution;
        }
    }
}