using Doctrina.Functions;
using Doctrina.Math.LinearAlgebra;
using Doctrina.Tests.Neural;
using FsCheck.Xunit;
using Xunit;

namespace Doctrina.Tests
{
    public class NeuronProperties
    {
        [Property(DisplayName = "Neuron can emulate the AND operator")]
        public void Property()
        {
            var createNeuron = NeuronRepresentation.Create(Logistic.Sigmoid);
            Vector weights = new[] {-30d, 20d, 20d};
            var neuron = createNeuron(weights);

            const double bias = 1d;
            Vector features = new[] {bias, 1d, 0d};
            var result = neuron(features);

            Assert.Equal(new[] {0d}.ToVector(), result);
        }

        [Property(DisplayName = "Neuron can emulate the OR operator")]
        public void Property2()
        {
        }

        [Property(DisplayName = "Neuron can emulate the NOT operator")]
        public void Property3()
        {
        }
    }
}