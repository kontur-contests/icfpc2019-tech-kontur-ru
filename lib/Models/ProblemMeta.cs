using System.Collections.Generic;
using System.Linq;

namespace lib.Models
{
    public class ProblemMeta
    {
        public ProblemMeta(int problemId, Problem problem)
        {
            ProblemId = problemId;
            Problem = problem;
        }

        public int ProblemId;
        public Problem Problem;
    }
}