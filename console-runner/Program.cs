using System;
using lib.Models;
using Microsoft.Extensions.CommandLineUtils;
using pipeline;

namespace console_runner
{
    class Program
    {
        public static void Main(string[] args)
        {
            var app = new CommandLineApplication();
            app.Name = "console-runner";
            app.HelpOption("-?|-h|--help");
            
            app.OnExecute(() =>
            {
                Console.WriteLine("Invalid argument: empty");
                return 1;
            });
            
            app.Command("add-dummy-meta", (command) =>
            {
                command.Description = "Add an example non-checked solution to DB";
                command.HelpOption("-?|-h|--help");

                command.OnExecute(() =>
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

                    return 0;
                });
            });

            app.Command("check-unchecked", (command) =>
            {
                command.Description = "Check all unchecked solution with official checker";
                command.HelpOption("-?|-h|--help");

                command.OnExecute(() =>
                {
                    Storage
                        .EnumerateUnchecked()
                        .ForEach(solution => solution.CheckOnline());

                    return 0;
                });
            });
            
            app.Execute(args);
        }
    }
}