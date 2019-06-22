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
        public int SolveOneProblem(ISolver solver, int id)
        {
            var state = ReadFromFile(id);
            var result = solver.Solve(state);
            Save(result, id);
            Console.WriteLine($"Solved {id} problem in {result.CalculateTime()} steps.");
            return result.CalculateTime();
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
                        total += result;
                    }
                });
            Console.WriteLine($"Total steps: {total}.");
        }

        public State ReadFromFile(int id)
        {
            var problem = ProblemReader.Read(id);
            return problem.ToState();
        }

        public void Save(List<List<ActionBase>> actions, int id)
        {
            var text = actions.Format();
            var fileName = Path.Combine(FileHelper.PatchDirectoryName("problems"), "all", $"prob-{id:000}.sol");
            File.WriteAllText(fileName, text);
        }
    }
}