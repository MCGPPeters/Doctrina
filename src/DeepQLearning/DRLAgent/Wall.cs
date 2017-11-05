using System;

namespace DeepQLearning.DRLAgent
{
    [Serializable]
    public class Wall
    {
        public Wall(Vector firstPoint, Vector secondPoint)
        {
            FirstPoint = firstPoint;
            SecondPoint = secondPoint;
        }

        public Vector FirstPoint { get; }
        public Vector SecondPoint { get; }
    }
}