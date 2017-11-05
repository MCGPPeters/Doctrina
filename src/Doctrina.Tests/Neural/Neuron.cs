using System;
using Doctrina.Math.LinearAlgebra;

namespace Doctrina.Tests.Neural
{
    /// <summary>
    ///     An activation is a specialization of a normalization where the output is going to be normalized in the interval (0,
    ///     1), e.g. 0 &gt; x &lt; 1
    /// </summary>
    /// <param name="x">The x.</param>
    /// <returns></returns>
    public delegate Vector Activation(Vector x);

    /// <summary>
    ///     Create a new neuron based on a parameter vector
    /// </summary>
    /// <param name="parameterVector">The parameter vector.</param>
    /// <returns></returns>
    public delegate Neuron CreateNeuron(Vector parameters);

    /// <summary>
    ///     A neuron is a specialisation of a hypothesis where the normalization is an activation
    /// </summary>
    /// <param name="featureVector">
    ///     The feature vector. The feature vector always includes a bias feature which has the value
    ///     1.0 as feature 0
    /// </param>
    /// <returns></returns>
    public delegate Vector Neuron(Vector features);

    public static class NeuronRepresentation
    {
        public static Func<Activation, CreateNeuron> Create => activation =>
            weights => features =>
                activation(new[]{(weights * features).Sum()});
    }
}