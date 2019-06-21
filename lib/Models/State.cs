using System.Collections.Generic;
using System.Linq;

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

        public void Apply(ActionBase action)
        {
            action.Apply(this);
            Worker.NextTurn();
        }

        public void Apply(IEnumerable<ActionBase> actions)
        {
            foreach (var action in actions)
            {
                Apply(action);
            }
        }

        public void Wrap()
        {
            Map[Worker.Position] = CellState.Wrapped;
            foreach (var manipulator in Worker.Manipulators)
            {
                var p = Worker.Position + manipulator;
                if (p.Inside(Map) && Map.IsReachable(p, Worker.Position))
                    Map[p] = CellState.Wrapped;
            }
        }

        public State Clone()
        {
            return new State(Worker.Clone(), Map.Clone(), Boosters.ToList());
        }
    }
}
