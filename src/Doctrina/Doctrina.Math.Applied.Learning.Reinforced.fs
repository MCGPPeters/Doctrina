namespace Doctrina.Math.Applied.Learning.Reinforced

open Doctrina.Math.Applied.Probability
open Doctrina.Math.Applied.Probability.Sampling

type Gamma = float
type Epsilon = float
type Alpha = float

type Reward = Reward of float 

type Action<'a> = Action of 'a

type State<'s> = State of 's

type Return<'s> = Return of State<'s> * float

type Utility<'s> = Utility of Expectation<Return<'s>>

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
    Dynamics: (State<'s> * Action<'a>) -> Randomized<State<'s> * Reward * bool> option
    Discount: Gamma
}

module Agent =
        
    let create (policy: Policy<'s, 'a>) (decision: Decision<'a>) : Agent<'s, 'a> =
        policy >> decision

module Prediction =

    let return' state (rewards: Reward list) (discount: Gamma) = 
        let r = rewards
                |> List.mapi (fun tick (Reward reward) -> 
                               (pown discount tick) * reward)
                |> List.sum
        Return (state, r)                   

    let rec find (utilities: Utility<'s> list) (state: State<'s>) =
         match utilities with
            | [] -> (Utility (Expectation (Return (state, 0.0))))
            | (Utility (Expectation (Return (s, value ))))::_ when s = state -> (Utility (Expectation (Return (state, value ))))
            | _::xs -> find xs state

    let update<'s when 's : equality> (utilities: Utility<'s> list) (current: State<'s>) (observation: (State<'s> * Reward)) (learningRate: Alpha) (discount: Gamma) =
        
        let (Utility ( Expectation( Return (_, currentReturn)))) = find utilities current
        let (Utility ( Expectation( Return (_, nextReturn)))) = find utilities (fst observation)

        let (Reward reward) = (snd observation)

        let currentReturn' = Utility ( Expectation( Return (current, currentReturn + learningRate * (reward + discount * (nextReturn - currentReturn)))))

        Doctrina.Collections.List.update (Utility ( Expectation( Return (current, currentReturn)))) currentReturn' utilities 

    let rec episode<'s, 'a when 's: equality> (environment: Environment<'s, 'a>) (agent: Agent<'s, 'a>) state utilities learningRate discount =

        let action = agent state   
        match action with
        | Some (Randomized (Action action)) -> 
            match environment.Dynamics (state, Action action) with
            | Some (Randomized (next, reward, final)) ->
                let utilities = update utilities state (next, reward) learningRate discount
                match final with
                | true -> utilities
                | false -> episode environment agent next utilities learningRate discount 
            | None -> utilities
        | None -> utilities        

    

                                              

module Objective = 

    [<Measure>]
    type reward

    type Reward = Merit<float<reward>>
    type Reward<'a> = Benefit<Transition<'a>, Reward>