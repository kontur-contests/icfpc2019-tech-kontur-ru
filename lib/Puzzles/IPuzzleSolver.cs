using lib.Models;

namespace lib.Puzzles
{
    public interface IPuzzleSolver
    {
        Problem Solve(Puzzle puzzle);
    }
}