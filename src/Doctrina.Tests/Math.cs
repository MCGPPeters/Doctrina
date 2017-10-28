using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Win32;

namespace Doctrina.Tests
{

    /// <summary>
    /// Naming conventions. These conventions are used a lot in machine learning. The following
    /// is a list of these conventions and the naming conventions used for variables in this library:
    /// 
    /// Notation:
    /// symbol (variable name) : Definition
    /// 
    /// α (alpha) : Learning rate a.k.a. step size. 
    /// J (function) : A function with some parameter vector
    /// h (hyptothesis) : A candidate function for which we want to determine if is fits a training sample
    /// </summary>
    public static class Regression
    {
        /// <summary>
        /// We can measure the accuracy of our hypothesis function by using a cost function (J). 
        /// This takes an average difference (actually a fancier version of an average) 
        /// of all the results of the hypothesis with inputs from x's and the actual output y's.
        /// 
        /// Squared error cost function
        /// </summary>
        /// <param name="hypothesis">The hypothesis.</param>
        /// <param name="samples">The samples.</param>
        /// <returns></returns>
        public static double MeanSquaredError(Func<double, double> hypothesis, IEnumerable<(double x, double y)> samples)
        {
            return samples.Average(sample => System.Math.Pow(hypothesis(sample.x) - sample.y, 2));
        }

        /// <summary>
        /// Learn the parameter value for the cost function for linear regression (MeanSquaredError)
        /// </summary>
        /// <param name="alpha">The alpha.</param>
        /// <param name="function">The cost function for which we want to learn the parameter values</param>
        /// <returns></returns>
        public static double GradientDescent(double alpha, Func<double, double> function)
        {
            
        }

        public static double Linear()
    }



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

        /// <summary>
        /// Adjusts the estimate towards the error between the next estimate (target)
        /// and the current estimate given a step size α (alpha).
        /// </summary>
        /// <param name="estimate">The estimate to adjust.</param>
        /// <param name="alpha">The step size or learning rate</param>
        /// <param name="target">The target.</param>
        /// <returns></returns>
        public static double Adjust(this double estimate, double alpha, double target)
        {
            var error = target - estimate;
            return estimate + alpha * error;
        }
    }


}