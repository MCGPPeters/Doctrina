namespace Doctrina.Collections

    open Doctrina.Math.Pure.Structure.Algebra.Structures

    module Seq =
        let monoid<'T> =
            { new Monoid<seq<'T>>() with
                override this.Zero() = Seq.empty
                override this.Combine(a,b) = Seq.append a b
            }

        let foldMap (monoid: _ Monoid) f =
            Seq.fold (fun s e -> monoid.Combine(s, f e)) (monoid.Zero())
    
    module List =
                               
        /// List monoid
        let monoid<'T> =
            { new Monoid<'T list>() with
                override this.Zero() = []
                override this.Combine(a,b) = a @ b }

        let inline zip xs ys = 
            let rec loop xs ys =
                let a' = LanguagePrimitives.GenericZero
                let b' = LanguagePrimitives.GenericZero
                match (xs, ys) with
                | x::xs, y::ys -> (x, y) :: loop xs ys
                | [], ys -> List.zip (List.replicate ys.Length a') ys
                | xs, [] -> List.zip xs (List.replicate xs.Length b')
            loop xs ys 

    module Set =
        let monoid<'T when 'T : comparison> =
            { new Monoid<Set<'T>>() with
                override this.Zero() = Set.empty
                override this.Combine(a,b) = Set.union a b }        