using System.Collections.Generic;
using lib.Models;

namespace lib.Solvers
{
    public interface ISolver
    {
        string GetName();
        int GetVersion();
        List<ActionBase> Solve(State state);
    }
}