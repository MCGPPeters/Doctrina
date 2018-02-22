type Probability = float
open System.Xml.XPath

type Event<'a> = Event of 'a * Probability

type Distribution< ^a when ^a : equality and ^a : (static member empty : ^a )
                    and ^a : (member Append : ^a -> ^a -> ^a )> = Distribution of 'a

module Distribution =
    type List<'T> with
        static member empty = []
    let certainly a = [Event (a, 1.0)] |> Distribution

    let inline join f (Distribution d) (Distribution d') =
            Distribution [for (x, p) in d do
                             for (y, q) in d' -> Event (f x y, p * q)]

module Monoid =

    open Distribution

    let empty<'a> = certainly List.empty

    let inline append (Distribution d) (Distribution d') = 
                 join map 