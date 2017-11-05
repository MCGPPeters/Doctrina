using Doctrina.Math;

namespace Doctrina.Tests
{
    public static partial class Regression
    {
        /// <summary>
        ///     Naming conventions. These conventions are used a lot in machine learning. The following
        ///     is a list of these conventions and the naming conventions used for variables in this library:
        ///     Notation:
        ///     symbol (variable name) : Definition
        ///     α (alpha) : Learning rate a.k.a. step size.
        ///     J (function) : A function with some parameter vector
        ///     h (hypothesis) : A candidate function for which we want to determine if is fits a training sample
        ///     π (policy) : A distribution over actions given states (the propability of taking a specific action out of a set of
        ///     actions that can be taken in state S)
        ///     θ (theta) : A parameter vector for a model / hypothesis
        /// </summary>
        //public static class Linear
        //{
        //    public static (Hypothesis, ParameterVector) GradientDescent(Alpha learningRate,
        //        (FeatureVector Features, double ActualOutput)[] samples, ParameterVector theta, Hypothesis hypothesis,
        //        Lambda regularizationParameter) => Optimization.GradientDescent(learningRate, Operations.Identity,
        //        samples, theta, hypothesis, regularizationParameter);
        //}
    }
}