using System;
using System.Collections.Generic;
using System.Text;
using lib.Models;

namespace lib
{
    public static class Extensions
    {
        public static bool Inside(this V v, Map map)
        {
            return 0 <= v.X && v.X < map.SizeX && 0 <= v.Y && v.Y < map.SizeY;
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

        public static bool IsReachable(this V src, V dest, Map map)
        {
            var dx = dest.X - src.X;
            var dy = dest.Y - src.Y;
            if (Math.Abs(dx) > Math.Abs(dy))
            {
                //var step = Math.Sign(dx);
                //for (int x = src.X + 1; x < dest.X; x += step)
                //{
                //    var y = 
                //}
            }
            throw new NotImplementedException();
        }
    }
}
