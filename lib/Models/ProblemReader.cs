using System;
using System.Collections.Generic;
using System.Linq;

namespace lib.Models
{
    public static class ProblemReader
    {
        public static Problem Read(string source)
        {
            var parts = source.Split('#');
            return new Problem
            {
                Map = ReadMap(parts[0]),
                Point = ReadPoint(parts[1]),
                Obstacles = parts[2].Split(';').Select(ReadMap).ToList(),
                Boosters = parts[3].Split(';').Select(ReadBooster).ToList(),
            };
        }

        private static V ReadPoint(string s)
        {
            var parts = s.TrimStart('(').TrimEnd(')').Split(',');
            return new V(int.Parse(parts[0]), int.Parse(parts[1]));
        }

        private static List<V> ReadMap(string s)
        {
            return s.Split(new[]{"),("}, StringSplitOptions.None).Select(ReadPoint).ToList();
        }

        private static Booster ReadBooster(string s)
        {
            switch (s[0])
            {
                case 'B':
                    return new Booster(BoosterType.Extension, ReadPoint(s.Substring(1)));
                case 'F':
                    return new Booster(BoosterType.FastWheels, ReadPoint(s.Substring(1)));
                case 'L':
                    return new Booster(BoosterType.Drill, ReadPoint(s.Substring(1)));
                case 'X':
                    return new Booster(BoosterType.MysteriousPoint, ReadPoint(s.Substring(1)));
                default:
                    throw new InvalidOperationException($"Unknown booster '{s}'");
            }
        }
    }
}