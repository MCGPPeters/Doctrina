using System;

namespace DeepQLearning.ConvnetSharp.Layers
{
    [Serializable]
    public class LayerDefinition
    {
        public const double L1DecayMul = double.MinValue;
        public const double L2DecayMul = double.MinValue;
        public const int NumberOfClasses = int.MinValue;
        public readonly int NFilters = int.MinValue;
        public string Activation;
        public double BiasPref = double.MinValue;
        public double DropProb = double.MinValue;

        public int GroupSize = int.MinValue;
        public int InDepth = int.MinValue;
        public int InSx = int.MinValue;
        public int InSy = int.MinValue;
        public int NumberOfNeurons = int.MinValue;
        public int OutSx = int.MinValue;
        public int OutSy = int.MinValue;

        public int OutDepth = int.MinValue;
        public int Pad = int.MinValue;
        public int Stride = int.MinValue;
        public int Sx = int.MinValue;
        public int Sy = int.MinValue;
        public string Type;
    }
}