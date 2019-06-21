using System;
using System.Collections.Generic;

namespace lib.Models
{
    public class Worker
    {
        public Point Position { get; set; }

        public int ExtensionCount { get; set; }
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
}