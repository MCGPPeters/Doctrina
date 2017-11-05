// file:	Math\LinearAlgebra\VectorStatics.cs
//
// summary:	Implements the vector statics class

using System;
using System.Linq;

namespace Doctrina.Math.LinearAlgebra
{
    /// <summary>A vector.</summary>
    public partial class Vector
    {
        /// <summary>Products the given v.</summary>
        /// <param name="v">A variable-length parameters list containing v.</param>
        /// <returns>A double.</returns>
        public static double Prod(Vector v)
        {
            var prod = v[0];
            for (var i = 1; i < v.Length; i++)
                prod *= v[i];
            return prod;
        }

        /// <summary>Sums the given v.</summary>
        /// <param name="v">A variable-length parameters list containing v.</param>
        /// <returns>A double.</returns>
        public static double Sum(Vector v) => v.ToArray().Sum();

        /// <summary>Outers.</summary>
        /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
        /// <param name="x">The Vector to process.</param>
        /// <param name="y">The Vector to process.</param>
        /// <returns>A Matrix.</returns>
        public static Matrix Outer(Vector x, Vector y)
        {
            if (x.Length != y.Length)
                throw new InvalidOperationException("Dimensions do not match!");

            var n = x.Length;
            var m = new Matrix(n);
            for (var i = 0; i < n; i++)
            for (var j = 0; j < n; j++)
                m[i, j] = x[i] * y[j];

            return m;
        }

        /// <summary>
        ///     Returns the vector raised to the specified power.
        /// </summary>
        /// <param name="v">Vector to raise to the power of.</param>
        /// <param name="power">Double that specifies the power.</param>
        /// <returns></returns>
        public static Vector Pow(Vector v, double power)
        {
            return Calc(v, d => System.Math.Pow(d, power));
        }

        /// <summary>Exponents the given v.</summary>
        /// <param name="v">A variable-length parameters list containing v.</param>
        /// <returns>A Vector.</returns>
        public static Vector Exp(Vector v)
        {
            return Calc(v, d => System.Math.Exp(d));
        }

        /// <summary>Logs the given v.</summary>
        /// <param name="v">A variable-length parameters list containing v.</param>
        /// <returns>A Vector.</returns>
        public static Vector Log(Vector v)
        {
            return Calc(v, d => System.Math.Log(d));
        }

        /// <summary>Calcs.</summary>
        /// <param name="v">A variable-length parameters list containing v.</param>
        /// <param name="f">The Func&lt;int,double,double&gt; to process.</param>
        /// <returns>A Vector.</returns>
        public static Vector Calc(Vector v, Func<double, double> f)
        {
            var result = v.ToArray();
            for (var i = 0; i < v.Length; i++)
                result[i] = f(result[i]);
            return result;
        }

        /// <summary>Calcs.</summary>
        /// <param name="v">A variable-length parameters list containing v.</param>
        /// <param name="f">The Func&lt;int,double,double&gt; to process.</param>
        /// <returns>A Vector.</returns>
        public static Vector Calc(Vector v, Func<int, double, double> f)
        {
            var result = v.ToArray();
            for (var i = 0; i < v.Length; i++)
                result[i] = f(i, result[i]);
            return result;
        }

        /// <summary>Ones.</summary>
        /// <param name="n">The int to process.</param>
        /// <returns>A Vector.</returns>
        public static Vector Ones(int n)
        {
            var x = new double[n];
            for (var i = 0; i < n; i++)
                x[i] = 1;

            return new Vector(x);
        }

        /// <summary>Zeros.</summary>
        /// <param name="n">The int to process.</param>
        /// <returns>A Vector.</returns>
        public static Vector Zeros(int n) => new Vector(n);

        /// <summary>Rands.</summary>
        /// <param name="n">The int to process.</param>
        /// <returns>A Vector.</returns>
        //public static Vector Rand(int n)
        //{
        //    var x = new double[n];
        //    for (var i = 0; i < n; i++)
        //        x[i] = Sampling.GetUniform();

        //    return new Vector(x);
        //}

        ///// <summary>
        /////     Random initializes a new Vector of size n, between a range.
        ///// </summary>
        ///// <param name="n">Vector dimension.</param>
        ///// <param name="min">Lower bound of the random initialization.</param>
        ///// <param name="max">Upper bound of the random initialization.</param>
        ///// <returns>Vector.</returns>
        //public static Vector Rand(int n, double min, double max)
        //{
        //    var x = new double[n];
        //    for (var i = 0; i < n; i++)
        //        x[i] = Sampling.GetUniform(min, max);

        //    return new Vector(x);
        //}

        ///// <summary>Normalise random.</summary>
        ///// <param name="n">The int to process.</param>
        ///// <param name="mean">(Optional) the mean.</param>
        ///// <param name="stdDev">(Optional) the standard development.</param>
        ///// <param name="precision">(Optional) the precision.</param>
        ///// <returns>A Vector.</returns>
        //public static Vector NormRand(int n, double mean = 0, double stdDev = 1, int precision = -1)
        //{
        //    var x = new double[n];
        //    for (var i = 0; i < n; i++)
        //        if (precision > -1)
        //            x[i] = System.Math.Round(Sampling.GetNormal(mean, stdDev), precision);
        //        else
        //            x[i] = Sampling.GetNormal(mean, stdDev);

