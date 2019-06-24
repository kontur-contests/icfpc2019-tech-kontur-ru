using System;
using System.Collections.Generic;
using System.Linq;
using lib.Models;
using lib.Solvers;
using Microsoft.Extensions.CommandLineUtils;

namespace console_runner.Commands
{
    public static class SolveCommand
    {
        public static void Register(CommandLineApplication app)
        {
            app.Command(
                "solve",
                (command) =>
                {
                    command.Description = "Solve all problems with all solvers";
                    command.HelpOption("-?|-h|--help");

                    var solverOption = command.Option(
                        "-s|--solver",
                        "Solver name prefix",
                        CommandOptionType.SingleValue);

                    var problemsOption = command.Option(
                        "-p|--problems",
                        "Single problem id or problem ids range",
                        CommandOptionType.SingleValue);

                    command.OnExecute(
                        () =>
                        {
                            var solvers = RunnableSolvers
                                .Enumerate()
                                .Select(x => x.Invoke())
                                .Where(x => !solverOption.HasValue() || solverOption.HasValue() && x.GetName().StartsWith(solverOption.Value()))
                                .ToList();

                            var problemIds = new List<int>();
                            if (problemsOption.HasValue())
                            {
                                if (int.TryParse(problemsOption.Value(), out var problemId))
                                    problemIds.Add(problemId);
                                else
                                {
                                    var parts = problemsOption.Value().Split(new []{".."}, StringSplitOptions.RemoveEmptyEntries);
                                    var pStart = int.Parse(parts[0]);
                                    var pEnd = int.Parse(parts[1]);
                                    problemIds.AddRange(Enumerable.Range(pStart, pEnd - pStart + 1));
                                    Console.WriteLine($"Will solve problems: {string.Join(", ", problemIds)}");
                                }
                            }

                            solvers.ForEach(
                                solver =>
                                {
                                    ProblemReader
                                        .ReadAll()
                                        .Where(x => !problemIds.Any() || problemIds.Contains(x.ProblemId))
                                        .ToList()
                                        .ForEach(problemMeta => Common.Solve(solver, problemMeta));
                                });

                            return 0;
                        });
                });
        }
    }
}