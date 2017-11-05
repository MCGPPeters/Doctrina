using System;
using System.Collections.Generic;
using System.Linq;
using Doctrina.Math;
using Operations = Doctrina.Tests.Math.Operations;

namespace Doctrina.Tests.ReinforcementLearning
{
    public static class Policy
    {
        /// <summary>
        ///     The epsilon-greedy policy, a value based implicit policy (makes descision based on value function),
        ///     either takes a random action with probability epsilon, or it takes the action for the highest
        ///     Q-value.
        ///     If epsilon is 1.0 then the actions are always random.
        ///     If epsilon is 0.0 then the actions are always argmax for the Q-values.
        /// </summary>
        /// <typeparam name="TState"></typeparam>
        /// <param name="epsilon"></param>
        /// <param name="nextRandomDouble">A function that should provide a random integer</param>
        /// <param name="nextRandomIntegerInRange">A function that should provide a unsigned random integer given a specific range</param>
        /// <returns></returns>
        public static Policy<TState> EpsilonGreedy<TState>(Epsilon epsilon, Func<Propability> nextRandomDouble,
            Func<(int minimumValue, int maximumValue), int> nextRandomIntegerInRange) =>
            actionValues =>
            {
                return state =>
                {
                    var actionDistribution = new Dictionary<Action, Propability>();

                    if (nextRandomDouble() < epsilon)
                    {
                        var randomIndex = nextRandomIntegerInRange((0, actionValues.Count - 1));
                        actionDistribution.Add(actionValues.ElementAt(randomIndex).Action, 1.0d);
                    }
                    else
                    {
                        var expectedMaximumReturn =
                            Operations.ArgMax(actionValues.Select(value => value.Return).ToArray());
                        var actionWithMaximumExpectedValue =
                            actionValues.Single(value => value.Return == expectedMaximumReturn);
                        actionDistribution.Add(actionWithMaximumExpectedValue.Action, 1.0d);
                    }

                    return new Distribution<Action>(actionDistribution);
                };
            };

        /// <summary>
        ///     The greedy policy, a value based implicit policy (makes descision based on value function),
        ///     takes the action with the highest expected Q-value.
        /// </summary>
        /// <typeparam name="TState"></typeparam>
        /// <returns></returns>
        public static Policy<TState> Greedy<TState>() =>
            actionValues =>
            {
                return state =>
                {
                    var actionDistribution = new Dictionary<Action, Propability>();

                    var expectedMaximumReturn = Operations.ArgMax(actionValues.Select(value => value.Return).ToArray());
                    var actionWithMaximumExpectedValue =
                        actionValues.Single(value => value.Return == expectedMaximumReturn);
                    actionDistribution.Add(actionWithMaximumExpectedValue.Action, 1.0d);

                    return new Distribution<Action>(actionDistribution);
                };
            };
    }
}