using System.Linq;

namespace lib.Models
{
    public enum CellState
    {
        Obstacle,
        Void,
        Wrapped
    }

    public class Map
    {
        protected bool Equals(Map other)
        {
            for (int x = 0; x < SizeX; x++)
            for (int y = 0; y < SizeY; y++)
                if (cells[y, x] != other[new V(x, y)])
                    return false;
            return true;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != this.GetType())
                return false;
            return Equals((Map)obj);
        }

        public override int GetHashCode()
        {
            if (cells == null)
                return 0;

            unchecked
            {
                int result = 0;
                for (int x = 0; x < SizeX; x++)
                for (int y = 0; y < SizeY; y++)
                    result = result * 37 + (int)cells[x, y];

                return result;
            }
        }

        private readonly CellState[,] cells;

        public Map(int sizeX, int sizeY)
        {
            SizeX = sizeX;
            SizeY = sizeY;
            cells = new CellState[sizeY, sizeX];
        }

        private Map(int sizeX, int sizeY, CellState[,] cells)
        {
            SizeX = sizeX;
            SizeY = sizeY;
            this.cells = cells;
        }

        public int SizeX { get; }
        public int SizeY { get; }

        public CellState this[V p]
        {
            get => cells[p.Y, p.X];
            set => cells[p.Y, p.X] = value;
        }

        public override string ToString()
        {
            var enumerable = Enumerable
                .Range(0, SizeY)
                .Select(
                    y =>
                    {
                        var strings = Enumerable
                            .Range(0, SizeX)
                            .Select(x => cells[SizeY - y - 1, x] == CellState.Void ? "."
                                : cells[SizeY - y - 1, x] == CellState.Obstacle ? "#" 
                                : "*")
                            .ToArray();
                        return string.Join("", strings);
                    });
            return string.Join("\n", enumerable);
        }

        public Map Clone()
        {
            return new Map(SizeX, SizeY, (CellState[,])cells.Clone());
        }
    }
}