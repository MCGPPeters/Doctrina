using System;
using System.Collections.Generic;

namespace DeepQLearning.ConvnetSharp.Layers
{
    [Serializable]
    public abstract class Layer
    {
        protected double DropProb;

        protected List<Volume> Filters;

        protected Volume InAct;
        protected int InDepth;

        protected double L1DecayMul;
        protected double L2DecayMul;
        protected int NumInputs;
        public Volume OutAct;

        public int OutDepth;
        public int OutSx;
        public int OutSy;
        protected int Sx;
        protected int Sy;
        public string Type;

        public abstract Gradient[] GetParamsAndGrads();
        public abstract Volume Forward(Volume volume, bool isTraining);
        public abstract double Backward(object y);
    }
}