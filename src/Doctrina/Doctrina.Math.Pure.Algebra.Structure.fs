namespace Doctrina.Math.Pure.Algebra.Structures.Algebraic

/// Groupoid (set with binary operation)
type Groupoid<'a> =
    /// <summary>
    /// Binary operation
    /// </summary>
    abstract Combine : 'a -> 'a -> 'a

/// Semigroup (set with associative binary operation)
type Semigroup<'a> =
    /// <summary>
    /// Associative operation
    /// </summary>
    inherit Groupoid<'a>
    
/// Monoid (associative binary operation with identity)
type Monoid<'a> =
    /// <summary>
    /// Identity
    /// </summary>
    abstract Zero : 'a

    inherit Semigroup<'a>

module Monoid =
    let inline sum () = 
        { new Monoid<_> with
            member __.Zero = LanguagePrimitives.GenericZero
            member __.Combine a b = a + b }  

    let inline product () =
        { new Monoid<_> with
            member __.Zero = LanguagePrimitives.GenericOne
            member __.Combine a b = a * b }          

namespace Doctrina.Math.Pure.Algebra.Structures.Categorical

module Functor = 
    let inline mapF builder f x = (^F: (member Map: ('e -> 'c) -> 'd -> 'f) (builder,f, x))

module Monad = 
    let inline Return builder x = (^M: (member Return: 'b -> 'c) (builder, x))
    let inline Bind builder m f = (^M: (member Bind: 'd -> ('e -> 'c) -> 'c) (builder, m, f))
    






