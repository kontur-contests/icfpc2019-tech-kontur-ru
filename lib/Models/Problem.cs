using System.Collections.Generic;
using System.Linq;

namespace lib.Models
{
    public class Problem
    {
        public List<V> Map { get; set; }
        public V Point { get; set; }
        public List<List<V>> Obstacles { get; set; }
        public List<Booster> Boosters { get; set; }

        public override string ToString()
        {
            return $"{string.Join(",", Map)}#{Point}#{string.Join(";", Obstacles.Select(x => string.Join(",", x)))}#{string.Join(";", Boosters)}";
        }
    }
}