using System;
using System.Linq;
using System.Threading.Tasks;
using lib.Models;
using lib.Solvers;
using Microsoft.Extensions.CommandLineUtils;
using pipeline;

namespace console_runner.Commands
{
    public static class SolveUnsolvedCommand
    {
        public static void Register(CommandLineApplication app)
        {
            app.Command(
                "solve-unsolved",
                (command) =>
                {
                    command.Description = "Create solutions for all nonexistent problem-solver pairs";
                    command.HelpOption("-?|-h|--help");

                    var threadsOption = command.Option(
                        "-t|--threads",
                        "Number of worker threads",
                        CommandOptionType.SingleValue);

                    command.OnExecute(
                        () =>
                        {
                            var threadsCount = threadsOption.HasValue() ? int.Parse(threadsOption.Value()) : Environment.ProcessorCount;
                            var threads = Enumerable.Range(0, threadsCount).ToList();
                            Parallel.ForEach(
                                threads,
                                new ParallelOptions {MaxDegreeOfParallelism = threadsCount},
                                thread =>
                                {
                                    while (true)
                                    {
                                        var solvers = RunnableSolvers
                                            .Enumerate()
                                            .OrderBy(_ => Guid.NewGuid())
                                            .Select(x => x.Invoke())
                                            .ToList();

                                        var problems = ProblemReader.ReadAll();

                                        solvers.ForEach(
                                            solver =>
                                            {
                                                var solved = Storage.EnumerateSolved(solver).Select(x => x.ProblemId);
                                                var unsolved = problems
                                                    .Select(x => x.ProblemId)
                                                    .Except(solved)
                                                    .OrderBy(_ => Guid.NewGuid())
                                                    .ToList()
                                                    .First();

                                                Common.Solve(solver, problems.Find(x => x.ProblemId == unsolved), thread);
                                            });
                                    }
                                });

                            return 0;
                        });
                });
        }
    }
}