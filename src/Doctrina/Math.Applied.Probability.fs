namespace Doctrina.Math.Applied.Probability

type Probability = float

type Distribution<'a> = ('a * Probability) list

type Probabilistic<'a, 'b> = 'a -> Distribution<'b>

type Spread<'a> = 'a list -> Distribution<'a>

type Event<'a> = 'a -> bool

type Sample< ^a when ^a: comparison > = 
    Sample of Set<'a>

module Distribution = 

    let uniform seq : Distribution<'a> =
        let length = List.length seq
        seq 
        |> List.map (fun e -> 
            (e , (1.0 / float length)))

    let impossible : Distribution<'a> = []
        

    let certainly a : Distribution<'a> = [(a, 1.0)]

module Computation = 

    open Distribution

    let outcome = fst
    let probability = snd

    let inline return' a : Distribution<'a> =   
        certainly a

    let join f (d: Distribution<'a>) (d': Distribution<'b>) : Distribution<'c> =
            [for x in d do
             for y in d' do
             yield (f (outcome x) (outcome y), (probability y) * (probability x))]

    let pair x y = (x, y) 
    

    // https://queue.acm.org/detail.cfm?id=3055303
    let inline bind (prior: Distribution<'a>) (likelihood: 'a -> Distribution<'b>) : Distribution<'b> = 
              [for x in prior do
               for y in likelihood (outcome x) do 
               yield (outcome y, (probability y) * (probability x))] 

    ///mapD :: (a -> b) -> Dist a -> Dist b
    let inline map (f: 'a -> 'b) (distribution: Distribution<'a>) : Distribution<'b> = 
             [for x in distribution do
               for y in f (outcome x) do 
               yield (y, probability x)]    

    let inline apply (fDistribution: Distribution<'a -> 'b>) (distribution: Distribution<'a>) : Distribution<'b> = 
             [for x in distribution do
              for f in fDistribution do 
              for y in (fst f) (outcome x) do
              yield (y, probability x)]                   

    let inline filter (distribution: Distribution<'a>) predicate : Distribution<'a> =
       distribution 
       |> List.filter predicate

    let inline (?) (predicate: 'a -> bool) (distribution: Distribution<'a>) : Probability =
       distribution 
       |> List.filter (fun x -> predicate (fst x))
       |> List.sumBy snd

    let inline argMax (distribution: Distribution<'a>) =
        distribution |> List.maxBy (fun x -> probability x)
        // can be rewriten using eta reduction as => distribution |> Seq.maxBy snd |> fst


    let inline (>>=) a b = bind a b

    type DistributionMonadBuilder() =
        member this.Bind (r, f) = bind r f
        member this.Return x = return' x
        member this.ReturnFrom m = m

    let probabilistic = DistributionMonadBuilder()

    let selectOne collection = uniform [ for v in collection do yield (v, List.filter (fun x -> x <> v) collection)]

    let rec selectMany n collection = 
        match n with
        | 0 -> return' ([], collection)
        | _ -> probabilistic {
                    let! (c1, x) = selectOne collection
                    let! (c2, xs) = selectMany (n - 1) x
                    return ((c1::c2), xs) }

    let select n collection = selectMany n (map (collection |> List.rev) collection) 

    type Die = int

    let die = uniform [1..6]

    let rec dice numberOfThrows : Distribution<Die list>= 
        match numberOfThrows with
        | 0 -> certainly []
        | _ -> join (fun x y -> x :: y) (die) (dice (numberOfThrows - 1)) 
    
    // What is the probability of throwing 2 sixes with 2 dice
    let twoSixes = (fun x -> x = [6;6]) ? (dice 2)

    let gt3 = (fun x -> x > 3) ? (die);;