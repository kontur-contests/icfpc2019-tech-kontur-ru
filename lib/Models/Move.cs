namespace lib.Models
{
    public class Move : ActionBase
    {
        public Move(V delta)
        {
            Delta = delta;
        }

        public V Delta { get; }

        public override string ToString()
        {
            return Delta.X == 0 && Delta.Y == -1 ? "S"
                : Delta.X == 0 && Delta.Y == 1 ? "W"
                : Delta.X == -1 && Delta.Y == 0 ? "A"
                : Delta.X == 1 && Delta.Y == 0 ? "D"
                : $"INVALID MOVE {Delta}";
        }

        public override void Apply(State state)
        {
            state.Worker.Position += Delta;
            if (state.Worker.FastWheelsTimeLeft > 0)
                state.Worker.Position += Delta;
        }
    }
}