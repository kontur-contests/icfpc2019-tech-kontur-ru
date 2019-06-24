using lib.Models.Actions;

namespace lib.Models
{
    public class TickWorkerState
    {
        public V Position { get; set; }
        public Direction Direction { get; set; }
        public bool Wrapped { get; set; }
        public ActionBase Action { get; set; }
        
        public override string ToString() => $"{nameof(Position)}: {Position}, {nameof(Direction)}: {Direction}, {nameof(Wrapped)}: {Wrapped}, {nameof(Action)}: {Action}";
    }
}