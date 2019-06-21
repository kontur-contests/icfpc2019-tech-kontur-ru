using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MongoDB.Driver.Core.Misc;

namespace lib.Models
{
    public class ProblemReader
    {
        public const string PART_1_INITIAL = "part-1-initial";
        public const string PART_1_EXAMPLE = "part-1-examples";
        
        public static ProblemReader Current = new ProblemReader(PART_1_INITIAL);

        public string Pack { get; }

        public ProblemReader(string pack)
        {
            Pack = pack;
        }

        public string GetProblemPath(int problem)
        {
            return Path.Combine(FileHelper.PatchDirectoryName("problems"), Pack, $"prob-{problem:000}.desc");
        }

        public Problem Read(int problem)
        {
            var fileName = GetProblemPath(problem);
            return Read(File.ReadAllText(fileName));
        }

        public List<ProblemMeta> ReadAll()
        {
            return Enumerable
                .Range(1, 150)
                .Select(i => new ProblemMeta(Pack, i, Read(i)))
                .ToList();
        }

        public string GetSolutionPath(int problem)
        {
            return Path.Combine(FileHelper.PatchDirectoryName("problems"), Pack, $"prob-{problem:000}.sol");
        }

        public string ReadSolutionBlob(int problem)
        {
            var fileName = GetSolutionPath(problem);
            return File.ReadAllText(fileName);
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
                default:
                    throw new InvalidOperationException($"Unknown booster '{s}'");
            }
        }
    }
}
