using System;
using System.Collections.Generic;
using System.Linq;
using lib.Models;

namespace lib.Puzzles
{
    public static class PuzzleConverter
    {
        public static List<V> ConvertMapToPoints(Map<Cell> map)
        {
            V start = null;
            for (int y = 0; y < map.SizeY && start == null; y++)
            {
                for (int x = 0; x < map.SizeX && start == null; x++)
                    if (map[new V(x, y)] == Cell.Inside)
                        start = new V(x, y);
            }

            var p = start;
            var d = Direction.Up;
            var result = new List<V>();

            while (p != start || result.Count == 0)
            {
                result.Add(p);

                int x = p.X, y = p.Y;

                if (d == Direction.Up)
                {
                    if (Has(x, y))
                        d = Direction.Right;
                    else if (Has(x - 1, y))
                        d = Direction.Up;
                    else
                        d = Direction.Left;
                }
                else
                if (d == Direction.Down)
                {
                    if (Has(x - 1, y - 1))
                        d = Direction.Left;
                    else if (Has(x, y - 1))
                        d = Direction.Down;
                    else
                        d = Direction.Right;
                }
                else
                if (d == Direction.Right)
                {
                    if (Has(x, y - 1))
                        d = Direction.Down;
                    else if (Has(x, y))
                        d = Direction.Right;
                    else
                        d = Direction.Up;
                }
                else
                if (d == Direction.Left)
                {
                    if (Has(x - 1, y))
                        d = Direction.Up;
                    else if (Has(x - 1, y - 1))
                        d = Direction.Left;
                    else
                        d = Direction.Down;
                }

                p = p.Shift((int)d);
            }

            bool Has(int x, int y)
            {
                var pp = new V(x, y);
                return pp.Inside(map) && map[pp] == Cell.Inside;
            }

            int sz = 0;
            while (sz != result.Count)
            {
                sz = result.Count;

                for (int i = 0; i < result.Count; i++)
                {
                    var a = i == 0 ? result.Last() : result[i - 1];
                    var b = result[i];
                    var c = result[(i + 1) % result.Count];

                    if ((b - a) * (c - b) == 0)
                    {
                        result.RemoveAt(i);
                        break;
                    }
                }
            }

            return result;
        }
    }
}