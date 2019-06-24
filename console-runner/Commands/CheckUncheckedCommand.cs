using Microsoft.Extensions.CommandLineUtils;
using pipeline;

namespace console_runner.Commands
{
    public static class CheckUncheckedCommand
    {
        public static void Register(CommandLineApplication app)
        {
            app.Command(
                "check-unchecked",
                (command) =>
                {
                    command.Description = "Check all unchecked solutions with official checker";
                    command.HelpOption("-?|-h|--help");

                    var geckodriverOption = command.Option(
                        "-g|--geckodriver",
                        "Override geckodriver exec name",
                        CommandOptionType.SingleValue);

                    command.OnExecute(
                        () =>
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
        }
    }
}