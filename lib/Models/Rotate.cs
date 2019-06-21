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
            throw new System.NotImplementedException();
        }
    }
}