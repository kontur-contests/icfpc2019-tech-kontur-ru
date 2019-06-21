using System;
using System.Collections.Generic;
using System.IO;
using lib;
using lib.Models;
using lib.Solvers;
using NUnit.Framework;

namespace tests.Solvers
{
    [TestFixture]
    internal class StupidSolverTests
    {
        [Test]
        public void Test()
        {
            int id = 50;

            var state = ReadFromFile(id);
            var solver = new StupidSolver(state);
            var result = solver.Solve();

            Save(result, id);
        }
        
        public State ReadFromFile(int id)
        {
            var reader = new ProblemReader(ProblemReader.PART_1_INITIAL);
            var problem = reader.Read(id);
            return problem.ToState();
        }

        public void Save(List<ActionBase> actions, int id)
        {
            Console.WriteLine($"Solved in {actions.Count} steps.");

            var text = actions.Format();
            var fileName = Path.Combine(FileHelper.PatchDirectoryName("problems"), ProblemReader.PART_1_INITIAL, $"prob-{id:000}.sol");
            File.WriteAllText(fileName, text);
        }
    }
}