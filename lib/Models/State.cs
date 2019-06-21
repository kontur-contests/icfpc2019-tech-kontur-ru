using System.Collections.Generic;

namespace lib.Models
{
    public class State
    {
        public Worker Worker { get; }
        public Map Map { get; }       
        public List<Booster> Boosters { get; }
    }
}