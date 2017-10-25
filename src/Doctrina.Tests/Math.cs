using System;

namespace Doctrina.Tests
{
    public static class Math
    {
        // Arg Max 
        /// <summary>
        ///   Gets the maximum element in a vector.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static T ArgMax<T>(this T[] values)
            where T : IComparable<T>
        {
            var max = values[0];
            for (var i = 1; i < values.Length; i++)
            {
                if (values[i].CompareTo(max) > 0)
                {
                    max = values[i];
                }
            }

            return max;
        }
    }
}