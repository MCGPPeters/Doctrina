namespace Doctrina.Math.Applied.Learning.Reinforced

open Doctrina.Math.Applied.Probability
open Doctrina.Math.Applied.Probability.Sampling
open Doctrina.Math.Applied.Probability.Computation
open Doctrina.Math.Applied.Probability.Distribution
open Doctrina.Collections

type Gamma = float
type Epsilon = float
type Alpha = float

type Reward = Reward of float 

type Action<'a> = Action of 'a

type State<'s> = State of 's

type Return<'t> = Return of 't * float

type V<'s> = V of 's

type Q<'s, 'a> = Q of 's * 'a

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

module TD = 
    let update (learningRate: Alpha) target (current: Return<'s>) =
        let (Return (state, value)) = current
        Expectation(Return (state, value + learningRate * (target - value)))

    let target (reward: Reward) (discount: Gamma) nextValue = 
        let (Return(state, value)) = nextValue
        let (Reward r) = reward
        let expected = r + (discount * value)
        Expectation( Return (state, expected))

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

    let rec find utilities (state: State<'s>) =
        match utilities with
        | [] -> (state, 0.0)
        | (s, value)::_ when s = state -> (state, value)
        | _::xs -> find xs state

    module TD0 =  

        let rec step environment (agent: Agent<'s, 'a>) state utilities learningRate discount =

            let action = agent state   
            let (Randomized(next, reward, final)) = environment.Dynamics (state, action)
            let (Expectation nextReturn) = utilities |> List.find (fun (Expectation (Return (s, _))) -> s = next)
            let (Expectation currentReturn) = utilities |> List.find (fun (Expectation (Return (s, _))) -> s = state)
            let (Expectation(Return(_, target))) = TD.target reward discount nextReturn
            let utilities' = utilities 
                             |> List.map (fun (Expectation (Return (s, value))) -> match s = state with 
                                                                                   | true -> (TD.update learningRate target currentReturn)  
                                                                                   | false -> (Expectation (Return(s, value))))
            match final with
            | true -> utilities'
            | false -> step environment agent next utilities' learningRate discount     

    
module Control =

    let inline greedy distribution = 
        let (Event (x, _)) = (distribution |> argMax)
        x

    let inline eGreedy epsilon distribution = 
        let uniform = uniform (List(0.0,[0.1..1.0]))
        let (Randomized sigma) = uniform |> pick
        match sigma > epsilon with
        | true -> let (Event (x, _)) = (distribution |> argMax)
                  x
        | false -> let (Randomized x) = distribution |> pick
                   x    

    let createPolicy initialPolicy = 
        Policy(fun (State position) -> 
            let map = Map.ofList initialPolicy
            map.[position])    

    module Sarsa =

        let rec step<'s, 'a when 's: equality and 's : comparison and 'a: equality> (environment: Environment<'s, 'a>) (observation: ((State<'s> * Action<'a>) * Reward)) policyMatrix decide qValues learningRate discount =

            let ((state, _), _) = observation
            let action = policyMatrix |> List.find (fun (s, _) -> s = state) |> snd      

            let (Randomized (next, reward, terminal)) = environment.Dynamics (state, action)

            let observation' = ((next, action), reward)
            let nextAction = policyMatrix |> List.find (fun (s, _) -> s = next) |> snd 

            let (Expectation(q)) = qValues 
                                   |> List.find (fun (Expectation(Return((s, a), _))) -> (s, a) = (state, action)) 
            let (Expectation(qt1)) = qValues 
                                     |> List.find (fun (Expectation(Return((s, a), _))) -> (s, a) = (next, nextAction)) 

            let (Expectation(Return(_, target))) = TD.target reward discount qt1
            let newQValue = TD.update learningRate target q                                  
            let qValues' = qValues 
                           |> List.map (fun (Expectation(Return((s, a), value))) -> match (s, a) = (state, action) with 
                                                                                    | true -> newQValue  
                                                                                    | false -> Expectation(Return ((s,a), value)))
            let (Expectation(Return((_, bestAction), _))) = qValues' 
                                                            |> List.filter (fun (Expectation(Return((s, _), _))) -> s = state) 
                                                            |> List.maxBy (fun (Expectation(Return ((_,_), value))) -> value)  

            let policyMatrix' = policyMatrix |> List.map (fun (s, a) -> match state = s with
                                                                        | true -> (s, bestAction)
                                                                        | false -> (s, a))                                                                            
            
            match terminal with
            | true -> policyMatrix', qValues'
            | false -> step environment observation' policyMatrix' decide qValues' learningRate discount 

module Objective = 

    [<Measure>]
    type reward

    type Reward = Merit<float<reward>>
    type Reward<'a> = Benefit<Transition<'a>, Reward>