using System.Linq;

namespace lib.Models
{
    public class Map : Map<CellState>
    {
        public Map(int sizeX, int sizeY)
            : base(sizeX, sizeY)
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
    }
}