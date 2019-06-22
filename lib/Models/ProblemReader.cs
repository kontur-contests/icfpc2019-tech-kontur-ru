using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JsonRpc.Client;
using JsonRpc.Http;
using lib.Models.API;

namespace lib.Models
{
    public static class ProblemReader
    {
        public static string GetProblemPath(int problem)
        {
            return Path.Combine(FileHelper.PatchDirectoryName("problems"), "all", $"prob-{problem:000}.desc");
        }

        public static Problem Read(int problem)
        {
            var fileName = GetProblemPath(problem);
            return Read(File.ReadAllText(fileName));
        }

        public static List<ProblemMeta> ReadAll()
        {
            return Enumerable
                .Range(1, 10000)
                .Where(i => File.Exists(GetProblemPath(i)))
                .Select(i => new ProblemMeta(i, Read(i)))
                .ToList();
        }
        
        public static async Task<Problem> ReadCurrentFromApiAsync()
        {
            var block = await Api.GetCurrentBlockchainBlock();
            return block.Problem;
        }

        public static Problem Read(string source)
        {
            var parts = source.Split('#');
            return new Problem
            {
                Map = ReadMap(parts[0]),
                Point = ReadPoint(parts[1]),
                Obstacles = parts[2].Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries).Select(ReadMap).ToList(),
                Boosters = parts[3].Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries).Select(ReadBooster).ToList()
            };
        }

        private static V ReadPoint(string s)
        {
            return s;
        }

        private static List<V> ReadMap(string s)
        {
            return s.Split(new[] {"),("}, StringSplitOptions.None).Select(ReadPoint).ToList();
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
                case 'R':
                    return new Booster(BoosterType.Teleport, ReadPoint(s.Substring(1)));
                case 'C':
                    return new Booster(BoosterType.Cloning, ReadPoint(s.Substring(1)));
                default:
                    throw new InvalidOperationException($"Unknown booster '{s}'");
            }
        }
    }
}
