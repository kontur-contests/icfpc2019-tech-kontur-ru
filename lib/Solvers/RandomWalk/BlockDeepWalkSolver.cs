using System;
using System.Collections.Generic;
using System.Linq;
using lib.Models;

namespace lib.Solvers.RandomWalk
{
    public class BlockDeepWalkSolver : ISolver
    {
        public string GetName()
        {
            return $"block-deep-walk-{usePalka}-{depth}-{blockSize}";
        }

        public int GetVersion()
        {
            return 1;
        }

        private readonly int blockSize;
        private readonly int depth;
        private readonly IEstimator estimator;
        private readonly bool usePalka;

        private readonly ActionBase[] availableActions =
        {
            new Rotate(true),
            new Rotate(false),
            new Move("0,1"),
            new Move("0,-1"),
            new Move("1,0"),
            new Move("-1,0")
        };
        private readonly List<List<ActionBase>> chains;

        public BlockDeepWalkSolver(int blockSize, int depth, IEstimator estimator, bool usePalka)
        {
            this.blockSize = blockSize;
            this.depth = depth;
            this.estimator = estimator;
            this.usePalka = usePalka;

            chains = availableActions.Select(x => new List<ActionBase> {x}).ToList();
            for (int i = 1; i < depth; i++)
            {
                chains = chains.SelectMany(c => availableActions.Select(a => c.Concat(new[] {a}).ToList())).ToList();
            }
        }

        public State GetBlock(State state)
        {
            var queue = new Queue<V>();
            var used = new HashSet<V>();
            queue.Enqueue(state.Worker.Position);
            used.Add(state.Worker.Position);

            V start = null;
            while (queue.Any() && start == null)
            {
                var cur = queue.Dequeue();

                for (var direction = 0; direction < 4; direction++)
                {
                    var next = cur.Shift(direction);
                    if (!next.Inside(state.Map) || state.Map[next] == CellState.Obstacle)
                        continue;

                    if (!used.Add(next))
                        continue;

                    if (state.Map[next] == CellState.Void)
                    {
                        start = next;
                        break;
                    }

                    queue.Enqueue(next);
                }
            }

            if (start == null)
                throw new InvalidOperationException();

            queue.Clear();
            queue.Enqueue(start);

            var size = 1;

            int minx = int.MaxValue, miny = int.MaxValue, maxx = int.MinValue, maxy = int.MinValue;
            while (queue.Any())
            {
                var cur = queue.Dequeue();

                for (var direction = 0; direction < 4; direction++)
                {
                    var next = cur.Shift(direction);
                    if (!next.Inside(state.Map) || state.Map[next] != CellState.Void)
                        continue;

                    if (!used.Add(next))
                        continue;

                    if (next.X < minx)
                        minx = next.X;
                    if (next.X > maxx)
                        maxx = next.X;
                    if (next.Y < miny)
                        miny = next.Y;
                    if (next.Y > maxy)
                        maxy = next.Y;

                    queue.Enqueue(next);
                    size++;
                    if (size >= blockSize)
                        break;
                }

                if (size >= blockSize)
                    break;
            }

            while (queue.Any())
            {
                var cur = queue.Dequeue();

                for (var direction = 0; direction < 4; direction++)
                {
                    var next = cur.Shift(direction);
                    if (!next.Inside(state.Map) || state.Map[next] != CellState.Void)
                        continue;

                    if (next.X < minx || next.Y < miny || next.X > maxx || next.Y > maxy)
                        continue;

                    if (!used.Add(next))
                        continue;

                    queue.Enqueue(next);
                }
            }

            var result = state.Clone();
            for (int x = 0; x < result.Map.SizeX; x++)
            for (int y = 0; y < result.Map.SizeY; y++)
            {
                var p = new V(x, y);
                if (result.Map[p] != CellState.Obstacle)
                {
                    if (!used.Contains(p))
                        result.Map[p] = CellState.Wrapped;
                }
            }

            result.UnwrappedLeft = result.Map.VoidCount();

            return result;
        }

