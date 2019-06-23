using System;

namespace lib.Models.Actions
{
    public class UseDrill : ActionBase
    {
        public override string ToString() => "L";

        public override Action Apply(State state, Worker worker)
        {
            if (state.DrillCount <= 0)
                throw new InvalidOperationException("No drills");

            state.DrillCount--;
            worker.DrillTimeLeft = worker.DrillTimeLeft == 0 ? Constants.DrillTime + 1 : worker.DrillTimeLeft + Constants.DrillTime;
            
            return () => state.DrillCount++;
        }
    }
}