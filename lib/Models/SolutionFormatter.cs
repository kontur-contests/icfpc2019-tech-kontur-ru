using System.Collections.Generic;

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