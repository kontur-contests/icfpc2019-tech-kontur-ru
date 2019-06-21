using lib.Models;
using pipeline;

namespace console_runner
{
    class Program
    {
        public static void Main()
        {
            var s = new SolutionMeta("1", "ABCDABCD", 800, "algo1", 1, 0.876);
            s.SaveToDb();
        }
    }
}