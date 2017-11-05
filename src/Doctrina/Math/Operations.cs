using System;
using System.Runtime.CompilerServices;

namespace Doctrina.Math
{
    public static class Operations
    {
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static T Identity<T>(T t) => t;


        // Arg Max 
        /// <summary>
        ///     Gets the maximum element in a vector.
        /// </summary>
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static T ArgMax<T>(this T[] values)
            where T : IComparable<T>
        {
            var max = values[0];
            for (var i = 1; i < values.Length; i++)
                if (values[i].CompareTo(max) > 0)
                    max = values[i];

            return max;
        }

        /// <summary>
        ///     Adjusts the estimate towards the error between the next estimate (target)
        ///     and the current estimate given a step size α (alpha).
        /// </summary>
        /// <param name="estimate">The estimate to adjust.</param>
        /// <param name="alpha">The step size or learning rate</param>
        /// <param name="target">The target.</param>
        /// <returns></returns>
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double Update(this double estimate, double alpha, double target)
        {
            var error = target - estimate;
            return estimate + alpha * error;
        }
    }
}