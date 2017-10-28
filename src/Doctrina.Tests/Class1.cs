using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Doctrina.Tests;
using Math = Doctrina.Tests.Math;
using Propability = Doctrina.Tests.PositiveProperFraction;

namespace Doctrina.Tests
{
    public static class EnumerableExtensions
    {
        private static IEnumerable<double> LinearlySpaced(double start, double end, int partitions) =>
            Enumerable.Range(0, partitions + 1)
                .Select(idx => idx != partitions
                    ? start + (end - start) / partitions * idx
                    : end);

        /// <summary>
        /// The calculation of an incremental mean work by correcting the 'error' between what we thought the mean
        /// 
        /// </summary>
        /// <param name="sequence">The sequence.</param>
        /// <returns></returns>
        public static double IncrementalMean(this IEnumerable<int> sequence)
        {
            var enumerable = sequence as int[] ?? sequence.ToArray();
            if (enumerable.Length <= 0) return 0;
            double mean = enumerable[0];
            
            for (var k = 1; k < enumerable.Length; k++)
            {
                double count = k + 1;
                var alpha = 1 / count;
                var target = enumerable[k];
                mean = mean.Adjust(alpha, target);
            }
            return mean;
        }

        public static IEnumerable<T> Closure<T>(
            T root,
            Func<T, IEnumerable<T>> children)
        {
            var seen = new HashSet<T>();
            var stack = new Stack<T>();
            stack.Push(root);

            while (stack.Count != 0)
            {
                var item = stack.Pop();
                if (seen.Contains(item))
                    continue;
                seen.Add(item);
                yield return item;
                foreach (var child in children(item))
                    stack.Push(child);
            }
        }
    }

    /// <summary>
    /// A simple example for Reinforcement Learning using table lookup Q-learning method.
    /// An agent "o" is on the left of a 1 dimensional world, the treasure is on the rightmost location.
    /// Run this program and to see how the agent will improve its strategy of finding the treasure.
    /// </summary>
    public class TreaseHuntProperties
    {

    }

    /// The agent state, is the agents internal representation of the state of the environment based on the observations
    /// it received from that environment, via its sensory system (the perceived state of the world). This is the state that
    /// is used to pick the next action it will take using its actuator and with that influencing the environment (external) state
    /// The agent state is represented by the type parameter TState

    /// <summary>
    /// A policy π is a distribution over actions given a <typeparam name="TState">state</typeparam>. I.e.
    /// the propability that the agent will take action A given being in a state S). 
    /// 
    /// In the case of a deterministic policy, there will allways be 1 action that will be chosen 
    /// i.e. the distribution will contain 1 action that has value 1 and the rest of the actions will get 0 propability
    /// 
    /// In a stocastic policy, the will be a true distribution over actions. Since a deterministic policy is a special
    /// case of a stocastic policy, the policy function is defined as such
    /// </summary>
    /// <typeparam name="TState"></typeparam>
    /// <param name="state"></param>
    /// <returns></returns>
    public delegate Distribution<Action<TState>> Policy<TState>(TState state, Action<TState>[] ioSpace);

    public class Distribution<T> : Dictionary<T, Propability>
    {
        public Distribution(IDictionary<T, Propability> dictionary) : base(dictionary)
        {
            if(System.Math.Abs(dictionary.Values.Select(propability => propability.Value).Sum()) > 1f) throw new ArgumentException("The sum of all probabilities does not add up to 1");
        }
    }

    public delegate IObservable<(TState nextState, Reward reward)> Environment<TState>(
        IObservable<Action<TState>> actions);

    public delegate Task<(TState nextState, Reward reward)> Action<TState>(TState state);

    /// <summary>
    /// Determines the expected / estimated return of taking <paramref name="action"></paramref> in <paramref name="state"></paramref> 
    /// </summary>
    /// <typeparam name="TState"></typeparam>
    /// <param name="action"></param>
    /// <param name="state">An n-tuple (object) containing all the features describing the agent state.</param>
    /// <returns></returns>
    public delegate Return ActionValue<TState>(Action<TState> action, TState state);
    
    /// <summary>
    /// Determines the expected / estimated return of being in <paramref name="state"></paramref> looking forward into the expected future
    /// </summary>
    /// <typeparam name="TState"></typeparam>
    /// <param name="state">An n-tuple (object) containing all the features describing the agent state.</param>
    /// <returns></returns>
    public delegate Return StateValue<in TState>(TState state);

    public delegate double Reward<in TState>(TState state);

    public class State<TState>
    {
        public TState Features { get; }

        public State(TState features, double value)
        {
            Features = features;
            Value = value;
            TransitionPropabilities = new Distribution<State<TState>>(new Dictionary<State<TState>, PositiveProperFraction>());
        }

        public double Value { get; private set; }

        public Reward ImmediateReward { get; set; }

        public Distribution<State<TState>> TransitionPropabilities { get; }

    }

    public static class Policies
    {
        
