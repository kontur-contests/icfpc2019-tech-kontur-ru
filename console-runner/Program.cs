﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
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
                
                var problemsOption = command.Option("-p|--problems",
                    "Single problem id or problem ids range",
                    CommandOptionType.SingleValue);

                command.OnExecute(() =>
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
                            var parts = problemsOption.Value().Split("..", StringSplitOptions.RemoveEmptyEntries);
                            var pStart = int.Parse(parts[0]);
                            var pEnd = int.Parse(parts[1]);
                            problemIds.AddRange(Enumerable.Range(pStart, pEnd - pStart + 1));
                            Console.WriteLine($"Will solve problems: {string.Join(", ", problemIds)}");
                        }
                    }

                    solvers.ForEach(solver =>
                    {
                        ProblemReader
                            .ReadAll()
                            .Where(x => !problemIds.Any() || problemIds.Contains(x.ProblemId))
                            .ToList()
                            .ForEach(problemMeta => Solve(solver, problemMeta));
                    });

                    return 0;
                });
            });

            app.Command("list-unsolved", (command) =>
            {
                command.Description = "List all tasks to be solved by current runnable solvers";
                command.HelpOption("-?|-h|--help");

                command.OnExecute(() =>
                {
                    var solvers = RunnableSolvers.Enumerate()
                        .Select(s => s.Invoke())
                        .OrderBy(s => s.GetName());
                    var problems = ProblemReader.ReadAll();
                    
                    foreach (var solver in solvers)
                    {
                        Console.Write($"{solver.GetName()}: ");
                        
                        var solved = Storage.EnumerateSolved(solver).Select(x => x.ProblemId);
                        Console.WriteLine(string.Join(", ", problems
                            .Select(x => x.ProblemId)
                            .Except(solved)));
                    }
                    
                    return 0;
                });
            });
            
            app.Command("solve-unsolved", (command) =>
            {
                command.Description = "Create solutions for all nonexistent problem-solver pairs";
                command.HelpOption("-?|-h|--help");

                var threadsOption = command.Option("-t|--threads",
                    "Number of worker threads",
                    CommandOptionType.SingleValue);

                command.OnExecute(() =>
                {
                    var threadsCount = threadsOption.HasValue() ? int.Parse(threadsOption.Value()) : Environment.ProcessorCount;
                    var threads = Enumerable.Range(0, threadsCount).ToList();
                    Parallel.ForEach(threads, new ParallelOptions {MaxDegreeOfParallelism = threadsCount}, thread =>
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

                                        Solve(solver, problems.Find(x => x.ProblemId == unsolved), thread);
                                    });
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

        private static void Solve(ISolver solver, ProblemMeta problemMeta, int? thread = null)
        {
            var prefix = thread.HasValue ? $"#{thread.Value}: " : string.Empty;

            Console.WriteLine($"{prefix}Solving {problemMeta.ProblemId} with {solver.GetName()} v{solver.GetVersion()}... ");
    
            var solutionMeta = RunnableSolvers.Solve(solver, problemMeta);
            solutionMeta.SaveToDb();
                        
            Console.WriteLine($"{prefix}Done in {solutionMeta.CalculationTookMs} ms, {solutionMeta.OurTime} time units");
        }
    }
}