namespace Doctrina.Math.Applied.Learning.Reinforced

open Doctrina.Math.Applied.Probability
open Doctrina.Math.Applied.Probability.Sampling

type Gamma = float
type Epsilon = float

type Reward = float 

type Action<'a> = Action of 'a

type State<'s> = State of 's

type Return<'s> = Return of State<'s> * float

type Utility<'s> = Utility of State<'s> * Expectation<Return<'s>>

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
    Dynamics: (State<'s> * Action<'a>) -> Randomized<State<'s>> option
    Discount: Gamma
    Reward: (State<'s> * Action<'a>) -> Reward
}

module Agent =
        
    let create (policy: Policy<'s, 'a>) (decision: Decision<'a>) : Agent<'s, 'a> =
        policy >> decision

module Prediction =

    let return' state (rewards: Reward list) (discount: Gamma) = 
        let r = rewards
                |> List.mapi (fun tick reward -> 
                               (pown discount tick) * float reward)
                |> List.sum
        Return (state, r)                   

module Objective = 

    [<Measure>]
    type reward

    type Reward = Merit<float<reward>>
    type Reward<'a> = Benefit<Transition<'a>, Reward>