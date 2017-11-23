// file:	Math\LinearAlgebra\MatrixStatics.cs
//
// summary:	Implements the matrix statics class

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using numl.Math.LinearAlgebra;

namespace Doctrina.Math.LinearAlgebra
{
    /// <summary>A matrix.</summary>
    public partial class Matrix
    {
        /// <summary>
        ///     Performs an element wise operation on the input Matrix.
        /// </summary>
        /// <param name="m">Matrix.</param>
        /// <param name="fnElementWiseOp">Function to apply.</param>
        /// <returns>A Matrix.</returns>
        public static Matrix Each(Matrix m, Func<double, double> fnElementWiseOp)
        {
            var copy = m.ToArray();
            for (var i = 0; i < m.Rows; i++)
            for (var j = 0; j < m.Cols; j++)
                copy[i][j] = fnElementWiseOp(copy[i][j]);
            return copy;
        }

        /// <summary>
        ///     Performs an element wise operation on the input Matrix.
        /// </summary>
        /// <param name="m">Matrix.</param>
        /// <param name="fnElementWiseOp">Function to update each cell specified by the value and cell coordinates.</param>
        /// <returns>A Matrix.</returns>
        public static Matrix Each(Matrix m, Func<double, int, int, double> fnElementWiseOp)
        {
            var copy = m.ToArray();
            for (var i = 0; i < m.Rows; i++)
            for (var j = 0; j < m.Cols; j++)
                copy[i][j] = fnElementWiseOp(copy[i][j], i, j);
            return copy;
        }

        /// <summary>
        ///     Performs an element-wise operation on the input Matrices.
        /// </summary>
        /// <param name="m1">First Matrix.</param>
        /// <param name="m2">Second Matrix.</param>
        /// <param name="fnElementWiseOp">Operation to perform on the value from the first and second matrices.</param>
        /// <returns>A Matrix.</returns>
        public static Matrix Each(Matrix m1, Matrix m2, Func<double, double, double> fnElementWiseOp)
        {
            if (m1.Rows != m2.Rows)
                throw new InvalidOperationException("The row dimensions do not match");
            if (m1.Cols != m2.Cols)
                throw new InvalidOperationException("The column dimensions do not match");

            var copy = m1.ToArray();
            for (var i = 0; i < m1.Rows; i++)
            for (var j = 0; j < m1.Cols; j++)
                copy[i][j] = fnElementWiseOp(m1[i, j], m2[i, j]);
            return copy;
        }

        /// <summary>Computes the trace of a matrix.</summary>
        /// <param name="m">Input Matrix.</param>
        /// <returns>trace.</returns>
        public static double Trace(Matrix m)
        {
            double t = 0;
            for (var i = 0; i < m.Rows && i < m.Cols; i++)
                t += m[i, i];
            return t;
        }

        /// <summary>Computes the sum of every element of the matrix.</summary>
        /// <param name="m">Input Matrix.</param>
        /// <returns>sum.</returns>
        public static double Sum(Matrix m)
        {
            double sum = 0;
            for (var i = 0; i < m.Rows; i++)
            for (var j = 0; j < m.Cols; j++)
                sum += m[i, j];
            return sum;
        }

        /// <summary>
        ///     Computes the sum of either the rows or columns of a matrix and returns a vector.
        /// </summary>
        /// <param name="m">Input Matrix.</param>
        /// <param name="t">Row or Column sum.</param>
        /// <returns>Vector Sum.</returns>
        public static Vector Sum(Matrix m, VectorType t)
        {
            if (t == VectorType.Row)
            {
                var result = new Vector(m.Cols);
                for (var i = 0; i < m.Cols; i++)
                for (var j = 0; j < m.Rows; j++)
                    result[i] += m[j, i];
                return result;
            }
            else
            {
                var result = new Vector(m.Rows);
                for (var i = 0; i < m.Rows; i++)
                for (var j = 0; j < m.Cols; j++)
                    result[i] += m[i, j];
                return result;
            }
        }

        /// <summary>Computes the sum of every element of the matrix.</summary>
        /// <param name="m">Input Matrix.</param>
        /// <param name="i">Zero-based index of the.</param>
        /// <param name="t">Row or Column sum.</param>
        /// <returns>sum.</returns>
        public static double Sum(Matrix m, int i, VectorType t) => m[i, t].Sum();

