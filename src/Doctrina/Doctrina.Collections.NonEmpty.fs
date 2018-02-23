namespace Doctrina.Collection.NonEmpty

type NonEmpty< ^a> = 
        | Singleton of ^a
        | List of Head : ^a * Tail : ^a list
        with
            member inline this.ToList = 
                match this with
                | Singleton x -> [x]
                | List (head, tail) -> head :: tail

module NonEmpty =

    let inline append (x: NonEmpty<'a>) (y: NonEmpty<'a>) = 
        match x, y with
        | Singleton x, Singleton y -> List (x, [y])
        | Singleton x, List (y, ys) -> List (x, y::ys)
        | List (x, xs), Singleton y -> List (x, y::xs |> List.rev)
        | List (x, xs), List(y, ys) -> List (x, y::xs |> List.rev |> List.append ys)

    let inline (@) (x: NonEmpty<'a>) (y: NonEmpty<'a>) = append x y   

    let inline map (f: 'a -> 'b) (nonEmpty: NonEmpty<'a>) : NonEmpty<'b> =
        match nonEmpty with
        | Singleton x -> (Singleton (f x))
        | List (x, xs) -> 
            let list = (f x) :: List.map f xs 
            List(List.head list, List.tail list)

    let inline bind (nonEmpty: NonEmpty<'a>) (f: 'a -> NonEmpty<'b>) : NonEmpty<'b> =
        match nonEmpty with
        | Singleton x -> f x
        | List (x, xs) -> xs |> List.map f |> List.fold append (f x)
            
    type NonEmptyMonadBuilder() =
        member __.Bind (r, f) = bind r f
        member __.Return x = Singleton x
        member __.ReturnFrom m = m

    let nonEmpty = NonEmptyMonadBuilder()                
                          