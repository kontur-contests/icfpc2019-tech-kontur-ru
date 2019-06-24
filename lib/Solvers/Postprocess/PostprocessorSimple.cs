using System;
using System.Collections.Generic;
using System.Linq;
using lib.Models;
using lib.Models.Actions;

namespace lib.Solvers.Postprocess
{
    public class PostprocessorSimple
    {
        private readonly State state;
        private readonly int[] startIndexes;

        public PostprocessorSimple(State state, Solved solved)
        {
            this.state = state;
            startIndexes = Enumerable.Range(0, solved.Actions.Count).Select(x => 0).ToArray();
            for (int i = 0; i < solved.Actions[0].Count; i++)
            {
                if (solved.Actions[0][i] is UseExtension || solved.Actions[0][i] is UseCloning)
                    startIndexes[0] = i + 2;
            }
        }

        public void TransferSmall()
        {
            for (int w = 0; w < state.History.Workers.Count; w++)
            {
                var ticks = state.History.Workers[w].Ticks;
                while (ticks.Count > 0 && !ticks[ticks.Count - 1].Wrapped)
                {
                    ticks.RemoveAt(ticks.Count - 1);
                }
            }

            var moved = true;
            while (moved)
            {
                moved = false;

                for (int w = 0; w < state.History.Workers.Count; w++)
                {
                    var ticks = state.History.Workers[w].Ticks;

                    var longSegments = new List<(int start, int end)>();
                    for (int i = startIndexes[w] + 1; i < ticks.Count; i++)
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

                        if (free > 10)
                            longSegments.Add((i, index - 1));
                    }

                    for (int i = 0; i < longSegments.Count; i++)
                    {
                        var filledStart = longSegments[i].end + 1;
                        var filledEnd = i == longSegments.Count - 1 ? ticks.Count - 1 : longSegments[i + 1].start - 1;

                        //move [filledStart, filledEnd]

                        var best = -1;
                        var bestww = -1;
                        var bestLen = int.MaxValue;

                        for (int ww = 0; ww < state.History.Workers.Count; ww++)
                        {
                            // if (ww != w)
                            //     continue;

                            var wwticks = state.History.Workers[ww].Ticks;

                            for (int j = startIndexes[ww] + 1; j < wwticks.Count && !moved; j++)
                            {
                                if (!wwticks[j].Wrapped)
                                    continue;
                                if (ww == w && filledStart <= j && j <= filledEnd)
                                    continue;

                                var mLen = Math.Min(
                                    (wwticks[j - 1].Position - ticks[filledStart].Position).MLen(),
                                    (wwticks[j].Position - ticks[filledEnd].Position).MLen());

                                if (mLen < bestLen)
                                {
                                    best = j;
                                    bestww = ww;
                                    bestLen = mLen;
                                }
                            }
                        }

                        if (bestLen <= 5)
                            moved = Transfer(w, filledStart, filledEnd, bestww, best);
                    }
                }
            }
        }

        public bool Transfer(int worker, int segmentStart, int segmentEnd, int targetWorker, int targetStart)
        {
            var newTicks = state.History.Workers[worker].Ticks.Take(segmentStart).ToList();
            while (newTicks.Count > startIndexes[worker] - 1 && !newTicks.Last().Wrapped)
                newTicks.RemoveAt(newTicks.Count - 1);

            var segment = state.History.Workers[worker].Ticks.Skip(segmentStart).Take(segmentEnd - segmentStart + 1).ToList();

            segmentEnd++;
            while (segmentEnd < state.History.Workers[worker].Ticks.Count && !state.History.Workers[worker].Ticks[segmentEnd].Wrapped)
                segmentEnd++;

            var wasTicks = newTicks.Count;
            newTicks.AddRange(state.History.Workers[worker].Ticks.Skip(segmentEnd));
            var beforeFix = newTicks.Count;
            FixStep(newTicks, wasTicks - 1);
            var afterFix = newTicks.Count;

            var newTargetTicks = newTicks;
            if (targetWorker == worker)
            {
                if (targetStart > wasTicks)
                    targetStart += (afterFix - beforeFix - segmentEnd + wasTicks);
            }
            else
            {
                newTargetTicks = state.History.Workers[targetWorker].Ticks.ToList();
            }

            newTargetTicks.InsertRange(targetStart, segment);
            FixStep(newTargetTicks, targetStart + segment.Count - 1);
            FixStep(newTargetTicks, targetStart - 1);

            if (targetWorker == worker)
            {
                if (state.History.Workers[worker].Ticks.Count <= newTicks.Count)
                    return false;
            }
            else
            {
                if (state.History.Workers[worker].Ticks.Count <= newTicks.Count && state.History.Workers[targetWorker].Ticks.Count <= newTargetTicks.Count)
                    return false;

                if (state.History.Workers[worker].Ticks.Count <= newTicks.Count || state.History.Workers[targetWorker].Ticks.Count <= newTargetTicks.Count)
                {
                    var prevTime = state.History.CalculateTime();
                    var newTime1 = state.History.Workers[worker].StartTick + newTicks.Count - 1;
                    var newTime2 = state.History.Workers[targetWorker].StartTick + newTargetTicks.Count - 1;
                    var newTime = Math.Max(newTime1, newTime2);
                    if (prevTime <= newTime)
                        return false;    
                }
            }

            state.History.Workers[worker].Ticks = newTicks;
            state.History.Workers[targetWorker].Ticks = newTargetTicks;
            return true;
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
                segment.Add(
                    new TickWorkerState()
                    {
                        Position = p,
                        Direction = tiks[position].Direction
                    });
            }

            var d1 = tiks[position].Direction;
            var d2 = tiks[position + 1].Direction;
            if (d1 == d2)
            {
            }
            else if (d1.Rotate(1) == d2 || d1.Rotate(-1) == d2)
            {
                segment.Add(
                    new TickWorkerState()
                    {
                        Position = tiks[position + 1].Position,
                        Direction = (segment.LastOrDefault() ?? tiks[position]).Direction
                    });
            }
            else
            {
                segment.Add(
                    new TickWorkerState()
                    {
                        Position = tiks[position + 1].Position,
                        Direction = (segment.LastOrDefault() ?? tiks[position]).Direction
                    });
                segment.Add(
                    new TickWorkerState()
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