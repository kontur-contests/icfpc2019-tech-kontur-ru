using System.Collections.Generic;
using System.Linq;

namespace lib.Models
{
    public class State
    {
        public State(Worker worker, Map map, List<Booster> boosters, int time, int? unwrappedLeft)
        {
            Worker = worker;
            Map = map;
            Boosters = boosters;
            Time = time;
            UnwrappedLeft = unwrappedLeft ?? Map.VoidCount();
        }

        public Worker Worker { get; }
        public Map Map { get; }
        public int UnwrappedLeft { get; set; }
        public List<Booster> Boosters { get; }
        public int Time { get; private set; }

        public void Apply(ActionBase action)
        {
            action.Apply(this);
            Worker.NextTurn();
            Time++;
        }

        public void Apply(IEnumerable<ActionBase> actions)
        {
            foreach (var action in actions)
                Apply(action);
        }

        public void Wrap()
        {
            WrapPoint(Worker.Position);

            foreach (var manipulator in Worker.Manipulators)
            {
                var p = Worker.Position + manipulator;
                if (p.Inside(Map) && Map.IsReachable(p, Worker.Position))
                    WrapPoint(p);
            }

            void WrapPoint(V pp)
            {
                if (Map[pp] == CellState.Void)
                    UnwrappedLeft--;
                Map[pp] = CellState.Wrapped;
            }
        }

        public State Clone()
        {
            return new State(Worker.Clone(), Map.Clone(), Boosters.ToList(), Time, UnwrappedLeft);
        }
    }
}