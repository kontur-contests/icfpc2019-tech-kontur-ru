using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Flurl.Http;
using lib.Models;

namespace lib.API
{
    public static class Api
    {
        private const string endpointUrl = "http://icfpc19-crunch1:8332/";

        public static async Task<BlockchainBlock> GetBlockchainBlock(int blockNumber = -1)
        {
            var prms = blockNumber == -1 ? new List<int>() : new List<int>{blockNumber};
            var response = await endpointUrl
                .PostJsonAsync(new
                {
                    jsonrpc = "2.0",
                    id = "c#",
                    method = "getblockinfo",
                    @params = prms
                })
                .ReceiveJson<UniversalResponse<GetBlockInfoResponse>>();
            return new BlockchainBlock(response.Result);
        }
    }
}