using lib.Models;

namespace lib.Solvers.RandomWalk
{
    public interface IEstimator
    {
        double Estimate(State state);
    }
}