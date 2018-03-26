#r @"C:\Users\maurice.peters\.nuget\packages\newid\3.0.1\lib\netstandard1.6\NewId.dll"
#load @"..\Doctrina\Doctrina.Collections.NonEmpty.fs"
#load @"..\Doctrina\Doctrina.fs"

#load @"..\Doctrina\Doctrina.Math.Applied.Probability.fs"
#load @"..\Doctrina\Doctrina.Math.Pure.Algebra.Structure.fs"
#load @"..\Doctrina\Doctrina.Math.Applied.Learning.fs"
#load @"..\Doctrina\Doctrina.Math.Applied.Learning.Reinforced.fs"
#load "Environments.fs"
#I @"C:\Users\maurice.peters\.nuget\packages\FSharp.Charting"
#load @"C:\Users\maurice.peters\.nuget\packages\fsharp.charting\0.91.1\lib\net45\FSharp.Charting.fsx"

open Doctrina.Tests.Environment.Grid
open Doctrina.Math.Applied.Learning.Reinforced
open Doctrina.Math.Applied.Learning.Reinforced.Control
open Doctrina.Math.Applied.Learning.Reinforced.Control.Sarsa
open Doctrina.Math.Applied.Probability
open Doctrina.Math.Applied.Probability.Sampling
open Doctrina.Math.Applied.Probability.Distribution
open Doctrina.Math.Applied.Probability
open Doctrina.Collections.NonEmpty
open Doctrina.Collections
open Doctrina.Math.Applied.Probability.Sampling
let rec evolve (grid: Grid) observation epochs policy decide (learningRate: Alpha) (discount: Gamma) qValues =

    let policyMatrix, qValues = step grid observation policy decide qValues learningRate discount

    match epochs = 1 with
    | true -> policyMatrix
    | _ -> evolve grid observation (epochs - 1) policyMatrix decide learningRate discount qValues

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
                            certainly (State position, Reward -0.04, false)
                        |> pick

    Discount = 0.999
}

let actionDistribution = uniform (List(Action Up, [Action Down; Action Left; Action Right; Action Idle]))

let pickAction actionDistribution = 
    let (Randomized action) = actionDistribution |> pick
    action

let initialPolicy = [
    (State(Position (0, 0)), Action Idle)
    (State(Position (0, 1)), Action Idle)
    (State(Position (0, 2)), Action Idle)
    (State(Position (1, 0)), Action Idle)
    (State(Position (1, 1)), Action Idle)
    (State(Position (1, 2)), Action Idle)
    (State(Position (2, 0)), Action Idle)
    (State(Position (2, 1)), Action Idle)
    (State(Position (2, 2)), Action Idle)
    (State(Position (3, 0)), Action Idle)
    (State(Position (3, 1)), Action Idle)
    (State(Position (3, 2)), Action Idle)]
