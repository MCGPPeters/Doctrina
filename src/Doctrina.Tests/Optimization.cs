using System.Collections.Generic;
using System.Linq;
using Doctrina.Math;

namespace Doctrina.Tests
{
    public static class Optimization
    {
        ///// <summary>
        ///// </summary>
        ///// <param name="learningRate">The learning rate.</param>
        ///// <param name="normalize"></param>
        ///// <param name="samples"></param>
        ///// <param name="theta"></param>
        ///// <param name="hypothesis"></param>
        ///// <param name="regularizationParameter">Prevents overfitting</param>
        ///// <returns></returns>
        //public static (Hypothesis, ParameterVector) GradientDescent(Alpha learningRate, Normalize normalize,
        //    (FeatureVector Features, double ActualOutput)[] samples, ParameterVector theta, Hypothesis hypothesis,
        //    Lambda regularizationParameter)
        //{
        //    var parameters = new List<Parameter>();

        //    for (var i = 0; i < theta.Count(); i++)
        //    {
        //        var weight = theta[i].Scalar * (1 - learningRate * (regularizationParameter / samples.Length)) -
        //                     learningRate * samples.Average(sample =>
        //                     {
        //                         var hypothesisOutput = hypothesis(sample.Features);
        //                         var error = hypothesisOutput - sample.ActualOutput;
        //                         return error * sample.Features[i].Value;
        //                     });
        //        parameters.Add(new Parameter(theta[i].Descriptor, weight));
        //    }
        //    var thetaPrime = parameters;
        //    return (Regression.CreateHypothesis(normalize)(new ParameterVector(thetaPrime)), thetaPrime);
        //}
    }
}