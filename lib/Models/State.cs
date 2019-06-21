using System.Collections.Generic;

namespace lib.Models
{
    public class State
    {
        public State(Worker worker, Map map, List<Booster> boosters)
        {
            Worker = worker;
            Map = map;
            Boosters = boosters;
        }

        public Worker Worker { get; }
        public Map Map { get; }       
        public List<Booster> Boosters { get; }
    }
}