namespace Math.Applied.Learning.Reinforced

open Doctrina
open Doctrina.Math.Applied.Probability

type Return = Return of float

type Action = Action of Undefined

type State<'a> = State of 'a

type Origin<'a> = Origin of State<'a>

type Destination<'a> = Destination of State<'a>

type Transition<'a> = {
    Origin : Origin<'a>
    Destination : Destination<'a>
    Action : Action
}

module Objective = 

    [<Measure>]
    type reward

    type Reward = Merit<float<reward>>
    type Reward<'a> = Benefit<Transition<'a>, Reward>

type Policy<'a> = State<'a> -> Distribution<Action>

type Episode<'a> = Experiment<Policy<'a>>

type Experience<'a> = Experience of Transition<'a> list

type Agent<'a> = {
    Policy: Policy<'a>
    Experience: Experience<'a>
}

// An observation is a discrete representation of environment as perceived by the agent using its sensory system
type Observation<'a> = Observation of 'a

type Environment<'a> = {
    ActionSpace: Action list
    ObservationSpace: Observation<'a>
} 

module Environment = 

    let step action = 