using System.Runtime.CompilerServices;
using Doctrina.Math.LinearAlgebra;

namespace Doctrina.Functions
{
    public static class Logistic
    {
        /// <summary>
        ///     Logistic function. Sigmoid functions have domain of all real numbers, with return value monotonically increasing
        ///     most often from 0 to 1 or alternatively from −1 to 1, depending on convention.
        ///     It is used to normalize real values to values between 0 and 1
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Vector Sigmoid(Vector vector)
        {
            return vector.Apply(d => 1 / (1 + System.Math.Exp(-d)));
        }
    }
}