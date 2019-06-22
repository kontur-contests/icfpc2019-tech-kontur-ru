using lib.Models;

namespace lib.Solvers.RandomWalk
{
    public interface IFastWorkerEstimator
    {
        double Estimate(State state, Worker worker);
    }
}