using System;
using System.Collections.Generic;
using System.Linq;
using lib.Models;
using lib.Models.Actions;

namespace lib.Solvers.RandomWalk
{
    public class ParallelDeepWalkSolver : ISolver
    {
        public string GetName()
        {
            return $"fast-parallel-deep-walk-{depth}-{usePalka}-{buy.Format()}-{estimator.Name}";
        }

        public int GetVersion()
        {
            return 1;
        }

        private readonly int depth;
        private readonly IEstimator estimator;
        private readonly bool usePalka;
        private readonly BoosterType[] buy;
        private readonly bool useWheels;
        private readonly bool useDrill;

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

        public ParallelDeepWalkSolver(int depth, IEstimator estimator, bool usePalka, bool useWheels, bool useDrill, BoosterType[] buy)
        {
            this.depth = depth;
            this.estimator = estimator;
            this.usePalka = usePalka;
            this.buy = buy;
            this.useWheels = useWheels;
            this.useDrill = useDrill;

            chains = availableActions.Select(x => new List<ActionBase> {x}).ToList();
            for (int i = 1; i < depth; i++)
            {
                chains = chains.SelectMany(c => availableActions.Select(a => c.Concat(new[] {a}).ToList())).ToList();
            }
        }

        public Solved Solve(State state)
        {
            var solution = new List<List<ActionBase>> {new List<ActionBase>()};

            state.BuyBoosters(buy);
            
            if (usePalka)
                BoosterMaster.CreatePalka(state, solution[0]);
            BoosterMaster.CloneAttack(state, solution);

            while (state.UnwrappedLeft > 0)
            {
                // Console.Out.WriteLine($"--BEFORE:\n{state.Print()}");
                
                var partialSolution = new List<List<ActionBase>>();

                while (partialSolution.Count < solution.Count)
                {
                    var part = SolvePart(state, partialSolution);
                    partialSolution.Add(part);
                    //Console.Out.WriteLine($"  PART:\n{part.Format()}");
                }

                for (int i = 0; i < partialSolution[0].Count; i++)
                {
                    for (int j = 0; j < partialSolution.Count; j++)
                        solution[j].Add(partialSolution[j][i]);
                    state.Apply(state.Workers.Select((w, wi) => (w, partialSolution[wi][i])).ToList());
                }

                // if (turn++ > 100)
                //     break;
            }

            return new Solved {Actions = solution, Buy = buy.ToList()};
        }

        public List<ActionBase> SolvePart(State state, List<List<ActionBase>> partialSolution)
        {
            var bestEstimation = double.MinValue;
            List<ActionBase> bestSolution = null;

            var usedWheels = partialSolution.Sum(x => x.Count(c => c is UseFastWheels));
            var useWheelsLocal = useWheels && (state.FastWheelsCount - usedWheels) > 0;

            var usedDrill = partialSolution.Sum(x => x.Count(c => c is UseDrill));
            var useDrillLocal = useDrill && (state.DrillCount - usedDrill) > 0;
            
            foreach (var chain2 in chains)
            {
                var chain = chain2.ToList();
                if (useWheelsLocal)
                {
                    chain.Insert(0, new UseFastWheels());
                    chain.RemoveAt(chain.Count - 1);
                }
                if (useDrillLocal)
                {
                    chain.Insert(0, new UseDrill());
                    chain.RemoveAt(chain.Count - 1);
                }
                var solution = new List<ActionBase>();
                var undos = new List<Action>();
                for (var c = 0; c < chain.Count; c++)
                {
                    var action = chain[c];
                    if (action is Move moveAction)
                    {
                        var worker = state.Workers[partialSolution.Count];
                        var nextPosition = worker.Position + moveAction.Shift;
                        if (!nextPosition.Inside(state.Map) || (state.Map[nextPosition] == CellState.Obstacle && worker.DrillTimeLeft <= 1))
                            break;
                    }

                    undos.Add(state.Apply(
                        state
                            .Workers
                            .Select(
                                (w, i) => (w, i < partialSolution.Count ? partialSolution[i][c]
                                    : i == partialSolution.Count ? action
                                    : new Wait()))
                            .ToList()));
                    solution.Add(action);
                    if (state.UnwrappedLeft == 0)
                        break;
                }

                while (solution.Count < depth)
                {
                    var wait = new Wait();
                    undos.Add(state.Apply(
                        state
                            .Workers
                            .Select((w, i) => (w, i < partialSolution.Count ? partialSolution[i][solution.Count] : wait))
                            .ToList()));
                    solution.Add(wait);
                }

                var estimation = estimator.Estimate(state, state.Workers[partialSolution.Count]);
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