        //    return new Vector(x);
        //}
        /// <summary>Dots.</summary>
        /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
        /// <param name="one">The one.</param>
        /// <param name="two">The two.</param>
        /// <returns>A double.</returns>
        public static double Dot(Vector one, Vector two)
        {
            if (one.Length != two.Length)
                throw new InvalidOperationException("Dimensions do not match!");

            double total = 0;
            for (var i = 0; i < one.Length; i++)
                total += one[i] * two[i];
            return total;
        }

        /// <summary>Normals.</summary>
        /// <param name="x">The Vector to process.</param>
        /// <returns>A double.</returns>
        public static double Norm(Vector x) => Norm(x, 2);

        /// <summary>Normals.</summary>
        /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
        /// <param name="x">The Vector to process.</param>
        /// <param name="p">The double to process.</param>
        /// <returns>A double.</returns>
        public static double Norm(Vector x, double p)
        {
            if (p < 1) throw new InvalidOperationException("p must be greater than 0");
            double value = 0;
            if (p == 1)
            {
                for (var i = 0; i < x.Length; i++)
                    value += System.Math.Abs(x[i]);

                return value;
            }
            if (p == int.MaxValue)
            {
                for (var i = 0; i < x.Length; i++)
                    if (System.Math.Abs(x[i]) > value)
                        value = System.Math.Abs(x[i]);
                return value;
            }
            if (p == int.MinValue)
            {
                for (var i = 0; i < x.Length; i++)
                    if (System.Math.Abs(x[i]) < value)
                        value = System.Math.Abs(x[i]);
                return value;
            }
            for (var i = 0; i < x.Length; i++)
                value += System.Math.Pow(System.Math.Abs(x[i]), p);

            return System.Math.Pow(value, 1 / p);
        }

        /// <summary>Diags.</summary>
        /// <param name="v">A variable-length parameters list containing v.</param>
        /// <returns>A Matrix.</returns>
        public static Matrix Diag(Vector v)
        {
            var m = Matrix.Zeros(v.Length);
            for (var i = 0; i < v.Length; i++)
                m[i, i] = v[i];
            return m;
        }

        /// <summary>Diags.</summary>
        /// <param name="v">A variable-length parameters list containing v.</param>
        /// <param name="n">The int to process.</param>
        /// <param name="d">The int to process.</param>
        /// <returns>A Matrix.</returns>
        public static Matrix Diag(Vector v, int n, int d)
        {
            var m = Matrix.Zeros(n, d);
            var min = System.Math.Min(n, d);
            for (var i = 0; i < min; i++)
                m[i, i] = v[i];
            return m;
        }

        /// <summary>Rounds.</summary>
        /// <param name="v">A variable-length parameters list containing v.</param>
        /// <param name="decimals">(Optional) the decimals.</param>
        /// <returns>A Vector.</returns>
        public static Vector Round(Vector v, int decimals = 0)
        {
            for (var i = 0; i < v.Length; i++)
                v[i] = System.Math.Round(v[i], decimals);
            return v;
        }

        /// <summary>Combines the given v.</summary>
        /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
        /// <param name="v">A variable-length parameters list containing v.</param>
        /// <returns>A Vector.</returns>
        public static Vector Combine(params Vector[] v)
        {
            if (v.Length == 0)
                throw new InvalidOperationException("Need to specify vectors to combine!");

            if (v.Length == 1)
                return v[0];

            var size = 0;
            for (var i = 0; i < v.Length; i++)
                size += v[i].Length;

            var r = new Vector(size);
            var z = -1;
            for (var i = 0; i < v.Length; i++)
            for (var j = 0; j < v[i].Length; j++)
                r[++z] = v[i][j];

            return r;
        }

        /// <summary>Sort order.</summary>
        /// <param name="vector">The vector.</param>
        /// <returns>The sorted order.</returns>
        public static Vector SortOrder(Vector vector)
        {
            return vector.Select((d, i) => new Tuple<int, double>(i, d))
                .OrderByDescending(t => t.Item2)
                .Select(t => t.Item1)
                .ToArray();
        }

        /// <summary>Query if 'vector' contains NaN.</summary>
        /// <param name="vector">The vector.</param>
        /// <returns>true if it succeeds, false if it fails.</returns>
        public static bool ContainsNaN(Vector vector)
        {
            var v = vector.ToArray();
            return v.Any(e => double.IsNaN(e));
        }

        /// <summary>Query if 'vector' is NaN.</summary>
        /// <param name="vector">The vector.</param>
        /// <returns>true if NaN, false if not.</returns>
        public static bool IsNaN(Vector vector)
        {
            var v = vector.ToArray();
            return v.All(e => double.IsNaN(e));
        }
    }
}