using System;
using System.Linq;

namespace lib.Models.Actions
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

        public override Action Apply(State state, Worker worker)
        {
            if (state.ExtensionCount <= 0)
                throw new InvalidOperationException("No extensions");

            var attachPositions = worker.Manipulators.ToList();
            attachPositions.Add(V.Zero);

            if (attachPositions.Any(x => x == Relative))
                throw new InvalidOperationException($"Manipulator {Relative} already exists");

            if (attachPositions.All(x => (x - Relative).MLen() != 1))
                throw new InvalidOperationException($"Manipulator {Relative} should be attached to existing manipulator or body");

            state.ExtensionCount--;
            worker.Manipulators.Add(Relative);

            var unwrap = state.Wrap();
            return () => 
            {
                unwrap();
                state.ExtensionCount++;
            };
        }
    }
}