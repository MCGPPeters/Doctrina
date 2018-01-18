namespace Matematiko.Algebra.Linear.Tests

open FsCheck.Xunit
open Swensen.Unquote

module VectorProperties =

    open Doctrina.Math.Pure.Algebra.Linear

    [<Property>]
    let ``Vector addition is commutative`` (u : Vector<int>)  (v : Vector<int>) =
        let a = u + v
        let b = v + u
        test <@ a = b @>

    [<Property>]
    let ``Vector addition is associative`` (u : Vector<int>)  (v : Vector<int>) (w : Vector<int>) =
        let a = (u + v) + w
        let b = u + (v + w)
        test <@ a = b @>

    [<Property>]
    let ``Scalar vector multiplication is associative`` (alpha: int) (beta: int) (v : Vector<int>) =
        test <@ alpha * (beta * v) = (alpha * beta) * v @>

    [<Property>]
    let ``Scalar vector multiplication distributes over vector addition`` (alpha: int) (v : Vector<int>) (w : Vector<int>) =
        let a = alpha * (v + w)
        let b = (alpha * v) + (alpha * w)
        test <@ a = b @>

    // [<Property>]
    // let ``Vector subtraction and vector addition are inverses of each other``(v : Vector<Integer>) (w : Vector<Integer>) =
    //     let f v = v + w
    //     let g v = v - w
    //     let ``g>>f`` = g >> f
    //     test <@ ``g>>f`` v = v @>

    [<Property>]
    let ``Vector dot products are commutative``(v : Vector<int>) (w : Vector<int>) =
        let a = w .* v
        let b = v .* w
        test <@ a = b @>    

    [<Property>]
    let ``(Homogeneity of dot-product): Multiplying one of the vectors in the dot-product is equivalent to multiplying the value of the dotproduct`` (alpha: int) (v : Vector<int>) (w : Vector<int>) =
        let a = (alpha * v) * w
        let b = alpha * (v * w)
        test <@ a = b @>   
