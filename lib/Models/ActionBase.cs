using System;

namespace lib.Models
{
    public abstract class ActionBase
    {
        public abstract Action Apply(State state, Worker worker);
    }
}