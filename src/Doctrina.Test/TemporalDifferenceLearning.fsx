#r @"C:\Users\maurice\.nuget\packages\newid\3.0.1\lib\netstandard1.6\NewId.dll"
#load @"..\Doctrina\Doctrina.Math.Pure.Algebra.Structure.fs"
#load @"..\Doctrina\Collections.fs"
#load @"..\Doctrina\Doctrina.Collections.NonEmpty.fs"
#load @"..\Doctrina\Doctrina.fs"
#load @"..\Doctrina\Doctrina.Math.Pure.Structure.Order.fs"
#load @"..\Doctrina\Doctrina.Math.Pure.Numbers.fs"
#load @"..\Doctrina\Doctrina.Math.Pure.Change.Calculus.fs"
#load @"..\Doctrina\Hyperparameter.fs"

#load @"..\Doctrina\Doctrina.Math.Applied.Probability.fs"
// #load @"d:\dev\Doctrina\src\Doctrina\Doctrina.Math.Pure.Algebra.Linear.fs"
#load @"..\Doctrina\Doctrina.Math.Applied.Optimization.fs"
#load @"..\Doctrina\Doctrina.Math.Applied.Learning.fs"
#load @"..\Doctrina\Doctrina.Math.Applied.Learning.Evolutionary.fs"
#load @"..\Doctrina\Doctrina.Math.Applied.Learning.Reinforced.fs"
#load @"..\Doctrina\Caching.fs"
#load @"..\Doctrina\Doctrina.Math.Applied.Learning.Neural.fs"
#load @"..\Doctrina\Doctrina.Math.Applied.Learning.Evolutionary.Neural.fs"
#load "Environments.fs"

open Doctrina.Tests.Environment.Grid
open Doctrina.Math.Applied.Learning.Reinforced
open Doctrina.Math.Applied.Learning.Reinforced.Prediction.TD0
open Doctrina.Math.Applied.Probability
open Doctrina.Math.Applied.Probability.Sampling
open Doctrina.Math.Applied.Probability.Distribution
open Doctrina.Math.Applied.Probability.Computation
open Doctrina.Math.Applied.Probability
open Doctrina.Collections
open Doctrina.Collections.List
open Doctrina.Math.Applied.Probability.Sampling
let rec evolve (grid: Grid) (agent: Agent<Position, Move>) epochs (learningRate: Alpha) (discount: Gamma) (utilities: Utility<Position> list) =
    let state = State (Position (0, 0))  

    let utilities = episode grid agent state utilities learningRate discount

    match epochs = 1 with
    | true -> utilities
    | _ -> evolve grid agent (epochs - 1) learningRate discount utilities

let grid: Grid = {
    Dynamics = fun (state, action) -> 
                    match state with
                    | (State position) -> 
                        let (Position (x, y)) = position
                        let reaction = match action with
                                        | Action Up -> Distribution (List(Event(Action Up, 0.8),[Event (Action Right, 0.1); Event (Action Down, 0.0); Event (Action Left, 0.1)]))
                                        | Action Right -> Distribution (List(Event (Action Up, 0.1), [Event (Action Right, 0.8); Event (Action Down, 0.1); Event (Action Left, 0.0)]))
                                        | Action Down -> Distribution (List(Event (Action Up, 0.0), [Event (Action Right, 0.1); Event (Action Down, 0.8); Event (Action Left, 0.1)]))
                                        | Action Left -> Distribution (List(Event (Action Up, 0.1), [Event (Action Right, 0.0); Event (Action Down, 0.1); Event (Action Left, 0.8)]))
                                        | Action Idle -> Distribution (List(Event (Action Up, 0.0), [Event (Action Right, 0.0); Event (Action Down, 0.0); Event (Action Left, 0.0)]))
                        let foo = reaction |> pick                                        
                        let next = match foo with 
                                    | Randomized (Action Left) -> 
                                        Position (x - 1, y)
                                    | Randomized (Action Right) -> 
                                        Position (x + 1, y)
                                    | Randomized (Action Up) ->
                                        Position (x, y + 1)
                                    | Randomized (Action Down) -> 
                                        Position (x, y - 1)
                                    | Randomized (Action Idle) -> position 
                        match next with 
                        | Position (0, 0) -> certainly (State next, Reward -0.04, false)
                        | Position (0, 1) -> certainly (State next, Reward -0.04, false)
                        | Position (0, 2) -> certainly (State next, Reward -0.04, false)
                        | Position (1, 0) -> certainly (State next, Reward -0.04, false)
                        | Position (1, 1) -> certainly (State position, Reward -0.04, false)                                                                                       
                        | Position (1, 2) -> certainly (State next, Reward -0.04, false)
                        | Position (2, 0) -> certainly (State next, Reward -0.04, false)
                        | Position (2, 1) -> certainly (State next, Reward -0.04, false)
                        | Position (2, 2) -> certainly (State next, Reward -0.04, false)
                        | Position (3, 0) -> certainly (State next, Reward -0.04, false)
                        | Position (3, 1) -> certainly (State next, Reward -1.0, true)                                                                                           
                        | Position (3, 2) -> certainly (State next, Reward 1.0, true)                                                                                           
                        | _ -> 
                            //printfn "%A" next
                            certainly (State position, Reward -0.04, false)
                        |> pick                                                                                                        
                        
    Discount = 0.999
}

let policy = 
    Policy(fun (State position) ->
                match position with
                | Position (0, 0) -> certainly (Action Up)
                | Position (0, 1) -> certainly (Action Up)
                | Position (0, 2) -> certainly (Action Right)
                | Position (1, 0) -> certainly (Action Left)
                | Position (1, 1) -> certainly (Action Up)
                | Position (1, 2) -> certainly (Action Right)
                | Position (2, 0) -> certainly (Action Left)
                | Position (2, 1) -> certainly (Action Up)
                | Position (2, 2) -> certainly (Action Right)
                | Position (3, 0) -> certainly (Action Left)
                | Position (3, 1) -> certainly (Action Idle)
                | Position (3, 2) -> certainly (Action Idle)
                | _ -> certainly (Action Idle))

let agent = Agent.create policy (fun x -> 
                                    let (Randomized action) = x |> pick
                                    action) 
let alpha = 0.1
let epochs = 5000
let gamma = 0.999
let state = State(Position (0, 0))
let utilities = [
        Utility (Expectation ( Return (State (Position (0, 0)), 0.0)));
        Utility (Expectation ( Return (State (Position (0, 1)), 0.0)));
        Utility (Expectation ( Return (State (Position (0, 2)), 0.0)));
        Utility (Expectation ( Return (State (Position (1, 0)), 0.0)));
        Utility (Expectation ( Return (State (Position (1, 1)), 0.0)));
        Utility (Expectation ( Return (State (Position (1, 2)), 0.0)));
        Utility (Expectation ( Return (State (Position (2, 0)), 0.0)));
        Utility (Expectation ( Return (State (Position (2, 1)), 0.0)));
        Utility (Expectation ( Return (State (Position (2, 2)), 0.0)));
        Utility (Expectation ( Return (State (Position (3, 0)), 0.0)));
        Utility (Expectation ( Return (State (Position (3, 1)), 0.0)));
        Utility (Expectation ( Return (State (Position (3, 2)), 0.0)));
        ]

evolve grid agent 100000 alpha gamma utilities 
                                                                                            
