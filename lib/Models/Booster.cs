namespace lib.Models
{
    public class Booster
    {
        public Booster(BoosterType type, Point position)
        {
            Type = type;
            Position = position;
        }

        public BoosterType Type { get; }
        public Point Position { get; }
    }
}