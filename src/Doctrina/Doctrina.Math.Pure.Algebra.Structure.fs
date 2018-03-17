namespace Doctrina.Math.Pure.Structure.Algebra.Structures

module Groupoid =

    let inline appendG builder a b = (^S: (member AppendG: 'a -> 'a -> 'a) (builder, a, b))

type Semigroup<'a> = 

    // An associative operation
    abstract (<->) : 'a * 'a -> 'a

module Monoid =

    let inline identity : 
    let inline (<->) builder a b = (^S: (member (<->): 'a -> 'a -> 'a) (builder, a, b))

module Functor = 
    let inline mapF builder f x = (^F: (member Map: ('e -> 'c) -> 'd -> 'f) (builder,f, x))

module Monad = 
    let inline Return builder x = (^M: (member Return: 'b -> 'c) (builder, x))
    let inline Bind builder m f = (^M: (member Bind: 'd -> ('e -> 'c) -> 'c) (builder, m, f))
    






