using System;
using System.Collections.Generic;
using System.Linq;

namespace EpidemicSimulator
{
    public static class RandomHelper
    {
        static readonly Random random = new Random();

        // The sum of values must be 1.
        public static T GetRandomElement<T>(this Dictionary<T, double> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (source.Count == 0) throw new ArgumentException("The source must not be empty.", nameof(source));

            var v = random.NextDouble();

            var sum = 0.0;
            foreach (var p in source)
            {
                sum += p.Value;
                if (v < sum) return p.Key;
            }

            return source.Last().Key;
        }
    }
}
