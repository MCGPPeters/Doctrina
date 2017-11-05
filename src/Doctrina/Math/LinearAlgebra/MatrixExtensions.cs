// file:	Math\LinearAlgebra\MatrixExtensions.cs
//
// summary:	Implements the matrix extensions class

using System;
using System.Collections.Generic;
using System.Linq;
using numl.Math.LinearAlgebra;

namespace Doctrina.Math.LinearAlgebra
{
    /// <summary>A matrix extensions.</summary>
    public static class MatrixExtensions
    {
        /// <summary>
        ///     Converts a matrix to a binary indicator matrix based on the specified predicate.
        /// </summary>
        /// <param name="m">Matrix.</param>
        /// <param name="fnPredicate">Function to test each Value.</param>
        /// <param name="trueValue">(Optional) Value to set when condition (<paramref name="fnPredicate" />) returns True</param>
        /// <param name="falseValue">(Optional) Value to set when condition (<paramref name="fnPredicate" />) returns False</param>
        /// <returns></returns>
        public static Matrix ToBinary(this Matrix m, Func<double, bool> fnPredicate, double trueValue = 1.0,
            double falseValue = 0.0)
        {
            var result = new Matrix(m.Rows, m.Cols);
            for (var i = 0; i < m.Rows; i++)
            for (var j = 0; j < m.Cols; j++)
                result[i, j] = fnPredicate(m[i, j]) ? trueValue : falseValue;
            return result;
        }

        /// <summary>
        ///     Performs an element-wise operation on the input Matrix.
        /// </summary>
        /// <param name="m">Matrix.</param>
        /// <param name="fnElementWiseOp">Function to apply.</param>
        /// <returns>A Matrix.</returns>
        public static Matrix Each(this Matrix m, Func<double, double> fnElementWiseOp) =>
            Matrix.Each(m, fnElementWiseOp);

        /// <summary>
        ///     Performs an element wise operation on the input Matrix.
        /// </summary>
        /// <param name="m">Matrix.</param>
        /// <param name="fnElementWiseOp">Function to apply to each cell specified by the value and cell coordinates.</param>
        /// <returns>A Matrix.</returns>
        public static Matrix Each(this Matrix m, Func<double, int, int, double> fnElementWiseOp) =>
            Matrix.Each(m, fnElementWiseOp);

        /// <summary>
        ///     Performs an element-wise operation on the input Matrices.
        /// </summary>
        /// <param name="m1">First Matrix.</param>
        /// <param name="m2">Second Matrix.</param>
        /// <param name="fnElementWiseOp">Operation to perform on the value from the first and second matrices.</param>
        /// <returns>A Matrix.</returns>
        public static Matrix Each(this Matrix m1, Matrix m2, Func<double, double, double> fnElementWiseOp) =>
            Matrix.Each(m1, m2, fnElementWiseOp);

        /// <summary>A Matrix extension method that stacks.</summary>
        /// <param name="m">Matrix.</param>
        /// <param name="t">Row or Column sum.</param>
        /// <returns>A Matrix.</returns>
        public static Matrix Stack(this Matrix m, Matrix t) => Matrix.Stack(m, t);

        /// <summary>A Matrix extension method that stacks.</summary>
        /// <param name="m">Matrix.</param>
        /// <param name="t">Row or Column sum.</param>
        /// <returns>A Matrix.</returns>
        public static Matrix VStack(this Matrix m, Matrix t) => Matrix.VStack(m, t);

        /// <summary>
        ///     Unrolls the matrix into an n x 1 vector.
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public static Vector Unshape(this Matrix m) => Matrix.Unshape(m);

        /// <summary>
        ///     Sorts the given Matrix by the specified row or column and returns the new Matrix.
        /// </summary>
        /// <param name="m">The Matrix</param>
        /// <param name="keySelector">Property selector to sort by.</param>
        /// <param name="t">
        ///     Specifies whether to sort horizontally (<see cref="VectorType.Col" />) or vertically (
        ///     <see cref="VectorType.Row" />).
        /// </param>
        /// <param name="isAscending">Determines whether to sort ascending or descending (Default: True)</param>
        /// <returns>New Matrix and Vector of original indices.</returns>
        public static Matrix Sort(this Matrix m, Func<Vector, double> keySelector,
            VectorType t, bool isAscending = true) => Matrix.Sort(m, keySelector, t, isAscending);

