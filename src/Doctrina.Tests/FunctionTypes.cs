using Doctrina.Math.LinearAlgebra;

namespace Doctrina.Tests
{
    /// <summary>
    ///     A function that normalizes the value of x so that it falls into a certain interval.
    /// </summary>
    /// <param name="x"></param>
    /// <returns></returns>
    public delegate Vector Normalize(Vector x);

    /// <summary>
    ///     A hypothesis is a model of a function of which we want to train the parameter values, which are the weights
    ///     each feature of a feature vector get assigned in the function the hypothesis represents
    /// </summary>
    /// <param name="features">The feature vector.</param>
    /// <returns></returns>
    public delegate Vector Hypothesis(Vector features);
}