        /// <summary>
        /// The epsilon-greedy policy, a value based implicit policy (makes descision based on value function),
        /// either takes a random action with probability epsilon, or it takes the action for the highest
        /// Q-value.
        ///    If epsilon is 1.0 then the actions are always random.
        ///    If epsilon is 0.0 then the actions are always argmax for the Q-values.
        /// </summary>
        /// <typeparam name="TState"></typeparam>
        /// <param name="epsilon"></param>
        /// <param name="actionValue"></param>
        /// <param name="nextRandomDouble">A function that should provide a random integer</param>
        /// <param name="nextRandomIntegerInRange">A function that should provide a unsigned random integer given a specific range</param>
        /// <returns></returns>
        public static Policy<TState> EpsilonGreedy<TState>(PositiveProperFraction epsilon, ActionValue<TState> actionValue, Func<PositiveProperFraction> nextRandomDouble, Func<(int minimumValue, int maximumValue), int> nextRandomIntegerInRange) =>
            (state, actionSpace) =>
            {
                var actionValues = actionSpace.ToDictionary(action => action, action => actionValue(action, state));
                var actionDistribution = new Dictionary<Action<TState>, Propability>();

                if (nextRandomDouble() < epsilon)
                {
                    var randomIndex = nextRandomIntegerInRange((0, actionSpace.Length - 1));
                    actionDistribution.Add(actionValues.ElementAt(randomIndex).Key, 1.0d);
                }
                else
                {
                    var expectedMaximumReturn = actionValues.Values.Select(@return => @return.Value).ToArray().ArgMax();
                    var actionWithMaximumExpectedValue =
                        actionValues.Single(pair => pair.Value == expectedMaximumReturn).Key;
                    actionDistribution.Add(actionWithMaximumExpectedValue, 1.0d);
                }
                
                return new Distribution<Action<TState>>(actionDistribution);
            };
    }
}

namespace ModelFree
{
    using static Math;

    public static class MonteCarlo
    {
        /// <summary>
        /// Values the specified alpha.
        /// </summary>
        /// <typeparam name="TState">The type of the state.</typeparam>
        /// <param name="state">The state.</param>
        /// <param name="alpha">The learning rate</param>
        /// <param name="target">The actual return</param>
        /// <returns></returns>
        private static double Value<TState>(this State<TState> state, PositiveProperFraction alpha, Return target)
            => state.Value.Adjust(alpha, target);
    }

    public static class TemporalDifference
    {
        /// <summary>
        /// Gets the estimated value of the current state (how good is it to be in this state?)
        /// </summary>
        /// <typeparam name="TState">The type of the state.</typeparam>
        /// <param name="state">The state.</param>
        /// <param name="successor">The successor state</param>
        /// <param name="alpha">The learning rate</param>
        /// <param name="gamma">The discount value</param>
        /// <returns></returns>
        private static Return Value<TState>(this State<TState> state, State<TState> successor, PositiveProperFraction alpha, PositiveProperFraction gamma)
        {
            //𝛿
            var tdTarget = state.ImmediateReward + gamma * successor.Value;
            return state.Value.Adjust(alpha, tdTarget);
        }

        public static StateValue<TState> Evaluate<TState>(TState currentState, PositiveProperFraction alpha, PositiveProperFraction gamma, StateValue<TState> current)
        {
            Reward immediateReward = 0;
            return valueOfNextState =>
            {
                var @return = current(currentState);
                var tdTarget = immediateReward + gamma * current(valueOfNextState); //estimated return
                var tdError = tdTarget - @return;
                return @return + alpha * tdError;
            };
        }
    }
}

namespace Markov
{
    public static class RewardProcess
    {
        /// <summary>
        /// The value of being in the start state (current state) which is the first state in the sample trajectory.
        /// </summary>
        /// <typeparam name="TState">The type of the state.</typeparam>
        /// <returns>The state value function</returns>
        public static Func<double, State<TState>, Return> StateValue<TState>()
        {
            return (gamma, state) =>
            {
                foreach (var stateTransitionPropability in state.TransitionPropabilities)
                {
                    stateTransitionPropability.Key.ImmediateReward =
                        Return<TState>()(gamma, Enumerable.Repeat(stateTransitionPropability.Key, 1));
                }
                var average = state.TransitionPropabilities.Sum(pair => pair.Value * pair.Key.ImmediateReward);
                return new Return(state.ImmediateReward + gamma * average);
            };
        }

        /// <summary>
        /// The return (Gt) is the total discounted reward for a sample trajectory within a Markov reward process
        /// </summary>
        /// <typeparam name="TState">The type of the state.</typeparam>
        /// <returns></returns>
        public static Func<double, IEnumerable<State<TState>>, Return> Return<TState>()
        {
            return (gamma, sampleTrajectory) => sampleTrajectory.Select((state, k) => (reward: state.ImmediateReward, k: k))
                .Aggregate(Func<TState>(gamma)).reward;
        }

        private static Func<(Reward reward, int k), (Reward reward, int k), (Reward reward, int k)> Func<TState>(double gamma)
        {
            return (state, successorState) =>
                (state.reward + System.Math.Pow(gamma, state.k) * successorState.reward, state.k);
        }
    }

    public static class DecisionProcess
    {
    
    }
}