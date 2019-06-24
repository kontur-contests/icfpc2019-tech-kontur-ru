using System;
using System.Collections.Generic;
using System.Linq;
using lib.Models;
using lib.Models.Actions;

namespace lib.Solvers.Postprocess
{
    public class PostprocessorSegments
    {
        private readonly State state;
        private readonly int startIndex;

        public PostprocessorSegments(State state, Solved solved)
        {
            this.state = state;
            for (int i = 0; i < solved.Actions[0].Count; i++)
            {
                if (solved.Actions[0][i] is UseExtension)
                    startIndex = i + 2;
            }
        }

        public void TransferSmall()
        {
            var ticks = state.History.Ticks;

            var longSegments = new List<(int start, int end)>();
            for (int i = startIndex + 1; i < ticks.Count; i++)
            {
                if (!ticks[i - 1].Wrapped || ticks[i].Wrapped)
                    continue;
                int free = 0;
                int index = i;
                while (index < ticks.Count && !ticks[index].Wrapped)
                {
                    free++;
                    index++;
                }

                if (free > 5)
                    longSegments.Add((i, index - 1));
            }
            longSegments.Add((ticks.Count, ticks.Count));

            var filledSegments = new List<List<TickWorkerState>>();
            var l = 0;
            for (int i = 0; i < longSegments.Count; i++)
            {
                var r = longSegments[i].start - 1;
                filledSegments.Add(ticks.Skip(l).Take(r - l + 1).ToList());
                l = longSegments[i].end + 1;
            }

            var result = filledSegments[0].ToList();
            filledSegments.RemoveAt(0);

            while (filledSegments.Any())
            {
                var besti = -1;
                var bestj = -1;
                var bestScore = int.MaxValue;

                for (int i = 0; i < filledSegments.Count; i++)
                {
                    for (int j = startIndex + 1; j < result.Count; j++)
                    {

                        var cur = (result[j - 1].Position - filledSegments[i].First().Position).MLen()
                                  + (filledSegments[i].Last().Position - result[j].Position).MLen();
                        if (bestScore > cur)
                        {
                            bestScore = cur;
                            besti = i;
                            bestj = j;
                        }
                    }
                }

                var best = result.ToList();

                best.InsertRange(bestj, filledSegments[besti]);
                FixStep(best, bestj + filledSegments[besti].Count - 1);
                FixStep(best, bestj - 1);

                filledSegments.RemoveAt(besti);
                result = best;
            }
            
            state.History = new History
            {
                Ticks = result
            };
        }
        
        public List<TickWorkerState> Transfer(List<TickWorkerState> tickWorkerStates, int segmentStart, int segmentEnd, int targetStart)
        {
            var newTicks = tickWorkerStates.Take(segmentStart).ToList();
            while (newTicks.Count > startIndex - 1 && !newTicks.Last().Wrapped)
                newTicks.RemoveAt(newTicks.Count - 1);

            var segment = tickWorkerStates.Skip(segmentStart).Take(segmentEnd - segmentStart + 1).ToList();

            segmentEnd++;
            while (segmentEnd < tickWorkerStates.Count && !tickWorkerStates[segmentEnd].Wrapped)
                segmentEnd++;

            var wasTicks = newTicks.Count;
            newTicks.AddRange(tickWorkerStates.Skip(segmentEnd));
            var beforeFix = newTicks.Count;
            FixStep(newTicks, wasTicks - 1);
            var afterFix = newTicks.Count;

            if (targetStart > wasTicks)
                targetStart += (afterFix - beforeFix - segmentEnd + wasTicks);

            newTicks.InsertRange(targetStart, segment);
            FixStep(newTicks, targetStart + segment.Count - 1);
            FixStep(newTicks, targetStart - 1);

            return newTicks;
        }

        private void FixStep(List<TickWorkerState> tiks, int position)
        {
            if (position == tiks.Count - 1)
                return;
            // find path from tiks[position] -> tiks[position+1]
            // rotate to valid direction

            var path = PathBuilder.Build(state.Map, tiks[position].Position, tiks[position + 1].Position);

            var segment = new List<TickWorkerState>();
            foreach (var p in path)
            {
                segment.Add(new TickWorkerState()
                {
                    Position = p,
                    Direction = tiks[position].Direction
                });
            }
            
            var d1 = tiks[position].Direction;
            var d2 = tiks[position + 1].Direction;
            if (d1 == d2)
            {
                
            } else if (d1.Rotate(1) == d2 || d1.Rotate(-1) == d2)
            {
                segment.Add(new TickWorkerState()
                {
                    Position = tiks[position + 1].Position,
                    Direction = (segment.LastOrDefault() ?? tiks[position]).Direction
                });
            }
            else
            {
                segment.Add(new TickWorkerState()
                {
                    Position = tiks[position + 1].Position,
                    Direction = (segment.LastOrDefault() ?? tiks[position]).Direction
                });
                segment.Add(new TickWorkerState()
                {
                    Position = tiks[position + 1].Position,
                    Direction = (segment.LastOrDefault() ?? tiks[position]).Direction.Rotate(1)
                });
            }
            
            tiks.InsertRange(position + 1, segment);
        }
        
        private static class PathBuilder
        {
            public static List<V> Build(Map map, V start, V end)
            {
                var queue = new Queue<V>();
                queue.Enqueue(start);

                var parent = new Map<V>(map.SizeX, map.SizeY);
                parent[start] = start;
                
                while (queue.Any() && parent[end] == null)
                {
                    var v = queue.Dequeue();
                    
                    for (var direction = 0; direction < 4; direction++)
                    {
                        var u = v.Shift(direction);
                        if (!u.Inside(map) || parent[u] != null || map[u] == CellState.Obstacle)
                            continue;

                        parent[u] = v;
                        queue.Enqueue(u);
                    }
                }
                
                var result = new List<V>();

                end = parent[end];
                while (end != start)
                {
                    result.Add(end);
                    end = parent[end];
                }

                result.Reverse();
                return result;
            }
        }
    }

}