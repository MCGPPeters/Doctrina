using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Doctrina.Math.LinearAlgebra;
using static System.Math;

namespace Doctrina.Functions
{
    public static class Activation
    {
        /// <summary>
        ///     Logistic function. Sigmoid functions have domain of all real numbers, with return value monotonically increasing
        ///     from (almost) 0 upto to (almost) 1
        ///     It is used to normalize real values to values between 0 and 1
        /// 
        ///     The sigmoid can be used for binary classification, by interpreting the output of the function as 
        ///     the propability that x belongs to class '1' (output &gt; 0.5) or '0' (otherwise)
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Sigmoid(double x)
        {
            var result =  1 / (1 + Exp(-x));
            return result;
            //partial derivative return (result, () => result * (1d - result));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector Sigmoid(Vector x)
        {
            return x.Each(d => Sigmoid(d));
        }

        /// <summary>
        /// can be used for binary classification, by interpreting the output of the function as 
        /// the propability that x belongs to class '1' (output &gt; 0.5) or '0' (otherwise)
        /// </summary>
        /// <param name="x">The x.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double HyperBolicTangent(double x)
        {
            return (Exp(2 * x) - 1) / (Exp(2 * x) + 1);
            //return (result, () => 1 - Pow(result, 2));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double RectifiedLinear(double x)
        {
            return Max(0, x);
            //return (result, () => x > 0d ? 1 : 0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector RectifiedLinear(Vector x)
        {
            return x.Each(d => RectifiedLinear(d));
            //return (result, () => x > 0d ? 1 : 0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double LeakyRectifiedLinear(double x)
        {
            return x > 0 ? x : 0.01 * x;
            //return (result, () => x > 0d ? 1 : 0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector LeakyRectifiedLinear(Vector x)
        {
            return x.Each(d => LeakyRectifiedLinear(d));
            //return (result, () => x > 0d ? 1 : 0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector SoftMax(Vector x)
        {
            var max = x.Max();
            var softmax = x.Each(v => Exp(v - max));

            var sum = softmax.Sum();

            softmax = softmax.Each(s => s / sum);

            return softmax;

            //return d * (1d - d);
        }
    }
}