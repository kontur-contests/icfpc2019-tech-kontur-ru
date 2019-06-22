using System;
using System.Collections.Generic;
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

            var replica = new Worker
            {
                Position = worker.Position,
                Manipulators = new List<V> {new V(1, 0), new V(1, 1), new V(1, -1)}
            };

            state.Workers.Add(replica);
            var unwrap = state.Wrap();

            return () =>
            {
                unwrap();
                state.CloningCount++;
            };
        }
    }
}