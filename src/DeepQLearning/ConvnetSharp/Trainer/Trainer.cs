using System;
using System.Collections.Generic;

namespace DeepQLearning.ConvnetSharp.Trainer
{
    [Serializable]
    public class Trainer
    {
        private readonly double _beta1;
        private readonly double _beta2;
        private readonly double _eps;
        private readonly List<double[]> _gsum; // last iteration gradients (used for momentum calculations)
        private readonly double _l1Decay;
        private readonly double _l2Decay;

        private readonly double _learningRate;
        private readonly string _method;

        private readonly double _momentum;
        private readonly Net _net;
        private readonly double _ro;

        private readonly List<double[]> xsum; // used in adam or adadelta
        public readonly double BatchSize;

        private double _k; // iteration counter

        public Trainer(Net net, Options options)
        {
            _net = net;

            _learningRate = options.LearningRate != double.MinValue ? options.LearningRate : 0.01;
            _l1Decay = options.L1Decay != double.MinValue ? options.L1Decay : 0.0;
            _l2Decay = options.L2Decay != double.MinValue ? options.L2Decay : 0.0;
            BatchSize = options.BatchSize != int.MinValue ? options.BatchSize : 1;

            // methods: sgd/adam/adagrad/adadelta/windowgrad/netsterov
            _method = string.IsNullOrEmpty(options.Method) ? "sgd" : options.Method;

            _momentum = options.Momentum != double.MinValue ? options.Momentum : 0.9;
            _ro = options.Ro != double.MinValue ? options.Ro : 0.95; // used in adadelta
            _eps = options.Eps != double.MinValue ? options.Eps : 1e-8; // used in adam or adadelta
            _beta1 = options.Beta1 != double.MinValue ? options.Beta1 : 0.9; // used in adam
            _beta2 = options.Beta2 != double.MinValue ? options.Beta2 : 0.999; // used in adam

            _gsum = new List<double[]>();
            xsum = new List<double[]>();
        }

