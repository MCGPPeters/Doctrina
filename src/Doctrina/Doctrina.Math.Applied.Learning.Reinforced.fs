namespace Doctrina.Math.Applied.Learning.Reinforced

open Doctrina.Math.Applied.Probability
open Doctrina.Math.Applied.Probability.Sampling

type Gamma = float
type Epsilon = float

type Reward = Reward of float
type Return = Return of float

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

type Environment<'s, 'a> = (State<'s> * Action<'a>) -> Distribution<State<'s>>

module Agent =
        
    let create (policy: Policy<'s, 'a>) (decision: Decision<'a>) : Agent<'s, 'a> =
        policy >> decision

module Prediction =

    let return' (visits: (State<'s> * Reward) list) (discount: Gamma) = 
        visits
            |> List.mapi (fun tick (_, reward) -> 
                            let (Reward r) = reward
                            (pown discount tick) * r)
            |> List.sum
            |> Return                    

module Objective = 

    [<Measure>]
    type reward

    type Reward = Merit<float<reward>>
    type Reward<'a> = Benefit<Transition<'a>, Reward>