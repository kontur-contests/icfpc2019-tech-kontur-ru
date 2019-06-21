namespace lib
{
    public class V
    {
        public V(int x, int y)
        {
            X = x;
            Y = y;
        }

        protected bool Equals(V other) => X == other.X && Y == other.Y;

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

        public int X { get; }
        public int Y { get; }
    }
}