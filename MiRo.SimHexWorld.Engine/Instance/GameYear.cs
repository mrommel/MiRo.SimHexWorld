using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace MiRo.SimHexWorld.Engine.Instance
{
    [TestFixture]
    public class GameYear : IComparable, IComparable<GameYear>, IEquatable<GameYear>
    {
        public GameYear() { }

        public GameYear(int year)
        {
            if (year < 0)
                BcAd = "BC";
            else
                BcAd = "AD";

            Year = Math.Abs(year);
        }

        public int Year { get; private set; }
        public string BcAd { get; private set; }

        public int Value
        {
            get
            {
                return (BcAd == "BC") ? -Year : Year;
            }
        }

        public int CompareTo(GameYear other)
        {
            return Value - other.Value;
        }

        public int CompareTo(object other)
        {
            return Value - (other as GameYear).Value;
        }

        public override string ToString()
        {
            return "" + Year + " " + BcAd;
        }

        public override bool Equals(object obj)
        {
            return Value == (obj as GameYear).Value;
        }

        public bool Equals(GameYear obj)
        {
            return Value == obj.Value;
        }

        public static bool operator ==(GameYear a1, GameYear a2)
        {
            return a1.Value == a2.Value;
        }

        public static bool operator !=(GameYear a1, GameYear a2)
        {
            return a1.Value != a2.Value;
        }

        public static bool operator <(GameYear a1, GameYear a2)
        {
            return a1.Value < a2.Value;
        }

        public static bool operator >(GameYear a1, GameYear a2)
        {
            return a1.Value > a2.Value;
        }

        #region tests

        [Test]
        public static void TestEquality()
        {
            Assert.True(new GameYear(-2000) == new GameYear(-2000), "2000 BC should be == 2000 BC");
            Assert.AreEqual(new GameYear(-2000), new GameYear(-2000), "2000 BC should equal 2000 BC");
            Assert.AreEqual(new GameYear(0), new GameYear(0), "0 AD should be 0 AD");
            Assert.AreEqual(new GameYear(2050), new GameYear(2050), "2050 AD should be 2050 AD");
        }

        [Test]
        public static void TestRelations()
        {
            Assert.Less(new GameYear(-4000), new GameYear(-2000), "4000 BC should be smaller than 2000 BC");
            Assert.Less(new GameYear(-2000), new GameYear(0), "2000 BC should be smaller than 0 AD");
            Assert.Less(new GameYear(0), new GameYear(2050), "0 AD should be smaller than 2050 AD");
        }

        [Test]
        public static void TestToString()
        {
            Assert.AreEqual(new GameYear(-4000).ToString(), "4000 BC");
            Assert.AreEqual(new GameYear(-2000).ToString(), "2000 BC");
            Assert.AreEqual(new GameYear(0).ToString(), "0 AD");
            Assert.AreEqual(new GameYear(1000).ToString(), "1000 AD");
            Assert.AreEqual(new GameYear(2050).ToString(), "2050 AD");
        }

        #endregion tests
    }
}