let alpha = 0.001
let epochs = 5000
let gamma = 0.999
let observation = ((State (Position (0, 0)), Action Idle), Reward 0.0)
let QValues = [
        (Expectation(Return((State(Position (0, 0)), Action Up), 0.0)));
        (Expectation(Return((State(Position (0, 0)), Action Down), 0.0)));
        (Expectation(Return((State(Position (0, 0)), Action Left), 0.0)));
        (Expectation(Return((State(Position (0, 0)), Action Right), 0.0)));
        (Expectation(Return((State(Position (0, 0)), Action Idle), 0.0)));

        (Expectation(Return((State(Position (0, 1)), Action Up), 0.0)));
        (Expectation(Return((State(Position (0, 1)), Action Down), 0.0)));
        (Expectation(Return((State(Position (0, 1)), Action Left), 0.0)));
        (Expectation(Return((State(Position (0, 1)), Action Right), 0.0)));
        (Expectation(Return((State(Position (0, 1)), Action Idle), 0.0)));

        (Expectation(Return((State(Position (0, 2)), Action Up), 0.0)));
        (Expectation(Return((State(Position (0, 2)), Action Down), 0.0)));
        (Expectation(Return((State(Position (0, 2)), Action Left), 0.0)));
        (Expectation(Return((State(Position (0, 2)), Action Right), 0.0)));
        (Expectation(Return((State(Position (0, 2)), Action Idle), 0.0)));

        (Expectation(Return((State(Position (1, 0)), Action Up), 0.0)));
        (Expectation(Return((State(Position (1, 0)), Action Down), 0.0)));
        (Expectation(Return((State(Position (1, 0)), Action Left), 0.0)));
        (Expectation(Return((State(Position (1, 0)), Action Right), 0.0)));
        (Expectation(Return((State(Position (1, 0)), Action Idle), 0.0)));

        (Expectation(Return((State(Position (1, 1)), Action Up), 0.0)));
        (Expectation(Return((State(Position (1, 1)), Action Down), 0.0)));
        (Expectation(Return((State(Position (1, 1)), Action Left), 0.0)));
        (Expectation(Return((State(Position (1, 1)), Action Right), 0.0)));
        (Expectation(Return((State(Position (1, 1)), Action Idle), 0.0)));

        (Expectation(Return((State(Position (1, 2)), Action Up), 0.0)));
        (Expectation(Return((State(Position (1, 2)), Action Down), 0.0)));
        (Expectation(Return((State(Position (1, 2)), Action Left), 0.0)));
        (Expectation(Return((State(Position (1, 2)), Action Right), 0.0)));
        (Expectation(Return((State(Position (1, 2)), Action Idle), 0.0)));

        (Expectation(Return((State(Position (2, 0)), Action Up), 0.0)));
        (Expectation(Return((State(Position (2, 0)), Action Down), 0.0)));
        (Expectation(Return((State(Position (2, 0)), Action Left), 0.0)));
        (Expectation(Return((State(Position (2, 0)), Action Right), 0.0)));
        (Expectation(Return((State(Position (2, 0)), Action Idle), 0.0)));

        (Expectation(Return((State(Position (2, 1)), Action Up), 0.0)));
        (Expectation(Return((State(Position (2, 1)), Action Down), 0.0)));
        (Expectation(Return((State(Position (2, 1)), Action Left), 0.0)));
        (Expectation(Return((State(Position (2, 1)), Action Right), 0.0)));
        (Expectation(Return((State(Position (2, 1)), Action Idle), 0.0)));

        (Expectation(Return((State(Position (2, 2)), Action Up), 0.0)));
        (Expectation(Return((State(Position (2, 2)), Action Down), 0.0)));
        (Expectation(Return((State(Position (2, 2)), Action Left), 0.0)));
        (Expectation(Return((State(Position (2, 2)), Action Right), 0.0)));
        (Expectation(Return((State(Position (2, 2)), Action Idle), 0.0)));

        (Expectation(Return((State(Position (3, 0)), Action Up), 0.0)));
        (Expectation(Return((State(Position (3, 0)), Action Down), 0.0)));
        (Expectation(Return((State(Position (3, 0)), Action Left), 0.0)));
        (Expectation(Return((State(Position (3, 0)), Action Right), 0.0)));
        (Expectation(Return((State(Position (3, 0)), Action Idle), 0.0)));
        
        (Expectation(Return((State(Position (3, 1)), Action Up), 0.0)));
        (Expectation(Return((State(Position (3, 1)), Action Down), 0.0)));
        (Expectation(Return((State(Position (3, 1)), Action Left), 0.0)));
        (Expectation(Return((State(Position (3, 1)), Action Right), 0.0)));
        (Expectation(Return((State(Position (3, 1)), Action Idle), 0.0)));
        
        (Expectation(Return((State(Position (3, 2)), Action Up), 0.0)));
        (Expectation(Return((State(Position (3, 2)), Action Down), 0.0)));
        (Expectation(Return((State(Position (3, 2)), Action Left), 0.0)));
        (Expectation(Return((State(Position (3, 2)), Action Right), 0.0)));
        (Expectation(Return((State(Position (3, 2)), Action Idle), 0.0)))]       

let epsilon = 0.1;

let result = evolve grid observation 200000 initialPolicy (eGreedy epsilon) alpha gamma QValues  |> List.map (fun (position, action) -> match action with
                                                                                                                                          | Action Up -> position, "^"
                                                                                                                                          | Action Left -> position, "<"
                                                                                                                                          | Action Right -> position, ">"
                                                                                                                                          | Action Down -> position, "V"
                                                                                                                                          | Action Idle -> position ,"*")

open FSharp.Charting

Chart.Point( [ for (State(Position(x, y)), move) in result -> (x, y) ], null, null , [for (State(Position(x, y)), move) in result -> move])
 
