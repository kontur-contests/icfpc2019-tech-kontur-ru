using System;
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
            Wrap();
            UnwrappedLeft = unwrappedLeft ?? Map.VoidCount();
        }

        public Worker Worker { get; private set; }
        public Map Map { get; }
        public int UnwrappedLeft { get; set; }
        public List<Booster> Boosters { get; }
        public int Time { get; private set; }

        public Action Apply(ActionBase action)
        {
            var prevWorker = Worker.Clone();
            var undo = action.Apply(this);
            Worker.NextTurn();
            Time++;
            return () =>
            {
                Time--;
                Worker = prevWorker;
                undo();
            };
        }

        public void Apply(IEnumerable<ActionBase> actions)
        {
            foreach (var action in actions)
                Apply(action);
        }

        public Action Wrap()
        {
            var res = new List<(V pos, CellState oldState)>();
            WrapPoint(Worker.Position);

            foreach (var manipulator in Worker.Manipulators)
            {
                var p = Worker.Position + manipulator;
                if (p.Inside(Map) && Map.IsReachable(p, Worker.Position))
                    WrapPoint(p);
            }

            void WrapPoint(V pp)
            {
                res.Add((pp, Map[pp]));
                if (Map[pp] == CellState.Void)
                    UnwrappedLeft--;
                Map[pp] = CellState.Wrapped;
            }

            return () => Unwrap(res);
        }

        public State Clone()
        {
            return new State(Worker.Clone(), Map.Clone(), Boosters.ToList(), Time, UnwrappedLeft);
        }

        public Action CollectBoosters()
        {
            var boostersToCollect = Boosters
                .Where(b => b.Position == Worker.Position && b.Type != BoosterType.MysteriousPoint)
                .ToList();
            foreach (var booster in boostersToCollect)
            {
                switch (booster.Type)
                {
                    case BoosterType.Extension:
                        Worker.ExtensionCount++;
                        break;
                    case BoosterType.FastWheels:
                        Worker.FastWheelsCount++;
                        break;
                    case BoosterType.Drill:
                        Worker.DrillCount++;
                        break;
                    case BoosterType.Teleport:
                        Worker.TeleportsCount++;
                        break;
                }
                Boosters.Remove(booster);
            }
            return () => Boosters.AddRange(boostersToCollect);
        }

        public void Unwrap(List<(V pos, CellState oldState)> wrappedCells)
        {
            foreach (var wrappedCell in wrappedCells)
            {
                Map[wrappedCell.pos] = wrappedCell.oldState;
                if (wrappedCell.oldState == CellState.Void) UnwrappedLeft++;
            }
        }
    }
}