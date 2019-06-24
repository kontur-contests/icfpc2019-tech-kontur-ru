using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using lib;
using lib.API;
using lib.Models;
using lib.Models.Actions;
using lib.Puzzles;
using lib.Solvers;
using Microsoft.Extensions.CommandLineUtils;
using pipeline;

namespace console_runner.Commands
{
    public static class SolveBlockCommand
    {
        public static void Register(CommandLineApplication app)
        {
            app.Command(
                "solve-block",
                (command) =>
                {
                    command.Description = "Create solution for the current puzzle";
                    command.HelpOption("-?|-h|--help");

                    var submitOption = command.Option(
                        "-s|--submit",
                        "Immediately submit block",
                        CommandOptionType.NoValue);

                    var blockNumberOption = command.Option(
                        "-b|--block",
                        "Calculate specific block",
                        CommandOptionType.SingleValue);

                    command.OnExecute(
                        async () => await SolveBlocks(blockNumberOption, submitOption));
                });
        }

        private static async Task<int> SolveBlocks(CommandOption blockNumberOption, CommandOption submitOption)
        {
            while (true)
            {
                await SolveBlock(blockNumberOption, submitOption);
                Console.WriteLine();
            }
        }

        private static async Task<int> SolveBlock(CommandOption blockNumberOption, CommandOption submitOption)
        {
            BlockchainBlock block;
            if (blockNumberOption.HasValue())
                block = await Api.GetBlockchainBlock(int.Parse(blockNumberOption.Value()));
            else
                block = await Api.GetBlockchainBlock();
            Console.WriteLine($"Solving block #{block.BlockNumber} ... {DateTime.Now}");

            var blockProblemPath = Path.Combine(FileHelper.PatchDirectoryName("problems"), "puzzles", $"block{block.BlockNumber:000}_orig.desc");

            if (submitOption.HasValue() && File.Exists(blockProblemPath))
            {
                Console.WriteLine("Already solved.");
                Thread.Sleep(TimeSpan.FromSeconds(10));
                return 0;
            }

            File.WriteAllText(blockProblemPath, block.Problem.ToString());

            var puzzlePath = Path.Combine(FileHelper.PatchDirectoryName("problems"), "puzzles", $"block{block.BlockNumber:000}.cond");
            File.WriteAllText(puzzlePath, block.Puzzle.ToString());

            Console.WriteLine($"Solving puzzle ...");

            var puzzleSolvers = new List<IPuzzleSolver>
            {
                new MstPuzzleSolver(),
            };

            var puzzleSolved = false;
            var ourProblemPath = Path.Combine(FileHelper.PatchDirectoryName("problems"), "puzzles", $"block{block.BlockNumber:000}.desc");
            foreach (var puzzleSolver in puzzleSolvers)
            {
                var ourProblem = puzzleSolver.Solve(block.Puzzle);
                if (!ourProblem.IsValidForPuzzle(block.Puzzle))
                    continue;

                puzzleSolved = true;
                File.WriteAllText(ourProblemPath, ourProblem.ToString());
            }

            if (!puzzleSolved)
                throw new Exception("Puzzle not solved.");

            var solvers = RunnableSolvers
                .PuzzleSolvers()
                .Select(x => x.Invoke())
                //.Take(1)
                .ToList();

            var mapSize = block.Problem.ToState().Map;
            Console.WriteLine($"Solving problem {mapSize.SizeX}x{mapSize.SizeY} with {solvers.Count} solvers ...");

            var results = new List<Tuple<ISolver, List<List<ActionBase>>>>();
            var stopwatch = Stopwatch.StartNew();

            foreach (var solver in solvers)
            {
                if (stopwatch.Elapsed > TimeSpan.FromMinutes(10))
                    break;

                var solved = solver.Solve(block.Problem.ToState().Clone());
                var calculationTime = stopwatch.ElapsedMilliseconds;

                var actions = solved.Actions;
                var time = solved.CalculateTime();
                var solutionBlob = solved.FormatSolution();

                var path = Path.Combine(FileHelper.PatchDirectoryName("problems"), "puzzles", $"block{block.BlockNumber:000}_sol_{solver.GetName()}_v{solver.GetVersion()}_{time}.sol");
                File.WriteAllText(path, solutionBlob);

                Console.WriteLine($"{solver.GetName()}_v{solver.GetVersion()} score = {time} time = {stopwatch.Elapsed}");

                new SolutionMeta(
                    block.BlockNumber,
                    solutionBlob,
                    time,
                    solver.GetName(),
                    solver.GetVersion(),
                    calculationTime,
                    null,
                    0
                ).SaveToDb(isBlockSolution: true);

                results.Add(Tuple.Create(solver, actions));
            }

            var (bestSolver, bestActions) = results
                .OrderBy(x => x.Item2.CalculateTime())
                .First();

            var solutionPath = Path.Combine(FileHelper.PatchDirectoryName("problems"), "puzzles", $"block{block.BlockNumber:000}_best_{bestSolver.GetName()}_v{bestSolver.GetVersion()}_{bestActions.CalculateTime()}.sol");
            File.WriteAllText(solutionPath, bestActions.Format());

            Console.WriteLine($"Best score = {bestActions.CalculateTime()}");

            if (submitOption.HasValue())
            {
                Console.WriteLine("Submitting block ...");
                var submissionResult = await Api.Submit(block.BlockNumber, solutionPath, ourProblemPath);
                if (submissionResult.Errors != null && submissionResult.Errors.Count > 0)
                    submissionResult.Errors.ToList()
                        .ForEach(
                            e => { Console.WriteLine($"Error {e.Key}: {e.Value}"); });
            }

            Console.WriteLine("Done.");
            return 0;
        }
    }
}
