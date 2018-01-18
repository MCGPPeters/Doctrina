namespace Doctrina.Learning.Reinforced

open Doctrina

type Reward = Reward of float

type Return = Return of float

type Action = Action of Undefined

type Transition<'state> = {
    Origin: 'state
    Destination: 'state
    Action: Action
    Return: Return
}

type Experience<'state> = Experience of Transition<'state> list