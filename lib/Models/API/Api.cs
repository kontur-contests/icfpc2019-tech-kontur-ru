using System.Threading;
using System.Threading.Tasks;
using JsonRpc.Client;
using JsonRpc.Http;

namespace lib.Models.API
{
    public class Api
    {
        public const string EndpointUrl = "http://localhost:8332/";

        public static async Task<BlockchainBlock> GetCurrentBlockchainBlock()
        {
            using (var handler = new HttpRpcClientHandler
            {
                EndpointUrl = EndpointUrl
            })
            {
                var client = new JsonRpcClient(handler);
                var response = await client.SendRequestAsync("getblockinfo", null, CancellationToken.None);
                return new BlockchainBlock(response.Result.ToObject<GetBlockInfoResponse>());
            }
        }
    }
}