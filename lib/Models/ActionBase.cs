namespace lib.Models
{
    public abstract class ActionBase
    {
        public abstract void Apply(State state);
    }
}