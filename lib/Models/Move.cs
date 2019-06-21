namespace lib.Models
{
    public class Move : ActionBase
    {
        public Move(Point delta)
        {
            Delta = delta;
        }

        public Point Delta { get; }
    }
}