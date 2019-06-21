namespace lib.Models
{
    public class Booster
    {
        public Booster(BoosterType type, V position)
        {
            Type = type;
            Position = position;
        }

        public BoosterType Type { get; }
        public V Position { get; }
    }
}