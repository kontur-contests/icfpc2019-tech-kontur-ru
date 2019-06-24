using System;
using console_runner.Commands;
using Microsoft.Extensions.CommandLineUtils;

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

            CheckUncheckedCommand.Register(app);
            SolveCommand.Register(app);
            SolveUnsolvedCommand.Register(app);
            SolveBlockCommand.Register(app);
            SubmitCommand.Register(app);
            CsvCommand.Register(app);
            
            app.Execute(args);
        }

        
    }
}