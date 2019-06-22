using lib.Models;

namespace lib.Puzzles
{
    internal interface IPuzzleSolver
    {
        Map<bool> Solve(Puzzle puzzle);
    }
}