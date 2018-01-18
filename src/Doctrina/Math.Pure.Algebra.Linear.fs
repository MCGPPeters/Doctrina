namespace Doctrina.Math.Pure.Algebra.Linear

open Doctrina.Collections

type Vector< ^a when ^a : (static member (+) : ^a -> ^a -> ^a )
                    and ^a : (static member (*) : ^a -> ^a -> ^a ) > =
        | Vector of list< ^a >
        with
            static member inline (*) (alpha, Vector v) = 
                match alpha with
                | 0 -> Vector (List.map (fun _ -> 0) v)
                | _ -> 
                    match v with
                    | [] -> Vector []
                    | _ ->  Vector (List.map (fun a -> a * alpha) v)

            static member inline (*) (Vector v, alpha) = 
                match alpha with
                | 0 -> Vector (List.map (fun _ -> 0) v)
                | _ -> 
                    match v with
                    | [] -> Vector []
                    | _ ->  Vector (List.map (fun a -> a * alpha) v)

                
            static member inline (+) (Vector u, Vector v) = 
                match (u, v) with
                | [], ys -> Vector ys
                | xs , [] -> Vector xs
                | _, _ ->
                    List.zip u v 
                        |> List.map (fun element -> fst element + snd element)
                        |> Vector

            static member inline (~-) (Vector v) =                
                Vector (List.map (fun x -> -x) v)
                // can be rewriten using eta reduction as => Vector (List.map (~-) v)
                
            static member inline (-) (Vector u, Vector v) =
                Vector u + (- Vector v)

            /// Dot Product
            static member inline (.*) (Vector u, Vector v) =
                List.zip u v 
                |> List.map (fun x -> fst x * snd x)
                |> List.fold (+) LanguagePrimitives.GenericZero

            // Hadamard product
            static member inline (*) (Vector u, Vector v) =
                List.zip u v 
                |> List.map (fun x -> fst x * snd x)
                |> Vector