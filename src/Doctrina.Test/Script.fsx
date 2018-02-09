#r @"C:\Users\maurice.peters\.nuget\packages\newid\3.0.1\lib\netstandard1.6\NewId.dll"
#load @"..\Doctrina\Doctrina.Math.Pure.Algebra.Structure.fs"
// #load @"d:\dev\Doctrina\src\Doctrina\Collections.fs"
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

let g: Grid = Grid (fun (state: State<Position * Kind>, action: Action<Move>) -> 
                    let (State state) = state
                    let (Position (x, y), kind) = state
                    let unchanged = Position (x, y)
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
                                                                                            | Position (0, 0) -> certainly (State (next, NonTerminal))
                                                                                            | Position (0, 1) -> certainly (State (next, NonTerminal))
                                                                                            | Position (0, 2) -> certainly (State (next, NonTerminal))
                                                                                            | Position (1, 0) -> certainly (State (next, NonTerminal))
                                                                                            | Position (1, 1) -> certainly (State (next, Obstacle))
                                                                                            | Position (1, 2) -> certainly (State (next, NonTerminal))
                                                                                            | Position (2, 0) -> certainly (State (next, NonTerminal))
                                                                                            | Position (2, 1) -> certainly (State (next, NonTerminal))
                                                                                            | Position (2, 2) -> certainly (State (next, NonTerminal))
                                                                                            | Position (3, 0) -> certainly (State (next, NonTerminal))
                                                                                            | Position (3, 1) -> certainly (State (next, Terminal))
                                                                                            | Position (3, 2) -> certainly (State (next, Terminal))
                                                                                            | _ -> certainly (State (unchanged, kind))))
let grid position =
    match position with 
    | Position (0, 0) -> Ok NonTerminal
    | Position (0, 1) -> Ok NonTerminal
    | Position (0, 2) -> Ok NonTerminal
    | Position (1, 0) -> Ok NonTerminal
    | Position (1, 1) -> Ok Obstacle
    | Position (1, 2) -> Ok NonTerminal
    | Position (2, 0) -> Ok NonTerminal
    | Position (2, 1) -> Ok NonTerminal
    | Position (2, 2) -> Ok NonTerminal
    | Position (3, 0) -> Ok NonTerminal
    | Position (3, 1) -> Ok Terminal
    | Position (3, 2) -> Ok Terminal
    | _ -> Error ("Non existent position " + position.ToString())
let rec episode grid (agent: Agent<Position, Move>) (result: Position * Reward) visits dynamics =
    let (position, _) = result
    let action = agent (State position)
    let result = match action with
                    | Some (Randomized action) -> 
                        let (Action action) = action
                        step grid position action dynamics
                    | None -> step grid position Stay dynamics
    match result with
    | Ok (position, reward) -> 
       match grid position with
       | Ok Terminal -> result :: visits |> List.rev
       | _ -> episode grid agent (position, reward) (result :: visits) dynamics
    | Error _ -> []

let rec utilities (visits: (State<'s> * Reward) list) (discount: Gamma) utils = 
    let (Return r) = Prediction.return' visits discount
    match visits with
    | [] -> utils
    | (position, _)::xs 
        -> match utils |> Map.tryFind position with
           | None -> 
                utilities xs discount (utils |> Map.add position (r, 1)) 
           | Some (utility, visitCount) -> 
                utilities xs discount (utils |> Map.add position (utility + r, visitCount + 1))                            

let rec evolve grid (agent: Agent<Position, Move>) epochs remaining (discount: Gamma) utils dynamics =
    let (position, reward) = 
        match reset grid dynamics with
        | Ok result -> result
        | _ -> (Position (0, 0), Reward 0.0)
    let visits = episode grid agent (position, reward) [] dynamics
                    |> List.map (fun visit -> 
                                        match visit with   
                                         | Ok (position, reward) -> Some ((State position), reward)
                                         | _ -> None)
                    |> List.choose id 
                                                       
    let utilities' = utilities visits discount utils

    match remaining = 1 with
    | true -> utilities' 
                |> Map.map (fun position visit -> ((fst visit) / (float (snd visit)), snd visit))
    | _ -> evolve grid agent epochs (remaining - 1) discount utilities' dynamics

let policy : Policy<Position, Move> = 
    fun position ->
        match position with 
        | State (Position (0, 0)) -> certainly (Action Up)
        | State (Position (0, 1)) -> certainly (Action Up)
        | State (Position (0, 2)) -> certainly (Action Right)
        | State (Position (1, 0)) -> certainly (Action Left)
        | State (Position (1, 1)) -> certainly (Action Up)
        | State (Position (1, 2)) -> certainly (Action Right)
        | State (Position (2, 0)) -> certainly (Action Left)
        | State (Position (2, 1)) -> certainly (Action Up)
        | State (Position (2, 2)) -> certainly (Action Right)
        | State (Position (3, 0)) -> certainly (Action Left)
        | State (Position (3, 1)) -> certainly (Action Stay)
        | State (Position (3, 2)) -> certainly (Action Stay)
        | _ -> impossible
let agent = Agent.create policy pick

// The transition dynamics T(s' | s, a)
// Given an action of the actor the environment reponds with the actual effect the action had
let dynamics action = 
    match action with
    | Up -> Distribution [(Up, 0.8); (Right, 0.1); (Down, 0.0); (Left, 0.1)]
    | Right -> Distribution [(Up, 0.1); (Right, 0.8); (Down, 0.1); (Left, 0.0)]
    | Down -> Distribution [(Up, 0.0); (Right, 0.1); (Down, 0.8); (Left, 0.1)]
    | Left -> Distribution [(Up, 0.1); (Right, 0.0); (Down, 0.1); (Left, 0.8)]
    | Stay -> Distribution [(Up, 0.25); (Right, 0.25); (Down, 0.25); (Left, 0.25)]

  

let (position, reward) = 
    match reset grid dynamics with
    | Ok result -> result
    | _ -> (Position (0, 0), Reward 0.0)

let epochs = 5000
let gamma = 0.999

evolve grid agent epochs epochs gamma Map.empty dynamics