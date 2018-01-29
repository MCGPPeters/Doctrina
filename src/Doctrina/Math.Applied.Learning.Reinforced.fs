namespace Math.Applied.Learning.Reinforced

open Doctrina
open Doctrina.Math.Applied.Probability

type Return = Return of float

type Action = Action of Undefined

type State<'a> = State of 'a

type Transition<'a> = {
    Origin : State<'a>
    Destination : State<'a>
    Action : Action
}

module Objective = 

    [<Measure>]
    type reward

    type Reward = Merit<float<reward>>
    type Reward<'a> = Objective<Transition<'a>, Reward>

type Policy<'a> = State<'a> -> Action

type Episode<'a> = Experiment<Policy<'a>>

type Experience<'state> = Experience of Transition<'state> list