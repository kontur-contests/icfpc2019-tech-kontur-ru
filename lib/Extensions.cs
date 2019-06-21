using System;
using System.Collections.Generic;
using System.Text;
using lib.Models;

namespace lib
{
    public static class Extensions
    {
        public static V RotateAroundZero(this V p, bool clockwise)
        {
            return clockwise 
                ? new V(p.Y, -p.X) 
                : new V(-p.Y, p.X);
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
            }
            throw new NotImplementedException();
        }

    }
}