        /// <summary>
        ///     Sorts the given Matrix by the specified row or column index and returns the new Matrix
        ///     along with the original indices.
        /// </summary>
        /// <param name="m">The Matrix</param>
        /// <param name="keySelector">Property selector to sort by.</param>
        /// <param name="t">
        ///     Specifies whether to sort horizontally (<see cref="VectorType.Col" />) or vertically (
        ///     <see cref="VectorType.Row" />).
        /// </param>
        /// <param name="isAscending">Determines whether to sort ascending or descending (Default: True)</param>
        /// <param name="indices">Vector of the original (<paramref name="t" />) indices before the sort operation.</param>
        /// <returns>New Matrix and Vector of original indices.</returns>
        public static Matrix Sort(this Matrix m, Func<Vector, double> keySelector,
            VectorType t, bool isAscending, out Vector indices) =>
            Matrix.Sort(m, keySelector, t, isAscending, out indices);

        /// <summary>
        ///     A Matrix extension method that determines the mean of the given parameters.
        /// </summary>
        /// <param name="source">The source to act on.</param>
        /// <param name="t">Row or Column sum.</param>
        /// <returns>The mean value.</returns>
        public static Vector Mean(this Matrix source, VectorType t) =>
            Matrix.Mean(source, t);

        /// <summary>
        ///     Computes the standard deviation of the given matrix
        /// </summary>
        /// <param name="source"></param>
        /// <param name="t">Return a Row or Column vector</param>
        /// <returns></returns>
        public static Vector StdDev(this Matrix source, VectorType t) =>
            Matrix.StdDev(source, t);

        /// <summary>
        ///     Returns a vector of the maximum values for each row/column.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Vector Max(this Matrix source, VectorType t) => Matrix.Max(source, t);

        /// <summary>
        ///     A Matrix extension method that determines the maximum of the given parameters.
        /// </summary>
        /// <param name="source">The source to act on.</param>
        /// <returns>The maximum value.</returns>
        public static double Max(this Matrix source) => Matrix.Max(source);

        /// <summary>
        ///     A Matrix extension method that determines the minimum of the given parameters.
        /// </summary>
        /// <param name="source">The source to act on.</param>
        /// <returns>The minimum value.</returns>
        public static double Min(this Matrix source) => Matrix.Min(source);

        /// <summary>
        ///     Returns a vector of the minimum values for each row/column.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Vector Min(this Matrix source, VectorType t) => Matrix.Min(source, t);

        /// <summary>
        ///     Returns a vector of the median values for each row or column.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Vector Median(this Matrix source, VectorType t) =>
            Matrix.Median(source, t);

        /// <summary>A Matrix extension method that covariances.</summary>
        /// <param name="source">The source to act on.</param>
        /// <param name="t">(Optional) Row or Column sum.</param>
        /// <returns>A Matrix.</returns>
        public static Matrix Covariance(this Matrix source, VectorType t = VectorType.Col) =>
            Matrix.Covariance(source, t);

        /// <summary>A Matrix extension method that covariance diagram.</summary>
        /// <param name="source">The source to act on.</param>
        /// <param name="t">(Optional) Row or Column sum.</param>
        /// <returns>A Vector.</returns>
        public static Vector CovarianceDiag(this Matrix source,
            VectorType t = VectorType.Col) => Matrix.CovarianceDiag(source, t);

        /// <summary>A Matrix extension method that correlations.</summary>
        /// <param name="source">The source to act on.</param>
        /// <param name="t">(Optional) Row or Column sum.</param>
        /// <returns>A Matrix.</returns>
        public static Matrix Correlation(this Matrix source, VectorType t = VectorType.Col) =>
            Matrix.Correlation(source, t);

