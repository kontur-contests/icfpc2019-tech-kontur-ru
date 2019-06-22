using System.Collections.Generic;
using lib.Models.Actions;

namespace lib.Models
{
    public static class SolutionFormatter
    {
        public static string Format(this IEnumerable<ActionBase> actions)
        {
            return string.Join("", actions);
        }
    }
}