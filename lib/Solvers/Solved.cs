using System;
using System.Collections.Generic;
using System.Linq;
using lib.Models;
using lib.Models.Actions;

namespace lib.Solvers
{
    public class Solved
    {
        public List<BoosterType> Buy { get; set; }
        public List<List<ActionBase>> Actions { get; set; }

        public string FormatSolution() => Actions.Format();
        public string FormatBuy() => Buy?.Format();

        public int CalculateTime() => Actions.CalculateTime();
        
        public int BuyCost()
        {
            return Buy?.Sum(
                b =>
                {
                    switch (b)
                    {
                        case BoosterType.Extension:
                            return 1000;
                        case BoosterType.Drill:
                            return 700;
                        case BoosterType.FastWheels:
                            return 300;
                        case BoosterType.Teleport:
                            return 1200;
                        case BoosterType.Cloning:
                            return 2000;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(b), b, null);
                    }
                }) ?? 0;
        }

    }
}