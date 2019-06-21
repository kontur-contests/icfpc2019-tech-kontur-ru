using System;
using System.Collections.Generic;
using System.Text;
using lib.Models;

namespace lib
{
    public static class Extensions
    {
        public static V RotateAroundZero(this V p, bool clockwise)
        {
            return clockwise 
                ? new V(p.Y, -p.X) 
                : new V(-p.Y, p.X);
        }
    }
}
