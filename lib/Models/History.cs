using System;
using System.Collections.Generic;
using System.Linq;
using lib.Models.Actions;
using lib.Solvers;

namespace lib.Models
{
    public class History
    {
        public List<WorkerHistory> Workers { get; set; } = new List<WorkerHistory>();

        public int CalculateTime()
        {
            return Workers.Max(x => x.StartTick + x.Ticks.Count - 1);
        }

        public Solved BuildSolved()
        {
            var actions = Enumerable.Range(0, Workers.Count).Select(_ => new List<ActionBase>()).ToList();

            for (var w = 0; w < Workers.Count; w++)
            {
                for (var i = 1; i < Workers[w].Ticks.Count; i++)
                {
                    var tick = Workers[w].Ticks[i];
                    var prev = Workers[w].Ticks[i - 1];
                    if (tick.Position != prev.Position)
                    {
                        if (tick.Direction != prev.Direction)
                            throw new InvalidOperationException("tick.Direction != prev.Direction");
                        var shift = tick.Position - prev.Position;
                        if (shift.MLen() != 1)
                            throw new InvalidOperationException("shift.MLen() != 1");
                        actions[w].Add(new Move(shift));
                    }
                    else if (tick.Direction != prev.Direction)
                    {
                        if (tick.Position != prev.Position)
                            throw new InvalidOperationException("tick.Position != prev.Position");

                        if (prev.Direction.Rotate(1) == tick.Direction)
                        {
                            actions[w].Add(new Rotate(true));
                        }
                        else if (prev.Direction.Rotate(-1) == tick.Direction)
                        {
                            actions[w].Add(new Rotate(false));
                        }
                        else
                            throw new InvalidOperationException("tick.Direction == prev.Direction + 2");
                    }
                    else if (tick.Action is UseExtension || tick.Action is UseCloning)
                    {
                        actions[w].Add(tick.Action);
                    }

                }
            }

            return new Solved
            {
                Actions = actions
            };
        }
    }
}