        /// <summary>Standard Matrix Norm.</summary>
        /// <param name="A">Input Matrix.</param>
        /// <param name="p">The double to process.</param>
        /// <returns>Standard Norm (double)</returns>
        public static double Norm(Matrix A, double p)
        {
            double norm = 0;
            for (var i = 0; i < A.Rows; i++)
            for (var j = 0; j < A.Cols; j++)
                norm += System.Math.Pow(System.Math.Abs(A[i, j]), p);
            return System.Math.Pow(norm, 1d / p);
        }

        /// <summary>Matrix Frobenius Norm.</summary>
        /// <param name="A">Input Matrix.</param>
        /// <returns>Frobenius Norm (double)</returns>
        public static double FrobeniusNorm(Matrix A) => System.Math.Sqrt((A.T * A).Trace());

        ///// <summary>Eigen Decomposition.</summary>
        ///// <param name="A">Input Matrix.</param>
        ///// <returns>Tuple(Eigen Values, Eigen Vectors)</returns>
        //public static Tuple<Doctrina.Math.LinearAlgebra.Vector, Matrix> Evd(Matrix A)
        //{
        //    var eigs = new Evd(A);
        //    eigs.Compute();
        //    return new Tuple<Doctrina.Math.LinearAlgebra.Vector, Matrix>(eigs.Eigenvalues, eigs.Eigenvectors);
        //}

        /// <summary>Singular Value Decomposition.</summary>
        /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
        /// <param name="A">Input Matrix.</param>
        /// <returns>Tuple(Matrix U, Vector S, Matrix V)</returns>
        public static Tuple<Matrix, Vector, Matrix> SVD(Matrix A) =>
            throw new NotImplementedException();

        /// <summary>NOT IMPLEMENTED!</summary>
        /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
        /// <param name="A">.</param>
        /// <returns>A Tuple&lt;Matrix,Matrix,Matrix&gt;</returns>
        public static Tuple<Matrix, Matrix, Matrix> LU(Matrix A)
        {
            // TODO: FINISH ALGORITHM
            if (A.Rows != A.Cols)
                throw new InvalidOperationException("Factorization requires a symmetric positive semidefinite matrix!");

            var n = A.Rows;

            var P = Pivot(A);
            var M = P * A;

            var L = Identity(n);
            var U = Zeros(n);

            for (var j = 0; j < n; j++)
            {
                L[j, j] = 1;

                for (var i = 0; i < j + 1; i++)
                {
                    U[i, j] = M[i, j];
                    for (var k = 0; k < i; k++)
                        U[i, j] -= U[k, j] * L[i, k];
                }


                for (var i = j; i < n; i++)
                {
                    L[i, j] = M[i, j];
                    for (var k = 0; k < j; k++)
                        L[i, j] -= U[k, j] * L[i, k];

                    if (U[j, j] == 0)
                        Debug.WriteLine("Unstable divisor...");

                    L[i, j] /= U[j, j];
                }
            }

            return new Tuple<Matrix, Matrix, Matrix>(P, L, U);
        }

        /// <summary>Pivots the given m.</summary>
        /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
        /// <param name="M">The Matrix to process.</param>
        /// <returns>A Matrix.</returns>
        public static Matrix Pivot(Matrix M)
        {
            if (M.Rows != M.Cols)
                throw new InvalidOperationException("Factorization requires a symmetric positive semidefinite matrix!");

            var m = M.Rows;
            var P = Identity(m);
            var row = new Tuple<int, double>(0, 0);
            for (var j = 0; j < m; j++)
            {
                row = new Tuple<int, double>(j, 0);
                for (var i = j; i < m; i++)
                    if (row.Item2 < System.Math.Abs(M[i, j]))
                        row = new Tuple<int, double>(i, System.Math.Abs(M[i, j]));

                if (row.Item1 != j)
                    P.SwapRow(j, row.Item1);
            }

            return P;
        }

