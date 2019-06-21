using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection.Metadata;
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
            
            app.Command("add-dummy-meta", (command) =>
            {
                command.Description = "Add an example non-checked solution to DB";
                command.HelpOption("-?|-h|--help");

                command.OnExecute(() =>
                {
                    const string problemPack = ProblemReader.EXAMPLES_PACK;
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
                command.Description = "Solve all problems with a stupid solver";
                command.HelpOption("-?|-h|--help");

                command.OnExecute(() =>
                {
                    ProblemReader.Current.ReadAll().ForEach(problemMeta =>
                    {
                        var solver = new StupidSolver();
                        var stopwatch = Stopwatch.StartNew();
                        
                        Console.Write($"Solving {problemMeta.ProblemPack}/{problemMeta.ProblemId} " +
                                      $"with {solver.GetName()} v{solver.GetVersion()}... ");
                        
                        var actions = solver.Solve(problemMeta.Problem.ToState());
                        var solutionBlob = actions.Format();

                        stopwatch.Stop();
                        var calculationTime = stopwatch.ElapsedMilliseconds;
                        
                        var solutionMeta = new SolutionMeta(
                            problemMeta.ProblemPack,
                            problemMeta.ProblemId,
                            solutionBlob,
                            actions.CalculateTime(),
                            solver.GetName(),
                            solver.GetVersion(),
                            calculationTime
                        );
                        
                        solutionMeta.SaveToDb();
                        
                        Console.WriteLine($"Done in {calculationTime} ms");
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
            
            app.Execute(args);
        }
    }
}