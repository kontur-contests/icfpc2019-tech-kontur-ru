using System.Linq;

namespace lib.Models
{
    public class Map : Map<CellState>
    {
        public Map(int sizeX, int sizeY)
            : base(sizeX, sizeY)
        {
        }

        protected Map(int sizeX, int sizeY, CellState[,] cells)
            : base(sizeX, sizeY, cells)
        {
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
                            .Select(x => cells[SizeY - y - 1, x] == CellState.Obstacle ? "#" : cells[SizeY - y - 1, x] == CellState.Void ? "." : "*")
                            .ToArray();
                        return string.Join("", strings);
                    });
            return string.Join("\n", enumerable);
        }
        
        public Map Clone()
        {
            return new Map(SizeX, SizeY, (CellState[,])cells.Clone());
        }

        public int VoidCount()
        {
            var result = 0;
            for (int x = 0; x < SizeX; x++)
            for (int y = 0; y < SizeY; y++)
            {
                if (this[new V(x, y)] == CellState.Void)
                    result++;
            }

            return result;
        }
    }
}