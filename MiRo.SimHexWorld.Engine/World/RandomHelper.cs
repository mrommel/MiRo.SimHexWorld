using System;
using Microsoft.Xna.Framework;

namespace MiRo.SimHexWorld.Engine.World
{
    public static class RandomHelper
    {
        private static readonly Random Rnd = new Random();
        public static Vector3 Position(Vector3 center, float dxdy)
        {
            return new Vector3(center.X + (float)Rnd.NextDouble() * dxdy, 0f, center.Z + (float)Rnd.NextDouble() * dxdy);
        }

/*
        public static T EnumValue<T>()
        {
            return Enum
                .GetValues(typeof(T))
                .Cast<T>()
                .OrderBy(x => rnd.Next())
                .FirstOrDefault();
        }
*/

        public static T EnumValue<T>(T t1, T t2, T t3)
        {
            int pos = Rnd.Next(3);

            return pos == 0 ? t1 : pos == 1 ? t2 : t3;
        }

        public static T EnumValue<T>(T t1, T t2)
        {
            int pos = Rnd.Next(2);

            return pos == 0 ? t1 : t2;
        }
    }
}