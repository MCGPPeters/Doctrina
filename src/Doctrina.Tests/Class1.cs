using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Doctrina.Math;
using Doctrina.Tests.ModelFree.Control;
using Doctrina.Tests.ModelFree.Markov;

namespace Doctrina.Tests
{
    /// <summary>
    ///     A simple example for Reinforcement Learning using table lookup Q-learning method.
    ///     An agent "o" is on the left of a 1 dimensional world, the treasure is on the rightmost location.
    ///     Run this program and to see how the agent will improve its strategy of finding the treasure.
    /// </summary>
    public class TreaseHuntProperties
    {
    }

    /// The AGENT STATE, is the agents internal representation (the state it thinks its in) of the state of the environment based on the observations
    /// it received from that environment, via its sensory system (the perceived state of the world). This is the state that
    /// is used to pick the next action it will take using its actuator and with that influencing the environment (external) state
    /// The agent state is represented by the type parameter TState
    /// POLICY EVALUATION is the step in the thinking process of the agent in which it evaluates the action it took in the origin state
    /// that effected the environment to transition to the destination state. It does this by comparing the maximum the total reward it thinks it can
    /// receive from this state onward. It can do this in a number of ways. Monte Carlo methods do this by calculating and comparing it to the actual total reward it can retreive from the 
    /// current state onward to the end of the episode. Temporal Difference methods do this by estimating the total reward it can retrieve
    /// 
    /// POLICY IMPROVEMENT is the step in the thinking process of the agent in which it corrects its estimation of the value of the action it took in the previous state
    /// that made it end up in the current state
    /// 
    /// The ACTION VALUE FUNCTION estimates how good it is to take an action in a certain state so that it will maximize the total reward it can receive WHILE interacting with the environment
    public class Distribution<T> : Dictionary<T, Propability>
    {
        public Distribution(IDictionary<T, Propability> dictionary) : base(dictionary)
        {
            if (System.Math.Abs(dictionary.Values.Select(propability => propability.Value).Sum()) > 1f)
                throw new ArgumentException("The sum of all probabilities does not add up to 1");
        }
    }

    public static class DistributionExtensions
    {
        /// <summary>
        ///     Stochastic selection of a sample <typeparamref name="T" /> from the distribution according to the corresponding
        ///     probabilities
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="distribution">The distribution.</param>
        /// <param name="nextRandomDoubleBetweenZeroAndOne">The next random double between zero and one.</param>
        /// <returns></returns>
        public static T Sample<T>(this Distribution<T> distribution, Func<double> nextRandomDoubleBetweenZeroAndOne)
        {
            var p = nextRandomDoubleBetweenZeroAndOne();
            var cumulativeProbability = 0.0;
            var chosenSample = default(T);
            foreach (var propability in distribution)
            {
                cumulativeProbability += propability.Value;
                if (p <= cumulativeProbability)
                {
                    chosenSample = propability.Key;
                    break;
                }
            }
            return chosenSample;
        }
    }

    /// <summary>
    ///     Determines the expected / estimated return of being in <paramref name="state"></paramref> looking forward into the
    ///     expected future
    /// </summary>
    /// <typeparam name="TState"></typeparam>
    /// <param name="state">An n-tuple (object) containing all the features describing the agent state.</param>
    /// <returns></returns>
    public delegate Return StateValue<in TState>(TState state);

    /// <summary>
    ///     A policy π is a distribution over actions given a
    ///     <typeparam name="TState">state</typeparam>
    ///     . I.e.
    ///     the propability that the agent will take action A given being in a state S).
    ///     In the case of a deterministic policy, there will allways be 1 action that will be chosen
    ///     i.e. the distribution will contain 1 action that has value 1 and the rest of the actions will get 0 propability
    ///     In a stocastic policy, the will be a true distribution over actions. Since a deterministic policy is a special
    ///     case of a stocastic policy, the policy function is defined as such
    /// </summary>
    /// <typeparam name="TState"></typeparam>
    /// <param name="actionValues"></param>
    /// <returns></returns>
    public delegate Func<TState, Distribution<Action>> Policy<TState>(List<Transition<TState>> actionValues);

    public delegate Return Update(Return estimate, Alpha learningRate, Return target);

