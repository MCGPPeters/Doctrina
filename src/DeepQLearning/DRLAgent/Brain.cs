using System;
using System.Collections.Generic;
using System.Linq;
using DeepQLearning.ConvnetSharp;
using DeepQLearning.ConvnetSharp.Layers;
using DeepQLearning.ConvnetSharp.Trainer;

namespace DeepQLearning.DRLAgent
{
    // An agent is in state0 and does action0
    // environment then assigns reward0 and provides new state, state1
    // Experience nodes store all this information, which is used in the
    // Q-learning update step

    // A Brain object does all the magic.
    // over time it receives some inputs and some rewards
    // and its job is to set the outputs to maximize the expected reward
    [Serializable]
    public class Brain
    {
        private readonly List<int> _actionWindow;
        private readonly TrainingWindow _averageLossWindow;
        private readonly TrainingWindow _averageRewardWindow;
        private readonly double _epsilonMin;
        private readonly List<Experience> _experience;
        private readonly int _experienceSize;
        private readonly double _gamma;
        private readonly double _learningStepsBurnin;
        private readonly double _learningStepsTotal;
        private readonly int _netInputs;
        private readonly List<double[]> _netWindow;
        private readonly int _numberOfActions;
        private readonly int _numberOfStates;

        private readonly List<double> _randomActionDistribution;
        private readonly List<double> _rewardWindow;
        private readonly double _startLearnThreshold;
        private readonly List<Volume> _stateWindow;
        private readonly Trainer _temporalDifferencetrainer;

        private readonly int _temporalWindow;

        private readonly Util _util;

        private readonly Net _valueNetwork;
        private readonly int _windowSize;
        private double _age;
        private double _epsilon;
        private double _forwardPasses;
        public double EpsilonTestTime;
        public bool Learning;

