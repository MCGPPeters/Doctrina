namespace Doctrina.Math.Applied.Learning.Reinforced

open Doctrina
open Doctrina.Math.Applied.Probability
open Doctrina.Math.Applied.Probability.Sampling

type Gamma = float
type Epsilon = float

type Reward = Reward of float

type Action<'a> = Action of 'a

type State<'s> = State of 's

type Origin<'s> = Origin of State<'s>

type Destination<'s> = Destination of State<'s>

type Transition<'s, 'a> = {
    Origin : Origin<'s>
    Destination : Destination<'s>
    Action : Action<'a>
}

type Policy<'s, 'a> = State<'s> -> Distribution<Action<'a>>

// The 
type Decision<'a> = Distribution<Action<'a>> -> Randomized<Action<'a>> option

type Episode<'s, 'a> = Experiment<Policy<'s, 'a>>

type Experience<'a> = Experience of Transition<'a> list

type Agent<'s, 'a> = State<'s> -> Randomized<Action<'a>> option

type Observation<'a> = Observation of 'a

type Environment<'s, 'a> = {
    States: State<'s> list
    ActionSpace: Action<'a> list
    ObservationSpace: Observation<'s>
    TransitionDynamics: (State<'s> * Action<'a>) -> Probability * State<'s>
}

module Agent =
        
    let create (policy: Policy<'s, 'a>) (decision: Decision<'a>) : Agent<'s, 'a> =
        policy >> decision

module Prediction =

    let return' (states: State<'s> list) reward (discount: Gamma) = 
        states
        |> List.mapi (fun time state -> (pown discount time) * reward(state)) 
        |> List.sum

//    let value     
//
//    let MonteCarlo = 

module Objective = 

    [<Measure>]
    type reward

    type Reward = Merit<float<reward>>
    type Reward<'a> = Benefit<Transition<'a>, Reward>



// An observation is a discrete representation of environment as perceived by the agent using its sensory system