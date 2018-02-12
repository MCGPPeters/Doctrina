namespace Doctrina.Tests

open Doctrina.Math.Applied.Learning.Reinforced
open Doctrina.Math.Applied.Probability
open Doctrina.Collections.List

module Environment =

    module Grid =

        type X = int
        type Y = int

        type Position = Position of X * Y

        type Kind =
        | Terminal
        | NonTerminal
        | Obstacle

        type Tile = {
            Position: Position
            Kind: Kind
        }

        type Move = 
        | Left
        | Right
        | Up
        | Down
        | Stay

        type Grid = Environment<Tile, Move> 

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

        let rec utilities (visits: (State<Tile> * float) list) (discount: Gamma) (returns: Return<Tile> list) =
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

            let utilities' = alligned
                                |> List.map (fun x -> 
                                                match x with
                                                | (Some (Return (state, value)), Some (Return (_, value'))) -> Return (state, value + value')
                                                | (Some r, None) -> r
                                                | (None, Some r) -> r)
                                |> List.groupBy (fun (Return (state, _)) -> state)
                                |> List.map (fun (key, values) -> (key, values |> List.sumBy (fun (Return(_, value)) -> value)))
                                |> List.map (fun x -> Return x)                                            

            match remaining = 1 with
            | true -> utilities' 
                        |> List.map (fun (Return (state, value)) -> Expectation (Return (state, value / float epochs)))
            | _ -> evolve grid agent epochs (remaining - 1) discount utilities'