using System;
using lib.Models;
using pipeline;

namespace console_runner
{
    class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                throw new Exception("Invalid argument: empty");
            }
            
            switch (args[0])
            {
                case "add-dummy-meta":
                {
                    const string problemPack = ProblemReader.PART_1_EXAMPLE;
                    const int problemId = 1;
            
                    var reader = new ProblemReader(problemPack);
                    var solutionBlob = reader.ReadSolutionBlob(problemId);
            
                    var meta = new SolutionMeta(
                        problemPack,
                        problemId,
                        solutionBlob,
                        -1,
                        "algo1",
                        1,
                        0.876
                    );
                    meta.SaveToDb();
                    
                    break;
                }
                
                case "check-unchecked":
                {
                    Storage
                        .EnumerateUnchecked()
                        .ForEach(solution => solution.CheckOnline());
                    
                    break;
                }

                default:
                {
                    throw new Exception($"Invalid argument: {args[0]}");
                }
            }
        }
    }
}