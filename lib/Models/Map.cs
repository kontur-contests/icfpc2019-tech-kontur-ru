using System.Collections.Generic;
using System.Linq;

namespace lib.Models
{
    public class Map
    {
        private readonly bool[,] cells;

        public Map(int sizeX, int sizeY)
        {
            SizeX = sizeX;
            SizeY = sizeY;
            cells = new bool[sizeY, sizeX];
        }

        public int SizeX { get; }
        public int SizeY { get; }

        public bool this[V p]
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
                            .Select(x => cells[SizeY - y - 1, x] ? "." : "#")
                            .ToArray();
                        return string.Join("", strings);
                    });
            return string.Join("\n", enumerable);
        }
    }
}