using System.Collections.Generic;
using System.Linq;

namespace lib.Models
{
    public class Worker
    {
        public Direction Direction { get; set; }
        public V Position { get; set; }
        public int FastWheelsTimeLeft { get; set; }
        public int DrillTimeLeft { get; set; }

        /// <summary>
        /// relative positions
        /// </summary>
        public List<V> Manipulators { get; set; }

        public void NextTurn()
        {
            if (FastWheelsTimeLeft > 0)
                FastWheelsTimeLeft--;
            if (DrillTimeLeft > 0)
                DrillTimeLeft--;
        }

        public override string ToString() => $"{Direction} {Position}, {nameof(FastWheelsTimeLeft)}: {FastWheelsTimeLeft}, {nameof(DrillTimeLeft)}: {DrillTimeLeft}, manipulators: {Manipulators.Count}";

        public Worker Clone()
        {
            var worker = (Worker)MemberwiseClone();
            worker.Manipulators = worker.Manipulators.ToList();
            return worker;
        }

        public List<V> GetManipulators(Direction workerDirection)
        {
            var delta = ((int)workerDirection - (int)Direction + 4) % 4;
            return Manipulators.Select(p => p.RotateAroundZero(delta)).ToList();
        }

        protected bool Equals(Worker other) => 
            Direction == other.Direction 
            && Equals(Position, other.Position) 
            && FastWheelsTimeLeft == other.FastWheelsTimeLeft 
            && DrillTimeLeft == other.DrillTimeLeft 
            && Manipulators.SequenceEqual(other.Manipulators);

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != this.GetType())
                return false;
            return Equals((Worker)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int)Direction;
                hashCode = (hashCode * 397) ^ Position.GetHashCode();
                hashCode = (hashCode * 397) ^ FastWheelsTimeLeft;
                hashCode = (hashCode * 397) ^ DrillTimeLeft;
                hashCode = (hashCode * 397) ^ Manipulators.GetHashCode();
                return hashCode;
            }
        }
    }
}