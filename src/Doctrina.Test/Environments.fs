open Math.Applied.Learning.Reinforced
open Doctrina.Math.Applied.Probability

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

        type Kind =
        | Terminal
        | NonTerminal
        | Obstacle

        type Action =
        | Left
        | Right
        | Up
        | Down

        let kind position =
            match position with 
            | Position (0, 0) -> Ok NonTerminal
            | Position (0, 1) -> Ok NonTerminal
            | Position (0, 2) -> Ok NonTerminal
            | Position (0, 3) -> Ok NonTerminal
            | Position (1, 0) -> Ok NonTerminal
            | Position (1, 1) -> Ok Obstacle
            | Position (1, 2) -> Ok NonTerminal
            | Position (1, 3) -> Ok Terminal
            | Position (2, 0) -> Ok NonTerminal
            | Position (2, 1) -> Ok NonTerminal
            | Position (2, 2) -> Ok NonTerminal
            | Position (2, 3) -> Ok Terminal
            | _ -> Error "Non existent state"

        let reward position =
            match kind position with 
            | Ok Terminal when position = Position (1, 3) -> Ok (Reward -1.0)
            | Ok Terminal when position = Position (2, 3) -> Ok (Reward 1.0)
            | Ok _ -> Ok (Reward -0.04)
            | Error x -> Error x

        // The transition probabilities T(s' | s, a)
        // Given an action of the actor the environment reponds with the actual effect the action had
        let effect action = 
            match action with
            | Up -> Distribution [(Up, 0.55); (Right, 0.25); (Down, 0.1); (Left, 0.1)]
            | Right -> Distribution [(Up, 0.25); (Right, 0.25); (Down, 0.25); (Left, 0.25)]
            | Down -> Distribution [(Up, 0.3); (Right, 0.2); (Down, 0.4); (Left, 0.1)]
            | Left -> Distribution [(Up, 0.1); (Right, 0.2); (Down, 0.1); (Left, 0.6)]

        let move grid (Position (x, y)) action =
            let unchanged = Position (x, y)
            let next = match action with   
                        | Left -> Position (x - 1, y)
                        | Right -> Position (x, x + 1)
                        | Up -> Position (x + 1, y)
                        | Down -> Position (x, y - 1)

            match grid |> List.contains next with
            | true -> match kind next with
                        | Ok Terminal | Ok NonTerminal -> Ok next
                        | Ok Obstacle -> Ok unchanged
                        | Error e -> Error e
            | false -> Ok unchanged                                          
           
        open Doctrina.Math.Applied.Probability.Sampling

        let (>>=) x y = Result.bind y x
        
        let evolve (Grid grid) position action = 
            let effect = effect action |> pick
            match effect with
            | Some (Randomized effect) -> 
                (move grid position effect) 
                    >>= reward 
                    >>= fun reward -> Ok (position, reward)
            | None -> Error "Unable to pick an action"          


