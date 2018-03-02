namespace Doctrina

type Undefined = exn

type Ordering = LT | EQ | GT

module Maybe =

    type MaybeBuilder() =

        member __.Bind(x, f) = 
            match x with
            | None -> None
            | Some a -> f a

        member __.Return(x) = 
            Some x
       
    let maybe = MaybeBuilder()