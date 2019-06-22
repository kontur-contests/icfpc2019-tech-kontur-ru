using System.Collections.Generic;
using lib.Models.API;

namespace lib.Models
{
    public class BlockchainBlock
    {
        public int BlockNumber;
        public int BlockSubmissions;
        public double BlockTimestamp;
        public List<string> ExcludedTeams;
        public Puzzle Puzzle;
        public Problem Problem;

        public BlockchainBlock(GetBlockInfoResponse dtoResponse)
        {
            BlockNumber = dtoResponse.Block;
            BlockSubmissions = dtoResponse.BlockSubs;
            BlockTimestamp = dtoResponse.BlockTs;
            ExcludedTeams = dtoResponse.Excluded;
            Puzzle = new Puzzle(dtoResponse.Puzzle);
            Problem = ProblemReader.Read(dtoResponse.Task);
        }
    }
}