        /// <summary>Enumerates reverse in this collection.</summary>
        /// <param name="source">The source to act on.</param>
        /// <param name="t">(Optional) Row or Column sum.</param>
        /// <returns>
        ///     An enumerator that allows foreach to be used to process reverse in this collection.
        /// </returns>
        public static IEnumerable<Vector> Reverse(this Matrix source,
            VectorType t = VectorType.Row) => Matrix.Reverse(source, t);

        /// <summary>Enumerates indices in this collection.</summary>
        /// <param name="source">The source to act on.</param>
        /// <param name="f">The Func&lt;Vector,bool&gt; to process.</param>
        /// <returns>
        ///     An enumerator that allows foreach to be used to process indices in this collection.
        /// </returns>
        public static IEnumerable<int> Indices(this Matrix source, Func<Vector, bool> f) =>
            Matrix.Indices(source, f);

        /// <summary>Enumerates indices in this collection.</summary>
        /// <param name="source">The source to act on.</param>
        /// <param name="f">The Func&lt;Vector,bool&gt; to process.</param>
        /// <param name="t">Row or Column sum.</param>
        /// <returns>
        ///     An enumerator that allows foreach to be used to process indices in this collection.
        /// </returns>
        public static IEnumerable<int> Indices(this Matrix source, Func<Vector, bool> f,
            VectorType t) => Matrix.Indices(source, f, t);

        /// <summary>A Matrix extension method that slices.</summary>
        /// <param name="m">Matrix.</param>
        /// <param name="indices">The indices.</param>
        /// <returns>A Matrix.</returns>
        public static Matrix Slice(this Matrix m, IEnumerable<int> indices) => Matrix.Slice(m, indices);

        /// <summary>A Matrix extension method that slices.</summary>
        /// <param name="m">Matrix.</param>
        /// <param name="indices">The indices.</param>
        /// <param name="t">Row or Column sum.</param>
        /// <returns>A Matrix.</returns>
        public static Matrix Slice(this Matrix m, IEnumerable<int> indices, VectorType t) =>
            Matrix.Slice(m, indices, t);

        /// <summary>
        ///     Slices the input matrix using starting and stopping positions.
        /// </summary>
        /// <param name="m">Source matrix.</param>
        /// <param name="minIndex">Minimum index to slice from.</param>
        /// <param name="maxIndex">Maximum index to slice.</param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Matrix Slice(this Matrix m, int minIndex, int maxIndex, VectorType t = VectorType.Row)
        {
            var array = t == VectorType.Row ? m.GetRows() : m.GetCols();
            return array.Skip(minIndex).Take(maxIndex - minIndex + 1).ToMatrix(t);
        }

        /// <summary>
        ///     Slices the Matrix returning only the cells specified by their location.
        /// </summary>
        /// <param name="m">Matrix.</param>
        /// <param name="indices">An 2-D location array of indexes to include.</param>
        /// <param name="t">Type of the first dimension in the location array.</param>
        /// <returns></returns>
        public static Matrix Slice(this Matrix m, IEnumerable<IEnumerable<int>> indices, VectorType t = VectorType.Row)
        {
            var vectors = new List<Vector>();

            var top = t == VectorType.Row ? m.Rows : m.Cols;

            for (var i = 0; i < top; i++)
                vectors.Add(m[i, t].Slice(indices.ElementAt(i)));

            return Matrix.Stack(t, vectors.ToArray());
        }

        /// <summary>A Matrix extension method that extracts this object.</summary>
        /// <param name="m">Matrix.</param>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="safe">(Optional) true to safe.</param>
        /// <returns>A Matrix.</returns>
        public static Matrix Extract(this Matrix m, int x, int y, int width, int height, bool safe = true) =>
            Matrix.Extract(m, x, y, width, height, safe);

        /// <summary>A Matrix extension method that choleskies the given m.</summary>
        /// <param name="m">Matrix.</param>
        /// <returns>A Matrix.</returns>
        public static Matrix Cholesky(this Matrix m) => Matrix.Cholesky(m);

