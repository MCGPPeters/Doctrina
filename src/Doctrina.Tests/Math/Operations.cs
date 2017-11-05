using System;
using Doctrina.Math;

namespace Doctrina.Tests.Math
{
    public static class Operations
    {
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static T Identity<T>(T t) => t;

        /// <summary>
        ///     Logistic function. Sigmoid functions have domain of all real numbers, with return value monotonically increasing
        ///     most often from 0 to 1 or alternatively from −1 to 1, depending on convention.
        ///     It is used to normalize real values to values between 0 and 1
        /// </summary>
        /// <param name="z"></param>
        /// <returns></returns>
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ProperFraction Sigmoid(double z) => 1 / (1 + System.Math.Exp(-z));

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