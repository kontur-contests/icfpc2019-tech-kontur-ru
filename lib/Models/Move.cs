using System;

namespace lib.Models
{
    public class Move : ActionBase
    {
        public Move(V shift)
        {
            Shift = shift;
        }

        public V Shift { get; }

        public override string ToString()
        {
            return Shift.X == 0 && Shift.Y == -1 ? "S"
                : Shift.X == 0 && Shift.Y == 1 ? "W"
                : Shift.X == -1 && Shift.Y == 0 ? "A"
                : Shift.X == 1 && Shift.Y == 0 ? "D"
                : $"INVALID MOVE {Shift}";
        }

        public override void Apply(State state)
        {
            ApplySingleMove(state, ignoreInvalidMove: false);
            if (state.Worker.FastWheelsTimeLeft > 0)
                ApplySingleMove(state, ignoreInvalidMove: true);
        }

        private void ApplySingleMove(State state, bool ignoreInvalidMove)
        {
            var newPosition = state.Worker.Position + Shift;
            if (!newPosition.Inside(state.Map) || state.Map[newPosition] == CellState.Obstacle && state.Worker.DrillTimeLeft == 0)
            {
                if (ignoreInvalidMove)
                    return;
                throw new InvalidOperationException($"Invalid move from {state.Worker.Position} to obstacle {newPosition}. Action: {this}");
            }

            state.Worker.Position += Shift;
            state.Wrap();
        }
    }
}