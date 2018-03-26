namespace Doctrina.Math.Pure.Structure.Algebra.Structures.Properties

open FsCheck.Xunit
open Swensen.Unquote

module MonoidProperties =

    open Doctrina.Math.Pure.Algebra.Structures.Algebraic

    [<Property>]
    let ``Monoid binary operator is associative`` (monoid: _ Monoid) (x) (y) (z) =

        let append = monoid.Combine

        let a = append (append x y) z
        let b = append x (append y z)
        
        test <@ a = b @>

    [<Property>]
    let ``Monoid left identity`` (monoid: _ Monoid) a =
        
        let append = monoid.Combine
        let empty = monoid.Zero

        test<@ append a empty = a @>

    [<Property>]
    let ``Monoid right identity`` (monoid: _ Monoid) a =
        
        let append = monoid.Combine
        let empty = monoid.Zero

        test<@ append empty a = a @>