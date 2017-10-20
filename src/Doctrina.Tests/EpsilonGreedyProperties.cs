using System.Linq;
using FsCheck;
using FsCheck.Xunit;
using Xunit;


namespace Doctrina.Tests
{
    public class EpsilonGreedyProperties
    {
        internal static class ArbitraryPositiveProperFraction
        {
            public static Arbitrary<PositiveProperFraction> PositiveProperFraction()
            {
                return Generators.PositiveProperFractionGenerator().ToArbitrary();
            }
        }

        public static class Generators
        {
            public static Gen<PositiveProperFraction> PositiveProperFractionGenerator()
            {
                return Arb
                    .From<double>()
                    .Generator
                    .Where(@double => @double >= 0d && @double <= 1d)
                    .Select(@double => new PositiveProperFraction(@double));
            }
        }

        [Property(
            DisplayName = "When epsilon is smaller that a random proper positive fraction, the action is chosen with the highest expected return",
            Arbitrary = new[] {typeof(ArbitraryPositiveProperFraction)
        })]
        public void Property(PositiveProperFraction epsilon, int state)
        {
            var cryptoRandom = new CryptoRandom();
            var positiveProperFraction = cryptoRandom.NextDouble();
            ActionValue<int> actionValue = (action, integerState) => new Return(action(integerState).attainedReward);
            var policy = Policies.EpsilonGreedy(epsilon, actionValue, () => positiveProperFraction, tuple => cryptoRandom.Next(tuple.minimumValue, tuple.maximumValue));

            Action<int> action1 = i => (i, 1);
            Action<int> action2 = i => (i, 2);
            Action<int> action3 = i => (i, 3);

            var distribution = policy(state, new[] {action1, action2, action3});

            if (epsilon < positiveProperFraction)
                Assert.Equal(3,(int)distribution.Single(pair => pair.Value == 1d).Key(state).attainedReward);
        }
    }
}
