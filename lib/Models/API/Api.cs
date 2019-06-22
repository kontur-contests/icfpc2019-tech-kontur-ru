using System.Threading;
using System.Threading.Tasks;
using JsonRpc.Client;
using JsonRpc.Http;

namespace lib.Models.API
{
    public static class Api
    {
        private const string endpointUrl = "http://icfpc19-crunch1:8332/";

        public static async Task<BlockchainBlock> GetCurrentBlockchainBlock()
        {
            using (var handler = new HttpRpcClientHandler
            {
                EndpointUrl = endpointUrl
            })
            {
                var client = new JsonRpcClient(handler);
                var response = await client.SendRequestAsync("getblockinfo", null, CancellationToken.None);
                return new BlockchainBlock(response.Result.ToObject<GetBlockInfoResponse>());
            }
        }
    }
}