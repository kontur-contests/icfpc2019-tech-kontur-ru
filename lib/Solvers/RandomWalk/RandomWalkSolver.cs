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
        private readonly TimeSpan timeout;
        private readonly List<ActionBase>[] rotates =
        {
            new List<ActionBase>(),
            new List<ActionBase> {new Rotate(true)},
            new List<ActionBase> {new Rotate(true), new Rotate(true)},
            new List<ActionBase> {new Rotate(false)},
            new List<ActionBase> {new Rotate(false), new Rotate(false)}
        };
        private readonly V[] shifts = {"0,1", "1,0", "0,-1", "-1,0"};

        public RandomWalkSolver(int depth, IEstimator estimator, Random random, TimeSpan timeout)
        {
            this.depth = depth;
            this.estimator = estimator;
            this.random = random;
            this.timeout = timeout;
        }

        public List<ActionBase> Solve(State state)
        {
            var bestEstimation = double.MinValue;
            List<ActionBase> bestSolution = null;

            var stopwatch = Stopwatch.StartNew();
            while (stopwatch.Elapsed < timeout)
            {
                var clone = state.Clone();
                var solution = SolveOne(clone);
                var estimation = estimator.Estimate(clone);
                if (estimation > bestEstimation)
                {
                    bestEstimation = estimation;
                    bestSolution = solution;
                }
            }

            return bestSolution;
        }

        private List<ActionBase> SolveOne(State state)
        {
            var actions = new List<ActionBase>();
            V shift = null;
            while (actions.Count < depth)
            {
                while (actions.Count < depth)
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

                while (actions.Count < depth)
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