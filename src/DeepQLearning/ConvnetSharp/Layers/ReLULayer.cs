using System;
using System.Collections.Generic;

namespace DeepQLearning.ConvnetSharp.Layers
{
    [Serializable]
    public class ReLULayer : Layer
    {
        public ReLULayer(LayerDefinition def)
        {
            // computed
            OutSx = def.InSx;
            OutSy = def.InSy;
            OutDepth = def.InDepth;
            Type = "relu";
        }

        public override Volume Forward(Volume volume, bool isTraining)
        {
            InAct = volume;
            var v2 = volume.Clone();
            var n = volume.W.Length;
            var v2W = v2.W;
            for (var i = 0; i < n; i++)
                if (v2W[i] < 0) v2W[i] = 0; // threshold at 0
            OutAct = v2;
            return OutAct;
        }

        public override double Backward(object y)
        {
            var v = InAct; // we need to set dw of this
            var v2 = OutAct;
            var n = v.W.Length;
            v.Dw = Util.Zeros(n); // zero out gradient wrt data
            for (var i = 0; i < n; i++)
                if (v2.W[i] <= 0) v.Dw[i] = 0; // threshold
                else v.Dw[i] = v2.Dw[i];

            return 0.0;
        }

        public override Gradient[] GetParamsAndGrads() => new List<Gradient>().ToArray();
    }
}