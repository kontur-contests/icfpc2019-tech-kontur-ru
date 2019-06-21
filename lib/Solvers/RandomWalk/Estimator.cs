using System.Linq;
using lib.Models;

namespace lib.Solvers.RandomWalk
{
    public class Estimator : IEstimator
    {
        public double Estimate(State state)
        {
            var pathBuilder = new PathBuilder(state.Map, state.Worker.Position);
            var distScore = 0;
            if (state.UnwrappedLeft > 0)
            {
                var list = state.Map
                    .EnumerateCells()
                    .Where(x => x.Item2 == CellState.Void)
                    .Select(x => pathBuilder.Distance(x.Item1))
                    .ToList();

                distScore = list.Min();
            }

            if (state.UnwrappedLeft == 0)
                return 1_000_000_000 - state.Time;
            
            return -state.Map.SizeX*10*state.UnwrappedLeft - distScore;
        }
    }
}