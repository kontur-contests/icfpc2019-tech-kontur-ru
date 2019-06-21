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

        public bool IsValid(State state)
        {
            var newPosition = state.Worker.Position + Shift;
            return newPosition.Inside(state.Map) && (state.Map[newPosition] != CellState.Obstacle || state.Worker.DrillTimeLeft > 0);
        }

        public override Action Apply(State state)
        {
            var undo = ApplySingleMove(state, ignoreInvalidMove: false);
            if (state.Worker.FastWheelsTimeLeft > 0)
            {
                var undo2 = ApplySingleMove(state, ignoreInvalidMove: true);
                return () =>
                {
                    undo();
                    undo2();
                };
            }
            return undo;
        }

        private Action ApplySingleMove(State state, bool ignoreInvalidMove)
        {
            var newPosition = state.Worker.Position + Shift;
            if (!newPosition.Inside(state.Map) || state.Map[newPosition] == CellState.Obstacle && state.Worker.DrillTimeLeft == 0)
            {
                if (ignoreInvalidMove)
                    return () => { };
                throw new InvalidOperationException($"Invalid move from {state.Worker.Position} to obstacle {newPosition}. Action: {this}");
            }

            state.Worker.Position += Shift;
            var unwrap = state.Wrap();
            var returnBoosters = state.CollectBoosters();
            return () =>
            {
                unwrap();
                returnBoosters();
            };
        }
    }
}