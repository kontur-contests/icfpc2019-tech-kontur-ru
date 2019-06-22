using System;
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
            Time = 0;
            Wrap();
            UnwrappedLeft = Map.VoidCount();
        }

        public Worker Worker { get; private set; }
        public Map Map { get; private set; }
        public int UnwrappedLeft { get; set; }
        public List<Booster> Boosters { get; private set; }
        public int Time { get; private set; }
        
        public int ExtensionCount { get; set; }
        public int FastWheelsCount { get; set; }
        public int DrillCount { get; set; }
        public int TeleportsCount { get; set; }
        public int CloningCount { get; set; }

        public Action Apply(ActionBase action)
        {
            var prevWorker = Worker.Clone();
            var undo = action.Apply(this, Worker);
            Worker.NextTurn();
            Time++;
            return () =>
            {
                Time--;
                Worker = prevWorker;
                undo();
            };
        }

        public Action ApplyRange(IEnumerable<ActionBase> actions)
        {
            var undos = new List<Action>();
            foreach (var action in actions)
                undos.Add(Apply(action));
            return () =>
            {
                for (var i = undos.Count - 1; i >= 0; i--)
                {
                    var action = undos[i];
                    action();
                }
            };
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
            var clone = (State)MemberwiseClone();
            clone.Map = clone.Map.Clone();
            clone.Worker = clone.Worker.Clone();
            clone.Boosters = clone.Boosters.ToList();
            return clone;
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
                        ExtensionCount++;
                        break;
                    case BoosterType.FastWheels:
                        FastWheelsCount++;
                        break;
                    case BoosterType.Drill:
                        DrillCount++;
                        break;
                    case BoosterType.Teleport:
                        TeleportsCount++;
                        break;
                    case BoosterType.Cloning:
                        CloningCount++;
                        break;
                }
                Boosters.Remove(booster);
            }
            return () =>
            {
                foreach (var booster in boostersToCollect)
                {
                    switch (booster.Type)
                    {
                        case BoosterType.Extension:
                            ExtensionCount--;
                            break;
                        case BoosterType.FastWheels:
                            FastWheelsCount--;
                            break;
                        case BoosterType.Drill:
                            DrillCount--;
                            break;
                        case BoosterType.Teleport:
                            TeleportsCount--;
                            break;
                        case BoosterType.Cloning:
                            CloningCount--;
                            break;
                    }
                    Boosters.Remove(booster);
                }
                Boosters.AddRange(boostersToCollect);
            };
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