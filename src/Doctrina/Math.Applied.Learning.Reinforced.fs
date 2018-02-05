namespace Math.Applied.Learning.Reinforced

open Doctrina
open Doctrina.Math.Applied.Probability
open Doctrina.Math.Pure.Numbers
open Doctrina.Math.Pure.Numbers

type Gamma = float
type Epsilon = float

type Reward = Reward of float

type Action = Action of Undefined

type State<'a> = State of 'a

type Origin<'a> = Origin of State<'a>

type Destination<'a> = Destination of State<'a>

type Transition<'a> = {
    Origin : Origin<'a>
    Destination : Destination<'a>
    Action : Action
}

type Policy<'a> = State<'a> -> Distribution<Action>

type Episode<'a> = Experiment<Policy<'a>>

type Experience<'a> = Experience of Transition<'a> list

type Agent<'a> = {
    Policy: Policy<'a>
    Experience: Experience<'a>
}

type Observation<'a> = Observation of 'a

type Environment<'a> = {
    States: State<'a> list
    ActionSpace: Action list
    ObservationSpace: Observation<'a>
    TransitionDynamics: (State<'a> * Action) -> Probability * State<'a>
}

module Simulation = 

     let episode (environment: Environment<'a>) (agents: Agent<'a> list) =
        fun action state ->
                let next = environment.TransitionDynamics state


module Prediction =

    let return' (states: State<'a> list) reward (discount: Gamma) = 
        states
        |> List.mapi (fun time state -> (pown discount time) * reward(state)) 
        |> List.sum

    let value     

    let MonteCarlo = 

module Objective = 

    [<Measure>]
    type reward

    type Reward = Merit<float<reward>>
    type Reward<'a> = Benefit<Transition<'a>, Reward>



// An observation is a discrete representation of environment as perceived by the agent using its sensory system
 

module Environment = 

    let step action = 