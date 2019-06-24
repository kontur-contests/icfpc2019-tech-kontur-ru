using System;
using System.Collections.Generic;
using lib.Models.Actions;
using lib.Solvers;

namespace lib.Models
{
    public class History
    {
        public List<TickWorkerState> Ticks { get; set; } = new List<TickWorkerState>();

        public Solved BuildSolved()
        {
            var actions = new List<List<ActionBase>> {new List<ActionBase>()};

            for (var i = 1; i < Ticks.Count; i++)
            {
                var tick = Ticks[i];
                var prev = Ticks[i - 1];
                if (tick.Position != prev.Position)
                {
                    if (tick.Direction != prev.Direction)
                        throw new InvalidOperationException("tick.Direction != prev.Direction");
                    var shift = tick.Position - prev.Position;
                    if (shift.MLen() != 1)
                        throw new InvalidOperationException("shift.MLen() != 1");
                    actions[0].Add(new Move(shift));
                }
                else if (tick.Direction != prev.Direction)
                {
                    if (tick.Position != prev.Position)
                        throw new InvalidOperationException("tick.Position != prev.Position");

                    if (prev.Direction.Rotate(1) == tick.Direction)
                    {
                        actions[0].Add(new Rotate(true));
                    }
                    else if (prev.Direction.Rotate(-1) == tick.Direction)
                    {
                        actions[0].Add(new Rotate(false));
                    }
                    else
                        throw new InvalidOperationException("tick.Direction == prev.Direction + 2");
                }
                else if (tick.Action is UseExtension)
                {
                    actions[0].Add(tick.Action);
                }

            }

            return new Solved
            {
                Actions = actions
            };
        }
    }
}