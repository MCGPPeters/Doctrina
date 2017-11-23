using Doctrina.Math;
using Doctrina.Math.LinearAlgebra;

namespace Doctrina.Functions
{
    public delegate Vector CostFunction(Vector theta);

    public static class Cost
    {
        public static double CrossEntropy(Vector y1, Vector y2)
        {
            return -(y2 * Vector.Log(y1 + Defaults.Epsilon)).Sum();
        }
    }
}