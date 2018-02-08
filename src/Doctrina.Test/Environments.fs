namespace Doctrina.Tests

open Doctrina.Math.Applied.Learning.Reinforced
open Doctrina.Math.Applied.Probability
open Doctrina.Math.Applied.Probability.Distribution

module Environment =

    module Grid =

        type X = int
        type Y = int

        type Position = Position of X * Y

        type Grid = 
            private Grid of Position list
                static member Create upperRight = 
                    let xs = [0..(fst upperRight)]
                    let ys = [0 .. (snd upperRight)]
                    [for x in xs do
                     for y in ys do yield Position (x, y)]
                     |> Grid

        type Kind =
        | Terminal
        | NonTerminal
        | Obstacle

        type Move = 
        | Left
        | Right
        | Up
        | Down
        | Stay

        let kind position =
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

        let reward position =
            match kind position with 
            | Ok Terminal when position = Position (3, 1) -> Ok (position, Reward -1.0)
            | Ok Terminal when position = Position (3, 2) -> Ok (position, Reward 1.0)
            | Ok _ -> Ok (position, Reward -0.04)
            | Error x -> Error x

        // The transition probabilities T(s' | s, a)
        // Given an action of the actor the environment reponds with the actual effect the action had
        let effect action = 
            match action with
            | Up -> Distribution [(Up, 0.55); (Right, 0.25); (Down, 0.1); (Left, 0.1)]
            | Right -> Distribution [(Up, 0.25); (Right, 0.25); (Down, 0.25); (Left, 0.25)]
            | Down -> Distribution [(Up, 0.3); (Right, 0.2); (Down, 0.4); (Left, 0.1)]
            | Left -> Distribution [(Up, 0.1); (Right, 0.2); (Down, 0.1); (Left, 0.6)]
            | Stay -> certainly Stay

        let move (Grid grid) (Position (x, y)) action =
            let unchanged = Position (x, y)
            let next = match action with   
                        | Left -> Position (x - 1, y)
                        | Right -> Position (x + 1, y)
                        | Up -> Position (x, y + 1)
                        | Down -> Position (x, y - 1)
                        | Stay -> Position (x, y)

            match grid |> List.contains next with
            | true -> match kind next with
                        | Ok Terminal | Ok NonTerminal -> next
                        | Ok Obstacle -> unchanged
                        | Error e -> unchanged
            | false -> unchanged                                          
           
        open Doctrina.Math.Applied.Probability.Sampling

        let (>>=) x y = Result.bind y x
        let step (grid: Grid) position action = 
            let effect = effect action |> pick
            match effect with
            | Some (Randomized effect) -> 
                move grid position effect 
                    |> reward
            | None -> Error "Unable to pick an action"
    
        let reset grid = step grid (Position (0, 0)) Stay                                

        let render (Grid grid) = 
            grid 
                |> List.map (fun position -> 
                        match kind position with
                        | Ok Terminal -> "*"
                        | Ok NonTerminal -> "-"
                        | _ -> "?")                   
