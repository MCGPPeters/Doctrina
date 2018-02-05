namespace Doctrina.Math.Applied.Probability

// A figure of merit is a quantity used to characterize the performance
open System
open System
type Merit<'a when 'a : comparison> = Merit of 'a

type Probability = float

type Expectation = Expectation of float

type Distribution<'a> = Distribution of ('a * Probability) list

type Transition<'a> = 'a -> Distribution<'a>

type Probabilistic<'a, 'b> = 'a -> Distribution<'b>

type Spread<'a> = 'a list -> Distribution<'a>

type Outcome<'a> = Outcome of 'a

// An event is a characterization of a list of outcomes, like "head occured within two coin flips" => 
type Event<'a> = Outcome<'a> list

// a sample space which is the set of all possible outcomes. I
type Samples<'a> = Samples of Outcome<'a> list

type Experiment<'a> = Outcome<'a> list -> Distribution<Event<'a>>

// is a function that maps an event or values of one or more variables 
// onto a real number intuitively representing some benefit gained associated with the event.
type Benefit<'a, 'TMerit when 'TMerit: comparison> = Event<'a> -> Merit<'TMerit>

type Sample< ^a when ^a: comparison > = 
    Sample of Set<'a>

module Distribution = 
    let uniform list : Distribution<'a> =
        let length = List.length list
        list 
        |>  List.map (fun e -> 
            (e , (1.0 / float length)))
        |> Distribution        

    let impossible : Distribution<'a> = Distribution []
    let certainly a : Distribution<'a> = Distribution [(a, 1.0)]

module Computation = 

    open Distribution

    let outcome = fst
    let probability = snd

    let inline return' a : Distribution<'a> =   
        certainly a

    let join f (Distribution d) (Distribution d') : Distribution<'c> =
            Distribution [for x in d do
                             for y in d' ->
                             (f (outcome x) (outcome y), (probability y) * (probability x))]

    let pair x y = (x, y) 
    

    // https://queue.acm.org/detail.cfm?id=3055303
    let inline bind (Distribution prior) (likelihood: 'a -> Distribution<'b>) : Distribution<'b> =
              Distribution [for x in prior do
                            let  (Distribution distribution) = (likelihood (outcome x))
                            for y in distribution ->
                               (outcome y, (probability y) * (probability x))] 

    ///mapD :: (a -> b) -> Dist a -> Dist b
    let inline map f (Distribution distribution) : Distribution<'b> = 
             Distribution [for x in distribution do
                              for y in f (outcome x) -> 
                              (y, probability x)]    

    let inline apply (Distribution fDistribution) (Distribution distribution) : Distribution<'b> = 
             Distribution [for x in distribution do
                              for f in fDistribution do 
                              for y in (fst f) (outcome x) ->
                              (y, probability x)]                   

    let inline filter (Distribution distribution) predicate : Distribution<'a> =
       distribution 
       |> List.filter predicate
       |> Distribution

    let inline (?) (predicate: 'a -> bool) (Distribution distribution) : Probability =
       distribution 
       |> List.filter (fun x -> predicate (fst x))
       |> List.sumBy snd

    let inline argMax (Distribution distribution) =
        distribution |> List.maxBy (fun x -> probability x)
        // can be rewriten using eta reduction as => distribution |> Seq.maxBy snd |> fst


    let inline (>>=) a b = bind a b

    type DistributionMonadBuilder() =
        member this.Bind (r, f) = bind r f
        member this.Return x = return' x
        member this.ReturnFrom m = m

    let probabilistic = DistributionMonadBuilder()

module Collections =

    open Computation
    open Distribution
    let selectOne collection = uniform [ for v in collection -> (v, List.filter (fun x -> x <> v) collection)]

    let rec selectMany n collection = 
        match n with
        | 0 -> return' ([], collection)
        | _ -> probabilistic {
                    let! (c1, x) = selectOne collection
                    let! (c2, xs) = selectMany (n - 1) x
                    return ((c1::c2), xs) }

    let inline select (n: int) = (selectMany n) >> map (fst >> List.rev)

    type Die = int

    let die = uniform [1..6]

    let rec dice numberOfThrows : Distribution<Die list> = 
        match numberOfThrows with
        | 0 -> certainly []
        | _ -> join (fun x y -> x :: y) (die) (dice (numberOfThrows - 1)) 
    
    // What is the probability of throwing 2 sixes with 2 dice
    let twoSixes = (fun x -> x = [6;6]) ? (dice 2)

    let gt3 = (fun x -> x > 3) ? (die);;

module Sampling = 

    type Randomized<'a> = Randomized of 'a

    let Guid = Randomized (Guid.NewGuid ())

    let rec scan (probability: Probability) (Distribution distribution) =
        match distribution with 
        | [] -> None 
        | (x, probability')::xs -> 
                match (probability <= probability') || xs = List.empty with
                | true -> Some (Randomized x)
                | _ -> scan (probability - probability') (Distribution xs) 

    let inline select distribution probability = 
        scan probability distribution

    let pick distribution = 
        let r = System.Random()
        select distribution (r.NextDouble())

    let random t = t >> pick    