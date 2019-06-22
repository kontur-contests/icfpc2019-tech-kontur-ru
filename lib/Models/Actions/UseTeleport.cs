using System;
using System.Linq;

namespace lib.Models.Actions
{
    public class UseTeleport : ActionBase
    {
        public override string ToString() => "R";

        public override Action Apply(State state, Worker worker)
        {
            if (state.TeleportCount <= 0)
                throw new InvalidOperationException("No teleports");
            
            if (state.Boosters.Any(b => b.Type == BoosterType.TeleportBeacon && b.Position == worker.Position))
                throw new InvalidOperationException($"There is {BoosterType.TeleportBeacon} in {worker.Position} already");
            
            if (state.Boosters.Any(b => b.Type == BoosterType.MysteriousPoint && b.Position == worker.Position))
                throw new InvalidOperationException($"There is {BoosterType.MysteriousPoint} in {worker.Position} already");
                
            state.TeleportCount--;
            var beacon = new Booster(BoosterType.TeleportBeacon, worker.Position);
            state.Boosters.Add(beacon);

            return () =>
            {
                state.Boosters.Remove(beacon);
                state.TeleportCount++;
            };
        }
    }
    
    
}