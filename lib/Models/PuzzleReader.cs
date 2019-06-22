using System.IO;
using System.Threading.Tasks;
using lib.API;

namespace lib.Models
{
    public static class PuzzleReader
    {
        public static async Task<Puzzle> ReadCurrentFromApiAsync()
        {
            var block = await Api.GetBlockchainBlock();
            return block.Puzzle;
        }

        private static string GetPuzzlePath(int puzzle)
        {
            return Path.Combine(FileHelper.PatchDirectoryName("problems"), "puzzles", $"puzzle-{puzzle:000}.cond");
        }
        
        public static Puzzle ReadFromFile(int puzzle)
        {
            var fileName = GetPuzzlePath(puzzle);
            return new Puzzle(File.ReadAllText(fileName));
        }
    }
}