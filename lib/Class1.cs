using System;
using System.Collections.Generic;

namespace lib
{
    public class Point
    {
        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; }
        public int Y { get; }
    }

    public class Map
    {
        private readonly bool[,] cells;

        public Map(bool[,] cells)
        {
            this.cells = cells;
        }

        public bool this[Point p] => cells[p.Y, p.X];
    }

    public class Worker
    {
        public Point Position { get; set; }

        public int FreeManipulatorCount { get; set; }
        public int FastWheelsCount { get; set; }
        public int DrillCount { get; set; }
        
        public int FastWheelsTimeLeft { get; set; }
        public int DrillTimeLeft { get; set; }
        
        /// <summary>
        /// relative positions
        /// </summary>
        public List<Point> Manipulators { get; set; }

        public void RotateCW()
        {
            throw new NotImplementedException();
        }
        
        public void RotateCCW()
        {
            throw new NotImplementedException();
        }
    }

    public enum BoosterType
    {
        
    }

    public class Booster
    {
        
    }

    public class State
    {
        public Worker Worker { get; }
        public Map Map { get; }       
        public List<Booster> Boosters { get; }
    }
}