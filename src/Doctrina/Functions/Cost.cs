using Doctrina.Math.LinearAlgebra;

namespace Doctrina.Functions
{
    public delegate Vector CostFunction(Vector theta);

    public static class Cost
    {
        //public static double Linear(Vector theta)
        //{
        //    int m = X.Rows;

        //    double j = 0.0;

        //    Vector s = (X * theta).ToVector();

        //    j = 1.0 / (2.0 * m) * ((s - Y) ^ 2.0).Sum();

        //    if (Lambda != 0)
        //    {
        //        j = Regularizer.Regularize(j, theta, m, Lambda);
        //    }

        //    return j;
        //}
    }
}