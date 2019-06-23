using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using System.Xml;
using console_runner.Commands;
using lib;
using lib.API;
using lib.Models;
using lib.Puzzles;
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