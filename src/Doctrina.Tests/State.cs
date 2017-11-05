using System.Collections.Generic;
using Doctrina.Math;

namespace Doctrina.Tests
{
    public class State<TState>
    {
        public State(TState features, double value)
        {
            Features = features;
            Value = value;
            TransitionPropabilities = new Distribution<State<TState>>(new Dictionary<State<TState>, Propability>());
        }

        public TState Features { get; }

        public double Value { get; }

        public Reward ImmediateReward { get; set; }

        public Distribution<State<TState>> TransitionPropabilities { get; }
    }
}