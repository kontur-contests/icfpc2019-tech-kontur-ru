using System;
using System.Linq;
using Microsoft.Extensions.CommandLineUtils;
using pipeline;

namespace console_runner.Commands
{
    public static class CsvCommand
    {
        public static void Register(CommandLineApplication app)
        {
            app.Command(
                "csv",
                (command) =>
                {
                    command.Description = "Generate expected csv that can be compared to organizers csv";
                    command.HelpOption("-?|-h|--help");

                    command.OnExecute(
                        () =>
                        {
                            Storage
                                .EnumerateBestSolutions()
                                .OrderBy(s => s.ProblemId)
                                .ToList()
                                .ForEach(solution => { Console.WriteLine($"{solution.ProblemId}, {solution.OurTime}, Ok"); });

                            return 0;
                        });
                });
        }
    }
}