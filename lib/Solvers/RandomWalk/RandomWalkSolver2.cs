using System;
using System.Collections.Generic;
using lib.Models;

namespace lib.Solvers.RandomWalk
{
    public class RandomWalkSolver2 : ISolver
    {
        public string GetName()
        {
            return "random-walk2";
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

        public RandomWalkSolver2(int depth, IEstimator estimator, Random random, int tryCount)
        {
            this.depth = depth;
            this.estimator = estimator;
            this.random = random;
            this.tryCount = tryCount;
        }

        public List<ActionBase> Solve(State state)
        {
            var solution = new List<ActionBase>();

            while (state.UnwrappedLeft > 0)
            {
                var part = SolvePart(state);
                solution.AddRange(part);
                state.Apply(part);
            }

            return solution;
        }

        public List<ActionBase> SolvePart(State state)
        {
            //Console.Out.WriteLine("START PART");

            var bestEstimation = double.MinValue;
            List<ActionBase> bestSolution = null;

            for (int i = 0; i < tryCount; i++)
            {
                var clone = state.Clone();
                var solution = SolveStep(clone);
                var estimation = estimator.Estimate(clone, state);
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

        private List<ActionBase> SolveStep(State state)
        {
            var actions = new List<ActionBase>();
            while (actions.Count < depth && state.UnwrappedLeft > 0)
            {
                var action = availableActions[random.Next(availableActions.Length)];
                if (action is Move moveAction)
                {
                    var nextPosition = state.Worker.Position + moveAction.Shift;
                    if (!nextPosition.Inside(state.Map) || state.Map[nextPosition] == CellState.Obstacle)
                        continue;
                }

                state.Apply(action);
                actions.Add(action);
            }

            return actions;
        }
    }
}