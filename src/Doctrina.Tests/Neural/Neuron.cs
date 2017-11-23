using System;
using System.Linq;
using Doctrina.Math.LinearAlgebra;

namespace Doctrina.Tests.Neural
{
    public static class NeuronRepresentation
    {
        /// <summary>
        ///     An activation is a specialization of a normalization where the output is going to be normalized in the interval (0,
        ///     1), e.g. 0 &gt; x &lt; 1
        /// </summary>
        /// <param name="x">The x.</param>
        /// <returns></returns>
        public delegate double Activation(double x);

        /// <summary>
        ///     Create a new neuron based on a parameter vector
        /// </summary>
        /// <param name="parameters">The parameter vector.</param>
        /// <returns></returns>
        public delegate Neuron CreateNeuron(Vector parameters);

        /// <summary>
        ///     A neuron is a specialisation of a hypothesis where the normalization is an activation
        /// </summary>
        /// <returns></returns>
        public delegate double Neuron(Vector inputs);

        public static Func<Activation, CreateNeuron> Create => activation =>
            weights => features =>
                activation(weights.Zip(features, (weight, feature) => weight * feature).Sum());
    }
}