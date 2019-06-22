using System;

namespace lib.Models.Actions
{
    public class Wait : ActionBase
    {
        public override string ToString() => "Z";
        
        public override Action Apply(State state, Worker worker)
        {
            return () => {};
        }
    }
}