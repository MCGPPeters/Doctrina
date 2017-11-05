using System;

namespace DeepQLearning.DRLAgent
{
    [Serializable]
    public class Item
    {
        public Vector Position;

        public Item(double x, double y, ObjectType objectType)
        {
            Position = new Vector(x, y);
            ObjectType = objectType;
            Radius = 10; // default radius
            Age = 0;
            Cleanup = false;
        }

        public int Age { get; set; }
        public bool Cleanup { get; set; }
        public double Radius { get; }
        public ObjectType ObjectType { get; }
    }
}