        /// <summary>Cholesky Factorization of a Matrix.</summary>
        /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
        /// <exception cref="SingularMatrixException">Thrown when a Singular Matrix error condition occurs.</exception>
        /// <param name="m">Input Matrix.</param>
        /// <returns>Cholesky Faxtorization (R.T would be other matrix)</returns>
        public static Matrix Cholesky(Matrix m)
        {
            if (m.Rows != m.Cols)
                throw new InvalidOperationException(
                    "Factorization requires a symmetric positive semi-definite matrix!");

            var n = m.Rows;
            var A = m.Copy();

            for (var k = 0; k < n; k++)
            {
                if (A[k, k] <= 0)
                    throw new SingularMatrixException("Matrix is not symmetric positive semi-definite!");

                A[k, k] = System.Math.Sqrt(A[k, k]);
                for (var j = k + 1; j < n; j++)
                    A[j, k] = A[j, k] / A[k, k];

                for (var j = k + 1; j < n; j++)
                for (var i = j; i < n; i++)
                    A[i, j] = A[i, j] - A[i, k] * A[j, k];
            }

            // put back zeros...
            for (var i = 0; i < n; i++)
            for (var j = i + 1; j < n; j++)
                A[i, j] = 0;

            return A;
        }

        /// <summary>Matrix Roundoff.</summary>
        /// <param name="m">Input Matrix.</param>
        /// <param name="decimals">(Optional) Max number of decimals (default 0 - integral members)</param>
        /// <returns>Rounded Matrix.</returns>
        public static Matrix Round(Matrix m, int decimals = 0)
        {
            for (var i = 0; i < m.Rows; i++)
            for (var j = 0; j < m.Cols; j++)
                m[i, j] = System.Math.Round(m[i, j], decimals);
            return m;
        }

        /// <summary>Backwards.</summary>
        /// <param name="A">Input Matrix.</param>
        /// <param name="b">The Vector to process.</param>
        /// <returns>A Vector.</returns>
        internal static Vector Backward(Matrix A, Vector b)
        {
            var x = Vector.Zeros(b.Length);
            for (var i = b.Length - 1; i > -1; i--)
            {
                double sum = 0;
                for (var j = i + 1; j < b.Length; j++)
                    sum += A[i, j] * x[j];

                x[i] = (b[i] - sum) / A[i, i];
            }

            return x;
        }

        /// <summary>Forwards.</summary>
        /// <param name="A">Input Matrix.</param>
        /// <param name="b">The Vector to process.</param>
        /// <returns>A Vector.</returns>
        internal static Vector Forward(Matrix A, Vector b)
        {
            var x = Vector.Zeros(b.Length);
            for (var i = 0; i < b.Length; i++)
            {
                double sum = 0;
                for (var j = 0; j < i; j++)
                    sum += A[i, j] * x[j];

                x[i] = (b[i] - sum) / A[i, i];
            }

            return x;
        }

        /// <summary>Dot product between a matrix and a vector.</summary>
        /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
        /// <param name="x">Matrix x.</param>
        /// <param name="v">Vector v.</param>
        /// <returns>Vector dot product.</returns>
        public static Vector Dot(Matrix x, Vector v)
        {
            if (v.Length != x.Cols)
                throw new InvalidOperationException("objects are not aligned");

            var toReturn = Vector.Zeros(x.Rows);
            for (var i = 0; i < toReturn.Length; i++)
                toReturn[i] = Vector.Dot(x[i, VectorType.Row], v);
            return toReturn;
        }

        /// <summary>Dot product between a matrix and a vector.</summary>
        /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
        /// <param name="v">Vector v.</param>
        /// <param name="x">Matrix x.</param>
        /// <returns>Vector dot product.</returns>
        public static Vector Dot(Vector v, Matrix x)
        {
            if (v.Length != x.Rows)
                throw new InvalidOperationException("objects are not aligned");

            var toReturn = Vector.Zeros(x.Cols);
            for (var i = 0; i < toReturn.Length; i++)
                toReturn[i] = Vector.Dot(x[i, VectorType.Col], v);
            return toReturn;
        }

        /// <summary>Determines the mean of the given parameters.</summary>
        /// <param name="source">Source for the.</param>
        /// <param name="t">Row or Column sum.</param>
        /// <returns>The mean value.</returns>
        public static Vector Mean(Matrix source, VectorType t)
        {
            var count = t == VectorType.Row ? source.Cols : source.Rows;
            var type = t == VectorType.Row ? VectorType.Col : VectorType.Row;
            var v = new Vector(count);
            for (var i = 0; i < count; i++)
                v[i] = source[i, type].Mean();
            return v;
        }

