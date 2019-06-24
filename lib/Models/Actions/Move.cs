using System;

namespace lib.Models.Actions
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

        public bool IsValid(State state, Worker worker)
        {
            var newPosition = worker.Position + Shift;
            return newPosition.Inside(state.Map) && (state.Map[newPosition] != CellState.Obstacle || worker.DrillTimeLeft > 0);
        }

        public override Action Apply(State state, Worker worker)
        {
            var undo = ApplySingleMove(state, worker, ignoreInvalidMove: false);
            if (worker.FastWheelsTimeLeft > 0)
            {
                var undo2 = ApplySingleMove(state, worker, ignoreInvalidMove: true);
                return () =>
                {
                    undo2();
                    undo();
                };
            }

            return undo;
        }

        private Action ApplySingleMove(State state, Worker worker, bool ignoreInvalidMove)
        {
            var newPosition = worker.Position + Shift;
            if (!newPosition.Inside(state.Map) || state.Map[newPosition] == CellState.Obstacle && worker.DrillTimeLeft == 0)
            {
                if (ignoreInvalidMove)
                    return () => {};
                throw new InvalidOperationException($"Invalid move from {worker.Position} to obstacle {newPosition}. Action: {this}");
            }

            worker.Position += Shift;
            return state.Wrap();
        }
    }
}