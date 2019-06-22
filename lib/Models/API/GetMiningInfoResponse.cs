using System.Collections.Generic;

namespace lib.Models.API
{
    public class GetMiningInfoResponse
    {
        public int Block;
        public List<string> Excluded;
        public string Puzzle;
        public string Task;
    }
}