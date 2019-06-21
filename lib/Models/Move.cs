namespace lib.Models
{
    public class Move : ActionBase
    {
        public Move(V delta)
        {
            Delta = delta;
        }

        public V Delta { get; }
    }
}