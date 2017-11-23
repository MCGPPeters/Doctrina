using System;
using System.Linq;
using Doctrina.Math.LinearAlgebra;

namespace Doctrina.Tests.Neural
{
    public static class Layer
    {

        /// <summary>
        ///     An activation is a specialization of a normalization where the output is going to be normalized in the interval (0,
        ///     1), e.g. 0 &gt; x &lt; 1
        /// </summary>
        /// <param name="x">The x.</param>
        /// <returns></returns>
        public delegate Vector Activation(Vector x);

        public delegate double Cost(Vector y1, Vector y2);

        public delegate Vector Model(Vector inputs, Vector weights);

        public static Model Create(int numberOfNeurons, Activation activation) =>
            (inputs, weights) =>
            {
                var weightMatrix = new Matrix(numberOfNeurons, weights.Length);
                return activation((weightMatrix * inputs.ToMatrix().Transpose()).ToVector());
            };

    }
}
