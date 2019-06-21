using System.Collections.Generic;
using System.Linq;

namespace lib.Models
{
    public class ProblemMeta
    {
        public ProblemMeta(string problemPack, int problemId, Problem problem)
        {
            ProblemPack = problemPack;
            ProblemId = problemId;
            Problem = problem;
        }

        public string ProblemPack;
        public int ProblemId;
        public Problem Problem;
    }
}