        public Dictionary<string, string> Train(Volume x, object y)
        {
            var start = new DateTime();
            _net.forward(x, true); // also set the flag that lets the net know we're just training
            var end = new DateTime();
            var fwdTime = end - start;

            start = new DateTime();
            var costLoss = _net.backward(y);
            var l2DecayLoss = 0.0;
            var l1DecayLoss = 0.0;
            end = new DateTime();
            var bwdTime = end - start;

            _k++;
            if (_k % BatchSize == 0)
            {
                var pglist = _net.getParamsAndGrads();

                // initialize lists for accumulators. Will only be done once on first iteration
                if (_gsum.Count == 0 && (_method != "sgd" || _momentum > 0.0))
                    for (var i = 0; i < pglist.Length; i++)
                    {
                        _gsum.Add(Util.Zeros(pglist[i].w.Length));
                        if (_method == "adam" || _method == "adadelta")
                            xsum.Add(Util.Zeros(pglist[i].w.Length));
                        else
                            xsum.Add(new List<double>().ToArray()); // conserve memory
                    }

                // perform an update for all sets of weights
                for (var i = 0; i < pglist.Length; i++)
                {
                    var pg = pglist[i]; // param, gradient, other options in future (custom learning rate etc)
                    var p = pg.w;
                    var g = pg.dw;

                    // learning rate for some parameters.
                    var l2_decay_mul = pg.l2_decay_mul != double.MinValue ? pg.l2_decay_mul : 1.0;
                    var l1_decay_mul = pg.l1_decay_mul != double.MinValue ? pg.l1_decay_mul : 1.0;
                    var l2_decay = this._l2Decay * l2_decay_mul;
                    var l1_decay = this._l1Decay * l1_decay_mul;

                    var plen = p.Length;
                    for (var j = 0; j < plen; j++)
                    {
                        l2DecayLoss += l2_decay * p[j] * p[j] / 2; // accumulate weight decay loss
                        l1DecayLoss += l1_decay * Math.Abs(p[j]);
                        var l1Grad = l1_decay * (p[j] > 0 ? 1 : -1);
                        var l2Grad = l2_decay * p[j];

                        var gij = (l2Grad + l1Grad + g[j]) / BatchSize; // raw batch gradient

                        var gsumi = _gsum[i];
                        var xsumi = xsum[i];
                        if (_method == "adam")
                        {
                            // adam update
                            gsumi[j] = gsumi[j] * _beta1 + (1 - _beta1) * gij; // update biased first moment estimate
                            xsumi[j] = xsumi[j] * _beta2 +
                                       (1 - _beta2) * gij * gij; // update biased second moment estimate
                            var biasCorr1 = gsumi[j] * (1 - Math.Pow(_beta1, _k)); // correct bias first moment estimate
                            var biasCorr2 = xsumi[j] * (1 - Math.Pow(_beta2, _k)); // correct bias second moment estimate
                            var dx = -_learningRate * biasCorr1 / (Math.Sqrt(biasCorr2) + _eps);
                            p[j] += dx;
                        }
                        else if (_method == "adagrad")
                        {
                            // adagrad update
                            gsumi[j] = gsumi[j] + gij * gij;
                            var dx = -_learningRate / Math.Sqrt(gsumi[j] + _eps) * gij;
                            p[j] += dx;
                        }
                        else if (_method == "windowgrad")
                        {
                            // this is adagrad but with a moving window weighted average
                            // so the gradient is not accumulated over the entire history of the run. 
                            // it's also referred to as Idea #1 in Zeiler paper on Adadelta. Seems reasonable to me!
                            gsumi[j] = _ro * gsumi[j] + (1 - _ro) * gij * gij;
                            var dx = -_learningRate / Math.Sqrt(gsumi[j] + _eps) *
                                     gij; // eps added for better conditioning
                            p[j] += dx;
                        }
                        else if (_method == "adadelta")
                        {
                            gsumi[j] = _ro * gsumi[j] + (1 - _ro) * gij * gij;
                            var dx = -Math.Sqrt((xsumi[j] + _eps) / (gsumi[j] + _eps)) * gij;
                            xsumi[j] = _ro * xsumi[j] + (1 - _ro) * dx * dx; // yes, xsum lags behind gsum by 1.
                            p[j] += dx;
                        }
                        else if (_method == "nesterov")
                        {
                            var dx = gsumi[j];
                            gsumi[j] = gsumi[j] * _momentum + _learningRate * gij;
                            dx = _momentum * dx - (1.0 + _momentum) * gsumi[j];
                            p[j] += dx;
                        }
                        else
                        {
                            // assume SGD
                            if (_momentum > 0.0)
                            {
                                // momentum update
                                var dx = _momentum * gsumi[j] - _learningRate * gij; // step
                                gsumi[j] = dx; // back this up for next iteration of momentum
                                p[j] += dx; // apply corrected gradient
                            }
                            else
                            {
                                // vanilla sgd
                                p[j] += -_learningRate * gij;
                            }
                        }
                        g[j] = 0.0; // zero out gradient so that we can begin accumulating anew
                    }
                }
            }

            // appending softmax_loss for backwards compatibility, but from now on we will always use cost_loss
            // in future, TODO: have to completely redo the way loss is done around the network as currently 
            // loss is a bit of a hack. Ideally, user should specify arbitrary number of loss functions on any layer
            // and it should all be computed correctly and automatically. 

            var result = new Dictionary<string, string>
            {
                {"fwd_time", fwdTime.TotalMilliseconds + " millisec"},
                {"bwd_time", bwdTime.TotalMilliseconds + " millisec"},
                {"l2_decay_loss", l2DecayLoss.ToString()},
                {"l1_decay_loss", l1DecayLoss.ToString()},
                {"cost_loss", costLoss.ToString()},
                {"loss", (costLoss + l1DecayLoss + l2DecayLoss).ToString()}
            };

            return result;
        }
    }
}