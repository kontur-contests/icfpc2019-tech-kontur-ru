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

        public override void Apply(State state)
        {
            state.Worker.Direction = (Direction)(((int)state.Worker.Direction + (Clockwise ? 1 : 3)) % 4);
            state.Worker.Manipulators = state.Worker.Manipulators.Select(v => v.RotateAroundZero(Clockwise)).ToList();
            state.Wrap();
        }
    }
}