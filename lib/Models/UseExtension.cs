using System;
using System.Linq;

namespace lib.Models
{
    public class UseExtension : ActionBase
    {
        public UseExtension(V relative)
        {
            Relative = relative;
        }

        public V Relative { get; }
        
        public override string ToString()
        {
            return $"B{Relative}";
        }

        public override void Apply(State state)
        {
            if (state.Worker.ExtensionCount <= 0)
                throw new InvalidOperationException("No extensions");

            var attachPositions = state.Worker.Manipulators.ToList();
            attachPositions.Add(V.Zero);
            
            if (attachPositions.Any(x => x == Relative))
                throw new InvalidOperationException($"Manipulator {Relative} already exists");
            
            if (attachPositions.All(x => (x - Relative).MLen() != 1))
                throw new InvalidOperationException($"Manipulator {Relative} should be attached to existing manipulator or body");
                
            state.Worker.ExtensionCount--;
            state.Worker.Manipulators.Add(Relative);
                
            state.Wrap();
        }
    }
}