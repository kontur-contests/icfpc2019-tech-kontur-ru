using System;

namespace lib.Models
{
    public class Wait : ActionBase
    {
        public override string ToString() => "Z";
        
        public override Action Apply(State state)
        {
            return () => {};
        }
    }
}