using System.Collections;
using System.Collections.Generic;
using lib.Models;

namespace lib.Strategies
{
    public class BruteForce
    {
        private readonly Dictionary<WorkerState, BestMoves> cache = new Dictionary<WorkerState, BestMoves>();
        private Map map;

        public List<ActionBase> Solve(State state)
        {
            map = state.Map;

            Brute(
                new WorkerState
                {
                    Direction = state.Worker.Direction,
                    Mask = new Map(state.Map.SizeX, state.Map.SizeY),
                    Position = state.Worker.Position
                }, 
                new BestMoves
                {
                    Distance = 0
                });

            return new List<ActionBase>();
        }

        private void Brute(WorkerState state, BestMoves moves)
        {
            if (state == null)
                return;
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

            for (var rotate = -1; rotate <= 1; rotate += 2)
            {
                var (newState, newMoves) = Rotate(state, moves, rotate);
                Brute(newState, newMoves);
            }
        }

        private (WorkerState, BestMoves) Go(WorkerState state, BestMoves moves, int direction)
        {
            var newPosition = state.Position + V.GetShift(direction);
            if (!newPosition.Inside(map))
                return (null, null);
            
            var newState = new WorkerState
            {
                Direction = state.Direction,
                Position = newPosition,
                Mask = state.Mask
            };

            Color(newState);

            var newMoves = new BestMoves
            {
                Action = new Move(V.GetShift(direction)),
                Distance = moves.Distance + 1,
                Previous = state
            };

            return (newState, newMoves);
        }

        private (WorkerState, BestMoves) Rotate(WorkerState state, BestMoves moves, int rotate)
        {
            var newDirection = state.Direction.Rotate(rotate);
            
            var newState = new WorkerState
            {
                Direction = newDirection,
                Position = state.Position,
                Mask = state.Mask
            };

            Color(newState);

            var newMoves = new BestMoves
            {
                Action = new Rotate(rotate == 1),
                Distance = moves.Distance + 1,
                Previous = state
            };

            return (newState, newMoves);
        }

        private void Color(WorkerState state)
        {
            state.Mask = state.Mask.Clone();
            state.Mask[state.Position] = CellState.Void;
        }

        private class WorkerState
        {
            public V Position;
            public Direction Direction;
            public Map Mask;

            protected bool Equals(WorkerState other) => Equals(Position, other.Position) && Direction == other.Direction && Equals(Mask, other.Mask);

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj))
                    return false;
                if (ReferenceEquals(this, obj))
                    return true;
                if (obj.GetType() != this.GetType())
                    return false;
                return Equals((WorkerState)obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    var hashCode = (Position != null ? Position.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (int)Direction;
                    hashCode = (hashCode * 397) ^ (Mask != null ? Mask.GetHashCode() : 0);
                    return hashCode;
                }
            }

            public override string ToString() => $"{nameof(Position)}: {Position}, {nameof(Direction)}: {Direction}, {nameof(Mask)}: {Mask}";
        }

        private class BestMoves
        {
            public int Distance;
            public WorkerState Previous;
            public ActionBase Action;
        }
    }
}