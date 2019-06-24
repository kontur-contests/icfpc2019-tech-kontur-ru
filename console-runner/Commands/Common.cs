using System;
using lib.Models;
using lib.Solvers;
using pipeline;

namespace console_runner.Commands
{
    public static class Common
    {
        public const double DefaultMinDelta = 1.2;
        
        public static void Solve(ISolver solver, ProblemMeta problemMeta, int? thread = null)
        {
            var prefix = thread.HasValue ? $"#{thread.Value}: " : string.Empty;

            Console.WriteLine($"{prefix}Solving {problemMeta.ProblemId} with {solver.GetName()} v{solver.GetVersion()}... ");
    
            new SolutionInProgress(problemMeta.ProblemId, solver.GetName(), solver.GetVersion()).SaveToDb();
            try
            {
                var solutionMeta = RunnableSolvers.Solve(solver, problemMeta);
                solutionMeta.SaveToDb();
                Console.WriteLine($"{prefix}Done in {solutionMeta.CalculationTookMs} ms, {solutionMeta.OurTime} time units");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}