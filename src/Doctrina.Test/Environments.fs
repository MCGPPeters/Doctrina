namespace Doctrina.Tests

open Doctrina.Math.Applied.Learning.Reinforced

module Environment =

    module Grid =

        

        type X = int
        type Y = int

        type Position = Position of X * Y

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

        type Grid = Grid of Environment<Position * Kind, Move> 

        let reward grid position =
            match grid position with 
            | Ok Terminal when position = Position (3, 1) -> Ok (position, Reward -1.0)
            | Ok Terminal when position = Position (3, 2) -> Ok (position, Reward 1.0)
            | Ok _ -> Ok (position, Reward -0.04)
            | Error x -> Error x

        let move grid (Position (x, y)) action =
            let unchanged = Position (x, y)
            let next = match action with   
                        | Left -> Position (x - 1, y)
                        | Right -> Position (x + 1, y)
                        | Up -> Position (x, y + 1)
                        | Down -> Position (x, y - 1)
                        | Stay -> unchanged

            match grid next with
            | Ok Terminal | Ok NonTerminal -> next
            | Ok Obstacle -> unchanged
            | Error _ -> unchanged                                      
           
        open Doctrina.Math.Applied.Probability.Sampling

        let step grid position action dynamics = 
            let effect = dynamics action |> pick
            match effect with
            | Some (Randomized effect) -> 
                move grid position effect 
                    |> reward grid
            | None -> Error "Unable to pick an action"
    
        let reset grid effect = step grid (Position (0, 0)) Stay effect                               