using System.Collections.Generic;
using lib.Models;

namespace lib.Solvers
{
    public interface ISolver
    {
        List<ActionBase> Solve(State state);
    }
}