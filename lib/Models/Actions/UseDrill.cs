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
            worker.DrillTimeLeft = Constants.DrillTime;
            return () => {};
        }
    }
}