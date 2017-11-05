using Doctrina.Math;
using Doctrina.Tests;
using Operations = Doctrina.Tests.Math.Operations;

namespace ModelFree.Control.Evaluation
{
    public static class TemporalDifference
    {
        /// <summary>
        ///     Gets the estimated value of the current state (how good is it to be in this state?)
        /// </summary>
        /// <typeparam name="TState">The type of the state.</typeparam>
        /// <param name="state">The state.</param>
        /// <param name="successor">The successor state</param>
        /// <param name="learningRate">The learning rate</param>
        /// <param name="discountValue">The discount value</param>
        /// <returns></returns>
        private static Return Value<TState>(this State<TState> state, State<TState> successor,
            Alpha learningRate, Gamma discountValue)
        {
            //𝛿
            var tdTarget = state.ImmediateReward + discountValue * successor.Value;
            return Operations.Update(state.Value, learningRate, tdTarget);
        }

        public static StateValue<TState> Evaluate<TState>(TState currentState, ProperFraction alpha,
            ProperFraction gamma, StateValue<TState> current)
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