        public List<ActionBase> Solve(State state)
        {
            var solution = new List<ActionBase>();

            //Console.Out.WriteLine("--INIT:\n" + state.Map);

            if (usePalka)
                CollectManipulators(state, solution);

            while (state.UnwrappedLeft > 0)
            {
                //Console.Out.WriteLine($"--BEFORE ({solution.Count}):\n{state.Map}");

                var sub = GetBlock(state);
                // Console.Out.WriteLine("--SUB:\n" + sub.Map);
                while (sub.UnwrappedLeft > 0)
                {
                    var part = SolvePart(sub);
                    solution.AddRange(part);
                    sub.Apply(part);
                    state.Apply(part);
                }
            }

            // Console.Out.WriteLine("--AFTER:\n" + state.Map);
            return solution;
        }

        private void CollectManipulators(State state, List<ActionBase> result)
        {
            var k = 0;

            while (true)
            {
                var boosters = state.Boosters.Where(b => b.Type == BoosterType.Extension).ToList();

                if (!boosters.Any())
                    return;

                var map = state.Map;
                var me = state.Worker;
                var pathBuilder = new PathBuilder(map, me.Position, false);

                var best = boosters.OrderBy(b => pathBuilder.Distance(b.Position)).First();

                var actions = pathBuilder.GetActions(best.Position);

                var y = k / 2 + 2;
                y = k % 2 == 0 ? -y : y;
                var add = new UseExtension(new V(1, y));
                k++;

                actions.Add(add);

                state.Apply(actions);
                result.AddRange(actions);
            }
        }

        private class PathBuilder
        {
            private readonly V start;
            private Queue<V> queue;
            private Map<int> distance;
            private Map<V> parent;

            public PathBuilder(Map map, V start, bool stop)
            {
                this.start = start;
                queue = new Queue<V>();
                queue.Enqueue(start);

                distance = new Map<int>(map.SizeX, map.SizeY);
                parent = new Map<V>(map.SizeX, map.SizeY);

                while (queue.Any())
                {
                    var v = queue.Dequeue();
                    if (map[v] == CellState.Void && stop)
                        break;

                    for (var direction = 0; direction < 4; direction++)
                    {
                        var u = v.Shift(direction);
                        if (!u.Inside(map) || parent[u] != null || map[u] == CellState.Obstacle)
                            continue;

                        parent[u] = v;
                        distance[u] = distance[v] + 1;
                        queue.Enqueue(u);
                    }
                }
            }

            public int Distance(V v) => parent[v] == null ? int.MaxValue : distance[v];

            public List<ActionBase> GetActions(V to)
            {
                var result = new List<ActionBase>();

                while (to != start)
                {
                    var from = parent[to];

                    result.Add(new Move(to - from));

                    to = from;
                }

                result.Reverse();
                return result;
            }
        }

        public List<ActionBase> SolvePart(State state)
        {
            // Console.Out.WriteLine("START PART");

            var bestEstimation = double.MinValue;
            List<ActionBase> bestSolution = null;

            foreach (var chain in chains)
            {
                var clone = state.Clone();

                var solution = new List<ActionBase>();
                foreach (var action in chain)
                {
                    if (action is Move moveAction)
                    {
                        var nextPosition = clone.Worker.Position + moveAction.Shift;
                        if (!nextPosition.Inside(clone.Map) || clone.Map[nextPosition] == CellState.Obstacle)
                            continue;
                    }

                    clone.Apply(action);
                    solution.Add(action);
                    if (clone.UnwrappedLeft == 0)
                        break;
                }

                var estimation = estimator.Estimate(clone, state);
                // Console.Out.Write($"  {estimation} {solution.Format()}");
                if (estimation > bestEstimation)
                {
                    bestEstimation = estimation;
                    bestSolution = solution;
                    // Console.Out.WriteLine(" -- better");
                }

                // else
                //     Console.Out.WriteLine();
            }

            return bestSolution;
        }
    }
}