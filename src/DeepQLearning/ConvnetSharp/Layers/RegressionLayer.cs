using System;
using System.Collections.Generic;

namespace DeepQLearning.ConvnetSharp.Layers
{
    [Serializable]
    public class RegressionLayer : Layer
    {
        public RegressionLayer(LayerDefinition def)
        {
            // computed
            NumInputs = def.InSx * def.InSy * def.InDepth;
            OutDepth = NumInputs;
            OutSx = 1;
            OutSy = 1;
            Type = "regression";
        }

        public override Volume Forward(Volume volume, bool is_training)
        {
            InAct = volume;
            OutAct = volume;
            return OutAct; // simply identity function for now
        }

        // y is a list here of size num_inputs
        // or it can be a number if only one value is regressed
        // or it can be a struct {dim: i, val: x} where we only want to 
        // regress on dimension i and asking it to have value x
        public override double Backward(object y)
        {
            // compute and accumulate gradient wrt weights and bias of this layer
            var x = InAct;
            x.Dw = Util.Zeros(x.W.Length); // zero out the gradient of input Vol
            var loss = 0.0;
            if (y.GetType() == typeof(Array))
            {
                var Y = (double[]) y;

                for (var i = 0; i < OutDepth; i++)
                {
                    var dy = x.W[i] - Y[i];
                    x.Dw[i] = dy;
                    loss += 0.5 * dy * dy;
                }
            }
            else if (y is double)
            {
                // lets hope that only one number is being regressed
                var dy = x.W[0] - (double) y;
                x.Dw[0] = dy;
                loss += 0.5 * dy * dy;
            }
            else
            {
                // assume it is a struct with entries .dim and .val
                // and we pass gradient only along dimension dim to be equal to val
                var i = ((Entry) y).Dim;
                var yi = ((Entry) y).Val;
                var dy = x.W[i] - yi;
                x.Dw[i] = dy;
                loss += 0.5 * dy * dy;
            }

            return loss;
        }

        public override Gradient[] GetParamsAndGrads() => new List<Gradient>().ToArray();
    }
}