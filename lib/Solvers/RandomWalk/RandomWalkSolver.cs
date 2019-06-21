using System.Collections.Generic;
using lib.Models;

namespace lib.Solvers.RandomWalk
{
    public class RandomWalkSolver : ISolver
    {
        private readonly int depth;
        private readonly IEstimator estimator;

        public RandomWalkSolver(int depth, IEstimator estimator)
        {
            this.depth = depth;
            this.estimator = estimator;
        }
        
        public List<ActionBase> Solve(State state)
        {
            return null;
        }
    }
}