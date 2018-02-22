namespace Doctrina.Collections

    // module Seq =
    //     let monoid<'T> =
    //         { new Monoid<seq<'T>>() with
    //             override this.Zero() = Seq.empty
    //             override this.Combine(a,b) = Seq.append a b
    //         }

    //     let foldMap (monoid: _ Monoid) f =
    //         Seq.fold (fun s e -> monoid.Combine(s, f e)) (monoid.Zero())
    
    type NonEmpty<'a> = NonEmpty of 'a * 'a list
    
    module List =

        let rec findFirst predicate list =
            match list with
            | [] -> []
            | x::_ when predicate x -> [x]
            | _::xs -> findFirst predicate xs

        let rec update original replacement list = 
            match list with
            | [] -> []
            | x::xs when x = original -> replacement :: xs
            | x::xs -> x :: update original replacement xs

        let rec allign xs ys compareBy =
            let xs = xs |> List.sortBy compareBy
            let ys = ys |> List.sortBy compareBy
            match (xs, ys) with
            | [], ys -> List.map (fun z -> (None, Some z)) ys 
            | xs, [] -> List.map (fun z -> (None, Some z)) xs
            | x :: xs, y :: ys -> 
                match (x, y) with
                | (xl, yl) when compareBy xl = compareBy yl -> (Some x, Some y) :: allign xs ys compareBy
                | (xl, yl) when compareBy xl < compareBy yl -> (Some x, None) :: allign xs (y::ys) compareBy
                | (xl, yl) when compareBy xl > compareBy yl -> (None, Some y) :: allign (x::xs) ys compareBy
                | _ -> []
                              
        /// List monoid
        // let monoid<'T> =
        //     { new Monoid<'T list>() with
        //         override this.Zero() = []
        //         override this.Combine(a,b) = a @ b }

        let inline zipZero xs ys zeroX zeroY = 
            let rec loop xs ys =
                match (xs, ys) with
                | x::xs, y::ys -> (x, y) :: loop xs ys
                | [], ys -> List.zip (List.replicate ys.Length zeroX) ys
                | xs, [] -> List.zip xs (List.replicate xs.Length zeroY)
            loop xs ys 

        let inline zip xs ys = zipZero xs ys LanguagePrimitives.GenericZero LanguagePrimitives.GenericZero       

    // module Set =
    //     let monoid<'T when 'T : comparison> =
    //         { new Monoid<Set<'T>>() with
    //             override this.Zero() = Set.empty
    //             override this.Combine(a,b) = Set.union a b }        