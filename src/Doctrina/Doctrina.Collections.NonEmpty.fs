namespace Doctrina.Collection.NonEmpty

type NonEmpty< ^a> = 
        | Singleton of ^a
        | List of Head : ^a * Tail : ^a list
        with
            member inline this.ToList = 
                match this with
                | Singleton x -> [x]
                | List (head, tail) -> head :: tail
                          