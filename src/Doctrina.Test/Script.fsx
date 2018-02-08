#r @"C:\Users\Maurice\.nuget\packages\newid\3.0.1\lib\netstandard1.6\NewId.dll"
#load @"f:\dev\Doctrina\src\Doctrina\Doctrina.Math.Pure.Algebra.Structure.fs"
// #load @"d:\dev\Doctrina\src\Doctrina\Collections.fs"
#load @"f:\dev\Doctrina\src\Doctrina\Doctrina.fs"
#load @"f:\dev\Doctrina\src\Doctrina\Doctrina.Math.Pure.Structure.Order.fs"
#load @"f:\dev\Doctrina\src\Doctrina\Doctrina.Math.Pure.Numbers.fs"
#load @"f:\dev\Doctrina\src\Doctrina\Doctrina.Math.Pure.Change.Calculus.fs"
#load @"f:\dev\Doctrina\src\Doctrina\Hyperparameter.fs"

#load @"f:\dev\Doctrina\src\Doctrina\Doctrina.Math.Applied.Probability.fs"
// #load @"d:\dev\Doctrina\src\Doctrina\Doctrina.Math.Pure.Algebra.Linear.fs"
#load @"f:\dev\Doctrina\src\Doctrina\Doctrina.Math.Applied.Optimization.fs"
#load @"f:\dev\Doctrina\src\Doctrina\Doctrina.Math.Applied.Learning.fs"
#load @"f:\dev\Doctrina\src\Doctrina\Doctrina.Math.Applied.Learning.Evolutionary.fs"
#load @"f:\dev\Doctrina\src\Doctrina\Doctrina.Math.Applied.Learning.Reinforced.fs"
#load @"f:\dev\Doctrina\src\Doctrina\Caching.fs"
#load @"f:\dev\Doctrina\src\Doctrina\Doctrina.Math.Applied.Learning.Neural.fs"
#load @"f:\dev\Doctrina\src\Doctrina\Doctrina.Math.Applied.Learning.Evolutionary.Neural.fs"
#load "Environments.fs"

open Doctrina.Tests.Environment.Grid
open Doctrina.Math.Applied.Learning.Reinforced
open Doctrina.Math.Applied.Probability
open Doctrina.Math.Applied.Probability.Sampling

let rec episode grid (agent: Agent<Position, Move>) (result: Position * Reward) visits =
    let (position, _) = result
    let action = agent (State position)
    let result = match action with
                    | Some (Randomized action) -> 
                        let (Action action) = action
                        step grid position action
                    | None -> step grid position Stay
    match result with
    | Ok (position, reward) -> 
       match kind position with
       | Ok Terminal -> result :: visits |> List.rev
       | _ -> episode grid agent (position, reward) (result :: visits)
    | Error _ -> []

let rec evolve grid (agent: Agent<Position, Move>) epochs remaining utilities =
    let (position, reward) = 
        match reset grid with
        | Ok result -> result
        | _ -> (Position (0, 0), Reward 0.0)
    let visits = episode grid agent (position, reward) [] 
                    |> List.map (fun visit -> 
                                        match visit with   
                                         | Ok (position, reward) -> Some ((State position), reward)
                                         | _ -> None)
                    |> List.choose id                                             
    let utilities' = match utilities |> Map.tryFind position with
                       | None -> 
                            let (Expectation (Reward reward)) = Prediction.utility visits 0.999
                            Map.add position (reward, 1) utilities
                       | Some (reward, visitCount) -> 
                            utilities |> Map.add position (reward, visitCount + 1)                                            
    match remaining = 0 with
    | true -> utilities' 
            |> Map.map (fun position visit -> fst visit / float (snd visit))
    | _ -> evolve grid agent epochs (remaining - 1) utilities'

let grid = Grid.Create(3, 4)

let agent = Agent.create policy pick

let (position, reward) = 
    match reset grid with
    | Ok result -> result
    | _ -> (Position (0, 0), Reward 0.0)

let epochs = 1000
evolve grid agent epochs epochs Map.empty
//
//    
//
//    
//    agent 
//    (render grid) |> List.map (fun s -> printf "%s") 
//    step grid (Position (3 , 0)) Up