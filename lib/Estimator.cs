using System.Linq;
using lib.Models;

namespace lib
{
    public class Estimator
    {
        public double Estimate(State state)
        {
            var wrappedCount = state.Map.EnumerateCells().Count(c => c.Item2 == CellState.Void);
            return wrappedCount;
        }
    }
}