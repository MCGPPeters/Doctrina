using Doctrina.Functions;
using Doctrina.Math.LinearAlgebra;
using Doctrina.Tests.Neural;
using FsCheck.Xunit;
using Xunit;

namespace Doctrina.Tests
{
    public class NeuronTests
    {
        [Fact(DisplayName = "Neuron can emulate the AND operator")]
        public void And()
        {
            var createNeuron = NeuronRepresentation.Create(Logistic.Sigmoid);
            Vector weights = new[] {-30d, 20d, 20d};
            var andNeuron = createNeuron(weights);

            const double bias = 1d;
            Vector features = new[] {bias, 1d, 0d};
            Vector result = andNeuron(features);

            Assert.True(new[] {0d}.ToVector() == result);

            features = new[] { bias, 0d, 1d };
            result = andNeuron(features);

            Assert.True(new[] { 0d }.ToVector() == result);

            features = new[] { bias, 0d, 0d };
            result = andNeuron(features);

            Assert.True(new[] { 0d }.ToVector() == result);

            features = new[] { bias, 1d, 1d };
            result = andNeuron(features);

            Assert.True(new[] { 1d }.ToVector() == result);

        }

        [Fact(DisplayName = "Neuron can emulate the OR operator")]
        public void Or()
        {
            var createNeuron = NeuronRepresentation.Create(Logistic.Sigmoid);
            Vector weights = new[] {-10d, 20d, 20d};
            var orNeuron = createNeuron(weights);

            const double bias = 1d;
            Vector features = new[] {bias, 1d, 1d};
            Vector result = orNeuron(features);

            Assert.True(new[] {1d}.ToVector() == result);

            features = new[] {bias, 1d, 0d};
            result = orNeuron(features);

            Assert.True(new[] {1d}.ToVector() == result);

            features = new[] {bias, 0d, 1d};
            result = orNeuron(features);

            Assert.True(new[] {1d}.ToVector() == result);

            features = new[] {bias, 0d, 0d};
            result = orNeuron(features);

            Assert.True(new[] {0d}.ToVector() == result);
        }

        [Fact(DisplayName = "Neuron can emulate the NOT operator")]
        public void Not()
        {
            var createNeuron = NeuronRepresentation.Create(Logistic.Sigmoid);
            Vector weights = new[] { 10d, -20d };
            var neuron = createNeuron(weights);

            const double bias = 1d;
            Vector features = new[] { bias, 1d };
            Vector result = neuron(features);

            Assert.True(new[] { 0d }.ToVector() == result);

            features = new[] { bias, 0d };
            result = neuron(features);

            Assert.True(new[] { 1d }.ToVector() == result);
        }

        [Fact(DisplayName = "Neuron can emulate the (NOT x1) AND (NOT x2)")]
        public void NotAndNot()
        {
            var createNeuron = NeuronRepresentation.Create(Logistic.Sigmoid);
            Vector weights = new[] { 10d, -20d, -20d };
            var nanNeuron = createNeuron(weights);

            const double bias = 1d;

            Vector sample = new[] { bias, 1d, 1d };
            Vector result = nanNeuron(sample);

            Assert.True(new[] { 0d }.ToVector() == result);

            sample = new[] { bias, 1d, 0d };
            result = nanNeuron(sample);

            Assert.True(new[] { 0d }.ToVector() == result);

            sample = new[] { bias, 0d, 1d };
            result = nanNeuron(sample);

            Assert.True(new[] { 0d }.ToVector() == result);

            sample = new[] { bias, 0d, 0d };
            result = nanNeuron(sample);

            Assert.True(new[] { 1d }.ToVector() == result);
        }
    }
}