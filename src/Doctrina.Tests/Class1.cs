using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using Doctrina.Tests;
using Propability = Doctrina.Tests.PositiveProperFraction;

namespace Doctrina.Tests
{
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

    public class Distribution<T> : ReadOnlyDictionary<T, Propability>
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

        public State(TState features)
        {
            Features = features;
        }

        public Reward ImmediateReward { get; }



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

    public static class TemporalDifference
    {
        public static StateValue<TState> Predict<TState>(TState currentState, PositiveProperFraction alpha, PositiveProperFraction gamma, StateValue<TState> current)
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
        /// The return is the total discounted reward for a sample trajectory within a Markov reward process
        /// </summary>
        /// <typeparam name="TState">The type of the state.</typeparam>
        /// <returns></returns>
        public static Func<double, IEnumerable<State<TState>>, Return> Return<TState>()
        {
            return (gamma, sampleTrajectory) => new Return(sampleTrajectory.Select(state => state.ImmediateReward)
                .Aggregate((immediateReward, rewardAfterSuccessorState) =>
                    immediateReward + gamma * rewardAfterSuccessorState));
        }
    }
}