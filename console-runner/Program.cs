using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection.Metadata;
using System.Xml;
using lib;
using lib.Models;
using lib.Solvers;
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

            app.Command("check-unchecked", (command) =>
            {
                command.Description = "Check all unchecked solution with official checker";
                command.HelpOption("-?|-h|--help");
                
                var geckodriverOption = command.Option("-g|--geckodriver",
                    "Override geckodriver exec name",
                    CommandOptionType.SingleValue);

                command.OnExecute(() =>
                {
                    var geckodriverExecName = "geckodriver";
                    if (geckodriverOption.HasValue())
                    {
                        geckodriverExecName = geckodriverOption.Value();
                    }
                    Storage
                        .EnumerateUnchecked()
                        .ForEach(solution => solution.CheckOnline(geckodriverExecName));

                    return 0;
                });
            });

            app.Command("solve", (command) =>
            {
                command.Description = "Solve all problems with all solvers";
                command.HelpOption("-?|-h|--help");
                
                var solverOption = command.Option("-s|--solver",
                    "Solver name prefix",
                    CommandOptionType.SingleValue);
                
                var problemOption = command.Option("-p|--problem",
                    "Problem id",
                    CommandOptionType.SingleValue);

                command.OnExecute(() =>
                {
                    var solvers = RunnableSolvers
                        .Enumerate()
                        .Select(x => x.Invoke())
                        .Where(x => !solverOption.HasValue() || solverOption.HasValue() && x.GetName().StartsWith(solverOption.Value()))
                        .ToList();
                    
                    solvers.ForEach(solver =>
                    {
                        ProblemReader
                            .ReadAll()
                            .Where(x => !problemOption.HasValue() || problemOption.HasValue() && x.ProblemId == int.Parse(problemOption.Value()))
                            .ToList()
                            .ForEach(problemMeta => Solve(solver, problemMeta));
                    });

                    return 0;
                });
            });

            app.Command("solve-unsolved", (command) =>
            {
                command.Description = "Create solutions for any nonexistent problem-solver pair";
                command.HelpOption("-?|-h|--help");
                
                var subsetSizeOption = command.Option("-n",
                    "Random subset size",
                    CommandOptionType.SingleValue);

                command.OnExecute(() =>
                {
                    var solvers = RunnableSolvers
                        .Enumerate()
                        .Select(x => x.Invoke())
                        .ToList();

                    var problems = ProblemReader.ReadAll();
                    
                    solvers.ForEach(solver =>
                    {
                        var solved = Storage.EnumerateSolved(solver).Select(x => x.ProblemId);
                        var unsolved = problems
                            .Select(x => x.ProblemId)
                            .Except(solved)
                            .OrderBy(_ => Guid.NewGuid())
                            .ToList();

                        if (subsetSizeOption.HasValue())
                        {
                            var subset = unsolved
                                .Take(int.Parse(subsetSizeOption.Value()))
                                .ToList();
                            
                            Console.WriteLine($"Unsolved by {solver.GetName()} v{solver.GetVersion()}: {string.Join(", ", subset)}");
                                
                            subset.ForEach(x => Solve(solver, problems.Find(y => x == y.ProblemId)));
                        }
                        else
                        {
                            Console.WriteLine($"Unsolved by {solver.GetName()} v{solver.GetVersion()}: {string.Join(", ", unsolved)}");
                                
                            unsolved.ForEach(x => Solve(solver, problems.Find(y => x == y.ProblemId)));
                        }
                    });

                    return 0;
                });
            });

            app.Command("submit", (command) =>
            {
                command.Description = "Prepare and submit all solutions";
                command.HelpOption("-?|-h|--help");
                
                var zipfileOption = command.Option("-z|--zipfile",
                    "Override zip file name",
                    CommandOptionType.SingleValue);
                
                command.OnExecute(() =>
                {
                    var solutionDirectory = FileHelper.PatchDirectoryName("solutions");
                    var submissionsDirectory = FileHelper.PatchDirectoryName("submissions");

                    if (Directory.Exists(solutionDirectory))
                    {
                        Directory.Delete(solutionDirectory, true);
                    }
                    Directory.CreateDirectory(solutionDirectory);
                    
                    if (!Directory.Exists(submissionsDirectory))
                    {
                        Directory.CreateDirectory(submissionsDirectory);
                    }
                    
                    Storage
                        .EnumerateCheckedAndCorrect()
                        .ForEach(solution =>
                        {
                            var fileName = $"prob-{solution.ProblemId:000}.sol";
                            File.WriteAllText(Path.Combine(solutionDirectory, fileName), solution.SolutionBlob);
                        });

                    var zipfileName = DateTimeOffset.Now.ToUnixTimeSeconds() + ".zip";
                    if (zipfileOption.HasValue())
                    {
                        zipfileName = zipfileOption.Value() + ".zip";
                    }
                    var submissionFile = Path.Combine(submissionsDirectory, zipfileName);
                    ZipFile.CreateFromDirectory(solutionDirectory, submissionFile);

                    return 0;
                });
            });
            
            app.Command("csv", (command) =>
            {
                command.Description = "Generate expected csv that can be compared to organizers csv";
                command.HelpOption("-?|-h|--help");
                
                command.OnExecute(() =>
                {
                    Storage
                        .EnumerateCheckedAndCorrect()
                        .OrderBy(s => s.ProblemId)
                        .ToList()
                        .ForEach(solution =>
                        {
                            Console.WriteLine($"{solution.ProblemId}, {solution.OurTime}, Ok");
                        });
                    
                    return 0;
                });
            });
            
            app.Execute(args);
        }

        private static void Solve(ISolver solver, ProblemMeta problemMeta)
        {
            Console.Write(
                $"Solving {problemMeta.ProblemId} " +
                $"with {solver.GetName()} v{solver.GetVersion()}... ");
    
            var solutionMeta = RunnableSolvers.Solve(solver, problemMeta);
            solutionMeta.SaveToDb();
                        
            Console.WriteLine($"Done in {solutionMeta.CalculationTookMs} ms, " +
                              $"{solutionMeta.OurTime} time units");
        }
    }
}