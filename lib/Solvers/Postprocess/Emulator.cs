using System;
using System.Collections.Generic;
using System.Linq;
using lib.Models;
using lib.Models.Actions;

namespace lib.Solvers.Postprocess
{
    public static class Emulator
    {
        public static Solved ParseSolved(string sol, string buy)
        {
            return new Solved
            {
                Actions = sol.Split('#').Select(ParseActions).ToList(),
                Buy = string.IsNullOrEmpty(buy) ? null : buy.ToBuyBoosters().ToList()
            };
        }

        private static List<ActionBase> ParseActions(string s)
        {
            var actions = new List<ActionBase>();
            for (int i = 0; i < s.Length; i++)
            {
                switch (s[i])
                {
                    case 'W': actions.Add(new Move("0,1")); break;
                    case 'S': actions.Add(new Move("0,-1")); break;
                    case 'A': actions.Add(new Move("-1,0")); break;
                    case 'D': actions.Add(new Move("1,0")); break;
                    case 'E': actions.Add(new Rotate(true)); break;
                    case 'Q': actions.Add(new Rotate(false)); break;
                    case 'C': actions.Add(new UseCloning()); break;
                    case 'L': actions.Add(new UseDrill()); break;
                    case 'F': actions.Add(new UseFastWheels()); break;
                    case 'Z': actions.Add(new Wait()); break;
                    case 'R': actions.Add(new UseTeleport()); break;
                    case 'T': actions.Add(new Shift(ParsePoint(s, ref i))); break;
                    case 'B': actions.Add(new UseExtension(ParsePoint(s, ref i))); break;
                }
            }

            return actions;
        }

        private static V ParsePoint(string s, ref int p0)
        {
            var index = s.IndexOf(')', p0);
            if (index == -1)
                throw new InvalidOperationException();
            var res = s.Substring(p0 + 1, index - p0);
            p0 = index; 
            return res;
        }

        public static void Emulate(State state, Solved solved)
        {
            if (solved.Buy != null)
                state.BuyBoosters(solved.Buy.ToArray());

            var ix = new List<int> {0};
            while (true)
            {
                var ixCount = ix.Count;
                var anyActed = false;
                var actions = new List<ActionBase>();
                for (var workerIndex = 0; workerIndex < ixCount; workerIndex++)
                {
                    var actionIndex = ix[workerIndex];
                    if (actionIndex >= solved.Actions[workerIndex].Count)
                        continue;
                    actions.Add(solved.Actions[workerIndex][actionIndex]);
                    ix[workerIndex]++;
                    anyActed = true;
                    if (solved.Actions[workerIndex][actionIndex] is UseCloning)
                        ix.Add(0);
                }

                if (!anyActed)
                    break;

                state.Apply(state.Workers.Select((w, i) => (w, actions[i])).ToList());
            }
        }
    }
}