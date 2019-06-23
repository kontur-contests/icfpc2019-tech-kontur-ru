using System;
using System.Collections.Generic;
using System.Linq;

namespace lib.Models
{
    public class Puzzle
    {
        public int BlockNumber;
        public int EpochNumber;
        public int TaskSize;
        public int MinVertices;
        public int MaxVertices;
        public int ManipulatorsCount;
        // ReSharper disable once IdentifierTypo
        public int FastwheelsCount;
        public int DrillsCount;
        // ReSharper disable once IdentifierTypo
        public int TeleportsCount;
        public int ClonesCount;
        public int SpawnsCount;
        public List<V> MustContainPoints;
        public List<V> MustNotContainPoints;

        public Puzzle(string encoded)
        {
            var parts = encoded.Split('#');

            var param = parts[0].Split(',').Select(int.Parse).ToList();
            BlockNumber = param[0];
            EpochNumber = param[1];
            TaskSize = param[2];
            MinVertices = param[3];
            MaxVertices = param[4];
            ManipulatorsCount = param[5];
            FastwheelsCount = param[6];
            DrillsCount = param[7];
            TeleportsCount = param[8];
            ClonesCount = param[9];
            SpawnsCount = param[10];

            MustContainPoints = parts[1]
                .TrimStart('(')
                .TrimEnd(')')
                .Replace("),(", "@")
                .Split('@')
                .Select(
                    x =>
                    {
                        var xy = x.Split(',').Select(int.Parse).ToList();
                        return new V(xy[0], xy[1]);
                    })
                .ToList();

            MustNotContainPoints = parts[2]
                .TrimStart('(')
                .TrimEnd(')')
                .Replace("),(", "@")
                .Split('@')
                .Select(
                    x =>
                    {
                        var xy = x.Split(',').Select(int.Parse).ToList();
                        return new V(xy[0], xy[1]);
                    })
                .ToList();
        }

        public override string ToString()
        {
            return $"{BlockNumber},{EpochNumber},{TaskSize},{MinVertices},{MaxVertices}," +
                   $"{ManipulatorsCount},{FastwheelsCount},{DrillsCount},{TeleportsCount},{ClonesCount},{SpawnsCount}" +
                   $"#{string.Join(",", MustContainPoints)}" +
                   $"#{string.Join(",", MustNotContainPoints)}";
        }
    }
}