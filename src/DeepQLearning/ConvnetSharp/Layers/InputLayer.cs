using System;
using System.Collections.Generic;

namespace DeepQLearning.ConvnetSharp.Layers
{
    [Serializable]
    public class InputLayer : Layer
    {
        public InputLayer(LayerDefinition def)
        {
            // required: depth
            OutDepth = def.OutDepth;

            // optional: default these dimensions to 1
            OutSx = def.OutSx;
            OutSy = def.OutSy;

            // computed
            Type = "input";
        }

        public override Volume Forward(Volume volume, bool isTraining)
        {
            InAct = volume;
            OutAct = volume;
            return OutAct; // simply identity function for now
        }

        public override double Backward(object y) => 0.0;

        public override Gradient[] GetParamsAndGrads() => new List<Gradient>().ToArray();
    }
}