        /// <summary>
        ///     Computes the standard deviation of the given matrix
        /// </summary>
        /// <param name="source"></param>
        /// <param name="t">Use column or row (default: Col)</param>
        /// <returns></returns>
        public static Vector StdDev(Matrix source, VectorType t = VectorType.Col)
        {
            var count = t == VectorType.Row ? source.Cols : source.Rows;
            var type = t == VectorType.Row ? VectorType.Col : VectorType.Row;
            var v = new Vector(count);
            for (var i = 0; i < count; i++)
                v[i] = source[i, type].StdDev();
            return v;
        }

        /// <summary>
        ///     Returns a vector of the maximum values for each row/column.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="t">Use column or row (default: Col)</param>
        /// <returns></returns>
        public static Vector Max(Matrix source, VectorType t = VectorType.Col)
        {
            var num = t == VectorType.Row ? source.Cols : source.Rows;
            var vectorType = t == VectorType.Row ? VectorType.Col : VectorType.Row;
            var vectors = new Vector(num);
            for (var i = 0; i < num; i++)
                vectors[i] = source[i, vectorType].Max();
            return vectors;
        }

        /// <summary>Determines the maximum of the given parameters.</summary>
        /// <param name="source">Source for the.</param>
        /// <returns>The maximum value.</returns>
        public static double Max(Matrix source)
        {
            var max = double.MinValue;
            for (var i = 0; i < source.Rows; i++)
            for (var j = 0; j < source.Cols; j++)
                if (source[i, j] > max)
                    max = source[i, j];

            return max;
        }

        /// <summary>
        ///     Returns a vector of the minimum values for each row/column.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="t">Use column or row (default: Col)</param>
        /// <returns></returns>
        public static Vector Min(Matrix source, VectorType t = VectorType.Col)
        {
            var num = t == VectorType.Row ? source.Cols : source.Rows;
            var vectorType = t == VectorType.Row ? VectorType.Col : VectorType.Row;
            var vectors = new Vector(num);
            for (var i = 0; i < num; i++)
                vectors[i] = source[i, vectorType].Min();
            return vectors;
        }

        /// <summary>Determines the minimum of the given parameters.</summary>
        /// <param name="source">Source for the.</param>
        /// <returns>The minimum value.</returns>
        public static double Min(Matrix source)
        {
            var min = double.MaxValue;
            for (var i = 0; i < source.Rows; i++)
            for (var j = 0; j < source.Cols; j++)
                if (source[i, j] < min)
                    min = source[i, j];

            return min;
        }

        /// <summary>
        ///     Returns a vector of the median values for each row or column.
        /// </summary>
        /// <param name="source">Matrix.</param>
        /// <param name="t">VectorType.</param>
        /// <returns></returns>
        public static Vector Median(Matrix source, VectorType t = VectorType.Col)
        {
            var vectors = t == VectorType.Row ? source.GetCols() : source.GetRows();
            return vectors.Select(s => s.Median()).ToVector();
        }

        /// <summary>Covariances.</summary>
        /// <param name="source">Source for the.</param>
        /// <param name="t">(Optional) Row or Column sum.</param>
        /// <returns>A Matrix.</returns>
        public static Matrix Covariance(Matrix source, VectorType t = VectorType.Col)
        {
            var length = t == VectorType.Row ? source.Rows : source.Cols;
            var m = new Matrix(length);
            //for (int i = 0; i < length; i++)
            for (var i = 0; i < length; i++)
                //for (int j = i; j < length; j++) // symmetric matrix
            for (var j = i; j < length; j++)
                m[i, j] = m[j, i] = source[i, t].Covariance(source[j, t]);
            return m;
        }

        /// <summary>Covariance diagram.</summary>
        /// <param name="source">Source for the.</param>
        /// <param name="t">(Optional) Row or Column sum.</param>
        /// <returns>A Vector.</returns>
        public static Vector CovarianceDiag(Matrix source, VectorType t = VectorType.Col)
        {
            var length = t == VectorType.Row ? source.Rows : source.Cols;
            var vector = new Vector(length);
            for (var i = 0; i < length; i++)
                vector[i] = source[i, t].Variance();
            return vector;
        }

        /// <summary>Correlations.</summary>
        /// <param name="source">Source for the.</param>
        /// <param name="t">(Optional) Row or Column sum.</param>
        /// <returns>A Matrix.</returns>
        public static Matrix Correlation(Matrix source, VectorType t = VectorType.Col)
        {
            var length = t == VectorType.Row ? source.Rows : source.Cols;
            var m = new Matrix(length);
            for (var i = 0; i < length; i++)
            for (var j = i; j < length; j++) // symmetric matrix
                m[i, j] = m[j, i] = source[i, t].Correlation(source[j, t]);
            return m;
        }

