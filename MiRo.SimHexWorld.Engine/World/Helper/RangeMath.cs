using System;

namespace MiRo.SimHexWorld.Engine.World.Helper
{
    public static class RangeMath<TA> where TA : IComparable
    {
        public static TA Min(TA a1, TA a2)
        {
            return a1.CompareTo(a2) <= 0 ? a1 : a2;
        }

        public static TA Max(TA a1, TA a2)
        {
            return a1.CompareTo(a2) >= 0 ? a1 : a2;
        }
    }
}
