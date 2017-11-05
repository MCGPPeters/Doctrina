using System;
using System.Collections.Generic;

namespace DeepQLearning.ConvnetSharp.Layers
{
    [Serializable]
    public class FullyConnectedLayer : Layer
    {
        private readonly Volume _biases;

        public FullyConnectedLayer(LayerDefinition def)
        {
            // required
            OutDepth = def.NumberOfNeurons;

            // optional 
            L1DecayMul = LayerDefinition.L1DecayMul != double.MinValue ? LayerDefinition.L1DecayMul : 0.0;
            L2DecayMul = LayerDefinition.L2DecayMul != double.MinValue ? LayerDefinition.L2DecayMul : 1.0;

            // computed
            NumInputs = def.InSx * def.InSy * def.InDepth;
            OutSx = 1;
            OutSy = 1;
            Type = "fc";

            // initializations
            var bias = def.BiasPref != double.MinValue ? def.BiasPref : 0.0;
            Filters = new List<Volume>();
            for (var i = 0; i < OutDepth; i++) Filters.Add(new Volume(1, 1, NumInputs));
            _biases = new Volume(1, 1, OutDepth, bias);
        }

        public override Volume Forward(Volume volume, bool is_training)
        {
            InAct = volume;
            var A = new Volume(1, 1, OutDepth, 0.0);
            var Vw = volume.W;
            for (var i = 0; i < OutDepth; i++)
            {
                var a = 0.0;
                var wi = Filters[i].W;
                for (var d = 0; d < NumInputs; d++)
                    a += Vw[d] * wi[d]; // for efficiency use Vols directly for now
                a += _biases.W[i];
                A.W[i] = a;
            }
            OutAct = A;
            return OutAct;
        }

        public override double Backward(object y)
        {
            var V = InAct;
            V.Dw = Util.Zeros(V.W.Length); // zero out the gradient in input Vol

            // compute gradient wrt weights and data
            for (var i = 0; i < OutDepth; i++)
            {
                var tfi = Filters[i];
                var chainGradient = OutAct.Dw[i];
                for (var d = 0; d < NumInputs; d++)
                {
                    V.Dw[d] += tfi.W[d] * chainGradient; // grad wrt input data
                    tfi.Dw[d] += V.W[d] * chainGradient; // grad wrt params
                }
                _biases.Dw[i] += chainGradient;
            }

            return 0.0;
        }

        public override Gradient[] GetParamsAndGrads()
        {
            var response = new List<Gradient>();
            for (var i = 0; i < OutDepth; i++)
                response.Add(new Gradient
                {
                    w = Filters[i].W,
                    dw = Filters[i].Dw,
                    l1_decay_mul = L1DecayMul,
                    l2_decay_mul = L2DecayMul
                });

            response.Add(new Gradient {w = _biases.W, dw = _biases.Dw, l1_decay_mul = 0.0, l2_decay_mul = 0.0});
            return response.ToArray();
        }
    }
}