    /// <summary>
    ///     Determines the expected / estimated return of each action in <paramref name="actionSpace"></paramref> whil being in
    ///     <paramref name="state"></paramref>
    /// </summary>
    /// <typeparam name="TState"></typeparam>
    /// <param name="actionSpace"></param>
    /// <param name="state">An n-tuple (object) containing all the features describing the agent state.</param>
    /// <returns>A distribution of the value for each action in the action space</returns>
    public delegate List<Transition<TState>> ActionValueFunction<TState>(Action[] actionSpace, TState state);

    public static class _
    {
        /// <summary>
        ///     Adjusts the estimated return towards the error between the next estimated or actual return (target)
        ///     and the current estimated return given a step size α (alpha).
        /// </summary>
        /// <param name="estimate">The estimate to adjust.</param>
        /// <param name="learningRate">The step size or learning rate</param>
        /// <param name="target">The target.</param>
        /// <returns></returns>
        public static Return Update(this Return estimate, Alpha learningRate, Return target)
        {
            var error = target - estimate;
            return estimate + learningRate * error;
        }

        /// <summary>
        /// </summary>
        /// <param name="immediateReward"></param>
        /// <param name="lambda"></param>
        /// <param name="expectedFutureReturn"></param>
        /// <returns></returns>
        public static Return Target(Reward immediateReward, ProperFraction lambda, Return expectedFutureReturn) =>
            immediateReward + lambda * expectedFutureReturn;
    }

    public class Observation
    {
    }

    /// <summary>
    ///     A command may or may not be executed by an actuator (an action), because of failure etc...
    /// </summary>
    public class Action
    {
    }

    public class Agent<TState>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Agent{TState}" /> class. On-Policy control is a special case of
        ///     On-Policy control. So the method chooses the general case so that it can be specialized using parameters
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="actionValueFunction">The action value function.</param>
        /// <param name="behaviorPolicy">
        ///     The behavior policy μ. The policy the agent uses to choose the next action to take. Define
        ///     both policies as the same to do on policy control
        /// </param>
        /// <param name="targetPolicy">
        ///     The target policy π. The policy we use to evaluate the behavior policy (e.g. used to perform policy evaluation,
        ///     in some algorithms called a critic). It calculates the value for the policy improvement algorithm.
        ///     The expected return could be calculated in a number of ways, for instance:
        ///     - Q-learning => Calculates the expected return using a value function for each action that can be taken in the
        ///     destination state of a transition. It the chooses an action greedily (the action that gives us the maximum expected
        ///     return) and uses that return
        ///     - Policy Gradient => Does not use a value function. Chooses the action according to the policy gradient and uses
        ///     the expected return of that action.
        ///     - Sarsa => Calculates the expected return using a value function for each action that can be taken in the
        ///     destination state of a transition. It the chooses an action epsilon greedily
        ///     -
        /// </param>
        /// <param name="learningRate">
        ///     The learning rate. Most often this is a fixed value. But this could be a calculated value,
        ///     for instance when using MC updates, when the learning rate is 1/n, where n is the total number of steps in the
        ///     episode
        /// </param>
        /// <param name="update">The update.</param>
        /// <param name="experienceSize"></param>
        public Agent(DecisionProcess<TState> model, ActionValueFunction<TState> actionValueFunction,
            Policy<TState> behaviorPolicy, Policy<TState> targetPolicy, Alpha learningRate, Update update,
            int experienceSize)
        {
            Model = model;
            ActionValueFunction = actionValueFunction;
            BehaviorPolicy = behaviorPolicy;
            TargetPolicy = targetPolicy;
            LearningRate = learningRate;
            Update = update;
            ExperienceSize = experienceSize;
            Experience = new LinkedList<Transition<TState>>();
        }

        private DecisionProcess<TState> Model { get; }
        private ActionValueFunction<TState> ActionValueFunction { get; }
        public Policy<TState> BehaviorPolicy { get; }
        private Policy<TState> TargetPolicy { get; }
        public Alpha LearningRate { get; }
        public Update Update { get; }
        public int ExperienceSize { get; }
        private LinkedList<Transition<TState>> Experience { get; }