        /// <summary>Enumerates reverse in this collection.</summary>
        /// <param name="source">Source for the.</param>
        /// <param name="t">(Optional) Row or Column sum.</param>
        /// <returns>
        ///     An enumerator that allows foreach to be used to process reverse in this collection.
        /// </returns>
        public static IEnumerable<Vector> Reverse(Matrix source,
            VectorType t = VectorType.Row)
        {
            var length = t == VectorType.Row ? source.Rows : source.Cols;
            for (var i = length - 1; i > -1; i--)
                yield return source[i, t];
        }

        /// <summary>Enumerates indices in this collection.</summary>
        /// <param name="source">Source for the.</param>
        /// <param name="f">The Func&lt;Vector,bool&gt; to process.</param>
        /// <returns>
        ///     An enumerator that allows foreach to be used to process indices in this collection.
        /// </returns>
        public static IEnumerable<int> Indices(Matrix source, Func<Vector, bool> f) =>
            Indices(source, f, VectorType.Row);

        /// <summary>Enumerates indices in this collection.</summary>
        /// <param name="source">Source for the.</param>
        /// <param name="f">The Func&lt;Vector,bool&gt; to process.</param>
        /// <param name="t">Row or Column sum.</param>
        /// <returns>
        ///     An enumerator that allows foreach to be used to process indices in this collection.
        /// </returns>
        public static IEnumerable<int> Indices(Matrix source, Func<Vector, bool> f,
            VectorType t)
        {
            var max = t == VectorType.Row ? source.Rows : source.Cols;
            for (var i = 0; i < max; i++)
                if (f(source[i, t]))
                    yield return i;
        }

        /// <summary>
        ///     Sorts the given Matrix by the specified row or column selector and returns the new Matrix
        /// </summary>
        /// <param name="source">The Matrix</param>
        /// <param name="keySelector">Property selector to sort by.</param>
        /// <param name="t">Specifies whether to sort horizontally or vertically.</param>
        /// <param name="ascending">Determines whether to sort ascending or descending (Default: True)</param>
        /// <returns>New Matrix and Vector of original indices.</returns>
        public static Matrix Sort(Matrix source, Func<Vector, double> keySelector,
            VectorType t, bool ascending = true)
        {
            Vector v;
            return Sort(source, keySelector, t, ascending, out v);
        }

        /// <summary>
        ///     Sorts the given Matrix by the specified row or column selector and returns the new Matrix
        ///     along with the original indices.
        /// </summary>
        /// <param name="source">The Matrix</param>
        /// <param name="keySelector">Property selector to sort by.</param>
        /// <param name="t">Specifies whether to sort horizontally or vertically.</param>
        /// <param name="ascending">Determines whether to sort ascending or descending (Default: True)</param>
        /// <param name="indices">Vector of <paramref name="t" /> indices in the original Matrix before the sort operation.</param>
        /// <returns>New Matrix and Vector of original indices.</returns>
        public static Matrix Sort(Matrix source, Func<Vector, double> keySelector,
            VectorType t, bool ascending, out Vector indices)
        {
            var max = t == VectorType.Row ? source.Rows : source.Cols;
            indices = Vector.Zeros(max);

            var vects = new List<Vector>(max);

            var arrays = t == VectorType.Row ? source.GetRows() : source.GetCols();

            var sort = (ascending
                ? arrays.Select((i, v) => new KeyValuePair<Vector, int>(i, v))
                    .OrderBy(o => keySelector(o.Key))
                : arrays.Select((i, v) => new KeyValuePair<Vector, int>(i, v))
                    .OrderByDescending(o => keySelector(o.Key))).ToArray();

            indices = sort.Select(s => s.Value).ToVector();

            return sort.Select(s => s.Key).ToMatrix(t);
        }

