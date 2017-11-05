using System;
using System.Collections.Generic;

namespace DeepQLearning.ConvnetSharp.Layers
{
    // An inefficient dropout layer
    // Note this is not most efficient implementation since the layer before
    // computed all these activations and now we're just going to drop them :(
    // same goes for backward pass. Also, if we wanted to be efficient at test time
    // we could equivalently be clever and upscale during train and copy pointers during test
    // todo: make more efficient.
    [Serializable]
    public class DropoutLayer : Layer
    {
        private readonly bool[] _dropped;

        private readonly Util _util = new Util();

        public DropoutLayer(LayerDefinition def)
        {
            // computed
            OutSx = def.InSx;
            OutSy = def.InSy;
            OutDepth = def.InDepth;
            Type = "dropout";
            DropProb = def.DropProb != double.NaN ? def.DropProb : 0.5;
            _dropped = new bool[OutSx * OutSy * OutDepth];
        }

        public override Volume Forward(Volume volume, bool is_training)
        {
            InAct = volume;

            var V2 = volume.Clone();
            var N = volume.W.Length;
            if (is_training)
                for (var i = 0; i < N; i++)
                    if (_util.Random.NextDouble() < DropProb)
                    {
                        // drop!
                        V2.W[i] = 0;
                        _dropped[i] = true;
                    }

                    else
                    {
                        _dropped[i] = false;
                    }
            else
                for (var i = 0; i < N; i++) V2.W[i] *= DropProb;
            OutAct = V2;
            return OutAct; // dummy identity function for now
        }

        public override double Backward(object y)
        {
            var V = InAct; // we need to set dw of this
            var chainGradient = OutAct;
            var N = V.W.Length;
            V.Dw = Util.Zeros(N); // zero out gradient wrt data
            for (var i = 0; i < N; i++)
                if (!_dropped[i])
                    V.Dw[i] = chainGradient.Dw[i]; // copy over the gradient

            return 0.0;
        }

        public override Gradient[] GetParamsAndGrads() => new List<Gradient>().ToArray();
    }
}