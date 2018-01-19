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

type Reward<'a> = Objective<Transition<'a>>

type Policy<'a> = State<'a> -> Action

type Episode<'a> = Experiment<Policy<'a>>

type Experience<'state> = Experience of Transition<'state> list