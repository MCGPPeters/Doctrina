namespace Doctrina.Maze

open Doctrina.Math.Applied.Learning.Neural

type Robot = {
    Sensors: SensoryNeuron list
    Effectors: MotorNeuron list
}

type Position = int * int

// The characterization of behavior is defined by what it has achieved
type Achievement<'a> = Achievement of 'a

type Destination = Achievement<Position>

module Sensors =

    type Range = SensoryNeuron
    type Radar = SensoryNeuron

module Effectors =

    type Steering = MotorNeuron
    type Propulsion = MotorNeuron

type Tree =
    | Leaf of int * int
    | Node of int * int * Tree list




module Maze = 
 
    let rec buildMaze (x, y) =
        let shuffleArray array =
            let n = Array.length array
            for x = 1 to n do
                let i = n - x
                let j = random.Next(i)
                let tmp = array.[i]
                array.[i] <- array.[j]
                array.[j] <- tmp
            array
     
        let neighbors = 
            [| x - 1, y; 
               x + 1, y; 
               x, y - 1; 
               x, y + 1 |]
            |> shuffleArray
            |> Array.filter(fun (x, y) -> x >= 0 && x < width && 
                                          y >= 0 && y < height)
        let visited = [ for x, y in neighbors do
                            if not tiles.[y].[x] then
                                tiles.[y].[x] <- true
                                yield buildMaze (x, y) ]
        if List.length visited > 0 then
            Node(x, y, visited) 
        else
            Leaf(x, y)
 
    let maze = buildMaze (0, 0)
 
// let window = Window()
// window.Title <- "Maze generation"
// let grid = Grid()
// grid.Background <- SolidColorBrush(Colors.Black)
// window.Content <- grid
 
// for i = 0 to width * 2  do
//     let column = ColumnDefinition()
//     column.Width <- GridLength(9.6, GridUnitType.Pixel)
//     grid.ColumnDefinitions.Add(column)
// for i = 0 to height * 2 do
//     let row = RowDefinition()
//     row.Height <- GridLength(9.6, GridUnitType.Pixel)
//     grid.RowDefinitions.Add(row)
 
// let rec drawMaze prevx prevy node =
//     let addTile (x, y) =
//         let tile = Rectangle()
//         tile.Width <- 9.6
//         tile.Height <- 9.6
//         tile.Fill <- SolidColorBrush(Colors.White)
//         grid.Children.Add(tile) |> ignore
//         Grid.SetColumn(tile, x)
//         Grid.SetRow(tile, y)
 
//     let colorGrid x y =
//         let finalX, finalY = x * 2, y * 2
//         let interX = finalX + prevx - x
//         let interY = finalY + prevy - y
//         addTile (finalX, finalY)
//         if (interX <> finalX || interY <> finalY) then
//             addTile (interX, interY)
 
//     match node with
//     | Node(x, y, children) ->
//         colorGrid x y
//         List.iter (drawMaze x y) children
//     | Leaf(x, y) -> 
//         colorGrid x y
 
// drawMaze 0 0 maze

