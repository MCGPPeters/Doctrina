using System;
using System.Collections.Generic;
using DeepQLearning.ConvnetSharp;

namespace DeepQLearning.DRLAgent
{
    [Serializable]
    public class Agent
    {
        public Agent(Brain brain)
        {
            Brain = brain;

            CurrentPosition = new Vector(50, 50);
            PreviousPosition = CurrentPosition;
            Angle = 0; // direction facing

            Actions = new List<double[]>
            {
                new[] {1d, 1d},
                new[] {0.8, 1},
                new[] {1, 0.8},
                new[] {0.5, 0},
                new[] {0, 0.5}
            };

            Radius = 10;
            Eyes = new List<Eye>();
            for (var k = 0; k < 9; k++)
                Eyes.Add(new Eye((k - 3) * 0.25));

            RewardBonus = 0.0;
            DigestionSignal = 0.0;

            // outputs on world
            RotationSpeedOfFirstWheel = 0.0;
            RotationSpeedOfSecondWheel = 0.0;

            PreviousActionIndex = -1;
        }

        private int ActionIndex { get; set; }
        private List<double[]> Actions { get; }
        public double Angle { get; set; }
        public double PreviousAngle { get; set; }
        private double RewardBonus { get; }
        public double DigestionSignal { get; set; }
        public Brain Brain { get; }
        public List<Eye> Eyes { get; }
        public Vector CurrentPosition { get; set; }
        public Vector PreviousPosition { get; set; }
        public double Radius { get; }
        public double RotationSpeedOfFirstWheel { get; set; }
        public double RotationSpeedOfSecondWheel { get; set; }
        private double PreviousActionIndex { get; }

        /// <summary>
        ///     Act in response to environment
        /// </summary>
        public void Act()
        {
            // create input to brain
            var numberOfEyes = Eyes.Count;
            var inputArray = new double[numberOfEyes * 3];
            for (var i = 0; i < numberOfEyes; i++)
            {
                var eye = Eyes[i];
                inputArray[i * 3] = 1.0;
                inputArray[i * 3 + 1] = 1.0;
                inputArray[i * 3 + 2] = 1.0;
                if (eye.ObjectType != ObjectType.Nothing)
                    inputArray[i * 3 + (int) eye.ObjectType] = eye.SensedProximity / eye.MaximumRange; // normalize to [0,1]
            }

            var input = new Volume(numberOfEyes, 3, 1)
            {
                W = inputArray
            };

            // get action from brain
            var actionIndex = Brain.SelectActionAccordingToPolicy(input);
            var action = Actions[actionIndex];
            ActionIndex = actionIndex; //back this up

            // demultiplex into behavior variables
            RotationSpeedOfFirstWheel = action[0] * 1;
            RotationSpeedOfSecondWheel = action[1] * 1;
        }

        public void EvaluateAction()
        {
            var reward = Reward();

            // pass to brain for learning
            Brain.EvaluateAction(reward);
        }

        private double Reward()
        {
            // compute reward 
            var proximityReward = 0.0;
            var numberOfEyes = Eyes.Count;
            for (var i = 0; i < numberOfEyes; i++)
            {
                var eye = Eyes[i];
                // agents dont like to see walls, especially up close
                proximityReward += eye.ObjectType == ObjectType.Wall ? eye.SensedProximity / eye.MaximumRange : 1.0;
            }
            proximityReward = proximityReward / numberOfEyes;
            proximityReward = Math.Min(1.0, proximityReward * 2);

            // agents like to go straight forward
            var forwardReward = 0.0;
            if (ActionIndex == 0 && proximityReward > 0.75) forwardReward = 0.1 * proximityReward;

            // agents like to eat good things
            var digestionReward = DigestionSignal;
            DigestionSignal = 0.0;

            var reward = proximityReward + forwardReward + digestionReward;
            return reward;
        }
    }
}