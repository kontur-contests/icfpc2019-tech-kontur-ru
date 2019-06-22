using System;
using System.Linq;

namespace lib.Models
{
    public class Rotate : ActionBase
    {
        public Rotate(bool clockwise)
        {
            Clockwise = clockwise;
        }

        public bool Clockwise { get; }
        
        public override string ToString()
        {
            return Clockwise ? "E" : "Q";
        }

        public override Action Apply(State state, Worker worker)
        {
            worker.Direction = (Direction)(((int)worker.Direction + (Clockwise ? 1 : 3)) % 4);
            worker.Manipulators = worker.Manipulators.Select(v => v.RotateAroundZero(Clockwise)).ToList();
            return state.Wrap();
        }
    }
}