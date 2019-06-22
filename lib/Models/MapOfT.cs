using System;
using System.Linq;

namespace lib.Models
{
    public class Map<T>
    {
        protected readonly T[,] cells;

        public Map(int sizeX, int sizeY)
        {
            SizeX = sizeX;
            SizeY = sizeY;
            cells = new T[sizeY, sizeX];
        }

        protected Map(int sizeX, int sizeY, T[,] cells)
        {
            SizeX = sizeX;
            SizeY = sizeY;
            this.cells = cells;
        }

        public int SizeX { get; }
        public int SizeY { get; }

        public T this[V p]
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
                            .Select(x => cells[SizeY - y - 1, x])
                            .ToArray();
                        return string.Join("", strings);
                    });
            return string.Join("\n", enumerable);
        }

        public Map<T> Clone()
        {
            return new Map<T>(SizeX, SizeY, (T[,])cells.Clone());
        }
    }
}