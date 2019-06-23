using System;

namespace lib.Models
{
    public class SubmissionSummary
    {
        public SubmissionSummary(int problemId, int moneySpent, int ourTime)
        {
            ProblemId = problemId;
            MoneySpent = moneySpent;
            OurTime = ourTime;
        }

        public int ProblemId;
        public int MoneySpent;
        public int OurTime;
        public string GeneratedByHost;
        public DateTime GeneratedAt;
    }
}