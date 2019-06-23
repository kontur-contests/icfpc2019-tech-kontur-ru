using lib.Models;

namespace lib.Solvers
{
    public interface ISolver
    {
        string GetName();
        int GetVersion();
        Solved Solve(State state);
    }
}