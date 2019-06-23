using System;
using System.Collections.Generic;
using System.Linq;
using lib.Models;
using lib.Models.Actions;

namespace lib.Solvers.RandomWalk
{
    public class RandomWalkSolver : ISolver
    {
        public string GetName()
        {
            return $"random-walk-{depth}/{tryCount}/{usePalka}/{estimator.Name}";
        }

        public int GetVersion()
        {
            return 1;
        }

        private readonly int depth;
        private readonly IEstimator estimator;
        private readonly Random random;
        private readonly int tryCount;

        private readonly ActionBase[] availableActions =
        {
            new Rotate(true), 
            new Rotate(false), 
            new Move("0,1"), 
            new Move("0,-1"), 
            new Move("1,0"), 
            new Move("-1,0")
        };
        private readonly bool usePalka;
        private readonly bool useWheels;

        public RandomWalkSolver(int depth, IEstimator estimator, Random random, int tryCount, bool usePalka, bool useWheels = false)
        {
            this.depth = depth;
            this.estimator = estimator;
            this.random = random;
            this.tryCount = tryCount;
            this.usePalka = usePalka;
            this.useWheels = useWheels;
        }

        public List<List<ActionBase>> Solve(State state)
        {
            var solution = new List<ActionBase>();
            
            if (usePalka)
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
                solution.AddRange(part);
                state.ApplyRange(part);
            }

            return new List<List<ActionBase>> {solution};
        }

        public List<ActionBase> SolvePart(State state)
        {
            //Console.Out.WriteLine("START PART");

            var bestEstimation = double.MinValue;
            List<ActionBase> bestSolution = null;

            for (int i = 0; i < tryCount; i++)
            {
                var clone = state;//.Clone();
                var undoes = new List<Action>();
                var solution = SolveStep(clone, undoes);
                var estimation = estimator.Estimate(clone, state.SingleWorker);
                undoes.Reverse();
                foreach (var undo in undoes)
                {
                    undo();
                }
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

            return bestSolution;
        }

        private List<ActionBase> SolveStep(State state, List<Action> undoes)
        {
            var actions = new List<ActionBase>();
            while (actions.Count < depth && state.UnwrappedLeft > 0)
            {
                var action = availableActions[random.Next(availableActions.Length)];
                if (action is Move moveAction)
                {
                    var nextPosition = state.SingleWorker.Position + moveAction.Shift;
                    if (!nextPosition.Inside(state.Map) || state.Map[nextPosition] == CellState.Obstacle)
                        continue;
                }

                undoes.Add(state.Apply(action));
                actions.Add(action);
            }

            return actions;
        }
    }
}