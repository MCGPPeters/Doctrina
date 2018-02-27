namespace Doctrina.Tests

open Doctrina.Math.Applied.Learning.Reinforced
open Doctrina.Math.Applied.Probability.Distribution
open Doctrina.Collections
open Doctrina.Math.Applied.Probability.Sampling

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
        | Idle
        // with
        //     static member inline Zero = Idle


        type Grid = Environment<Position, Move> 

        