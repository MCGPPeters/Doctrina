namespace Doctrina

open Doctrina.Math.Pure.Numbers
open Doctrina.Math.Pure.Structure.Order

/// <summary>
///     Step size or learning rate. A rational number in the closed interval [0, 1]
/// </summary>
type Alpha 
    = private Alpha of float
        static member Create n = 
            match n with
            | Interval.Closed 0.0 1.0 -> Ok (Alpha n)
            | _ -> Error "Alpha must be rational number in the closed interval [0, 1]"

/// <summary>
///     Discount factor
/// </summary>
type Gamma = Gamma of ProperFraction