        //---------------- structural
        /// <summary>Stack a set of vectors into a matrix.</summary>
        /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
        /// <param name="type">.</param>
        /// <param name="vectors">.</param>
        /// <returns>A Matrix.</returns>
        internal static Matrix Stack(VectorType type, params Vector[] vectors)
        {
            if (vectors.Length == 0)
                throw new InvalidOperationException("Cannot construct Matrix from empty vector set!");

            if (!vectors.All(v => v.Length == vectors[0].Length))
                throw new InvalidOperationException("Vectors must all be of the same length!");

            var n = type == VectorType.Row ? vectors.Length : vectors[0].Length;
            var d = type == VectorType.Row ? vectors[0].Length : vectors.Length;

            var m = Zeros(n, d);
            for (var i = 0; i < vectors.Length; i++)
                m[i, type] = vectors[i];

            return m;
        }

        /// <summary>Stack a set of vectors into a matrix.</summary>
        /// <param name="vectors">.</param>
        /// <returns>A Matrix.</returns>
        public static Matrix Stack(params Vector[] vectors) =>
            Stack(VectorType.Row, vectors);

        /// <summary>Stacks.</summary>
        /// <param name="vectors">.</param>
        /// <returns>A Matrix.</returns>
        public static Matrix VStack(params Vector[] vectors) =>
            Stack(VectorType.Col, vectors);

        /// <summary>Stack a set of vectors into a matrix.</summary>
        /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
        /// <param name="m">Input Matrix.</param>
        /// <param name="t">Row or Column sum.</param>
        /// <returns>A Matrix.</returns>
        public static Matrix Stack(Matrix m, Matrix t)
        {
            if (m.Cols != t.Cols)
                throw new InvalidOperationException("Invalid dimension for stack operation!");

            var p = new Matrix(m.Rows + t.Rows, t.Cols);
            for (var i = 0; i < p.Rows; i++)
            for (var j = 0; j < p.Cols; j++)
                if (i < m.Rows)
                    p[i, j] = m[i, j];
                else
                    p[i, j] = t[i - m.Rows, j];

            return p;
        }

        /// <summary>Stacks.</summary>
        /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
        /// <param name="m">Input Matrix.</param>
        /// <param name="t">Row or Column sum.</param>
        /// <returns>A Matrix.</returns>
        public static Matrix VStack(Matrix m, Matrix t)
        {
            if (m.Rows != t.Rows)
                throw new InvalidOperationException("Invalid dimension for stack operation!");

            var p = new Matrix(m.Rows, m.Cols + t.Cols);
            for (var i = 0; i < p.Rows; i++)
            for (var j = 0; j < p.Cols; j++)
                if (j < m.Cols)
                    p[i, j] = m[i, j];
                else
                    p[i, j] = t[i, j - m.Cols];

            return p;
        }

        /// <summary>Slices.</summary>
        /// <param name="m">Input Matrix.</param>
        /// <param name="indices">The indices.</param>
        /// <returns>A Matrix.</returns>
        public static Matrix Slice(Matrix m, IEnumerable<int> indices) => m.Slice(indices, VectorType.Row);

        /// <summary>Slices.</summary>
        /// <param name="m">Input Matrix.</param>
        /// <param name="indices">The indices.</param>
        /// <param name="t">Row or Column sum.</param>
        /// <returns>A Matrix.</returns>
        public static Matrix Slice(Matrix m, IEnumerable<int> indices, VectorType t)
        {
            var q = indices.Distinct();

            var rows = t == VectorType.Row ? q.Where(j => j < m.Rows).Count() : m.Rows;
            var cols = t == VectorType.Col ? q.Where(j => j < m.Cols).Count() : m.Cols;

            var n = new Matrix(rows, cols);

            var i = -1;
            foreach (var j in q.OrderBy(k => k))
                n[++i, t] = m[j, t];

            return n;
        }

        /// <summary>
        ///     Unshapes the given Matrix into a Vector form along the <paramref name="dimensionType" /> axis.
        ///     <para>
        ///         Reads from the source Matrix and stacks from right to left when <paramref name="dimensionType" /> equals
        ///         'Col' otherwise uses a bottom up approach.
        ///     </para>
        /// </summary>
        /// <param name="m">The Matrix to act on.</param>
        /// <param name="dimensionType">Type of the dimension to use when unrolling the Matrix.</param>
        /// <returns>Matrix.</returns>
        public static Vector Unshape(Matrix m, VectorType dimensionType = VectorType.Col) =>
            Vector.Combine(dimensionType == VectorType.Col
                ? m.GetCols().ToArray()
                : m.GetRows().ToArray());

