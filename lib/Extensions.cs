using System;
using System.Collections.Generic;
using lib.Models;

namespace lib
{
    public static class Extensions
    {
        // TODO: Fix, should be wrong when using "fast wheels" boost
        public static int CalculateTime(this List<ActionBase> actions)
        {
            return actions.Count;
        }
        
        public static bool Inside(this V v, Map map)
        {
            return 0 <= v.X && v.X < map.SizeX && 0 <= v.Y && v.Y < map.SizeY;
        }

        public static IEnumerable<(V, CellState)> EnumerateCells(this Map map)
        {
            for (int x = 0; x < map.SizeX; x++)
            for (int y = 0; y < map.SizeY; y++)
            {
                var p = new V(x, y);
                yield return (p, map[p]);
            }
        }

        public static V Shift(this V v, int direction)
        {
            return v + V.GetShift(direction);
        }

        public static Direction Rotate(this Direction direction, int delta)
        {
            var newDirection = (4 + ((int)direction + delta) % 4) % 4;
            return (Direction)newDirection;
        }

        public static V RotateAroundZero(this V p, bool clockwise)
        {
            return clockwise
                ? new V(p.Y, -p.X)
                : new V(-p.Y, p.X);
        }

        public static V RotateAroundZero(this V p, int rotationIndex)
        {
            rotationIndex = (4 + rotationIndex % 4) % 4;
            switch (rotationIndex)
            {
                case 0: return p;
                case 1: return new V(p.Y, -p.X);
                case 2: return new V(-p.X, -p.Y);
                case 3: return new V(-p.Y, p.X);
                default: throw new Exception();
            }
        }

        public static TValue GetOrAdd<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, Func<TKey, TValue> createIfNone)
        {
            if (dict.TryGetValue(key, out var value))
                return value;
            var newValue = createIfNone(key);
            dict.Add(key, newValue);
            return newValue;
        }

        public static bool IsReachable(this Map map, V src, V dest)
        {
            if (map[src] == CellState.Obstacle || map[dest] == CellState.Obstacle)
                return false;
            var x0 = src.X;
            var y0 = src.Y;
            var x1 = dest.X;
            var y1 = dest.Y;
            var dx = x1 - x0;
            var dy = y1 - y0;
            if (Math.Abs(dy) == Math.Abs(dx))
            {
                if (dx == 0) return true;
                var ix = (x0 < x1) ? 1 : -1;
                var iy = (y0 < y1) ? 1 : -1;
                for (int y = y0 + iy, x = x0 + ix; y != y1; y += iy, x += ix)
                {
                    if (map[new V(x, y)] == CellState.Obstacle) return false;
                }
            }
            else if (Math.Abs(dy) >= Math.Abs(dx))
            {
                if (dy < 0) return IsReachable(map, dest, src);
                var nx = 2 * dy * x0 + dx + dy;
                for (int y = y0 + 1; y < y1; y++)
                {
                    var x = nx / (2 * dy);
                    if (nx % (2 * dy) != 0 && map[new V(x, y)] == CellState.Obstacle) return false;
                    nx += 2 * dx;
                    x = nx / (2 * dy);
                    if (nx % (2*dy) != 0 && map[new V(x, y)] == CellState.Obstacle) return false;
                }
            }
            else
            {
                if (dx < 0) return IsReachable(map, dest, src);
                var ny = 2 * dx * y0 + dx + dy;
                for (int x = x0 + 1; x < x1; x++)
                {
                    if (ny % (2 * dx) != 0 && map[new V(x, ny / (2 * dx))]== CellState.Obstacle) return false;
                    ny += 2 * dy;
                    if (ny % (2 * dx) != 0 && map[new V(x, ny / (2 * dx))]== CellState.Obstacle) return false;
                }
            }
            return true;
        }
    }
}
