using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

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

    /// <summary>
    /// A policy π is a distribution over actions given a <typeparam name="TState">state</typeparam>. I.e.
    /// the propability that the agent will take action A given being in a state S)
    /// </summary>
    /// <typeparam name="TState"></typeparam>
    /// <typeparam name="TReward"></typeparam>
    /// <param name="state"></param>
    /// <returns></returns>
    public delegate Distribution<Action<TState, TReward>> Policy<TState, TReward>(TState state, Action<TState, TReward>[] actionSpace);

    public class Distribution<T> : ReadOnlyDictionary<T, float>
    {
        public Distribution(IDictionary<T, float> dictionary) : base(dictionary)
        {
            if(Math.Abs(dictionary.Values.Sum()) > 0f) throw new ArgumentException("The sum of all probabilities does not add up to 1");
        }
    }

    public delegate (TState, TReward) Action<TState, TReward>(TState state);

    public static class Policies
    {
        public static Policy<TState, TReward> EpsilonGreedy<TState, TReward>(float epsilon) => (state, actionSpace) =>
        {
            
        } 
    }
}
