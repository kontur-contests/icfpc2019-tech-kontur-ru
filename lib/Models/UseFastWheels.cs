using System;

namespace lib.Models
{
    public class UseFastWheels : ActionBase
    {
        public override string ToString() => "F";

        public override Action Apply(State state, Worker worker)
        {
            if (state.FastWheelsCount <= 0)
                throw new InvalidOperationException("No fast wheels");

            state.FastWheelsCount--;
            worker.FastWheelsTimeLeft = Constants.FastWheelsTime;
            return () => {};
        }
    }
}