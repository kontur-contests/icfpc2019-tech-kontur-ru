using System;
using System.IO;
using System.IO.Compression;
using lib;
using lib.API;
using lib.Models;
using Microsoft.Extensions.CommandLineUtils;
using pipeline;

namespace console_runner.Commands
{
    public static class SubmitCommand
    {
        public static void Register(CommandLineApplication app)
        {
            app.Command(
                "submit",
                (command) =>
                {
                    command.Description = "Prepare and submit all solutions";
                    command.HelpOption("-?|-h|--help");

                    var zipfileOption = command.Option(
                        "-z|--zipfile",
                        "Override zip file name",
                        CommandOptionType.SingleValue);
                    
                    var minDeltaOption = command.Option(
                        "-d|--min-delta",
                        $"Override minimum delta (default {Common.defaultMinDelta})",
                        CommandOptionType.SingleValue);

                    command.OnExecute(
                        () =>
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
                                .EnumerateBestSolutions(Api.GetBalance().GetAwaiter().GetResult(), minDeltaOption.HasValue() ? int.Parse(minDeltaOption.Value()) : Common.defaultMinDelta)
                                .ForEach(
                                    solution =>
                                    {
                                        var solFileName = $"prob-{solution.ProblemId:000}.sol";
                                        File.WriteAllText(Path.Combine(solutionDirectory, solFileName), solution.SolutionBlob);
                                        
                                        if (!string.IsNullOrEmpty(solution.BuyBlob))
                                        {
                                            var buyFileName = $"prob-{solution.ProblemId:000}.buy";
                                            File.WriteAllText(Path.Combine(solutionDirectory, buyFileName), solution.BuyBlob);
                                        }

                                        new SubmissionSummary(solution.ProblemId, solution.MoneySpent, solution.OurTime).SaveToDb();
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
        }
    }
}