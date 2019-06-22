using System.Collections.Generic;

namespace lib.API
{
    public class GetBlockInfoResponse
    {
        public int Block;
        public int BlockSubs;
        public double BlockTs;
        public List<string> Excluded;
        public string Puzzle;
        public string Task;
    }
}