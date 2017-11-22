using Doctrina.Functions;
using Doctrina.Math.LinearAlgebra;
using Doctrina.Tests.Neural;
using Xunit;

namespace Doctrina.Tests
{
    public class NetworkTests
    {
        [Fact(DisplayName = "Network can emulate the XNOR")]
        public void Xnor()
        {
            const double bias = 1d;
            var createNeuron = NeuronRepresentation.Create(Logistic.Sigmoid);

            Vector andWeights = new[] {-30d, 20d, 20d };
            var andNeuron = createNeuron(andWeights);

            Vector nanWeights = new[] { 10d, -20d, -20d };
            var nanNeuron = createNeuron(nanWeights);

            Vector orWeights = new[] { -10d, 20d, 20d };
            var orNeuron = createNeuron(orWeights);

            Vector sample1 = new[] { bias, 0d, 0d };
            Vector sample2 = new[] { bias, 0d, 1d };
            Vector sample3 = new[] { bias, 1d, 0d };
            Vector sample4 = new[] { bias, 0d, 0d };

            var andNeuronOutput = andNeuron(sample1);
            var nanNeuronOutput = nanNeuron(sample1);

            Vector hiddenLayerOutput = new[] {bias, andNeuronOutput.ToDouble(), nanNeuronOutput.ToDouble() };
            var orNeuronOutput = orNeuron(hiddenLayerOutput);

            Assert.True(new[] { 1d }.ToVector() == orNeuronOutput);

            andNeuronOutput = andNeuron(sample2);
            nanNeuronOutput = nanNeuron(sample2);
            hiddenLayerOutput = new[] { bias, andNeuronOutput.ToDouble(), nanNeuronOutput.ToDouble() };
            orNeuronOutput = orNeuron(hiddenLayerOutput);

            Assert.True(new[] { 0d }.ToVector() == orNeuronOutput);

            andNeuronOutput = andNeuron(sample3);
            nanNeuronOutput = nanNeuron(sample3);
            hiddenLayerOutput = new[] { bias, andNeuronOutput.ToDouble(), nanNeuronOutput.ToDouble() };
            orNeuronOutput = orNeuron(hiddenLayerOutput);

            Assert.True(new[] { 0d }.ToVector() == orNeuronOutput);

            andNeuronOutput = andNeuron(sample4);
            nanNeuronOutput = nanNeuron(sample4);
            hiddenLayerOutput = new[] { bias, andNeuronOutput.ToDouble(), nanNeuronOutput.ToDouble() };
            orNeuronOutput = orNeuron(hiddenLayerOutput);

            Assert.True(new[] { 1d }.ToVector() == orNeuronOutput);
        }
    }

    public class BackpropagationTests
    {
        [Fact(DisplayName = "Network can emulate the XNOR")]
        public void Xnor()
        {

        }
    }
}