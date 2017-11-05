using System;
using System.Collections.Generic;

namespace DeepQLearning.ConvnetSharp.Layers
{
    [Serializable]
    public class SVMLayer : Layer
    {
        public SVMLayer(LayerDefinition def)
        {
            // computed
            NumInputs = def.InSx * def.InSy * def.InDepth;
            OutDepth = NumInputs;
            OutSx = 1;
            OutSy = 1;
            Type = "svm";
        }

        public override Volume Forward(Volume volume, bool is_training)
        {
            InAct = volume;
            OutAct = volume; // nothing to do, output raw scores
            return volume;
        }

        public override double Backward(object y)
        {
            var index = (int) y;

            // compute and accumulate gradient wrt weights and bias of this layer
            var x = InAct;
            x.Dw = Util.Zeros(x.W.Length); // zero out the gradient of input Vol

            // we're using structured loss here, which means that the score
            // of the ground truth should be higher than the score of any other 
            // class, by a margin
            var yscore = x.W[index]; // score of ground truth
            const double margin = 1.0;
            var loss = 0.0;
            for (var i = 0; i < OutDepth; i++)
            {
                if (index == i) continue;
                var ydiff = -yscore + x.W[i] + margin;
                if (ydiff > 0)
                {
                    // violating dimension, apply loss
                    x.Dw[i] += 1;
                    x.Dw[index] -= 1;
                    loss += ydiff;
                }
            }

            return loss;
        }

        public override Gradient[] GetParamsAndGrads() => new List<Gradient>().ToArray();
    }
}