using System.Linq;
using lib.Models;

namespace lib.Strategies
{
    class Estimator
    {
        public double Estimate(State state)
        {
            var wrappedCount = state.Map.EnumerateCells().Count(c => c.Item2 == CellState.Wrapped);
            return wrappedCount;
        }
    }
}