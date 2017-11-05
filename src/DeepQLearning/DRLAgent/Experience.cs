using System;

namespace DeepQLearning.DRLAgent
{
    [Serializable]
    public class Experience
    {
        public int ActionTaken;
        public double[] CurrentState;
        public double[] PreviousState;
        public double RewardReceived;
    }
}