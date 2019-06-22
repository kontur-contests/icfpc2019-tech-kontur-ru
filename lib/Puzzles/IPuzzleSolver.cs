using lib.Models;

namespace lib.Puzzles
{
    internal interface IPuzzleSolver
    {
        Problem Solve(Puzzle puzzle);
    }
}