        /// <summary>
        ///     Reshapes the supplied Vector into a Matrix form.
        /// </summary>
        /// <param name="v">Source vector to act on.</param>
        /// <param name="dimension">Length of the specified dimension.</param>
        /// <param name="dimensionType">Dimension type to use for creating a <paramref name="dimension" /> by n matrix.</param>
        /// <param name="byVector">Direction to process, i.e. Row = Fill Down then Right, or Col = Fill Right then Down</param>
        /// <returns></returns>
        public static Matrix Reshape(Vector v, int dimension,
            VectorType dimensionType = VectorType.Col, VectorType byVector = VectorType.Row)
        {
            var x = dimensionType == VectorType.Row ? dimension : v.Length / dimension;
            var y = dimensionType == VectorType.Col ? dimension : v.Length / dimension;
            return Reshape(v, x, y, byVector);
        }

        /// <summary>
        ///     Reshapes the supplied Vector into a Matrix form.
        /// </summary>
        /// <param name="v">Vector to reshape.</param>
        /// <param name="rows">Height of the matrix to return</param>
        /// <param name="columns">Width of the matrix to return</param>
        /// <param name="byVector">Direction to process, i.e. Row = Fill Down then Right, or Col = Fill Right then Down</param>
        /// <returns></returns>
        public static Matrix Reshape(Vector v, int rows, int columns,
            VectorType byVector = VectorType.Row)
        {
            if (rows * columns != v.Length)
                throw new InvalidOperationException(
                    string.Format("Cannot reshape Vector of length {0} into a {1} x {2} Matrix.", v.Length, rows,
                        columns));

            var m = new Matrix(rows, columns);

            var counter = 0;

            switch (byVector)
            {
                case VectorType.Row:
                {
                    for (var i = 0; i < columns; i++)
                    for (var k = 0; k < rows; k++)
                        m[k, i] = v[counter++];
                }
                    break;
                case VectorType.Col:
                {
                    for (var i = 0; i < rows; i++)
                    for (var k = 0; k < columns; k++)
                        m[i, k] = v[counter++];
                }
                    break;
            }

            return m;
        }

        /// <summary>
        ///     Reshapes the supplied matrix into a new matrix shape.
        /// </summary>
        /// <param name="m">Matrix to reshape.</param>
        /// <param name="rows">Number of rows of the new matrix.</param>
        /// <param name="cols">Number of columns of the new matrix.</param>
        /// <returns>Matrix.</returns>
        public static Matrix Reshape(Matrix m, int rows, int cols)
        {
            var result = new Matrix(rows, cols);

            var width = rows > m.Rows ? m.Rows : rows;
            var height = cols > m.Cols ? m.Cols : cols;

            for (var y = 0; y < height; y++)
            for (var x = 0; x < width; x++)
                result[y, x] = m[y, x];

            return result;
        }

        /// <summary>Extracts this object.</summary>
        /// <param name="m">Input Matrix.</param>
        /// <param name="x">Matrix x.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="safe">(Optional) true to safe.</param>
        /// <returns>A Matrix.</returns>
        public static Matrix Extract(Matrix m, int x, int y, int width, int height, bool safe = true)
        {
            var m2 = Zeros(height, width);
            for (var i = y; i < y + height; i++)
            for (var j = x; j < x + width; j++)
                if (safe && i < m.Rows && j < m.Cols)
                    m2[i - y, j - x] = m[i, j];

            return m2;
        }

        /// <summary>Diagrams the given m.</summary>
        /// <param name="m">Input Matrix.</param>
        /// <returns>A Vector.</returns>
        public static Vector Diag(Matrix m)
        {
            var length = m.Cols > m.Rows ? m.Rows : m.Cols;
            var v = Vector.Zeros(length);
            for (var i = 0; i < length; i++)
                v[i] = m[i, i];
            return v;
        }

        /// <summary>Statistics.</summary>
        /// <param name="x">Matrix x.</param>
        /// <param name="t">(Optional) Row or Column sum.</param>
        /// <returns>A Matrix[].</returns>
        public static Matrix[] Stats(Matrix x, VectorType t = VectorType.Row)
        {
            var length = t == VectorType.Row ? x.Cols : x.Rows;
            var type = t == VectorType.Row ? VectorType.Col : VectorType.Row;
            var result = new Matrix[length];
            for (var i = 0; i < length; i++)
                result[i] = x[i, type].Stats();
            return result;
        }
    }
}