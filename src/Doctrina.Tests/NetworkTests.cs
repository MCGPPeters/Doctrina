using System.Collections.Generic;
using System.Linq;
using Doctrina.Functions;
using Doctrina.Math;
using Doctrina.Math.LinearAlgebra;
using Doctrina.Tests.Neural;
using Xunit;
using Activation = Doctrina.Functions.Activation;

namespace Doctrina.Tests
{
    public class NetworkTests
    {
        [Fact(DisplayName = "Network can emulate the XNOR")]
        public void Xnor()
        {
            var createNeuron = NeuronRepresentation.Create(Activation.Sigmoid);

            Vector andWeights = new[] { -30d, 20d, 20d };
            var andNeuron = createNeuron(andWeights);

            Vector nanWeights = new[] { 10d, -20d, -20d };
            var nanNeuron = createNeuron(nanWeights);

            Vector orWeights = new[] { -10d, 20d, 20d };
            var orNeuron = createNeuron(orWeights);

            Vector sample1 = new[] { Defaults.Bias, 0d, 0d };
            Vector sample2 = new[] { Defaults.Bias, 0d, 1d };
            Vector sample3 = new[] { Defaults.Bias, 1d, 0d };
            Vector sample4 = new[] { Defaults.Bias, 0d, 0d };

            var andNeuronOutput = andNeuron(sample1);
            var nanNeuronOutput = nanNeuron(sample1);

            Vector hiddenLayerOutput = new[] { Defaults.Bias, andNeuronOutput, nanNeuronOutput };
            var orNeuronOutput = orNeuron(hiddenLayerOutput);

            Assert.True(System.Math.Abs(1d - orNeuronOutput) < Defaults.Epsilon);

            andNeuronOutput = andNeuron(sample2);
            nanNeuronOutput = nanNeuron(sample2);
            hiddenLayerOutput = new[] { Defaults.Bias, andNeuronOutput, nanNeuronOutput };
            orNeuronOutput = orNeuron(hiddenLayerOutput);

            Assert.True( System.Math.Abs(orNeuronOutput) < Defaults.Epsilon);

            andNeuronOutput = andNeuron(sample3);
            nanNeuronOutput = nanNeuron(sample3);
            hiddenLayerOutput = new[] { Defaults.Bias, andNeuronOutput, nanNeuronOutput };
            orNeuronOutput = orNeuron(hiddenLayerOutput);

            Assert.True(System.Math.Abs(orNeuronOutput) < Defaults.Epsilon);

            andNeuronOutput = andNeuron(sample4);
            nanNeuronOutput = nanNeuron(sample4);
            hiddenLayerOutput = new[] { Defaults.Bias, andNeuronOutput, nanNeuronOutput };
            orNeuronOutput = orNeuron(hiddenLayerOutput);

            Assert.True(System.Math.Abs(1d - orNeuronOutput) < Defaults.Epsilon);
        }
    }

    public class BackpropagationTests
    {
        /// <summary>
        /// Ands this instance.
        /// </summary>
        [Fact(DisplayName = "Network can emulate the AND after training")]
        public void And()
        {
            Lambda learningRate = 0.01;

            //random initialization of weights
            Vector weightsOfFirstLayer = new[] { Sampling.GetUniform(), Sampling.GetUniform(), Sampling.GetUniform(), Sampling.GetUniform(), Sampling.GetUniform(), Sampling.GetUniform() };
            Vector weightsOfSecondLayer = new[] { Sampling.GetUniform(), Sampling.GetUniform(), Sampling.GetUniform(), Sampling.GetUniform(), Sampling.GetUniform(), Sampling.GetUniform() };
            Vector weightsOfOutputLayer = new[] { Sampling.GetUniform(), Sampling.GetUniform() };

            var sample1 = new Sample(new[] { Defaults.Bias, 0d, 0d }, new[] { 0d });
            var sample2 = new Sample(new[] { Defaults.Bias, 0d, 1d }, new[] { 0d });
            var sample3 = new Sample(new[] { Defaults.Bias, 1d, 0d }, new[] { 0d });
            var sample4 = new Sample(new[] { Defaults.Bias, 1d, 1d }, new[] { 1d });

            var samples = new List<Sample> { sample1, sample2, sample3, sample4 };

            var firstHiddenLayer = Layer.Create(5, Activation.RectifiedLinear);
            var secondHiddenLayer = Layer.Create(5, Activation.RectifiedLinear);
            var outputLayer = Layer.Create(1, Activation.SoftMax);

            /// Backpropagation





            //train
            for (var i = 0; i < 100; i++)
            {
                // (1) take a random sample from the 4 samples
                var index = Sampling.GetUniform(0, 3);
                var input = samples[index].Input;
                var expectedOutput = samples[index].Expected;

                /// (1) Forward pass, calculate the output
                var outputOffirstHiddenLayer = firstHiddenLayer(input, weightsOfFirstLayer);
                var outputOfSecondHiddenLayer = secondHiddenLayer(outputOffirstHiddenLayer, weightsOfSecondLayer);
                var output = outputLayer(outputOfSecondHiddenLayer, weightsOfOutputLayer);

                /// (2) Error back propagation (backward pass)
                var outputError = expectedOutput - output; // softmax using cross entropy loss cancelles out the need for the derivative



                /// (3) Update weights using an optimizer like Gradient Descent etc...
            }





        }
    }

    public class Sample
    {
        public Vector Input { get; }
        public Vector Expected { get; }

        public Sample(Vector input, Vector expected)
        {
            Input = input;
            Expected = expected;
        }
    }
}