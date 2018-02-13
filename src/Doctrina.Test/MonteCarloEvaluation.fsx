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
open Doctrina.Math.Applied.Probability
open Doctrina.Collections.List
open Doctrina.Math.Applied.Probability.Sampling
let rec episode (grid: Grid) (agent: Agent<Tile, Move>) state visits =
    let action = agent state
    let result = match action with
                    | Some (Randomized action) -> (grid.Dynamics (state, action))
                    | None -> (grid.Dynamics (state, Action Stay))                 
    match result with
    | (Some (Randomized state)) -> 
       let (State tile) = state
       match tile.Kind with
       | Terminal -> (state, grid.Reward (state, Action Stay)) :: visits |> List.rev
       | _ -> episode grid agent (state) ((state, grid.Reward (state, Action Stay)) :: visits)
    | None -> []

let rec utilities (visits: (State<Tile> * Reward) list) (discount: Gamma) (returns: Return<Tile> list) =
    match visits with
    | [] -> returns |> List.rev
    | (tile, _)::xs 
        -> match returns |> List.tryFind (fun (Return (state, _)) -> tile = state) with
           | None -> 
                match xs with
                | [] -> 
                    utilities xs discount returns
                | _ -> let g = Prediction.return' tile (xs |> List.map snd) discount
                       utilities xs discount ( g :: returns)
           | Some _ -> utilities xs discount returns           
let rec evolve (grid: Grid) (agent: Agent<Tile, Move>) epochs remaining (discount: Gamma) (returns: Return<Tile> list) =
    let state = State {Position = Position (0, 0); Kind = NonTerminal}    
    let visits = episode grid agent state []

    let alligned = allign (utilities visits discount []) returns (fun (Return ( _, y)) -> y)

    let returns' = alligned
                        |> List.map (fun x -> 
                                        match x with
                                        | (Some (Return (state, value)), Some (Return (_, value'))) -> Return (state, value + value')
                                        | (Some r, None) -> r
                                        | (None, Some r) -> r)
                        |> List.groupBy (fun (Return (state, _)) -> state)
                        |> List.map (fun (key, values) -> (key, values |> List.sumBy (fun (Return(_, value)) -> value)))
                        |> List.map (Return)                                            

    match remaining = 1 with
    | true -> returns' 
                |> List.map (fun (Return (state, value)) -> Expectation (Return (state, value / float epochs)))
    | _ -> evolve grid agent epochs (remaining - 1) discount returns'

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
let epochs = 50000
let remaining = epochs
let gamma = 0.999

evolve grid agent epochs remaining gamma []