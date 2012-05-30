using System;
using System.Collections.Generic;
using System.Linq;

namespace MiRo.SimHexWorld.Engine.World.Helper
{
    public static class EnumerableExtensions
    {
        private static Random rnd = new Random();
        public static IEnumerable<T> Shift<T>(this IEnumerable<T> source, int shiftBy)
        {
            List<T> newList = new List<T>();

            for (int i = 0; i < source.Count(); ++i)
                newList.Add(source.ElementAt((i + shiftBy) % source.Count()));

            return newList;
        }

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
        {
            return source.Shuffle(rnd);
        }

        public static IEnumerable<T> Shuffle<T>(
            this IEnumerable<T> source, Random rng)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (rng == null) throw new ArgumentNullException("rng");

            return source.ShuffleIterator(rng);
        }

        private static IEnumerable<T> ShuffleIterator<T>(
            this IEnumerable<T> source, Random rng)
        {
            T[] buffer = source.ToArray();
            for (int n = 0; n < buffer.Length; n++)
            {
                int k = rng.Next(n, buffer.Length);
                yield return buffer[k];

                buffer[k] = buffer[n];
            }
        }

        public static T PickRandom<T>(this IList<T> list)
        {
            return list[rnd.Next(list.Count)];
        }

        public static bool IsSubset<T>(this IList<T> list, IList<T> test)
        {
            bool subset = true;

            foreach (T t in test)
                if (!list.Contains(t))
                    subset = false;

            return subset;
        }

        public static string ToString<T>(this IList<T> list)
        {
            return string.Join(", ", list);
        }

    }
}