        private Func<Agent<TState>, Func<Transition<TState>, Observation, Task<Transition<TState>>>> CreateAgent
            => agent => (transition, observation) =>
            {
                var previousState = transition.Origin;
                var currentState = SetCurrentState(transition.Origin, observation);
                transition.Destination = currentState;

                if (agent.Experience.Count == agent.ExperienceSize)
                    //todo. determine what is an optimal way to use the replay memory memory efficiently
                    agent.Experience.RemoveLast();
                agent.Experience.AddFirst(transition);

                //when getting a new observation. determine the reward as a result of previously taken action
                var immediateReward = agent.Model.RewardFunction(previousState, currentState);

                //Determine action values. The action values are basically an encoding of what action the agent thinks will effect the environment in the most
                //advantagious way for the agent. The can been seen as potential / candidate transitions (effects on the environment) to choose from (an action will effect the transition of the environment)
                //What effect the choice will actually have on the environment is only known when the transition actually takes place. When that happens, the agent will evaluate its choice by looking
                //at what effect its chosen action actually had on the environment
                var candidateTransitions = agent.ActionValueFunction(Model.ActionSpace, currentState);
                var targetPolicyActionDistribution = agent.TargetPolicy(candidateTransitions)(currentState);
                var nextActionAccordingToTargetPolicy =
                    targetPolicyActionDistribution.Sample(new CryptoRandom().NextDouble);
                var expectedReturnWhenFollowingTargetPolicy = candidateTransitions
                    .Single(actionValue => actionValue.Action == nextActionAccordingToTargetPolicy).Return;

                // Policy evaluation
                var target = _.Target(immediateReward, Model.DiscountFactor, expectedReturnWhenFollowingTargetPolicy);
                transition.Return = Update(transition.Return, agent.LearningRate, target);
                //todo update value function so that it will more accurately reflect the value of the action the agent took in the origin state the affected the transition of the environment to the destination state

                //choose action according to propability
                var behaviorPolicyActionDistribution = BehaviorPolicy(candidateTransitions)(currentState);
                var nextActionAccordingToBehaviorPolicy =
                    behaviorPolicyActionDistribution.Sample(new CryptoRandom().NextDouble);
                var expectedReturnWhenFollowingBehaviorPolicy = candidateTransitions
                    .Single(actionValue => actionValue.Action == nextActionAccordingToTargetPolicy).Return;

                //todo execute action

                return Task.FromResult(new Transition<TState>(currentState, nextActionAccordingToBehaviorPolicy,
                    expectedReturnWhenFollowingBehaviorPolicy));
            };

        private TState SetCurrentState(TState modelCurrentState, Observation observation) =>
            throw new NotImplementedException();
    }

    namespace ModelFree
    {
        namespace Control
        {
            public class Transition<TState>
            {
                public Transition(TState origin, Action action, Return @return)
                {
                    Origin = origin;
                    Action = action;
                    Return = @return;
                }

                /// <summary>
                ///     The state the agent thinks it is in before taking an action
                /// </summary>
                public TState Origin { get; }

                /// <summary>
                ///     The state the agent thinks it is in after taking the action it chose in the origin state
                /// </summary>
                public TState Destination { get; set; }

                /// <summary>
                ///     The action the agent chose in the origin state which affected the transition of the environment to the destination
                ///     state
                /// </summary>
                public Action Action { get; }

                /// <summary>
                ///     The actual return the of the destination state after choosing the action of this transition in the origin state
                /// </summary>
                public Return Return { get; set; }
            }
        }

        namespace Markov
        {
            /// <summary>
            ///     A Partially Observable Markov Decision Process (POMDP), which is a generalization of a Markov Decision Process
            ///     (MDP)
            /// </summary>
            /// <typeparam name="TState"></typeparam>
            public class DecisionProcess<TState>
            {
                public DecisionProcess(TState initialState, IEnumerable<TState> stateSpace,
                    IObservable<Observation> observations, Action[] actionSpace,
                    Func<TState, TState, Reward> rewardFunction, Lambda discountFactor)
                {
                    CurrentState = initialState;
                    StateSpace = stateSpace;
                    Observations = observations;
                    ActionSpace = actionSpace;
                    RewardFunction = rewardFunction;
                    DiscountFactor = discountFactor;
                }

                public TState CurrentState { get; set; }
                public IEnumerable<TState> StateSpace { get; }
                private IObservable<Observation> Observations { get; }
                public Action[] ActionSpace { get; }
                public Func<TState, TState, Reward> RewardFunction { get; }
                public Lambda DiscountFactor { get; }
            }
        }
    }
}