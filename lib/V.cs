using System;

namespace lib
{
    public class V : IEquatable<V>
    {
        public V(int x, int y)
        {
            X = x;
            Y = y;
        }

        public bool Equals(V other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != this.GetType())
                return false;
            return Equals((V)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (X * 397) ^ Y;
            }
        }

        public override string ToString()
            => $"({X},{Y})";

        public static implicit operator V(string s)
        {
            var parts = s.TrimStart('(').TrimEnd(')').Split(',');
            return new V(int.Parse(parts[0]), int.Parse(parts[1]));
        }

        public int X { get; }
        public int Y { get; }

        public int MLen() => Math.Abs(X) + Math.Abs(Y);

        private static readonly V[] shifts = {new V(1, 0), new V(0, -1), new V(-1, 0), new V(0, 1)};

        public static V GetShift(int direction)
        {
            return shifts[direction];
        }

        public static V operator+(V a, V b) => new V(a.X + b.X, a.Y + b.Y);

        public static V operator-(V a, V b) => new V(a.X - b.X, a.Y - b.Y);

        public static V Zero => new V(0, 0);

        public static bool operator==(V left, V right) => Equals(left, right);

        public static bool operator!=(V left, V right) => !Equals(left, right);

        public static int operator *(V a, V b) => a.X * b.Y - a.Y * b.X;
    }
}