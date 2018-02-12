#r @"C:\Users\maurice\.nuget\packages\newid\3.0.1\lib\netstandard1.6\NewId.dll"
#load @"..\Doctrina\Doctrina.Math.Pure.Algebra.Structure.fs"
#load @"..\Doctrina\Collections.fs"
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
open Doctrina.Math.Applied.Probability
open Doctrina.Math.Applied.Probability.Sampling
open Doctrina.Math.Applied.Probability.Distribution
open Doctrina.Math.Applied.Probability.Computation

let grid: Grid = {
    Dynamics = (fun (state: State<Tile>, action: Action<Move>) -> 
                    let (State state) = state
                    let unchanged = state.Position
                    let (Position (x, y)) = state.Position
                    Doctrina.Math.Applied.Probability.Computation.bind (match action with
                                                                                | Action Up -> Distribution [(Up, 0.8); (Right, 0.1); (Down, 0.0); (Left, 0.1)]
                                                                                | Action Right -> Distribution [(Up, 0.1); (Right, 0.8); (Down, 0.1); (Left, 0.0)]
                                                                                | Action Down -> Distribution [(Up, 0.0); (Right, 0.1); (Down, 0.8); (Left, 0.1)]
                                                                                | Action Left -> Distribution [(Up, 0.1); (Right, 0.0); (Down, 0.1); (Left, 0.8)]
                                                                                | Action Stay -> Distribution [(Up, 0.25); (Right, 0.25); (Down, 0.25); (Left, 0.25)]) 
                                                                                (fun action -> 
                                                                                            let next = match action with 
                                                                                                        | Left -> 
                                                                                                            Position (x - 1, y)
                                                                                                        | Right -> 
                                                                                                            Position (x + 1, y)
                                                                                                        | Up ->
                                                                                                            Position (x, y + 1)
                                                                                                        | Down -> 
                                                                                                            Position (x, y - 1)
                                                                                                        | Stay -> unchanged
                                                                                            match next with 
                                                                                            | Position (0, 0) -> certainly (State {Position = next; Kind = NonTerminal})
                                                                                            | Position (0, 1) -> certainly (State {Position = next; Kind = NonTerminal})
                                                                                            | Position (0, 2) -> certainly (State {Position = next; Kind = NonTerminal})
                                                                                            | Position (1, 0) -> certainly (State {Position = next; Kind = NonTerminal})
                                                                                            | Position (1, 1) -> certainly (State state)
                                                                                            | Position (1, 2) -> certainly (State {Position = next; Kind = NonTerminal})
                                                                                            | Position (2, 0) -> certainly (State {Position = next; Kind = NonTerminal})
                                                                                            | Position (2, 1) -> certainly (State {Position = next; Kind = NonTerminal})
                                                                                            | Position (2, 2) -> certainly (State {Position = next; Kind = NonTerminal})
                                                                                            | Position (3, 0) -> certainly (State {Position = next; Kind = NonTerminal})
                                                                                            | Position (3, 1) -> certainly (State {Position = next; Kind = Terminal})
                                                                                            | Position (3, 2) -> certainly (State {Position = next; Kind = Terminal})
                                                                                            | _ -> certainly (State state)) |> pick);
    Discount = 0.999;
    Reward = (fun (state: State<Tile>, action: Action<Move>) -> 
                let (State state) = state
                match state.Kind with 
                | Terminal when state.Position = Position (3, 1) -> -1.0
                | Terminal when state.Position = Position (3, 2) ->  1.0
                | _ -> -0.04)
}
let policy : Policy<Tile, Move> = 
    fun (State tile) ->
        match tile.Position with 
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
        | Position (3, 1) -> certainly (Action Stay)
        | Position (3, 2) -> certainly (Action Stay)
        | _ -> impossible
let agent = Agent.create policy pick
let epochs = 10
let remaining = epochs
let gamma = 0.999

evolve grid agent epochs remaining gamma []