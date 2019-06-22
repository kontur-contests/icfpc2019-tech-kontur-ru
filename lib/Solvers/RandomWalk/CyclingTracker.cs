using System.Collections.Generic;
using lib.Models;

namespace lib.Solvers.RandomWalk
{
    public class CyclingTracker
    {
        private readonly Dictionary<string, int> unwrappedLeftByWorker = new Dictionary<string, int>();
        public void AddState(State state)
        {
            unwrappedLeftByWorker[state.SingleWorker.ToString()] = state.UnwrappedLeft;
        }

        public bool IsCycled(State state)
        {
            return unwrappedLeftByWorker.TryGetValue(state.SingleWorker.ToString(), out var unwrappedCount) && unwrappedCount == state.UnwrappedLeft;
        }
    }
}