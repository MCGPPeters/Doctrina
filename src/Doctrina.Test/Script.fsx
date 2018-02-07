open Doctrina.Math.Applied.Learning.Reinforced
open Doctrina.Math.Applied.Probability.Sampling
#r @"C:\Users\maurice.peters\.nuget\packages\newid\3.0.1\lib\netstandard1.6\NewId.dll"
#load @"d:\dev\Doctrina\src\Doctrina\Doctrina.Math.Pure.Algebra.Structure.fs"
// #load @"d:\dev\Doctrina\src\Doctrina\Collections.fs"
#load @"d:\dev\Doctrina\src\Doctrina\Doctrina.fs"
#load @"d:\dev\Doctrina\src\Doctrina\Doctrina.Math.Pure.Structure.Order.fs"
#load @"d:\dev\Doctrina\src\Doctrina\Doctrina.Math.Pure.Numbers.fs"
#load @"d:\dev\Doctrina\src\Doctrina\Doctrina.Math.Pure.Change.Calculus.fs"
#load @"d:\dev\Doctrina\src\Doctrina\Hyperparameter.fs"

#load @"d:\dev\Doctrina\src\Doctrina\Doctrina.Math.Applied.Probability.fs"
// #load @"d:\dev\Doctrina\src\Doctrina\Doctrina.Math.Pure.Algebra.Linear.fs"
#load @"d:\dev\Doctrina\src\Doctrina\Doctrina.Math.Applied.Optimization.fs"
#load @"d:\dev\Doctrina\src\Doctrina\Doctrina.Math.Applied.Learning.fs"
#load @"d:\dev\Doctrina\src\Doctrina\Doctrina.Math.Applied.Learning.Evolutionary.fs"
#load @"d:\dev\Doctrina\src\Doctrina\Doctrina.Math.Applied.Learning.Reinforced.fs"
#load @"d:\dev\Doctrina\src\Doctrina\Caching.fs"
#load @"d:\dev\Doctrina\src\Doctrina\Doctrina.Math.Applied.Learning.Neural.fs"
#load @"d:\dev\Doctrina\src\Doctrina\Doctrina.Math.Applied.Learning.Evolutionary.Neural.fs"
#load "Environments.fs"

open Doctrina.Tests.Environment.Grid

let act observation 

let cleaningRobot = 
    let grid = Grid.Create(3, 4)
    let agent = Agent.create policy pick

    let ok (position, reward) = reset grid

    let rec evolve result epochs episodes =
        let (position, reward) = result
        let action = agent position 
        let result = step grid (State position) action
        match epochs = 0 with
        | true -> episodes::[result]
        | _ -> episodes::(evolve result (epochs - 1) episodes) 

    

    
    agent 
    (render grid) |> List.map (fun s -> printf "%s") 
    step grid (Position (3 , 0)) Up