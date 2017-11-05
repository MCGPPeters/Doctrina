using System;
using System.Collections.Generic;
using System.Linq;
using Doctrina.Tests;

namespace ModelFree.Markov
{
    public static class RewardProcess
    {
        /// <summary>
        ///     The value of being in the start state (current state) which is the first state in the sample trajectory.
        /// </summary>
        /// <typeparam name="TState">The type of the state.</typeparam>
        /// <returns>The state value function</returns>
        public static Func<double, State<TState>, Return> StateValue<TState>()
        {
            return (gamma, state) =>
            {
                foreach (var stateTransitionPropability in state.TransitionPropabilities)
                    stateTransitionPropability.Key.ImmediateReward =
                        Return<TState>()(gamma, Enumerable.Repeat(stateTransitionPropability.Key, 1));
                var average = state.TransitionPropabilities.Sum(pair => pair.Value * pair.Key.ImmediateReward);
                return new Return(state.ImmediateReward + gamma * average);
            };
        }

        /// <summary>
        ///     The return (Gt) is the total discounted reward for a sample trajectory within a Markov reward process
        /// </summary>
        /// <typeparam name="TState">The type of the state.</typeparam>
        /// <returns></returns>
        public static Func<double, IEnumerable<State<TState>>, Return> Return<TState>()
        {
            return (gamma, sampleTrajectory) => sampleTrajectory
                .Select((state, k) => (reward: state.ImmediateReward, k: k))
                .Aggregate(Accumulator(gamma)).reward;
        }

        private static Func<(Reward reward, int k), (Reward reward, int k), (Reward reward, int k)> Accumulator(
            double gamma)
        {
            return (state, successorState) =>
                (state.reward + Math.Pow(gamma, state.k) * successorState.reward, state.k);
        }
    }
}