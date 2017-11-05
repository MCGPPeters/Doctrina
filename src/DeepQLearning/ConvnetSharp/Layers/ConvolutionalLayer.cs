using System;
using System.Collections.Generic;

namespace DeepQLearning.ConvnetSharp.Layers
{
    // - ConvLayer does convolutions (so weight sharing spatially)
    [Serializable]
    public class ConvolutionalLayer : Layer
    {
        private readonly Volume _biases;
        private readonly int _pad;
        private readonly int _stride;

        public ConvolutionalLayer(LayerDefinition def)
        {
            // required
            OutDepth = def.NFilters;
            Sx = def.Sx; // filter size. Should be odd if possible, it's cleaner.
            InDepth = def.InDepth;

            // optional
            Sy = def.Sy != int.MinValue ? def.Sy : Sx;
            _stride = def.Stride != int.MinValue ? def.Stride : 1; // stride at which we apply filters to input volume
            _pad = def.Pad != int.MinValue ? def.Pad : 0; // amount of 0 padding to add around borders of input volume
            L1DecayMul = LayerDefinition.L1DecayMul != double.MinValue ? LayerDefinition.L1DecayMul : 0.0;
            L2DecayMul = LayerDefinition.L2DecayMul != double.MinValue ? LayerDefinition.L2DecayMul : 1.0;

            // computed
            // note we are doing floor, so if the strided convolution of the filter doesnt fit into the input
            // volume exactly, the output volume will be trimmed and not contain the (incomplete) computed
            // final application.
            OutSx = (int) Math.Floor((double) (def.InSx + _pad * 2 - Sx) / _stride + 1);
            OutSy = (int) Math.Floor((double) (def.InSy + _pad * 2 - Sy) / _stride + 1);
            Type = "conv";

            // initializations
            var bias = def.BiasPref != double.MinValue ? def.BiasPref : 0.0;
            Filters = new List<Volume>();
            for (var i = 0; i < OutDepth; i++) Filters.Add(new Volume(Sx, Sy, InDepth));
            _biases = new Volume(1, 1, OutDepth, bias);
        }

        public override Volume Forward(Volume volume, bool isTraining)
        {
            // optimized code by @mdda that achieves 2x speedup over previous version

            InAct = volume;
            var A = new Volume(OutSx | 0, OutSy | 0, OutDepth | 0, 0.0);

            var V_sx = volume.Sx | 0;
            var V_sy = volume.Sy | 0;
            var xy_stride = _stride | 0;

            for (var d = 0; d < OutDepth; d++)
            {
                var f = Filters[d];
                var x = -_pad | 0;
                var y = -_pad | 0;
                for (var ay = 0; ay < OutSy; y += xy_stride, ay++)
                {
                    // xy_stride
                    x = -_pad | 0;
                    for (var ax = 0; ax < OutSx; x += xy_stride, ax++)
                    {
                        // xy_stride

                        // convolve centered at this particular location
                        var a = 0.0;
                        for (var fy = 0; fy < f.Sy; fy++)
                        {
                            var oy = y + fy; // coordinates in the original input array coordinates
                            for (var fx = 0; fx < f.Sx; fx++)
                            {
                                var ox = x + fx;
                                if (oy >= 0 && oy < V_sy && ox >= 0 && ox < V_sx)
                                    for (var fd = 0; fd < f.Depth; fd++)
                                        // avoid function call overhead (x2) for efficiency, compromise modularity :(
                                        a += f.W[(f.Sx * fy + fx) * f.Depth + fd] *
                                             volume.W[(V_sx * oy + ox) * volume.Depth + fd];
                            }
                        }
                        a += _biases.W[d];
                        A.Set(ax, ay, d, a);
                    }
                }
            }
            OutAct = A;
            return OutAct;
        }

        public override double Backward(object _y)
        {
            var V = InAct;
            V.Dw = Util.Zeros(V.W.Length); // zero out gradient wrt bottom data, we're about to fill it

            var V_sx = V.Sx | 0;
            var V_sy = V.Sy | 0;
            var xy_stride = _stride | 0;

            for (var d = 0; d < OutDepth; d++)
            {
                var f = Filters[d];
                var x = -_pad | 0;
                var y = -_pad | 0;
                for (var ay = 0; ay < OutSy; y += xy_stride, ay++)
                {
                    // xy_stride
                    x = -_pad | 0;
                    for (var ax = 0; ax < OutSx; x += xy_stride, ax++)
                    {
                        // xy_stride

                        // convolve centered at this particular location
                        var chainGradient = OutAct.GetGradient(ax, ay, d); // gradient from above, from chain rule
                        for (var fy = 0; fy < f.Sy; fy++)
                        {
                            var oy = y + fy; // coordinates in the original input array coordinates
                            for (var fx = 0; fx < f.Sx; fx++)
                            {
                                var ox = x + fx;
                                if (oy >= 0 && oy < V_sy && ox >= 0 && ox < V_sx)
                                    for (var fd = 0; fd < f.Depth; fd++)
                                    {
                                        // avoid function call overhead (x2) for efficiency, compromise modularity :(
                                        var ix1 = (V_sx * oy + ox) * V.Depth + fd;
                                        var ix2 = (f.Sx * fy + fx) * f.Depth + fd;
                                        f.Dw[ix2] += V.W[ix1] * chainGradient;
                                        V.Dw[ix1] += f.W[ix2] * chainGradient;
                                    }
                            }
                        }
                        _biases.Dw[d] += chainGradient;
                    }
                }
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
                    l2_decay_mul = L2DecayMul,
                    l1_decay_mul = L1DecayMul
                });
            response.Add(new Gradient {w = _biases.W, dw = _biases.Dw, l1_decay_mul = 0.0, l2_decay_mul = 0.0});
            return response.ToArray();
        }
    }
}