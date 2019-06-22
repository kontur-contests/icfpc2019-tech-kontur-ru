using System.Collections.Generic;
using System.Linq;
using lib.Models;

namespace lib.Puzzles
{
    public class MstPuzzleSolver : BasePuzzleSolver, IPuzzleSolver
    {
        public Problem Solve(Puzzle puzzle)
        {
            var map = MarkCells(puzzle);

            var completed = Complete(map, puzzle);

            return completed;
        }

        private Map<PuzzleCell> MarkCells(Puzzle puzzle)
        {
            var map = new Map<PuzzleCell>(puzzle.TaskSize, puzzle.TaskSize);
            for (int x = 0; x < map.SizeX; x++)
            for (int y = 0; y < map.SizeY; y++)
                map[new V(x, y)] = PuzzleCell.Unknown;

            var not = puzzle.MustNotContainPoints.ToList();
            not.Insert(0, V.Zero);

            foreach (var n in not)
                map[n] = PuzzleCell.Outside;

            MarkCells(map, puzzle.MustContainPoints, PuzzleCell.Inside);

            foreach (var n in not)
                map[n] = PuzzleCell.Unknown;

            MarkCells(map, not, PuzzleCell.Outside);

            return map;
        }

        private void MarkCells(Map<PuzzleCell> map, List<V> points, PuzzleCell type)
        {
            var toAdd = points.ToList();

            if (toAdd.Any())
            {
                map[toAdd[0]] = type;
                toAdd.RemoveAt(0);
            }
            
            while (toAdd.Any())
            {
                var pathBuilder = new PathBuilder(map, type);

                var best = 0;
                for (int i = 1; i < toAdd.Count; i++)
                    if (pathBuilder.Distance(toAdd[i]) < pathBuilder.Distance(toAdd[best]))
                        best = i;

                var path = pathBuilder.GetPath(toAdd[best]);
                foreach (var x in path)
                    map[x] = type;

                toAdd.RemoveAt(best);
            }
        }

        private class PathBuilder
        {
            private readonly Map<PuzzleCell> map;
            private readonly Map<int> distance;
            private readonly Map<V> parent;
            
            public PathBuilder(Map<PuzzleCell> map, PuzzleCell type)
            {
                this.map = map;
                var queue = new LinkedList<V>();
                for (int x = 0; x < map.SizeX; x++)
                for (int y = 0; y < map.SizeY; y++)
                {
                    if (map[new V(x, y)] == type)
                        queue.AddLast(new V(x, y));
                }

                distance = new Map<int>(map.SizeX, map.SizeY);
                parent = new Map<V>(map.SizeX, map.SizeY);
                
                while (queue.Any())
                {
                    var v = queue.First();
                    queue.RemoveFirst();
                    
                    for (var direction = 0; direction < 4; direction++)
                    {
                        var u = v.Shift(direction);
                        if (!u.Inside(map) || parent[u] != null || map[u] != PuzzleCell.Unknown)
                            continue;

                        parent[u] = v;
                        distance[u] = distance[v] + 1;
                        queue.AddLast(u);
                    }
                }
            }

            public int Distance(V v) => parent[v] == null ? int.MaxValue : distance[v];

            public List<V> GetPath(V to)
            {
                var result = new List<V>();

                while (map[to] == PuzzleCell.Unknown)
                {
                    result.Add(to);
                    to = parent[to];
                }

                result.Reverse();
                return result;
            }
        }
    }
}