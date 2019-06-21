namespace lib.Models
{
    public class Rotate : ActionBase
    {
        public Rotate(bool clockwise)
        {
            Clockwise = clockwise;
        }

        public bool Clockwise { get; }
    }
}