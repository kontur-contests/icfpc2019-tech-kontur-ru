using System;
using System.Linq;

namespace lib.Models.Actions
{
    public class Shift : ActionBase
    {
        public Shift(V target)
        {
            Target = target;
        }

        public V Target { get; }

        public override string ToString() => $"T{Target}";

        public override Action Apply(State state, Worker worker)
        {
            if (!state.Boosters.Any(b => b.Type == BoosterType.TeleportBeacon && b.Position == Target))
                throw new InvalidOperationException($"There is no {BoosterType.TeleportBeacon} in {Target}");
            
            worker.Position = Target;
            return state.Wrap();
        }
    }
}