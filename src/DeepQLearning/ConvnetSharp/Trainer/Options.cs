using System;

namespace DeepQLearning.ConvnetSharp.Trainer
{
    [Serializable]
    public class Options
    {
        public int BatchSize = int.MinValue;
        public double Beta1 = double.MinValue;
        public double Beta2 = double.MinValue;
        public double Eps = double.MinValue;
        public double L1Decay = double.MinValue;
        public double L2Decay = double.MinValue;

        public double LearningRate = double.MinValue;
        public string Method = string.Empty;
        public double Momentum = double.MinValue;
        public double Ro = double.MinValue;
    }
}