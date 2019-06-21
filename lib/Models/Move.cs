using System;

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
            ApplySingleMove(state, ignoreInvalidMove: false);
            if (state.Worker.FastWheelsTimeLeft > 0)
                ApplySingleMove(state, ignoreInvalidMove: true);
        }

        private void ApplySingleMove(State state, bool ignoreInvalidMove)
        {
            var newPosition = state.Worker.Position + Delta;
            if (!newPosition.Inside(state.Map) || state.Map[newPosition] == CellState.Obstacle && state.Worker.DrillTimeLeft == 0)
            {
                if (ignoreInvalidMove)
                    return;
                throw new InvalidOperationException($"Invalid move from {state.Worker.Position} to obstacle {newPosition}. Action: {this}");
            }

            state.Worker.Position += Delta;
            state.Wrap();
        }
    }
}