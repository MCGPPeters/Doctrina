namespace Doctrina.Math.Applied.Learning.Reinforced

open Doctrina.Math.Applied.Probability
open Doctrina.Math.Applied.Probability.Sampling
open Doctrina.Collections.List
open Doctrina.Math.Applied.Probability.Computation

type Gamma = float
type Epsilon = float
type Alpha = float

type Reward = Reward of float 

type Action<'a> = Action of 'a

type State<'s> = State of 's

type Return<'t> = Return of 't * float

type Utility<'s> = Utility of Expectation<Return<State<'s>>>

type Q<'s, 'a> = Q of Expectation<Return<State<'s> * Action<'a>>>

type Origin<'s> = Origin of State<'s>

type Destination<'s> = Destination of State<'s>

type Transition<'s, 'a> = {
    Origin : Origin<'s>
    Destination : Destination<'s>
    Action : Action<'a>
}

type Policy<'s, 'a> = Policy of (State<'s> -> Distribution<Action< ^a>>)

//type Episode<'s, 'a> = Experiment<Policy<'s, 'a>>

//type Experience< ^a> = Experience of Transition<'a> list

type Agent<'s, 'a> = State<'s> -> Action<'a>

type Observation<'a> = Observation of 'a


type Environment<'s, 'a> = {
    Dynamics: (State<'s> * Action<'a>) -> Randomized<(State<'s> * Reward * bool)>
    Discount: Gamma
}

module Agent =
        
    let inline create< 's, 'a > (policy: Policy<'s, 'a>) decide : Agent<'s, 'a> =
        let (Policy policy) = policy
        policy >> decide

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

    module TD0 =  

        let update<'s when 's : equality> utilities current (observation: (State<'s> * Reward)) (learningRate: Alpha) (discount: Gamma) =
            
            let (Utility ( Expectation( Return (_, currentReturn)))) = find utilities current
            let (Utility ( Expectation( Return (_, nextReturn)))) = find utilities (fst observation)

            let (Reward reward) = (snd observation)

            let currentReturn' = Utility ( Expectation( Return (current, currentReturn + learningRate * (reward + discount * (nextReturn - currentReturn)))))

            update (Utility ( Expectation( Return (current, currentReturn)))) currentReturn' utilities 

        let rec episode environment (agent: Agent<'s, 'a>) state utilities learningRate discount =

            let action = agent state   
            let (Randomized(next, reward, final)) = environment.Dynamics (state, action)
            let utilities = update utilities state (next, reward) learningRate discount
            match final with
            | true -> utilities
            | false -> episode environment agent next utilities learningRate discount       

    
module Control =

    let inline greedy distribution = distribution |> argMax

    module Sarsa =

        let update<'s, 'a when 's : equality and 'a : equality> (actionValues: Q<'s, 'a> list) (current: State<'s> * Action<'a>) (observation: ((State<'s> * Action<'a>) * Reward)) (learningRate: Alpha) (discount: Gamma) =

            actionValues |> findFirst  (fun (Q(Expectation(Return(x, _)))) -> x = current) 
                                    |> List.collect (fun (Q ( Expectation( Return (_, currentReturn)))) -> 
                                                        findFirst (fun (Q(Expectation(Return(x, _)))) -> x = fst observation) actionValues
                                                            |> List.collect (fun (Q ( Expectation( Return (_, nextReturn)))) ->
                                                                let (Reward reward) = snd observation
                                                                let currentReturn' = Q ( Expectation( Return (current, currentReturn + learningRate * (reward + discount * (nextReturn - currentReturn)))))
                                                                update (Q ( Expectation( Return (current, currentReturn)))) currentReturn' actionValues))



        // let rec episode<'s, 'a when 's: equality and 'a: equality> (environment: Environment<'s, 'a>) (observation: ((State<'s> * Action<'a>) * Reward)) policy actionValues learningRate discount =

        //     let agent = policy |> Agent.create 

        //     let action = agent (fst(fst observation))   
        //     match action with
        //     | Some (Randomized (Action action)) -> 
        //         match environment.Dynamics (state, Action action) with
        //         | Some (Randomized (next, reward, final)) ->
        //             let utilities = update actionValues state ((nextState, action), reward) learningRate discount
        //             match final with
        //             | true -> utilities
        //             | false -> episode environment agent next utilities learningRate discount 
        //         | None -> actionValues
        //     | None -> actionValues
                                                            

            
                        
            

                                              

module Objective = 

    [<Measure>]
    type reward

    type Reward = Merit<float<reward>>
    type Reward<'a> = Benefit<Transition<'a>, Reward>