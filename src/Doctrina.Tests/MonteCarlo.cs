using Doctrina.Math;
using Doctrina.Tests;
using Operations = Doctrina.Tests.Math.Operations;

namespace ModelFree.Control.Evaluation
{
    public static class MonteCarlo
    {
        /// <summary>
        ///     Values the specified alpha.
        /// </summary>
        /// <typeparam name="TState">The type of the state.</typeparam>
        /// <param name="state">The state.</param>
        /// <param name="learningRate">The learning rate</param>
        /// <param name="target">The actual return</param>
        /// <returns></returns>
        private static double Value<TState>(this State<TState> state, Alpha learningRate,
            Return target)
            => Operations.Update(state.Value, learningRate, target);
    }
}