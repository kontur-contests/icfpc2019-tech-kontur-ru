using System;
using System.Linq;
using lib.API;
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

                    var minDeltaOption = command.Option(
                        "-d|--min-delta",
                        $"Override minimum delta (default {Common.defaultMinDelta})",
                        CommandOptionType.SingleValue);
                    
                    command.OnExecute(
                        () =>
                        {
                            Storage
                                .EnumerateBestSolutions(Api.GetBalance().GetAwaiter().GetResult(), minDeltaOption.HasValue() ? int.Parse(minDeltaOption.Value()) : Common.defaultMinDelta)
                                .OrderBy(s => s.ProblemId)
                                .ToList()
                                .ForEach(solution => { Console.WriteLine($"{solution.ProblemId}, {solution.OurTime}, Ok"); });

                            return 0;
                        });
                });
        }
    }
}