        /// <summary>A Matrix extension method that rounds.</summary>
        /// <param name="m">Matrix.</param>
        /// <param name="decimals">(Optional) the decimals.</param>
        /// <returns>A Matrix.</returns>
        public static Matrix Round(this Matrix m, int decimals = 0) => Matrix.Round(m, decimals);

        /// <summary>A Matrix extension method that normals.</summary>
        /// <param name="m">Matrix.</param>
        /// <returns>A double.</returns>
        public static double Norm(this Matrix m) => Matrix.Norm(m, 2);

        /// <summary>A Matrix extension method that normals.</summary>
        /// <param name="m">Matrix.</param>
        /// <param name="p">The double to process.</param>
        /// <returns>A double.</returns>
        public static double Norm(this Matrix m, double p) => Matrix.Norm(m, p);

        ///// <summary>A Matrix extension method that eigs the given m.</summary>
        ///// <param name="m">Matrix.</param>
        ///// <returns>A Tuple&lt;Vector,Matrix&gt;</returns>
        //public static Tuple<Doctrina.Math.LinearAlgebra.Vector, Matrix> Eigs(this Matrix m) => Matrix.Evd(m);

        /// <summary>A Matrix extension method that svds the given m.</summary>
        /// <param name="m">Matrix.</param>
        /// <returns>A Tuple&lt;Matrix,Vector,Matrix&gt;</returns>
        public static Tuple<Matrix, Vector, Matrix> SVD(this Matrix m) => Matrix.SVD(m);

        /// <summary>Computes the trace of a matrix.</summary>
        /// <param name="m">Matrix.</param>
        /// <returns>trace.</returns>
        public static double Trace(this Matrix m) => Matrix.Trace(m);

        /// <summary>
        ///     Computes the entire sum of every element in the Matrix.
        /// </summary>
        /// <param name="m">Matrix.</param>
        /// <returns>Double.</returns>
        public static double Sum(this Matrix m) => Matrix.Sum(m);

        /// <summary>
        ///     Computes the sum of either the rows or columns of a matrix and returns a vector.
        /// </summary>
        /// <param name="m">Matrix.</param>
        /// <param name="t">Row or Column sum.</param>
        /// <returns>Vector Sum.</returns>
        public static Vector Sum(this Matrix m, VectorType t) => Matrix.Sum(m, t);

        /// <summary>
        ///     Computes the sum of either the rows or columns of a matrix and returns a vector.
        /// </summary>
        /// <param name="m">Matrix.</param>
        /// <param name="i">Zero-based index of the.</param>
        /// <param name="t">Row or Column sum.</param>
        /// <returns>Vector Sum.</returns>
        public static double Sum(Matrix m, int i, VectorType t) => Matrix.Sum(m, i, t);

        /// <summary>A Matrix extension method that diagrams the given m.</summary>
        /// <param name="m">Matrix.</param>
        /// <returns>A Vector.</returns>
        public static Vector Diag(this Matrix m) => Matrix.Diag(m);

        /// <summary>A Matrix extension method that statistics.</summary>
        /// <param name="m">Matrix.</param>
        /// <returns>A Matrix[].</returns>
        public static Matrix[] Stats(this Matrix m) => Matrix.Stats(m, VectorType.Row);

        /// <summary>A Matrix extension method that statistics.</summary>
        /// <param name="m">Matrix.</param>
        /// <param name="t">Row or Column sum.</param>
        /// <returns>A Matrix[].</returns>
        public static Matrix[] Stats(this Matrix m, VectorType t) => Matrix.Stats(m, t);

        /// <summary>
        ///     computes matrix determinant NOTE: currently using cholesky factorization to save time so non
        ///     symmetric positive semi-definite matrices will cause problems...
        /// </summary>
        /// <param name="m">Matrix.</param>
        /// <returns>Determinant.</returns>
        public static double Det(this Matrix m) => Matrix.Det(m);
    }
}