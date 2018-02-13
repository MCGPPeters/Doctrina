namespace Doctrina.Tests

open Doctrina.Math.Applied.Learning.Reinforced

module Environment =

    module Grid =

        type X = int
        type Y = int

        type Position = Position of X * Y

        type Move = 
        | Left
        | Right
        | Up
        | Down
        | Stay

        type Grid = Environment<Position, Move> 