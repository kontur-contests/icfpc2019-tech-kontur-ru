using System;

namespace lib.Models.Actions
{
    public class UseFastWheels : ActionBase
    {
        public override string ToString() => "F";

        public override Action Apply(State state, Worker worker)
        {
            if (state.FastWheelsCount <= 0)
                throw new InvalidOperationException("No fast wheels at " + state.Time);

            state.FastWheelsCount--;

            worker.FastWheelsTimeLeft = worker.FastWheelsTimeLeft == 0 ? Constants.FastWheelsTime + 1 : worker.FastWheelsTimeLeft + Constants.FastWheelsTime;
            return () => state.FastWheelsCount++;
        }
    }
}