using System.Collections.Generic;
using lib.Models;
using lib.Models.Actions;

namespace lib.Solvers
{
    public interface ISolver
    {
        string GetName();
        int GetVersion();
        List<ActionBase> Solve(State state);
    }
}