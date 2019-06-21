using System;
using System.Collections.Generic;
using System.Diagnostics;
using lib.Models;

namespace lib.Solvers.RandomWalk
{
    public class RandomWalkSolver : ISolver
    {
        public string GetName()
        {
            return "random-walk";
        }

        public int GetVersion()
        {
            return 1;
        }
        
        private readonly int depth;
        private readonly IEstimator estimator;
        private readonly Random random;
        private readonly int tryCount;
        private readonly List<ActionBase>[] rotates =
        {
            new List<ActionBase>(),
            new List<ActionBase> {new Rotate(true)},
            new List<ActionBase> {new Rotate(true), new Rotate(true)},
            new List<ActionBase> {new Rotate(false)},
            new List<ActionBase> {new Rotate(false), new Rotate(false)}
        };
        private readonly V[] shifts = {"0,1", "1,0", "0,-1", "-1,0"};

        public RandomWalkSolver(int depth, IEstimator estimator, Random random, int tryCount)
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
                var estimation = estimator.Estimate(clone);
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
            V shift = null;
            while (actions.Count < depth && state.UnwrappedLeft > 0)
            {
                while (actions.Count < depth && state.UnwrappedLeft > 0)
                {
                    var nextShift = shifts[random.Next(shifts.Length)];
                    if (nextShift == shift)
                        continue;

                    var nextRotate = rotates[random.Next(rotates.Length)];
                    actions.AddRange(nextRotate);
                    state.Apply(nextRotate);
                    shift = nextShift;
                    break;
                }

                while (actions.Count < depth && state.UnwrappedLeft > 0)
                {
                    var nextPosition = state.Worker.Position + shift;
                    if (!nextPosition.Inside(state.Map) || state.Map[nextPosition] == CellState.Obstacle)
                        break;

                    var action = new Move(shift);
                    actions.Add(action);
                    state.Apply(action);
                }
            }

            return actions;
        }
    }
}