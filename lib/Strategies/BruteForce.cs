using System.Collections;
using System.Collections.Generic;
using lib.Models;

namespace lib.Strategies
{
    internal class BruteForce
    {
        private readonly Dictionary<WorkerState, BestMoves> cache = new Dictionary<WorkerState, BestMoves>();
        
        public List<ActionBase> Solve(State state)
        {
            return new List<ActionBase>();
        }

        private void Brute(WorkerState state, BestMoves moves)
        {
            if (cache.TryGetValue(state, out var previousMoves))
            {
                if (moves.Distance >= previousMoves.Distance)
                    return;
            }

            cache[state] = moves;

            for (var direction = 0; direction < 4; direction++)
            {
                var (newState, newMoves) = Go(state, moves, direction);
                Brute(newState, newMoves);
            }

            for (var rotate = 0; rotate < 2; rotate++)
            {
                var (newState, newMoves) = Rotate(state, moves, rotate == 0);
                Brute(newState, newMoves);
            }
        }

        private (WorkerState, BestMoves) Go(WorkerState state, BestMoves moves, int direction)
        {
            var newPosition = state.Position + V.GetShift(direction);
            var newMask = state.Mask;

            var newState = new WorkerState
            {
                Direction = state.Direction,
                Position = newPosition,
                Mask = newMask
            };

            var newMoves = new BestMoves
            {
                Action = new Move(V.GetShift(direction)),
                Distance = moves.Distance + 1,
                Previous = state
            };

            return (newState, newMoves);
        }

        private (WorkerState, BestMoves) Rotate(WorkerState state, BestMoves moves, bool clockwise)
        {
            var newDirection = state.Direction;//todo
            var newMask = state.Mask;

            var newState = new WorkerState
            {
                Direction = state.Direction,
                Position = state.Position,
                Mask = newMask
            };

            var newMoves = new BestMoves
            {
                Action = new Rotate(clockwise),
                Distance = moves.Distance + 1,
                Previous = state
            };

            return (newState, newMoves);
        }

        private class WorkerState
        {
            public V Position;
            public int Direction;
            public BitArray Mask;
        }

        private class BestMoves
        {
            public int Distance;
            public WorkerState Previous;
            public ActionBase Action;
        }
    }
}