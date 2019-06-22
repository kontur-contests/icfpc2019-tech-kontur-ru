using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace lib.Models
{
    public class Problem
    {
        public List<V> Map { get; set; }
        public V Point { get; set; }
        public List<List<V>> Obstacles { get; set; }
        public List<Booster> Boosters { get; set; }

        public override string ToString()
        {
            return $"{string.Join(",", Map)}#{Point}#{string.Join(";", Obstacles.Select(x => string.Join(",", x)))}#{string.Join(";", Boosters)}";
        }

        public bool IsValidForPuzzle(Puzzle puzzle)
        {
            var convertedMap = ProblemConverter.ConvertMap(Map, Obstacles);

            // non-negative coordinates
            if (Map.Any(v => v.X < 0 || v.Y < 0))
                return false;
            
            // no obstacles
            if (Obstacles.Count > 0)
                return false;
            
            // initial position of worker is within M
            if (convertedMap[Point] == CellState.Obstacle)
                return false;
            
            // at least one of maximal dimensions is larger than tSize - floor(0.1*tSize)
            var requiredDimension = puzzle.TaskSize - 0.1 * puzzle.TaskSize;
            if (convertedMap.SizeX < requiredDimension && convertedMap.SizeY < requiredDimension)
                return false;
            
            // area is at least ceil(0.2*tSize^2)
            if (convertedMap.VoidCount() < 0.2 * puzzle.TaskSize * puzzle.TaskSize)
                return false;
            
            // vMin <= number of vertices <= vMax
            if (Map.Count < puzzle.MinVertices || Map.Count > puzzle.MaxVertices)
                return false;
            
            // number of boosters is correct
            if (Boosters.Count(b => b.Type == BoosterType.Extension) != puzzle.ManipulatorsCount)
                return false;
            if (Boosters.Count(b => b.Type == BoosterType.FastWheels) != puzzle.FastwheelsCount)
                return false;
            if (Boosters.Count(b => b.Type == BoosterType.Drill) != puzzle.DrillsCount)
                return false;
            if (Boosters.Count(b => b.Type == BoosterType.Teleport) != puzzle.TeleportsCount)
                return false;
            if (Boosters.Count(b => b.Type == BoosterType.Cloning) != puzzle.ClonesCount)
                return false;
            if (Boosters.Count(b => b.Type == BoosterType.MysteriousPoint) != puzzle.SpawnsCount)
                return false;
            
            // map contains all necessary squares
            if (puzzle.MustContainPoints.Any(p => convertedMap[p] == CellState.Obstacle))
                return false;

            //Console.WriteLine(convertedMap);
            
            // map does not contain unnecessary squares
            if (puzzle.MustNotContainPoints.Any(p => p.Inside(convertedMap) && convertedMap[p] != CellState.Obstacle))
                return false;

            return true;
        }
    }
}