using lib.Models;

namespace lib.Solvers.RandomWalk
{
    public interface IWorkerEstimator
    {
        double Estimate(State state, State prevState, Worker worker);
    }
}