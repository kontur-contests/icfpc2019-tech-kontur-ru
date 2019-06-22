using System;

namespace lib.Models
{
    public class SolutionInProgress
    {
        public SolutionInProgress(int problemId, string algorithmId, int algorithmVersion)
        {
            ProblemId = problemId;
            AlgorithmId = algorithmId;
            AlgorithmVersion = algorithmVersion;
        }

        public int ProblemId;
        public string AlgorithmId;
        public int AlgorithmVersion;
        public string HostName;
        public DateTime StartedAt;
    }
}