using System;
using System.Linq;

namespace lib.Models.Actions
{
    public class UseCloning : ActionBase
    {
        public override string ToString() => "C";

        public override Action Apply(State state, Worker worker)
        {
            if (state.CloningCount <= 0)
                throw new InvalidOperationException("No clonings");
            
            if (!state.Boosters.Any(b => b.Type == BoosterType.MysteriousPoint && b.Position == worker.Position))
                throw new InvalidOperationException($"Invalid cloning position {worker.Position}");

            state.CloningCount--;

            var replica = worker.Clone();
            replica.FastWheelsTimeLeft = 0;
            replica.DrillTimeLeft = 0;
            
            state.Workers.Add(replica);

            return () => state.CloningCount++;
        }
    }
}