using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using lib;
using lib.Models;
using lib.Solvers;
using NUnit.Framework;

namespace tests.Solvers
{
    internal class SolverTestsBase
    {
        public int SolveOneProblem(ISolver solver, int id)
        {
            var state = ReadFromFile(id);
            var result = solver.Solve(state);
            Save(result, id);
            Console.WriteLine($"Solved {id} problem in {result.Count} steps.");
            return result.Count;
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

        public void Save(List<ActionBase> actions, int id)
        {
            var text = actions.Format();
            var fileName = Path.Combine(FileHelper.PatchDirectoryName("problems"), "all", $"prob-{id:000}.sol");
            File.WriteAllText(fileName, text);
        }
    }
}