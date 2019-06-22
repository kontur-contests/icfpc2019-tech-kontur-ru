using System;

namespace lib.Models.Actions
{
    public abstract class ActionBase
    {
        public abstract Action Apply(State state, Worker worker);
    }
}