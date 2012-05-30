using System;
using NUnit.Framework;

namespace MiRo.SimHexWorld.Engine.World
{
    public class Range<T> where T : IComparable<T>, IEquatable<T>
    {
        T _from, _to;

        public Range(T from, T to)
        {
            _from = from;
            _to = to;
        }

        public bool IsIn(T value)
        {
            return _from.CompareTo( value ) <= 0 && value.CompareTo(_to ) <= 0; 
        }

        public T From
        {
            get
            {
                return _from;
            }
        }

        public T To
        {
            get
            {
                return _to;
            }
        }
    }

    [TestFixture]
    public class IntRangeTest
    {
        #region tests

        [Test]
        public static void Test0In0To5Range()
        {
            var r = new Range<int>(0, 5);
            Assert.True(r.IsIn(0), "0 should be in [0,5]");
        }

        [Test]
        public static void Test1In0To5Range()
        {
            var r = new Range<int>(0, 5);
            Assert.True(r.IsIn(1), "1 should be in [0,5]");
        }

        [Test]
        public static void Test2In0To5Range()
        {
            var r = new Range<int>(0, 5);
            Assert.True(r.IsIn(2), "2 should be in [0,5]");
        }

        [Test]
        public static void Test3In0To5Range()
        {
            var r = new Range<int>(0, 5);
            Assert.True(r.IsIn(3), "3 should be in [0,5]");
        }

        [Test]
        public static void Test4In0To5Range()
        {
            var r = new Range<int>(0, 5);
            Assert.True(r.IsIn(4), "4 should be in [0,5]");
        }

        [Test]
        public static void Test5In0To5Range()
        {
            var r = new Range<int>(0, 5);
            Assert.True(r.IsIn(5), "5 should be in [0,5]");
        }

        [Test]
        public static void TestMinus1In0To5Range()
        {
            Range<int> r = new Range<int>(0, 5);
            Assert.False(r.IsIn(-1), "-1 should NOT be in [0,5]");
        }

        [Test]
        public static void Test6In0To5Range()
        {
            Range<int> r = new Range<int>(0, 5);
            Assert.False(r.IsIn(6), "6 should NOT be in [0,5]");
        }
        #endregion
    }
}