        public Brain(int numberOfStates, int numberOfActions, TrainingOptions trainingOptions, double doubleComparisonTollerance)
        {
            _util = new Util();

            // in number of time steps, of temporal memory
            // the ACTUAL input to the net will be (x,a) temporal_window times, and followed by current x
            // so to have no information from previous time step going into value function, set to 0.
            _temporalWindow = trainingOptions.TemporalWindow != int.MinValue ? trainingOptions.TemporalWindow : 1;
            // size of experience replay memory
            _experienceSize = trainingOptions.ExperienceSize != int.MinValue ? trainingOptions.ExperienceSize : 30000;
            // number of examples in experience replay memory before we begin learning
            _startLearnThreshold = trainingOptions.StartLearnThreshold != double.MinValue
                ? trainingOptions.StartLearnThreshold
                : Math.Floor(Math.Min(_experienceSize * 0.1, 1000));
            // gamma is a crucial parameter that controls how much plan-ahead the agent does. In [0,1]
            _gamma = Math.Abs(trainingOptions.Gamma - double.MinValue) > doubleComparisonTollerance 
                ? trainingOptions.Gamma 
                : 0.8;

            // number of steps we will learn for
            _learningStepsTotal = trainingOptions.LearningStepsTotal != int.MinValue 
                ? trainingOptions.LearningStepsTotal 
                : 100000;
            // how many steps of the above to perform only random actions (in the beginning)?
            _learningStepsBurnin = trainingOptions.LearningStepsBurnin != int.MinValue 
                ? trainingOptions.LearningStepsBurnin 
                : 3000;
            // what epsilon value do we bottom out on? 0.0 => purely deterministic policy at end
            _epsilonMin = Math.Abs(trainingOptions.MinimalEpsilon - double.MinValue) > doubleComparisonTollerance 
                ? trainingOptions.MinimalEpsilon 
                : 0.05;
            // what epsilon to use at test time? (i.e. when learning is disabled)
            EpsilonTestTime = Math.Abs(trainingOptions.EpsilonTestTime - double.MinValue) > doubleComparisonTollerance 
                ? trainingOptions.EpsilonTestTime 
                : 0.00;

            // advanced feature. Sometimes a random action should be biased towards some values
            // for example in flappy bird, we may want to choose to not flap more often
            if (trainingOptions.RandomActionDistribution != null)
            {
                // this better sum to 1 by the way, and be of length this.num_actions
                _randomActionDistribution = trainingOptions.RandomActionDistribution;
                if (_randomActionDistribution.Count != numberOfActions)
                    Console.WriteLine("TROUBLE. random_action_distribution should be same length as num_actions.");

                var sumOfRandomActionDistribution = _randomActionDistribution.Sum();
                if (Math.Abs(sumOfRandomActionDistribution - 1.0) > doubleComparisonTollerance)
                    Console.WriteLine("TROUBLE. random_action_distribution should sum to 1!");
            }
            else
            {
                _randomActionDistribution = new List<double>();
            }

            // states that go into neural net to predict optimal action look as
            // x0,a0,x1,a1,x2,a2,...xt
            // this variable controls the size of that temporal window. Actions are
            // encoded as 1-of-k hot vectors
            _netInputs = numberOfStates * _temporalWindow + numberOfActions * _temporalWindow + numberOfStates;
            _numberOfStates = numberOfStates;
            _numberOfActions = numberOfActions;
            _windowSize = Math.Max(_temporalWindow, 2); // must be at least 2, but if we want more context even more
            _stateWindow = new List<Volume>();
            _actionWindow = new List<int>();
            _rewardWindow = new List<double>();
            _netWindow = new List<double[]>();

            // Init wth dummy data
            for (var i = 0; i < _windowSize; i++) _stateWindow.Add(new Volume(1, 1, 1));
            for (var i = 0; i < _windowSize; i++) _actionWindow.Add(0);
            for (var i = 0; i < _windowSize; i++) _rewardWindow.Add(0.0);
            for (var i = 0; i < _windowSize; i++) _netWindow.Add(new[] {0.0});

            // create [state -> value of all possible actions] modeling net for the value function
            var layerDefinitions = new List<LayerDefinition>();
            if (trainingOptions.LayerDefinitions != null)
            {
                // this is an advanced usage feature, because size of the input to the network, and number of
                // actions must check out. This is not very pretty Object Oriented programming but I can't see
                // a way out of it :(
                layerDefinitions = trainingOptions.LayerDefinitions;
                if (layerDefinitions.Count < 2) Console.WriteLine("TROUBLE! must have at least 2 layers");
                if (layerDefinitions[0].Type != "input") Console.WriteLine("TROUBLE! first layer must be input layer!");
                if (layerDefinitions[layerDefinitions.Count - 1].Type != "regression")
                    Console.WriteLine("TROUBLE! last layer must be input regression!");
                if (layerDefinitions[0].OutDepth * layerDefinitions[0].OutSx * layerDefinitions[0].OutSy != _netInputs)
                    Console.WriteLine(
                        "TROUBLE! Number of inputs must be num_states * temporal_window + num_actions * temporal_window + num_states!");
                if (layerDefinitions[layerDefinitions.Count - 1].NumberOfNeurons != _numberOfActions)
                    Console.WriteLine("TROUBLE! Number of regression neurons should be num_actions!");
            }
            else
            {
                // create a very simple neural net by default
                layerDefinitions.Add(new LayerDefinition {Type = "input", OutSx = 1, OutSy = 1, OutDepth = _netInputs});
                if (trainingOptions.HiddenLayerSizes != null)
                {
                    // allow user to specify this via the option, for convenience
                    var hl = trainingOptions.HiddenLayerSizes;
                    layerDefinitions.AddRange(hl.Select(t => new LayerDefinition
                    {
                        Type = "fc",
                        NumberOfNeurons = t,
                        Activation = "relu"
                    }));
                }
            }

            // Create the network
            _valueNetwork = new Net();
            _valueNetwork.makeLayers(layerDefinitions);

            // and finally we need a Temporal Difference Learning trainer!
            var options = new Options {LearningRate = 0.01, Momentum = 0.0, BatchSize = 64, L2Decay = 0.01};
            if (trainingOptions.options != null)
                options = trainingOptions.options; // allow user to overwrite this

            _temporalDifferencetrainer = new Trainer(_valueNetwork, options);

            // experience replay
            _experience = new List<Experience>();

            // various housekeeping variables
            _age = 0; // incremented every backward()
            _forwardPasses = 0; // incremented every forward()
            _epsilon = 1.0; // controls exploration exploitation tradeoff. Should be annealed over time
            //this.last_input = [];
            _averageRewardWindow = new TrainingWindow(1000, 10);
            _averageLossWindow = new TrainingWindow(1000, 10);
            Learning = true;
        }

        private int RandomAction()
        {
            // a bit of a helper function. It returns a random action
            // we are abstracting this away because in future we may want to 
            // do more sophisticated things. For example some actions could be more
            // or less likely at "rest"/default state.

            var action = _util.Random.Next(0, _numberOfActions);

            if (_randomActionDistribution.Count != 0)
            {
                // okay, lets do some fancier sampling:
                var p = _util.RandomDouble(0, 1.0);
                var cumprob = 0.0;
                for (var k = 0; k < _numberOfActions; k++)
                {
                    cumprob += _randomActionDistribution[k];
                    if (p < cumprob)
                    {
                        action = k;
                        break;
                    }
                }
            }

            return action;
        }

        private Action Policy(double[] s)
        {
            // compute the value of doing any action in this state
            // and return the argmax action and its value
            var svol = new Volume(1, 1, _netInputs) {W = s};
            var actionValues = _valueNetwork.forward(svol, false);
            var maxk = 0;
            var maxval = actionValues.W[0];
            for (var k = 1; k < _numberOfActions; k++)
                if (actionValues.W[k] > maxval)
                {
                    maxk = k;
                    maxval = actionValues.W[k];
                }
            return new Action {Action1 = maxk, Value = maxval};
        }

