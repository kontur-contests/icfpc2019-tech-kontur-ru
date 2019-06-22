using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using JsonRpc.Client;
using JsonRpc.Http;
using lib.Models.API;

namespace lib.Models
{
    public static class PuzzleReader
    {
        public static async Task<Puzzle> ReadCurrentFromApiAsync()
        {
            using (var handler = new HttpRpcClientHandler
            {
                EndpointUrl = Api.EndpointUrl
            })
            {
                var client = new JsonRpcClient(handler);
                var response = await client.SendRequestAsync("getmininginfo", null, CancellationToken.None);
                return new Puzzle(response.Result.ToObject<GetMiningInfoResponse>().Puzzle);
            }
        }
        
        public static string GetPuzzlePath(int puzzle)
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