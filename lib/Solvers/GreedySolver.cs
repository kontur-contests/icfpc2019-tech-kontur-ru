using System.Collections.Generic;
using System.Linq;
using lib.Models;
using lib.Models.Actions;
using lib.Solvers.RandomWalk;

namespace lib.Solvers
{
    public class GreedySolver : ISolver
    {
        private readonly IEstimator estimator;

        public GreedySolver(IEstimator estimator)
        {
            this.estimator = estimator;
        }

        public string GetName() => "greedy";

        public int GetVersion() => 1;

        private readonly V[] shifts = {"0,1", "1,0", "0,-1", "-1,0"};

        public Solved Solve(State state)
        {
            var result = new List<ActionBase>();

            while (state.UnwrappedLeft > 0)
            {
                var actions = new List<ActionBase>();
                actions.Add(new Rotate(true));
                actions.Add(new Rotate(false));
                actions.AddRange(shifts.Select(s => new Move(s)).Where(m => m.IsValid(state, state.SingleWorker)));
                var best = actions.Select(a => EstimateAction(a, state)).OrderByDescending(a => a.score).FirstOrDefault();
                //Console.WriteLine(best);
                result.Add(best.action);
                state.Apply(best.action);
            }

            return new Solved {Actions = new List<List<ActionBase>> {result}};
        }

        private (ActionBase action, double score) EstimateAction(ActionBase action, State state)
        {
            var undo = state.Apply(action);
            var score = estimator.Estimate(state, state.SingleWorker);
            undo();
            return (action, score);
        }
    }
}