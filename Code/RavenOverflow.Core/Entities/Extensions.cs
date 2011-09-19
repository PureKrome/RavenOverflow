using System;
using System.Collections.Generic;

namespace RavenOverflow.Core.Entities
{
    public static class Extensions
    {
        public static IList<T> ToRandomList<T>(this IList<T> source, int numberOfItems)
        {
            if (numberOfItems <= 0)
            {
                throw new ArgumentOutOfRangeException("numberOfItems");
            }

            IList<T> results = new List<T>();

            // Based upon: http://stackoverflow.com/questions/48087/select-a-random-n-elements-from-listt-in-c
            var rand = new Random();
            double needed = numberOfItems;
            int available = source.Count;

            while (results.Count < numberOfItems)
            {
                if (rand.NextDouble() < needed/available)
                {
                    results.Add(source[available - 1]);
                    needed--;
                }
                available--;
            }

            return results;
        }
    }
}