using System.Collections.Generic;

namespace lib.Models
{
    public class Problem
    {
        public List<V> Map { get; set; }
        public V Point { get; set; }
        public List<List<V>> Obstacles { get; set; }
        public List<Booster> Boosters { get; set; }
    }
}