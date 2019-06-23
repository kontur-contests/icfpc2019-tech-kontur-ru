using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using lib;
using lib.Models;
using lib.Models.Actions;
using lib.Solvers;

namespace tests.Solvers
{
    internal class SolverTestsBase
    {
        public Solved SolveOneProblem(ISolver solver, int id)
        {
            var state = ReadFromFile(id);
            var result = solver.Solve(state);
            Save(result, id);
            Console.WriteLine($"Solved {id} problem in {result.CalculateTime()} steps.");
            // LogSolution(id, result);
            return result;
        }
        
        public int SolvePuzzleProblem(ISolver solver, int blockId)
        {
            var state = ReadFromPuzzleFile(blockId);
            var result = solver.Solve(state);
            SavePuzzle(result, blockId);
            Console.WriteLine($"Solved puzzle {blockId} problem in {result.CalculateTime()} steps.");
            // LogSolution(id, result);
            return result.CalculateTime();
        }

        private void LogSolution(int id, List<List<ActionBase>> result)
        {
            var state1 = ReadFromFile(id);
            foreach (var action in result)
            {
                foreach (var item in action)
                {
                    state1.Apply(item);
                    Console.WriteLine(state1.Time + ": " + item + " -> " + string.Join(" | ", state1.Workers));
                }
            }
        }

        public int SolveOneProblemWithCluster(ISolver solver, int id)
        {
            var state = ReadFromFile(id);
            state.ClustersState = new ClustersState(ClustersStateReader.Read(id), state);
            
            var pathFileName = Path.Combine(FileHelper.PatchDirectoryName("clusters.v2"), $"prob-{id:000}.path");
            if (File.Exists(pathFileName))
                state.ClustersState.Path = File.ReadAllLines(pathFileName).Select(int.Parse).ToList();

            var result = solver.Solve(state);
            Save(result, id);
            Console.WriteLine($"Solved {id} problem in {result.CalculateTime()} steps.");
            return result.CalculateTime();
        }

        public void SolveSomeProblems(Func<ISolver> solverProvider, List<int> ids)
        {
            var total = 0;
            var sync = new object();

            Parallel.For(
                0,
                ids.Count,
                i =>
                {
                    var id = ids[i];
                    var result = SolveOneProblem(solverProvider(), id);

                    lock (sync)
                    {
                        total += result.CalculateTime();
                    }
                });
            Console.WriteLine($"Total steps: {total}.");
        }

        public State ReadFromFile(int id)
        {
            var problem = ProblemReader.Read(id);
            return problem.ToState();
        }

        public State ReadFromPuzzleFile(int id)
        {
            var problem = ProblemReader.ReadPuzzleTask(id);
            return problem.ToState();
        }

        public void Save(Solved solved, int id, string suffix = null)
        {
            var text = solved.FormatSolution();
            var fileName = Path.Combine(FileHelper.PatchDirectoryName("problems"), "all", $"prob-{id:000}{suffix}.sol");
            var buyFileName = Path.Combine(FileHelper.PatchDirectoryName("problems"), "all", $"prob-{id:000}{suffix}.buy");
            File.WriteAllText(fileName, text);
            File.WriteAllText(buyFileName, solved.FormatBuy());
        }
        
        public void SavePuzzle(Solved solved, int blockId)
        {
            var text = solved.FormatSolution();
            var fileName = Path.Combine(FileHelper.PatchDirectoryName("problems"), "puzzles", $"block{blockId:000}.sol");
            File.WriteAllText(fileName, text);
            Save(solved, 500 + blockId);
        }
    }
}