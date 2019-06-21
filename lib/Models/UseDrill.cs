using System;

namespace lib.Models
{
    public class UseDrill : ActionBase
    {
        public override string ToString() => "L";

        public override void Apply(State state)
        {
            if (state.Worker.DrillCount <= 0)
                throw new InvalidOperationException("No drills");

            state.Worker.DrillCount--;
            state.Worker.DrillTimeLeft = Constants.DrillTime;
        }
    }
}