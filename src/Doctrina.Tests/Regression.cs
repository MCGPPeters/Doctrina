using System;
using Doctrina.Math.LinearAlgebra;

namespace Doctrina.Tests
{
    public static partial class Regression
    {
        public static Func<Normalize, Func<Vector, Hypothesis>> CreateHypothesis => normalize =>
            theta => features =>
                (theta.ToMatrix().Transpose() * features).ToVector();
    }
}