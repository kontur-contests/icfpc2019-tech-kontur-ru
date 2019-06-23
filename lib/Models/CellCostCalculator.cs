using System;
using System.Diagnostics;
using System.Linq;

namespace lib.Models
{
    public class CellCostCalculator
    {
        private readonly State state;

        public CellCostCalculator(State state)
        {
            this.state = state;
            cost = state.Map.EnumerateCells().Where(c => c.state == CellState.Void).Sum(c => CellCost(c.pos, state.Map));
        }

        public double Cost => cost;

        private Map Map => state.Map;

        public void BeforeWrapCell(V pos)
        {
            var prevCost = 0.0;
            foreach (var dir in dirs)
            {
                var n = dir + pos;
                if (n.Inside(Map) && Map[n] == CellState.Void)
                    prevCost += CellCost2(n, Map)- CellCost(n, Map);
            }
            cost += prevCost - CellCost(pos, Map);
        }

        public void AfterUnwrapCell(V pos)
        {
            var prevCost = 0.0;
            var freeNsCount = 0;
            foreach (var dir in dirs)
            {
                var n = dir + pos;
                if (n.Inside(Map) && Map[n] == CellState.Void)
                {
                    freeNsCount++;
                    prevCost += CellCost(n, Map) - CellCost2(n, Map);
                }
            }
            cost += prevCost + weights[freeNsCount];
        }

        private static V[] dirs = new V[] { "1,0", "-1,0", "0,1", "0,-1" };//, "2,0", "-2,0", "0,2", "0,-2" };

        private static double[] weights = new[] { 3, 2, 1, 0, 0.0 };

        private double cost;

        private double CellCost(V pos, Map map)
        {
            var freeNs = 0;
            foreach (var dir in dirs)
            {
                var n = dir + pos;
                if (n.Inside(map) && map[n] == CellState.Void) freeNs++;
            }
            return weights[freeNs];
        }

        private double CellCost2(V pos, Map map)
        {
            var freeNs = 0;
            foreach (var dir in dirs)
            {
                var n = dir + pos;
                if (n.Inside(map) && map[n] == CellState.Void) freeNs++;
            }
            return weights[freeNs-1];
        }
    }
}