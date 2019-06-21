using System.Collections.Generic;
using System.Linq;

namespace lib.Models
{
    public class Worker
    {
        public Direction Direction { get; set; }

        public List<V> GetManipulators(Direction workerDirection)
        {
            var delta = ((int)workerDirection - (int)Direction + 4) % 4;
            return Manipulators.Select(p => p.RotateAroundZero(delta)).ToList();
        }

        public V Position { get; set; }

        public int ExtensionCount { get; set; }
        public int FastWheelsCount { get; set; }
        public int DrillCount { get; set; }
        
        public int FastWheelsTimeLeft { get; set; }
        public int DrillTimeLeft { get; set; }

        /// <summary>
        /// relative positions
        /// </summary>
        public List<V> Manipulators { get; set; }

        public void NextTurn()
        {
            if (FastWheelsTimeLeft > 0)
                FastWheelsTimeLeft--;
            if (DrillTimeLeft > 0)
                DrillTimeLeft--;
        }

        public Worker Clone()
        {
            var worker = (Worker)MemberwiseClone();
            worker.Manipulators = worker.Manipulators.ToList();
            return worker;
        }
    }
}