using System;
using System.Threading;
using System.Threading.Tasks;
using JsonRpc.Client;
using JsonRpc.Http;
using lib.Models.API;

namespace lib.Models
{
    public static class PuzzleReader
    {
        public static async Task<Puzzle> GetCurrentPuzzleFromApiAsync()
        {
            using (var handler = new HttpRpcClientHandler
            {
                EndpointUrl = $"http://{host}:{port}/"
            })
            {
                var client = new JsonRpcClient(handler);
                var response = await client.SendRequestAsync("getmininginfo", null, CancellationToken.None);
                return new Puzzle(response.Result.ToObject<GetMiningInfoResponse>().Puzzle);
            }
        }
        
        private const int port = 8332;
        private const string host = "localhost";
    }
}