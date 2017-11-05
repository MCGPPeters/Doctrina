using System;

namespace DeepQLearning.DRLAgent
{
    [Serializable]
    public class Eye
    {
        public Eye(double angle)
        {
            Angle = angle;
            MaximumRange = 85;
            SensedProximity = 85;
            ObjectType = ObjectType.Nothing;
        }

        // Angle of the eye relative to the agent
        public double Angle { get; }

        // Maximum proximity range
        public double MaximumRange { get; }

        // Proximity of what the eye is seeing. will be set in world.tick()
        public double SensedProximity { get; set; }

        // what type of object does the eye see?; set; }
        public ObjectType ObjectType { get; set; }
    }
}