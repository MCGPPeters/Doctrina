using System;
using System.Collections.Generic;
using DeepQLearning.ConvnetSharp.Layers;

namespace DeepQLearning.ConvnetSharp.Trainer
{
    [Serializable]
    public class TrainingOptions
    {
        public double MinimalEpsilon { get; set; } = double.MinValue;
        public double EpsilonTestTime { get; set; } = double.MinValue;
        public int ExperienceSize { get; set; } = int.MinValue;

        public double Gamma { get; set; } = double.MinValue;
        public int[] HiddenLayerSizes { get; }
        public List<LayerDefinition> LayerDefinitions { get; set; }
        public int LearningStepsBurnin { get; set; } = int.MinValue;
        public int LearningStepsTotal { get; set; } = int.MinValue;

        public Options options;
        public List<double> RandomActionDistribution { get; }
        public int StartLearnThreshold { get; set; } = int.MinValue;
        public int TemporalWindow { get; set; } = int.MinValue;
    }
}