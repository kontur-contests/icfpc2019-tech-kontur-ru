using System.Linq;
using lib.Models;

namespace lib.Solvers.RandomWalk
{
    public class Estimator : IEstimator
    {
        public double Estimate(State state)
        {
            var wrappedCount = state.Map.EnumerateCells().Count(c => c.Item2 == CellState.Void);
            return wrappedCount;
        }
    }
}