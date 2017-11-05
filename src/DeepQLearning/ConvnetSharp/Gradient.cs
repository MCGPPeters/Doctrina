using System;

namespace DeepQLearning.ConvnetSharp
{
    [Serializable]
    public class Gradient
    {
        public double[] dw;
        public double l1_decay_mul = double.MinValue;
        public double l2_decay_mul = double.MinValue;
        public double[] w;
    }
}