        private double[] GetNetInput(Volume xt)
        {
            // return s = (x,a,x,a,x,a,xt) state vector. 
            // It's a concatenation of last window_size (x,a) pairs and current state x
            var w = new List<double>();

            // start with current state and now go backwards and append states and actions from history temporal_window times
            w.AddRange(xt.W);

            var n = _windowSize;
            for (var k = 0; k < _temporalWindow; k++)
            {
                // state
                w.AddRange(_stateWindow[n - 1 - k].W);
                // action, encoded as 1-of-k indicator vector. We scale it up a bit because
                // we dont want weight regularization to undervalue this information, as it only exists once
                var action1Ofk = new double[_numberOfActions];
                for (var q = 0; q < _numberOfActions; q++) action1Ofk[q] = 0.0;
                action1Ofk[_actionWindow[n - 1 - k]] = 1.0 * _numberOfStates;
                w.AddRange(action1Ofk);
            }

            return w.ToArray();
        }

        public int SelectActionAccordingToPolicy(Volume inputArray)
        {
            // compute forward (behavior) pass given the input neuron signals from body
            _forwardPasses += 1;

            // create network input
            int action;
            double[] netInput;
            if (_forwardPasses > _temporalWindow)
            {
                // we have enough to actually do something reasonable
                netInput = GetNetInput(inputArray);
                if (Learning)
                    _epsilon = Math.Min(1.0,
                        Math.Max(_epsilonMin,
                            1.0 - (_age - _learningStepsBurnin) / (_learningStepsTotal - _learningStepsBurnin)));
                else
                    _epsilon = EpsilonTestTime; // use test-time value

                var randomDouble = _util.RandomDouble(0, 1);
                if (randomDouble < _epsilon)
                {
                    // choose a random action with epsilon probability
                    action = RandomAction();
                }
                else
                {
                    // otherwise use our policy to make decision
                    var maxact = Policy(netInput);
                    action = maxact.Action1;
                }
            }
            else
            {
                // pathological case that happens first few iterations 
                // before we accumulate window_size inputs
                netInput = new List<double>().ToArray();
                action = RandomAction();
            }

            // remember the state and action we took for backward pass
            _netWindow.RemoveAt(0);
            _netWindow.Add(netInput);
            _stateWindow.RemoveAt(0);
            _stateWindow.Add(inputArray);
            _actionWindow.RemoveAt(0);
            _actionWindow.Add(action);

            return action;
        }

        public void EvaluateAction(double reward)
        {
            _averageRewardWindow.Add(reward);

            _rewardWindow.RemoveAt(0);
            _rewardWindow.Add(reward);

            if (!Learning) return;

            // various book-keeping
            _age += 1;

            // it is time t+1 and we have to store (s_t, a_t, r_t, s_{t+1}) as new experience
            // (given that an appropriate number of state measurements already exist, of course)
            if (_forwardPasses > _temporalWindow + 1)
            {
                var experience = new Experience();
                var n = _windowSize;
                experience.PreviousState = _netWindow[n - 2];
                experience.ActionTaken = _actionWindow[n - 2];
                experience.RewardReceived = _rewardWindow[n - 2];
                experience.CurrentState = _netWindow[n - 1];

                if (_experience.Count < _experienceSize)
                {
                    _experience.Add(experience);
                }
                else
                {
                    // replace. finite memory!
                    var ri = _util.Random.Next(0, _experienceSize);
                    _experience[ri] = experience;
                }
            }

            // learn based on experience, once we have some samples to go on
            // this is where the magic happens...
            if (_experience.Count > _startLearnThreshold)
            {
                var avcost = 0.0;
                for (var k = 0; k < _temporalDifferencetrainer.BatchSize; k++)
                {
                    var re = _util.Random.Next(0, _experience.Count);
                    var experience = _experience[re];
                    var x = new Volume(1, 1, _netInputs)
                    {
                        W = experience.PreviousState
                    };
                    var maxact = Policy(experience.CurrentState);
                    var r = experience.RewardReceived + _gamma * maxact.Value;

                    var ystruct = new Entry {Dim = experience.ActionTaken, Val = r};
                    var loss = _temporalDifferencetrainer.Train(x, ystruct);
                    avcost += double.Parse(loss["loss"]);
                }

                avcost = avcost / _temporalDifferencetrainer.BatchSize;
                _averageLossWindow.Add(avcost);
            }
        }

        public string VisSelf()
        {
            var t = "";
            t += "experience replay size: " + _experience.Count + System.Environment.NewLine;
            t += "exploration epsilon: " + _epsilon + System.Environment.NewLine;
            t += "age: " + _age + System.Environment.NewLine;
            t += "average Q-learning loss: " + _averageLossWindow.get_average() + System.Environment.NewLine;
            t += "smooth-ish reward: " + _averageRewardWindow.get_average() + System.Environment.NewLine;

            return t;
        }
    }
}