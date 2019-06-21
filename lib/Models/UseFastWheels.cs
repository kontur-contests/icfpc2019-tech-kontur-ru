using System;

namespace lib.Models
{
    public class UseFastWheels : ActionBase
    {
        public override string ToString() => "F";

        public override Action Apply(State state)
        {
            if (state.Worker.FastWheelsCount <= 0)
                throw new InvalidOperationException("No fast wheels");

            state.Worker.FastWheelsCount--;
            state.Worker.FastWheelsTimeLeft = Constants.FastWheelsTime;
            return () => {};
        }
    }
}