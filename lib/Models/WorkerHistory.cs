using System.Collections.Generic;

namespace lib.Models
{
    public class WorkerHistory
    {
        public int StartTick { get; set; }
        public List<TickWorkerState> Ticks { get; set; } = new List<TickWorkerState>();
    }
}