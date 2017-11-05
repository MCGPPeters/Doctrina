namespace Doctrina.Tests
{
    public class GtProperties
    {
        ///// <summary>
        ///// Facts the specified sequence.
        ///// </summary>
        ///// <param name="sequence">The sequence.</param>
        //[Property(DisplayName = "Incremental mean", Arbitrary = new[] { typeof(ArbitraryNonInfinitNorNanDouble) })]
        //public void Fact(int[] sequence)
        //{
        //    var d = sequence.Length > 0d ? sequence.IncrementalMean(1 / ((double)k + 1)) : 0d;
        //    var d1 = (sequence.Length > 0 ? sequence.Average() : 0d);
        //    ;
        //    const double epsilon = .0000000001;
        //    Assert.True(System.Math.Abs(d1 - d) <= epsilon);
        //}


        //[Fact(
        //    DisplayName ="When epsilon is smaller that a random proper positive fraction, the action is chosen with the highest expected return")
        //    /*Arbitrary = new[] {typeof(ArbitraryMarkovRewardProcessSample)}, Verbose = true)*/]
        //public void Property()
        //{
        //    var s1 = new State<double>(-23) { ImmediateReward = -1 };
        //    var s2 = new State<double>(-13) { ImmediateReward = -2 };
        //    var s3 = new State<double>(1.5) { ImmediateReward = -2 };
        //    var s4 = new State<double>(0.8) { ImmediateReward = 1 };
        //    var s5 = new State<double>(4.3) { ImmediateReward = -2 };
        //    var s6 = new State<double>(10) { ImmediateReward = 10 };
        //    var s7 = new State<double>(0) { ImmediateReward = 0};

        //    s1.TransitionPropabilities.Add(s2, 0.1);
        //    s2.TransitionPropabilities.Add(s3, 0.5);
        //    s3.TransitionPropabilities.Add(s5, 0.8);
        //    s3.TransitionPropabilities.Add(s7, 0.2);
        //    s4.TransitionPropabilities.Add(s2, 0.2);
        //    s4.TransitionPropabilities.Add(s3, 0.4);
        //    s4.TransitionPropabilities.Add(s5, 0.4);
        //    s5.TransitionPropabilities.Add(s4, 0.4);
        //    s5.TransitionPropabilities.Add(s6, 0.6);
        //    s6.TransitionPropabilities.Add(s7, 1);

        //    //Assert.Equal(4.3.ToString(), Markov.RewardProcess.StateValue<double>()(1, s5).Value.ToString());
        //    Assert.Equal((1.5).ToString(), Markov.RewardProcess.StateValue<double>()(1, s3).Value.ToString());
        //}

        //internal static class ArbitraryPositiveProperFraction
        //{
        //    public static Arbitrary<ProperFraction> PositiveProperFraction()
        //    {
        //        return Generators.PositiveProperFractionGenerator().ToArbitrary();
        //    }
        //}

        //internal static class ArbitraryNonInfinitNorNanDouble
        //{
        //    public static Arbitrary<double> NonInfinityNorNan()
        //    {
        //        return Generators.NonNanNorInfinity().ToArbitrary();
        //    }
        //}        //internal static class ArbitraryPositiveProperFraction
        //{
        //    public static Arbitrary<ProperFraction> PositiveProperFraction()
        //    {
        //        return Generators.PositiveProperFractionGenerator().ToArbitrary();
        //    }
        //}

        //internal static class ArbitraryNonInfinitNorNanDouble
        //{
        //    public static Arbitrary<double> NonInfinityNorNan()
        //    {
        //        return Generators.NonNanNorInfinity().ToArbitrary();
        //    }
        //}

        //internal static class ArbitraryMarkovRewardProcessSample
        //{
        //    public static Arbitrary<State<int>[]> Sample()
        //    {
        //        var gen =
        //            from sampleLength in Arb.Generate<PositiveInt>()
        //            from integers in Gen.ArrayOf(sampleLength.Get, Arb.Generate<int>())
        //            from reward in Arb.Generate<double>()
        //            select integers.Select(i => new State(i) {ImmediateReward = reward}).ToArray();
        //        return gen.ToArbitrary();
        //    }
        //}

        //public static class Generators
        //{
        //    public static Gen<ProperFraction> PositiveProperFractionGenerator()
        //    {
        //        return Arb
        //            .From<double>()
        //            .Generator
        //            .Where(@double => @double >= 0d && @double <= 1d)
        //            .Select(@double => new ProperFraction(@double));
        //    }

        //    public static Gen<State<int>> IntegerStateGenerator()
        //    {
        //        return Arb
        //            .From<int>()
        //            .Generator
        //            .Select(i => new Transition<int>(i) { Reward = i});
        //    }

        //    public static Gen<double> NonNanNorInfinity()
        //    {
        //        return Arb.Default.Float().Filter(d => !double.IsInfinity(d) && !double.IsNaN(d)).Generator;
        //    }
        //}

        public class EpsilonGreedyProperties
        {
            //[Property(
            //    DisplayName = "When epsilon is smaller that a random proper positive fraction, the action is chosen with the highest expected return",
            //    Arbitrary = new[] {typeof(ArbitraryPositiveProperFraction)}, Verbose = true)]
            //public void Property(PositiveProperFraction epsilon, int state)
            //{
            //    var cryptoRandom = new CryptoRandom();
            //    var positiveProperFraction = cryptoRandom.NextDouble();

            //    ActionValue<int> actionValue = (action, integerState) =>
            //    {
            //        var result = action(integerState);
            //        switch (result)
            //        {
            //            case 1:
            //                return 1;
            //            case 2:
            //                return 2;
            //            case 3:
            //                return 3;
            //            default:
            //                return 0;
            //        }
            //    };

            //    var policy = Policies.EpsilonGreedy(epsilon, actionValue, () => positiveProperFraction, tuple => cryptoRandom.Next(tuple.minimumValue, tuple.maximumValue));

            //    Action<int> action1 = i => Task.FromResult<(int nextState, Reward reward)>((1, 1));
            //    Action<int> action2 = i => Task.FromResult<(int nextState, Reward reward)>((1, 2));
            //    Action<int> action3 = i => Task.FromResult<(int nextState, Reward reward)>((1, 3));

            //    var distribution = policy(state, new[] {action1, action2, action3});

            //    if (epsilon < positiveProperFraction)
            //        Assert.Equal(3, distribution.Single(pair => pair.Value == 1d).Key(state));
            //}
        }
    }
}