using System;
using System.Collections.Generic;
using System.Linq;
using Doctrina.Tests.Math;

namespace Doctrina.Tests
{
    public static class EnumerableExtensions
    {
        /// <summary>
        ///     Generating linearly spaced values between <paramref name="start" /> and <paramref name="end" />
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <param name="partitions">The partitions.</param>
        /// <returns></returns>
        private static IEnumerable<double> LinearlySpaced(double start, double end, int partitions) =>
            Enumerable.Range(0, partitions + 1)
                .Select(idx => idx != partitions
                    ? start + (end - start) / partitions * idx
                    : end);

        /// <summary>
        ///     The calculation of an incremental mean work by correcting the 'error' between what we thought the mean
        /// </summary>
        /// <param name="sequence">The sequence.</param>
        /// <returns></returns>
        public static double IncrementalMean(this IEnumerable<int> sequence)
        {
            var enumerable = sequence as int[] ?? sequence.ToArray();
            if (enumerable.Length <= 0) return 0;
            double mean = enumerable[0];

            for (var k = 1; k < enumerable.Length; k++)
            {
                double count = k + 1;
                var alpha = 1 / count;
                var target = enumerable[k];
                mean = mean.Update(alpha, target);
            }
            return mean;
        }

        public static IEnumerable<T> Closure<T>(
            T root,
            Func<T, IEnumerable<T>> children)
        {
            var seen = new HashSet<T>();
            var stack = new Stack<T>();
            stack.Push(root);

            while (stack.Count != 0)
            {
                var item = stack.Pop();
                if (seen.Contains(item))
                    continue;
                seen.Add(item);
                yield return item;
                foreach (var child in children(item))
                    stack.Push(child);
            }
        }
    }
}