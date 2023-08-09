using System;
using System.Collections.Generic;

namespace TrackOMatic
{
    static class Extensions
    {
        private static Random random = new();

        public static void Shuffle<T>(this IList<T> list, int seed)
        {
            random = new(seed);
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
