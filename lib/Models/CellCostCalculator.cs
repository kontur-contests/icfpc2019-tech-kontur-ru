using System;
using System.Diagnostics;
using System.Linq;
using MongoDB.Driver.Core.Configuration;

namespace lib.Models
{
    public class CellCostCalculator
    {
        private readonly State state;
        private readonly Map<double> costs;

        public CellCostCalculator(State state)
        {
            this.state = state;
            costs = new Map<double>(state.Map.SizeX, state.Map.SizeY);
            for (int x = 0; x < costs.SizeX; x++)
            for (int y = 0; y < costs.SizeY; y++)
            {
                var p = new V(x,y);
                var value = CellObstacleCost(p, state.Map);
                costs[p] = value;
                if (state.Map[p] == CellState.Void)
                    cost += value;
            }
        }

        private double CellObstacleCost(V pos, Map map)
        {
            var wallsCount = 0;
            foreach (var dir in dirs)
            {
                var n = dir + pos;
                if (!n.Inside(map) || map[n] == CellState.Obstacle) wallsCount++;
            }
            return wallsCount;
        }

        public double Cost => cost;

        private Map Map => state.Map;

        public void BeforeWrapCell(V pos)
        {
            cost -= costs[pos];
        }

        public void AfterUnwrapCell(V pos)
        {
            cost += costs[pos];
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