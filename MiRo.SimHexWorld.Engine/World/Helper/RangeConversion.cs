using System;
using System.Collections.Generic;

namespace MiRo.SimHexWorld.Engine.World.Helper
{
    public class RangeConversion<TA, TB> where TA : IComparable
    {
        private class Range
        {
            private readonly TA _from;
            private readonly TA _to;
            public readonly TB Result;

            public Range(TA from, TA to, TB result)
            {
                _from = RangeMath<TA>.Min(from, to);
                _to = RangeMath<TA>.Max(from, to);
                Result = result;
            }

            public bool IsInRange(TA value)
            {
                return _from.CompareTo(value) <= 0 && _to.CompareTo(value) >= 0;
            }

            public override string ToString()
            {
                return "Range[" + _from + ", " + _to + "] => " + Result;
            }
        }

        public RangeConversion(TB def)
        {
            _default = def;
        }

        private readonly List<Range> _ranges = new List<Range>();
        private readonly TB _default;

        public void AddMapping(TA from, TA to, TB value)
        {
            _ranges.Add(new Range(from, to, value));
        }

        public TB Find(TA value)
        {
            foreach (Range r in _ranges)
                if (r.IsInRange(value))
                    return r.Result;

            return _default;
        }

        //public override string ToString()
        //{
        //    return "RangeConversion: [" + ranges.ToString<Range>() + ", default: " + _default.ToString() + " ]";
        //}
    }
}
