using System;
using System.Collections.Generic;
using DeepQLearning.ConvnetSharp.Layers;

namespace DeepQLearning.ConvnetSharp
{
    [Serializable]
    public class Net
    {
        private readonly Util util = new Util();
        public List<Layer> layers = new List<Layer>();

        // constructor

        // takes a list of layer definitions and creates the network layer objects
        public void makeLayers(List<LayerDefinition> defs)
        {
            // few checks
            util.Assert(defs.Count >= 2, "Error! At least one input layer and one loss layer are required.");
            util.Assert(defs[0].Type == "input",
                "Error! First layer must be the input layer, to declare size of inputs");

            var new_defs = new List<LayerDefinition>();
            for (var i = 0; i < defs.Count; i++)
            {
                var def = defs[i];

                if (def.Type == "softmax" || def.Type == "svm")
                    new_defs.Add(new LayerDefinition {Type = "fc", NumberOfNeurons = LayerDefinition.NumberOfClasses});

                if (def.Type == "regression")
                    new_defs.Add(new LayerDefinition {Type = "fc", NumberOfNeurons = def.NumberOfNeurons});

                if ((def.Type == "fc" || def.Type == "conv") && def.BiasPref == int.MinValue)
                {
                    def.BiasPref = 0.0;
                    if (!string.IsNullOrEmpty(def.Activation) && def.Activation == "relu")
                        def.BiasPref = 0.1;
                }

                new_defs.Add(def);

                if (!string.IsNullOrEmpty(def.Activation))
                    if (def.Activation == "relu")
                    {
                        new_defs.Add(new LayerDefinition {Type = "relu"});
                    }
                    else if (def.Activation == "sigmoid")
                    {
                        new_defs.Add(new LayerDefinition {Type = "sigmoid"});
                    }
                    else if (def.Activation == "tanh")
                    {
                        new_defs.Add(new LayerDefinition {Type = "tanh"});
                    }
                    else if (def.Activation == "maxout")
                    {
                        // create maxout activation, and pass along group size, if provided
                        var gs = def.GroupSize != int.MinValue ? def.GroupSize : 2;
                        new_defs.Add(new LayerDefinition {Type = "maxout", GroupSize = gs});
                    }
                    else
                    {
                        Console.WriteLine("ERROR unsupported activation " + def.Activation);
                    }

                if (def.DropProb != double.MinValue && def.Type != "dropout")
                    new_defs.Add(new LayerDefinition {Type = "dropout", DropProb = def.DropProb});
            }

            defs = new_defs;

            // create the layers
            layers = new List<Layer>();
            for (var i = 0; i < defs.Count; i++)
            {
                var def = defs[i];
                if (i > 0)
                {
                    var prev = layers[i - 1];
                    def.InSx = prev.OutSx;
                    def.InSy = prev.OutSy;
                    def.InDepth = prev.OutDepth;
                }

                switch (def.Type)
                {
                    case "fc":
                        layers.Add(new FullyConnectedLayer(def));
                        break;
                    //case "lrn": this.layers.Add(new LocalResponseNormalizationLayer(def)); break;
                    case "dropout":
                        layers.Add(new DropoutLayer(def));
                        break;
                    case "input":
                        layers.Add(new InputLayer(def));
                        break;
                    //case "softmax": this.layers.Add(new SoftmaxLayer(def)); break;
                    case "regression":
                        layers.Add(new RegressionLayer(def));
                        break;
                    case "conv":
                        layers.Add(new ConvolutionalLayer(def));
                        break;
                    //case "pool": this.layers.Add(new PoolLayer(def)); break;
                    case "relu":
                        layers.Add(new ReLULayer(def));
                        break;
                    //case "sigmoid": this.layers.Add(new SigmoidLayer(def)); break;
                    //case "tanh": this.layers.Add(new TanhLayer(def)); break;
                    //case "maxout": this.layers.Add(new MaxoutLayer(def)); break;
                    case "svm":
                        layers.Add(new SVMLayer(def));
                        break;
                    default:
                        Console.WriteLine("ERROR: UNRECOGNIZED LAYER TYPE: " + def.Type);
                        break;
                }
            }
        }

        // forward prop the network. 
        // The trainer class passes is_training = true, but when this function is
        // called from outside (not from the trainer), it defaults to prediction mode
        public Volume forward(Volume V, bool is_training)
        {
            var act = layers[0].Forward(V, is_training);

            for (var i = 1; i < layers.Count; i++)
                act = layers[i].Forward(act, is_training);
            return act;
        }

        public double getCostLoss(Volume V, int y)
        {
            forward(V, false);
            var N = layers.Count;
            var loss = layers[N - 1].Backward(y);
            return loss;
        }

        // backprop: compute gradients wrt all parameters
        public double backward(object y)
        {
            var N = layers.Count;
            var loss = layers[N - 1].Backward(y); // last layer assumed to be loss layer
            for (var i = N - 2; i >= 0; i--)
                // first layer assumed input
                layers[i].Backward(y);

            return loss;
        }

        public Gradient[] getParamsAndGrads()
        {
            // accumulate parameters and gradients for the entire network
            var response = new List<Gradient>();
            for (var i = 0; i < layers.Count; i++)
            {
                var layer_reponse = layers[i].GetParamsAndGrads();
                for (var j = 0; j < layer_reponse.Length; j++)
                    response.Add(layer_reponse[j]);
            }

            return response.ToArray();
        }

        public int getPrediction()
        {
            // this is a convenience function for returning the argmax
            // prediction, assuming the last layer of the net is a softmax
            var S = layers[layers.Count - 1];
            util.Assert(S.Type == "softmax", "getPrediction function assumes softmax as last layer of the net!");

            var p = S.OutAct.W;
            var maxv = p[0];
            var maxi = 0;
            for (var i = 1; i < p.Length; i++)
                if (p[i] > maxv)
                {
                    maxv = p[i];
                    maxi = i;
                }

            return maxi; // return index of the class with highest class probability
        }
    }
}