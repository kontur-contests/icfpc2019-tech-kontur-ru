using System;

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

        public override string ToString()
        {
            switch (Type)
            {
                case BoosterType.Extension:
                    return $"B{Position}";
                case BoosterType.Drill:
                    return $"L{Position}";
                case BoosterType.FastWheels:
                    return $"F{Position}";
                case BoosterType.MysteriousPoint:
                    return $"X{Position}";
                case BoosterType.Teleport:
                    return $"R{Position}";
                case BoosterType.Cloning:
                    return $"C{Position}";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}