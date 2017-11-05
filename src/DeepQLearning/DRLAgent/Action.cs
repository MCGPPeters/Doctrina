using System;

namespace DeepQLearning.DRLAgent
{
    [Serializable]
    public struct Action
    {
        public int Action1 { get; set; }
        public double Value { get; set; }
    }
}