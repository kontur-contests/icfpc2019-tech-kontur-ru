using System.Linq;
using System.Runtime.CompilerServices;
using lib.Models;

namespace lib.Solvers.RandomWalk
{
    public class CompactWrappingEstimator : IEstimator
    {
        public double Estimate(State state, State prevState)
        {
            
            
            var pathBuilder = new PathBuilder(state.Map, state.Worker.Position);
            
            var distScore = 0;
            var bbPenalty = 0.0;
            if (state.UnwrappedLeft > 0)
            {
                var list = state.Map
                    .EnumerateCells()
                    .Where(x => x.Item2 == CellState.Void)
                    .Select(x => pathBuilder.Distance(x.Item1))
                    .ToList();

                distScore = list.Min();

                var points = state.Map
                    .EnumerateCells()
                    .Where(c => c.state == CellState.Wrapped)
                    .Select(c => c.pos)
                    .ToList();
                var minX = points.Min(p => p.X);
                var minY = points.Min(p => p.Y);
                var maxX = points.Max(p => p.X);
                var maxY = points.Max(p => p.Y);
                bbPenalty = (maxX - minX + maxY - minY) / (double)state.Map.SizeX;

            }

            if (state.UnwrappedLeft == 0)
                return 1_000_000_000 - state.Time;

            return -state.Map.SizeX*10*state.UnwrappedLeft - distScore + state.Map.SizeX*300*bbPenalty;
        }
    }
}