using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MiRo.SimHexWorld.Engine.World.Maps
{
    /// <summary>
    /// directions
    /// </summary>
    public class HexDirection
    {
        public const int None = -1;
        public const int NorthEast = 0;
        public const int East = 1;
        public const int SouthEast = 2;
        public const int SouthWest = 3;
        public const int West = 4;
        public const int NorthWest = 5;

        private readonly int _value;

        static readonly Random Rnd = new Random();

        public HexDirection(int dir)
        {
            _value = dir % 6;
        }

        /// <summary>
        /// list of NorthEast, East, SouthEast, SouthWest, West, NorthWest
        /// </summary>
        public static IEnumerable<HexDirection> All
        {
            get
            {
                return new HexDirection[] { NorthEast, East, SouthEast, SouthWest, West, NorthWest };
            }
        }

        float offset = MathHelper.TwoPi / 12f;
        float turn = MathHelper.TwoPi / 6f;

        public float Angle
        {
            get
            {
                switch (_value)
                {
                    case NorthEast: // NorthEast
                        return offset + turn * 2f;
                    case East: // East - good
                        return offset + turn;
                    case SouthEast: // SouthEast
                        return offset;
                    case SouthWest:
                        return offset + turn * 5f;
                    case West: // good
                        return offset + turn * 4f;
                    case NorthWest:
                        return offset + turn * 3f;
                    default:
                        throw new Exception("no angle for " + _value);
                }
            }
        }

        public HexDirection(String dir)
        {
            if (dir[0].CompareTo('A') == 0)
                _value = NorthEast;
            else if (dir[0].CompareTo('B') == 0)
                _value = East;
            else if (dir[0].CompareTo('C') == 0)
                _value = SouthEast;
            else if (dir[0].CompareTo('D') == 0)
                _value = SouthWest;
            else if (dir[0].CompareTo('E') == 0)
                _value = West;
            else if (dir[0].CompareTo('F') == 0)
                _value = NorthWest;
            else
                throw new ArgumentException(String.Format("Unknown Direction: {0}", dir[0]));
        }

        public int Direction
        {
            get
            {
                return _value;
            }
        }

        public HexDirection Reverse
        {
            get
            {
                return MapDirection(_value + 3);
            }
        }

        public static HexDirection Random
        {
            get
            {
                return new HexDirection(Rnd.Next(6));
            }
        }

        private static int MapDirection(int newValue)
        {
            while (newValue < 0)
                newValue += 6;

            if (newValue > 5)
                newValue %= 6;

            return newValue;
        }

        public static HexDirection operator +(HexDirection hdir, int dir)
        {
            return new HexDirection(MapDirection(hdir.Direction + dir));
        }

        public static HexDirection operator -(HexDirection hdir, int dir)
        {
            return new HexDirection(MapDirection(hdir.Direction - dir));
        }

        public static implicit operator int(HexDirection dir)
        {
            return dir.Direction;
        }

        public static implicit operator HexDirection(int dir)
        {
            return new HexDirection(dir);
        }

        public override string ToString()
        {
            switch (_value)
            {
                case 0:
                    return "NorthEast";
                case 1:
                    return "East";
                case 2:
                    return "SouthEast";
                case 3:
                    return "SouthWest";
                case 4:
                    return "West";
                case 5:
                    return "NorthWest";
                default:
                    return new HexDirection(MapDirection(_value)).ToString();
            }
        }
    }
}
