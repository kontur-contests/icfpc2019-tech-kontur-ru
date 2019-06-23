using lib.Models;

namespace lib.Solvers.RandomWalk
{
    public interface IEstimator
    {
        string Name { get; }
        double Estimate(State state, Worker worker);
    }
}