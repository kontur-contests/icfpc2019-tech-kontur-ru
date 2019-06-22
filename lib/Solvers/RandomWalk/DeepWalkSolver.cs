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
            return $"deep-walk-{estimator.GetName()}";
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
        private CyclingTracker cyclingTracker;
        private readonly bool useWheels;

        public DeepWalkSolver(int depth, IEstimator estimator, bool useWheels = false)
        {
            this.useWheels = useWheels;
            this.estimator = estimator;

            chains = availableActions.Select(x => new List<ActionBase> {x}).ToList();
            for (int i = 1; i < depth; i++)
            {
                chains = chains.SelectMany(c => availableActions.Select(a => c.Concat(new[] {a}).ToList())).ToList();
            }
        }

        public List<List<ActionBase>> Solve(State state)
        {
            this.cyclingTracker = new CyclingTracker();
            var solution = new List<ActionBase>();
            
            BoosterMaster.CreatePalka(state, solution);

            while (state.UnwrappedLeft > 0)
            {
                if (useWheels && state.FastWheelsCount > 0)
                {
                    var useFastWheels = new UseFastWheels();
                    solution.Add(useFastWheels);
                    state.Apply(useFastWheels);
                }
                var part = SolvePart(state);
                foreach (var action in part)
                {
                    state.Apply(action);
                    cyclingTracker.AddState(state);
                }
                solution.AddRange(part);
            }

            return new List<List<ActionBase>> {solution};
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
                        var nextPosition = clone.SingleWorker.Position + moveAction.Shift;
                        if (!nextPosition.Inside(clone.Map) || clone.Map[nextPosition] == CellState.Obstacle)
                            continue;
                    }

                    clone.Apply(action);
                    solution.Add(action);
                    if (clone.UnwrappedLeft == 0)
                        break;
                }

                if (cyclingTracker.IsCycled(clone))
                    continue;
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