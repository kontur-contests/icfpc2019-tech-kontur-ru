using System;
using System.IO;
using System.IO.Compression;
using lib;
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
                                .EnumerateCheckedAndCorrect()
                                .ForEach(
                                    solution =>
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
        }
    }
}